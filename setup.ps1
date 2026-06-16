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

# ---------------------------------------------------------------------------
# 1. dotnet user-secrets
# ---------------------------------------------------------------------------
if ($Secrets -or $All) {
    Write-Host "Setting dotnet user-secrets..." -ForegroundColor Yellow
    $project = "src/BrandsAdvisory/BrandsAdvisory.csproj"

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

    Write-Host "dotnet user-secrets set." -ForegroundColor Green
}

# ---------------------------------------------------------------------------
# 2. GitHub Secrets via gh CLI
# ---------------------------------------------------------------------------
if ($GitHub -or $All) {
    Write-Host "Setting GitHub Secrets..." -ForegroundColor Yellow

    gh secret set AZURE_CLIENT_ID        --body $config.AzureClientId
    gh secret set AZURE_SUBSCRIPTION_ID  --body $config.SubscriptionId
    gh secret set AZURE_RESOURCE_GROUP   --body $config.ResourceGroup
    gh secret set AZURE_WEBAPP_NAME      --body $config.AppName
    gh secret set APP_NAME               --body $config.AppName
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
    gh secret set LOG_ANALYTICS_NAME     --body $config.LogAnalyticsName
    gh secret set SITE_URL               --body $config.SiteUrl

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
param logAnalyticsName      = '$($config.LogAnalyticsName)'
param siteUrl               = '$($config.SiteUrl)'
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
