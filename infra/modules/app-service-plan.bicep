// ---------------------------------------------------------------------------
// App Service Plan (Linux, B1)
// Deployed to the central/shared resource group so the plan can be shared
// across multiple Web Apps without duplicating costs.
// ---------------------------------------------------------------------------

@description('Azure region for the App Service Plan.')
param location string

@description('Name of the App Service Plan.')
param planName string

resource appServicePlan 'Microsoft.Web/serverfarms@2023-12-01' = {
  name: planName
  location: location
  sku: {
    name: 'B1'
    tier: 'Basic'
  }
  kind: 'linux'
  properties: {
    reserved: true // required for Linux plans
  }
}

// ---------------------------------------------------------------------------
// Outputs
// ---------------------------------------------------------------------------

@description('Resource ID of the App Service Plan. Passed to the Web App and the custom-domain certificate.')
output planId string = appServicePlan.id
