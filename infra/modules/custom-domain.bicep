// ---------------------------------------------------------------------------
// Custom Domain + Free Managed SSL Certificate
//
// Handles www subdomain with a CNAME-based App Service managed certificate.
// The apex domain binding is added for ownership verification only;
// HTTPS for the apex is best handled by redirecting apex → www at your
// DNS registrar or via Azure Front Door.
//
// DNS records required BEFORE deploying:
//
//   www subdomain (CNAME-based managed cert):
//     www.{customDomain}       CNAME   {appName}.azurewebsites.net
//     asuid.www.{customDomain} TXT     {domainVerificationId}
//
//   Apex domain (ownership verification only, no managed SSL):
//     {customDomain}           A       {app-service-outbound-ip}
//     asuid.{customDomain}     TXT     {domainVerificationId}
//
// The domain verification ID is available in the Azure Portal under:
//   App Service → Custom domains → Custom domain verification ID
//
// ---------------------------------------------------------------------------

@description('Azure region for the managed certificate resource.')
param location string

@description('Name of the App Service web app.')
param appName string

@description('Apex custom domain, e.g. example.com. The www subdomain is derived automatically.')
param customDomain string

@description('Resource ID of the App Service Plan. Required for App Service managed certificates.')
param appServicePlanId string

@description('Optional existing managed certificate name to reuse (e.g. www.example.com-myapp). If set, certificate creation is skipped.')
param existingManagedCertificateName string = ''

@description('Tags to apply to the managed certificate resource.')
param tags object = {}

// Reference the existing web app
resource webApp 'Microsoft.Web/sites@2023-12-01' existing = {
  name: appName
}

// ---------------------------------------------------------------------------
// Step 1a: Apex hostname binding (ownership/TXT verification, no SSL)
// ---------------------------------------------------------------------------
resource apexHostNameBinding 'Microsoft.Web/sites/hostNameBindings@2023-12-01' = {
  name: customDomain
  parent: webApp
  properties: {
    siteName: appName
    hostNameType: 'Verified'
    sslState: 'Disabled'
    customHostNameDnsRecordType: 'A'
  }
}

// ---------------------------------------------------------------------------
// Step 1b: www hostname binding (without SSL initially)
// ---------------------------------------------------------------------------
resource wwwHostNameBinding 'Microsoft.Web/sites/hostNameBindings@2023-12-01' = {
  name: 'www.${customDomain}'
  parent: webApp
  dependsOn: [
    apexHostNameBinding
  ]
  properties: {
    siteName: appName
    hostNameType: 'Verified'
    sslState: 'Disabled'
    customHostNameDnsRecordType: 'CName'
  }
}

// ---------------------------------------------------------------------------
// Step 2: Managed certificate for www
// - If existingManagedCertificateName is provided, reuse it.
// - Otherwise create a free App Service managed certificate.
// ---------------------------------------------------------------------------
resource existingWwwCertificate 'Microsoft.Web/certificates@2023-12-01' existing = if (!empty(existingManagedCertificateName)) {
  name: existingManagedCertificateName
}

resource newWwwCertificate 'Microsoft.Web/certificates@2023-12-01' = if (empty(existingManagedCertificateName)) {
  name: 'cert-www-${replace(customDomain, '.', '-')}'
  location: location
  tags: tags
  properties: {
    serverFarmId: appServicePlanId
    canonicalName: 'www.${customDomain}'
  }
  dependsOn: [wwwHostNameBinding]
}

var certificateThumbprint = !empty(existingManagedCertificateName)
  ? existingWwwCertificate!.properties.thumbprint
  : newWwwCertificate!.properties.thumbprint

// ---------------------------------------------------------------------------
// Step 3: Enable SNI SSL on the www binding via a nested deployment.
//         A nested module is required here because Bicep does not allow
//         updating the same resource twice in a single template.
//         ARM processes the child deployment after the certificate is issued.
// ---------------------------------------------------------------------------
module wwwSslBinding 'custom-domain-ssl.bicep' = {
  name: 'www-ssl-binding'
  dependsOn: [
    wwwHostNameBinding
  ]
  params: {
    appName: appName
    hostname: 'www.${customDomain}'
    thumbprint: certificateThumbprint
  }
}
