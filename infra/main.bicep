// ---------------------------------------------------------------------------
// main.bicep — Brands Advisory CMS infrastructure entry point
//
// targetScope = 'subscription' so that modules can be deployed to two
// separate resource groups in a single pass:
//
//   centralResourceGroupName  — shared resources (Plan, Cosmos, KV, Storage, AppInsights)
//                               These resources already exist and are managed separately.
//                               This deployment reads their properties, ensures the
//                               Cosmos DB SQL database/container exist, and assigns
//                               RBAC roles to the new Web App's Managed Identity.
//
//   appResourceGroupName      — Web App + custom domain (created/updated by this deployment)
//
// Deploys / modifies:
//   - Azure Web App (.NET 10, Linux)                  → app RG       (created)
//   - RBAC role assignments for the Managed Identity  → central RG   (idempotent)
//   - Custom domain + SSL binding (conditional)        → app RG       (created)
//
// Reads (existing, never modified):
//   - App Service Plan, Cosmos DB, Key Vault, Storage Account, Application Insights
// Ensures (create/update, idempotent):
//   - Cosmos DB SQL database + container in the existing Cosmos account
// ---------------------------------------------------------------------------

targetScope = 'subscription'

// ---------------------------------------------------------------------------
// Parameters
// ---------------------------------------------------------------------------

@description('Azure region for new resources (Web App, custom domain).')
param location string = 'germanywestcentral'

@description('Tags applied to all resources created by this deployment (Web App, custom domain certificate).')
param tags object = {
  environment: 'prod'
  'managed-by': 'bicep'
  project: 'training-architect'
  tier: 'application'
}

@description('Resource group that holds the existing shared infrastructure.')
param centralResourceGroupName string

@description('Resource group for the Web App and custom domain.')
param appResourceGroupName string

@description('Name of the App Service web app.')
param appName string

@description('Name of the existing App Service Plan in the central resource group.')
param planName string

@description('Name of the existing Cosmos DB account.')
param cosmosAccountName string

@description('Cosmos DB database name.')
param cosmosDatabaseId string

@description('Cosmos DB container name.')
param cosmosContainerName string

@description('Cosmos DB container partition key path.')
param cosmosPartitionKeyPath string = '/type'

@description('Name of the existing Key Vault.')
param keyVaultName string

@description('Name of the certificate stored in Key Vault.')
param keyVaultCertificateName string

@description('Entra ID tenant ID.')
param tenantId string

@description('App Registration client (application) ID.')
param clientId string

@description('Name of the existing Storage Account.')
param storageAccountName string

@description('Name of the existing Application Insights resource.')
param appInsightsName string

@description('Public site URL, e.g. https://www.example.com. Used for canonical and Open Graph meta tags.')
param siteUrl string

// Derive the apex domain from siteUrl for the custom domain module.
var apexDomain = contains(siteUrl, 'azurewebsites.net')
  ? ''
  : replace(replace(replace(siteUrl, 'https://', ''), 'http://', ''), 'www.', '')

// ---------------------------------------------------------------------------
// Existing central resources — read-only, never created or modified here
// ---------------------------------------------------------------------------

resource existingPlan 'Microsoft.Web/serverfarms@2023-12-01' existing = {
  name: planName
  scope: resourceGroup(centralResourceGroupName)
}

resource existingCosmos 'Microsoft.DocumentDB/databaseAccounts@2024-05-15' existing = {
  name: cosmosAccountName
  scope: resourceGroup(centralResourceGroupName)
}

resource existingKeyVault 'Microsoft.KeyVault/vaults@2023-07-01' existing = {
  name: keyVaultName
  scope: resourceGroup(centralResourceGroupName)
}

resource existingStorage 'Microsoft.Storage/storageAccounts@2023-01-01' existing = {
  name: storageAccountName
  scope: resourceGroup(centralResourceGroupName)
}

resource existingAppInsights 'Microsoft.Insights/components@2020-02-02' existing = {
  name: appInsightsName
  scope: resourceGroup(centralResourceGroupName)
}

// ---------------------------------------------------------------------------
// Module: Cosmos DB SQL database + container  →  central RG
// ---------------------------------------------------------------------------
module cosmosData 'modules/cosmos-database-container.bicep' = {
  name: 'cosmos-data'
  scope: resourceGroup(centralResourceGroupName)
  params: {
    cosmosAccountName: cosmosAccountName
    databaseName: cosmosDatabaseId
    containerName: cosmosContainerName
    partitionKeyPath: cosmosPartitionKeyPath
  }
}

// ---------------------------------------------------------------------------
// Module: Web App  →  app RG
// ---------------------------------------------------------------------------
module appService 'modules/app-service.bicep' = {
  name: 'appService'
  scope: resourceGroup(appResourceGroupName)
  params: {
    location: location
    appName: appName
    planId: existingPlan.id
    keyVaultUrl: existingKeyVault.properties.vaultUri
    keyVaultCertificateName: keyVaultCertificateName
    cosmosEndpoint: existingCosmos.properties.documentEndpoint
    cosmosDatabaseId: cosmosDatabaseId
    cosmosContainerName: cosmosContainerName
    tenantId: tenantId
    clientId: clientId
    storageBlobEndpoint: existingStorage.properties.primaryEndpoints.blob
    appInsightsConnectionString: existingAppInsights.properties.ConnectionString
    siteUrl: siteUrl
    tags: tags
  }
}

// ---------------------------------------------------------------------------
// Module: Key Vault RBAC  →  central RG (where the KV lives)
// ---------------------------------------------------------------------------
module keyVaultRbac 'modules/keyvault-rbac.bicep' = {
  name: 'keyVaultRbac'
  scope: resourceGroup(centralResourceGroupName)
  params: {
    keyVaultName: keyVaultName
    principalId: appService.outputs.principalId
  }
}

// ---------------------------------------------------------------------------
// Module: Cosmos DB RBAC  →  central RG (where Cosmos lives)
// Default scope is container-level (least privilege).
// ---------------------------------------------------------------------------
module cosmosRbac 'modules/cosmos-rbac.bicep' = {
  name: 'cosmosRbac'
  scope: resourceGroup(centralResourceGroupName)
  params: {
    cosmosAccountName: cosmosAccountName
    principalId: appService.outputs.principalId
    cosmosDatabaseId: cosmosDatabaseId
    cosmosContainerName: cosmosContainerName
  }
}

// ---------------------------------------------------------------------------
// Module: Storage RBAC  →  central RG (where the Storage Account lives)
// ---------------------------------------------------------------------------
module storageRbac 'modules/storage-rbac.bicep' = {
  name: 'storageRbac'
  scope: resourceGroup(centralResourceGroupName)
  params: {
    storageAccountName: storageAccountName
    principalId: appService.outputs.principalId
  }
}

// ---------------------------------------------------------------------------
// Module: Application Insights RBAC  →  central RG (where AppInsights lives)
// ---------------------------------------------------------------------------
module appInsightsRbac 'modules/appinsights-rbac.bicep' = {
  name: 'appInsightsRbac'
  scope: resourceGroup(centralResourceGroupName)
  params: {
    appInsightsName: appInsightsName
    principalId: appService.outputs.principalId
  }
}

// ---------------------------------------------------------------------------
// Module: Custom Domain + Managed SSL (optional)  →  app RG
// Deployed automatically when siteUrl is not an azurewebsites.net URL.
// DNS records must be configured at the registrar before running this.
// ---------------------------------------------------------------------------
module domain 'modules/custom-domain.bicep' = if (!empty(apexDomain)) {
  name: 'custom-domain'
  scope: resourceGroup(appResourceGroupName)
  params: {
    location: location
    appName: appName
    customDomain: apexDomain
    appServicePlanId: existingPlan.id
    tags: tags
  }
}

// ---------------------------------------------------------------------------
// Outputs
// ---------------------------------------------------------------------------

@description('Name of the deployed Web App.')
output webAppName string = appService.outputs.webAppName

@description('Cosmos DB account endpoint URI.')
output cosmosEndpoint string = existingCosmos.properties.documentEndpoint

@description('Next steps after deployment.')
output deploymentInstructions string = '''
Post-deployment steps:
1. Add the App Service URL to the Redirect URIs in the Entra ID App Registration
   (Authentication → Add a platform → Web → https://<appName>.azurewebsites.net/signin-oidc)
2. Verify the certificate is uploaded to Key Vault under the configured name
3. Configure a custom domain and SSL binding in the App Service if required
4. Set WEBSITE_RUN_FROM_PACKAGE=1 before deploying the application package
'''
