# Copy this file to config.ps1 and fill in your values.
# NEVER commit config.ps1 to source control.

$config = @{
    # Azure
    SubscriptionId            = "__SUBSCRIPTION_ID__"
    CentralResourceGroupName  = "__CENTRAL_RESOURCE_GROUP__"  # shared: Plan, Cosmos, KV, Storage, AppInsights
    AppResourceGroupName      = "__APP_RESOURCE_GROUP__"      # this app: Web App, custom domain

    # App Service
    AppName              = "__APP_NAME__"
    PlanName             = "__PLAN_NAME__"

    # Cosmos DB
    # Recommended shared DB setup:
    # - one generic DB name for shared throughput (for example: shared-content-db)
    # - one container per app/workload (for example: training-architect, app2-content)
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

    # Foundry
    FoundryAccountName = "__FOUNDRY_ACCOUNT_NAME__"

    # Syncfusion
    # Used for: dotnet user-secrets (local)
    #            Key Vault secret (production)
    # NOT used for: GitHub Secrets or Bicep parameters
    SyncfusionLicenseKey = "__SYNCFUSION_LICENSE_KEY__"

    # Site
    # Used for: canonical URLs and Open Graph meta tags.
    # Custom domains and SSL bindings are managed manually in App Service.
    SiteUrl              = "https://training-architect.azurewebsites.net"
    Author               = "__AUTHOR__"

    # MCP
    # Public endpoint for the athlete-data MCP server.
    McpAthleteDataEndpoint = "__MCP_ATHLETE_DATA_ENDPOINT__"

    # Foundry Agent
    # Endpoint + agent name used for coaching agent invocation.
    FoundryProjectEndpoint = "__FOUNDRY_PROJECT_ENDPOINT__"
    FoundryProjectAgentName = "__FOUNDRY_PROJECT_AGENT_NAME__"

    # GitHub Actions OIDC
    AzureClientId        = "__AZURE_CLIENT_ID__"
}
