# Troubleshooting & FAQ (AI Agent Reference) — Blazor Edition

---

## 🛑 MANDATORY AGENT PROTOCOL: Build/Component Issues

**CRITICAL: When ANY build error or component issue occurs, follow this protocol EXACTLY:**

### 🚨 IMMEDIATE RESPONSE (NO EXCEPTIONS)
1. **⛔ STOP ALL FIXES IMMEDIATELY** - Do not attempt any modifications without skill consultation
2. **🔍 IDENTIFY the exact component** from error message (e.g., SfTextBox, SfGrid, SfButton, etc.)
3. **📖 READ ENTIRE component skill file** using `read_file` with `startLine=1, endLine=999999`
   - Path pattern: `skills/syncfusion-blazor-{component}/SKILL.md`
   - Examples: `syncfusion-blazor-datagrid`, `syncfusion-blazor-inputs`, `syncfusion-blazor-buttons`
4. **✅ EXTRACT skill requirements**:
   - Required @using directives
   - NuGet package names and versions  
   - Parameter names and types
   - Service registrations needed
   - Common troubleshooting solutions
5. **🔧 APPLY ONLY skill-based solutions** - Never generate fixes from memory
6. **🔄 VERIFY each fix** against skill file specifications before applying
7. **✅ CONFIRM .csproj and Program.cs** match skill file requirements

### 🚫 ABSOLUTELY PROHIBITED ACTIONS
- ❌ Generate solutions without reading skill file first
- ❌ Replace Syncfusion components with HTML elements  
- ❌ Remove Syncfusion components to "make it work"
- ❌ Use trial-and-error fixes without skill validation
- ❌ Make multiple changes simultaneously
- ❌ Skip component skill consultation for ANY component error

---

## Table of Contents

1. [Installation & Setup](#installation--setup)
2. [Build & Runtime Errors](#build--runtime-errors)
3. [Component Issues](#component-issues)

---

## Installation & Setup Issues

When user encounters setup errors:

### Syncfusion NuGet package not found
→ **Direct to component skill** for exact package list. Each component skill lists required `Syncfusion.Blazor.*` NuGet packages.
→ Install via: `dotnet add package Syncfusion.Blazor.Inputs` (or appropriate package)

### .csproj or project file not found
→ **Verify user is in Blazor project root.** Check for `.csproj`, `.sln`, or `Program.cs` files. Guide to project detection (Stage 2).

### Blazor version incompatible (< 8.0.0)
→ **Inform user** to upgrade: `dotnet add package Microsoft.AspNetCore.Components.WebAssembly --version 8.0.0` (or latest)
→ Ensure Syncfusion packages match Blazor version compatibility

### Services not registered
→ **Verify `Program.cs` contains:** `builder.Services.AddSyncfusionBlazor();`
→ Direct to component skill for any service-specific setup

---

## Build & Runtime Error Protocol

### CSS import errors / theme not loading

**For Blazor WebAssembly (WASM):**
→ Verify CSS link in `wwwroot/index.html` `<head>`: 
```html
<link href="_content/Syncfusion.Blazor.Themes/fluent2.css" rel="stylesheet" />
```
→ Check that Syncfusion NuGet packages are properly installed in `.csproj`
→ Rebuild and restart dev server: `dotnet watch`

**For Blazor WebApp (Server or Auto-Render):**
→ Verify CSS link in `Components/App.razor` `<head>`:
```razor
<head>
    <link href="_content/Syncfusion.Blazor.Themes/fluent2.css" rel="stylesheet" />
    <!-- other head content -->
</head>
```
→ Ensure `HeadOutlet` component is in your routing structure if using SSR
→ Check that Syncfusion NuGet packages are properly installed in `.csproj`
→ Rebuild and restart dev server: `dotnet run`

**Common issue:** Theme CSS imported in wrong location (WASM projects need `index.html`, WebApp projects need `App.razor`)

### Missing Razor component directives (e.g., "Cannot find SfTextBox")
→ **Load component skill** (TextBox, Grid, Button, etc.). Reference required `@using Syncfusion.Blazor.*` directives.
→ Ensure `Program.cs` has `builder.Services.AddSyncfusionBlazor();`

### Enum ambiguity errors (e.g., "'ButtonType' is ambiguous reference")
→ **CRITICAL**: Always use full namespace for Syncfusion enums
→ **Fix**: Replace `ButtonType.Primary` with `Syncfusion.Blazor.Buttons.ButtonType.Primary`
→ **Why**: Same enum names exist in different Syncfusion components
→ **Load component skill** to verify exact namespace requirements

### License activation errors
→ Ensure Syncfusion license is registered in `Program.cs` or via environment variable
→ Verify license key format and registration code in app startup
→ Check Syncfusion documentation for license registration method

### Event handler issues / callback not firing
→ **Load component skill.** Each Syncfusion Blazor component has specific event delegates (e.g., `ValueChange`, `ActionBegin`, etc.)
→ Ensure event handlers use proper C# async/await pattern: `@on<EventName>="@HandleEvent"`
→ Verify parameter types match component's event args

---

## Component-Specific Issues Protocol

**Any component-specific issue → Load corresponding component skill IMMEDIATELY**

Examples of component-specific issues in Blazor:
- SfTextBox not showing value → TextBox Skill troubleshooting
- SfGrid pagination not working → Grid Skill troubleshooting
- SfDatePicker format wrong → DatePicker Skill troubleshooting
- SfDropDownList filtering not enabled → DropDown Skill troubleshooting
- SfButton click not firing → Button Skill troubleshooting
- SfCheckBox not binding two-way → CheckBox Skill troubleshooting

**Component skill structure always includes:**
- Required `@using` directives for component namespace
- `[Parameter]` properties and default values
- Event handler patterns with proper `EventCallback<T>` types
- Common pitfalls and Blazor-specific solutions
- Syncfusion service injections (if needed for advanced features)
- Example `.razor` component usage with C# code-behind

---

## Agent Guidelines

**When user reports issues:**
1. Classify error type (setup / build / component-specific / feature request)
2. Setup/build issues → Use this troubleshooting guide (NuGet, Program.cs, wwwroot/index.html)
3. Component issues → **Load component skill** immediately (SfTextBox, SfGrid, SfButton, etc.)
4. Feature gaps → Suggest component skill or custom Razor implementation
5. Ambiguous errors → Ask clarifying questions before loading skills

**Blazor-specific diagnostics to verify:**
- ✓ `.csproj` has Syncfusion NuGet package references with matching versions
- ✓ `Program.cs` includes `builder.Services.AddSyncfusionBlazor();`
- ✓ `wwwroot/index.html` has correct CSS link: `<link href="_content/Syncfusion.Blazor.Themes/..." rel="stylesheet" />`
- ✓ `.razor` component has `@using Syncfusion.Blazor.*` directives
- ✓ Syncfusion components use correct C# naming (SfGrid, SfTextBox, etc.)
- ✓ Event handlers are `EventCallback<T>` with proper async/await patterns

**When checking component skills:**
- Navigate to component skill (TextBox, Grid, Button, etc.)
- Review "Getting Started" for `@using` directives and component setup
- Check "Common Issues" section for Blazor-specific troubleshooting
- Reference "[Parameter]" properties and "Events" for implementation details
- Verify example code uses correct Razor syntax and C# code-behind

**If issue NOT resolved by component skill:**
→ Escalate to user with component skill reference and diagnostic steps
→ Suggest manual fix with Razor code example and explanation
→ Do NOT generate new Blazor code without understanding root cause

