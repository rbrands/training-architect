// ---------------------------------------------------------------------------
// Hostname SSL Binding Update
//
// Updates an existing App Service hostname binding to enable SNI SSL.
// Called by custom-domain.bicep after the managed certificate is issued.
//
// Using a nested deployment (module) is the correct Bicep pattern for
// updating a resource that was created in the parent template, since
// a single Bicep file cannot declare the same resource twice.
// ---------------------------------------------------------------------------

@description('Name of the App Service web app.')
param appName string

@description('The custom hostname to enable SSL for, e.g. www.brands-advisory.com.')
param hostname string

@description('Thumbprint of the managed certificate to bind.')
param thumbprint string

resource webApp 'Microsoft.Web/sites@2023-12-01' existing = {
  name: appName
}

resource hostNameBinding 'Microsoft.Web/sites/hostNameBindings@2023-12-01' = {
  name: hostname
  parent: webApp
  properties: {
    siteName: appName
    sslState: 'SniEnabled'
    thumbprint: thumbprint
  }
}
