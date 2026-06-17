// ---------------------------------------------------------------------------
// Cosmos DB SQL container in an existing shared-throughput database.
//
// The Cosmos DB account and the database (with shared throughput) are managed
// by central infrastructure. This module only creates the app-specific container
// inside that database. No throughput is set here — the container inherits from
// the shared database RU/s pool.
// ---------------------------------------------------------------------------

@description('Name of the existing Cosmos DB account.')
param cosmosAccountName string

@description('Name of the existing shared Cosmos DB SQL database.')
param databaseName string

@description('Cosmos DB SQL container name (app-specific).')
param containerName string

@description('Partition key path for the container.')
param partitionKeyPath string = '/type'

@description('Default TTL for container items in seconds. -1 enables TTL with no automatic expiry unless item-level _ttl is set.')
param defaultTtl int = -1

resource cosmosAccount 'Microsoft.DocumentDB/databaseAccounts@2024-05-15' existing = {
  name: cosmosAccountName
}

// Reference the existing shared database — managed by central infrastructure.
resource cosmosDatabase 'Microsoft.DocumentDB/databaseAccounts/sqlDatabases@2024-05-15' existing = {
  parent: cosmosAccount
  name: databaseName
}

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
  }
}

@description('Resource ID of the ensured Cosmos DB SQL database.')
output databaseResourceId string = cosmosDatabase.id

@description('Resource ID of the ensured Cosmos DB SQL container.')
output containerResourceId string = cosmosContainer.id
