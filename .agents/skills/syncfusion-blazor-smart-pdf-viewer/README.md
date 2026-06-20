# Syncfusion Blazor Smart PDF Viewer — Skill

## Overview

The **syncfusion-blazor-smart-pdf-viewer** skill enables AI-assisted code generation for the Syncfusion Blazor Smart PDF Viewer (`SfSmartPdfViewer`). It produces minimal, copy-pasteable C# and Razor code to embed AI-powered PDF viewing, document summarization, smart redaction, and smart form filling capabilities in Blazor applications.

---

## Key Features

- **AI-Powered Document Summarization** — Generate document summaries using Azure OpenAI or custom AI services
- **Smart Redaction** — Automatically detect and redact sensitive information (PII, credit cards, emails, etc.)
- **Smart Form Filling** — AI-powered automatic form field population
- **Custom AI Integration** — Bring your own AI service via `IChatInferenceService` interface

---

## Skill Structure

```
syncfusion-blazor-smart-pdf-viewer/
├── SKILL.md                      # Skill rules, routing, and code generation guidelines
├── README.md                     # This file
└── reference/
    ├── getting-started.md        # Minimal setup, NuGet packages, service registration, Azure OpenAI config
    ├── smart-redaction.md        # AI-powered redaction patterns (PII, credit cards, emails, etc.)
    ├── smart-fill.md             # Smart form filling configuration
    ├── document-summarizer.md    # Document summarization and AI assistant settings
    └── custom-ai-service.md      # Custom AI service integration guide
```

---

## Quick Start

### 1. Create a Blazor Web App Project

```bash
dotnet new blazor -n MyBlazorApp
cd MyBlazorApp
```

### 2. Install Syncfusion Blazor Packages

```bash
dotnet add package Syncfusion.Blazor.SfSmartPdfViewer
dotnet add package Syncfusion.Blazor.Themes
```

### 3. Install AI Packages (Azure OpenAI)

```bash
dotnet add package Azure.AI.OpenAI
dotnet add package Microsoft.Extensions.AI
dotnet add package Microsoft.Extensions.AI.OpenAI --version 9.8.0-preview.1.25412.6
```

### 4. Register Services in `Program.cs`

```csharp
using Syncfusion.Blazor;
using Azure.AI.OpenAI;
using Microsoft.Extensions.AI;
using Syncfusion.Blazor.AI;
using System.ClientModel;

var builder = WebApplication.CreateBuilder(args);

// Add SignalR with large message size for PDF operations
builder.Services.AddSignalR(o => { o.MaximumReceiveMessageSize = 102400000; });
builder.Services.AddMemoryCache();
builder.Services.AddSyncfusionBlazor();

// Configure Azure OpenAI
string azureOpenAiKey = "your-api-key";
string azureOpenAiEndpoint = "your-endpoint-url";
string azureOpenAiModel = "your-deployment-name";

AzureOpenAIClient azureOpenAIClient = new AzureOpenAIClient(
    new Uri(azureOpenAiEndpoint), 
    new ApiKeyCredential(azureOpenAiKey)
);
IChatClient azureOpenAiChatClient = azureOpenAIClient
    .GetChatClient(azureOpenAiModel)
    .AsIChatClient();

builder.Services.AddChatClient(azureOpenAiChatClient);
builder.Services.AddSingleton<IChatInferenceService, SyncfusionAIService>();

var app = builder.Build();
```

### 5. Add Namespaces to `_Imports.razor`

```razor
@using Syncfusion.Blazor
@using Syncfusion.Blazor.SmartPdfViewer
```

### 6. Add Stylesheet and Scripts to `App.razor`

```html
<head>
    <!-- Other head content -->
    <link href="_content/Syncfusion.Blazor.Themes/bootstrap5.css" rel="stylesheet" />
</head>

<body>
    <!-- Other body content -->
    <script src="_content/Syncfusion.Blazor.SfPdfViewer/scripts/syncfusion-blazor-sfsmartpdfviewer.min.js" type="text/javascript"></script>
</body>
```

### 7. Add Smart PDF Viewer Component to a Razor Page

```razor
@page "/pdf-viewer"
@rendermode InteractiveServer

<SfSmartPdfViewer 
    Height="100%" 
    Width="100%" 
    DocumentPath="https://cdn.syncfusion.com/content/pdf/http-succinctly.pdf">
</SfSmartPdfViewer>
```

### 8. Run the App

```bash
dotnet run
```

---

## AI Service Options

The Smart PDF Viewer supports multiple AI service backends:

| Service | Package | Use Case |
|---|---|---|
| **Azure OpenAI** | `Azure.AI.OpenAI` | Production cloud-based AI |
| **Ollama** | `OllamaSharp` | Self-hosted local models |
| **Custom AI Service** | Implement `IChatInferenceService` | Bring your own AI service |

---

## Reference File Routing

Use the table below to find the correct reference file for any feature request.

| Reference File | Use When … |
|---|---|
| **getting-started.md** | Getting started, minimal setup, NuGet packages, service registration, Azure OpenAI/Ollama configuration, loading a PDF |
| **smart-redaction.md** | Implementing AI-powered redaction, configuring redaction patterns (Person Names, Email Addresses, Phone Numbers, Credit Card Numbers, etc.) |
| **smart-fill.md** | Implementing smart form filling, configuring AI-powered automatic PDF form field population |
| **document-summarizer.md** | Implementing document summarization, configuring AI assistant settings, prompt suggestions, streaming responses |
| **custom-ai-service.md** | Integrating custom AI services, implementing `IChatInferenceService`, error handling with custom chat clients |

---