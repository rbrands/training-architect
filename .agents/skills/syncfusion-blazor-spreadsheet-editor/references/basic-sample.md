# Basic Spreadsheet Sample (foundation)

> This is the **only** file that renders the page layout. All features patch into this sample.

## Prerequisites and Setup Requirements

Before using the Blazor Spreadsheet component, ensure the following setup is complete:

### 1. NuGet Package Installation

Required packages in your `.csproj` file:

```
dotnet add package Syncfusion.Blazor.Spreadsheet
dotnet add package Syncfusion.Blazor.Themes
```

### 2. Program.cs Configuration

For **Blazor Web App (Server mode) & Auto**:

```csharp
using Syncfusion.Blazor;

.....

builder.Services.AddSyncfusionBlazor();
.......

```

For **Blazor WebAssembly**:

```csharp
using Syncfusion.Blazor;

.....
builder.Services.AddSyncfusionBlazor();

.....

```

For **Blazor WebAssembly or Auto mode**, register the service in both the server and client `Program.cs` files.

### 3. _Imports.razor Configuration

Add these namespaces to `Components/_Imports.razor` (Web App) or `_Imports.razor` (WebAssembly):

```cshtml
@using Syncfusion.Blazor.Spreadsheet
```

### 4. App.razor Configuration

Add the Syncfusion stylesheet and script references in `Components/App.razor`:

```html
<head>
    <!-- Other head content -->
    <!-- Syncfusion Blazor Spreadsheet control's theme style sheet -->
    <link href="_content/Syncfusion.Blazor.Themes/bootstrap5.css" rel="stylesheet" />
</head>

<body>
    <!-- Other body content -->
    <!-- Syncfusion Blazor Spreadsheet control's scripts -->
    <script src="_content/Syncfusion.Blazor.Spreadsheet/scripts/syncfusion-blazor-spreadsheet.min.js" type="text/javascript"></script>
</body>
```

| RenderMode | Code |
|-----|-----|
| Auto | @rendermode InteractiveAuto |
| WebAssembly | @rendermode InteractiveWebAssembly |
| Server| @rendermode InteractiveServer |

---

## Basic spreadsheet code

### Minimal code
```csharp
@page "/spreadsheet-sample"
@using Syncfusion.Blazor.Spreadsheet

// Use only when button creating sample
@using Syncfusion.Blazor.Buttons

// #BUTTON

<SfSpreadsheet @ref="_spreadsheetRef"
               DataSource="DataSourceBytes"
               // #PROPERTY
               // #EVENTS
               >
    <SpreadsheetRibbon></SpreadsheetRibbon>
</SfSpreadsheet>

@code {
    private SfSpreadsheet _spreadsheetRef { get; set; }
    public byte[] DataSourceBytes { get; set; }

    protected override void OnInitialized()
    {
        // Option A: local .xlsx — when the user supplies a file path or root folder,
        // use that path DIRECTLY as a string literal. Do NOT construct it with Path.Combine(Directory.GetCurrentDirectory(), ...)
        string filePath = "#WORKBOOK_PATH";
        DataSourceBytes = File.ReadAllBytes(filePath);

        // Option B: Base64 string
        string base64String = "#BASE64_STRING";
        DataSourceBytes = Convert.FromBase64String(base64String); 
    }

    // #API METHODS
}
```
## Placeholders

| Placeholder | Description |
|---|---|
| `#BUTTON` | Buttons from feature files should be inserted here |
| `#PROPERTY` | Property from feature files should be placed here |
| `#EVENTS` | Events from feature files should be placed here |
| `#API METHODS` | API Methods from feature files should be placed here |
| `#WORKBOOK_PATH` | replace with exact local workbook path here |
| `#BASE64_STRING` | replace with Base64 string here |