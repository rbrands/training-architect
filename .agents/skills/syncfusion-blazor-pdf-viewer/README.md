# Syncfusion Blazor PDF Viewer Skills

Create Blazor application by integrating Syncfusion Blazor PDF Viewer control for viewing and editing PDF documents.

See **[SKILL.md](SKILL.md)** for the full intent-routing guide and rules.

---

## Two Modes

### Generate C# Code for the User's Project *(default)*

Produces production-ready C# code and adds it directly into the user's project files.

**Trigger keywords:** "code", "snippet", "how to write", "Program.cs", "show me", "sample", "example code", "generate code for".

---

## Quick Start

## Prerequisites and Setup Requirements

Before using the Blazor PDF Viewer component, ensure the following setup is complete:

### 1. NuGet Package Installation

Required packages in your `.csproj` file:

```
dotnet add package Syncfusion.Blazor.SfPdfViewer
dotnet add package Syncfusion.Blazor.Themes
```

- If the project were `Web Assembly` the install the below package too

```
dotnet add package SkiaSharp.Views.Blazor
```

### 2. Program.cs Configuration

For **Blazor Web App (Server mode) & Auto**:

```csharp
using Syncfusion.Blazor;

.....

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Configure SignalR for large file uploads
builder.Services.AddSignalR(o => { o.MaximumReceiveMessageSize = 102400000; });

// Add memory cache
builder.Services.AddMemoryCache();

.......

```

For **Blazor WebAssembly**:

```csharp
using Syncfusion.Blazor;

.....

builder.Services.AddMemoryCache();
//Add Syncfusion Blazor service to the container
builder.Services.AddSyncfusionBlazor();

.....

```

For **Blazor WebAssembly or Auto mode**, register the service in both the server and client `Program.cs` files.

### 3. _Imports.razor Configuration

Add these namespaces to `Components/_Imports.razor` (Web App) or `_Imports.razor` (WebAssembly):

```cshtml
@using Syncfusion.Blazor
@using Syncfusion.Blazor.SfPdfViewer
```

### 4. App.razor Configuration

Add the Syncfusion stylesheet and script references in `Components/App.razor`:

```html
<head>
    <!-- Other head content -->
    <!-- Syncfusion Blazor PDF Viewer control's theme style sheet -->
    <link href="_content/Syncfusion.Blazor.Themes/bootstrap5.css" rel="stylesheet" />
</head>

<body>
    <!-- Other body content -->
    <!-- Syncfusion Blazor PDF Viewer control's scripts -->
    <script src="_content/Syncfusion.Blazor.SfPdfViewer/scripts/syncfusion-blazor-sfpdfviewer.min.js" type="text/javascript"></script>
</body>
```


| RenderMode | Code |
|-----|-----|
| Auto | @rendermode InteractiveAuto |
| WebAssembly | @rendermode InteractiveWebAssembly |
| Server| @rendermode InteractiveServer |

---

## Code References

All templates and snippets used by the skill are in the `references/` folder:

| File | Contents |
| --- | --- |
| **getting-started.md** | Foundation setup and minimal PDF Viewer component. |
| **toolbar-properties.md** | Comprehensive toolbar customization using PdfViewerToolbarSettings for Pdfviewer. |
| **toolbar-methods.md** | Programmatic control methods for toolbar visibility and behavior for Pdfviewer. |
| **navigation-toolbar-settings.md** | Settings for configuring the navigation toolbar, including related properties and code examples. |
| **magnification.md** | Programmatic zoom/magnification control for the Pdfviewer. |
| **text-search.md** | Complete text search functionality for PDF document content discovery and navigation for Pdfviewer. |
| **annotation-events.md** | Comprehensive reference for annotation lifecycle, interaction, and signature-specific events for Pdfviewer. |
| **signature-annotation-events.md** | Events fired when users interact with signature annotations (add, select, move, resize, modify, remove). |
| **annotation-properties.md** | Reference for enabling/disabling annotation feature types via properties for Pdfviewer. |
| **annotation-selector-settings.md** | Properties for configuring the default selection appearance for all annotation types in the PDF Viewer. |
| **annotation-settings.md** | Configuration for default appearance, behavior, size constraints, and restrictions across 20+ annotation types for Pdfviewer. |
| **annotation-methods.md** | Programmatic API methods for controlling annotation interactions within a PDF document. |
| **area-annotation-settings.md** | Properties for configuring default appearance, behavior, and restrictions for area annotations. |
| **arrow-annotation-settings.md** | Properties for configuring default appearance, behavior, and restrictions for arrow annotations. |
| **circle-annotation-settings.md** | Properties for configuring default appearance, behavior, and restrictions for circle annotations. |
| **custom-stamp-annotation-settings.md** | Properties for configuring default appearance, behavior, and restrictions for custom stamp/image annotations. |
| **distance-annotation-settings.md** | Properties for configuring default appearance, behavior, and restrictions for distance measurement annotations. |
| **free-text-annotation-settings.md** | Properties for configuring default appearance, behavior, and restrictions for free text annotations. |
| **handwritten-signature-settings.md** | Properties for configuring default appearance, behavior, and restrictions for handwritten signature annotations. |
| **highlight-annotation-settings.md** | Properties for configuring default appearance, behavior, and restrictions for highlight annotations. |
| **ink-annotation-settings.md** | Properties for configuring default appearance, behavior, and restrictions for ink (freehand drawing) annotations. |
| **line-annotation-settings.md** | Properties for configuring default appearance, behavior, and restrictions for line annotations. |
| **measurement-annotations-settings.md** | Properties for configuring default units and ratio values for measurement annotations. |
| **perimeter-annotation-settings.md** | Properties for configuring default appearance, behavior, and restrictions for perimeter measurement annotations. |
| **polygon-annotation-settings.md** | Properties for configuring default appearance, behavior, and restrictions for polygon annotations. |
| **radius-annotation-settings.md** | Properties for configuring default appearance, behavior, and restrictions for radius measurement annotations. |
| **rectangle-annotation-settings.md** | Properties for configuring default appearance, behavior, and restrictions for rectangle annotations. |
| **squiggly-annotation-settings.md** | Properties for configuring default appearance, behavior, and restrictions for squiggly (wavy underline) annotations. |
| **stamp-annotation-settings.md** | Properties for configuring default appearance, behavior, and restrictions for built-in stamp/image annotations. |
| **stickynotes-annotation-settings.md** | Properties for configuring default appearance, behavior, and restrictions for sticky note annotations. |
| **strikethrough-annotation-settings.md** | Properties for configuring default appearance, behavior, and restrictions for strikethrough annotations. |
| **underline-annotation-settings.md** | Properties for configuring default appearance, behavior, and restrictions for underline annotations. |
| **volume-annotation-settings.md** | Properties for configuring default appearance, behavior, and restrictions for volume measurement annotations. |
| **redaction-settings.md** | Complete configuration API for redaction settings to permanently remove sensitive information from PDFs. |
| **programmatical-annotation-methods.md** | Complete API for programmatic annotation lifecycle management, redaction, and export/import workflows. |
| **command-manager.md** | Dual feature reference for custom keyboard shortcuts and comment panel customization. |
| **command-panel.md** | Comment panel feature configuration, including visibility control, module injection, and related settings. |
| **document-properties.md** | Document-level configuration, metadata, and lifecycle events for PDF handling. |
| **file-save-properties.md** | File download and annotation export/import functionality with complete event-driven workflows. |
| **ui-properties.md** | Core UI properties and hyperlink interaction events for customizing the viewer experience. |
| **form-field-properties.md** | Core form field configuration and design mode control properties for Pdfviewer. |
| **form-field-settings.md** | Visual appearance and styling configuration for form fields with properties and comprehensive decision guidance. |
| **form-designer-events.md** | Comprehensive form field event reference for lifecycle, interaction, validation, and data management for Pdfviewer. |
| **programmatical-form-field-methods.md** | Complete API for programmatic form field lifecycle management, batch operations, and export/import workflows. |
| **thumbnail-bookmark.md** | Comprehensive reference for programmatic navigation, text selection, and thumbnail/bookmark panel management. |
| **contextmenu-settings.md** | When the user needs to customize the PDF Viewer's as context menu settings. |
| **undo-redo-properties.md** | Comprehensive undo/redo functionality for tracking and reversing document changes for Pdfviewer. |
| **navigation-methods.md** | Programmatic page navigation control and document information retrieval for Pdfviewer. |
| **navigation-properties.md** | Properties for page state tracking, navigation toolbar customization, and UI synchronization in the PDF viewer. |
| **page-organizer.md** | Complete API for programmatic page manipulation, document structure editing, and page organizer UI configuration. |
| **page-organizer-settings.md** | Settings for page organization features such as deletion, insertion, rotation, duplication, and other page manipulation capabilities. |
| **print.md** | Print capabilities configuration, quality control, and event-driven workflows for PDF documents. |
| **text-selection-and-extraction.md** | Describes about the funtionlities of the text selection and text extraction. |
| **tile-rendering.md** | Tile rendering settings, including properties for enabling tile rendering and configuring tile coordinates. |
| **pdfviewer-methods.md** | All core methods available on the SfPdfViewer component for controlling lifecycle and document operations. |
| **pdfviewer-properties.md** | General properties of the SfPdfViewer component covering UI appearance, interaction mode, font support, and document configuration. |
| **azure-container-deployment.md** | Comprehensive guide for deploying Blazor PDF Viewer applications to Azure using Docker containers, Azure Container Registry (ACR), and Azure App Service. |
| **annotation-enum-properties.md** | Enum definitions for annotation types |
| **annotation-general-enum-properties.md** | General annotation enum properties (e.g., `Bound` type) |
| **annotation-settings-enum-properties.md** | Enum properties for annotation settings |
| **form-fields-enum-properties.md** | Enum properties for form fields |
| **general-enum-properties.md** | General enums (`InteractionMode`, `ZoomMode`, `ContextMenuAction`, etc.) |
| **keyboard-enum-properties.md** | Keyboard shortcut enum properties |
| **pdf-annotation-enum-properties.md** | PDF-specific annotation enums (e.g., `AllowedInteraction`) |
| **toolbar-enum-properties.md** | Toolbar item enums (`ToolbarItem`, `AnnotationToolbarItem`, etc.) |

### Example Prompts

#### Mode 1 / Code-Generation Prompts (C# snippets for your project)

*Use these when you want C# code to embed in your project (e.g., Program.cs).*

**Examples:**

- "Show me the C# code to create a Pdfviewer with the hide the default toolbar."
- "Generate a C# snippet to add the annotations programmatically for Pdfviewer."
- "Provide C# code to customize the UI for the search functionalities for Pdfviewer."
- "Show me how to set the formfieldsettings for the Pdfviewer"

## License

Syncfusion .NET PDF library requires a commercial license for production use. A [free community license](https://www.syncfusion.com/products/communitylicense) is available for qualifying organizations.