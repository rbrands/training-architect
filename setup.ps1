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

.EXAMPLE
    .\setup.ps1 -All
    .\setup.ps1 -KeyVault    # Key Vault secrets only
    .\setup.ps1 -Secrets     # dotnet user-secrets only
    .\setup.ps1 -GitHub      # GitHub Secrets only
    .\setup.ps1 -Bicep       # Generate bicepparam only

.NOTES
    Requires: GitHub CLI (gh) for -GitHub flag.
    Install:  winget install GitHub.cli
#>
param(
    [switch]$Secrets,
    [switch]$GitHub,
    [switch]$Bicep,
    [switch]$KeyVault,
    [switch]$All
)

# ---------------------------------------------------------------------------
# Load config
# ---------------------------------------------------------------------------
$configPath = Join-Path $PSScriptRoot "config.ps1"
if (-not (Test-Path $configPath)) {
    Write-Host "config.ps1 not found." -ForegroundColor Red
    Write-Host "Copy config.example.ps1 to config.ps1 and fill in your values." -ForegroundColor Yellow
    exit 1
}
. $configPath

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
    $project = "src/TrainingArchitect/TrainingArchitect.csproj"

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
    dotnet user-secrets set "FoundryProjectEndpoint"                                   $config.FoundryProjectEndpoint -p $project
    dotnet user-secrets set "FoundryProjectAgentName"                                  $config.FoundryProjectAgentName -p $project

    $appInsightsConnectionString = Get-AppInsightsConnectionString `
        -SubscriptionId $config.SubscriptionId `
        -ResourceGroupName $config.CentralResourceGroupName `
        -AppInsightsName $config.AppInsightsName

    dotnet user-secrets set "APPLICATIONINSIGHTS_CONNECTION_STRING"                    $appInsightsConnectionString -p $project

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
