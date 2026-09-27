$ErrorActionPreference = 'Stop'
$repoRoot = Split-Path (Split-Path $PSScriptRoot -Parent) -Parent
$fixture = Join-Path ([System.IO.Path]::GetTempPath()) ([guid]::NewGuid().ToString())
$originalExitCode = $global:LASTEXITCODE
$passed = 0

function Assert-True {
    param([bool]$Condition, [string]$Message)
    if (-not $Condition) { throw $Message }
}

function Invoke-Setup {
    param([hashtable]$Parameters, [string]$ExpectedError)
    $failure = $null
    try { & (Join-Path $fixture 'setup.ps1') @Parameters *>&1 | Out-Null }
    catch { $failure = $_.Exception.Message }
    if ($ExpectedError) {
        Assert-True ($null -ne $failure -and $failure -like $ExpectedError) "Expected '$ExpectedError', got '$failure'."
    }
    elseif ($failure) { throw $failure }
}

function Reset-Fixture {
    $script:state = @{
        Local = 'test-production@1'; Production = 'test-production@1'; Staging = 'test-production@1'
        Sticky = @('SiteUrl', 'Author'); Writes = 0; Calls = [System.Collections.Generic.List[object]]::new()
        Fail = $false; InvalidJson = $false; WrongSlot = $false; WrongReadback = $false
        DropSticky = $false; MissingAgentSticky = $false; ProductionChanged = $false
    }
    $script:config = @{
        SubscriptionId = 'test-subscription'; AppResourceGroupName = 'test-group'; AppName = 'test-app'
        CentralResourceGroupName = '__CENTRAL_RESOURCE_GROUP__'; AppInsightsName = '__APP_INSIGHTS_NAME__'
        FoundryProjectAgentName = 'test-production@1'; FoundryProjectStagingAgentName = 'test-staging@2'
    }
    Save-Config
}

function Save-Config {
    $config | Export-Clixml (Join-Path $fixture 'config.xml')
}

function az {
    $arguments = @($args)
    $state.Calls.Add(@{ Command = 'az'; Arguments = $arguments })
    $global:LASTEXITCODE = 0
    Assert-True ($arguments -contains '--subscription') 'Missing explicit subscription.'
    Assert-True ($arguments[([array]::IndexOf($arguments, '--subscription') + 1)] -eq 'test-subscription') 'Wrong subscription.'
    if ($state.Fail) { $global:LASTEXITCODE = 1; return 'SENSITIVE_CLI_OUTPUT' }
    if ($arguments[0] -eq 'resource' -and $arguments[1] -eq 'show') {
        $expected = '/subscriptions/test-subscription/resourceGroups/test-group/providers/Microsoft.Web/sites/test-app/slots/staging'
        Assert-True ($arguments[([array]::IndexOf($arguments, '--ids') + 1)] -eq $expected) 'Wrong resource ID.'
        if ($state.WrongSlot) { return 'wrong-slot' }
        return $expected
    }
    Assert-True (($arguments[0..2] -join ' ') -eq 'webapp config appsettings') 'Unexpected Azure operation.'
    Assert-True ($arguments[([array]::IndexOf($arguments, '--resource-group') + 1)] -eq 'test-group') 'Wrong resource group.'
    Assert-True ($arguments[([array]::IndexOf($arguments, '--name') + 1)] -eq 'test-app') 'Wrong app.'
    $isStaging = $arguments -contains '--slot'
    if ($isStaging) {
        Assert-True ($arguments[([array]::IndexOf($arguments, '--slot') + 1)] -ceq 'staging') 'Wrong slot.'
    }
    if ($arguments[3] -eq 'list') {
        if ($state.InvalidJson) { return 'invalid-json' }
        Assert-True ($arguments -contains '--query') 'Settings output must be filtered.'
        return (@{ agent = $(if ($isStaging) { $state.Staging } else { $state.Production }); sticky = $state.Sticky } | ConvertTo-Json)
    }
    Assert-True ($arguments[3] -eq 'set' -and $isStaging) 'Only staging settings may be written.'
    Assert-True ($arguments -contains '--slot-settings' -and $arguments -notcontains '--settings') 'Write must mark the setting sticky.'
    $settingIndex = [array]::IndexOf($arguments, '--slot-settings') + 1
    $setting = $arguments[$settingIndex]
    Assert-True ($setting -clike 'FoundryProjectAgentName=*') 'Unexpected setting written.'
    Assert-True ($arguments[$settingIndex + 1] -eq '--output') 'Unexpected additional setting.'
    $state.Writes++
    if (-not $state.WrongReadback) { $state.Staging = $setting.Split('=', 2)[1] }
    if ($state.DropSticky) { $state.Sticky = @() }
    if (-not $state.MissingAgentSticky) { $state.Sticky += 'FoundryProjectAgentName' }
    if ($state.ProductionChanged) { $state.Production = 'concurrent-change' }
}

function dotnet {
    $arguments = @($args)
    $state.Calls.Add(@{ Command = 'dotnet'; Arguments = $arguments })
    $global:LASTEXITCODE = 0
    Assert-True ($arguments[0] -eq 'user-secrets') 'Unexpected dotnet operation.'
    Assert-True ($arguments -contains (Join-Path $fixture 'src/TrainingArchitect/TrainingArchitect.csproj')) 'Project must be rooted at the script directory.'
    if ($state.Fail) { $global:LASTEXITCODE = 1; return 'SENSITIVE_CLI_OUTPUT' }
    if ($arguments[1] -eq 'list') {
        if ($state.InvalidJson) { return 'invalid-json' }
        return "//BEGIN`n$(@{ FoundryProjectAgentName = $state.Local } | ConvertTo-Json)`n//END"
    }
    Assert-True ($arguments[1] -eq 'set') 'Unexpected secrets operation.'
    if ($arguments[2] -eq 'FoundryProjectAgentName') {
        $state.Writes++
        if (-not $state.WrongReadback) { $state.Local = $arguments[3] }
    }
}

function gh { throw 'GitHub must not be invoked by agent commands.' }

try {
    New-Item $fixture -ItemType Directory | Out-Null
    Copy-Item (Join-Path $repoRoot 'setup.ps1') (Join-Path $fixture 'setup.ps1')
    Set-Content (Join-Path $fixture 'config.ps1') '$config = Import-Clixml (Join-Path $PSScriptRoot "config.xml")'

    foreach ($target in @('Local', 'StagingSlot')) {
        foreach ($profile in @('Production', 'Staging')) {
            Reset-Fixture
            if ($profile -eq 'Production') { $state.Local = $state.Staging = 'test-staging@2' }
            Invoke-Setup @{ AgentTarget = $target; Agent = $profile }
            $expected = if ($profile -eq 'Production') { 'test-production@1' } else { 'test-staging@2' }
            $actual = if ($target -eq 'Local') { $state.Local } else { $state.Staging }
            Assert-True ($actual -ceq $expected -and $state.Writes -eq 1) 'Selection failed.'
            $expectedCommand = if ($target -eq 'Local') { 'dotnet' } else { 'az' }
            Assert-True (@($state.Calls | Where-Object Command -ne $expectedCommand).Count -eq 0) 'Target isolation failed.'
            Assert-True ($state.Production -ceq 'test-production@1') 'Production changed.'
            if ($target -eq 'StagingSlot') {
                Assert-True ($state.Sticky -contains 'SiteUrl' -and $state.Sticky -contains 'Author' -and $state.Sticky -contains 'FoundryProjectAgentName') 'Sticky settings lost.'
            }
            $passed++
        }
        foreach ($readOnly in @(@{ ShowAgent = $true }, @{ Agent = 'Staging'; WhatIf = $true })) {
            Reset-Fixture
            $readOnly.AgentTarget = $target
            Invoke-Setup $readOnly
            Assert-True ($state.Writes -eq 0) 'Read-only command performed a write.'
            $passed++
        }
        foreach ($fault in @('Fail', 'InvalidJson', 'WrongReadback')) {
            Reset-Fixture
            $state[$fault] = $true
            $expectedError = switch ($fault) {
                'Fail' { '*output is suppressed*' }
                'InvalidJson' { '*Could not parse*' }
                'WrongReadback' { '*readback did not match*' }
            }
            Invoke-Setup @{ AgentTarget = $target; Agent = 'Staging' } $expectedError
            $passed++
        }
        foreach ($invalid in @('', '__FOUNDRY_PROJECT_STAGING_AGENT_NAME__', 'agent with spaces', 'agent@')) {
            Reset-Fixture
            $config.FoundryProjectStagingAgentName = $invalid
            Save-Config
            Invoke-Setup @{ AgentTarget = $target; Agent = 'Staging' } '*FoundryProjectStagingAgentName*'
            Assert-True ($state.Writes -eq 0) 'Invalid reference caused a write.'
            $passed++
        }
    }

    foreach ($fault in @('WrongSlot', 'DropSticky', 'MissingAgentSticky', 'ProductionChanged')) {
        Reset-Fixture
        $state[$fault] = $true
        $expectedError = switch ($fault) {
            'WrongSlot' { '*slot could not be verified*' }
            'DropSticky' { '*sticky app setting was not preserved*' }
            'MissingAgentSticky' { '*swap protection readback did not match*' }
            'ProductionChanged' { '*Production agent changed*' }
        }
        Invoke-Setup @{ AgentTarget = 'StagingSlot'; Agent = 'Staging' } $expectedError
        $passed++
    }

    Reset-Fixture
    $state.Production = 'unexpected-production'
    Invoke-Setup @{ AgentTarget = 'StagingSlot'; Agent = 'Staging' } '*Production agent does not match*'
    Assert-True ($state.Writes -eq 0) 'Production mismatch caused a write.'
    Invoke-Setup @{ AgentTarget = 'StagingSlot'; Agent = 'Production' }
    Assert-True ($state.Staging -ceq 'test-production@1' -and $state.Production -ceq 'unexpected-production') 'Returning staging to production profile failed.'
    $passed++

    foreach ($key in @('SubscriptionId', 'AppResourceGroupName', 'AppName')) {
        Reset-Fixture
        $config.Remove($key)
        Save-Config
        Invoke-Setup @{ AgentTarget = 'StagingSlot'; Agent = 'Staging' } "*Set $key*"
        Assert-True ($state.Calls.Count -eq 0) 'Missing target should fail before CLI calls.'
        $passed++
    }

    foreach ($parameters in @(
        @{ Agent = 'Staging' }, @{ AgentTarget = 'Local' },
        @{ AgentTarget = 'Production'; Agent = 'Staging' },
        @{ AgentTarget = 'Local'; Agent = 'Staging'; Secrets = $true },
        @{ AgentTarget = 'StagingSlot'; Agent = 'Staging'; All = $true },
        @{ AgentTarget = 'Local'; Agent = 'Staging'; ShowAgent = $true }
    )) {
        Reset-Fixture
        Invoke-Setup $parameters '*'
        Assert-True ($state.Calls.Count -eq 0) 'Invalid parameter combination reached a CLI.'
        $passed++
    }

    foreach ($initial in @('test-staging@2', '', '__PLACEHOLDER__')) {
        Reset-Fixture
        $state.Local = $initial
        Invoke-Setup @{ Secrets = $true }
        $expected = if ($initial -eq 'test-staging@2') { $initial } else { 'test-production@1' }
        Assert-True ($state.Local -ceq $expected) 'General secrets setup changed the local preference incorrectly.'
        $passed++
    }

    Reset-Fixture
    Remove-Item (Join-Path $fixture 'config.ps1')
    Invoke-Setup @{ AgentTarget = 'StagingSlot'; ShowAgent = $true } '*config.ps1 not found*'
    Assert-True ($state.Calls.Count -eq 0) 'Missing config reached a CLI.'
    $passed++
    Write-Host "PASS: $passed isolated agent-switch scenarios. No real CLI commands or private configuration used."
}
finally {
    Remove-Item $fixture -Recurse -Force -ErrorAction SilentlyContinue
    $global:LASTEXITCODE = $originalExitCode
}