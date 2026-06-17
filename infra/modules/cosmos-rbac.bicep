// ---------------------------------------------------------------------------
// Cosmos DB RBAC — Cosmos DB Built-in Data Contributor role assignment
//
// Grants the Web App Managed Identity read/write access to Cosmos DB
// using Cosmos DB native RBAC (sqlRoleDefinitions), NOT Azure RBAC.
// Scope is set to the account root so the SDK can read account metadata
// during client initialization, which is required for DefaultAzureCredential.
//
// Built-in role: Cosmos DB Built-in Data Contributor
// Role Definition ID: 00000000-0000-0000-0000-000000000002
//
// No primary key needed; uses DefaultAzureCredential for authentication.
// ---------------------------------------------------------------------------

@description('Name of the existing Cosmos DB account.')
param cosmosAccountName string

@description('Principal ID (object ID) of the Web App Managed Identity.')
param principalId string

var roleScope = '${cosmosAccount.id}/'

// ---------------------------------------------------------------------------
// Reference the Cosmos DB account
// ---------------------------------------------------------------------------
resource cosmosAccount 'Microsoft.DocumentDB/databaseAccounts@2024-05-15' existing = {
  name: cosmosAccountName
}

// ---------------------------------------------------------------------------
// Cosmos DB SQL Role Assignment — Built-in Data Contributor
// Web App Managed Identity gains read/write access to the container
// Account scope is required for metadata reads during SDK initialization.
// ---------------------------------------------------------------------------
resource cosmosRoleAssignment 'Microsoft.DocumentDB/databaseAccounts/sqlRoleAssignments@2024-05-15' = {
  name: guid(cosmosAccount.id, principalId, '00000000-0000-0000-0000-000000000002', roleScope)
  parent: cosmosAccount
  properties: {
    roleDefinitionId: '${cosmosAccount.id}/sqlRoleDefinitions/00000000-0000-0000-0000-000000000002'
    principalId: principalId
    scope: roleScope
  }
}
