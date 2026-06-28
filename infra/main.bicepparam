// ---------------------------------------------------------------------------
// main.bicepparam — Parameter values for main.bicep
//
// Copy this file, replace all __PLACEHOLDER__ values with real values,
// and keep the copy out of source control.
// ---------------------------------------------------------------------------

using './main.bicep'

// Resource Groups
param centralResourceGroupName = '__CENTRAL_RESOURCE_GROUP__'  // shared: Plan, Cosmos, KV, Storage, AppInsights
param appResourceGroupName = '__APP_RESOURCE_GROUP__'          // this app: Web App, custom domain

// App Service
param appName = '__APP_NAME__'
param planName = '__PLAN_NAME__'

// Cosmos DB
// Use a generic shared DB name to share RU/s across multiple app containers,
// for example: shared-content-db
param cosmosAccountName = '__COSMOS_ACCOUNT_NAME__'
param cosmosDatabaseId = '__DATABASE_ID__'
// Use app-scoped container names, for example: training-architect
param cosmosContainerName = '__CONTAINER_NAME__'

// Key Vault
param keyVaultName = '__KEY_VAULT_NAME__'
param keyVaultCertificateName = '__CERT_NAME__'

// Entra ID / App Registration
param tenantId = '__TENANT_ID__'
param clientId = '__CLIENT_ID__'

// Storage
param storageAccountName = '__STORAGE_ACCOUNT_NAME__'

// Application Insights
param appInsightsName = '__APP_INSIGHTS_NAME__'

// Site
// Set to your custom domain (e.g. https://www.example.com) or the azurewebsites.net default URL.
// If this is a custom domain, the Bicep template automatically deploys hostname bindings + managed SSL.
param siteUrl = '__SITE_URL__'

// Optional: reuse an existing App Service managed certificate for www SSL binding
// (for example: www.example.com-myapp). Leave empty to create a new managed certificate.
param existingManagedCertificateName = '__EXISTING_MANAGED_CERTIFICATE_NAME__'

// MCP
// Public MCP endpoint used by the app for athlete-data tool calls.
param mcpAthleteDataEndpoint = '__MCP_ATHLETE_DATA_ENDPOINT__'
