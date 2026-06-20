# Material Design 3 Projects: Stage 4 Framework Implementation

**Back to:** [Stage 4: Core Design System Decisions](stage-4-theming-and-design-system.md)

**Framework:** Material Design 3 + Blazor (System-First, Token-First)  
**Syncfusion Theme:** Material3 for Blazor  
**Implementation Stage:** Stage 4 (Design System) → Stage 5 (Code Generation) → Stage 7 (Validation)

---

## Overview

Material Design 3 is a comprehensive design system with specific rules for spacing, color, elevation, and typography. You apply Material tokens as utilities to build consistent UIs.

**Output:** Material Design 3 token system documented for Stage 5 code generation.

---

## Table of Contents

1. [Setup & Installation](#setup--installation)
2. [Material Design 3 Philosophy](#material-design-3-philosophy)
3. [Color System (Tokens)](#color-system-tokens)
4. [Spacing Grid (4dp)](#spacing-grid-4dp)
5. [Typography Roles](#typography-roles)
6. [Responsive Breakpoints](#responsive-breakpoints)
7. [Elevation & Surfaces](#elevation--surfaces)
8. [Configuration](#configuration)
9. [Stage 5: Code Generation](#stage-5-code-generation)
10. [Stage 7: Validation Checklist](#stage-7-validation-checklist)

---

## Setup & Installation

### 1. Install Syncfusion NuGet Package

```bash
dotnet add package Syncfusion.Blazor
```

### 2. Create Material Tokens File in C#

Create `Components/MaterialTokens.cs`:

```csharp
// Material 3 Design Tokens
public static class MaterialTokens
{
  public static class Colors
  {
    public const string Primary = "#0891b2";
    public const string OnPrimary = "#ffffff";
    public const string PrimaryContainer = "#cffafe";
    public const string OnPrimaryContainer = "#001f26";
    
    public const string Secondary = "#536d7e";
    public const string OnSecondary = "#ffffff";
    public const string SecondaryContainer = "#d8f1ff";
    public const string OnSecondaryContainer = "#001f2e";
    
    public const string Tertiary = "#6b5b95";
    public const string OnTertiary = "#ffffff";
    public const string TertiaryContainer = "#eaddff";
    public const string OnTertiaryContainer = "#22005d";
    
    public const string Error = "#ef4444";
    public const string OnError = "#ffffff";
    public const string ErrorContainer = "#ffebe9";
    public const string OnErrorContainer = "#410e0b";
    
    public const string Surface = "#fffbfe";
    public const string OnSurface = "#1c1b1f";
    public const string SurfaceContainerLowest = "#ffffff";
    public const string SurfaceContainerLow = "#f7f2f6";
    public const string SurfaceContainer = "#f3eef2";
    public const string SurfaceContainerHigh = "#ede9ed";
    public const string SurfaceContainerHighest = "#e8e3e7";
  }
  
  public static class Spacing
  {
    public const string XS = "4px";   // 1dp
    public const string SM = "8px";   // 2dp
    public const string MD = "12px";  // 3dp
    public const string LG = "16px";  // 4dp
    public const string XL = "24px";  // 6dp
    public const string XXL = "32px"; // 8dp
    public const string XXXL = "48px"; // 12dp
  }
  
  public static class Elevation
  {
    public const string Level0 = "none";
    public const string Level1 = "0px 1px 3px rgba(0, 0, 0, 0.12)";
    public const string Level2 = "0px 3px 6px rgba(0, 0, 0, 0.16)";
    public const string Level3 = "0px 6px 10px rgba(0, 0, 0, 0.2)";
    public const string Level4 = "0px 10px 15px rgba(0, 0, 0, 0.24)";
    public const string Level5 = "0px 15px 22px rgba(0, 0, 0, 0.28)";
  }
}
```

### 3. Register Services in Program.cs

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

### 4. Create CSS Variables in `wwwroot/css/material3-tokens.css`

```css
:root {
  /* Material 3 Color Tokens */
  --color-primary: #0891b2;
  --color-on-primary: #ffffff;
  --color-primary-container: #cffafe;
  --color-on-primary-container: #001f26;
  
  --color-secondary: #536d7e;
  --color-on-secondary: #ffffff;
  --color-secondary-container: #d8f1ff;
  --color-on-secondary-container: #001f2e;
  
  --color-tertiary: #6b5b95;
  --color-on-tertiary: #ffffff;
  --color-tertiary-container: #eaddff;
  --color-on-tertiary-container: #22005d;
  
  --color-error: #ef4444;
  --color-on-error: #ffffff;
  --color-error-container: #ffebe9;
  --color-on-error-container: #410e0b;
  
  --color-surface: #fffbfe;
  --color-on-surface: #1c1b1f;
  --color-surface-container: #f3eef2;
  --color-surface-container-high: #ede9ed;
  
  /* Spacing (4dp grid) */
  --space-xs: 4px;
  --space-sm: 8px;
  --space-md: 12px;
  --space-lg: 16px;
  --space-xl: 24px;
  --space-2xl: 32px;
  --space-3xl: 48px;
  
  /* Elevation */
  --elevation-1: 0px 1px 3px rgba(0, 0, 0, 0.12);
  --elevation-2: 0px 3px 6px rgba(0, 0, 0, 0.16);
  --elevation-3: 0px 6px 10px rgba(0, 0, 0, 0.2);
  --elevation-4: 0px 10px 15px rgba(0, 0, 0, 0.24);
  --elevation-5: 0px 15px 22px rgba(0, 0, 0, 0.28);
}

/* Dark Mode */
@media (prefers-color-scheme: dark) {
  :root {
    --color-surface: #1c1b1f;
    --color-on-surface: #fffbfe;
    --color-surface-container: #49454e;
    --color-surface-container-high: #49454e;
  }
}
```

### 5. Import Styles

**For Blazor WebAssembly (WASM) - Use `wwwroot/index.html`:**

```html
<!DOCTYPE html>
<html lang="en">
<head>
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <title>Blazor App</title>
    
    <!-- Syncfusion Material3 theme -->
    <link href="_content/Syncfusion.Blazor/themes/material3.css" rel="stylesheet" />
    
    <!-- Material 3 design tokens -->
    <link href="css/material3-tokens.css" rel="stylesheet" />
    
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
    <title>Blazor App</title>
    
    <!-- Syncfusion Material3 theme -->
    <link href="_content/Syncfusion.Blazor/themes/material3.css" rel="stylesheet" />
    
    <!-- Material 3 design tokens -->
    <link href="css/material3-tokens.css" rel="stylesheet" />
    
    <link href="app.css" rel="stylesheet" />
</head>

<body>
    <Routes />
    @* Other app content *@
</body>
```

---

## Material Design 3 Philosophy

**System-First in Blazor:** Material Design 3 has specific rules—follow them for visual cohesion in Razor components.

- **4dp Grid:** All spacing must be multiples of 4dp (4, 8, 12, 16, 24, 32...)
- **Tonal System:** Colors have semantic roles (primary, secondary, tertiary, error)
- **Elevation:** 5 levels (0-5) for depth using tonal surfaces, not shadows
- **Typography:** Fixed roles (Display, Headline, Title, Body, Label) with preset sizes
- **Responsive:** Canonical layouts (Compact < 600dp, Medium 600-840dp, Expanded > 840dp)

**You're committing to:** Material's constraints enable consistency across Blazor projects.

---

## Color System (Tokens)

### Material 3 Semantic Colors

**Primary, Secondary, Tertiary Tones:**
Each has: base color, container, on-color, on-container

```ts
--color-primary: #0891b2           // Brand color
--color-on-primary: #ffffff        // Text on primary
--color-primary-container: #cffafe // Light container
--color-on-primary-container: #001f26 // Dark text on container
```

**Semantic Error Color:**
```ts
--color-error: #ef4444
--color-on-error: #ffffff
--color-error-container: #ffebe9
--color-on-error-container: #410e0b
```

**Surface & On-Surface:**
```ts
--color-surface: #fffbfe           // Background
--color-on-surface: #1c1b1f        // Text on background
--color-surface-container-low: #f7f2f6
--color-surface-container: #f3eef2
--color-surface-container-high: #ede9ed
--color-surface-container-highest: #e8e3e7
```

### Usage in Razor

```razor
<div style="background-color: var(--color-primary); color: var(--color-on-primary);">
  Primary card
</div>

<div style="background-color: var(--color-surface-container); color: var(--color-on-surface);">
  Container surface
</div>
```

---

## Spacing Grid (4dp)

Material uses **4dp as base unit** (not 8pt like Tailwind/Bootstrap).

### Spacing Scale

```ts
--space-xs: 4px    // 1dp (minimal)
--space-sm: 8px    // 2dp (small)
--space-md: 12px   // 3dp (medium)
--space-lg: 16px   // 4dp (standard)
--space-xl: 24px   // 6dp (large)
--space-2xl: 32px  // 8dp (extra large)
--space-3xl: 48px  // 12dp (huge)
```

### Usage

```razor
<div style="padding: var(--space-lg); gap: var(--space-md);">
  Content with Material spacing
</div>
```

**Rule:** Never use arbitrary spacing like `14px` or `18px`. Use only Material tokens.

---

## Typography Roles

### Material Typography Scale

```ts
// Display (Large headlines)
Display Large:   57px / 64px line-height / 700 weight
Display Medium:  45px / 52px line-height / 700 weight
Display Small:   36px / 44px line-height / 400 weight

// Headline (Section titles)
Headline Large:  32px / 40px line-height / 700 weight
Headline Medium: 28px / 36px line-height / 700 weight
Headline Small:  24px / 32px line-height / 700 weight

// Title (Card titles, labels)
Title Large:     22px / 28px line-height / 700 weight
Title Medium:    16px / 24px line-height / 500 weight
Title Small:     14px / 20px line-height / 500 weight

// Body (Main content)
Body Large:      16px / 24px line-height / 400 weight
Body Medium:     14px / 20px line-height / 500 weight
Body Small:      12px / 16px line-height / 500 weight

// Label (Buttons, captions)
Label Large:     14px / 20px line-height / 700 weight
Label Medium:    12px / 16px line-height / 700 weight
Label Small:     11px / 16px line-height / 700 weight
```

### CSS Variables

```css
:root {
  --font-display-large: 57px;
  --font-display-medium: 45px;
  --font-display-small: 36px;
  
  --font-headline-large: 32px;
  --font-headline-medium: 28px;
  --font-headline-small: 24px;
  
  --font-title-large: 22px;
  --font-title-medium: 16px;
  --font-title-small: 14px;
  
  --font-body-large: 16px;
  --font-body-medium: 14px;
  --font-body-small: 12px;
  
  --font-label-large: 14px;
  --font-label-medium: 12px;
  --font-label-small: 11px;
}
```

### Usage

```razor
<h1 style="font-size: var(--font-display-large); font-weight: 700;">
  Page Title
</h1>

<p style="font-size: var(--font-body-large); line-height: 24px;">
  Body text
</p>

<SfButton CssClass="e-material" @onclick="OnButtonClick">
  Button
</SfButton>

@code {
  private async Task OnButtonClick()
  {
    // Handle button click
  }
}
```

---

## Responsive Breakpoints

Material's canonical layouts:

```ts
Compact:   < 600dp    (phones)
Medium:    600-840dp  (tablets)
Expanded:  > 840dp    (desktops)
```

### CSS Media Queries

```css
/* Mobile (Compact) - base */
.layout { width: 100%; padding: var(--space-sm); }

/* Tablet (Medium) - 600px+ */
@media (min-width: 600px) {
  .layout { width: 100%; padding: var(--space-md); }
}

/* Desktop (Expanded) - 840px+ */
@media (min-width: 840px) {
  .layout { width: 100%; max-width: 1200px; margin: 0 auto; padding: var(--space-lg); }
}
```

---

## Elevation & Surfaces

### Material Elevation Levels (0-5)

Use **tonal surfaces** instead of shadows:

```css
/* Elevation 0 - Base */
background: var(--color-surface);
box-shadow: none;

/* Elevation 1 - Slight lift */
background: var(--color-surface-container-low);
box-shadow: var(--elevation-1);

/* Elevation 2 */
background: var(--color-surface-container);
box-shadow: var(--elevation-2);

/* Elevation 3 - Cards, dropdowns */
background: var(--color-surface-container-high);
box-shadow: var(--elevation-3);

/* Elevation 4 - Modals, dialogs */
background: var(--color-surface-container-high);
box-shadow: var(--elevation-4);

/* Elevation 5 - Top-level (tooltips) */
background: var(--color-surface-container-highest);
box-shadow: var(--elevation-5);
```

**Rule:** Elevation = tonal surface + shadow combo. Never shadow alone.

---

## Configuration

All Material Design 3 decisions live in:
- `src/material-tokens.ts` — Token values
- `src/index.css` — CSS custom properties
- `tailwind.config.js` or similar — If extending with utilities

**No per-component configuration.** Material enforces consistency globally.

---

## Stage 5: Code Generation

### Generated Components Use Material Tokens

```razor
@* ✅ CORRECT: Uses Material tokens *@
<div style="background-color: var(--color-primary-container); 
            color: var(--color-on-primary-container); 
            padding: var(--space-lg); 
            border-radius: var(--space-md); 
            box-shadow: var(--elevation-2);">
  Material Card
</div>

<h1 style="font-size: var(--font-headline-large); font-weight: 700;">
  Headline
</h1>

<SfButton CssClass="e-material">
  Syncfusion Material Button
</SfButton>
```

### Syncfusion Blazor Components

Syncfusion Material3 theme automatically styles Blazor components with Material tokens:

```razor
@using Syncfusion.Blazor.Grids

<SfGrid TValue="Employee" DataSource="Employees" AllowPaging="true">
  @* Inherits Material3 styling automatically *@
  <GridColumns>
    <GridColumn Field="Name" Header="Name" Width="150"></GridColumn>
    <GridColumn Field="Department" Header="Department" Width="150"></GridColumn>
  </GridColumns>
</SfGrid>

@code {
  private List<Employee> Employees { get; set; } = new();
  
  protected override async Task OnInitializedAsync()
  {
    Employees = await GetEmployeesAsync();
  }
}
```

---

## Stage 7: Validation Checklist

**Before deploying Stage 5 Razor components, verify:**

### Material Tokens

- ✅ All colors use Material token names (primary, secondary, error, surface, etc.)
- ✅ All spacing from 4dp grid (4px, 8px, 12px, 16px, 24px, 32px, 48px)
- ✅ Typography uses Material roles (Display, Headline, Title, Body, Label)
- ✅ Elevation 0-5 correctly applied (not random shadows)
- ✅ Tonal surfaces used for depth (not shadows alone)
- ✅ No hardcoded colors, spacing, or font sizes

### Responsive & Accessibility

- ✅ Responsive layouts match Material breakpoints (Compact/Medium/Expanded)
- ✅ Dark mode colors invert properly (`prefers-color-scheme: dark`)
- ✅ Text contrast ≥ 4.5:1 (WCAG AA)
- ✅ Touch targets ≥ 44x44px

### Blazor Component Structure

- ✅ `.razor` files use `style` attributes with Material token CSS variables
- ✅ Syncfusion Blazor components have `@using Syncfusion.Blazor.*` directives
- ✅ Component logic in `@code` block
- ✅ `[Parameter]` used for component inputs
- ✅ `EventCallback<T>` used for component outputs
- ✅ All Syncfusion components inherit Material3 theme automatically

### Build & Deployment

- ✅ Syncfusion Material3 theme imported in `wwwroot/index.html`
- ✅ Material tokens CSS file imported in `wwwroot/index.html`
- ✅ Syncfusion services registered in `Program.cs`
- ✅ `dotnet build` succeeds without errors or warnings
- ✅ All `.razor` components render correctly with Material tokens
