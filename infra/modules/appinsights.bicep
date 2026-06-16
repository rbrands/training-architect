// ---------------------------------------------------------------------------
// Application Insights + Log Analytics Workspace
//
// Deploys a workspace-based Application Insights resource for end-to-end
// observability of the Blazor Web App.
//
// Telemetry collected automatically:
//   - HTTP requests (Static SSR + API endpoints)
//   - Dependency calls (Cosmos DB, Key Vault, Azure Storage)
//   - Exceptions and failed requests
//   - Performance metrics
// ---------------------------------------------------------------------------

@description('Azure region for all resources.')
param location string

@description('Name of the Application Insights resource.')
param appInsightsName string

@description('Name of the Log Analytics workspace.')
param logAnalyticsName string

// ---------------------------------------------------------------------------
// Log Analytics Workspace
// ---------------------------------------------------------------------------
resource logAnalytics 'Microsoft.OperationalInsights/workspaces@2023-09-01' = {
  name: logAnalyticsName
  location: location
  properties: {
    sku: { name: 'PerGB2018' }
    retentionInDays: 30
  }
}

// ---------------------------------------------------------------------------
// Application Insights (workspace-based)
// ---------------------------------------------------------------------------
resource appInsights 'Microsoft.Insights/components@2020-02-02' = {
  name: appInsightsName
  location: location
  kind: 'web'
  properties: {
    Application_Type: 'web'
    WorkspaceResourceId: logAnalytics.id
    RetentionInDays: 30
    publicNetworkAccessForIngestion: 'Enabled'
    publicNetworkAccessForQuery: 'Enabled'
  }
}

// ---------------------------------------------------------------------------
// Outputs
// ---------------------------------------------------------------------------

@description('Application Insights connection string. Pass to App Service as APPLICATIONINSIGHTS_CONNECTION_STRING.')
output connectionString string = appInsights.properties.ConnectionString

@description('Application Insights instrumentation key (legacy — prefer connectionString).')
output instrumentationKey string = appInsights.properties.InstrumentationKey
