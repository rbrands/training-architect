# Syncfusion Blazor Theming Resources

**⚠️ MANDATORY:** After making your framework choice in Stage 4, you MUST consult **Skill: syncfusion-blazor-themes** for detailed implementation guidance before proceeding to Stage 5 code generation:

## Quick Reference: Syncfusion Blazor Themes

| Framework | Theme | CSS Import Location | Example | Default |
|-----------|-------|---------------------|---------|---------|
| **Fluent**/**Modern** | **Fluent 2 Light** | **WASM:** `wwwroot/index.html`<br/>**WebApp:** `Components/App.razor` | `<link href="_content/Syncfusion.Blazor.Themes/fluent2.css" rel="stylesheet" />` | ✅ **DEFAULT** |
| **Tailwind**/**Greenfield** | Tailwind3 | **WASM:** `wwwroot/index.html`<br/>**WebApp:** `Components/App.razor` | `<link href="_content/Syncfusion.Blazor.Themes/tailwind3.css" rel="stylesheet" />` | Alternative |
| **Bootstrap** | Bootstrap5 | **WASM:** `wwwroot/index.html`<br/>**WebApp:** `Components/App.razor` | `<link href="_content/Syncfusion.Blazor.Themes/bootstrap5.css" rel="stylesheet" />` | Alternative |
| **Material** | Material3 | **WASM:** `wwwroot/index.html`<br/>**WebApp:** `Components/App.razor` | `<link href="_content/Syncfusion.Blazor.Themes/material3.css" rel="stylesheet" />` | Alternative |
| **Fluent (Dark)** | Fluent 2 Dark | **WASM:** `wwwroot/index.html`<br/>**WebApp:** `Components/App.razor` | `<link href="_content/Syncfusion.Blazor.Themes/fluent2-dark.css" rel="stylesheet" />` | Alternative |

### Theme Import Location by Project Type:

**🔧 Blazor WebAssembly (WASM):**
- Import theme CSS in `wwwroot/index.html` `<head>` section
- Example: `<link href="_content/Syncfusion.Blazor.Themes/tailwind3.css" rel="stylesheet" />`
- Reason: `index.html` is the app entry point; theme must load before Blazor app initializes

**🔧 Blazor WebApp (Server or Auto-Render):**
- Import theme CSS in `Components/App.razor` `<head>` section
- Example: Place link tag in the root component before any child components render
- Reason: `App.razor` is the root component that renders the app shell; themes applied at component level for consistency

**⚠️ Critical:** Ensure theme CSS is imported in `<head>` section BEFORE any components render to avoid flash of unstyled content (FOUC).

**NuGet Installation:**
All themes are available in `Syncfusion.Blazor.Themes` NuGet package.

## By Use Case:

Read the **Skill: syncfusion-blazor-themes** for below features

### Applying a Theme
- Refer to **Built-in Themes** for Blazor component CSS import in `wwwroot/index.html`
- Check NuGet package version matching requirements (all Syncfusion.Blazor.* packages should match)
- Review per-component CSS imports if needed for optimization

### Implementing Dark Mode - IF USER ASK/WANT
- Refer to **Dark Mode Implementation** for:
  - Global dark mode with `e-dark-mode` class on root element (in `App.razor`)
  - Per-component dark mode customization in Blazor components
  - Runtime theme switching patterns using `CssClass` parameter in Syncfusion components

### Customizing Colors & Tokens
- Refer to **CSS Variables Customization** for:
  - Theme-specific CSS variable structures (Material 3, Fluent 2, Bootstrap 5.3, Tailwind 3.4)
  - RGB vs hex value formats for CSS custom properties
  - Runtime color modifications via `CssClass` or inline styles in `.razor.css` files

### Using Icons
- Refer to **Icon Library** for:
  - Using Syncfusion icons in Blazor components with `<span class="e-icons e-icon-name"></span>`
  - Sizing modes (small, medium, large) via CSS classes
  - Icon customization and styling in Blazor component CSS

### Advanced Theming
- Refer to **Advanced Features** for:
  - Touch mode / size modes in Syncfusion Blazor components
  - Font customization across Syncfusion components via CSS variables
