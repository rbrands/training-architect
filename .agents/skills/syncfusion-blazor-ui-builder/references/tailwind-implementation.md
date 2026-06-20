# Tailwind CSS in Blazor Projects: Stage 4 Framework Implementation

**Back to:** [Stage 4: Core Design System Decisions](stage-4-theming-and-design-system.md)

**Framework:** Tailwind CSS (Utility-First) in Blazor  
**Syncfusion Theme:** Syncfusion Tailwind3 for Blazor  
**Implementation Stage:** Stage 4 (Design System) → Stage 5 (Code Generation) → Stage 7 (Validation)

---

## Overview

Tailwind CSS is a utility-first CSS framework that works excellently with Blazor. This means:
- You compose styles using small, reusable utility classes in `.razor` markup
- You own ALL design decisions (spacing scale, colors, typography)
- Syncfusion Tailwind3 theme integrates seamlessly with Tailwind's visual language
- Stage 5 generates pure Tailwind utility classes directly in Razor components (NOT CSS modules)
- Tailwind configuration applies to entire Blazor project

**Output:** Framework-specific design system decisions documented for Stage 5 code generation.

---

## Table of Contents

1. [Setup & Installation](#setup--installation)
2. [Framework Philosophy](#framework-philosophy)
3. [What You're Committing To](#what-youre-committing-to)
4. [Spacing Scale](#spacing-scale)
5. [Color System](#color-system)
6. [Typography Configuration](#typography-configuration)
7. [Dark Mode Strategy](#dark-mode-strategy)
8. [Responsive Breakpoints](#responsive-breakpoints)
9. [tailwind.config.js Setup](#tailwindconfigjs-setup)
10. [Stage 5: Code Generation](#stage-5-code-generation)
11. [Stage 7: Validation Checklist](#stage-7-validation-checklist)
12. [Syncfusion + Tailwind Integration](#syncfusion--tailwind-integration)

---

## Setup & Installation

### 1. Install Tailwind Dependencies (npm)

If using Blazor with npm-based Tailwind (recommended for most projects):

```bash
npm install tailwindcss @tailwindcss/postcss postcss
```

These packages provide:
- `tailwindcss` — Core Tailwind CSS framework
- `@tailwindcss/postcss` — PostCSS plugin for Tailwind v4
- `postcss` — CSS transformation engine

### 2. Create/Update PostCSS Configuration

Create or update `postcss.config.mjs` at project root:

```mjs
export default {
  plugins: {
    "@tailwindcss/postcss": {},
  }
}
```

This enables Tailwind's unified plugin system (Tailwind v4+).

### 3. Create/Update Tailwind Configuration

Create or update `tailwind.config.js` at project root:

```js
export default {
  content: [
    './Components/**/*.{razor,html}',  // ← Blazor components
    './Pages/**/*.{razor,html}',       // ← Blazor pages
    './Shared/**/*.{razor,html}',      // ← Shared components
    './App.razor',                     // ← App root
  ],
  theme: {
    extend: {
      colors: {
        primary: '#0891b2',        // Your Stage 4 brand color
        'primary-dark': '#0d7377',
        'primary-light': '#06b6d4',
      },
      spacing: {
        xs: '4px',
        sm: '8px',
        md: '16px',
        lg: '24px',
        xl: '32px',
        '2xl': '48px',
      },
    },
  },
  plugins: [],
}
```

**Important:** Update `content` array to include all `.razor` and `.html` file paths. This tells Tailwind which files to scan for class names.

### 4. Create/Update CSS File

Create or update `wwwroot/css/app.css`:

```css
@import "tailwindcss";
```

This single import replaces older `@tailwind` directives and automatically includes:
- `@tailwind base;`
- `@tailwind components;`
- `@tailwind utilities;`

### 5. Import CSS and Syncfusion Theme in index.html

**For Blazor WebAssembly (WASM) - Update `wwwroot/index.html`:**

```html
<!DOCTYPE html>
<html lang="en">
<head>
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <title>My Blazor App</title>
    
    <!-- Syncfusion Tailwind3 theme (unified, includes all component CSS) -->
    <link href="_content/Syncfusion.Blazor.Themes/tailwind3.css" rel="stylesheet" />
    
    <!-- Your Tailwind CSS -->
    <link href="css/app.css" rel="stylesheet" />
</head>
<body>
    <div id="app"></div>
    <script src="_framework/blazor.web.js"></script>
</body>
</html>
```

**For Blazor WebApp (Server or Auto-Render) - Update `Components/App.razor`:**

```razor
@* In Components/App.razor *@

<head>
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <title>My Blazor App</title>
    
    <!-- Syncfusion Tailwind3 theme (unified, includes all component CSS) -->
    <link href="_content/Syncfusion.Blazor.Themes/tailwind3.css" rel="stylesheet" />
    
    <!-- Your Tailwind CSS -->
    <link href="css/app.css" rel="stylesheet" />
</head>

<body>
    <Routes />
    @* Other app content *@
</body>
```

**Important Import Order (Both Project Types):**
1. Syncfusion Tailwind3 theme first (provides base component styles)
2. Your `app.css` with Tailwind utilities second (can override if needed)

This ensures your Tailwind utilities layer on top of Syncfusion's base component styles.

### 6. Register Syncfusion Services in Program.cs

Add Syncfusion Blazor services in `Program.cs`:

```csharp
using Syncfusion.Blazor;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");

// Register Syncfusion Blazor services
builder.Services.AddSyncfusionBlazor();

builder.Services.AddScoped(sp => 
    new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

await builder.Build().RunAsync();
```

### Verification Checklist

- ✅ `tailwindcss`, `@tailwindcss/postcss`, `postcss` installed via npm
- ✅ `postcss.config.mjs` configured with `@tailwindcss/postcss` plugin
- ✅ `tailwind.config.js` configured with `.razor` file paths in `content` array
- ✅ `tailwind.config.js` configured with your colors, spacing, typography
- ✅ `wwwroot/css/app.css` has `@import "tailwindcss";`
- ✅ `wwwroot/index.html` imports Syncfusion Tailwind3 theme link
- ✅ `wwwroot/index.html` imports `css/app.css` link
- ✅ `Program.cs` registers Syncfusion Blazor services with `AddSyncfusionBlazor()`

---

## Framework Philosophy

**Utility-First Principle:** Build layouts and components by composing utility classes directly in your Razor markup.

```razor
@* Tailwind way: Utilities in class attribute *@
<header class="fixed top-0 left-0 right-0 z-1000 flex items-center bg-white border-b border-gray-200 px-6 py-4 shadow-sm">
  <button class="flex items-center justify-center w-10 h-10 rounded-md hover:bg-gray-100 transition-colors">
    <span class="e-icons e-menu"></span>
  </button>
</header>

@* NOT the Tailwind way: Separate CSS classes *@
<header class="header">
  <button class="header-button"></button>
</header>
```

**Why Utility-First?**
- ✅ No naming conflicts (no need to name classes)
- ✅ Smallest CSS bundles (Tailwind purges unused classes automatically)
- ✅ Self-documenting (classes describe what they do)
- ✅ Consistent with Tailwind's design philosophy
- ✅ Perfect alignment with Tailwind3 Syncfusion theme

**📖 For More Information:** [Tailwind CSS: Styling with Utility Classes](https://tailwindcss.com/docs/styling-with-utility-classes)

---

## What You're Committing To

- **Utility-first CSS** — compose styles from utilities in `.razor` markup, not semantic classes
- **You own all design decisions** — spacing, colors, typography are your responsibility
- **Default 8pt spacing scale** — Tailwind uses 4px as the base unit
- **Syncfusion Tailwind3 theme** — Blazor components inherit Tailwind's visual language automatically
- **No CSS modules for styling** — utilities are applied directly to `class` attribute in Razor
- **Dark mode via CSS class** — `dark:` prefix + class toggle on `<html>` element in `App.razor`

---

## Spacing Scale

### Default Scale (8pt Grid)

Tailwind's default spacing uses an 8pt grid (4px as the base unit):

| Tailwind | Pixels | Use Case |
|----------|--------|----------|
| `0` | 0px | Reset/remove spacing |
| `px` | 1px | Borders, dividers |
| `0.5` | 2px | Minimal spacing |
| `1` | 4px | Tight spacing (form fields, buttons) |
| `2` | 8px | Small spacing (gaps between elements) |
| `3` | 12px | Compact spacing |
| `4` | 16px | **Standard spacing** (most common) |
| `6` | 24px | Comfortable spacing |
| `8` | 32px | Large spacing (section padding) |
| `12` | 48px | Extra large spacing |
| `16` | 64px | Page padding, major separations |

### Recommendation

Use Tailwind's default 8pt spacing scale. It's proven, well-documented, and matches component frameworks.

**Only override if:**
- Your design system requires 4pt granularity (rare)
- You have very specific spacing requirements documented in Stage 4 core decisions

### Custom Spacing (If Needed)

If you need additional values, extend in `tailwind.config.js`:

```js
export default {
  theme: {
    extend: {
      spacing: {
        xs: '4px',   // Extra small
        sm: '8px',   // Small
        md: '16px',  // Medium
        lg: '24px',  // Large
        xl: '32px',  // Extra large
        '2xl': '48px', // 2x Large
      },
    },
  },
}
```

---

## Color System

### Semantic Color Palette

Define your semantic colors in `tailwind.config.js`:

```js
export default {
  theme: {
    extend: {
      colors: {
        // Primary brand color
        primary: {
          50: '#f0f9ff',
          100: '#e0f2fe',
          200: '#bae6fd',
          300: '#7dd3fc',
          400: '#38bdf8',
          500: '#0ea5e9',  // Primary base
          600: '#0284c7',
          700: '#0369a1',
          800: '#075985',
          900: '#0c3d66',
        },
        
        // Semantic colors (Syncfusion handles these)
        success: '#10b981',
        warning: '#f59e0b',
        error: '#ef4444',
        info: '#3b82f6',
      },
    },
  },
}
```

### Color Space

Use hex values (Tailwind standard) or convert from OKLCH (recommended from Stage 4):

**Example: Convert OKLCH to Hex**
```
OKLCH(55% 0.12 200) [from Stage 4 decisions]
↓
Hex: #0891b2 (professional teal)
```

### Tinted Neutrals (Optional)

For subtle brand cohesion, add a tint to grays toward your primary hue:

```js
// If primary is teal (#0891b2)
// Tinted grays lean slightly blue
neutral: {
  50: '#f8fafc',   // Tinted white
  100: '#f1f5f9',  // Very light gray
  200: '#e2e8f0',  // Light gray (tinted)
  300: '#cbd5e1',
  400: '#94a3b8',
  500: '#64748b',
  600: '#475569',
  700: '#334155',
  800: '#1e293b',
  900: '#0f172a',  // Tinted black
}
```

---

## Typography Configuration

### Modular Scale

Define your typography scale in `tailwind.config.js` using a consistent ratio (1.25 recommended):

```js
export default {
  theme: {
    fontSize: {
      // Base modular scale (1.25 ratio)
      xs: ['12px', { lineHeight: '1.5' }],     // 12px
      sm: ['14px', { lineHeight: '1.5' }],     // 14px (12 × 1.17)
      base: ['16px', { lineHeight: '1.6' }],   // 16px (14 × 1.14) — BODY TEXT
      lg: ['18px', { lineHeight: '1.6' }],     // 18px
      xl: ['20px', { lineHeight: '1.5' }],     // 20px (16 × 1.25)
      '2xl': ['24px', { lineHeight: '1.4' }],  // 24px (20 × 1.2)
      '3xl': ['30px', { lineHeight: '1.3' }],  // 30px (24 × 1.25)
      '4xl': ['36px', { lineHeight: '1.2' }],  // 36px (30 × 1.2)
      '5xl': ['48px', { lineHeight: '1.1' }],  // 48px (36 × 1.33)
    },
  },
}
```

### Minimum Text Size

**Never use font-size smaller than 16px for body text.** Smaller fonts strain eyes and fail accessibility on mobile.

### Line Height Rules

- **Body text:** `1.5` to `1.6` (readable, spacious)
- **Light text on dark:** Add `0.1` extra (e.g., `1.7`) because light text reads heavier
- **Headlines:** `1.1` to `1.3` (compact, strong)
- **Labels/captions:** `1.4` to `1.5` (slightly spacious for clarity)

---

## Dark Mode Strategy

### Enable Dark Mode (Class Strategy)

In `tailwind.config.js`:

```js
export default {
  darkMode: 'class', // Use class selector (not prefers-color-scheme)
  // ... rest of config
}
```

### Dark Mode Utilities

Use `dark:` prefix to define dark mode styles in Razor:

```razor
@* Light mode default → Dark mode with dark: prefix *@
<div class="bg-white dark:bg-gray-900 text-gray-900 dark:text-gray-100">
  <h1 class="text-lg dark:text-lg font-bold dark:font-semibold">Title</h1>
  <p class="text-gray-600 dark:text-gray-300">Body text</p>
</div>
```

### Runtime Theme Toggle

Toggle dark mode by adding/removing the `dark` class on `<html>` element:

```razor
@* ThemeToggle.razor *@
@inject IJSRuntime JS

<button @onclick="ToggleDarkMode" class="p-2 rounded-md hover:bg-gray-100 dark:hover:bg-gray-800">
  <span class="e-icons @(isDark ? "e-sun" : "e-moon")"></span>
</button>

@code {
  private bool isDark = false;
  
  private async Task ToggleDarkMode()
  {
    isDark = !isDark;
    if (isDark)
      await JS.InvokeVoidAsync("eval", "document.documentElement.classList.add('dark')");
    else
      await JS.InvokeVoidAsync("eval", "document.documentElement.classList.remove('dark')");
  }
}
```

Or use an interop helper in `wwwroot/js/theme.js`:

```javascript
export function toggleDarkMode(isDark) {
  if (isDark) {
    document.documentElement.classList.add('dark');
  } else {
    document.documentElement.classList.remove('dark');
  }
}
```

Then call from Blazor:

```csharp
await JS.InvokeVoidAsync("toggleDarkMode", isDark);
```

### Dark Mode Colors

Add dark-mode-specific grays:

```js
export default {
  theme: {
    extend: {
      colors: {
        'dark-surface': '#1f2937',  // Dark mode background
        'dark-border': '#374151',   // Dark mode borders
      },
    },
  },
}
```

---

## Responsive Breakpoints

### Default Breakpoints

Tailwind uses these responsive prefixes (mobile-first):

| Prefix | Media Query | Use Case |
|--------|-------------|----------|
| *(none)* | Base styles | Mobile (default) |
| `sm:` | `min-width: 640px` | Small tablets |
| `md:` | `min-width: 768px` | Tablets |
| `lg:` | `min-width: 1024px` | Desktops |
| `xl:` | `min-width: 1280px` | Large desktops |
| `2xl:` | `min-width: 1536px` | Extra large displays |

### Usage Example

```tsx
// Mobile-first: start with mobile layout
// Scale up with breakpoints
<div className="w-full md:w-1/2 lg:w-1/3 p-4 md:p-6 lg:p-8">
  <GridComponent dataSource={data} />
</div>
```

### Custom Breakpoints (If Needed)

Override breakpoints in `tailwind.config.js`:

```js
export default {
  theme: {
    screens: {
      'sm': '640px',
      'md': '768px',
      'lg': '1024px',
      'xl': '1280px',
      '2xl': '1536px',
      // Add custom breakpoints if needed
      'tablet': '768px',
      'desktop': '1024px',
    },
  },
}
```

---

## tailwind.config.js Setup

Complete configuration example for Stage 4 decisions:

```js
export default {
  content: [
    './index.html',
    './src/**/*.{js,ts,jsx,tsx}',
  ],
  
  darkMode: 'class',
  
  theme: {
    extend: {
      colors: {
        // Primary brand color (your Stage 4 choice)
        primary: {
          50: '#f0f9ff',
          100: '#e0f2fe',
          200: '#bae6fd',
          300: '#7dd3fc',
          400: '#38bdf8',
          500: '#0ea5e9',  // Base
          600: '#0284c7',
          700: '#0369a1',
          800: '#075985',
          900: '#0c3d66',
        },
        
        // Semantic colors (Syncfusion theme applies these)
        success: '#10b981',
        warning: '#f59e0b',
        error: '#ef4444',
        info: '#3b82f6',
        
        // Dark mode surfaces
        'dark-surface': '#1f2937',
        'dark-border': '#374151',
      },
      
      spacing: {
        xs: '4px',
        sm: '8px',
        md: '16px',
        lg: '24px',
        xl: '32px',
        '2xl': '48px',
      },
      
      fontSize: {
        xs: ['12px', { lineHeight: '1.5' }],
        sm: ['14px', { lineHeight: '1.5' }],
        base: ['16px', { lineHeight: '1.6' }],
        lg: ['18px', { lineHeight: '1.6' }],
        xl: ['20px', { lineHeight: '1.5' }],
        '2xl': ['24px', { lineHeight: '1.4' }],
        '3xl': ['30px', { lineHeight: '1.3' }],
        '4xl': ['36px', { lineHeight: '1.2' }],
        '5xl': ['48px', { lineHeight: '1.1' }],
      },
    },
  },
  
  plugins: [],
}
```

---

## Stage 5: Code Generation

### What Stage 5 Generates for Tailwind Projects

**Output:** Blazor `.razor` components using pure Tailwind utility classes

```razor
@* ✅ CORRECT: Pure Tailwind utilities in .razor markup *@
@using Syncfusion.Blazor.Grids

<header class="fixed top-0 left-0 right-0 z-1000 flex items-center bg-white dark:bg-gray-900 border-b border-gray-200 dark:border-gray-700 px-6 py-4 shadow-sm">
  <div class="flex items-center gap-3">
    <button class="flex items-center justify-center w-10 h-10 bg-transparent hover:bg-gray-100 dark:hover:bg-gray-800 rounded-md transition-colors duration-200" @onclick="ToggleMenu">
      <span class="e-icons e-menu"></span>
    </button>
    <h1 class="text-xl font-semibold text-gray-900 dark:text-white">Dashboard</h1>
  </div>
</header>

@* Responsive example with Syncfusion Grid *@
<section class="w-full md:w-1/2 lg:w-1/3 p-4 md:p-6 lg:p-8">
  <SfGrid TValue="GridModel" DataSource="Data">
    <GridColumns>
      <GridColumn Field="Name" Type="ColumnType.String"></GridColumn>
      <GridColumn Field="Email" Type="ColumnType.String"></GridColumn>
    </GridColumns>
  </SfGrid>
</section>

@code {
  private List<GridModel> Data { get; set; }
  
  private void ToggleMenu()
  {
    // Handle menu toggle
  }
  
  protected override void OnInitialized()
  {
    // Initialize data
    Data = new List<GridModel>();
  }
  
  public class GridModel
  {
    public string Name { get; set; }
    public string Email { get; set; }
  }
}
```

### Component Structure

- **No CSS modules** — all styling via `class` attribute in Razor
- **Responsive prefixes** — `sm:`, `md:`, `lg:`, `xl:` for breakpoints
- **Dark mode prefixes** — `dark:` for dark mode variants
- **Syncfusion components** — inherit Tailwind theming automatically (via Syncfusion Tailwind3 theme)
- **Transitions** — `transition-all duration-300` for state changes
- **Component parameters** — use `@using` directives and C# properties

### Syncfusion Integration

Syncfusion Tailwind3 theme automatically styles Blazor components to match your `tailwind.config.js`:

```razor
@* DataGrid.razor *@
@using Syncfusion.Blazor.Grids

<SfGrid TValue="Employee" DataSource="Employees" AllowPaging="true" AllowSorting="true">
  <GridColumns>
    <GridColumn Field="EmployeeId" HeaderText="ID"></GridColumn>
    <GridColumn Field="FirstName" HeaderText="First Name"></GridColumn>
    <GridColumn Field="Title" HeaderText="Title"></GridColumn>
  </GridColumns>
</SfGrid>

@* Inherits Tailwind colors, spacing, typography from Syncfusion Tailwind3 theme *@

@code {
  private List<Employee> Employees { get; set; }
  
  protected override void OnInitialized()
  {
    Employees = GetEmployees();
  }
}
```

---

## Stage 7: Validation Checklist

**Before deploying Stage 5 code, verify the following:**

### Tailwind Syntax

- ✅ All class names are valid Tailwind utilities (no typos like `pd-4` instead of `p-4`)
- ✅ No spaces in class names (use hyphens: `bg-gray-100` not `bg gray 100`)
- ✅ Responsive prefixes use correct format: `md:`, not `@md` or `(md)`
- ✅ Dark mode pairs use `dark:` prefix: `dark:bg-gray-900`
- ✅ Razor `class` attribute syntax correct (no `className` like React)

### Color Usage

- ✅ Colors match `tailwind.config.js` extended palette
- ✅ Primary color used consistently (not multiple brand colors)
- ✅ Semantic colors (success, warning, error, info) applied correctly
- ✅ Dark mode has sufficient contrast (test with tools like WebAIM)
- ✅ Syncfusion component colors inherited correctly from theme

### Spacing & Layout

- ✅ Spacing values from configured scale: `xs`, `sm`, `md`, `lg`, `xl`, `2xl`
- ✅ No arbitrary spacing values like `p-7` or `gap-13`
- ✅ Responsive layout uses mobile-first approach (base → `md:` → `lg:`)
- ✅ Blazor component parameters configured correctly

### Typography

- ✅ Font sizes from configured scale (xs through 5xl)
- ✅ Body text is 16px minimum (base or larger)
- ✅ Line heights appropriate: `1.5` for body, `1.2-1.3` for headlines
- ✅ Headline hierarchy is clear (visual contrast between sizes)

### Dark Mode (Blazor)

- ✅ Dark mode class toggle works on `<html>` element via JS interop
- ✅ All foreground colors have `dark:` pairs
- ✅ Dark mode text has sufficient contrast (4.5:1 minimum)
- ✅ Test: Toggle dark mode, verify readability and color accuracy
- ✅ ThemeToggle component or similar works correctly

### Syncfusion Integration (Blazor)

- ✅ Syncfusion Tailwind3 theme CSS imported in `wwwroot/index.html`
- ✅ Syncfusion Blazor services registered in `Program.cs` with `AddSyncfusionBlazor()`
- ✅ Syncfusion components inherit Tailwind colors (no custom CSS needed)
- ✅ Component spacing aligns with Tailwind spacing scale
- ✅ No CSS variable usage in component styling (pure utilities only)
- ✅ `@using Syncfusion.Blazor.*` directives present in components

### Accessibility

- ✅ Interactive elements have focus states: `focus:ring-2 focus:ring-primary`
- ✅ Buttons have touch targets ≥ 44x44px (via padding or explicit size)
- ✅ Color contrast meets WCAG AA (4.5:1 minimum)
- ✅ Reduced motion supported: animations disabled if `prefers-reduced-motion`
- ✅ ARIA labels on form inputs and complex components
- ✅ Keyboard navigation works (Tab, Enter, Escape)

### Build & Performance

- ✅ Tailwind CSS is purged (unused classes removed in production)
- ✅ CSS bundle size is reasonable (~20-40KB for typical Blazor app)
- ✅ No console errors or warnings
- ✅ C# compilation succeeds without errors
- ✅ Blazor component rendering is performant (no unnecessary re-renders)

### Browser Testing

- ✅ Layout responsive at breakpoints (320px, 768px, 1024px, 1280px)
- ✅ Dark mode works in Chrome, Firefox, Safari, Edge
- ✅ No layout shifts or jank during transitions
- ✅ Images and media scale appropriately
- ✅ Blazor event handlers (@onclick, @onchange) work correctly
- ✅ Data binding updates UI correctly

### Blazor-Specific Checks

- ✅ Components have proper `@page` directives if routable
- ✅ `@using` directives present for Syncfusion components
- ✅ Component parameters use `[Parameter]` attribute
- ✅ Lifecycle methods (OnInitializedAsync) properly implemented
- ✅ Event callbacks use `EventCallback<T>` pattern if needed
- ✅ Two-way binding (@bind-*) used where appropriate

---

## Syncfusion + Tailwind Integration (Blazor)

### Theme Import (Index.html Entry Point)

Use the unified Syncfusion Tailwind3 theme import in `wwwroot/index.html`:

```html
<!DOCTYPE html>
<html lang="en">
<head>
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <title>My Blazor App</title>
    
    <!-- Unified Syncfusion Tailwind3 theme (all component CSS included) -->
    <link href="_content/Syncfusion.Blazor.Themes/tailwind3.css" rel="stylesheet" />
    
    <!-- Your Tailwind CSS -->
    <link href="css/app.css" rel="stylesheet" />
</head>
<body>
    <div id="app"></div>
    <script src="_framework/blazor.web.js"></script>
</body>
</html>
```

**Benefits of unified theme import:**
- ✅ Single link contains all Syncfusion Blazor component styles
- ✅ No need for individual component CSS imports
- ✅ Smaller CSS bundle (optimized loading)
- ✅ Consistent theming across all Syncfusion components
- ✅ Works with Blazor's component system automatically

### Dark Mode in Syncfusion (Blazor)

Syncfusion Tailwind3 theme respects Tailwind's dark mode automatically:

```razor
@* Dark mode works when e-dark class is on <html> element *@
@* In App.razor or parent component *@

<div class="@(isDarkMode ? "dark" : "")">
  <SfGrid TValue="Employee" DataSource="Employees">
    @* Uses dark theme colors automatically *@
  </SfGrid>
</div>
```

Toggling dark mode class on `<html>`:

```csharp
// In Blazor component
private async Task ToggleDarkMode(bool isDark)
{
  if (isDark)
    await JS.InvokeVoidAsync("eval", "document.documentElement.classList.add('dark')");
  else
    await JS.InvokeVoidAsync("eval", "document.documentElement.classList.remove('dark')");
}
```

📖 **For detailed Syncfusion theming:** See [Syncfusion Blazor Theming Resources](syncfusion-themes.md)

---

## Summary

**Tailwind CSS + Syncfusion Tailwind3 Theme in Blazor:**
- ✅ Utility-first approach with pure CSS classes in `.razor` markup
- ✅ Complete design system flexibility
- ✅ Smallest CSS bundles (purged automatically)
- ✅ Perfect alignment with Tailwind's philosophy
- ✅ Seamless Syncfusion Blazor component integration
- ✅ Works perfectly with Blazor's component system and data binding

**Implementation Checklist:**
- ✅ Tailwind CSS installed via npm (`tailwindcss`, `@tailwindcss/postcss`, `postcss`)
- ✅ `postcss.config.mjs` configured
- ✅ `tailwind.config.js` configured with `.razor` file paths
- ✅ `wwwroot/css/app.css` has Tailwind imports
- ✅ `wwwroot/index.html` imports Syncfusion Tailwind3 theme and app.css
- ✅ `Program.cs` registers Syncfusion Blazor services
- ✅ Design tokens (colors, spacing, typography) configured in `tailwind.config.js`

**Next Steps:**
- ✅ Complete Stage 4 core decisions (Sections 1-8, 10)
- → Stage 5: Code generation uses `tailwind.config.js` to generate `.razor` components
- → Stage 7: Validate against Blazor + Tailwind checklist above

---

**Ready for Stage 5 code generation?** The design system is locked. Stage 5 will generate Blazor components using pure Tailwind utility classes that respect all Stage 4 decisions.
