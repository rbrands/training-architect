# Bootstrap 5 in Blazor Projects: Stage 4 Framework Implementation

**Back to:** [Stage 4: Core Design System Decisions](stage-4-theming-and-design-system.md)

**Framework:** Bootstrap 5 + Blazor (Component-First, Utility-First)  
**Syncfusion Theme:** Bootstrap5 for Blazor  
**Implementation Stage:** Stage 4 (Design System) → Stage 5 (Code Generation) → Stage 7 (Validation)

---

## Overview

Bootstrap 5 is a component framework with utility-first styling for Blazor. You use semantic color names, 8pt spacing grid, and responsive utilities to build consistent UIs in `.razor` components.

**Output:** Bootstrap 5 design system decisions documented for Stage 5 Razor component generation.

---

## Table of Contents

1. [Setup & Installation](#setup--installation)
2. [Bootstrap Philosophy](#bootstrap-philosophy)
3. [Color System (Semantic)](#color-system-semantic)
4. [Spacing Grid (8pt)](#spacing-grid-8pt)
5. [Typography System](#typography-system)
6. [Responsive Breakpoints](#responsive-breakpoints)
7. [Grid System](#grid-system)
8. [Configuration](#configuration)
9. [Stage 5: Code Generation](#stage-5-code-generation)
10. [Stage 7: Validation Checklist](#stage-7-validation-checklist)

---

## Setup & Installation

### 1. Install Bootstrap NuGet Package

```bash
dotnet add package Bootstrap
dotnet add package Syncfusion.Blazor.Themes
```

### 2. Create CSS Configuration (`wwwroot/css/bootstrap-config.css`)

```css
/* Bootstrap 5 Custom Properties */
:root {
  /* Semantic Colors */
  --bs-primary: #0891b2;
  --bs-secondary: #536d7e;
  --bs-success: #10b981;
  --bs-danger: #ef4444;
  --bs-warning: #f59e0b;
  --bs-info: #3b82f6;
  --bs-light: #f9fafb;
  --bs-dark: #1f2937;
  
  /* Spacing (8pt grid) */
  --bs-spacer: 1rem; /* 16px */
  
  /* Typography */
  --bs-body-font-size: 16px;
  --bs-body-line-height: 1.5;
}

/* Import Bootstrap from node_modules via wwwroot */
@import url('../../node_modules/bootstrap/dist/css/bootstrap.min.css');
```

### 3. Register Services in `Program.cs`

```csharp
// Program.cs
var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

// Add Syncfusion Blazor services
builder.Services.AddSyncfusionBlazor();

await builder.Build().RunAsync();
```

### 4. Import Styles

**For Blazor WebAssembly (WASM) - Use `wwwroot/index.html`:**

```html
<!DOCTYPE html>
<html lang="en">
<head>
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <title>Blazor Bootstrap App</title>
    
    <!-- Bootstrap CSS -->
    <link href="_content/Bootstrap/css/bootstrap.min.css" rel="stylesheet" />
    
    <!-- Syncfusion Bootstrap5 Theme -->
    <link href="_content/Syncfusion.Blazor.Themes/bootstrap5.css" rel="stylesheet" />
    
    <!-- Custom Bootstrap config -->
    <link href="css/bootstrap-config.css" rel="stylesheet" />
    
    <link href="app.css" rel="stylesheet" />
</head>
<body>
    <div id="app"></div>
    <script src="_framework/blazor.web.js"></script>
</body>
</html>
```

**For Blazor WebApp (Server or Auto-Render) - Use `Components/App.razor`:**

```razor
@* In Components/App.razor *@

<head>
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <title>Blazor Bootstrap App</title>
    
    <!-- Bootstrap CSS -->
    <link href="_content/Bootstrap/css/bootstrap.min.css" rel="stylesheet" />
    
    <!-- Syncfusion Bootstrap5 Theme -->
    <link href="_content/Syncfusion.Blazor.Themes/bootstrap5.css" rel="stylesheet" />
    
    <!-- Custom Bootstrap config -->
    <link href="css/bootstrap-config.css" rel="stylesheet" />
    
    <link href="app.css" rel="stylesheet" />
</head>

<body>
    <Routes />
    @* Other app content *@
</body>
```

---

## Bootstrap Philosophy

**Utility-First Foundation:** Bootstrap provides utilities for spacing, colors, display, and responsive design in Razor markup.

- **Semantic Colors:** 9 semantic names (primary, secondary, success, danger, warning, info, light, dark, body)
- **8pt Spacing Grid:** Base unit is 1rem (16px); scale: 0.25x, 0.5x, 1x, 1.5x, 2x, 3x
- **Responsive Utilities:** Mobile-first prefixes (no prefix for mobile, `sm:`, `md:`, `lg:`, `xl:`, `xxl:` for larger)
- **12-Column Grid:** Layout system with responsive breakpoints
- **Component Utilities:** Reusable patterns for common UI elements

**You're committing to:** Bootstrap's conventions in Razor `.class` attributes for speed and consistency.

---

## Color System (Semantic)

### Bootstrap Semantic Colors

**9 Named Colors:**
```ts
primary:   #0891b2  // Brand color, CTAs
secondary: #536d7e  // Secondary actions
success:   #10b981  // Success states
danger:    #ef4444  // Errors, destructive
warning:   #f59e0b  // Warnings
info:      #3b82f6  // Information
light:     #f9fafb  // Light backgrounds
dark:      #1f2937  // Dark backgrounds
body:      #1c1b1f  // Body text
```

### Usage as CSS Classes/Variables

```razor
@* Background colors *@
<div class="bg-primary">Primary background</div>
<div class="bg-success">Success background</div>
<div class="bg-danger">Danger background</div>

@* Text colors *@
<p class="text-primary">Primary text</p>
<p class="text-secondary">Secondary text</p>

@* Borders *@
<div class="border border-primary">Primary border</div>
<div class="border border-success">Success border</div>

@* CSS variables *@
<div style="background-color: var(--bs-primary); color: var(--bs-light);">
  Using Bootstrap CSS variables
</div>
```

### Dark Mode

Toggle dark mode by adding `data-bs-theme="dark"` to `<html>`:

```razor
@inject IJSRuntime JS

<button @onclick="ToggleDarkMode">Toggle Dark Mode</button>

@code {
  private async Task ToggleDarkMode()
  {
    var isDark = await JS.InvokeAsync<bool>("eval", 
      "document.documentElement.getAttribute('data-bs-theme') === 'dark'");
    
    var newTheme = isDark ? "light" : "dark";
    await JS.InvokeVoidAsync("eval", 
      $"document.documentElement.setAttribute('data-bs-theme', '{newTheme}')");
  }
}
```

---

## Spacing Grid (8pt)

Bootstrap uses **1rem = 16px** as base. Scale multipliers: 0.25x, 0.5x, 1x, 1.5x, 2x, 3x...

### Spacing Scale

```ts
--bs-spacer: 1rem (16px)

xs: 4px   (0.25rem)
sm: 8px   (0.5rem)
md: 16px  (1rem)
lg: 24px  (1.5rem)
xl: 32px  (2rem)
2xl: 48px (3rem)
```

### Usage as Utilities

```razor
@* Padding *@
<div class="p-3">Padding 16px</div>
<div class="p-md">Padding 16px (Bootstrap alias)</div>
<div class="px-4 py-2">Horizontal 24px, vertical 8px</div>

@* Margin *@
<div class="m-2">Margin 8px</div>
<div class="mt-3 mb-2">Margin top 16px, bottom 8px</div>
<div class="ms-auto">Margin-left auto (push right)</div>

@* Gap (flexbox/grid) *@
<div class="d-flex gap-3">Gap 16px</div>
<div class="row g-4">Grid gutter 24px</div>
```

**Rule:** Use Bootstrap spacing scale only. Never arbitrary classes like `p-18` or custom padding values.

---

## Typography System

### Bootstrap Typography Scale

```ts
Body text (base):      16px / 1.5 line-height / 400 weight
Small text:            14px / 1.5 line-height / 400 weight
Large text:            18px / 1.5 line-height / 400 weight

Display 1:             56px / 1.2 line-height / 300 weight
Display 2:             48px / 1.2 line-height / 300 weight
Display 3:             40px / 1.2 line-height / 300 weight
Display 4:             32px / 1.2 line-height / 300 weight
Display 5:             28px / 1.2 line-height / 300 weight
Display 6:             24px / 1.2 line-height / 300 weight

H1: 36px / 700 weight
H2: 32px / 700 weight
H3: 28px / 700 weight
H4: 24px / 700 weight
H5: 20px / 700 weight
H6: 16px / 700 weight

Lead: 20px / 1.5 line-height (emphasized body)
Small/Muted: 14px / 400 weight
```

### Usage as Classes

```razor
@* Headings *@
<h1>H1 automatic Bootstrap styling</h1>
<h2 class="display-3">Display text</h2>

@* Body text utilities *@
<p class="lead">Lead text (emphasized)</p>
<p class="small">Small text</p>
<p class="text-muted">Muted text</p>

@* Custom sizing *@
<p class="fs-3">Font size 3 (28px)</p>
<p class="fw-bold">Font weight bold</p>
```

---

## Responsive Breakpoints

### Bootstrap's 6 Breakpoints

```ts
xs:  < 576px   (mobile, default)
sm:  ≥ 576px   (small phones)
md:  ≥ 768px   (tablets)
lg:  ≥ 992px   (desktops)
xl:  ≥ 1200px  (large desktops)
xxl: ≥ 1400px  (extra large)
```

### Mobile-First Utilities

```razor
@* Responsive padding *@
<div class="p-2 p-md-3 p-lg-4">
  Mobile 8px → Tablet 16px → Desktop 24px
</div>

@* Responsive display *@
<div class="d-none d-md-block">Hide on mobile, show on tablet+</div>
<div class="d-md-none">Show on mobile, hide on tablet+</div>

@* Responsive grid columns *@
<div class="row">
  <div class="col-12 col-md-6 col-lg-4">Responsive column</div>
</div>

@* Responsive text alignment *@
<p class="text-start text-md-center text-lg-end">
  Left on mobile → center on tablet → right on desktop
</p>
```

**Rule:** Start with mobile (base classes), add `-md-`, `-lg-`, `-xl-` suffixes for larger screens.

---

## Grid System

### 12-Column Responsive Grid

```razor
@* Basic grid *@
<div class="row">
  <div class="col">Auto-width column</div>
  <div class="col">Auto-width column</div>
</div>

@* Responsive columns *@
<div class="row">
  <div class="col-12 col-md-6 col-lg-4">
    Mobile: 100% width
    Tablet: 50% width
    Desktop: 33% width
  </div>
</div>

@* Grid with gutters *@
<div class="row g-3">@* 16px gutter *@
  <div class="col-md-6"></div>
  <div class="col-md-6"></div>
</div>

@* Nested grid *@
<div class="row">
  <div class="col-md-8">
    <div class="row">
      <div class="col-6">Nested</div>
      <div class="col-6">Nested</div>
    </div>
  </div>
</div>
```

---

## Configuration

All Bootstrap decisions live in:
- `wwwroot/css/bootstrap-config.css` — CSS custom properties (colors, spacing, typography)
- Bootstrap CSS variables — Runtime access via `var(--bs-primary)`, etc.
- `Program.cs` — Syncfusion service registration
- `wwwroot/index.html` — CSS imports for Bootstrap and Syncfusion themes

---

## Stage 5: Code Generation

### Generated Components Use Bootstrap Utilities

```razor
@* ✅ CORRECT: Bootstrap semantic colors + utilities *@
<div class="bg-primary text-white p-4 rounded">
  Primary card with padding
</div>

<div class="row g-3">
  <div class="col-md-6">
    <p class="text-secondary small">Secondary text</p>
  </div>
</div>

@* Status colors *@
<span class="badge bg-success">Success</span>
<span class="badge bg-danger">Error</span>
<span class="badge bg-warning">Warning</span>
```

### Syncfusion Blazor Components

Syncfusion Bootstrap5 theme automatically styles components:

```razor
@using Syncfusion.Blazor.Grids

<SfGrid TValue="Employee" DataSource="Employees" AllowPaging="true" AllowSorting="true">
  @* Inherits Bootstrap5 styling automatically *@
  <GridColumns>
    <GridColumn Field="Name" Header="Name" Width="150"></GridColumn>
    <GridColumn Field="Email" Header="Email" Width="150"></GridColumn>
  </GridColumns>
</SfGrid>

@code {
  private List<Employee> Employees { get; set; } = new();

  protected override async Task OnInitializedAsync()
  {
    Employees = await GetEmployeesAsync();
  }

  private async Task<List<Employee>> GetEmployeesAsync()
  {
    // Fetch data
    return new List<Employee>();
  }
}
```

---

## Stage 7: Validation Checklist

- ✅ All colors use Bootstrap semantic names (primary, secondary, success, danger, warning, info)
- ✅ All spacing from Bootstrap scale (xs, sm, md, lg, xl, 2xl)
- ✅ Typography uses Bootstrap scale (body, h1-h6, display, lead)
- ✅ Responsive utilities follow mobile-first pattern (`col-md-6`, `d-md-block`)
- ✅ Grid system uses 12 columns correctly
- ✅ No hardcoded colors, spacing, or font sizes in `.razor` components
- ✅ Dark mode works with `data-bs-theme="dark"` toggle via JavaScript interop
- ✅ Text contrast ≥ 4.5:1 (WCAG AA)
- ✅ Touch targets ≥ 44x44px (Bootstrap ensures this by default)
- ✅ Syncfusion Bootstrap5 theme imported in `wwwroot/index.html`
- ✅ Syncfusion services registered in `Program.cs`
- ✅ All Syncfusion Blazor components have `@using` directives
- ✅ Components use `[Parameter]` for data binding
- ✅ Event callbacks properly typed (e.g., `EventCallback<GridActionEventArgs>`)
- ✅ Build succeeds without CSS conflicts
- ✅ Blazor component lifecycle methods (`OnInitializedAsync`, `OnParametersSetAsync`) properly implemented
