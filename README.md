# Training Architect

*AI-powered cycling coach backed by intervals.icu*

Training Architect is a Blazor web application that connects to [intervals.icu](https://intervals.icu) and gives athletes a personalised virtual coaching experience. An AI-powered coach analyses current fitness data (CTL, ATL, TSB) and proposes structured weekly training plans, which the athlete can accept, adjust, or reject directly in the app.

It is the user interface for the data and coaching layer published in [intervals-icu-sync](https://github.com/rbrands/intervals-icu-sync), reusing that repository's coaching logic and prompts. If you don't want to set up the MCP server in your own GenAI tooling, the hosted version at [training-architect.com](https://training-architect.com) can be used straight away.

**Key capabilities:**

- **Virtual coach** — structured prompt interface powered by a Microsoft Foundry Agent (coaching orchestration)
- **intervals.icu integration** — reads athlete fitness data and workout library via the intervals.icu REST API
- **Structured plan proposals** — weekly training plans with day-by-day workouts including zone, TSS, IF, and coaching notes
- **Athlete confirmation flow** — Accept / Adjust / Reject without a full page reload; decision forwarded to the coaching agent
- **Azure-native** — Cosmos DB, Key Vault, Managed Identity, zero secrets stored anywhere
  - Key Vault for sensitive configuration — secrets loaded at startup via `AddAzureKeyVault()`, only in production
  - Application Insights for request tracking, dependency monitoring, and exception logging
- **Infrastructure as Code** — Bicep templates for all Azure resources
- **CI/CD with GitHub Actions** — automated deployment using OIDC Federated Credentials (no client secrets)

---

## Tech Stack

| Layer | Technology |
|---|---|
| Framework | [.NET 10 / Blazor Web App](https://dotnet.microsoft.com/en-us/apps/aspnet/web-apps/blazor) |
| Hosting | [Azure Web App (App Service)](https://learn.microsoft.com/en-us/azure/app-service/) |
| Database | [Azure Cosmos DB NoSQL](https://learn.microsoft.com/en-us/azure/cosmos-db/nosql/) |
| Authentication | [Microsoft Entra ID](https://learn.microsoft.com/en-us/entra/identity/) via [Microsoft.Identity.Web](https://github.com/AzureAD/microsoft-identity-web) |
| MCP Integration | Intervals MCP Server (`prepare_week_data`) called by the app backend |
| GenAI Agent | Microsoft Foundry Agent (for coaching orchestration) |
| UI Components | [Syncfusion Blazor](https://www.syncfusion.com/blazor-components) (Community License) |
| CI/CD | [GitHub Actions](https://docs.github.com/en/actions) |
| Observability | Azure Application Insights + Log Analytics |

### Dependency Diagram

```mermaid
graph TD
    subgraph Solution["training-architect Solution (.NET 10)"]
        BA["TrainingArchitect\n(Blazor SSR Host)"]
        BAC["TrainingArchitect.Client\n(Blazor WASM)"]
        CORE["TrainingArchitect.Core\n(Class Library)"]
        INFRA["TrainingArchitect.Infrastructure\n(Class Library)"]
    end

  subgraph GenAI["GenAI Layer"]
    FDRY["Microsoft Foundry Agent\n(Coaching Orchestration)"]
  end

  subgraph MCP["MCP Layer"]
    IMCP["Intervals MCP Server\n(prepare_week_data)"]
  end

  INTV["intervals.icu REST API"]

    BA --> BAC
    BA --> CORE
    BA --> INFRA
    BAC --> CORE
    INFRA --> CORE
  BA -. invokes .-> FDRY
  BA -. calls tool .-> IMCP
  IMCP -. fetches athlete data .-> INTV
  IMCP -. returns prepared week data .-> BA
  FDRY -. uses contracts from .-> CORE
```

---

## Training Architect — UI Shell

The repository includes a scaffolded front-end for an AI-powered cycling coaching feature
("Training Architect"). All orchestration, agent calls, auth flows, and MCP integration are
**intentionally left as stubs** with `// TODO:` markers pointing to the integration contract.

### Pages

| Route | Render mode | Description |
|---|---|---|
| `/coach` | `InteractiveWebAssembly` | Coaching panel; backed by `IChatService` |
| `/blog` | `InteractiveWebAssembly` | knowledge base with articles around training |

### Render-mode Strategy

```
Static SSR (default)
  └─ /training-plan  ← StreamRendering; IAuthContext resolved server-side
        └─ <PlanConfirmation rendermode="InteractiveWebAssembly" />
              Athlete can Accept / Adjust / Reject without a full page reload.
              Decision bubbles up via EventCallback → TODO: forward to agent.

InteractiveWebAssembly (minimal surface)
  ├─ /coach        ← IChatService scoped per WASM session
```

The WASM surface is kept minimal: only the two pages that require client-side reactivity
(`/coach`) run as full WASM circuits.

### Orchestration Plug-in Points

Replace each stub in `TrainingArchitect.Core/Services/` with the real implementation and
re-register it in `TrainingArchitect/Program.cs` (server) and
`TrainingArchitect.Client/Program.cs` (WASM).

| Interface | Stub | Integration point |
|---|---|---|
| `IChatService` | `StubChatService` | Invoke the Microsoft Foundry Agent and forward messages through the MCP server |
| `IAuthContext` | `StubAuthContext` | Read OIDC subject claim from `IHttpContextAccessor`; resolve `AthleteTier` from entitlement store — **never from client input** |
| `IIntervalsDataProvider` | `StubIntervalsDataProvider` | Call the intervals.icu REST API with the athlete's stored API key (retrieved server-side from Key Vault) |

### MCP Athlete Data Endpoint

The endpoint at `/api/athlete-data` now supports credential forwarding via headers and executes the MCP tool `prepare_week_data` server-side.

- Required request headers:
  - `X-Intervals-Athlete-Id`
  - `X-Intervals-Api-Key`
- Required server configuration in `src/TrainingArchitect/appsettings.json`:
  - `Mcp:AthleteData:Endpoint`

For local development, set real values with user-secrets instead of committing non-placeholder values.

### New Core Models

| Model | Purpose |
|---|---|
| `ChatMessage` | Single message in the conversation (`User` / `Assistant` roles) |
| `TrainingPlan` | Weekly plan with ordered `PlannedWorkout` entries |
| `PlannedWorkout` | Day, title, type, duration, TSS, IF, zone, coaching description |
| `AthleteProfile` | Athlete ID, display name, `AthleteTier` (resolved server-side) |
| `AthleteSnapshot` | Current CTL / ATL / TSB and `FitnessPoint` history from intervals.icu |
| `PlanDecision` | Athlete's verdict on a proposed plan (`Accepted` / `AdjustmentRequested` / `Rejected`) |

---

## Architecture Decisions

### Render Modes

- **Static SSR** is the default render mode for all public pages (`/`, `/blog`, `/blog/{slug}`). This ensures fast initial load times and full SEO indexability without JavaScript requirements. All public pages include full SEO meta tags: title, description, Open Graph (`og:*`), article tags, and canonical URLs. Article detail pages use the article's own title and summary for dynamic meta tags.
- **InteractiveWebAssembly** is used for admin edit pages (`/admin/about`, `/admin/articleeditor/{id?}`, `/admin/legal`) that require Syncfusion Grid and Rich Text Editor interactivity. These pages live in the `TrainingArchitect.Client` Blazor WebAssembly project and are served by the host server. Data is fetched via minimal API endpoints (`/api/about`, `/api/projects`, `/api/articles`) that require the `SiteAdmin` role.

### Owner-Only Editing

There is no separate admin user database. The site owner is identified by the **`SiteAdmin` App Role** assigned in the Entra ID Enterprise Application. The role is checked on every request server-side via `IOwnerService` — UI visibility alone is never relied upon.

Define the app role once in **Entra ID → App registrations → your app → App roles → Create app role** with these values:

- Display name: `Site Administrator`
- Allowed member types: `Users/Groups`
- Value: `SiteAdmin`
- Description: `Users in this group have admin rights.`
- Do you want to enable this app role?: `Yes`

```
User logs in via Entra ID
       ↓
App Role claims included in token
       ↓
IOwnerService.IsOwner() checks user.IsInRole("SiteAdmin")
       ↓
IsOwner cascaded as bool to all Blazor components
```

To grant access: **Entra ID → Enterprise Applications → your app → Users and groups → Add user/group → assign role `Site Administrator`**.

Important: the application authorization check uses the role **value** (`SiteAdmin`) in the token claim, not the display name.

### Observability

Application Insights is configured with a Log Analytics workspace backend.
Telemetry is collected automatically for:
- All HTTP requests (Static SSR and API endpoints)
- Dependency calls (Cosmos DB, Key Vault, Azure Storage)
- Exceptions and failed requests
- Performance metrics

The connection string is stored in App Service configuration
(`APPLICATIONINSIGHTS_CONNECTION_STRING`) — not a secret,
safe to store as an App Setting.

Locally, Application Insights telemetry is disabled by default
unless `APPLICATIONINSIGHTS_CONNECTION_STRING` is set in user-secrets.

### Data Model

All data is stored in the Cosmos DB container configured by the deployment (`cosmosContainerName`) with a `type` field as logical partition:

| type | Description |
|---|---|
| `about` | Single document, id = `about` |
| `article` | One document per article, partition key = `article` |

---

## Project Structure

```
training-architect.slnx
infra/
├── main.bicep                      # Entry point — orchestrates all modules
├── main.bicepparam                 # Production parameter values
├── main.local.bicepparam           # Local parameter values (gitignored)
└── modules/
    ├── app-service.bicep           # App Service Plan + Linux Web App (.NET 10)
    ├── appinsights.bicep           # Application Insights + Log Analytics workspace
    ├── cosmos.bicep                # Cosmos DB account, database, container
    ├── cosmos-rbac.bicep           # Cosmos DB Built-in Data Contributor → Web App identity
    ├── custom-domain.bicep         # Apex + www hostname bindings
    ├── custom-domain-ssl.bicep     # Free managed SSL certificate for www subdomain
    ├── keyvault.bicep              # Key Vault
    ├── keyvault-rbac.bicep         # KV Certificate User + Secrets User → Web App identity
    ├── storage.bicep               # Storage Account + images blob container
    └── storage-rbac.bicep          # Storage Blob Data Contributor → Web App identity
src/
├── TrainingArchitect/             # Blazor Web App host (SSR + API)
│   ├── Components/
│   │   ├── Layout/             # NavMenu, MainLayout
│   │   └── Pages/              # Public pages (Static SSR)
│   ├── Endpoints/              # Minimal API endpoints for admin data
│   │   ├── AboutEndpoints.cs
│   │   ├── ArticleEndpoints.cs
│   ├── Models/
│   │   └── UserInfo.cs         # DTO for /api/user (auth state for WASM)
│   └── Program.cs
├── TrainingArchitect.Client/      # Blazor WebAssembly client (admin pages)
│   ├── Pages/Admin/            # Admin edit pages (InteractiveWebAssembly)
│   │   ├── AboutEditor.razor
│   │   ├── ArticleEditor.razor
│   ├── Services/
│   │   ├── ApiAuthenticationStateProvider.cs  # Calls /api/user
│   │   ├── HttpAboutRepository.cs
│   │   ├── HttpArticleRepository.cs
│   └── Program.cs
├── TrainingArchitect.Core/        # Domain layer (no infrastructure dependencies)
│   ├── Interfaces/
│   │   ├── IRepository.cs          # Generic base repository interface
│   │   ├── IArticleRepository.cs
│   │   ├── IAboutRepository.cs
│   │   └── IOwnerService.cs
│   ├── Models/
│   │   ├── CosmosDocument.cs       # Base class for all Cosmos DB documents
│   │   ├── Article.cs
│   │   ├── AboutContent.cs
│   │   └── ProfileLink.cs
│   └── Services/
│       └── OwnerService.cs
└── TrainingArchitect.Infrastructure/  # Data access layer (Cosmos DB)
    └── Repositories/
        ├── CosmosRepository.cs     # Generic base repository (Cosmos DB SDK)
        ├── ArticleRepository.cs
        └── AboutRepository.cs
```

Public pages use Static SSR and depend only on `Core` interfaces, served from Cosmos DB via `Infrastructure`. Admin pages run as WebAssembly in `TrainingArchitect.Client` and call the host server's minimal API endpoints — all protected by the `SiteAdmin` role.

---

## Initial Setup (once per project)

Before GitHub Actions can deploy infrastructure and code, a one-time manual setup is required to bootstrap the deployment identity and permissions.

### 1. Create Resource Groups
```bash
az group create \
  --name <app-resource-group-name> \
  --location <location>

az group create \
  --name <central-resource-group-name> \
  --location <location>
```

### 2. Create Deployment Service Principal

Use `Create-ServicePrincipalForDeployment.ps1` from [cloud-admin-toolkit](https://github.com/rbrands/cloud-admin-toolkit):
```powershell
.\Create-ServicePrincipalForDeployment.ps1 -ConfigName <project-name>
```
This script also assigns **Contributor** and **User Access Administrator** roles on the target resource group.

For this repository, infrastructure deployment runs at **subscription scope** and orchestrates resources across two resource groups (`app` and `central`) in one deployment.
Because of that, the deployment principal must be able to validate and execute deployments at `/subscriptions/<id>`.

> **Important (multi-RG/shared storage setup):**
> This deployment uses an **existing** Storage Account from the `central` resource group.
> Ensure the blob container `images` exists before first image upload.
> If it does not exist yet, create it manually:
>
> ```bash
> az storage container create \
>   --name images \
>   --account-name <storage-account-name> \
>   --auth-mode login
> ```

```powershell
# Optional: create/update the deployment principal using your toolkit script
.\Create-ServicePrincipalForDeployment.ps1 -ConfigName <project-name>

# Required for this repository's subscription-scope Bicep deployment
az role assignment create \
  --assignee-object-id <service-principal-object-id> \
  --assignee-principal-type ServicePrincipal \
  --role Contributor \
  --scope /subscriptions/<azure-subscription-id>
```

If the same principal creates RBAC assignments in deployment, assign **User Access Administrator** (or **Owner**) on the relevant scope as well.

### 3. Add OIDC Federated Credential

Use `Add-FederatedCredentialForGitHub.ps1` from [cloud-admin-toolkit](https://github.com/rbrands/cloud-admin-toolkit):
```powershell
.\Add-FederatedCredentialForGitHub.ps1 -ConfigName <project-name>
```

After this step no client secret is stored anywhere. OIDC handles authentication via GitHub's identity provider.

### 4. Create App Registration with Certificate

Use `Create-AppRegistrationWithCertificate.ps1` from [cloud-admin-toolkit](https://github.com/rbrands/cloud-admin-toolkit):
```powershell
.\Create-AppRegistrationWithCertificate.ps1 -ConfigName <project-name>
```

This creates the Entra ID App Registration for user authentication and generates a self-signed certificate.

After the script completes, copy the **Application (client) ID** from the output and set it in `config.ps1`:
```powershell
ClientId = "<application-client-id>"
```

Configure Redirect URIs in Entra ID:

- Azure Portal: Entra ID → App registrations → your app → Authentication → Add a platform → Web
- Add each environment URI that can receive sign-in callbacks:
  - `https://localhost:7000/signin-oidc`
  - `https://app-training-architect-dev.azurewebsites.net/signin-oidc`
  - `https://app-training-architect-staging.azurewebsites.net/signin-oidc`
  - `https://app-training-architect.azurewebsites.net/signin-oidc`
  - `https://training-architect.com/signin-oidc` (optional, custom domain)

Only keep URIs for environments you actually use. Unused redirect URIs should be removed.

### 5. Configure and Run Setup Script
```powershell
# Copy and fill in all values
cp config.example.ps1 config.ps1

# Set dotnet user-secrets, GitHub Secrets, and generate
# main.local.bicepparam in one step
.\setup.ps1 -All
```

Minimum SEO-related config values in `config.ps1`:

- `SiteUrl = "https://www.example.com"`
- `Author = "__AUTHOR__"`

Foundry-related config values in `config.ps1`:

- `FoundryProjectEndpoint = "__FOUNDRY_PROJECT_ENDPOINT__"`
- `FoundryProjectAgentName = "__FOUNDRY_PROJECT_AGENT_NAME__"`

### 6. CI/CD flow (after first push)

```bash
git push origin main
```

Manual local infrastructure deployment (without GitHub Actions):

```bash
az deployment sub create \
  --name ta-local-$(date +%Y%m%d%H%M%S) \
  --location germanywestcentral \
  --template-file infra/main.bicep \
  --parameters infra/main.local.bicepparam
```

Important: pass the parameter file exactly as shown above (without `@` before the path).

deploy-infrastructure.yml creates:

- Key Vault
- Cosmos DB account, database, container
- RBAC assignments for the existing shared Storage Account
- App Service Plan + Web App
- All RBAC role assignments (Managed Identity)

For multi-resource-group installations with shared central resources, the `images` blob container must already exist in the shared Storage Account.

Application deployment flow:

- Pull request to `main` deploys app changes to slot `dev`.
- Push to `main` deploys app changes to slot `staging`.
- Production is promoted only via manual workflow `swap-staging-to-production.yml` (swap `staging` -> `production`).

This makes `staging` the only release source for `production`.

### 7. Upload Certificate to Key Vault (one-time manual step)

After the first infrastructure deployment, upload the certificate:
Azure Portal → <key-vault-name> → Certificates → Generate/Import → Import
→ Upload the .pem file generated by Create-AppRegistrationWithCertificate.ps1
→ Certificate name must match CERT_NAME in config.ps1

This is the only step that cannot be automated — storing the private key in CI/CD would defeat the purpose of using Key Vault.

### 7a. Set Key Vault Secrets

Assign the **Key Vault Secrets Officer** role to your account using `Set-KeyVaultRoleAssignment.ps1` from cloud-admin-toolkit, then run:

```powershell
.\setup.ps1 -KeyVault
```

Secrets stored in Key Vault:

| Secret name | Maps to config key |
|---|---|
| `Syncfusion--LicenseKey` | `Syncfusion:LicenseKey` |

> **Note:** Key Vault secret names use `--` as a separator, which Azure App Configuration maps to `:` in .NET configuration.

### 8. Trigger final deployment

After uploading the certificate, trigger a new deployment:
```bash
git commit --allow-empty -m "chore: trigger deployment after certificate upload"
git push origin main
```

The application is now fully deployed and operational.

---

> **What is manual vs automated:**
>
> | Step | How |
> |---|---|
> | Resource Group | Manual — az group create |
> | Service Principal + OIDC | cloud-admin-toolkit scripts |
> | App Registration + Certificate | cloud-admin-toolkit scripts |
> | All Azure resources (KV, Cosmos, Storage, App Service) | Bicep via GitHub Actions |
> | Certificate upload to Key Vault | Manual — one-time after first deployment |
> | Application deployment | GitHub Actions |
> | Configuration | setup.ps1 + config.ps1 |

## CI/CD

Two GitHub Actions workflows handle automated deployment:

| Workflow | Trigger | What it does |
|---|---|---|
| `deploy-app.yml` | PR to `main` or push to `main` when `src/**` changes | Builds the Blazor app and deploys by slot (`dev` for PRs, `staging` for `main` pushes) |
| `swap-staging-to-production.yml` | Manual (`workflow_dispatch`) | Runs pre-checks on `staging`, swaps `staging` to `production`, then runs post-checks on `production` |
| `deploy-infrastructure.yml` | Push to `main` when `infra/**` changes | Deploys Bicep templates to Azure |

Both workflows use **OIDC Federated Credentials** for authentication — no client secrets are stored in GitHub.

### Deployment Slots

The App Service is provisioned with two deployment slots:
- `dev`
- `staging`

Slot URLs:
- Dev: `https://<APP_NAME>-dev.azurewebsites.net`
- Staging: `https://<APP_NAME>-staging.azurewebsites.net`
- Production: `https://<APP_NAME>.azurewebsites.net` (or your custom domain)

Deployment and promotion flow (recommended):

```bash
# PR to main -> deploy to dev slot
# Push to main -> deploy to staging slot

# Promote staging -> production (manual approval point)
az webapp deployment slot swap \
  --resource-group <APP_RESOURCE_GROUP> \
  --name <APP_NAME> \
  --slot staging \
  --target-slot production
```

There is no `dev -> staging` swap in the standard flow.
`staging` is refreshed by direct deployment from `main`, and only `staging -> production` is swapped.

### Authentication Setup (OIDC)

1. Create a Service Principal using `Create-ServicePrincipalForDeployment.ps1` (see step 2 in [Initial Setup](#initial-setup-once-per-project))
2. Add a Federated Credential to the Service Principal:
   - **Issuer:** `https://token.actions.githubusercontent.com`
   - **Subject:** `repo:{org}/{repo}:ref:refs/heads/main`
   - **Audiences:** `api://AzureADTokenExchange`

See: https://docs.github.com/en/actions/concepts/security/openid-connect

> **How OIDC authentication works in the workflows:**
> The `AZURE_CLIENT_ID` secret identifies *which* Service Principal to authenticate as.
> The Federated Credential on that Service Principal then tells Azure *which* GitHub repo and branch it trusts.
> OIDC replaces the **client secret** — instead of a password, GitHub issues a short-lived signed token that Azure verifies against the Federated Credential. No secret is ever stored.

Scripts for creating the service principal and configuring the federated credential are available in [rbrands/cloud-admin-toolkit](https://github.com/rbrands/cloud-admin-toolkit).

### Required GitHub Secrets

All secrets are set automatically by `setup.ps1 -GitHub` (see [Initial Setup](#initial-setup-once-per-project), step 6). No manual configuration in the GitHub UI is needed.

The following secrets are set:

| Secret | Description |
|---|---|
| `AZURE_CLIENT_ID` | App ID of the deployment service principal |
| `AZURE_SUBSCRIPTION_ID` | Azure Subscription ID |
| `APP_RESOURCE_GROUP` | Resource group name for the App Service |
| `AZURE_RESOURCE_GROUP` | Legacy alias for the App Service resource group |
| `APP_NAME` | Azure Web App name |
| `AZURE_WEBAPP_NAME` | Legacy alias for the Azure Web App name |
| `PLAN_NAME` | App Service Plan name |
| `COSMOS_ACCOUNT_NAME` | Cosmos DB account name |
| `COSMOS_DATABASE_ID` | Database name |
| `COSMOS_CONTAINER_NAME` | Container name |
| `KEY_VAULT_NAME` | Key Vault name |
| `CERT_NAME` | Certificate name in Key Vault |
| `CLIENT_ID` | App Registration Client ID |
| `TENANT_ID` | Entra ID Tenant ID |
| `STORAGE_ACCOUNT_NAME` | Azure Storage Account name |
| `APP_INSIGHTS_NAME` | Application Insights resource name |
| `LOG_ANALYTICS_NAME` | Log Analytics workspace name |
| `FOUNDRY_ACCOUNT_NAME` | Foundry account name used for Managed Identity RBAC assignment (`Foundry User`) |
| `SITE_URL` | Public site URL, e.g. `https://www.example.com` |
| `AUTHOR` | Global author name used for HTML metadata |
| `FOUNDRY_PROJECT_ENDPOINT` | Foundry project endpoint used by the coaching agent integration |
| `FOUNDRY_PROJECT_AGENT_NAME` | Foundry agent name used by the coaching agent integration |

> **Note:** `SYNCFUSION_LICENSE_KEY` is **not** a GitHub Secret. It is stored in Azure Key Vault and loaded at startup via `AddAzureKeyVault()`. Set it with `setup.ps1 -KeyVault`.

### Infrastructure Deployment Runbook (Quick Checklist)

Use this checklist before or during a failed `deploy-infrastructure.yml` run.

1. Confirm workflow context
  - `deploy-infrastructure.yml` deploys at **subscription scope** (`scope: subscription`).
  - Because of this, resource-group-only permissions are not sufficient.
2. Confirm OIDC identity is correct
  - `AZURE_CLIENT_ID`, `TENANT_ID`, and `AZURE_SUBSCRIPTION_ID` must point to the intended deployment service principal and subscription.
3. Confirm minimum RBAC for the deployment principal
  - Required at subscription scope: `Contributor`.
  - Required where RBAC assignments are created by IaC: `User Access Administrator` (or `Owner`) on the relevant scope.
4. Verify role assignments quickly

```bash
az role assignment list --all \
  --query "[?principalId=='<SERVICE_PRINCIPAL_OBJECT_ID>'].{role:roleDefinitionName,scope:scope}" \
  --output table
```

5. If missing, assign subscription-level Contributor

```bash
az role assignment create \
  --assignee-object-id <SERVICE_PRINCIPAL_OBJECT_ID> \
  --assignee-principal-type ServicePrincipal \
  --role Contributor \
  --scope /subscriptions/<AZURE_SUBSCRIPTION_ID>
```

6. Re-run workflow after RBAC propagation
  - Role assignments can take a short time to propagate.
  - Re-run `deploy-infrastructure.yml` after assignment.

#### Known Error Pattern

If GitHub Actions shows this error:

```text
AuthorizationFailed: ... does not have authorization to perform action
'Microsoft.Resources/deployments/validate/action' over scope '/subscriptions/...'
```

Then the deployment identity is missing effective subscription-scope permissions (or propagation is not complete yet).

### Deploying Infrastructure (Bicep)

Infrastructure is defined in `infra/main.bicep`.

**1. Generate the local parameter file** (if not already done via `setup.ps1 -Bicep`):

```powershell
.\setup.ps1 -Bicep
# Generates infra/main.local.bicepparam from config.ps1
```

**2. Deploy to Azure:**

```bash
az deployment sub create \
  --location <azure-region> \
  --template-file infra/main.bicep \
  --parameters infra/main.local.bicepparam
```

This deploys:
- Azure Cosmos DB account, database, and container
- Azure App Service Plan + Web App (.NET 10, Linux)
- All App Service configuration (Entra ID, Cosmos DB endpoint, Key Vault URL)
- Key Vault RBAC — **Key Vault Certificate User** and **Key Vault Secrets User** roles for the Web App Managed Identity
  - Certificate User: reads the authentication certificate
  - Secrets User: reads secrets via `AddAzureKeyVault()` at startup (e.g. `Syncfusion:LicenseKey`)
- Cosmos DB RBAC — **Built-in Data Contributor** role for the Web App Managed Identity

> **Note:** Local developer access is intentionally **not** assigned by Bicep. Assign your own Entra ID user only when needed via `az cosmosdb sql role assignment create` (see [Local Development](#local-development)).

> **Note:** The Key Vault is created automatically by the Bicep template. After the first deployment, upload the authentication certificate to Key Vault manually: **Azure Portal → kv-{name} → Certificates → Generate/Import**. The Key Vault URI is an output of the Bicep deployment and does not need to be configured separately.

### Legal Page

Before going live, fill in the placeholder values in [`src/TrainingArchitect/Components/Pages/Legal.razor`](src/TrainingArchitect/Components/Pages/Legal.razor):

- `__STREET_ADDRESS__` — Street and house number
- `__ZIP_CITY__` — Postal code and city
- `__CONTACT_EMAIL__` — Public contact email address

---

### Custom Domain (optional)

Custom domain support is built into the Bicep templates. When `customDomain` is non-empty, `main.bicep` deploys `modules/custom-domain.bicep`, which:

- Adds an apex hostname binding (ownership verification via A + TXT records)
- Adds a `www` hostname binding with a **free App Service managed certificate** (CNAME-based SSL)

The `www` subdomain gets full HTTPS. For the apex domain, redirect `example.com` → `https://www.example.com` at your DNS registrar (most registrars support this natively). This is the recommended pattern and avoids the complexity of apex domain certificates.

#### Step 1 — Configure DNS at your registrar

| Record type | Name | Value |
|---|---|---|
| `CNAME` | `www` | `<appName>.azurewebsites.net` |
| `TXT` | `asuid.www` | *(domain verification ID — see below)* |
| `A` | `@` (apex) | *(outbound IP of the App Service)* |
| `TXT` | `asuid` | *(domain verification ID — see below)* |

**Get the domain verification ID:**
```
Azure Portal → App Service → Custom domains → Custom domain verification ID
```

**Get the outbound IP address:**
```
Azure Portal → App Service → Properties → Outbound IP addresses (first entry)
```

> **DNS propagation:** Allow 5–30 minutes after adding records before deploying. The Bicep deployment will fail if Azure cannot resolve the domain.

#### Step 2 — Set `SiteUrl` to your custom domain in config.ps1

```powershell
SiteUrl = "https://www.example.com"  # or https://example.com
```

`main.bicep` derives the apex domain from `SiteUrl` automatically: non-`azurewebsites.net` URLs trigger the custom domain deployment.

#### Step 3 — Re-run setup to update secrets and bicepparam

```powershell
.\setup.ps1 -Bicep
```

#### Step 4 — Push to trigger deployment

```bash
git commit --allow-empty -m "chore: deploy custom domain"
git push origin main
```

`deploy-infrastructure.yml` passes `siteUrl=${{ secrets.SITE_URL }}` to Bicep.
`main.bicep` derives the apex domain from `siteUrl` and then the deployment:
1. Adds hostname bindings for apex and www
2. Issues a free managed certificate for `www.{apexDomain}` (CNAME validation)
3. Enables SNI SSL on the www binding via a nested deployment

#### After deployment

Add `https://www.{apexDomain}/signin-oidc` to the **Redirect URIs** in your Entra ID App Registration:
```
Azure Portal → App registrations → your app → Authentication → Add URI
```

#### Required GitHub Secret

No additional secret required — `SiteUrl` is already the single source of truth. The apex domain is derived from it automatically in Bicep.

---

## Local Development

### Prerequisites

**1. Trust the local HTTPS developer certificate** (once per machine):

```bash
dotnet dev-certs https --trust
```

Without this, the OIDC callback over HTTPS will fail with "Correlation failed" in the browser.

**2. Add the local redirect URI to the Entra ID App Registration:**

In the Azure Portal → **App registrations** → your app → **Authentication** → add:

```
https://localhost:7000/signin-oidc
```

**3. Log in with the Azure CLI** (once per session, needed for Key Vault certificate loading and Cosmos DB access via `DefaultAzureCredential`):

```bash
az login
```

**4. Assign your own user for local Cosmos DB data-plane access** (run when your user needs direct container access):

```powershell
# Load project config values
# Ensure config.ps1 exists and all __PLACEHOLDER__ values are replaced first
. .\config.ps1

# Resolve your Entra object ID and set the target scope to one container
$principalId = az ad signed-in-user show --query id -o tsv
$scope = "/dbs/$($config.CosmosDatabaseId)/colls/$($config.CosmosContainerName)"

# Optional but recommended when multiple subscriptions are available
az account set --subscription $config.SubscriptionId

# Cosmos DB Built-in Data Contributor (container-level)
az cosmosdb sql role assignment create `
  --account-name $config.CosmosAccountName `
  --resource-group $config.CentralResourceGroupName `
  --scope $scope `
  --principal-id $principalId `
  --role-definition-id "00000000-0000-0000-0000-000000000002"
```

This keeps local developer assignments explicit and prevents accidental overwrites across projects.

**5. Set local secrets** (once, see [Setup](#setup) above):

```powershell
Copy-Item config.example.ps1 config.ps1
# Edit config.ps1 and replace all __PLACEHOLDER__ values
.\setup.ps1 -Secrets
```

> **Note:** Key Vault is only used in production. Locally, all secrets are set via `dotnet user-secrets` — no Azure Key Vault connection is required during development.

### Running the app

Always use the `https` profile — the OIDC flow requires HTTPS for cookies to work correctly:

```bash
dotnet run --project src/TrainingArchitect --launch-profile https
```

or
```bash
dotnet watch --project src/TrainingArchitect run --launch-profile https --no-restore
```

The host project in `src/TrainingArchitect` is the only supported startup target. The client project is loaded by the host and should not be started directly.

The app starts at `https://localhost:7000`.

> **Note:** Running with `--launch-profile http` (plain HTTP) will cause "Correlation failed" on the login callback because secure cookies cannot be set over HTTP.

---

## Versioning

This repository uses **Semantic Versioning** (`MAJOR.MINOR.PATCH`) and tracks releases in [CHANGELOG.md](CHANGELOG.md).

- Source of truth for release history: `CHANGELOG.md`
- Source of truth for runtime UI version display: `src/TrainingArchitect.Core/Models/ApplicationVersionInfo.cs`

When creating a new release:

1. Update `ApplicationVersionInfo.Current` with the new semantic version and release date.
2. Add a matching entry in `CHANGELOG.md` using the same version and date.

---

## License

MIT — see [LICENSE](LICENSE)
---

Maintained by  
**Robert Brands**  
Freelance IT Consultant | Solution Architect | Cloud Adoption & GenAI