// ---------------------------------------------------------------------------
// Cosmos DB RBAC — Cosmos DB Built-in Data Contributor role assignment
//
// Grants the Web App Managed Identity read/write access to Cosmos DB data
// using Cosmos DB native RBAC (sqlRoleDefinitions), NOT Azure RBAC.
//
// Built-in role: Cosmos DB Built-in Data Contributor
// Role Definition ID: 00000000-0000-0000-0000-000000000002
//
// This role allows the identity to read and write documents.
// No primary key is needed; authentication uses DefaultAzureCredential.
// ---------------------------------------------------------------------------

@description('Name of the existing Cosmos DB account.')
param cosmosAccountName string

@description('Principal ID (object ID) to assign the role to. Typically the Web App Managed Identity.')
param principalId string

@description('Type of the principal. Defaults to ServicePrincipal for Managed Identities.')
#disable-next-line no-unused-params
param principalType string = 'ServicePrincipal'

// ---------------------------------------------------------------------------
// Reference the Cosmos DB account
// ---------------------------------------------------------------------------
resource cosmosAccount 'Microsoft.DocumentDB/databaseAccounts@2024-05-15' existing = {
  name: cosmosAccountName
}

// ---------------------------------------------------------------------------
// Cosmos DB SQL Role Assignment — Built-in Data Contributor
// ---------------------------------------------------------------------------
resource cosmosRoleAssignment 'Microsoft.DocumentDB/databaseAccounts/sqlRoleAssignments@2024-05-15' = {
  name: guid(cosmosAccount.id, principalId, '00000000-0000-0000-0000-000000000002')
  parent: cosmosAccount
  properties: {
    roleDefinitionId: '${cosmosAccount.id}/sqlRoleDefinitions/00000000-0000-0000-0000-000000000002'
    principalId: principalId
    scope: cosmosAccount.id
  }
}
