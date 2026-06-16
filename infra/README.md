# Infrastructure — Brands Advisory CMS

Bicep templates for deploying the Brands Advisory CMS to Azure.

---

## What gets deployed

| Resource | Description |
|---|---|
| App Service Plan | Linux, B1 SKU |
| Web App | Linux, .NET 10, System-Assigned Managed Identity |
| Cosmos DB Account | NoSQL API, Free Tier, Session consistency |
| Cosmos DB Database | Configured via `cosmosDatabaseId` parameter |
| Cosmos DB Container | 400 RU/s, partition key `/type` |
| Azure Key Vault | RBAC authorization enabled, soft-delete on |
| Role Assignment | Key Vault Certificate User for the Web App Managed Identity |

---

## Prerequisites

- [Azure CLI](https://learn.microsoft.com/cli/azure/install-azure-cli) installed
- [Bicep CLI](https://learn.microsoft.com/azure/azure-resource-manager/bicep/install) installed (or use `az bicep install`)
- Logged in: `az login`
- Target resource group already exists:
  ```bash
  az group create --name rg-brands-advisory --location germanywestcentral
  ```
---

## Parameters

Copy `main.bicepparam`, fill in all `__PLACEHOLDER__` values, and keep the copy out of source control (add it to `.gitignore`).

| Placeholder | Description |
|---|---|
| `__APP_NAME__` | Web App name (must be globally unique, e.g. `brands-advisory`) |
| `__PLAN_NAME__` | App Service Plan name (e.g. `plan-brands-advisory`) |
| `__COSMOS_ACCOUNT_NAME__` | Cosmos DB account name (globally unique) |
| `__DATABASE_ID__` | Cosmos DB database name (e.g. `brands-advisory`) |
| `__CONTAINER_NAME__` | Cosmos DB container name (e.g. `content`) |
| `__KEY_VAULT_NAME__` | Key Vault name (must be globally unique, e.g. `kv-brands-advisory`) |
| `__CERT_NAME__` | Certificate name as stored in Key Vault |
| `__TENANT_ID__` | Entra ID Directory (tenant) ID |
| `__CLIENT_ID__` | App Registration Application (client) ID |
| `__SYNCFUSION_LICENSE_KEY__` | Syncfusion Community/Commercial license key (served to WASM client via `/api/config`) |

---

## Deploy

```bash
az deployment group create \
  --resource-group rg-brands-advisory \
  --template-file infra/main.bicep \
  --parameters infra/main.bicepparam
```

To preview changes without deploying (what-if):

```bash
az deployment group what-if \
  --resource-group rg-brands-advisory \
  --template-file infra/main.bicep \
  --parameters infra/main.bicepparam
```

---

## Post-deployment steps

1. **App Registration — Redirect URI**  
   In the Azure Portal: Entra ID → App registrations → your app → Authentication → Add a platform → Web  
   Add: `https://<APP_NAME>.azurewebsites.net/signin-oidc`

2. **Key Vault certificate**  
   Ensure the certificate is already uploaded to Key Vault under the name matching `__CERT_NAME__`.  
   The Web App Managed Identity receives the **Key Vault Certificate User** role automatically via the `keyvault-rbac` module.

3. **Deploy the application**  
   Publish the .NET app to the Web App using GitHub Actions or:
   ```bash
   dotnet publish src/BrandsAdvisory -c Release -o ./publish
   az webapp deploy --resource-group rg-brands-advisory --name __APP_NAME__ --src-path ./publish
   ```

4. **Custom domain** (optional)  
   App Service → Custom domains → Add custom domain, then bind an SSL certificate.

---

## Notes

### Cosmos DB Free Tier
Only **one Free Tier account is allowed per Azure subscription**.  
If your subscription already has a Free Tier Cosmos DB account, open `modules/cosmos.bicep` and change:
```bicep
enableFreeTier: false
```

### Key Vault not managed by these templates
The Key Vault is intentionally excluded to avoid accidental deletion of certificates.  
Create or manage it independently via the Azure Portal or a separate deployment.
