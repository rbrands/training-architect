# Stage 6: Dependencies

**Purpose:** Detect required NuGet packages, resolve version conflicts, prepare dotnet package installation command from nuget.org with latest stable versions.

## ⚠️ CRITICAL: NuGet Source Requirements

**ALWAYS install packages from nuget.org (official NuGet repository):**
- **Source**: `https://api.nuget.org/v3/index.json`
- **Version**: Use latest stable version from nuget.org
- **Command Format**: Include `--source https://api.nuget.org/v3/index.json` in all commands
- **Verification**: Query nuget.org API to verify latest stable versions before generating commands

**AI Should:**

1. **Detect Required NuGet Packages:**
   - Scan generated code `@using` statements
   - List all `Syncfusion.Blazor.*` packages used
   - **MANDATORY**: `Syncfusion.Blazor.Themes` package (required for CSS theming)
     - Default theme: **Fluent 2 Light** (`_content/Syncfusion.Blazor.Themes/fluent2.css`)
     - Alternative themes: Fluent 2 Dark, Bootstrap5, Material3, Tailwind3
   - Check for other dependencies (additional theme packages if user overrides default)
   - **MANDATORY**: Always read [nuget-packages.md](./nuget-packages.md) to map components to their correct NuGet packages (do NOT skip this)

2. **Verify Latest Versions from nuget.org:**
   - Query nuget.org API for latest stable versions of required packages
   - URL format: `https://api.nuget.org/v3-flatcontainer/{packageId}/index.json`
   - Example: `https://api.nuget.org/v3-flatcontainer/syncfusion.blazor.grid/index.json`
   - Select latest stable version (non-preview, non-beta)

3. **Check Project's .csproj file:**
   - What NuGet packages already installed?
   - What versions are currently in use?
   - Any version conflicts?
   - Are packages from nuget.org source?

4. **Resolve Conflicts:**
   - If Syncfusion package already installed:
     - Is version compatible?
     - Compare with latest version from nuget.org
     - Suggest upgrade to latest stable if current version is outdated
   - If Syncfusion Blazor framework version doesn't match:
     - Recommend resolution (upgrade all to same version)
     - **All Syncfusion.Blazor.* packages MUST use identical versions**
   - If package source is not nuget.org:
     - Warn user and recommend reinstalling from official source

5. **Prepare Installation Command:**
   - Generate `dotnet add package` commands with `--source` parameter
   - Use latest stable versions from nuget.org
   - List packages to add
   - List packages to upgrade (if needed)
   - Include version consistency checks

**Example Output:**

```
✓ Dependency Analysis

📦 Checking Latest Versions from nuget.org...
  ✓ Syncfusion.Blazor.Inputs: 32.1.19 (latest stable)
  ✓ Syncfusion.Blazor.Buttons: 32.1.19 (latest stable)
  ✓ Syncfusion.Blazor.Grid: 32.1.19 (latest stable)
  ✓ Syncfusion.Blazor.Themes: 32.1.19 (latest stable) [MANDATORY]

New NuGet Packages to Install:
  - Syncfusion.Blazor.Themes (v32.1.19) [MANDATORY for CSS theming]
  - Syncfusion.Blazor.Inputs (v32.1.19)
  - Syncfusion.Blazor.Buttons (v32.1.19)
  - Syncfusion.Blazor.Grid (v32.1.19)

Existing Packages:
  ✓ Syncfusion.Blazor.Core@32.1.19 (compatible, latest)
  ✓ Syncfusion.Blazor@32.1.19 (compatible, latest)

Conflicts: None

Source: nuget.org (official NuGet repository)

Install Commands (run from project root):
# Install from nuget.org with latest stable versions
$ dotnet add package Syncfusion.Blazor.Themes --source https://api.nuget.org/v3/index.json
$ dotnet add package Syncfusion.Blazor.Inputs --source https://api.nuget.org/v3/index.json
$ dotnet add package Syncfusion.Blazor.Buttons --source https://api.nuget.org/v3/index.json
$ dotnet add package Syncfusion.Blazor.Grid --source https://api.nuget.org/v3/index.json

Or all at once (PowerShell):
$packages = @('Syncfusion.Blazor.Themes', 'Syncfusion.Blazor.Inputs', 'Syncfusion.Blazor.Buttons', 'Syncfusion.Blazor.Grid')
$packages | ForEach-Object { dotnet add package $_  --source https://api.nuget.org/v3/index.json }
 from nuget.org?
[Run dotnet add package] [Show Commands] [Skip]
```

**Installation Options:**
1. **Automatic** - AI runs `dotnet add package` commands via terminal with `--source` parameter
2. **Manual** - User copies commands and runs in their terminal (commands include nuget.org source)
3. **Visual Studio** - User adds packages via NuGet Package Manager UI (ensure nuget.org is selected as source)

**Status:** AI detects, verifies latest versions from nuget.org, and prepares installation commands. User decides whether to install now or later.

---

## Version Conflict Resolution Examples

### Example 1: Existing Packages with Outdated Versions
```
⚠️ Version Conflicts Detected

Existing Packages (Outdated):
  ⚠️ Syncfusion.Blazor.Core@24.1.41 (current)
      → Latest: 32.1.19 (from nuget.org)
  ⚠️ Syncfusion.Blazor@24.1.41 (current)
      → Latest: 32.1.19 (from nuget.org)

Recommendation: Upgrade existing packages to latest stable version (32.1.19)

Upgrade Commands:
$ dotnet add package Syncfusion.Blazor.Core --source https://api.nuget.org/v3/index.json
$ dotnet add package Syncfusion.Blazor --source https://api.nuget.org/v3/index.json

Then install new packages at same version:
$ dotnet add package Syncfusion.Blazor.Grid --source https://api.nuget.org/v3/index.json
```

### Example 2: Version Consistency Enforcement
```
⚠️ Version Mismatch Detected

Installed Packages:
  ✓ Syncfusion.Blazor.Core@32.1.19
  ✗ Syncfusion.Blazor.Grid@26.2.10 (mismatched!)

Issue: All Syncfusion.Blazor.* packages MUST use the same version.

Resolution:
Option A: Upgrade mismatched package to 32.1.19 (recommended)
$ dotnet add package Syncfusion.Blazor.Grid --source https://api.nuget.org/v3/index.json

Option B: Downgrade all to 26.2.10 (not recommended, use latest)
```

### Example 3: Missing Syncfusion.Blazor.Themes Package
```
✗ CRITICAL: Missing Required Package

Issue: Syncfusion.Blazor.Themes package is NOT installed
Impact: Component CSS theming will NOT work without this package

Required Action:
$ dotnet add package Syncfusion.Blazor.Themes --source https://api.nuget.org/v3/index.json

Verify CSS import in index.html or App.razor:
<link href="_content/Syncfusion.Blazor.Themes/fluent2.css" rel="stylesheet" />
```

---

## ⚠️ CRITICAL: Component-Specific Scripts

**These components need additional scripts in your app entry point:**
- SfPdfViewer2 → `_content/Syncfusion.Blazor.SfPdfViewer/scripts/syncfusion-blazor-sfpdfviewer.min.js`
- SfSmartPdfViewer → `_content/Syncfusion.Blazor.SfSmartPdfViewer/scripts/syncfusion-blazor-sfsmartpdfviewer.min.js`
- SfDocumentEditorContainer → `_content/Syncfusion.Blazor.WordProcessor/scripts/syncfusion-blazor-documenteditor.min.js`
- SfSpreadsheet → `_content/Syncfusion.Blazor.Spreadsheet/scripts/syncfusion-blazor-spreadsheet.min.js`

**Add after base Syncfusion script in `wwwroot/index.html` (WASM) or `Components/App.razor` (WebApp), before `blazor.web.js`.**

**Refer to:** `references/nuget-packages.md` for complete script reference details.

---

## ⚠️ PROGRAM.CS FOR PDF COMPONENTS

**For SfPdfViewer2 or SfSmartPdfViewer, add to Program.cs:**

```csharp
using Syncfusion.Blazor;

builder.Services.AddSignalR(o => { o.MaximumReceiveMessageSize = 102400000; });
builder.Services.AddMemoryCache();
builder.Services.AddSyncfusionBlazor();
```

- `AddSignalR()` with `MaximumReceiveMessageSize = 102400000` (100MB for large PDF uploads)
- `AddMemoryCache()` for PDF caching
- `AddSyncfusionBlazor()` required for all Syncfusion components

---

## How to Query nuget.org API (For Reference)

**Get Latest Version:**
```bash
# PowerShell example
$packageId = "Syncfusion.Blazor.Grid"
$url = "https://api.nuget.org/v3-flatcontainer/$packageId/index.json"
$versions = (Invoke-RestMethod -Uri $url).versions
$latestStable = $versions | Where-Object { $_ -notmatch 'preview|beta|alpha' } | Select-Object -Last 1
Write-Output "Latest stable version: $latestStable"
```

**Alternative: Use NuGet CLI**
```bash
dotnet list package --outdated --source https://api.nuget.org/v3/index.json
```

---

## Best Practices

1. ✅ **Always specify source**: Include `--source https://api.nuget.org/v3/index.json` in commands
2. ✅ **Use latest stable**: Query nuget.org for latest non-preview versions
3. ✅ **Version consistency**: All Syncfusion.Blazor.* packages must match versions
4. ✅ **Include Themes package**: Syncfusion.Blazor.Themes is MANDATORY for CSS theming
5. ✅ **Verify after install**: Check .csproj file to confirm correct versions
6. ✅ **Document versions**: Keep track of installed versions for team consistency
7. ⚠️ **Avoid preview versions**: Only use stable releases unless testing specific features
8. ⚠️ **Check compatibility**: Ensure package versions are compatible with .NET SDK version
**User Interaction:**
User confirms package installation or does it manually:
```
Ready to install dependencies?
[Run dotnet add package] [Show Commands] [Skip]
```

**Installation Options:**
1. **Automatic** - AI runs `dotnet add package` commands via terminal
2. **Manual** - User copies commands and runs in their terminal/Visual Studio Package Manager
3. **Visual Studio** - User adds packages via NuGet Package Manager UI

**Status:** AI detects and prepares. User decides whether to install now or later.
