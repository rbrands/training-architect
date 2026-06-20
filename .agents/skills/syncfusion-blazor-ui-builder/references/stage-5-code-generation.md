# Stage 5: Code Generation

**Purpose:** Generate production-ready Blazor `.razor` components, CSS, and C# code with accessibility and web standards compliance.

---

## ⛔ MANDATORY: Read `nuget-packages.md` First

**Before code generation or package installation:**
1. **Read:** `nuget-packages.md`
2. **Validate:** Each component's package name exists in that file
3. **Reject:** Non-existent packages (don't try to install them)

---

## CRITICAL: Read Component Skills BEFORE Code Generation

**THIS STEP IS NOT OPTIONAL - Must be completed before writing any code**

### Step 1: Extract All Selected Components and Icons from `component-mapping.json`

Parse `component-mapping.json` and extract all unique Syncfusion components, skills, and icons.

**Display this output BEFORE reading any component skills:**

```
Syncfusion Components: [list unique components]
Skills: [list unique skills]
Icons: [list unique iconCss values]
```

**DO NOT skip or substitute any components with native HTML.**

---

### Step 2: Validate NuGet Packages

**Read [references/nuget-packages.md](nuget-packages.md) and verify each component's package exists:**

- Check: Does `Syncfusion.Blazor.ComponentName` exist in the file?
- Reject: Non-existent packages (e.g., `Syncfusion.Blazor.AppBar` doesn't exist)
- Correct: Update `component-mapping.json` if packages don't exist

---

### Step 3: Read Getting-Started for EACH Component Skill

**For every single validated component from Step 2:**

1. **Read:** `.codestudio/skills/<skill-name>/references/getting-started.md`
   - This is the ONLY authoritative source for imports, styles, and setup
   - Do NOT generate imports without reading this first
   
2. **Extract and document:**
   - NuGet package name (e.g., `Syncfusion.Blazor.Grid`)
   - Exact `@using` statement for the component (e.g., `@using Syncfusion.Blazor.Grids`)
   - **Style imports (CRITICAL)** - Component-specific CSS if needed
   - Theme CSS if applicable (usually already loaded in `wwwroot/index.html`)
   - Required Blazor services/registration (if any, in `Program.cs`)
   - Base dependencies

---

### Step 4: If Complex Features Needed

- Read: `.codestudio/skills/<skill-name>/SKILL.md` for complete API documentation
- Read feature-specific guides: `<skill-name>/references/filtering.md`, `validation.md`, `styling.md`, etc.

### Step 5: (CRITICAL) Apply Syncfusion Theme - Default: Fluent 2 Light

**🎨 DEFAULT THEME: Fluent 2 Light**

Unless the user explicitly requested a different theme in Stage 4, apply **Fluent 2 Light** theme:
- CSS Import: `_content/Syncfusion.Blazor.Themes/fluent2.css`
- Advantages: Modern Microsoft design, WCAG 2.1 AA compliant, professional appearance

**Read the Syncfusion themes guide for all theme options and CSS import locations:**

1. **Read:** `.codestudio/skills/syncfusion-blazor-ui-builder/references/syncfusion-themes.md`
2. **Apply theme CSS in correct location:**
   - **WASM:** `wwwroot/index.html` `<head>` section (before `<div id="app"></div>`)
   - **WebApp:** `Components/App.razor` `<head>` section (before components render)

### Step 6: Verify Script Registration (BEFORE Code Generation)

**⚠️ CRITICAL - DO THIS FIRST, NOT LAST:**
Many AI workflows make the mistake of generating code first, then trying to add scripts later. This causes components to render but be non-interactive (gauges don't animate, charts are static, tooltips don't work).

**You MUST verify App.razor setup BEFORE generating any code:**

**For Blazor WebApp (Server/Auto/WebAssembly):**
1. Read `Components/App.razor`
2. **Check for JavaScript Script** (BEFORE `<Routes/>`):
   ```html
   <script src="_content/Syncfusion.Blazor.Core/scripts/syncfusion-blazor.min.js" type="text/javascript"></script>
   ```
3. **Check for CSS Theme** (in `<head>`):
   ```html
   <link href="_content/Syncfusion.Blazor.Themes/bootstrap5.css" rel="stylesheet" />
   ```
4. If EITHER is missing, **ADD THEM NOW** before proceeding

**For Blazor WebAssembly Standalone App:**
1. Read `wwwroot/index.html`
2. **Check for JavaScript Script** (BEFORE `blazor.web.js`):
   ```html
   <script src="_content/Syncfusion.Blazor.Core/scripts/syncfusion-blazor.min.js" type="text/javascript"></script>
   ```
3. **Check for CSS Theme** (in `<head>`):
   ```html
   <link href="_content/Syncfusion.Blazor.Themes/bootstrap5.css" rel="stylesheet" />
   ```
4. If EITHER is missing, **ADD THEM NOW** before proceeding

**Why This Must Come First:**
- CSS alone = components render but look broken/misaligned
- JavaScript alone = components may throw errors or render nothing
- BOTH = components work correctly with full interactivity

### Step 6.1: MANDATORY Component-Specific Script Check (CRITICAL)

**⚠️ CRITICAL: Some components require ADDITIONAL scripts beyond the base `syncfusion-blazor.min.js`**

Before generating code, check if ANY of these components are in your component mapping:
- **SfPdfViewer2** (from Syncfusion.Blazor.SfPdfViewer)
- **SfSmartPdfViewer** (from Syncfusion.Blazor.SfSmartPdfViewer)
- **SfDocumentEditorContainer** (from Syncfusion.Blazor.WordProcessor)
- **SfSpreadsheet** (from Syncfusion.Blazor.Spreadsheet)

**If ANY of these components are present in Stage 3's component-mapping.json:**

1. **Read** `nuget-packages.md` section "Component-Specific Script References"
2. **Identify** which component scripts are needed
3. **Add component scripts** to your app entry point:
   - **WASM:** Add to `wwwroot/index.html` after base Syncfusion script
   - **WebApp:** Add to `Components/App.razor` after base Syncfusion script
4. **Script Template** (add BEFORE `<Routes/>` or `<div id=\"app\"></div>`):
   ```html
   <!-- Syncfusion Blazor PDF Viewer control's scripts -->
   <script src=\"_content/Syncfusion.Blazor.SfPdfViewer/scripts/syncfusion-blazor-sfpdfviewer.min.js\" type=\"text/javascript\"></script>\n\n   <!-- Syncfusion Blazor Smart PDF Viewer control's scripts -->
   <script src=\"_content/Syncfusion.Blazor.SfSmartPdfViewer/scripts/syncfusion-blazor-sfsmartpdfviewer.min.js\" type=\"text/javascript\"></script>\n\n   <!-- Syncfusion Blazor Document Editor control's scripts -->
   <script src=\"_content/Syncfusion.Blazor.WordProcessor/scripts/syncfusion-blazor-documenteditor.min.js\" type=\"text/javascript\"></script>\n\n   <!-- Syncfusion Blazor Spreadsheet control's scripts -->
   <script src=\"_content/Syncfusion.Blazor.Spreadsheet/scripts/syncfusion-blazor-spreadsheet.min.js\" type=\"text/javascript\"></script>\n   ```

**⚠️ WITHOUT THESE SCRIPTS:**
- Components will not render or will be non-functional
- PDF Viewer will fail to display documents
- Spreadsheet will not calculate or display data
- Document Editor will not function
- Smart PDF Viewer will not work

### Step 7: MANDATORY Folder Structure - Components/Pages

**⚠️ CRITICAL REQUIREMENT:** ALL generated `.razor` page files MUST be created in the `Components/Pages/` folder structure.

**Folder Organization Rules (MANDATORY):**
- **Page components with `@page` directive** → MUST go in `Components/Pages/` folder
- **Reusable components WITHOUT `@page`** → Go in `Components/Shared/` or feature-specific folders
- **Each page file MUST have a unique `@page` route** (no duplicates across any page files)

**Invalid Folder Locations (❌ DO NOT USE):**
- ❌ Placing page files in project root
- ❌ Placing page files in `Pages/` folder (old Blazor convention - use `Components/Pages/` instead)
- ❌ Creating `.razor` pages in `Components/Shared/` (that folder is for reusable components only)
- ❌ Mixing page files with reusable components in same folder

**Correct Folder Structure (✅ REQUIRED):**
```
YourBlazorProject/
├── Components/
│   ├── Pages/                    ← ALL page files (.razor with @page) go HERE
│   │   ├── Home.razor            (page with @page "/")
│   │   ├── Home.razor.css        (page-specific styles)
│   │   ├── Login.razor           (page with @page "/login")
│   │   ├── Login.razor.css       (page-specific styles)
│   │   ├── Dashboard.razor       (page with @page "/dashboard")
│   │   └── Dashboard.razor.css   (page-specific styles)
│   │
│   ├── Shared/                   ← Reusable components (NO @page directive)
│   │   ├── LoginForm.razor       (reusable component, no route)
│   │   ├── LoginForm.razor.css
│   │   ├── Header.razor
│   │   └── Footer.razor
│   │
│   ├── App.razor                 (root component, renders pages)
│   └── _Imports.razor            (global @using statements)
```

### Step 8: NOW Generate Code Using Extracted Information

Only after completing Steps 1-7 (including folder verification), generate the `.razor` component file using the exact `@using` statements and styles extracted from component skills.

**ALL Selected Components MUST Be Used.** Do not substitute Stage 3 Syncfusion components with native HTML alternatives.

**⚠️ MANDATORY: ENUM NAMESPACE QUALIFICATION**
- **CRITICAL**: Always use full namespace for enums: `Syncfusion.Blazor.Buttons.ButtonType.Primary` not `ButtonType.Primary`
- **Why**: Prevents "ambiguous reference" errors when same enum names exist in different components
- **Verification**: All enum references MUST include full namespace path

**Common Mistake to Avoid:**
❌ Generate code, then try to add Syncfusion services later → Results in missing services, broken components
✅ Read getting-started FIRST, extract NuGet package names and required services, THEN generate code with all `@using` directives included

**Why This Order Is Critical:**
- Component skills contain the authoritative setup syntax for Blazor
- Service registration in `Program.cs` is often forgotten and causes broken components
- Reading first ensures: correct `@using` directives, correct services registered, no missing dependencies, error-free code
- Compatibility with Syncfusion Blazor skill updates

---

## Code Generation Process

**After reading all component skills, AI Should:**

1. **Generate `.razor` component file**:
   - Blazor component with `@using` directives
   - Proper Syncfusion Blazor component imports
   - C# code-behind with parameters and state management
   - Event handlers and data binding
   - Error handling and validation
   - WCAG 2.1 AA accessibility markup (ARIA labels, semantic HTML, focus management)
   - XML documentation comments explaining usage

2. **Generate CSS stylesheet** (based on project preference):
   - CSS Style: `.css` or component-scoped CSS in `.razor.css`
   - Tailwind: Class-based styling via `class` attribute
   - Inline: Style attributes in Razor markup
   - Responsive design: Mobile-first (320px, 768px, 1024px+)
   - Light/dark theme support if needed

3. **Generate C# code-behind**:
   - Component parameters (equivalent to Blazor props)
   - State properties and lifecycle methods (`OnInitializedAsync`, `OnParametersSetAsync`)
   - Event handler method signatures

4. **Reference code standards** from:
   - web-standards.md (accessibility + security rules)
   - Component skill's feature-specific guides (filtering.md, validation.md, styling.md, etc.)

**Code Generation Standards:**

- **Component Imports:** Use exact `@using` syntax from component skill's getting-started.md (e.g., `@using Syncfusion.Blazor.Grids`)
- **Service Registration:** Ensure component services are registered in `Program.cs` (extracted from getting-started.md)
- **Theme CSS:** Include the Syncfusion single package theme from `references/syncfusion-themes.md` in `wwwroot/index.html`
 **Read:** `.codestudio/skills/syncfusion-blazor-ui-builder/references/syncfusion-themes.md`
- **Semantic HTML:** Use proper HTML5 elements (`<form>`, `<label>`, `<button>`, etc.) in Razor markup
- **Accessibility:** ARIA labels, roles, aria-describedby, aria-invalid where needed
- **C# Code:** No dynamic or null-forgiving operators without validation, strong typing
- **Error Handling:** Try-catch blocks in async methods, user-friendly error messages
- **Responsive:** Flex/Grid layouts, media queries
- **Performance:** Use `@key` directive for list rendering, `shouldRender` for conditional updates
- **Security:** HTML encode outputs, sanitize inputs, no hardcoded secrets, validate data binding
- **Comments:** XML documentation (`///`) on methods, explain complex logic

### Media (MANDATORY)

- **Placeholder Images:** Use [Unsplash](https://unsplash.com) for high-quality placeholder images
  - Format: `https://images.unsplash.com/photo-[id]?w=[width]&h=[height]&fit=crop`
  - Example: `https://images.unsplash.com/photo-1560472354-b33ff0c44a43?w=200&h=100&fit=crop`
  - Always specify dimensions (width x height) in the URL
  - Use relevant keywords for context-appropriate images

### Icon Handling (MANDATORY)

**CRITICAL: ALL Icons from Stage 3 Selection List MUST Be Used**

Every icon from the Stage 3 selection list MUST appear in the generated code:
- Navigation icons MUST be used in navigation elements
- Action icons MUST be used for corresponding action buttons
- Data visualization icons MUST be used in chart/grid related UI
- Creation icons MUST be used for add/create actions
- All selected icons MUST be implemented in appropriate UI locations based on their semantic meaning

**Step 1: Attempt to find icon from Component Mapping**
- **Always run the ComponentMapper script first** to retrieve semantic icon mappings (BM25 search against EJ2 icons).
- Refer to the Component skill guidelines for properly adding icons in Syncfusion Blazor components, and avoid adding icons unnecessarily.
- **If the mapping score is greater than 5**, use the corresponding icon CSS class:`<span className="e-icons e-user"></span>`
- **Never leave empty space** - either icon or emoji, not blank

**Step 2: If Icon Not Found or Score Too Low**
- ❌ DO NOT skip or use empty space
- ✅ Update the element's `icon_hint` in `component-mapping.json`
- ✅ Run the ComponentMapper script again: `py components_search.py ../component-mapping.json`
- ✅ Re-check the script output for improved icon match

**Step 3: If Still Not Found**
- ✅ Use emoji fallback: `<span>📧</span>` or appropriate emoji
- Document why icon wasn't found in code comments
- Maintain visual consistency with other components

**Example**:
```json
// Before: icon_hint was too vague
"icon_hint": "email"

// After: updated with more keywords
"icon_hint": "email envelope mail message send"
```

### Button & Icon Styling with Syncfusion (MANDATORY)

**Principle:** Let Syncfusion own button dimensions and styling. Use Tailwind only for layout around buttons.

❌ **INCORRECT** - Overriding Syncfusion button sizes with Tailwind in Blazor:
```razor
<button class="e-control e-btn e-large p-4 w-32 h-12">
  <span class="e-icons e-play"></span>
  Play
</button>
```

✅ **CORRECT** - Syncfusion owns button, Tailwind owns layout:
```razor
<div class="flex gap-3">
  <SfButton CssClass="e-primary" IsPrimary="true">
    <span class="e-icons e-video"></span>
    Play
  </SfButton>

  <SfButton CssClass="e-outline">
    <span class="e-icons e-circle-info"></span>
    More Info
  </SfButton>
</div>
```

**Why This Works:**
- Syncfusion defines sizing + alignment internally
- Icons align correctly with Syncfusion's design system
- No padding collision or override conflicts
- Consistent appearance across all Syncfusion components

---

### Component Reuse Across UI (Same Component, Multiple Places)

**Principle:** One Syncfusion component type can be reused throughout your UI with customizations. For example, `ButtonComponent` can serve as the Login button, Forgot Password link, and Sign Up button—each customized via CSS variables.

**Example - Button Used in Multiple Places (Blazor):**
```razor
@* LoginForm.razor *@
@using Syncfusion.Blazor.Buttons;

<div class="login-button">
  <SfButton IsPrimary="true">Login</SfButton>
</div>

<div class="forgot-password">
  <SfButton CssClass="e-flat e-small">Forgot Password?</SfButton>
</div>

<div class="sign-up">
  <SfButton CssClass="e-outline">Sign Up Here</SfButton>
</div>
```

```css
/* LoginForm.razor.css */
/* Primary button - main CTA */
.login-button :deep(.e-btn) {
  --bs-primary: #0d6efd;
  width: 100%;
}

/* Flat button - link-style action */
.forgot-password :deep(.e-btn) {
  --bs-primary: #6c757d;
  background: transparent;
  border: none;
  text-decoration: underline;
  font-size: 14px;
}

/* Outline button - secondary action */
.sign-up :deep(.e-btn) {
  --bs-primary: #6c757d;
  border: 1px solid #6c757d;
}
```

---

### Reading Component Skills BEFORE Using generate code (MANDATORY)

**CRITICAL:** Do NOT assume component properties or APIs.

**Required Process:**
1. **Identify all mapped components** from Stage 3 output
   - E.g., GridComponent, ChartComponent, SidebarComponent, etc.
   - **MUST read getting-started for EVERY component in the Stage 3 list** - no exceptions

2. **For EACH component**, read the component skill:
   - Location: `.codestudio/skills/<component-skill>/references/getting-started.md`
   - Extract: imports, style imports, required props, setup code
   - Read: feature-specific guides (filtering, sorting, validation, styling, etc.)

3. **DO NOT generate code without reading** component skill documentation
   - Don't assume prop names or API structure
   - Don't guess at event handler names
   - Don't skip required setup or initialization

**Example - Reading SfGrid Skill:**
```
Before generating code:
1. Read: .codestudio/skills/syncfusion-blazor-datagrid/references/getting-started.md
   → Extract: @using Syncfusion.Blazor.Grids
   → Extract: Required services in Program.cs (e.g., builder.Services.AddSyncfusionBlazor())
   → Read: required parameters, data binding structure, column definitions

2. Read: .codestudio/skills/syncfusion-blazor-datagrid/references/sorting.md
   → Understand: AllowSorting parameter, SortSettings structure

3. Read: .codestudio/skills/syncfusion-blazor-datagrid/references/filtering.md
   → Understand: AllowFiltering parameter, FilterSettings structure

4. NOW generate code with correct @using directives, parameters, and data binding
```

**What Syncfusion Blazor Component Skills Contain:**
- ✅ Authoritative `@using` directives and service registration
- ✅ Complete API documentation (parameters, events, methods)
- ✅ Feature-specific patterns (sorting, filtering, validation)
- ✅ Best practices and performance considerations for Blazor
- ✅ Accessibility requirements
- ✅ Theme customization options for Blazor components

**Common Mistakes to Avoid (Blazor):**
- ❌ Guessing parameter names → Read skill documentation
- ❌ Missing service registration → Extract from getting-started.md and add to Program.cs
- ❌ Wrong event handler names → Copy from component skill examples (e.g., `OnChange`, `OnActionComplete`)
- ❌ Missing `@using` directives → Include all required namespaces from getting-started.md
- ❌ Incomplete service setup → Follow skill's recommended initialization in Program.cs
- ❌ Duplicate `@page` routes in multiple files → Use unique routes per page component
- ❌ Putting pages in wrong folder → Pages go in `Components/Pages/`, components in `Components/Shared/`
- ❌ Missing `@rendermode` in interactive pages → Set render mode at page or app level for interactivity
- ❌ Syncfusion components without render mode → Interactive components need `@rendermode` in Web App
- ❌ Using enum names without namespace qualification → Same enum can exist across different components; always reference the namespace explicitly (e.g., `Syncfusion.Blazor.Grids.GridSortDirection`) to avoid conflicts

---

**Example Output Files (Blazor):**

```
Components/
  ├── Pages/
  │   ├── Login.razor            (Page with @page directive)
  │   ├── Login.razor.css        (Page-scoped styles)
  │   └── Login.razor.cs         (Code-behind, optional)
  │
  └── Shared/
      ├── LoginForm.razor        (Reusable component, NO @page)
      ├── LoginForm.razor.css    (Component-scoped styles)
      └── LoginForm.razor.cs     (Code-behind, optional)
```

**File Organization Rules:**
- **Pages with `@page` directive** → `Components/Pages/` folder
- **Reusable components** → `Components/Shared/` or feature-specific folders
- Each page component gets its own namespace and route

---

## Component Integration & File Mapping

**Generated files MUST be wired to display in the app:**

### ⚠️ CRITICAL: Avoid Duplicate Routes

**DO NOT use the same `@page` route in multiple files.** Each page must have a unique route.

❌ **INCORRECT** - Duplicate routes cause conflicts:
```razor
<!-- Components/Pages/Login.razor -->
@page "/"

<!-- Components/Pages/Home.razor -->
@page "/"    <!-- ❌ CONFLICT! -->
```

✅ **CORRECT** - Unique routes for each page:
```razor
<!-- Components/Pages/Login.razor -->
@page "/login"

<!-- Components/Pages/Home.razor -->
@page "/"
```

---

### 1. **Reusable Component Namespace** (`Components/Shared/LoginForm.razor`):
   ```razor
   @namespace MyApp.Components.Shared
   @using Syncfusion.Blazor.Inputs
   
   <div class="login-container">
     <!-- Component markup -->
   </div>
   ```

### 2. **Page Component with Interactive RenderMode** (`Components/Pages/Login.razor`):
   ```razor
   @page "/login"
   @namespace MyApp.Components.Pages
   @rendermode InteractiveServer
   @using Syncfusion.Blazor.Inputs
   @using MyApp.Components.Shared
   
   <LoginForm />
   ```

**Render Mode Guidance:**
- **`@rendermode InteractiveAuto`** - Server on initial load, WebAssembly after (Blazor Web App)
- **`@rendermode InteractiveServer`** - Always server-side interactivity (requires SignalR)
- **`@rendermode InteractiveWebAssembly`** - Client-side WebAssembly only
- **No `@rendermode`** - Static HTML (no interactivity)

3. **Ensure CSS is loaded**:
   - Component-scoped CSS: Automatically loaded via `LoginForm.razor.css`
   - Framework CSS (Tailwind/Bootstrap): Applied via `class` attributes
   - **Syncfusion theme CSS registration depends on your Blazor project type:**

#### For Blazor WebAssembly (WASM):
Theme CSS already imported in `wwwroot/index.html` from Stage 4:
```html
<!-- In wwwroot/index.html <head> -->
<link href="_content/Syncfusion.Blazor.Themes/tailwind3.css" rel="stylesheet" />
```

#### For Blazor WebApp (Server or Auto-Render):
Theme CSS imported in `Components/App.razor` from Stage 4:
```razor
<!-- In Components/App.razor <head> section -->
<head>
    <link href="_content/Syncfusion.Blazor.Themes/tailwind3.css" rel="stylesheet" />
    <!-- other head content -->
</head>
```

**Without proper namespacing, imports, and theme registration, component won't render in Blazor app.**

## Script Registration (REQUIRED)

**CRITICAL:** Syncfusion Blazor components require JavaScript interop scripts. Missing scripts = unresponsive components.

**Load Order (mandatory):**
1. CSS Themes (in `<head>`)
2. Syncfusion Scripts (in `<body>`, before Blazor bootstrap)
3. Blazor bootstrap script (`_framework/blazor.web.js`, last)

---

#### Blazor WebAssembly (WASM)

**File:** `wwwroot/index.html`

```html
<!DOCTYPE html>
<html>
<head>
    <link href="_content/Syncfusion.Blazor.Themes/tailwind3.css" rel="stylesheet" />
</head>
<body>
    <div id="app"></div>
    
    <!-- Syncfusion scripts (before Blazor) -->
    <script src="_content/Syncfusion.Blazor.Core/scripts/syncfusion-blazor.min.js"></script>
    
    <!-- Blazor bootstrap (last) -->
    <script src="_framework/blazor.web.js"></script>
</body>
</html>
```

---

#### Blazor WebApp (Server or Auto-Render)

**File:** `Components/App.razor`

```razor
<!DOCTYPE html>
<html>
<head>
    <link href="_content/Syncfusion.Blazor.Themes/tailwind3.css" rel="stylesheet" />
</head>
<body>
    <!-- Syncfusion scripts (before Routes) -->
    <script src="_content/Syncfusion.Blazor.Core/scripts/syncfusion-blazor.min.js"></script>
    
    <!-- HeadOutlet for render mode and component configuration -->
    <HeadOutlet @rendermode="InteractiveServer" />
    
    <Routes @rendermode="new InteractiveServerRenderMode()" />
    
    <script src="_framework/blazor.web.js"></script>
</body>
</html>
```

**Render Mode Configuration:**
- **`InteractiveAutoRenderMode`** - Recommended for Web Apps: server on first load, switches to WASM
- **`InteractiveServerRenderMode`** - Server-side interactivity with SignalR
- **`InteractiveWebAssemblyRenderMode`** - Client-side WASM only

**Global Render Mode** (in App.razor) applies to all child routes unless overridden with page-level `@rendermode`.

---

#### Troubleshooting Quick Reference

| Issue | Cause | Fix |
|-------|-------|-----|
| **404: syncfusion-blazor.min.js** | NuGet missing or project not built | `dotnet add package Syncfusion.Blazor.Core && dotnet build` |
| **Components unresponsive** | Script after Blazor bootstrap | Move script **before** `blazor.web.js` |
| **Partial functionality** | Missing component-specific scripts | Check component skill for additional scripts |

**Verify:** DevTools (F12) → Network → Reload → Confirm `syncfusion-blazor.min.js` loads with 200 status before `blazor.web.js`

**📖 Component-Specific Scripts:** RichTextEditor, PDFViewer, FileManager may require additional scripts. Refer to individual component skills.

---

## Syncfusion Blazor Component and Theme Package Installation

**CRITICAL:** After code generation completes, you MUST install all Syncfusion Blazor NuGet packages that were used in the generated code. Run the appropriate `dotnet add package` command for each package identified during the component skill reading phase (Stage 5, Steps 1-4).

Example:
```bash
dotnet add package Syncfusion.Blazor.Grid
dotnet add package Syncfusion.Blazor.Charts
dotnet add package Syncfusion.Blazor.Buttons
dotnet add package Syncfusion.Blazor.Inputs
dotnet add package Syncfusion.Blazor.Themes

```

**Installation locations:**
- NuGet packages installed via `dotnet add package` 
- Syncfusion CSS themes pre-configured in `Program.cs` via `builder.Services.AddSyncfusionBlazor()`
- Theme CSS imported in `wwwroot/index.html` (from Stage 4)

**Without installing these packages, the generated Blazor code will fail to compile and render.**

---

## Best Practices Checklist

Before deploying generated Blazor components, verify:

### ✅ File Organization
- [ ] Page components are in `Components/Pages/` folder
- [ ] Reusable components are in `Components/Shared/` or feature folders
- [ ] Each component has its own namespace matching folder structure

### ✅ Routing & Navigation
- [ ] Each page has a **unique** `@page` route (no duplicates)
- [ ] Child component routes don't conflict with parent routes
- [ ] `@page` directive only in page components, NOT in reusable components

### ✅ Interactive Render Mode (Web App)
- [ ] Global render mode set in `Components/App.razor` (`InteractiveServerRenderMode` recommended)
- [ ] Page components have `@rendermode` directive if they need interactivity
- [ ] Syncfusion interactive components are inside pages with `@rendermode`
- [ ] `<HeadOutlet />` included in `App.razor` for per-component metadata
- [ ] If using `InteractiveServer`: Configure SignalR in `Program.cs` and ensure server can handle concurrent WebSocket connections

### ✅ Syncfusion Integration
- [ ] All required NuGet packages installed via `dotnet add package`
- [ ] Service registration in `Program.cs` (`AddSyncfusionBlazor()`)
- [ ] Theme CSS imported in `App.razor` or `wwwroot/index.html`
- [ ] Scripts registered: `syncfusion-blazor.min.js` before Blazor bootstrap
- [ ] All component `@using` directives included from getting-started.md

### ✅ Code Quality
- [ ] Accessibility: ARIA labels, semantic HTML, keyboard navigation
- [ ] Error handling: Try-catch in async methods, user-friendly messages
- [ ] Performance: `@key` for lists, conditional `shouldRender`
- [ ] Security: HTML-encoded outputs, no hardcoded secrets

---

**User Interaction:** 
Optional review of generated code. No blocking confirmation.

**Status:** AI generates without user decision. User can review/adjust if needed.
