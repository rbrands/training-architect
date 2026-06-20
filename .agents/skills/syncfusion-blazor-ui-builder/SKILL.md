---
name: syncfusion-blazor-ui-builder
description: Generates production-ready webpages powered by Syncfusion Blazor Components. Orchestrates 8-stage workflow that handles design thinking, component picking, code generation, and validation with built-in WCAG 2.1 AA accessibility and responsive design. Use when the user asks to create web components, build UI pages, design interfaces, or generate frontend code for Blazor applications.
metadata:
  author: "Syncfusion Inc"
  version: "1.0"
---

# Building Blazor UI

## Overview

The **Building Blazor UI** skill is a frontend-only Blazor component generator that orchestrates an AI agent through 8 stages to generate production-ready UI components powered by Syncfusion Blazor.

## What This Skill Does

**✅ Generates:**
- Blazor components (`.razor` files) with C# code-behind and reactive properties
- CSS stylesheets (scoped `.razor.css` files, Tailwind utilities, custom CSS)
- C# models and records for component parameters, state, and event handlers
- Syncfusion Blazor component integration with correct @using directives and parameters
- Client-side form validation logic with C# data annotations
- WCAG 2.1 AA accessibility markup (ARIA attributes, semantic HTML)
- Responsive CSS with mobile-first breakpoints
- Component namespace organization and exports

**❌ Does NOT Generate:**
- Backend code (API endpoints, server handlers, middleware)
- Database schemas or EF Core models
- Authentication/authorization logic (generates UI forms only)
- Server-side validation
- Routing configuration
- Environment secrets or infrastructure config

## Quick Start

### Prerequisites

1. **Active Blazor project** (.NET 8.0+, Blazor WebAssembly or WebApp)
2. **.NET CLI** installed (run `dotnet --version` to verify)
3. **Syncfusion Blazor NuGet packages** (auto-installed if missing):
   ```bash
   dotnet add package Syncfusion.Blazor
   ```

### Basic Usage

**Example 1: Generate a Login Form**

```
User: "Create a login form with email, password, and remember me checkbox"

Skill executes:
  → Stage 1: Identifies login form component type
  → Stage 2: Detects project structure (.NET version, Blazor type, CSS framework)
  → Stage 3-4: AI creates optimal component-mapping.json → maps to Syncfusion Blazor components
  → Stage 5: Generates LoginForm.razor with validation
  → Stage 6: Validates WCAG 2.1 AA compliance
  → Stage 7: Installs NuGet dependencies
  → Stage 8: Inserts code into project

Output:
  ✓ Components/LoginForm/LoginForm.razor
  ✓ Components/LoginForm/LoginForm.razor.css
  ✓ Mock data included (C# properties with sample data)
```

**Example 2: Generate a Data Table**

```
User: "Build a customer data table with sorting and filtering"

Output:
  ✓ Components/CustomerTable/CustomerTable.razor (with Syncfusion SfGrid)
  ✓ Mock data included (C# List<Customer> with sample records)
  ✓ Responsive design (mobile-first)
  ✓ Accessibility compliance
```

## How It Works: 8-Stage AI Orchestration (Stateless)

The skill orchestrates **8 stages of pure AI reasoning** with **two user decision points**.

**Key Architecture:**
- **Stateless design**: Conversation history maintains state
- **Pure AI reasoning**: Each stage reads guidance docs, analyzes context, makes decisions
- **2 user decision gates**: Stage 3 (component confirmation) + Stage 6 (validation result)
- **6 fully automated stages**: 1, 2, 4, 5, 7, and final code insertion
- **Dedicated theming stage**: Stage 4 locks design system before code generation

**⚠️ CRITICAL WORKFLOW ORDER - DO NOT DEVIATE:**

```
User Request
    ↓
[Stage 1: Intent Analysis] 
  AI reads query → identifies component type & features
    ↓
[Stage 2: Project Detection]
  AI scans .csproj, Program.cs → detect .NET version, Blazor type, CSS strategy, preferences
    ↓
[Stage 3: Layout Analysis & Component Mapping] ⭐ USER DECISION #1
  AI analyzes requirements → creates optimal component-mapping.json
  AI maps to specific Syncfusion Blazor components (3+ components with Sf* prefix)
  User confirms component selection
    ↓
[Stage 4: Theming & Design System]
  AI locks CSS framework → Syncfusion Blazor theme mapping
  AI selects color system (OKLCH, brand colors)
  AI confirms spacing/typography scale (4pt grid, 1.25 ratio)
  AI confirms theme import location (wwwroot/index.html for WASM, Components/App.razor for WebApp)
  Design system decisions locked before code generation
    ↓
[Stage 5: Code Generation]
  AI generates .razor, .razor.css, C# models
  Uses theming decisions from Stage 4
  With accessibility + responsive design built-in
  Includes @code block with C# properties and lifecycle methods
    ↓
[Stage 6: Validation] ⭐ USER DECISION #2
  AI validates WCAG 2.1 AA + security + performance + theming
  Binary result: PASS ✓ or FAIL ✗
  User confirms or overrides
    ↓
[Stage 7: Dependencies]
  AI detects required NuGet packages (Syncfusion.Blazor.* + CSS framework)
  Presents dotnet add package commands or runs them
    ↓
[Stage 8: Code Insertion]
  AI inserts files into Components/ folder
  Updates @using directives and Program.cs services
  Runs dotnet build verification
    ↓
✓ Complete
```

**Stage Descriptions:**

- **Stage 1 (Intent Analysis)**: Parse user query, identify component type and features. Read: `references/stage-1-intent-analysis.md`
- **Stage 2 (Project Detection)**: Auto-detect .NET version, Blazor type (WASM/WebApp), CSS strategy, component directory. Read: `references/stage-2-project-detection.md`
- **Stage 3 (Layout Analysis & Component Mapping)**: AI analyzes requirements, creates optimal component-mapping.json, maps to Syncfusion Blazor components (Sf* prefix). User confirms 3+ component selection. Read: `references/stage-3-layout-analysis.md`
- **Stage 4 (Theming & Design System)**: Lock CSS framework, Syncfusion Blazor theme, color system (OKLCH), spacing (4pt), typography (1.25 ratio), theme import location (WASM vs WebApp). Read: `references/stage-4-theming-and-design-system.md`
- **Stage 5 (Code Generation)**: Generate .razor files with C# @code blocks and design tokens from Stage 4 applied + accessibility + responsive design. Read: `references/stage-5-code-generation.md`
- **Stage 6 (Validation)**: Validate WCAG 2.1 AA, security, performance, theming integration. Binary pass/fail. Read: `references/stage-7-validation.md` + `assets/validation-rules.md`
- **Stage 7 (Dependencies)**: Detect NuGet packages (Syncfusion.Blazor.* + CSS framework), resolve conflicts, prepare dotnet install commands. Read: `references/stage-6-dependencies.md` + `references/nuget-packages.md`
- **Stage 8 (Code Insertion)**: AI inserts files into Components/ folder, updates @using directives and Program.cs services, runs `dotnet build` verification.

**User Interaction Summary:**

| Stage | Interaction |
|-------|-------------|
| 1 | None (AI analyzes) |
| 2 | Confirm auto-detected settings |
| 3 | ⭐ Confirm component selection (3+ Syncfusion Blazor components) |
| 4 | Confirm theming decisions (CSS framework, colors, spacing, typography, WASM vs WebApp) |
| 5 | None (AI generates) |
| 6 | ⭐ Confirm validation result (pass/fail/override) |
| 7 | Optional (confirm dotnet add package) |
| 8 | None (AI executes) |

**Total user decision gates: 2** (Stage 3: components, Stage 6: validation). Rest fully automated with AI reasoning + guidance docs.

## Agent Instructions

### When User Requests UI Component Generation

1. **Validate scope**: Confirm request is for frontend components (not backend/API)
2. **Load guidance**: Read `stage-1-intent-analysis.md` to understand Stage 1
3. **Execute 8-stage flow**: Follow the orchestration flow shown above
4. **Progressive disclosure**: Load stage guides on-demand; load support references only when needed
5. **Maintain conversation history**: Each stage reads previous decisions from conversation context (stateless)

### Stage Execution & Reference Loading

**Stage 1: Intent Analysis**
- Read: `references/stage-1-intent-analysis.md`
- Task: Parse user query, identify component type, resolve ambiguities
- Output: Component type + modifiers + target directory

**Stage 2: Project Detection**
- Read: `references/stage-2-project-detection.md`
- Task: Auto-detect Blazor framework, language, CSS strategy, formatting rules
- Output: Project configuration + user confirmation

**Stage 3: Layout Analysis & Component Mapping** ⭐ USER DECISION #1
- Read: `references/stage-3-layout-analysis.md`
- Task: Analyze user requirements → create optimal component-mapping.json → map to Syncfusion components (3+ minimum)
- Output: component-mapping.json + Component mapping + User confirmation

**Stage 4: Theming & Design System** (NEW)
- Read: `references/stage-4-theming-and-design-system.md`
- Task: Lock CSS framework, Syncfusion theme, color system (OKLCH), spacing (4pt grid), typography (1.25 ratio), responsive breakpoints
- Output: Design system decisions confirmed and ready for code generation

**Stage 5: Code Generation**
- Read: `references/stage-5-code-generation.md`
- Task: Generate Blazor .tsx, CSS, TypeScript interfaces using theming from Stage 4
- Ensure: WCAG 2.1 AA accessibility, responsive design, token architecture applied
- Output: Generated files ready for review

**Stage 6: Validation** ⭐ USER DECISION #2
- Read: `references/stage-7-validation.md` + `assets/validation-rules.md` + `references/WEB-STANDARDS.md`
- Task: Validate against WCAG 2.1 AA, security, performance, theming integration standards
- Auto-apply fixes where possible
- Output: Binary result (PASS ✓ or FAIL ✗) → user confirms or overrides

**Stage 7: Dependencies**
- Read: `references/stage-6-dependencies.md` + `references/nuget-packages.md`
- Task: Detect required packages (Syncfusion + CSS framework), resolve version conflicts
- Output: npm install command or auto-install

**Stage 8: Code Insertion**
- Task: Insert generated files into project, update imports, verify build
- Output: Success report with file paths



### Boundary Rules (CRITICAL)

**AI agents executing this skill MUST:**

1. **Frontend only**: Never generate backend code (API endpoints, database schemas, middleware)
2. **Mock data only**: Use C# properties with hardcoded samples; no `HttpClient.GetAsync()` to real APIs
3. **No secrets**: Exception: `appsettings.json` or `appsettings.Development.json` for `SyncfusionLicenseKey` when user provides
4. **Blazor components only**: Generate `.razor` files in Components/ directory
5. **Redirect backend requests**: *"This skill generates frontend UI only. Backend integration is your app's responsibility. Ready to generate the frontend?"*

### Error Handling

If any stage fails:

1. **Retry once** with same approach
2. **If retry fails**, attempt workaround or skip to next stage
3. **Notify user** with error message from stage output
4. **Offer recovery**: *"Would you like to go back to Stage 3 and choose a different layout?"*
5. **Reference**: `references/build.md` for common errors

### Resource Loading Strategy (MANDATORY: Read All Skills and References Completely)

**Load SKILL.md first** (you're reading it now) ~400 lines

**Load stage guides on-demand** (each <200 lines):
- `stage-1-intent-analysis.md` → During Stage 1
- `stage-2-project-detection.md` → During Stage 2
- `stage-3-layout-analysis.md` → During Stage 3
- etc.

**Load support references only when needed**:
- `web-standards.md` → When validating in Stage 5
- `build.md` → When errors occur
- `assets/validation-rules.md` → When validating in Stage 5

**Result**: Initial load ~400 lines (SKILL.md only). Full spec available on-demand, never exceeding Agent Skills context limits.

## Configuration & User Customization

### Auto-Detected Settings

During **Stage 2 (Project Detection)**, AI automatically detects:

- **.NET Version**: .NET 8.0, .NET 9.0, etc. (from `.csproj`)
- **Blazor Type**: WebAssembly (WASM) or WebApp (Server/Auto-Render)
- **CSS Framework**: Tailwind CSS, Bootstrap 5, Material Design, or Greenfield custom
- **Styling Strategy**: Scoped `.razor.css`, class attributes, or inline styles
- **Formatting**: EditorConfig rules (indentation, line endings)
- **Component Directory**: `Components/`, structure from `.csproj`

### User Override Options

In **Stage 2**, user can override any detected setting:

```
Detected Settings:
  .NET Version: 8.0
  Blazor Type: WebAssembly
  CSS Framework: Tailwind CSS
  Component Directory: Components/

[Confirm] [Override Each] [Cancel]
```

### Syncfusion License Configuration

The skill handles license key setup:

1. **Check** for existing `SyncfusionLicenseKey` in `appsettings.Development.json`
2. **If missing**, prompt user: *"Get a free Community License at https://www.syncfusion.com/account/manage-trials"*
3. **If provided**, write to `appsettings.Development.json` + inject `Syncfusion.Licensing.RegisterLicense()` in `Program.cs`
4. **If skipped**, proceed but warn that watermark will appear in preview

---

## Code Generation Standards

All generated code includes:

### Accessibility (WCAG 2.1 AA)
- ✅ Semantic HTML5 (`<form>`, `<label>`, `<input>`, `<button>`)
- ✅ ARIA labels and descriptions
- ✅ Keyboard navigation (tab order, focus management with `@ref`)
- ✅ Color contrast ≥ 4.5:1
- ✅ Focus indicators on interactive elements

### Responsive Design
- ✅ Mobile-first CSS (320px base, then scale up)
- ✅ Flexbox/Grid layouts (no fixed widths)
- ✅ Media queries at 768px, 1024px+ breakpoints
- ✅ Touch-friendly buttons (44x44px minimum)

### Security
- ✅ Input validation with C# data annotations
- ✅ No `@((MarkupString)html)` without sanitization
- ✅ No hardcoded secrets
- ✅ No XSS vulnerabilities

### Performance
- ✅ `@key` directive for list rendering
- ✅ `ShouldRender()` for conditional re-renders
- ✅ `OnInitializedAsync()` for async initialization
- ✅ Virtualization for large lists

### C# & Type Safety
- ✅ Full type coverage (no dynamic types without validation)
- ✅ `[Parameter]` attributes for component inputs
- ✅ `EventCallback<T>` for event handlers
- ✅ C# nullable reference types (`#nullable enable`)

## Supported Use Cases

- **Login form**: SfTextBox (email), SfTextBox (password), SfCheckBox (remember), SfButton (submit)
- **Data table**: SfGrid with sorting, filtering, paging, row selection
- **Dashboard**: Multiple components orchestrated (header, sidebar, main content, footer)
- **Registration wizard**: Multi-step form with SfStepper and validation

## Troubleshooting

**Common Issues:**

| Issue | Solution |
|-------|----------|
| "Project type not detected" | Ensure `.csproj` exists with Blazor/Syncfusion dependencies |
| "Syncfusion license watermark appears" | Add license key via Stage 2 prompt |
| "Build fails after insertion" | Check `references/build.md` for conflict resolution |
| "Component not rendering" | Verify @using directives and ensure parent component imports correctly |

**Full guide**: See `references/build.md`

## Additional Resources

### Quick Reference by Use Case

| Need | Reference File |
|------|-----------------|
| Understanding workflow | This SKILL.md file |
| How Stage X works | `references/stage-X-*.md` |
| Validation rules | `assets/validation-rules.md` |
| Accessibility/security | `references/WEB-STANDARDS.md` |
| Troubleshooting | `references/build.md` |

## Support

For issues or questions:
1. Check `references/build.md` for common problems
2. Verify your project meets prerequisites (.NET 8.0+, Blazor WebAssembly or WebApp)
3. Ensure Syncfusion Blazor license is valid and registered
4. Review generated code compliance report for warnings

---