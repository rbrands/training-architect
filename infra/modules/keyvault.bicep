// ---------------------------------------------------------------------------
// Azure Key Vault
//
// Creates a Key Vault with RBAC authorization (not Access Policies).
// Role assignments are managed separately in keyvault-rbac.bicep.
// The certificate for Entra ID authentication must be uploaded manually
// after the Key Vault is created.
// ---------------------------------------------------------------------------

@description('Azure region for all resources.')
param location string

@description('Name of the Key Vault.')
param keyVaultName string

@description('Entra ID Tenant ID.')
param tenantId string

resource keyVault 'Microsoft.KeyVault/vaults@2023-07-01' = {
  name: keyVaultName
  location: location
  properties: {
    sku: {
      family: 'A'
      name: 'standard'
    }
    tenantId: tenantId
    enableRbacAuthorization: true
    enableSoftDelete: true
    softDeleteRetentionInDays: 90
    enabledForDeployment: false
    enabledForDiskEncryption: false
    enabledForTemplateDeployment: false
  }
}

// ---------------------------------------------------------------------------
// Outputs
// ---------------------------------------------------------------------------
@description('Name of the deployed Key Vault.')
output keyVaultName string = keyVault.name

@description('URI of the Key Vault.')
output keyVaultUri string = keyVault.properties.vaultUri
