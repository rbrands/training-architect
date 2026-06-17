# Infrastructure — Training Architect

Bicep templates for deploying Training Architect to Azure.

---

## What gets deployed

| Resource | Description |
|---|---|
| App Service Plan | Existing shared plan (read-only in this deployment) |
| Web App | Linux, .NET 10, System-Assigned Managed Identity |
| Cosmos DB Account | Existing shared account (read-only in this deployment) |
| Cosmos DB Database | Existing shared database (managed by central infrastructure, not created here) |
| Cosmos DB Container | Created via `cosmosContainerName` in the existing shared database; inherits shared DB RU/s |
| Azure Key Vault | Existing shared vault (read-only in this deployment) |
| Role Assignments | RBAC for Web App Managed Identity on central resources |

---

## Prerequisites

- [Azure CLI](https://learn.microsoft.com/cli/azure/install-azure-cli) installed
- [Bicep CLI](https://learn.microsoft.com/azure/azure-resource-manager/bicep/install) installed (or use `az bicep install`)
- Logged in: `az login`
- Central and app resource groups already exist:
  ```bash
   az group create --name rg-brands-advisory-central --location germanywestcentral
   az group create --name rg-training-architect --location germanywestcentral
  ```
---

## Parameters

Copy `main.bicepparam`, fill in all `__PLACEHOLDER__` values, and keep the copy out of source control (add it to `.gitignore`).

| Placeholder | Description |
|---|---|
| `__APP_NAME__` | Web App name (must be globally unique, e.g. `training-architect`) |
| `__PLAN_NAME__` | App Service Plan name (e.g. `plan-training-architect`) |
| `__COSMOS_ACCOUNT_NAME__` | Cosmos DB account name (globally unique) |
| `__DATABASE_ID__` | Cosmos DB database name (e.g. `shared-content-db`) |
| `__CONTAINER_NAME__` | Cosmos DB container name (e.g. `training-architect`) |
| `__KEY_VAULT_NAME__` | Key Vault name (must be globally unique, e.g. `kv-training-architect`) |
| `__CERT_NAME__` | Certificate name as stored in Key Vault |
| `__TENANT_ID__` | Entra ID Directory (tenant) ID |
| `__CLIENT_ID__` | App Registration Application (client) ID |
| `__SYNCFUSION_LICENSE_KEY__` | Syncfusion Community/Commercial license key (served to WASM client via `/api/config`) |

---

## Deploy

```bash
az deployment sub create \
   --location germanywestcentral \
  --template-file infra/main.local.bicep \
  --parameters infra/main.bicepparam
```

To preview changes without deploying (what-if):

```bash
az deployment sub what-if \
   --location germanywestcentral \
  --template-file infra/main.local.bicep \
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
   dotnet publish src/TrainingArchitect -c Release -o ./publish
   az webapp deploy --resource-group rg-training-architect --name __APP_NAME__ --src-path ./publish
   ```

4. **Custom domain** (optional)  
   App Service → Custom domains → Add custom domain, then bind an SSL certificate.

---

## Notes

### Cosmos DB Throughput and Free Tier
Only **one Free Tier Cosmos DB account is allowed per subscription** and includes up to **1000 RU/s** and **25 GB** free in that account.

This deployment does **not** create the Cosmos account; it expects an existing shared account.
It only ensures database and container resources inside that account.

Throughput is **not** configured in this project and is owned by the central infrastructure project.

### Cosmos DB naming convention
Recommended pattern:

- Use one generic shared DB name (for example `shared-content-db`) for shared RU/s.
- Use one container per app/workload (for example `training-architect`, `app2-content`).
- Keep partition key design workload-specific per container.

### Shared central resources are not created here
App Service Plan, Cosmos account, Key Vault, Storage Account, and Application Insights are treated as existing shared resources and are not created by this template.
