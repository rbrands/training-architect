// ---------------------------------------------------------------------------
// Key Vault RBAC — Key Vault Certificate User role assignment
//
// Grants the Web App Managed Identity read access to certificates stored
// in an existing Key Vault. The role is scoped to the Key Vault resource.
//
// Built-in role: Key Vault Certificate User
// Role Definition ID: db79e9a7-68ee-4b58-9aeb-b90e7c24fcba
// ---------------------------------------------------------------------------

@description('Name of the existing Key Vault.')
param keyVaultName string

@description('Principal ID (object ID) to assign the role to. Typically the Web App Managed Identity.')
param principalId string

@description('Type of the principal. Defaults to ServicePrincipal for Managed Identities.')
param principalType string = 'ServicePrincipal'

// ---------------------------------------------------------------------------
// Reference the existing Key Vault (created separately)
// ---------------------------------------------------------------------------
resource keyVault 'Microsoft.KeyVault/vaults@2023-07-01' existing = {
  name: keyVaultName
}

// ---------------------------------------------------------------------------
// Role Assignment — Key Vault Certificate User
// ---------------------------------------------------------------------------
resource keyVaultCertificateUserAssignment 'Microsoft.Authorization/roleAssignments@2022-04-01' = {
  // Use a deterministic GUID based on vault + principal to make deployment idempotent
  name: guid(keyVault.id, principalId, 'db79e9a7-68ee-4b58-9aeb-b90e7c24fcba')
  scope: keyVault
  properties: {
    // Key Vault Certificate User (built-in)
    roleDefinitionId: subscriptionResourceId(
      'Microsoft.Authorization/roleDefinitions',
      'db79e9a7-68ee-4b58-9aeb-b90e7c24fcba'
    )
    principalId: principalId
    principalType: principalType
  }
}

// ---------------------------------------------------------------------------
// Role Assignment — Key Vault Secrets User
// Allows the Web App Managed Identity to read secrets.
// Required for reading secrets via AddAzureKeyVault() in Program.cs at startup.
// ---------------------------------------------------------------------------
resource keyVaultSecretsUserAssignment 'Microsoft.Authorization/roleAssignments@2022-04-01' = {
  name: guid(keyVault.id, principalId, '4633458b-17de-408a-b874-0445c86b69e6')
  scope: keyVault
  properties: {
    // Key Vault Secrets User (built-in)
    roleDefinitionId: subscriptionResourceId(
      'Microsoft.Authorization/roleDefinitions',
      '4633458b-17de-408a-b874-0445c86b69e6'
    )
    principalId: principalId
    principalType: principalType
  }
}
