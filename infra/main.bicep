// ---------------------------------------------------------------------------
// main.bicep — Brands Advisory CMS infrastructure entry point
//
// Deploys:
//   - Azure Cosmos DB account, database, and container
//   - Azure App Service Plan + Web App (.NET 10, Linux)
//   - Azure Key Vault
//   - Azure Storage Account (images)
//   - Application Insights + Log Analytics Workspace
//   - RBAC role assignments for the Web App Managed Identity
//   - Custom domain + SSL binding (conditional, only when siteUrl is set)
// ---------------------------------------------------------------------------

targetScope = 'resourceGroup'

// ---------------------------------------------------------------------------
// Parameters
// ---------------------------------------------------------------------------

@description('Azure region for all new resources.')
param location string = 'germanywestcentral'

@description('Name of the App Service web app.')
param appName string

@description('Name of the App Service Plan.')
param planName string

@description('Globally unique name for the Cosmos DB account.')
param cosmosAccountName string

@description('Cosmos DB database name.')
param cosmosDatabaseId string

@description('Cosmos DB container name.')
param cosmosContainerName string

@description('Name of the existing Key Vault.')
param keyVaultName string

@description('Name of the certificate stored in Key Vault.')
param keyVaultCertificateName string

@description('Entra ID tenant ID.')
param tenantId string

@description('App Registration client (application) ID.')
param clientId string

@description('Globally unique name for the Storage Account (3-24 lowercase alphanumeric).')
param storageAccountName string

@description('Name of the Application Insights resource.')
param appInsightsName string

@description('Name of the Log Analytics workspace.')
param logAnalyticsName string

@description('Public site URL, e.g. https://brands-advisory.com. Used for canonical and Open Graph meta tags.')
param siteUrl string

// Derive the apex domain from siteUrl for the custom domain module.
// Empty when siteUrl still points to azurewebsites.net (= no custom domain).
// Strips https://, http://, and the www. prefix to get the bare apex domain.
var apexDomain = contains(siteUrl, 'azurewebsites.net')
  ? ''
  : replace(replace(replace(siteUrl, 'https://', ''), 'http://', ''), 'www.', '')

// ---------------------------------------------------------------------------
// Module: Cosmos DB
// ---------------------------------------------------------------------------
module cosmos 'modules/cosmos.bicep' = {
  name: 'cosmos'
  params: {
    location: location
    accountName: cosmosAccountName
    databaseName: cosmosDatabaseId
    containerName: cosmosContainerName
  }
}

// ---------------------------------------------------------------------------
// Module: Key Vault
// ---------------------------------------------------------------------------
module keyVault 'modules/keyvault.bicep' = {
  name: 'keyvault'
  params: {
    location: location
    keyVaultName: keyVaultName
    tenantId: tenantId
  }
}

// ---------------------------------------------------------------------------
// Module: Storage Account (article images)
// ---------------------------------------------------------------------------
module storage 'modules/storage.bicep' = {
  name: 'storage'
  params: {
    location: location
    storageAccountName: storageAccountName
  }
}

// ---------------------------------------------------------------------------
// Module: Application Insights + Log Analytics
// ---------------------------------------------------------------------------
module appInsights 'modules/appinsights.bicep' = {
  name: 'appinsights'
  params: {
    location: location
    appInsightsName: appInsightsName
    logAnalyticsName: logAnalyticsName
  }
}

// ---------------------------------------------------------------------------
// Module: App Service + Web App
// ---------------------------------------------------------------------------
module appService 'modules/app-service.bicep' = {
  name: 'appService'
  params: {
    location: location
    appName: appName
    planName: planName
    keyVaultUrl: keyVault.outputs.keyVaultUri
    keyVaultCertificateName: keyVaultCertificateName
    cosmosEndpoint: cosmos.outputs.cosmosEndpoint
    cosmosDatabaseId: cosmosDatabaseId
    cosmosContainerName: cosmosContainerName
    tenantId: tenantId
    clientId: clientId
    storageBlobEndpoint: storage.outputs.blobEndpoint
    appInsightsConnectionString: appInsights.outputs.connectionString
    siteUrl: siteUrl
  }
}

// ---------------------------------------------------------------------------
// Module: Key Vault RBAC (only when Key Vault exists)
// ---------------------------------------------------------------------------
module keyVaultRbac 'modules/keyvault-rbac.bicep' = {
  name: 'keyVaultRbac'
  params: {
    keyVaultName: keyVault.outputs.keyVaultName
    principalId: appService.outputs.principalId
  }
}

// ---------------------------------------------------------------------------
// Module: Cosmos DB RBAC (Built-in Data Contributor for Managed Identity)
// ---------------------------------------------------------------------------
module cosmosRbac 'modules/cosmos-rbac.bicep' = {
  name: 'cosmosRbac'
  params: {
    cosmosAccountName: cosmosAccountName
    principalId: appService.outputs.principalId
  }
}

// ---------------------------------------------------------------------------
// Module: Storage RBAC (Blob Data Contributor for Managed Identity)
// ---------------------------------------------------------------------------
module storageRbac 'modules/storage-rbac.bicep' = {
  name: 'storageRbac'
  params: {
    storageAccountName: storageAccountName
    principalId: appService.outputs.principalId
  }
}

// ---------------------------------------------------------------------------
// Module: Application Insights RBAC (Monitoring Metrics Publisher)
// Allows the Managed Identity to publish telemetry via AAD authentication
// instead of the instrumentation key.
// ---------------------------------------------------------------------------
module appInsightsRbac 'modules/appinsights-rbac.bicep' = {
  name: 'appInsightsRbac'
  params: {
    appInsightsName: appInsightsName
    principalId: appService.outputs.principalId
  }
}

// ---------------------------------------------------------------------------
// Module: Custom Domain + Managed SSL (optional)
// Deployed automatically when siteUrl is not an azurewebsites.net URL.
// DNS records must be configured at the registrar before running this.
// ---------------------------------------------------------------------------
module domain 'modules/custom-domain.bicep' = if (!empty(apexDomain)) {
  name: 'custom-domain'
  params: {
    location: location
    appName: appName
    customDomain: apexDomain
    appServicePlanId: appService.outputs.appServicePlanId
  }
}

// ---------------------------------------------------------------------------
// Outputs
// ---------------------------------------------------------------------------

@description('Name of the deployed Web App.')
output webAppName string = appService.outputs.webAppName

@description('Cosmos DB account endpoint URI.')
output cosmosEndpoint string = cosmos.outputs.cosmosEndpoint

@description('Next steps after deployment.')
output deploymentInstructions string = '''
Post-deployment steps:
1. Add the App Service URL to the Redirect URIs in the Entra ID App Registration
   (Authentication → Add a platform → Web → https://<appName>.azurewebsites.net/signin-oidc)
2. Verify the certificate is uploaded to Key Vault under the configured name
3. Configure a custom domain and SSL binding in the App Service if required
4. Set WEBSITE_RUN_FROM_PACKAGE=1 before deploying the application package
'''
