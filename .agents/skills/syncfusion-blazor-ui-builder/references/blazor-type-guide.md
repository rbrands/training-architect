# How to Change/Specify Blazor Project Type (BlazorWASM vs BlazorWebApp)

## Quick Answer

There are **3 ways** to specify or change the Blazor Type:

### 1. **User States It Directly** (Easiest)
```
User: "I have a BlazorWASM project"
 OR
User: "I'm using Blazor WebApp (server-rendered)"

→ AI respects user declaration
→ All downstream decisions use this type
```

### 2. **AI Auto-Detects** (Automatic)
```
AI runs Stage 2 project detection and automatically identifies:
  ✓ BlazorWASM: By finding wwwroot/index.html with <div id="app"></div>
  ✓ BlazorWebApp: By finding Components/App.razor with routing

Presents confirmation:
  "I detected BlazorWebApp. Is this correct?
   [Yes] [No, it's BlazorWASM] [I'm not sure]"
```

### 3. **User Overrides Auto-Detection** (During Stage 2)
```
AI: "I detected BlazorWebApp based on Components/App.razor"
User: "Actually, change it to BlazorWASM"

→ Stage 2 updates detection
→ All downstream stages use BlazorWASM
```

---

## Understanding the Difference

### BlazorWASM (WebAssembly)
- **What it is:** Client-side Blazor app running in the browser
- **When to use:** When you need a pure frontend app, PWA, or offline capability
- **Theme CSS location:** `wwwroot/index.html` `<head>`
- **Entry point:** `wwwroot/index.html` with `<div id="app"></div>`
- **Program.cs signature:** `WebAssemblyHostBuilder.CreateDefault()`

**Example directory structure:**
```
BlazorApp/
├── wwwroot/
│   ├── index.html          ← Theme CSS goes HERE
│   ├── css/
│   │   └── app.css
│   └── app.js
├── Components/             (optional)
│   └── Pages/
│       └── Home.razor
├── Program.cs
├── .csproj
└── appsettings.json
```

### BlazorWebApp (Server or Auto-Render)
- **What it is:** Full-stack Blazor app with server and client rendering
- **When to use:** When you need server-side logic, databases, or hybrid rendering
- **Theme CSS location:** `Components/App.razor` `<head>`
- **Entry point:** `Components/App.razor` root component
- **Program.cs signature:** `WebApplication.CreateBuilder()`

**Example directory structure:**
```
BlazorApp/
├── Components/
│   ├── App.razor           ← Theme CSS goes HERE
│   ├── Routes.razor
│   ├── Layout/
│   │   └── MainLayout.razor
│   ├── Pages/
│   │   └── Home.razor
│   └── Shared/
├── wwwroot/
│   └── css/
│       └── app.css
├── Program.cs
├── .csproj
└── appsettings.json
```

---

## Detection Checklist

**If auto-detection fails or is ambiguous, use this checklist:**

| File/Setting | BlazorWASM | BlazorWebApp |
|-----------|-----------|--------------|
| **Check: wwwroot/index.html?** | ✅ EXISTS (entry point) | ❌ Does NOT exist |
| **Check: Components/App.razor?** | ❌ Usually NOT in root | ✅ EXISTS (required, root component) |
| **Check: Program.cs** | `WebAssemblyHostBuilder` | `WebApplication.CreateBuilder()` |
| **Check: Services** | `.AddScoped<HttpClient>()` | `.AddRazorComponents()` |
| **Check: .csproj SDK** | `BlazorWebAssembly` | `Microsoft.NET.Sdk.Web` |
| **Theme CSS location** | `wwwroot/index.html` | `Components/App.razor` |
| **Rendering** | 100% client-side | Server-side + optional client-side |

---

## How Project Type Affects Generated Code

Once you specify Blazor Type in **Stage 2**, it determines these downstream decisions:

### Stage 4: Theme Registration
```
If BlazorWASM:
  → Theme CSS import in: wwwroot/index.html
  → Example: <link href="_content/Syncfusion.Blazor.Themes/tailwind3.css" rel="stylesheet" />

If BlazorWebApp:
  → Theme CSS import in: Components/App.razor
  → Example: <head>
               <link href="_content/Syncfusion.Blazor.Themes/tailwind3.css" rel="stylesheet" />
             </head>
```

### Stage 5: Code Generation
```
If BlazorWASM:
  → Generated components in: Components/ or wwwroot/components/
  → CSS imports reference wwwroot/ paths
  → No server-side logic

If BlazorWebApp:
  → Generated components in: Components/ (standard location)
  → CSS imports reference _content/ paths
  → Can include server-side integration patterns
```

### Stage 8: Theme CSS Verification
```
If BlazorWASM:
  → Verify: wwwroot/index.html contains Syncfusion theme link

If BlazorWebApp:
  → Verify: Components/App.razor contains Syncfusion theme link
```

---

## Step-by-Step: How to Specify Blazor Type

### Scenario 1: User States It First
```
User Input:
"I have a BlazorWASM project, create a login form"

AI Response:
✓ Acknowledged: BlazorWASM project type
✓ Stage 2: Auto-detect other settings (CSS, .NET version, etc.)
✓ Stage 4: Confirm theme CSS location: wwwroot/index.html
✓ Stage 5+: Generate code using BlazorWASM structure
```

### Scenario 2: AI Detects, User Confirms
```
AI Detection (Stage 2):
"I detected the following:
 ✓ Framework: .NET 8.0
 ✓ Blazor Project Type: BlazorWebApp
   └─ Found: Components/App.razor with routing
 ✓ CSS: Tailwind CSS
 [Confirm] [Override Blazor Type]"

User Response:
[Confirm]

→ Proceed with BlazorWebApp for all stages
```

### Scenario 3: User Overrides Auto-Detection
```
AI Detection (Stage 2):
"✓ Blazor Project Type: BlazorWebApp (detected)"

User Input:
"Actually, it's BlazorWASM"

AI Update:
✓ Blazor Project Type: BlazorWASM ⭐ UPDATED
  └─ Theme CSS Location: wwwroot/index.html (adjusted)

→ All downstream stages use BlazorWASM
```

---

## Programmatic Change in Code

If you need to manually verify or change the Blazor Type in configuration:

### Check in Program.cs

**For BlazorWASM, look for:**
```csharp
// Program.cs in Blazor WebAssembly project
var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
await builder.Build().RunAsync();
```

**For BlazorWebApp, look for:**
```csharp
// Program.cs in Blazor WebApp project
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

var app = builder.Build();
app.UseRouting();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
```

---

## Common Issues & Solutions

### Issue: "Theme CSS not loading"
**Check:** Is your project BlazorWASM or BlazorWebApp?

**If BlazorWASM:**
- Verify `wwwroot/index.html` contains:
  ```html
  <head>
    <link href="_content/Syncfusion.Blazor.Themes/tailwind3.css" rel="stylesheet" />
  </head>
  ```

**If BlazorWebApp:**
- Verify `Components/App.razor` `<head>` contains:
  ```razor
  <head>
    <link href="_content/Syncfusion.Blazor.Themes/tailwind3.css" rel="stylesheet" />
  </head>
  ```

### Issue: "Components not rendering"
**Check:** Is the theme CSS in the correct location for your project type?

**BlazorWASM:** Theme must be in `wwwroot/index.html`  
**BlazorWebApp:** Theme must be in `Components/App.razor`

### Issue: "Unsure if my project is BlazorWASM or BlazorWebApp"
**Solution:** Check these three files:
1. Does `wwwroot/index.html` exist? → **BlazorWASM**
2. Does `Components/App.razor` exist? → **BlazorWebApp**
3. Still unsure? Check `Program.cs` for `WebAssemblyHostBuilder` (WASM) vs `WebApplication.CreateBuilder()` (WebApp)

---

## Summary

| Task | How | Result |
|------|-----|--------|
| **Specify at start** | User says "I have BlazorWASM project" | All stages use BlazorWASM |
| **Let AI detect** | AI reads wwwroot/index.html or Components/App.razor | AI auto-identifies type |
| **Override detection** | User says "Actually, it's BlazorWebApp" | All stages switch to BlazorWebApp |
| **Verify my project type** | Check wwwroot/index.html (WASM) or Components/App.razor (WebApp) | Know your project structure |

**Remember:** Once Blazor Type is set in Stage 2, it determines **theme CSS location** (the most critical downstream decision) and all other code generation patterns.
