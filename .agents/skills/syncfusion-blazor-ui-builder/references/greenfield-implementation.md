# Greenfield Projects: Stage 4 Custom CSS Implementation

**Back to:** [Stage 4: Core Design System Decisions](stage-4-theming-and-design-system.md)

**Framework:** Blazor (No CSS Framework)  
**Approach:** Single CSS file with tokens and semantic classes  
**Implementation Stage:** Stage 4 (Design System) → Stage 5 (Code Generation) → Stage 7 (Validation)

---

## Overview

Greenfield projects build a **custom CSS design system from scratch** in Blazor. This means:
- No framework (no Tailwind, Bootstrap, or Material Design)
- Single CSS file with all styles (`wwwroot/css/styles.css`)
- CSS Variables (tokens) for colors, spacing, typography
- Semantic class names that describe layout elements, not appearance
- Maximum control, maximum responsibility
- Stage 5 generates `.razor` components using your custom semantic classes

**Advantage:** Complete flexibility to design exactly what you need  
**Trade-off:** You own ALL design decisions (spacing, colors, typography, responsive breakpoints)

**Output:** Custom CSS design system documented for Stage 5 Razor component generation.

---

## Table of Contents

1. [CSS Token System](#css-token-system)
2. [Semantic Class Naming](#semantic-class-naming)
3. [File Organization](#file-organization)
4. [Setup & Installation](#setup--installation)
5. [Custom CSS Structure](#custom-css-structure)
6. [Responsive Design Pattern](#responsive-design-pattern)
7. [Stage 5: Code Generation](#stage-5-code-generation)
8. [Stage 7: Validation Checklist](#stage-7-validation-checklist)
9. [Syncfusion Integration](#syncfusion-integration)

---

## CSS Token System

Tokens are the foundation of your design system. Define them as CSS variables in the `:root` selector:

### Color Tokens

```css
:root {
  /* Primary brand color (from Stage 4 decisions) */
  --primary-color: #0891b2;        /* Your chosen brand color */
  --primary-hover: #0d7377;         /* Hover state */
  --primary-light: #06b6d4;         /* Light variant */
  
  /* Semantic colors */
  --success-color: #10b981;
  --warning-color: #f59e0b;
  --error-color: #ef4444;
  --info-color: #3b82f6;
  
  /* Text colors */
  --text-primary: #111827;          /* Main text */
  --text-secondary: #6b7280;        /* Secondary text */
  --text-light: #9ca3af;            /* Light/disabled text */
  
  /* Background colors */
  --bg-primary: #ffffff;            /* Main background */
  --bg-secondary: #f9fafb;          /* Secondary background */
  --bg-tertiary: #f3f4f6;           /* Tertiary background */
  
  /* Borders */
  --border-color: #e5e7eb;
}
```

### Spacing Tokens

```css
:root {
  /* Spacing scale (from Stage 4 decisions) */
  --spacing-xs: 4px;
  --spacing-sm: 8px;
  --spacing-md: 16px;
  --spacing-lg: 24px;
  --spacing-xl: 32px;
  --spacing-2xl: 48px;
  
  /* Layout dimensions */
  --header-height: 70px;
  --sidebar-width: 260px;
  --sidebar-collapsed-width: 60px;
}
```

### Typography Tokens

```css
:root {
  /* Font stack */
  --font-family: 'Inter', -apple-system, BlinkMacSystemFont, 'Segoe UI', sans-serif;
  
  /* Font sizes (from Stage 4 modular scale) */
  --font-size-xs: 12px;
  --font-size-sm: 14px;
  --font-size-base: 16px;
  --font-size-lg: 18px;
  --font-size-xl: 20px;
  --font-size-2xl: 24px;
  --font-size-3xl: 30px;
  
  /* Line heights */
  --line-height-tight: 1.2;
  --line-height-normal: 1.5;
  --line-height-relaxed: 1.6;
}
```

### Shadow & Other Tokens

```css
:root {
  /* Shadows */
  --shadow-sm: 0 1px 2px 0 rgba(0, 0, 0, 0.05);
  --shadow-md: 0 4px 6px -1px rgba(0, 0, 0, 0.1);
  --shadow-lg: 0 10px 15px -3px rgba(0, 0, 0, 0.1);
  
  /* Borders & Radius */
  --border-radius: 8px;
  --border-radius-sm: 4px;
  --border-radius-lg: 12px;
  
  /* Transitions */
  --transition-speed: 250ms;
  --transition-timing: ease-in-out;
}
```

---

## Semantic Class Naming

Classes describe **WHAT they are**, not **HOW they look**. This keeps your design system flexible and maintainable.

### Good Semantic Names

```css
/* ✅ Good: Describes purpose */
.page-header { }          /* Header section of a page */
.kpi-card { }             /* Key Performance Indicator card */
.status-badge { }         /* Status indicator badge */
.sidebar-navigation { }   /* Main navigation sidebar */
.dashboard-header { }     /* Dashboard header */
```

### Bad: Appearance-Based Names

```css
/* ❌ Bad: Tied to appearance */
.blue-box { }             /* What if you need to change color? */
.large-text { }           /* What if text size changes? */
.flex-row { }             /* Utility-like naming (use custom CSS instead) */
.margin-top-16 { }        /* Utility-like naming */
```

### Naming Convention (BEM)

For complex components, use Block__Element--Modifier (BEM):

```css
/* Block: Main component */
.kpi-card { }

/* Element: Sub-part of block */
.kpi-card__header { }
.kpi-card__title { }
.kpi-card__value { }

/* Modifier: Variation */
.kpi-card--active { }
.kpi-card--disabled { }
.status-badge--success { }
.status-badge--error { }
```

---

## File Organization

Structure your single CSS file in logical sections:

```css
/* ============================================
   Design System - Custom CSS
   ============================================ */

/* 1. CSS Variables (Tokens) */
:root {
  /* Colors, spacing, typography, shadows */
}

/* 2. Global Reset & Base Styles */
* { }
body { }
h1, h2, h3 { }

/* 3. Layout Sections */
.dashboard-header { }
.sidebar-navigation { }
.main-content { }

/* 4. Component Classes */
.kpi-card { }
.chart-card { }
.status-badge { }
.button { }

/* 5. Utility Helpers (minimal) */
.mt-lg { margin-top: var(--spacing-lg); }
.text-center { text-align: center; }

/* 6. Responsive Breakpoints */
@media (max-width: 1024px) { }
@media (max-width: 768px) { }
@media (max-width: 480px) { }

/* 7. Accessibility & Preferences */
@media (prefers-reduced-motion: reduce) { }
@media (prefers-color-scheme: dark) { }
```

---

## Setup & Installation

### 1. Create CSS File

Create `wwwroot/css/styles.css`:

```css
/* ============================================
   Your Project Name - Design System
   ============================================ */

:root {
  /* Define all your tokens here */
}

/* Global styles, layout, components */
```

### 2. Import in Blazor App

**For Blazor WebAssembly (WASM) - Use `wwwroot/index.html`:**

```html
<!DOCTYPE html>
<html lang="en">
<head>
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <title>Blazor App</title>
    
    <!-- Syncfusion Blazor theme (optional for Syncfusion components) -->
    <link href="_content/Syncfusion.Blazor.Themes/fluent2.css" rel="stylesheet" />
    
    <!-- Your custom CSS design system -->
    <link href="css/styles.css" rel="stylesheet" />
    
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
    
    <!-- Syncfusion Blazor theme (optional for Syncfusion components) -->
    <link href="_content/Syncfusion.Blazor.Themes/fluent2.css" rel="stylesheet" />
    
    <!-- Your custom CSS design system -->
    <link href="css/styles.css" rel="stylesheet" />
    
    <link href="app.css" rel="stylesheet" />
</head>

<body>
    <Routes />
    @* Other app content *@
</body>
```

### 3. Register Services in Program.cs

```csharp
// Program.cs
var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

// Add Syncfusion Blazor services (if using Syncfusion components)
builder.Services.AddSyncfusionBlazor();

await builder.Build().RunAsync();
```

### 4. Verification Checklist

- ✅ `wwwroot/css/styles.css` created with `:root` tokens
- ✅ CSS file imported in `wwwroot/index.html`
- ✅ Syncfusion services registered in `Program.cs` (if needed)
- ✅ CSS variables available globally in all `.razor` components
- ✅ `dotnet build` succeeds without errors

---

## Custom CSS Structure

### Example: Layout Section

Define semantic classes for major layout regions:

```css
/* Main Container */
.admin-dashboard {
  min-height: 100vh;
  display: flex;
  flex-direction: column;
  background-color: var(--bg-secondary);
}

/* Header Section */
.dashboard-header {
  position: fixed;
  top: 0;
  left: 0;
  right: 0;
  height: var(--header-height);
  background-color: var(--bg-primary);
  border-bottom: 1px solid var(--border-color);
  display: flex;
  align-items: center;
  padding: 0 var(--spacing-lg);
  z-index: 1000;
  box-shadow: var(--shadow-sm);
}

.header-left,
.header-center,
.header-right {
  display: flex;
  align-items: center;
  gap: var(--spacing-md);
}
```

### Example: Component Section

Define semantic classes for reusable components:

```css
/* KPI Card Component */
.kpi-card {
  background-color: var(--bg-primary);
  border: 1px solid var(--border-color);
  border-radius: var(--border-radius);
  padding: var(--spacing-lg);
  display: flex;
  gap: var(--spacing-md);
  transition: all var(--transition-speed) var(--transition-timing);
  box-shadow: var(--shadow-sm);
}

.kpi-card:hover {
  box-shadow: var(--shadow-md);
  transform: translateY(-2px);
}

.kpi-card__icon {
  font-size: 36px;
  min-width: 48px;
  height: 48px;
  display: flex;
  align-items: center;
  justify-content: center;
  background-color: var(--bg-secondary);
  border-radius: var(--border-radius);
}

.kpi-card__title {
  font-size: var(--font-size-sm);
  font-weight: 500;
  color: var(--text-secondary);
  margin-bottom: 4px;
}

.kpi-card__value {
  font-size: var(--font-size-3xl);
  font-weight: 700;
  color: var(--text-primary);
}
```

### Example: Status Badge Component

```css
.status-badge {
  padding: 6px 12px;
  border-radius: 16px;
  font-size: var(--font-size-xs);
  font-weight: 600;
  text-transform: uppercase;
  letter-spacing: 0.5px;
  display: inline-block;
}

.status-badge--success {
  background-color: rgba(16, 185, 129, 0.1);
  color: var(--success-color);
}

.status-badge--error {
  background-color: rgba(239, 68, 68, 0.1);
  color: var(--error-color);
}

.status-badge--warning {
  background-color: rgba(245, 158, 11, 0.1);
  color: var(--warning-color);
}
```

---

## Responsive Design Pattern

### Mobile-First Approach

Define base styles for mobile, then scale up:

```css
/* Mobile (base) */
.main-content {
  margin-left: 0;
  padding: var(--spacing-md);
}

.kpi-grid {
  display: grid;
  grid-template-columns: 1fr;
  gap: var(--spacing-md);
}

/* Tablet (768px+) */
@media (min-width: 768px) {
  .main-content {
    margin-left: var(--sidebar-width);
    padding: var(--spacing-lg);
  }
  
  .kpi-grid {
    grid-template-columns: repeat(2, 1fr);
    gap: var(--spacing-lg);
  }
}

/* Desktop (1024px+) */
@media (min-width: 1024px) {
  .kpi-grid {
    grid-template-columns: repeat(4, 1fr);
  }
}

/* Large Desktop (1440px+) */
@media (min-width: 1440px) {
  .content-wrapper {
    max-width: 1400px;
    margin: 0 auto;
  }
}
```

### Breakpoint Strategy

Use consistent breakpoints based on Stage 4 decisions:

```css
/* Mobile: 320px - 767px (default) */
/* Tablet: 768px - 1023px */
@media (min-width: 768px) { }

/* Desktop: 1024px - 1439px */
@media (min-width: 1024px) { }

/* Large Desktop: 1440px+ */
@media (min-width: 1440px) { }
```

---

## Stage 5: Code Generation

### What Stage 5 Generates

**Output:** Blazor `.razor` components using your custom semantic classes

```razor
@* ✅ Example generated component *@
@page "/dashboard"

<div class="admin-dashboard">
  <header class="dashboard-header">
    <div class="header-left">
      <h1 class="logo">Dashboard</h1>
    </div>
    <div class="header-center">
      <input class="search-input" placeholder="Search..." />
    </div>
    <div class="header-right">
      <button class="notification-btn" @onclick="OnNotificationClick">
        <span class="e-icons e-bell"></span>
      </button>
    </div>
  </header>
  
  <main class="main-content">
    <section class="kpi-section">
      <div class="kpi-grid">
        <div class="kpi-card">
          <div class="kpi-card__icon">📊</div>
          <div class="kpi-card__content">
            <div class="kpi-card__title">Revenue</div>
            <div class="kpi-card__value">$45,231</div>
            <span class="status-badge status-badge--success">+12%</span>
          </div>
        </div>
      </div>
    </section>
  </main>
</div>

@code {
  private async Task OnNotificationClick()
  {
    // Handle notification click
  }
}
```

### Component Structure

- **HTML uses semantic class names** (`.kpi-card`, `.dashboard-header`, etc.)
- **All styling from `wwwroot/css/styles.css`**
- **CSS variables drive design consistency**
- **Responsive design via media queries**
- **No inline styles** (all in CSS file)
- **C# code-behind in `@code` block** for interactivity

---

## Stage 7: Validation Checklist

**Before deploying Stage 5 Razor components, verify:**

### CSS Variables (Tokens)

- ✅ All colors use CSS variables (no hardcoded hex/rgb)
- ✅ All spacing uses CSS variables (no hardcoded px values)
- ✅ All font sizes use CSS variables
- ✅ All shadows use CSS variables
- ✅ All transitions use CSS variables

### Class Naming

- ✅ Classes are semantic (describe purpose, not appearance)
- ✅ No utility-like names (no `.flex-row`, `.mt-16`, `.text-blue`)
- ✅ BEM naming followed for complex components (`.card__header--active`)
- ✅ Consistent naming across all Razor components

### Responsive Design

- ✅ Mobile-first approach (base styles for mobile)
- ✅ Breakpoints at 768px, 1024px, 1440px
- ✅ Layout responsive at all breakpoints
- ✅ Images/media scale appropriately
- ✅ No horizontal scroll on mobile

### Spacing & Layout

- ✅ Margins/padding use spacing variables (`--spacing-md`)
- ✅ Layout sections properly sized (header, sidebar, main-content)
- ✅ Grid/flex gaps use spacing variables
- ✅ No arbitrary spacing values

### Typography

- ✅ Font sizes use typography variables
- ✅ Line heights applied correctly
- ✅ Body text ≥ 16px
- ✅ Headlines have clear hierarchy
- ✅ Font-family defined globally

### Colors & Contrast

- ✅ All colors use CSS variables
- ✅ Text contrast ≥ 4.5:1 (WCAG AA)
- ✅ Focus states clearly visible
- ✅ Status colors accessible (not color-only)

### Accessibility

- ✅ Focus states on interactive elements
- ✅ Button/link touch targets ≥ 44x44px
- ✅ Reduced motion support: `@media (prefers-reduced-motion: reduce)`
- ✅ High contrast mode support: `@media (prefers-contrast: high)`
- ✅ Dark mode support: `@media (prefers-color-scheme: dark)`

### Blazor Component Structure

- ✅ `.razor` files have `.class` attributes (not `className`)
- ✅ Event handlers use `@onclick`, `@onchange` directives
- ✅ Component logic in `@code` block
- ✅ `[Parameter]` used for component inputs
- ✅ `EventCallback<T>` used for component outputs
- ✅ No inline styles (all in `wwwroot/css/styles.css`)

### Performance

- ✅ CSS file size reasonable (< 50KB)
- ✅ No unused CSS classes
- ✅ No console errors or warnings
- ✅ `dotnet build` succeeds without errors
- ✅ No warnings in Razor components

### Dark Mode (If Supported)

- ✅ CSS variables defined for dark mode
- ✅ Text has sufficient contrast in dark mode
- ✅ All components tested in dark mode
- ✅ Dark mode toggle works at runtime via JavaScript interop

---

## Syncfusion Integration (Optional)

### Import Syncfusion Theme

In `wwwroot/index.html`, import Syncfusion Blazor theme CSS BEFORE your custom CSS:

```html
<!-- Syncfusion Blazor theme (optional for Syncfusion components) -->
<link href="_content/Syncfusion.Blazor/themes/bootstrap5.css" rel="stylesheet" />

<!-- Your custom CSS (can override Syncfusion if needed) -->
<link href="css/styles.css" rel="stylesheet" />
```

**Note:** Syncfusion Blazor components will inherit your CSS variables when possible. For complete customization, add CSS variable overrides in your `wwwroot/css/styles.css`.

### Syncfusion Component Example

If using Syncfusion Blazor components alongside your custom CSS:

```razor
@using Syncfusion.Blazor.Grids

<SfGrid TValue="Employee" DataSource="Employees" AllowPaging="true">
  @* Styled by your custom CSS variables and Syncfusion theme *@
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

### Custom CSS for Syncfusion Components

If Syncfusion Blazor components don't inherit your tokens perfectly, add custom CSS in `wwwroot/css/styles.css`:

```css
/* Override Syncfusion component colors */
.e-grid {
  --syncfusion-primary: var(--primary-color);
  --syncfusion-success: var(--success-color);
  --syncfusion-error: var(--error-color);
}

/* Map your tokens to Syncfusion Blazor buttons */
.e-btn.e-primary {
  background-color: var(--primary-color);
}

.e-btn.e-primary:hover {
  background-color: var(--primary-hover);
}
```

### Register Syncfusion Services

In `Program.cs`:

```csharp
builder.Services.AddSyncfusionBlazor();
```

---

## Summary

**Greenfield + Custom CSS in Blazor:**
- ✅ Single `wwwroot/css/styles.css` file
- ✅ CSS Variables (tokens) for all design values
- ✅ Semantic class names describing layout/components
- ✅ Complete design control
- ✅ Responsive with mobile-first approach
- ✅ Accessibility built-in
- ✅ Dark mode support optional but recommended
- ✅ Optional Syncfusion Blazor component integration

**Next Steps:**
- ✅ Complete Stage 4 core decisions (Sections 1-8, 10)
- ✅ Plan your CSS token structure
- ✅ Create `wwwroot/css/styles.css` with semantic classes
- ✅ Register services in `Program.cs`
- ✅ Import CSS in `wwwroot/index.html`
- → Stage 5: Code generation uses your custom classes in `.razor` components
- → Stage 7: Validate against this checklist

---

**Ready for Stage 5 code generation?** Your custom CSS design system is locked. Stage 5 will generate `.razor` components using your semantic class names and CSS variables.
