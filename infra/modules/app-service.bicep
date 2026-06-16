// ---------------------------------------------------------------------------
// App Service Plan + Web App (Linux, .NET 10)
// The Web App uses a System-Assigned Managed Identity so that it can
// access Key Vault for the authentication certificate without storing
// any credentials in app settings.
// ---------------------------------------------------------------------------

@description('Azure region for all resources.')
param location string

@description('Name of the App Service web app.')
param appName string

@description('Name of the App Service Plan.')
param planName string

@description('Linux framework version string for .NET.')
param dotnetVersion string = 'DOTNETCORE|10.0'

@description('URI of the Azure Key Vault, e.g. https://<vault-name>.vault.azure.net/.')
param keyVaultUrl string

@description('Name of the certificate stored in Key Vault.')
param keyVaultCertificateName string

@description('Cosmos DB account endpoint URI.')
param cosmosEndpoint string

@description('Cosmos DB database name.')
param cosmosDatabaseId string

@description('Cosmos DB container name.')
param cosmosContainerName string

@description('Entra ID tenant ID.')
param tenantId string

@description('App Registration client (application) ID.')
param clientId string

@description('Primary blob endpoint URI of the Storage Account for article images.')
param storageBlobEndpoint string

@description('Application Insights connection string.')
param appInsightsConnectionString string

@description('Public site URL, e.g. https://brands-advisory.com. Used for canonical and Open Graph meta tags.')
param siteUrl string

// ---------------------------------------------------------------------------
// App Service Plan
// ---------------------------------------------------------------------------
resource appServicePlan 'Microsoft.Web/serverfarms@2023-12-01' = {
  name: planName
  location: location
  // Basic B1 - upgrade to S1 or higher for deployment slots
  sku: {
    name: 'B1'
    tier: 'Basic'
  }
  kind: 'linux'
  properties: {
    reserved: true // required for Linux plans
  }
}

// ---------------------------------------------------------------------------
// Web App
// ---------------------------------------------------------------------------
resource webApp 'Microsoft.Web/sites@2023-12-01' = {
  name: appName
  location: location
  kind: 'app,linux'
  identity: {
    type: 'SystemAssigned'
  }
  properties: {
    serverFarmId: appServicePlan.id
    httpsOnly: true
    // ARR Affinity disabled: sticky sessions are only needed
    // for SignalR (Interactive Server mode).
    // This app uses Static SSR + InteractiveWebAssembly —
    // no server-side state, no persistent connections.
    // Disabling also removes the ARRAffinity cookie which
    // is irrelevant for this architecture and adds
    // unnecessary cookie overhead (GDPR consideration).
    clientAffinityEnabled: false
    siteConfig: {
      linuxFxVersion: dotnetVersion
      appCommandLine: 'dotnet BrandsAdvisory.dll'
      // alwaysOn requires at least a Standard plan; disabled for B1
      alwaysOn: false
      // HTTP/2 enables multiplexing and header compression
      // for faster loading of Blazor's JS/CSS assets.
      http20Enabled: true
      appSettings: [
        {
          name: 'ASPNETCORE_ENVIRONMENT'
          value: 'Production'
        }
        // ----- Entra ID / Microsoft.Identity.Web -----
        {
          name: 'AzureAd__TenantId'
          value: tenantId
        }
        {
          name: 'AzureAd__ClientId'
          value: clientId
        }
        {
          name: 'AzureAd__ClientCertificates__0__SourceType'
          value: 'KeyVault'
        }
        {
          name: 'AzureAd__ClientCertificates__0__KeyVaultUrl'
          value: keyVaultUrl
        }
        {
          name: 'AzureAd__ClientCertificates__0__KeyVaultCertificateName'
          value: keyVaultCertificateName
        }
        // ----- Cosmos DB -----
        {
          name: 'CosmosDb__EndpointUri'
          value: cosmosEndpoint
        }
        {
          name: 'CosmosDb__DatabaseId'
          value: cosmosDatabaseId
        }
        {
          name: 'CosmosDb__ContainerName'
          value: cosmosContainerName
        }
        // ----- Storage -----
        {
          name: 'Storage__BlobEndpoint'
          value: storageBlobEndpoint
        }
        // ----- Key Vault URL (for IConfiguration Key Vault source) -----
        {
          name: 'KeyVault__Url'
          value: keyVaultUrl
        }
        // ----- Application Insights -----
        {
          name: 'APPLICATIONINSIGHTS_CONNECTION_STRING'
          value: appInsightsConnectionString
        }
        {
          name: 'ApplicationInsightsAgent_EXTENSION_VERSION'
          value: '~3'
        }
        {
          name: 'XDT_MicrosoftApplicationInsights_Mode'
          value: 'Recommended'
        }
        // ----- Site URL (canonical + Open Graph meta tags) -----
        {
          name: 'SiteUrl'
          value: siteUrl
        }
      ]
    }
  }
}

// ---------------------------------------------------------------------------
// Outputs
// ---------------------------------------------------------------------------

@description('Name of the deployed Web App.')
output webAppName string = webApp.name

@description('Principal ID of the Web App System-Assigned Managed Identity. Used for role assignments.')
output principalId string = webApp.identity.principalId

@description('Resource ID of the App Service Plan. Required by the custom-domain module for managed certificates.')
output appServicePlanId string = appServicePlan.id
