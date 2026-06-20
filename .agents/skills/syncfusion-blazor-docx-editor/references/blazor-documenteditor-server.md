# Blazor DocumentEditor Server Setup

Step-by-step configuration to add Syncfusion DocumentEditor to a Blazor Server application, from NuGet installation through component rendering and document loading.

## Install NuGet Packages

Add the required packages to enable DocumentEditor functionality and styling:

```powershell
dotnet add package Syncfusion.Blazor.WordProcessor
dotnet add package Syncfusion.Blazor.Themes
dotnet restore
```

## Register Namespaces

Import the Syncfusion namespaces in `_Imports.razor`:

```razor
@using Syncfusion.Blazor
@using Syncfusion.Blazor.DocumentEditor
```

## Configure Services

Register Syncfusion Blazor service in `Program.cs` with SignalR hub configuration for large document transfers:

```csharp
using Syncfusion.Blazor;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor().AddHubOptions(o => 
{ 
    o.MaximumReceiveMessageSize = 102400000; 
});

builder.Services.AddSyncfusionBlazor();

var app = builder.Build();
```

**Placeholders**
- `102400000` → Replace with {maxMessageSizeBytes} (currently 100MB; increase for larger documents)

## Add Theme and Script Resources

Add stylesheet and script references to the `<head>` section:

- For **.NET 6**: add to `Pages/_Layout.cshtml`
- For **.NET 8+**: add to `Pages/_Host.cshtml`

```html
<head>
    <!-- Syncfusion Blazor DocumentEditor control's theme style sheet -->
    <link href="_content/Syncfusion.Blazor.Themes/bootstrap5.css" rel="stylesheet" />
    <!-- Syncfusion Blazor DocumentEditor control's scripts -->
    <script src="_content/Syncfusion.Blazor.WordProcessor/scripts/syncfusion-blazor-documenteditor.min.js" type="text/javascript"></script>
</head>
```

**Placeholders**
- `bootstrap5` → Replace with preferred theme (e.g., `fluent-2`, `material-3`)

## Add DocumentEditor Component (Default)

Render the component in a Razor page (e.g., `Pages/Index.razor`):

```razor
<SfDocumentEditorContainer EnableToolbar="true"></SfDocumentEditorContainer>
```

**Notes**: By default, `SfDocumentEditorContainer` creates an internal `SfDocumentEditor` instance. To hook into DocumentEditor events directly, set `UseDefaultEditor="false"` and define your own `SfDocumentEditor` component with event handlers.

## Load Existing Documents

To open a Word document on component initialization, use the `Created` event:

```razor
@using System.IO;
@using Syncfusion.Blazor.DocumentEditor;

<SfDocumentEditorContainer @ref="container" EnableToolbar="true">
    <DocumentEditorContainerEvents Created="OnCreated"></DocumentEditorContainerEvents>
</SfDocumentEditorContainer>

@code {
    SfDocumentEditorContainer container;

    private void OnCreated(object args)
    {
        string filePath = "wwwroot/data/GettingStarted.docx";
        using (FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
        {
            // Load Word document from file
            WordDocument document = WordDocument.Load(fileStream, ImportFormatType.Docx);
            // Convert to JSON format for editor
            string json = JsonSerializer.Serialize(document);
            // Free resources immediately
            document.Dispose();
            document = null;
            
            // Get editor instance and open document
            SfDocumentEditor editor = container.DocumentEditor;
            editor.OpenAsync(json);
            json = null;
        }
    }
}
```

**Placeholders**
- `wwwroot/data/GettingStarted.docx` → Replace with {documentPath}
- `GettingStarted` → Replace with {documentName}

## File-based Changes

- `_Imports.razor` → add Syncfusion namespaces
- `Program.cs` → add service registration with hub options
- `Pages/_Layout.cshtml` or `Pages/_Host.cshtml` → add theme stylesheet and script
- `wwwroot/data/` → place Word documents to load
- `Pages/Index.razor` (or target page) → add component and load handler
