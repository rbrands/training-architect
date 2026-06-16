// ---------------------------------------------------------------------------
// Storage RBAC — Storage Blob Data Contributor + Storage Blob Delegator
//
// Storage Blob Data Contributor: read/write/delete blobs (server-side upload API)
// Storage Blob Delegator: issue User Delegation Keys for SAS tokens (direct upload)
//
// Built-in roles:
//   Storage Blob Data Contributor  ba92f5b4-2d11-453d-a403-e96b0029c9fe
//   Storage Blob Delegator         db58b8e5-c6ad-4a2a-8342-4190687cbf4a
// ---------------------------------------------------------------------------

@description('Name of the existing Storage Account.')
param storageAccountName string

@description('Principal ID (object ID) to assign the role to. Typically the Web App Managed Identity.')
param principalId string

@description('Type of the principal. Defaults to ServicePrincipal for Managed Identities.')
param principalType string = 'ServicePrincipal'

// ---------------------------------------------------------------------------
// Reference the Storage Account
// ---------------------------------------------------------------------------
resource storageAccount 'Microsoft.Storage/storageAccounts@2023-01-01' existing = {
  name: storageAccountName
}

// ---------------------------------------------------------------------------
// Role Assignment — Storage Blob Data Contributor
// ---------------------------------------------------------------------------
resource storageBlobDataContributorAssignment 'Microsoft.Authorization/roleAssignments@2022-04-01' = {
  // Use a deterministic GUID based on storage account + principal to make deployment idempotent
  name: guid(storageAccount.id, principalId, 'ba92f5b4-2d11-453d-a403-e96b0029c9fe')
  scope: storageAccount
  properties: {
    // Storage Blob Data Contributor (built-in)
    roleDefinitionId: subscriptionResourceId(
      'Microsoft.Authorization/roleDefinitions',
      'ba92f5b4-2d11-453d-a403-e96b0029c9fe'
    )
    principalId: principalId
    principalType: principalType
  }
}

// ---------------------------------------------------------------------------
// Role Assignment — Storage Blob Delegator
// Required for GetUserDelegationKeyAsync(), which is needed to issue
// User Delegation SAS tokens from a Managed Identity (no account key).
// ---------------------------------------------------------------------------
resource storageBlobDelegatorAssignment 'Microsoft.Authorization/roleAssignments@2022-04-01' = {
  name: guid(storageAccount.id, principalId, 'db58b8e5-c6ad-4a2a-8342-4190687cbf4a')
  scope: storageAccount
  properties: {
    // Storage Blob Delegator (built-in)
    roleDefinitionId: subscriptionResourceId(
      'Microsoft.Authorization/roleDefinitions',
      'db58b8e5-c6ad-4a2a-8342-4190687cbf4a'
    )
    principalId: principalId
    principalType: principalType
  }
}
