# Infrastructure — Training Architect

Bicep templates for deploying Training Architect to Azure.

---

## What gets deployed

| Resource | Description |
|---|---|
| App Service Plan | Existing shared plan (read-only in this deployment) |
| Web App | Linux, .NET 10, System-Assigned Managed Identity, plus deployment slots `dev` and `staging` |
| Cosmos DB Account | Existing shared account (read-only in this deployment) |
| Cosmos DB Database | Existing shared database (managed by central infrastructure, not created here) |
| Cosmos DB Container | Created via `cosmosContainerName` in the existing shared database; inherits shared DB RU/s |
| Azure Key Vault | Existing shared vault (read-only in this deployment) |
| Role Assignments | RBAC for Web App Managed Identity on central resources |

Tagging for App Service resources:
- Production web app uses the deployment `tags` object from `main.bicep` (default: `environment=prod`).
- Slot `dev` is tagged with `environment=dev`.
- Slot `staging` is tagged with `environment=staging`.

Deployment slot settings (sticky on swap):
- `ASPNETCORE_ENVIRONMENT`
- `SiteUrl`

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

1. **App Registration — Redirect URIs**  
   In the Azure Portal: Entra ID → App registrations → your app → Authentication → Add a platform → Web.  
   Add each environment URI that can receive sign-in callbacks:

   - `https://localhost:7000/signin-oidc`
   - `https://training-architect-dev.azurewebsites.net/signin-oidc`
   - `https://training-architect-staging.azurewebsites.net/signin-oidc`
   - `https://training-architect.azurewebsites.net/signin-oidc`
   - `https://training-architect.com/signin-oidc` (optional, custom domain)

   Keep only the URIs for environments that are active.

2. **App Registration — Admin app role (`SiteAdmin`)**
   In the Azure Portal: Entra ID → App registrations → your app → App roles → Create app role

   Use these values:
   - Display name: `Site Administrator`
   - Allowed member types: `Users/Groups`
   - Value: `SiteAdmin`
   - Description: `Users in this group have admin rights.`
   - Do you want to enable this app role?: `Yes`

   Assign the role via: Entra ID → Enterprise Applications → your app → Users and groups → Add user/group → assign `Site Administrator`.

   Important: the application authorization check uses the role value (`SiteAdmin`) from the token claim, not the display name.

3. **Legal page content management**
   The legal page (`/legal`) is not populated from configuration values.
   It is managed as online content and edited in the admin UI at `/admin/legal`.

   Source of truth: persisted content in Cosmos DB (about document).

4. **Key Vault certificate**  
   Ensure the certificate is already uploaded to Key Vault under the name matching `__CERT_NAME__`.  
   The Web App Managed Identity receives the **Key Vault Certificate User** role automatically via the `keyvault-rbac` module.

5. **Deploy the application**  
   Publish the .NET app to the Web App using GitHub Actions or:
   ```bash
   dotnet publish src/TrainingArchitect -c Release -o ./publish
   az webapp deploy --resource-group rg-training-architect --name __APP_NAME__ --src-path ./publish
   ```

6. **Custom domain** (optional)  
   App Service → Custom domains → Add custom domain, then bind an SSL certificate.

7. **Deployment slots — URLs and promotion flow**

    Slot URLs:
    - Dev: `https://<APP_NAME>-dev.azurewebsites.net`
    - Staging: `https://<APP_NAME>-staging.azurewebsites.net`
    - Production: `https://<APP_NAME>.azurewebsites.net` (or your custom domain)

    Deployment flow:

    - Pull request to `main` deploys to `dev`.
    - Push to `main` deploys to `staging`.

    Promote changes with slot swap:

    ```bash
    # Validate in staging, then promote staging -> production
    az webapp deployment slot swap \
       --resource-group <APP_RESOURCE_GROUP> \
       --name <APP_NAME> \
       --slot staging \
       --target-slot production
    ```

    Recommended order: validate `dev`, validate `staging`, then swap `staging` to `production`.
    Standard flow does not use `dev -> staging` swap.

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
