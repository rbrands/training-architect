<#
.SYNOPSIS
    Sets up local development secrets, GitHub Secrets, Key Vault secrets, and Bicep parameters
    from a single config.ps1 file.

.DESCRIPTION
    Reads config.ps1 (not committed) and applies values to:
      -Secrets  dotnet user-secrets for local development
      -GitHub   GitHub repository secrets via gh CLI
      -Bicep    Generates infra/main.local.bicepparam
      -KeyVault Sets Key Vault secrets via Azure CLI
      -All      All four targets

        Alternatively, select or inspect a Foundry agent independently:
            -AgentTarget Local|StagingSlot -Agent Production|Staging
            -AgentTarget Local|StagingSlot -ShowAgent
        Agent commands cannot be combined with the setup flags. Production is an
        agent profile, never a deployment target. -WhatIf performs reads only.

.EXAMPLE
    .\setup.ps1 -All
    .\setup.ps1 -KeyVault    # Key Vault secrets only
    .\setup.ps1 -Secrets     # dotnet user-secrets only
    .\setup.ps1 -GitHub      # GitHub Secrets only
    .\setup.ps1 -Bicep       # Generate bicepparam only

.EXAMPLE
    .\setup.ps1 -AgentTarget Local -Agent Staging
    .\setup.ps1 -AgentTarget StagingSlot -Agent Staging -WhatIf
    .\setup.ps1 -AgentTarget StagingSlot -Agent Staging
    .\setup.ps1 -AgentTarget StagingSlot -ShowAgent
    .\setup.ps1 -AgentTarget StagingSlot -Agent Production

.NOTES
    Agent commands require PowerShell 7 and config.ps1. Local requires dotnet.
    StagingSlot requires an authenticated Azure CLI session with read access to
    production app settings and write access to staging settings and the app's
    slot configuration names. It uses an explicit subscription, without login
    or changing the global Azure account context. Updates may restart staging.
    Do not run concurrently with deployments or swaps. The selected agent stays
    with its slot during swaps; infrastructure deployments may reset the value.
    Requires: GitHub CLI (gh) for -GitHub flag.
    Install:  winget install GitHub.cli
#>
[CmdletBinding(SupportsShouldProcess, DefaultParameterSetName = 'Setup')]
param(
    [Parameter(ParameterSetName = 'Setup')]
    [switch]$Secrets,
    [Parameter(ParameterSetName = 'Setup')]
    [switch]$GitHub,
    [Parameter(ParameterSetName = 'Setup')]
    [switch]$Bicep,
    [Parameter(ParameterSetName = 'Setup')]
    [switch]$KeyVault,
    [Parameter(ParameterSetName = 'Setup')]
    [switch]$All,
    [Parameter(ParameterSetName = 'SelectAgent')]
    [Parameter(ParameterSetName = 'ShowAgent')]
    [ValidateSet('Local', 'StagingSlot')]
    [string]$AgentTarget,
    [Parameter(ParameterSetName = 'SelectAgent')]
    [ValidateSet('Production', 'Staging')]
    [string]$Agent,
    [Parameter(ParameterSetName = 'ShowAgent')]
    [switch]$ShowAgent
)

if ($PSCmdlet.ParameterSetName -ne 'Setup' -and
    (-not $AgentTarget -or (-not $Agent -and -not $ShowAgent))) {
    throw 'Specify -AgentTarget Local|StagingSlot and either -Agent Production|Staging or -ShowAgent.'
}
if ($PSCmdlet.ParameterSetName -eq 'Setup' -and
    -not $PSCmdlet.ShouldProcess('Setup targets selected by flags', 'Apply configuration')) {
    return
}

# ---------------------------------------------------------------------------
# Load config
# ---------------------------------------------------------------------------
$configPath = Join-Path $PSScriptRoot "config.ps1"
if (-not (Test-Path $configPath)) {
    throw 'config.ps1 not found. Copy config.example.ps1 to config.ps1 and fill in your values.'
}
. $configPath

$project = Join-Path $PSScriptRoot 'src/TrainingArchitect/TrainingArchitect.csproj'

function Invoke-AgentCommand {
    param([string]$Command, [string[]]$Arguments)

    $commandOutput = & $Command @Arguments 2>&1
    if ($LASTEXITCODE -ne 0) {
        throw "$Command failed (exit code $LASTEXITCODE). Check installation, authentication and target access. Command output is suppressed to protect secrets."
    }
    return ($commandOutput -join "`n")
}

function Get-AgentConfigValue {
    param([string]$Key)

    $value = [string]$config[$Key]
    if ([string]::IsNullOrWhiteSpace($value) -or $value.Contains('__')) {
        throw "Set $Key in config.ps1 before selecting this agent or target."
    }
    return $value.Trim()
}

function Get-AgentReference {
    param([string]$Profile)

    $key = if ($Profile -eq 'Staging') { 'FoundryProjectStagingAgentName' } else { 'FoundryProjectAgentName' }
    $reference = Get-AgentConfigValue $key
    if ($reference -notmatch '^[A-Za-z0-9][A-Za-z0-9_-]*(?:@[A-Za-z0-9][A-Za-z0-9._-]*)?$') {
        throw "$key must contain an agent name, optionally followed by @version."
    }
    return $reference
}

function Get-LocalAgent {
    $json = Invoke-AgentCommand dotnet @('user-secrets', 'list', '--json', '-p', $project)
    $json = ($json -split '\r?\n' | Where-Object { $_.Trim() -notin @('//BEGIN', '//END') }) -join "`n"
    try {
        $values = ConvertFrom-Json -InputObject $json -AsHashtable -ErrorAction Stop
        return [string]$values['FoundryProjectAgentName']
    }
    catch {
        throw 'Could not parse user-secrets JSON. No agent setting was changed.'
    }
}

function Set-LocalAgent {
    param([string]$Reference)

    Invoke-AgentCommand dotnet @('user-secrets', 'set', 'FoundryProjectAgentName', $Reference, '-p', $project) | Out-Null
    if ((Get-LocalAgent) -cne $Reference) {
        throw 'Local agent readback did not match the requested reference. Check -ShowAgent before retrying.'
    }
}

if ($AgentTarget -eq 'Local') {
    $currentAgent = Get-LocalAgent
    Write-Host "Local user-secret FoundryProjectAgentName: $(if ($currentAgent) { $currentAgent } else { '(not set)' })"
    if ($env:FoundryProjectAgentName) {
        Write-Warning 'The current process has a FoundryProjectAgentName environment override; it takes precedence over user-secrets.'
    }
    if ($ShowAgent) { return }
    $selectedAgent = Get-AgentReference $Agent
    if ($PSCmdlet.ShouldProcess('Local user-secret FoundryProjectAgentName', "Select $selectedAgent")) {
        Set-LocalAgent $selectedAgent
        Write-Host "Local user-secret FoundryProjectAgentName: $selectedAgent"
        Write-Host 'Restart the local host. Environment, launch-profile or command-line settings can override user-secrets.'
    }
    return
}

function Get-AppServiceAgent {
    param([string[]]$TargetArguments, [switch]$StagingSlot)

    $arguments = @('webapp', 'config', 'appsettings', 'list') + $TargetArguments
    if ($StagingSlot) { $arguments += @('--slot', 'staging') }
    $arguments += @('--query', "{agent: [?name=='FoundryProjectAgentName'] | [0].value, sticky: [?slotSetting].name}", '--output', 'json', '--only-show-errors')
    $json = Invoke-AgentCommand az $arguments
    try {
        $settings = ConvertFrom-Json -InputObject $json -AsHashtable -ErrorAction Stop
        if ($settings -isnot [System.Collections.IDictionary] -or -not $settings.Contains('sticky')) {
            throw 'Unexpected settings response.'
        }
        return $settings
    }
    catch {
        throw 'Could not parse App Service agent settings. Check -ShowAgent before retrying.'
    }
}

if ($AgentTarget -eq 'StagingSlot') {
    $subscriptionId = Get-AgentConfigValue 'SubscriptionId'
    $resourceGroup = Get-AgentConfigValue 'AppResourceGroupName'
    $appName = Get-AgentConfigValue 'AppName'
    $targetArguments = @('--subscription', $subscriptionId, '--resource-group', $resourceGroup, '--name', $appName)
    $slotId = "/subscriptions/$subscriptionId/resourceGroups/$resourceGroup/providers/Microsoft.Web/sites/$appName/slots/staging"
    $actualSlotId = Invoke-AgentCommand az @('resource', 'show', '--ids', $slotId, '--subscription', $subscriptionId, '--query', 'id', '--output', 'tsv', '--only-show-errors')
    if ($actualSlotId.Trim() -ine $slotId) {
        throw 'The configured staging slot could not be verified. No settings were changed.'
    }

    $current = Get-AppServiceAgent $targetArguments -StagingSlot
    Write-Host "Target: $slotId"
    Write-Host "Staging FoundryProjectAgentName: $(if ($current.agent) { $current.agent } else { '(not set)' })"
    Write-Host "Agent slot setting (swap protection): $($current.sticky -ccontains 'FoundryProjectAgentName')"
    if ($ShowAgent) { return }

    $selectedAgent = Get-AgentReference $Agent
    $production = Get-AppServiceAgent $targetArguments
    if ($Agent -eq 'Staging' -and $production.agent -cne (Get-AgentReference 'Production')) {
        throw 'Production agent does not match FoundryProjectAgentName in config.ps1. Resolve the mismatch before selecting Staging. Production was not changed.'
    }

    if ($PSCmdlet.ShouldProcess($slotId, "Select $selectedAgent and mark FoundryProjectAgentName as a slot setting")) {
        Write-Warning 'Updating app settings can restart the staging slot. Do not run concurrently with deployments or slot swaps.'
        Invoke-AgentCommand az (@('webapp', 'config', 'appsettings', 'set') + $targetArguments + @(
            '--slot', 'staging', '--slot-settings', "FoundryProjectAgentName=$selectedAgent", '--output', 'none', '--only-show-errors'
        )) | Out-Null

        $updated = Get-AppServiceAgent $targetArguments -StagingSlot
        if ($updated.agent -cne $selectedAgent -or $updated.sticky -cnotcontains 'FoundryProjectAgentName') {
            throw 'Staging agent or swap protection readback did not match. Check -ShowAgent before retrying or swapping slots.'
        }
        foreach ($settingName in $current.sticky) {
            if ($updated.sticky -cnotcontains $settingName) {
                throw 'An existing sticky app setting was not preserved. Review slot settings before swapping slots.'
            }
        }
        $productionAfter = Get-AppServiceAgent $targetArguments
        if ($productionAfter.agent -cne $production.agent) {
            throw 'Production agent changed during the operation. Review concurrent deployments or swaps; this command only writes to staging.'
        }
        Write-Host "Staging FoundryProjectAgentName: $selectedAgent (slot setting: True)"
        Write-Host 'Production agent unchanged. This temporary selection survives swaps but may be reset by an infrastructure deployment.'
    }
    return
}

function Set-AzureSubscriptionContext {
    param(
        [Parameter(Mandatory = $true)]
        [string]$SubscriptionId
    )

    if ([string]::IsNullOrWhiteSpace($SubscriptionId) -or $SubscriptionId.StartsWith('__')) {
        throw 'SubscriptionId is required before Azure CLI-based setup can run.'
    }

    az account set --subscription $SubscriptionId | Out-Null
    if ($LASTEXITCODE -ne 0) {
        throw "Failed to set the Azure CLI subscription context to '$SubscriptionId'. Make sure you are logged in with az login."
    }
}

function Get-AppInsightsConnectionString {
    param(
        [Parameter(Mandatory = $true)]
        [string]$SubscriptionId,

        [Parameter(Mandatory = $true)]
        [string]$ResourceGroupName,

        [Parameter(Mandatory = $true)]
        [string]$AppInsightsName
    )

    if ([string]::IsNullOrWhiteSpace($SubscriptionId) -or $SubscriptionId.StartsWith('__') -or
        [string]::IsNullOrWhiteSpace($ResourceGroupName) -or $ResourceGroupName.StartsWith('__') -or
        [string]::IsNullOrWhiteSpace($AppInsightsName) -or $AppInsightsName.StartsWith('__')) {
        Write-Host "Skipping Application Insights connection string resolution (config placeholders still present)." -ForegroundColor Gray
        return $null
    }

    Set-AzureSubscriptionContext -SubscriptionId $SubscriptionId

    $connectionString = az resource show `
        --resource-group $ResourceGroupName `
        --name $AppInsightsName `
        --resource-type Microsoft.Insights/components `
        --api-version 2020-02-02 `
        --query properties.ConnectionString `
        -o tsv

    if ($LASTEXITCODE -ne 0 -or [string]::IsNullOrWhiteSpace($connectionString)) {
        throw "Failed to resolve the Application Insights connection string for '$AppInsightsName' in resource group '$ResourceGroupName'."
    }

    return $connectionString.Trim()
}

# ---------------------------------------------------------------------------
# 1. dotnet user-secrets
# ---------------------------------------------------------------------------
if ($Secrets -or $All) {
    Write-Host "Setting dotnet user-secrets..." -ForegroundColor Yellow
    $localAgent = Get-LocalAgent
    if ([string]::IsNullOrWhiteSpace($localAgent) -or $localAgent.Contains('__')) {
        Set-LocalAgent (Get-AgentReference 'Production')
    }
    else {
        Write-Host "Preserving local agent: $localAgent. Use -AgentTarget Local -Agent Production|Staging to switch."
    }

    dotnet user-secrets set "AzureAd:TenantId"                                        $config.TenantId             -p $project
    dotnet user-secrets set "AzureAd:ClientId"                                        $config.ClientId             -p $project
    dotnet user-secrets set "AzureAd:ClientCertificates:0:KeyVaultUrl"                "https://$($config.KeyVaultName).vault.azure.net" -p $project
    dotnet user-secrets set "AzureAd:ClientCertificates:0:KeyVaultCertificateName"    $config.CertName             -p $project
    dotnet user-secrets set "CosmosDb:EndpointUri"                                    $config.CosmosEndpointUri    -p $project
    dotnet user-secrets set "CosmosDb:DatabaseId"                                     $config.CosmosDatabaseId     -p $project
    dotnet user-secrets set "CosmosDb:ContainerName"                                  $config.CosmosContainerName  -p $project
    dotnet user-secrets set "Storage:BlobEndpoint"                                    $config.StorageBlobEndpoint  -p $project
    dotnet user-secrets set "KeyVault:Url"                                            "https://$($config.KeyVaultName).vault.azure.net" -p $project
    dotnet user-secrets set "Syncfusion:LicenseKey"                                   $config.SyncfusionLicenseKey -p $project
    dotnet user-secrets set "SiteUrl"                                                  $config.SiteUrl              -p $project
    dotnet user-secrets set "Author"                                                   $config.Author               -p $project
    dotnet user-secrets set "Mcp:AthleteData:Endpoint"                                $config.McpAthleteDataEndpoint -p $project
    dotnet user-secrets set "Mcp:AthleteData:TimeoutSeconds"                          $config.McpAthleteDataTimeoutSeconds -p $project
    dotnet user-secrets set "Mcp:AthleteData:ConnectionTimeoutSeconds"                $config.McpAthleteDataConnectionTimeoutSeconds -p $project
    dotnet user-secrets set "Mcp:AthleteData:MaxRetryAttempts"                        $config.McpAthleteDataMaxRetryAttempts -p $project
    dotnet user-secrets set "FoundryProjectEndpoint"                                   $config.FoundryProjectEndpoint -p $project

    $appInsightsConnectionString = Get-AppInsightsConnectionString `
        -SubscriptionId $config.SubscriptionId `
        -ResourceGroupName $config.CentralResourceGroupName `
        -AppInsightsName $config.AppInsightsName

    if (-not [string]::IsNullOrWhiteSpace($appInsightsConnectionString)) {
        dotnet user-secrets set "APPLICATIONINSIGHTS_CONNECTION_STRING"                    $appInsightsConnectionString -p $project
    }
    Write-Host "dotnet user-secrets set." -ForegroundColor Green
}

# ---------------------------------------------------------------------------
# 2. GitHub Secrets via gh CLI
# ---------------------------------------------------------------------------
if ($GitHub -or $All) {
    Write-Host "Setting GitHub Secrets..." -ForegroundColor Yellow

    gh secret set AZURE_CLIENT_ID          --body $config.AzureClientId
    gh secret set AZURE_SUBSCRIPTION_ID   --body $config.SubscriptionId
    gh secret set CENTRAL_RESOURCE_GROUP  --body $config.CentralResourceGroupName
    gh secret set APP_RESOURCE_GROUP      --body $config.AppResourceGroupName
    gh secret set AZURE_RESOURCE_GROUP    --body $config.AppResourceGroupName
    gh secret set APP_NAME                --body $config.AppName
    gh secret set AZURE_WEBAPP_NAME       --body $config.AppName
    gh secret set PLAN_NAME              --body $config.PlanName
    gh secret set COSMOS_ACCOUNT_NAME    --body $config.CosmosAccountName
    gh secret set COSMOS_DATABASE_ID     --body $config.CosmosDatabaseId
    gh secret set COSMOS_CONTAINER_NAME  --body $config.CosmosContainerName
    gh secret set KEY_VAULT_NAME         --body $config.KeyVaultName
    gh secret set CERT_NAME              --body $config.CertName
    gh secret set CLIENT_ID              --body $config.ClientId
    gh secret set TENANT_ID              --body $config.TenantId
    gh secret set STORAGE_ACCOUNT_NAME   --body $config.StorageAccountName
    gh secret set APP_INSIGHTS_NAME      --body $config.AppInsightsName
    gh secret set FOUNDRY_ACCOUNT_NAME   --body $config.FoundryAccountName
    gh secret set SITE_URL               --body $config.SiteUrl
    gh secret set AUTHOR                 --body $config.Author
    gh secret set MCP_ATHLETE_DATA_ENDPOINT --body $config.McpAthleteDataEndpoint
    gh secret set MCP_ATHLETE_DATA_TIMEOUT_SECONDS --body $config.McpAthleteDataTimeoutSeconds
    gh secret set MCP_ATHLETE_DATA_CONNECTION_TIMEOUT_SECONDS --body $config.McpAthleteDataConnectionTimeoutSeconds
    gh secret set MCP_ATHLETE_DATA_MAX_RETRY_ATTEMPTS --body $config.McpAthleteDataMaxRetryAttempts
    gh secret set FOUNDRY_PROJECT_ENDPOINT --body $config.FoundryProjectEndpoint
    gh secret set FOUNDRY_PROJECT_AGENT_NAME --body $config.FoundryProjectAgentName

    Write-Host "GitHub Secrets set." -ForegroundColor Green
}

# ---------------------------------------------------------------------------
# 3. Generate infra/main.local.bicepparam
# ---------------------------------------------------------------------------
if ($Bicep -or $All) {
    Write-Host "Generating infra/main.local.bicepparam..." -ForegroundColor Yellow

    $content = @"
// ---------------------------------------------------------------------------
// main.local.bicepparam — generated by setup.ps1, do not commit
// ---------------------------------------------------------------------------
using './main.bicep'

param centralResourceGroupName = '$($config.CentralResourceGroupName)'
param appResourceGroupName     = '$($config.AppResourceGroupName)'
param appName               = '$($config.AppName)'
param planName              = '$($config.PlanName)'
param cosmosAccountName     = '$($config.CosmosAccountName)'
param cosmosDatabaseId      = '$($config.CosmosDatabaseId)'
param cosmosContainerName   = '$($config.CosmosContainerName)'
param keyVaultName          = '$($config.KeyVaultName)'
param keyVaultCertificateName = '$($config.CertName)'
param tenantId              = '$($config.TenantId)'
param clientId              = '$($config.ClientId)'
param storageAccountName    = '$($config.StorageAccountName)'
param appInsightsName       = '$($config.AppInsightsName)'
param foundryAccountName    = '$($config.FoundryAccountName)'
param siteUrl               = '$($config.SiteUrl)'
param author                = '$($config.Author)'
param mcpAthleteDataEndpoint = '$($config.McpAthleteDataEndpoint)'
param mcpAthleteDataTimeoutSeconds = '$($config.McpAthleteDataTimeoutSeconds)'
param mcpAthleteDataConnectionTimeoutSeconds = '$($config.McpAthleteDataConnectionTimeoutSeconds)'
param mcpAthleteDataMaxRetryAttempts = '$($config.McpAthleteDataMaxRetryAttempts)'
param foundryProjectEndpoint = '$($config.FoundryProjectEndpoint)'
param foundryProjectAgentName = '$($config.FoundryProjectAgentName)'
"@

    $outputPath = Join-Path $PSScriptRoot "infra/main.local.bicepparam"
    Set-Content -Path $outputPath -Value $content -Encoding UTF8

    Write-Host "infra/main.local.bicepparam generated." -ForegroundColor Green
}

# ---------------------------------------------------------------------------
# 4. Key Vault Secrets via Azure CLI
# Requires: Key Vault Secrets Officer role on the Key Vault
# Assign with: Set-KeyVaultRoleAssignment.ps1 from cloud-admin-toolkit
# ---------------------------------------------------------------------------
if ($KeyVault -or $All) {
    Write-Host "Setting Key Vault secrets..." -ForegroundColor Yellow
    Write-Host "Note: Requires 'Key Vault Secrets Officer' role." `
        -ForegroundColor Gray
    Write-Host "      Use Set-KeyVaultRoleAssignment.ps1 from " `
        -ForegroundColor Gray
    Write-Host "      cloud-admin-toolkit to assign the role." `
        -ForegroundColor Gray

    Set-AzureSubscriptionContext -SubscriptionId $config.SubscriptionId

    az keyvault secret set `
        --vault-name $config.KeyVaultName `
        --name "Syncfusion--LicenseKey" `
        --value $config.SyncfusionLicenseKey `
        --output none

    Write-Host "Key Vault secrets set." -ForegroundColor Green
    Write-Host "  Syncfusion--LicenseKey → Syncfusion:LicenseKey" `
        -ForegroundColor Gray
}

Write-Host "Done." -ForegroundColor Green
