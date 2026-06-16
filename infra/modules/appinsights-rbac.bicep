// ---------------------------------------------------------------------------
// Application Insights RBAC — Monitoring Metrics Publisher
//
// Grants the Web App Managed Identity permission to publish telemetry
// to Application Insights using AAD authentication (no instrumentation key).
//
// Built-in role:
//   Monitoring Metrics Publisher  3913510d-42f4-4e42-8a64-420c390055eb
// ---------------------------------------------------------------------------

@description('Name of the existing Application Insights resource.')
param appInsightsName string

@description('Principal ID (object ID) to assign the role to. Typically the Web App Managed Identity.')
param principalId string

@description('Type of the principal. Defaults to ServicePrincipal for Managed Identities.')
param principalType string = 'ServicePrincipal'

// ---------------------------------------------------------------------------
// Reference the Application Insights resource
// ---------------------------------------------------------------------------
resource appInsights 'Microsoft.Insights/components@2020-02-02' existing = {
  name: appInsightsName
}

// ---------------------------------------------------------------------------
// Role Assignment — Monitoring Metrics Publisher
// ---------------------------------------------------------------------------
resource metricsPublisherAssignment 'Microsoft.Authorization/roleAssignments@2022-04-01' = {
  // Deterministic GUID based on resource + principal to make deployment idempotent
  name: guid(appInsights.id, principalId, '3913510d-42f4-4e42-8a64-420c390055eb')
  scope: appInsights
  properties: {
    // Monitoring Metrics Publisher (built-in)
    roleDefinitionId: subscriptionResourceId(
      'Microsoft.Authorization/roleDefinitions',
      '3913510d-42f4-4e42-8a64-420c390055eb'
    )
    principalId: principalId
    principalType: principalType
  }
}
