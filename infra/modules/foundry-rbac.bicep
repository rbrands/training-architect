// ---------------------------------------------------------------------------
// Foundry RBAC — Foundry User role assignment
//
// Grants a Managed Identity permission to invoke models/agents in Foundry.
//
// Built-in role:
//   Foundry User  53ca6127-db72-4b80-b1b0-d745d6d5456d
// ---------------------------------------------------------------------------

@description('Name of the existing Foundry account (Microsoft.CognitiveServices/accounts).')
param foundryAccountName string

@description('Principal ID (object ID) to assign the role to. Typically the Web App or slot Managed Identity.')
param principalId string

@description('Type of the principal. Defaults to ServicePrincipal for Managed Identities.')
param principalType string = 'ServicePrincipal'

// ---------------------------------------------------------------------------
// Role Assignment — Foundry User
// ---------------------------------------------------------------------------
resource foundryUserAssignment 'Microsoft.CognitiveServices/accounts/providers/roleAssignments@2022-04-01' = {
  // Extension resource form avoids ARM validating Microsoft.CognitiveServices/accounts
  // with the role assignment API version.
  name: '${foundryAccountName}/Microsoft.Authorization/${guid(foundryAccountName, principalId, '53ca6127-db72-4b80-b1b0-d745d6d5456d')}'
  properties: {
    roleDefinitionId: subscriptionResourceId(
      'Microsoft.Authorization/roleDefinitions',
      '53ca6127-db72-4b80-b1b0-d745d6d5456d'
    )
    principalId: principalId
    principalType: principalType
  }
}
