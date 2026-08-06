# CLAUDE.md — training-architect

## Language
- All code, comments, variable names, method names, and class names must be in English
- XML documentation comments must be in English
- Git commit messages must be in English
- README and technical documentation must be in English
- Prompts may be written in German or English — always respond and generate code in English regardless

## Build & Run

```bash
# Run locally (always use https profile — OIDC requires HTTPS)
dotnet run --project src/TrainingArchitect --launch-profile https
# App starts at https://localhost:7000

# Build solution
dotnet build training-architect.slnx
```

The host project in `src/TrainingArchitect` is the only supported startup target.

Prerequisites for local development:
- `dotnet dev-certs https --trust` (once per machine)
- `az login` (once per session — needed for DefaultAzureCredential)
- Secrets set via `dotnet user-secrets` (never appsettings.Development.json)

## Security
- Never put real secrets, keys, passwords, or tokens in code
- All sensitive configuration values must use placeholders in the format `__PLACEHOLDER_NAME__`
- Real values belong in `dotnet user-secrets`, Azure App Service Configuration, or GitHub Secrets
- Never hardcode OIDs, Tenant IDs, Client IDs, or Cosmos DB connection strings
- Cosmos DB access uses Managed Identity via `DefaultAzureCredential` — never primary keys
- Key Vault is only used in production; locally all secrets go through `dotnet user-secrets`

## Code Style
- Follow standard C# naming conventions (PascalCase for public members, `_camelCase` for private fields)
- Use file-scoped namespaces
- Use primary constructors where appropriate (.NET 10)
- Prefer `async`/`await` throughout
- Keep Blazor components focused and small
- Separate concerns: UI in Blazor projects, business logic and models in Core project

## Architecture

### Project Layout
```
src/
├── TrainingArchitect/             # Blazor Web App host — Static SSR + minimal API endpoints
├── TrainingArchitect.Client/      # Blazor WebAssembly — admin pages (InteractiveWebAssembly)
├── TrainingArchitect.Core/        # Domain layer — interfaces, models, no infrastructure deps
└── TrainingArchitect.Infrastructure/  # Data access — Cosmos DB repositories
```

### Render Mode Rules
- **Static SSR** is the default for all public pages
- **InteractiveWebAssembly** only for components that require client-side reactivity:
  - `/coach` — coaching chat (IChatService)
  - `<PlanConfirmation>` island inside the otherwise-SSR `/training-plan` page
- Never use `@rendermode InteractiveServer`

### Authorization
- Owner-only features must always check `IOwnerService` server-side
- Never rely on UI visibility alone for access control
- The `SiteAdmin` App Role (Entra ID) is the single source of truth for owner access

### Data Access
- All data access goes through repository interfaces in `TrainingArchitect.Core/Interfaces/`
- Cosmos DB repositories live in `TrainingArchitect.Infrastructure/`
- Admin API endpoints in `TrainingArchitect/Endpoints/` are protected by the `SiteAdmin` role

### Training Architect Stubs
The `/coach` and `/training-plan` pages are scaffolded with stub implementations:

| Interface | Stub | Replace with |
|---|---|---|
| `IChatService` | `StubChatService` | Real orchestration pipeline (e.g. Microsoft.Extensions.AI / Semantic Kernel) |
| `IAuthContext` | `StubAuthContext` | OIDC subject claim from `IHttpContextAccessor`; resolve `AthleteTier` server-side |
| `IIntervalsDataProvider` | `StubIntervalsDataProvider` | intervals.icu REST API with athlete's API key from Key Vault |

Register replacements in `TrainingArchitect/Program.cs` (server) and `TrainingArchitect.Client/Program.cs` (WASM).

## Dependencies
- Syncfusion Community License is in use — only use `Syncfusion.Blazor.*` packages already referenced
- Do not add new NuGet packages without explicit request

## Documentation
- Keep README.md up to date when adding new features, configuration values, or deployment steps
- Add XML doc comments to all public interfaces and methods
- Document any new `__PLACEHOLDER__` values in README.md under the Setup section
