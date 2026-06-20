# Basic Pdf Viewer Sample (foundation)

> This is the **only** file that renders the page layout. All features patch into this sample.

## Prerequisites and Setup Requirements

Before using the Blazor PDF Viewer component, ensure the following setup is complete:

- **Note**: Need to check and alter the required changes alone in the proj files. Don't add any content by assemption. Before apply the changes in the necessary file need to confirm with user. 

### 1. NuGet Package Installation

If using an interactive render mode such as WebAssembly or Auto, ensure the required .NET workloads are installed for SkiaSharp usage in a Blazor Web App. Run the following command:

```
dotnet workload install wasm-tools

dotnet workload install wasm-tools-net8 (For .NET 8.0) or dotnet workload install wasm-tools-net9 (For .NET 9.0) or dotnet workload install wasm-tools-net10 (For .NET 10.0)

```

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

Need to add the below line for 

For **Blazor Web App (Server mode) & Auto**:

```csharp
using Syncfusion.Blazor;

.....

builder.Services.AddSignalR(o => { o.MaximumReceiveMessageSize = 102400000; });

builder.Services.AddMemoryCache();
//Add Syncfusion Blazor service to the container.
builder.Services.AddSyncfusionBlazor();

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
    <!-- Modify the HeadOutlet with the necessary project type render mode -->
    <HeadOutlet @rendermode="{Set the render Mode from the below table}" />
    <!-- Other head content -->
    <!-- Syncfusion Blazor PDF Viewer control's theme style sheet -->
    <link href="_content/Syncfusion.Blazor.Themes/bootstrap5.css" rel="stylesheet" />
</head>

<body>
    <!-- Routes the HeadOutlet with the necessary project type render mode -->
    <Routes @rendermode="{Set the render Mode from the below table}" />
    <!-- Other body content -->
    <!-- Syncfusion Blazor PDF Viewer control's scripts -->
    <script src="_content/Syncfusion.Blazor.SfPdfViewer/scripts/syncfusion-blazor-sfpdfviewer.min.js" type="text/javascript"></script>
</body>
```


| RenderMode | Code |
|-----|-----|
| Auto | InteractiveAuto |
| WebAssembly | InteractiveWebAssembly |
| Server| InteractiveServer |

---

## Basic PDF Viewer Component

```csharp
@page "/pdfviewer-sample"
@using Syncfusion.Blazor
@using Syncfusion.Blazor.SfPdfViewer
@using Syncfusion.Blazor.Buttons

<!-- BASE_PATCH:UI_BUTTONS -->
<!-- Buttons from feature files will be inserted here -->

<SfPdfViewer2 @ref="_pdfviewer"
               DocumentPath="https://cdn.syncfusion.com/content/pdf/pdf-succinctly.pdf"
               Height="100%"
               Width="100%">
               <!-- "PROPERTY" should be placed here -->
               <!-- "EVENTS" should be placed here-->
</SfPdfViewer2>

@code {
    private SfPdfViewer2 _pdfviewer { get; set; }
}
```

### ⚠️ Important Notes

- If the `DocumentPath`, `Height`, `Width` property were already assigned in the modified file, then use the same property values in the code snippet.
- When loading PDF files from the local wwwroot folder, you MUST use the following path format:

```
DocumentPath="wwwroot/path-to-file/file-name.pdf"
```

**Examples:**
- ✅ **CORRECT**: `DocumentPath="wwwroot/test.pdf"` (file in wwwroot root)
- ❌ **WRONG**: `DocumentPath="test.pdf"` (missing wwwroot prefix)
