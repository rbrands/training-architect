# Getting Started with Blazor Smart PDF Viewer

## Prerequisites
- Azure OpenAI Account or Ollama for self-hosted models

## Create Blazor Web App
Create a Blazor Web App using Visual Studio 2022.

## Install NuGet Packages
Package Manager:
```
Install-Package Syncfusion.Blazor.SfSmartPdfViewer
Install-Package Syncfusion.Blazor.Themes
```

## Register Blazor Service

Add namespaces to ~/_Imports.razor:
```
@using Syncfusion.Blazor
@using Syncfusion.Blazor.SmartPdfViewer
```

Add service to ~/Program.cs:
```csharp
using Syncfusion.Blazor;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSignalR(o => { o.MaximumReceiveMessageSize = 102400000; });
builder.Services.AddMemoryCache();
builder.Services.AddSyncfusionBlazor();

var app = builder.Build();
```

## Configure Azure OpenAI (Primariliy Included in the sample)

### Install packages via Package Manager:

```
Install-Package Azure.AI.OpenAI
Install-Package Microsoft.Extensions.AI
Install-Package Microsoft.Extensions.AI.OpenAI -Version 9.8.0-preview.1.25412.6
```

### Add to ~/Program.cs:
```csharp
using Azure.AI.OpenAI;
using Microsoft.Extensions.AI;
using Syncfusion.Blazor.AI;
using System.ClientModel;

var builder = WebApplication.CreateBuilder(args);

string azureOpenAiKey = "api-key";
string azureOpenAiEndpoint = "endpoint URL";
string azureOpenAiModel = "deployment-name";

AzureOpenAIClient azureOpenAIClient = new AzureOpenAIClient(new Uri(azureOpenAiEndpoint), new ApiKeyCredential(azureOpenAiKey));
IChatClient azureOpenAiChatClient = azureOpenAIClient.GetChatClient(azureOpenAiModel).AsIChatClient();

builder.Services.AddChatClient(azureOpenAiChatClient);
builder.Services.AddSingleton<IChatInferenceService, SyncfusionAIService>();

var app = builder.Build();
```

## Configure Ollama (Optional)

### Install OllamaSharp:

```
Install-Package OllamaSharp -Version 5.3.6
```

Add to ~/Program.cs:
```csharp
using Microsoft.Extensions.AI;
using OllamaSharp;
using Syncfusion.Blazor;
using Syncfusion.Blazor.AI;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSyncfusionBlazor();

string aiModel = "llama2:7b";
HttpClient httpClient = new HttpClient
{
    BaseAddress = new Uri("http://localhost:11434"),
    Timeout = Timeout.InfiniteTimeSpan
};

IChatClient chatClient = new OllamaApiClient(httpClient, aiModel);
builder.Services.AddChatClient(chatClient);
builder.Services.AddSingleton<IChatInferenceService, SyncfusionAIService>();

var app = builder.Build();
```

## Add Stylesheet and Scripts

### App.razor Configuration

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
    <script src="_content/Syncfusion.Blazor.SfPdfViewer/scripts/syncfusion-blazor-sfsmartpdfviewer.min.js" type="text/javascript"></script>
</body>
```

---

## Add Component

### Add render mode to ~/Pages/Home.razor:

```razor
@rendermode InteractiveServer
```

Add component:
```razor
<SfSmartPdfViewer Height="100%" Width="100%" DocumentPath="@DocumentPath">
</SfSmartPdfViewer>
```
