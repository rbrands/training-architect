# Blazor DocumentEditor WebApp Setup

Getting a Blazor Web App ready to use Document Editor requires service registration, namespace imports, and theme/script references.

## Install NuGet Packages

Required packages for Document Editor functionality and theming.

```powershell
dotnet add package Syncfusion.Blazor.WordProcessor
dotnet add package Syncfusion.Blazor.Themes
dotnet restore
```

## Register Syncfusion Service

Add service registration in `Program.cs` to enable Syncfusion components across the application.

```csharp
using Syncfusion.Blazor;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddInteractiveWebAssemblyComponents();
builder.Services.AddSyncfusionBlazor();

var app = builder.Build();
```

**Note:** For `WebAssembly` or `Auto` render modes, register the service in both the server and client project `Program.cs` files.

## Import Namespaces

Add required namespaces to `_Imports.razor` for component access.

```csharp
@using Syncfusion.Blazor
@using Syncfusion.Blazor.DocumentEditor
```

## Add Stylesheet and Script Resources

Include theme and component script in `~/Components/App.razor` `<head>` and `<body>` sections.

```html
<head>
     <!-- Syncfusion Blazor DocumentEditor control's theme style sheet-->
    <link href="_content/Syncfusion.Blazor.Themes/bootstrap5.css" rel="stylesheet" />
</head>
<body>
    <!-- Syncfusion Blazor DocumentEditor control's scripts -->
    <script src="_content/Syncfusion.Blazor.WordProcessor/scripts/syncfusion-blazor-documenteditor.min.js" type="text/javascript"></script>
</body>
```

**Placeholders**
- `bootstrap5.css` → Replace with `{themeName}.css` (e.g., `material3.css`, `fabric.css`)

## Add Document Editor Component

Initialize the Document Editor container with toolbar enabled in a `.razor` file.

```razor
@rendermode InteractiveAuto

<SfDocumentEditorContainer EnableToolbar=true></SfDocumentEditorContainer>
```

**Note:** For per-page/component interactivity, define the render mode directive. Set `@rendermode InteractiveAuto`, `@rendermode InteractiveServer`, or `@rendermode InteractiveWebAssembly` based on your configuration.


## Load Existing Document

Open a Word document by converting it to SFDT format during initialization using the `Created` event.

```razor
@using System.IO;
@using Syncfusion.Blazor.DocumentEditor;

<SfDocumentEditorContainer @ref="container" EnableToolbar=true>
    <DocumentEditorContainerEvents Created="OnCreated"></DocumentEditorContainerEvents>
</SfDocumentEditorContainer>

@code {
    SfDocumentEditorContainer container;

    public void OnCreated(object args)
    {
        string filePath = "wwwroot/data/GettingStarted.docx";
        using (FileStream fileStream = new FileStream(filePath, System.IO.FileMode.Open, System.IO.FileAccess.Read))
        {
            // Load .docx and convert to SFDT JSON format
            WordDocument document = WordDocument.Load(fileStream, ImportFormatType.Docx);
            string json = JsonSerializer.Serialize(document);
            document.Dispose();
            document = null;  // null out reference to allow garbage collection
            
            // Open SFDT content in editor
            SfDocumentEditor editor = container.DocumentEditor;
            editor.OpenAsync(json);
            json = null;  // null out reference to free memory
        }
    }
}
```


## Rules
1. The documentation contains two types of initialization:
   - Basic Document Editor initialization
   - Document Editor initialization with loading an existing Word document

2. Determine the user's intent from the input:
   - If the user asks for "basic setup", "simple initialization", "toolbar only",
     or does NOT mention loading a document → show ONLY the basic initialization.
   - If the user asks for "load existing document", "open Word file", "SFDT",
     "Created event", or "docx loading" → show ONLY the document-loading initialization.

3. Do NOT display both sections together.
4. Do NOT summarize or explain unless explicitly asked.
5. Display the selected section exactly as it appears in the Markdown,
   preserving code blocks and formatting.
``

**Placeholders**
- `wwwroot/data/GettingStarted.docx` → Replace with `{documentPath}`

## File Changes Summary

- `Program.cs` → Add `AddSyncfusionBlazor()` service registration
- `_Imports.razor` → Import `Syncfusion.Blazor` and `Syncfusion.Blazor.DocumentEditor` namespaces
- `Components/App.razor` → Add stylesheet link in `<head>` and script in `<body>`
- `Pages/*.razor` → Add `SfDocumentEditorContainer` component with render mode if using per-page interactivity
