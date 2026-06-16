# Copilot Instructions for brands-advisory-cms

## Language
- All code, comments, variable names, method names, 
  and class names must be in English
- XML documentation comments must be in English
- Git commit messages must be in English
- README and technical documentation must be in English
- Prompts may be written in German or English - 
  always respond and generate code in English regardless

## Security
- Never put real secrets, keys, passwords, or tokens in code
- All sensitive configuration values must use placeholders 
  in the format __PLACEHOLDER_NAME__
- Real values belong in appsettings.Development.json (gitignored),
  Azure App Service Configuration, or GitHub Secrets
- Never hardcode OIDs, Tenant IDs, Client IDs, or 
  Cosmos DB connection strings

## Code Style
- Follow standard C# naming conventions (PascalCase for 
  public members, camelCase for private fields with _ prefix)
- Use file-scoped namespaces
- Use primary constructors where appropriate (.NET 10)
- Prefer async/await throughout
- Keep Blazor components focused and small
- Separate concerns: UI in Blazor project, 
  business logic and models in Core project

## Architecture
- Default render mode is Static SSR
- Only use @rendermode InteractiveWebAssembly for 
  edit components that require it
- Owner-only features must always check IOwnerService,
  never rely on UI visibility alone
- All data access goes through ICosmosDbService
- Cosmos DB access uses Managed Identity via 
  DefaultAzureCredential - never use primary keys

## Documentation
- Keep README.md up to date when adding new features, 
  configuration values, or deployment steps
- Add XML doc comments to all public interfaces and methods
- Document any new __PLACEHOLDER__ values in README.md 
  under the Setup section

## Dependencies
- Syncfusion Community License is in use - 
  only use Syncfusion.Blazor.* packages already referenced
- Do not add new NuGet packages without explicit request

## Local Development Secrets
- Use dotnet user-secrets for all local development secrets
- Never use appsettings.Development.json for secrets
- A set-secrets.sh.example file in the repository root 
  documents which secrets are required
- Run `dotnet user-secrets init` is already configured 
  in the .csproj with UserSecretsId "brands-advisory-cms"
- Set secrets with: 
  dotnet user-secrets set "Section:Key" "value"
  from the src/BrandsAdvisory/ directory
  