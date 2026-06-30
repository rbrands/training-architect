// ---------------------------------------------------------------------------
// Web App (Linux, .NET 10)
// The App Service Plan lives in the shared resource group and is passed in
// via planId. The Web App uses a System-Assigned Managed Identity so that
// it can access Key Vault for the authentication certificate without storing
// any credentials in app settings.
// ---------------------------------------------------------------------------

@description('Azure region for the Web App.')
param location string

@description('Name of the App Service web app.')
param appName string

@description('Resource ID of the App Service Plan (may live in a different resource group).')
param planId string

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

@description('Public site URL, e.g. https://www.example.com. Used for canonical and Open Graph meta tags.')
param siteUrl string

@description('Author name used for the global HTML author meta tag.')
param author string

@description('Public MCP endpoint for athlete data tool calls, e.g. https://intervals-mcp.training-architect.com/mcp.')
param mcpAthleteDataEndpoint string

@description('Microsoft Foundry project endpoint for coaching agent invocation.')
param foundryProjectEndpoint string

@description('Microsoft Foundry agent name for coaching agent invocation.')
param foundryProjectAgentName string

@description('Tags to apply to the Web App resource.')
param tags object = {}

@description('Name of the development slot.')
param devSlotName string = 'dev'

@description('Name of the staging slot.')
param stagingSlotName string = 'staging'

var devSlotSiteUrl = 'https://${appName}-${devSlotName}.azurewebsites.net'
var stagingSlotSiteUrl = 'https://${appName}-${stagingSlotName}.azurewebsites.net'

// Shared application settings for all slots.
var baseAppSettings = [
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
  // ----- Syncfusion license (resolved by App Service from Key Vault) -----
  {
    name: 'Syncfusion__LicenseKey'
    value: '@Microsoft.KeyVault(SecretUri=${keyVaultUrl}secrets/Syncfusion--LicenseKey)'
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
  {
    name: 'Mcp__AthleteData__Endpoint'
    value: mcpAthleteDataEndpoint
  }
  {
    name: 'FoundryProjectEndpoint'
    value: foundryProjectEndpoint
  }
  {
    name: 'FoundryProjectAgentName'
    value: foundryProjectAgentName
  }
]

// ---------------------------------------------------------------------------
// Web App
// ---------------------------------------------------------------------------
resource webApp 'Microsoft.Web/sites@2023-12-01' = {
  name: appName
  location: location
  kind: 'app,linux'
  tags: tags
  identity: {
    type: 'SystemAssigned'
  }
  properties: {
    serverFarmId: planId
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
      appCommandLine: 'dotnet TrainingArchitect.dll'
      // Keep production warm to avoid cold starts after idle periods.
      alwaysOn: true
      // HTTP/2 enables multiplexing and header compression
      // for faster loading of Blazor's JS/CSS assets.
      http20Enabled: true
      appSettings: [
        {
          name: 'ASPNETCORE_ENVIRONMENT'
          value: 'Production'
        }
        ...baseAppSettings
        {
          name: 'SiteUrl'
          value: siteUrl
        }
        {
          name: 'Author'
          value: author
        }
      ]
    }
  }
}

// Settings listed here stay with the slot during swap operations.
resource slotSettings 'Microsoft.Web/sites/config@2023-12-01' = {
  name: 'slotConfigNames'
  parent: webApp
  properties: {
    appSettingNames: [
      'ASPNETCORE_ENVIRONMENT'
      'SiteUrl'
      'Author'
    ]
  }
}

resource devSlot 'Microsoft.Web/sites/slots@2023-12-01' = {
  name: devSlotName
  parent: webApp
  location: location
  kind: 'app,linux'
  identity: {
    type: 'SystemAssigned'
  }
  tags: union(tags, {
    environment: 'dev'
  })
  properties: {
    serverFarmId: planId
    httpsOnly: true
    clientAffinityEnabled: false
    siteConfig: {
      linuxFxVersion: dotnetVersion
      appCommandLine: 'dotnet TrainingArchitect.dll'
      // Dev slot: disable Always On to reduce non-production runtime cost.
      alwaysOn: false
      http20Enabled: true
      appSettings: [
        {
          name: 'ASPNETCORE_ENVIRONMENT'
          value: 'Development'
        }
        ...baseAppSettings
        {
          name: 'SiteUrl'
          value: devSlotSiteUrl
        }
        {
          name: 'Author'
          value: author
        }
      ]
    }
  }
}

resource stagingSlot 'Microsoft.Web/sites/slots@2023-12-01' = {
  name: stagingSlotName
  parent: webApp
  location: location
  kind: 'app,linux'
  identity: {
    type: 'SystemAssigned'
  }
  tags: union(tags, {
    environment: 'staging'
  })
  properties: {
    serverFarmId: planId
    httpsOnly: true
    clientAffinityEnabled: false
    siteConfig: {
      linuxFxVersion: dotnetVersion
      appCommandLine: 'dotnet TrainingArchitect.dll'
      // Staging slot: disable Always On to reduce non-production runtime cost.
      alwaysOn: false
      http20Enabled: true
      appSettings: [
        {
          name: 'ASPNETCORE_ENVIRONMENT'
          value: 'Staging'
        }
        ...baseAppSettings
        {
          name: 'SiteUrl'
          value: stagingSlotSiteUrl
        }
        {
          name: 'Author'
          value: author
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

@description('Resource ID of the App Service Plan.')
output appServicePlanId string = planId

@description('Name of the development deployment slot.')
output devSlotName string = devSlot.name

@description('Name of the staging deployment slot.')
output stagingSlotName string = stagingSlot.name

@description('Principal ID of the development deployment slot Managed Identity.')
output devSlotPrincipalId string = devSlot.identity.principalId

@description('Principal ID of the staging deployment slot Managed Identity.')
output stagingSlotPrincipalId string = stagingSlot.identity.principalId
