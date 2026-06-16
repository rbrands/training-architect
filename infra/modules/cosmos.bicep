// ---------------------------------------------------------------------------
// Azure Cosmos DB (NoSQL API)
// Creates an account, a database, and a single container.
// Free Tier is enabled by default — only one Free Tier account is allowed
// per subscription. Set enableFreeTier to false if another already exists.
// ---------------------------------------------------------------------------

@description('Azure region for all resources.')
param location string

@description('Globally unique name for the Cosmos DB account.')
param accountName string

@description('Name of the Cosmos DB database.')
param databaseName string

@description('Name of the Cosmos DB container.')
param containerName string

@description('Partition key path for the container.')
param partitionKeyPath string = '/type'

@description('Default TTL for container items in seconds. -1 = TTL enabled, items live forever unless they set their own _ttl. Omit or set to 0 to disable TTL entirely.')
param defaultTtl int = -1

// ---------------------------------------------------------------------------
// Cosmos DB Account
// ---------------------------------------------------------------------------
resource cosmosAccount 'Microsoft.DocumentDB/databaseAccounts@2024-05-15' = {
  name: accountName
  location: location
  kind: 'GlobalDocumentDB'
  properties: {
    // Free Tier: first 1000 RU/s and 25 GB free — one per subscription
    enableFreeTier: true
    databaseAccountOfferType: 'Standard'
    consistencyPolicy: {
      defaultConsistencyLevel: 'Session'
    }
    locations: [
      {
        locationName: location
        failoverPriority: 0
        isZoneRedundant: false
      }
    ]
    // NoSQL API — no additional capabilities needed
    capabilities: []
  }
}

// ---------------------------------------------------------------------------
// Database
// ---------------------------------------------------------------------------
resource cosmosDatabase 'Microsoft.DocumentDB/databaseAccounts/sqlDatabases@2024-05-15' = {
  parent: cosmosAccount
  name: databaseName
  properties: {
    resource: {
      id: databaseName
    }
  }
}

// ---------------------------------------------------------------------------
// Container (provisioned throughput, 400 RU/s)
// ---------------------------------------------------------------------------
resource cosmosContainer 'Microsoft.DocumentDB/databaseAccounts/sqlDatabases/containers@2024-05-15' = {
  parent: cosmosDatabase
  name: containerName
  properties: {
    resource: {
      id: containerName
      partitionKey: {
        paths: [
          partitionKeyPath
        ]
        kind: 'Hash'
      }
      defaultTtl: defaultTtl
    }
    options: {
      throughput: 400
    }
  }
}

// ---------------------------------------------------------------------------
// Outputs
// ---------------------------------------------------------------------------

@description('Cosmos DB account endpoint URI.')
output cosmosEndpoint string = cosmosAccount.properties.documentEndpoint

@description('Cosmos DB account name.')
output cosmosAccountName string = cosmosAccount.name
