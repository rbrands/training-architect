# Copy this file to config.ps1 and fill in your values.
# NEVER commit config.ps1 to source control.

$config = @{
    # Azure
    SubscriptionId       = "__SUBSCRIPTION_ID__"
    ResourceGroup        = "__RESOURCE_GROUP__"

    # App Service
    AppName              = "__APP_NAME__"
    PlanName             = "__PLAN_NAME__"

    # Cosmos DB
    CosmosAccountName    = "__COSMOS_ACCOUNT_NAME__"
    CosmosDatabaseId     = "__COSMOS_DATABASE_ID__"
    CosmosContainerName  = "__COSMOS_CONTAINER_NAME__"
    CosmosEndpointUri    = "__COSMOS_ENDPOINT_URI__"

    # Key Vault
    KeyVaultName         = "__KEY_VAULT_NAME__"
    CertName             = "__CERT_NAME__"

    # Entra ID
    TenantId             = "__TENANT_ID__"
    ClientId             = "__CLIENT_ID__"

    # Storage
    StorageAccountName   = "__STORAGE_ACCOUNT_NAME__"
    StorageBlobEndpoint  = "__STORAGE_BLOB_ENDPOINT__"

    # Application Insights
    AppInsightsName    = "__APP_INSIGHTS_NAME__"
    LogAnalyticsName   = "__LOG_ANALYTICS_NAME__"

    # Syncfusion
    # Used for: dotnet user-secrets (local)
    #            Key Vault secret (production)
    # NOT used for: GitHub Secrets or Bicep parameters
    SyncfusionLicenseKey = "__SYNCFUSION_LICENSE_KEY__"

    # Site
    # Used for: canonical URLs, Open Graph meta tags, and custom domain deployment.
    # If this is a custom domain (not *.azurewebsites.net), the Bicep template
    # automatically deploys the hostname binding and a free managed SSL certificate
    # for the www subdomain. DNS records must exist at the registrar first.
    # See README.md → Custom Domain for required DNS records.
    SiteUrl              = "https://brands-advisory.azurewebsites.net"

    # GitHub Actions OIDC
    AzureClientId        = "__AZURE_CLIENT_ID__"
}
