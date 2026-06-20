# Blazor DocumentEditor WASM Setup

Complete setup for DocumentEditor in Blazor WebAssembly apps; includes NuGet installation, service registration, resource linking, and component initialization.

## Install NuGet Packages

Install the required Syncfusion packages for DocumentEditor functionality.

```powershell
Install-Package Syncfusion.Blazor.WordProcessor
Install-Package Syncfusion.Blazor.Themes
```

Or via .NET CLI:

```bash
dotnet add package Syncfusion.Blazor.WordProcessor
dotnet add package Syncfusion.Blazor.Themes
dotnet restore
```

## Register Syncfusion Service

Import namespaces in `_Imports.razor`:

```razor
@using Syncfusion.Blazor
@using Syncfusion.Blazor.DocumentEditor
```

Register the Syncfusion service in `Program.cs`:

```csharp
using Syncfusion.Blazor;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

// Register Syncfusion Blazor service
builder.Services.AddSyncfusionBlazor();

await builder.Build().RunAsync();
```

## Add Stylesheet and Script Resources

Add theme stylesheet and DocumentEditor script to the `<head>` section of `index.html`:

```html
<head>
     <!-- Syncfusion Blazor DocumentEditor control's theme style sheet-->
    <link href="_content/Syncfusion.Blazor.Themes/bootstrap5.css" rel="stylesheet" />
    <!-- Syncfusion Blazor DocumentEditor control's scripts -->
    <script src="_content/Syncfusion.Blazor.WordProcessor/scripts/syncfusion-blazor-documenteditor.min.js" type="text/javascript"></script>
</head>
```

**Placeholders**
- `bootstrap5.css` → Replace with `{themeName}` (e.g., `fluent2.css`, `fabric.css`, `tailwind.css`, `material.css`)

Themes are available via Static Web Assets, CDN, or Custom Resource Generator (CRG).

## Add DocumentEditor Component

Add the component to your page (e.g., `Pages/Index.razor`):

```razor
<SfDocumentEditorContainer EnableToolbar=true></SfDocumentEditorContainer>
```

**Placeholders**
- `Pages/Index.razor` → Replace with `{targetPagePath}`
- `EnableToolbar=true` → Optional; set to `false` or omit for editor without toolbar

## Dependencies

Syncfusion auto-installs dependencies: Syncfusion.Blazor.Calendars, Syncfusion.Blazor.Core, Syncfusion.Blazor.Data, Syncfusion.Blazor.DropDowns, Syncfusion.Blazor.Navigations, Syncfusion.WordProcessor.AspNet.Core, System.Text.Json. Initial page load may take up to 6 seconds for browser to load all dependency packages.

## File-based Changes

- `_Imports.razor` → Add Syncfusion namespace imports
- `Program.cs` → Register `AddSyncfusionBlazor()` service
- `index.html` → Add stylesheet and script references in `<head>`
- `Pages/Index.razor` (or your target page) → Add `<SfDocumentEditorContainer>` component
