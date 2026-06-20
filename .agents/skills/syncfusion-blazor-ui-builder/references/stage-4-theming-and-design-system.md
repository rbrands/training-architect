# Stage 4: Theming & Design System Selection

**🎨 DEFAULT THEME: Fluent 2 Light**

Unless explicitly overridden by the user, all components will use **Fluent 2 Light** theme, which provides:
- Modern Microsoft design language
- WCAG 2.1 AA accessibility compliance by default
- Excellent contrast and readability
- Professional appearance for enterprise applications

**Purpose:** Understand design system trade-offs and lock theming decisions before code generation.

## Overview

This stage is about **decision-making clarity**, not code generation. You'll:

- **Confirm your CSS framework choice** (from Stage 2) and understand its design philosophy
- **Select a Syncfusion theme** that aligns with your framework, not against it
- **Define color architecture** based on principles (perceptual uniformity, contrast, brand cohesion)
- **Establish spacing and typography scales** that respect readability and hierarchy
- **Plan responsive strategy** that matches your framework's conventions
- **Document token structure** so Stage 5 can generate consistent code
- **Install all Syncfusion Blazor packages** in a single command to maintain version consistency
- **Configure Syncfusion services** in `Program.cs` for proper initialization

**Key Insight:** Your framework choice IS a design system choice. Tailwind, Bootstrap, and Material Design have fundamentally different philosophies about spacing, color, and component structure. Stage 4 ensures your Syncfusion theme and custom CSS align with your framework's philosophy, not against it.

This file provides theming guidance for Syncfusion Blazor components. For framework-specific theming implementations in your Blazor project, also refer to:
- **[greenfield-implementation.md](greenfield-implementation.md)** — Custom CSS design system with tokens and semantic classes for Blazor
- **[tailwind-implementation.md](tailwind-implementation.md)** — Tailwind CSS utility-first approach with Syncfusion Tailwind3 theme in Blazor
- **[material-implementation.md](material-implementation.md)** — Material Design 3 system-first approach with Syncfusion Material3 theme in Blazor
- **[bootstrap-implementation.md](bootstrap-implementation.md)** — Bootstrap 5 component-first approach with Syncfusion Bootstrap5 theme in Blazor

**Output:** Design system decisions documented and ready for implementation in Stage 5 (.razor component generation).

---

## Table of Contents

1. [CSS Framework Philosophy](#1-css-framework-philosophy)
2. [Syncfusion Theme Alignment](#2-syncfusion-theme-alignment)
3. [Color System Architecture](#3-color-system-architecture)
4. [Spacing & Typography Systems](#4-spacing--typography-systems)
5. [Responsive Strategy](#5-responsive-strategy)
6. [Motion & Accessibility Standards](#6-motion--accessibility-standards)
7. [Design System Token Architecture](#7-design-system-token-architecture)
8. [Syncfusion Component Integration](#8-syncfusion-component-integration)
   - [8.1 Theme Registration](#81-theme-registration)
   - [8.2 Script Registration](#82-script-registration)
   - [8.3 Custom CSS Coordination](#83-custom-css-coordination)
9. [Load Your Framework Reference (MANDATORY)](#9-load-your-framework-reference-mandatory)
10. [Stage 4 Decision Checklist](#10-stage-4-decision-checklist)
11. [What Stage 5 Does With These Decisions](#what-stage-5-does-with-these-decisions)

---

## 1. CSS Framework Philosophy

**Input:** CSS framework detected in Stage 2

**Decision Point:** Your framework choice defines everything downstream. Understand what you're committing to:

### Understanding Your Framework

**Tailwind CSS (Utility-First)**
- Philosophy: Build with small, reusable utility classes
- Design implication: You own the scale decisions (spacing, colors, typography)
- Syncfusion Blazor pairing: Use Syncfusion Tailwind3 theme (Blazor components styled to match Tailwind's visual language)
- Trade-off: More flexible, but requires discipline to maintain consistency
- **Blazor setup:** Import Syncfusion Tailwind3 CSS in `wwwroot/index.html` and configure `tailwind.config.js` in project root

**Bootstrap (Utility-First with Component Foundation)**
- Philosophy: Semantic color names and 12-column grid; compose with utilities
- Design implication: Bootstrap's 8pt spacing scale, semantic colors (primary, secondary, success, danger, warning, info)
- Syncfusion Blazor pairing: Use Syncfusion Bootstrap5 theme (Blazor components match Bootstrap's styling)
- Trade-off: Opinionated defaults for speed, but less distinctive designs
- **Blazor setup:** Import Syncfusion Bootstrap5 CSS in `wwwroot/index.html`, Bootstrap 5 CSS, and optionally configure SCSS variables
- **See:** [bootstrap-implementation.md](bootstrap-implementation.md)

**Material Design (Utility-First with System Rules)**
- Philosophy: Comprehensive design system with specific rules (4dp grid, elevation system, semantic tokens)
- Design implication: Follow Material's spacing, typography, elevation, and color token rules
- Syncfusion Blazor pairing: Use Syncfusion Material3 theme (Blazor components follow Material Design 3 specs)
- Trade-off: Highly opinionated, but visually cohesive if you commit fully
- **Blazor setup:** Import Syncfusion Material3 CSS in `wwwroot/index.html` and define Material tokens in CSS custom properties
- **See:** [material-implementation.md](material-implementation.md)

**Greenfield (No Framework)**
- Philosophy: You design the system from scratch
- Design implication: You must make ALL decisions (spacing, color, typography)
- Syncfusion Blazor pairing: Recommend Syncfusion Tailwind3 theme as visual baseline for Blazor components
- Trade-off: Maximum flexibility, maximum responsibility
- **Blazor setup:** Import Syncfusion Tailwind3 CSS, define custom CSS variables in `wwwroot/css/`

### The Non-Obvious Truth

**You cannot mix frameworks.** Don't use Bootstrap utility classes with Tailwind's spacing scale, or Material tokens with Bootstrap's component defaults. Each framework makes specific assumptions about spacing ratios, color derivation, and component structure. Mixing them creates visual conflicts and maintainability nightmares.

### Decision: Confirm Your Framework

Review Stage 2's detection:
- Is the detected framework correct?
- Does it match your project's intent?
- Are you committed to that framework's philosophy?

If Stage 2 detected wrong, or you want to override: **Document why.** Design decisions need reasoning.

**→ MANDATORY:** After confirming, you MUST proceed to **Section 9** to load your framework-specific implementation reference. Do not skip this step.

**Output:** CSS framework philosophy understood and confirmed

---

## 2. Syncfusion Theme Alignment

**Core Principle:** Syncfusion components must coordinate with your CSS framework, not fight it.

### Why Theme Matching Matters

Syncfusion provides pre-built themes to match popular CSS frameworks. When your Syncfusion theme aligns with your CSS framework, component styling integrates seamlessly. When they don't align, you'll spend Stage 5+ fighting style conflicts and writing excessive custom CSS.

**Example of alignment (Blazor):**
- Tailwind Blazor project + Syncfusion Tailwind3 theme = SfGrid uses Tailwind's color scale, spacing logic, responsive conventions
- Bootstrap Blazor project + Syncfusion Bootstrap5 theme = SfGrid inherits Bootstrap's component styling patterns
- Material Blazor project + Syncfusion Material3 theme = SfGrid follows Material Design elevation, spacing rules, typography system

**Example of misalignment (DON'T DO THIS):**
- Tailwind Blazor project + Syncfusion Bootstrap5 theme = SfGrid looks like Bootstrap; your Tailwind components look like Tailwind; visual incoherence
- Material Blazor project + Syncfusion Bootstrap5 theme = Material components in Syncfusion, Bootstrap styling everywhere else; confusion

### Theme Selection Decision Tree

**If you detected Tailwind in Stage 2:**
→ Use **Syncfusion Tailwind3** theme for Blazor components
- Why: Syncfusion components styled to Tailwind's visual language (clean, utility-based, minimal ornamentation)
- You'll extend with Tailwind utilities in your `.razor` components, not custom CSS

**If you detected Bootstrap in Stage 2:**
→ Use **Syncfusion Bootstrap5** theme for Blazor components
- Why: Syncfusion components styled as Bootstrap components (component-first, opinionated defaults)
- You'll extend with Bootstrap utilities or component classes in your `.razor` components

**If you detected Material Design in Stage 2:**
→ Use **Syncfusion Material3** theme for Blazor components
- Why: Syncfusion components follow Material Design System (elevation, spacing scale, semantic color system)
- You'll extend with Material tokens and Material-aligned CSS in your Blazor stylesheets

**If Stage 2 detected no framework (greenfield Blazor project):**
→ Default to **Syncfusion Tailwind3** theme for Blazor components
- Why: Tailwind's utility-first philosophy is most flexible for new projects
- You're responsible for all design decisions; Tailwind3 gives you structure without being opinionated

### The Hard Rule

**Never pair a Syncfusion theme with a mismatched CSS framework.** If you do:
- Stage 5 code generation will be inconsistent
- Custom CSS will grow chaotic as you patch conflicts
- Your design system won't scale

If your project uses a framework Stage 2 didn't detect, document it now and choose the matching theme.

**→ REQUIRED:** Your theme choice is locked. This determines your **Section 9 framework reference file**.

**Output:** Syncfusion theme aligned with CSS framework

---

## 3. Color System Architecture

### 3.1 Color Space Decision

**Use OKLCH instead.** It's perceptually uniform: equal steps in lightness *look* equal regardless of hue. This is critical for accessible contrast and visual hierarchy.

**Compatibility Note:** OKLCH works in all modern browsers. For older browser support, generate fallback hex values, but design in OKLCH.

### 3.2 Brand Color & Semantic Palette

**The Decision Point:** Your color palette must have clear roles.

Define:
1. **Primary color** (brand, CTAs, key actions)
2. **Semantic colors** (success, warning, error, info)
3. **Neutral scale** (text, backgrounds, borders)
4. **Surface colors** (cards, modals, containers) — optional if neutrals are sufficient

**Framework-Specific Consideration (Blazor):**

- **Tailwind Blazor projects:** You'll configure colors in `tailwind.config.js`. Tailwind's default scale includes semantic naming (primary, success, etc.). Follow that pattern in your `wwwroot/css/` stylesheets.
- **Bootstrap Blazor projects:** Bootstrap enforces semantic colors (primary, success, warning, danger). Respect Bootstrap's naming convention in your component CSS to avoid confusion.
- **Material Blazor projects:** Material Design has specific color token naming (primary, secondary, tertiary, error) with fixed roles. Don't deviate in your Blazor component styling.

**The Anti-Pattern:** Creating a palette that looks good in isolation but doesn't respect framework conventions. Your custom primary color might be beautiful, but if it breaks your framework's semantic system, it creates inconsistency.

### 3.3 Tinted Neutrals for Cohesion

**Why tinted neutrals matter:**

Pure gray (zero chroma) feels lifeless next to a brand color. Adding a *tiny* chroma value (0.005-0.015) tinted toward your brand hue creates subconscious visual cohesion without reading as "tinted."

**Example:**
- Brand color: oklch(55% 0.12 200) [teal]
- Pure gray: oklch(50% 0 0) [lifeless against teal]
- Tinted gray: oklch(50% 0.01 200) [cohesive, still looks gray]

**But avoid the reflex:** Don't always tint toward warm orange or cool blue. That's the laziest default. Tint toward *this specific project's* brand hue.

### 3.4 Dark Mode: Structural, Not Inverted

**The Misconception:** Dark mode is just light mode inverted.

**The Reality:** Dark mode requires different design thinking:
- Light mode uses shadow for depth
- Dark mode uses lighter surfaces for depth (no shadows)
- Light mode uses vibrant accents
- Dark mode desaturates accents slightly (vibrant colors feel aggressive on dark)
- Light mode: text is dark on light
- Dark mode: text is light on dark, usually needs slightly reduced font weight

**Framework Consideration (Blazor):**
- Tailwind Blazor users: Define dark mode in `tailwind.config.js` with strategy (class or prefers-color-scheme)
- Bootstrap Blazor users: Bootstrap has dark mode CSS variables; use them in `wwwroot/css/`
- Material Blazor users: Material 3 specifies dark mode behavior; follow Material's rules in your stylesheets

**Decision Point:** Are you supporting dark mode?
- If **no**: You're done here
- If **yes**: Plan it now. It's not an afterthought, it's a design system choice

**📖 For Syncfusion Dark Mode:**
Refer to **Skill: syncfusion-blazor-themes** → **Dark Mode Implementation** for:
- Enabling dark mode with `e-dark-mode` class globally
- Per-component dark mode customization
- Runtime theme switching implementation (toggles, checkboxes)
- Dark mode behavior for all Syncfusion components

**Output:** Color system architecture decided

---

## 4. Spacing & Typography Systems

### 4.1 Spacing Grid: Framework Defaults

**Decision Point:** Your framework likely has a spacing default. Respect it or override consciously.

**Tailwind Default:** 8pt grid (4px, 8px, 12px, 16px, 20px, 24px...)
- Why: Matches Bootstrap and common UI conventions
- When to override: If you need finer granularity (e.g., 4pt grid: 4px, 8px, 12px...)

**Bootstrap Default:** 8pt grid (with `-xs` variant for 4px)
- Why: Standard for component frameworks
- When to override: Rarely; Bootstrap's scale works

**Material Default:** 4dp grid (Material's base unit)
- Why: Material Design specifies this
- When to override: Never; use Material's spacing or break Material conventions intentionally
- **Blazor setup:** Define in CSS variables in `wwwroot/css/material-spacing.css`

**No Framework Default:** Choose 4pt for maximum flexibility or 8pt for simplicity
- Why: 4pt gives finer control; 8pt is simpler and still adequate
- Recommendation: 4pt if you're designing custom Blazor components; 8pt if you want speed
- **Blazor setup:** Define in CSS custom properties in your stylesheets

**The Anti-Pattern:** Creating a spacing scale that ignores your framework's base unit. If Tailwind uses 4px as the atom, don't add arbitrary 5px or 6px values—you'll break the rhythm.

### 4.2 Typography Hierarchy: Modular Scale, Not Random

**Non-Obvious Principle:** Too many font sizes that are too close together create muddy hierarchy.

Compare:
- ❌ Muddy: 14px, 15px, 16px, 18px, 20px (hard to distinguish hierarchy)
- ✅ Clear: 12px, 16px, 20px, 28px, 36px (obvious visual progression)

**Use a consistent ratio.** Common options:
- 1.25 (major third) — good balance, subtle but clear
- 1.33 (perfect fourth) — more contrast
- 1.5 (perfect fifth) — high contrast, for designs where hierarchy needs to be obvious

**Minimum Body Text Size:** Never smaller than 16px on screen. Smaller than this strains eyes and fails WCAG on mobile.

**Line Height Rule:** 1.5-1.6 for body text. Increase for light text on dark (add 0.05-0.1 because light text reads heavier). Decrease for headlines (1.2 is fine for short text).

**Framework-Specific Notes (Blazor):**
- Tailwind: Provides a default typography scale; customize it in `tailwind.config.js` and apply in `.razor` components
- Bootstrap: Provides default sizes (sm, base, lg, etc.); extend or override in `wwwroot/css/`
- Material: Specifies exact sizes for headline, body, label tiers; use Material's system in your stylesheets

**Decision Point:** Are you using your framework's default typography, or defining custom sizes?
- Default: Faster, proven, but less distinctive
- Custom: More flexibility, but requires testing for hierarchy clarity

**Output:** Spacing and typography systems decided

---

## 5. Responsive Strategy

### 5.1 Mobile-First Thinking

**Principle:** Start with mobile constraints, scale UP with breakpoints. Never design desktop-first and shrink.

Why: Mobile constraints force clarity. Desktop flexibility comes naturally. If you design desktop-first, you'll add unnecessary complexity that breaks on mobile.

### 5.2 Breakpoint Decision

**Standard breakpoints (mobile-first):**
- **Base (0px):** Mobile-optimized layout
- **Tablet (768px):** Content begins expanding
- **Desktop (1024px):** Full layout capability
- **Large (1280px+):** Multi-column, spacious layouts

**Framework Conventions:**
- **Tailwind:** Uses `sm`, `md`, `lg`, `xl` breakpoints; these map to the above
- **Bootstrap:** Uses `xs`, `sm`, `md`, `lg`, `xl`; similar logic
- **Material:** No built-in responsive framework; design with Material's guidelines in mind

**Content-Driven Overrides:** If your content needs a breakpoint at 600px or 900px, add it. Framework defaults are starting points, not absolutes.

**Decision Point:** Are you using your framework's default breakpoints?
- Default: Proven, consistent with framework ecosystem
- Custom: Only if content requires it; document the reasoning

### 5.3 Container Queries vs Viewport Queries

**Viewport queries** (`@media min-width`) work for page-level layouts. **Container queries** work for components that need to respond to their container width, not viewport.

**Example:**
- A card in a narrow sidebar should be compact
- The same card in a main content area should be spacious
- Container queries handle this automatically; viewport queries can't

**Modern best practice:** Use container queries for component layouts, viewport queries for page layouts.

**Compatibility Note:** Container queries are modern (90%+ browser support). If you need older browser support, viewport queries are your fallback.

---

### 5.4 Viewport Coverage: Quick Reference

**CSS Reset (in `wwwroot/css/app.css` or similar):**
```css
html, body {
  width: 100%;
  height: 100%;
  margin: 0;
  padding: 0;
}

#app {
  width: 100%;
  height: 100%;
}
```

**Full-Height Container (standard Blazor web app):**
```css
.container {
  width: 100%;
  min-height: 100vh;  /* NOT height: 100vh */
  padding: var(--space-sm);
}
```

**Mobile Web App (PWA with notches):**
```css
.full-screen {
  width: 100%;
  min-height: 100dvh;
  padding-top: max(var(--space-sm), env(safe-area-inset-top));
  padding-bottom: max(var(--space-sm), env(safe-area-inset-bottom));
}
```

**Key Rules:**
- Use `width: 100%`, NOT `100vw` (avoids scrollbar issues)
- Use `min-height: 100vh`, NOT `height: 100vh` (allows content expansion)
- Use `100dvh` on mobile (dynamic viewport height, accounts for address bars)

**Output:** Responsive strategy decided

---

## 6. Motion & Accessibility Standards

### 6.1 Motion Purpose and Timing

**Rule:** Animations serve specific purposes. Don't animate for aesthetics alone.

Good uses:
- **Transitions:** State changes (button press, hover)
- **Reveals:** Elements appearing (dropdown open, toast notification)
- **Feedback:** User actions acknowledged (loading spinner, success checkmark)

Bad uses:
- Decorative floating elements
- Parallax scrolling
- Anything that doesn't communicate intent

**Standard durations:**
- Micro (100ms): Hover feedback, immediate response
- Standard (300ms): Transitions, state changes, small reveals
- Slow (500ms): Major layout changes, important reveals

**The Non-Obvious Truth:** Slower isn't always better. 300ms feels responsive. 500ms feels sluggish. 100ms feels snappy but can feel jarring on slower devices.

### 6.2 Reduced Motion: Non-Negotiable

**WCAG Requirement:** Respect `prefers-reduced-motion: reduce` by removing animations.

This isn't optional accessibility—it's a legal requirement for accessible UI. Users with vestibular disorders experience motion sickness from animations.

**Implementation:** When a user has `prefers-reduced-motion` enabled, disable all animations (duration → 0ms).

### 6.3 Touch Targets

**Rule:** Interactive elements must be at least 44x44px (WCAG recommendation).

This includes:
- Buttons
- Form inputs
- Links
- Checkbox/radio areas

**Space them at least 8px apart** to prevent accidental touches.

**Visual vs Touch Size:** A button might *look* like 24x24px (visual icon), but its touch target should be 44x44px via padding or pseudo-elements.

### 6.4 Color Contrast

**WCAG 2.1 AA requirement:** 4.5:1 minimum contrast for text and UI components.

This means:
- Dark text on light background must be dark enough
- Light text on dark background must be light enough
- Placeholder text counts—it needs contrast too

**Common fail:** Light gray placeholder text on white. It looks good but fails accessibility.

**Testing:** Don't trust your eyes. Use WCAG contrast checkers to verify.

**Decision Point:** Are you aiming for AA (minimum legal requirement) or AAA (higher standard, harder to achieve)?

**Output:** Motion & accessibility standards understood

---

## 7. Design System Token Architecture

### 7.1 Token Naming: Semantic, Not Descriptive

**The Problem with Descriptive Names:**
- `--blue-600`, `--padding-16` are hardcoded to specific values
- If you need to change "blue" to "purple," you rename every token and break meaning
- New team members don't understand *why* a color is used

**Semantic Naming:**
- `--color-primary` means "brand color" (value irrelevant)
- `--space-lg` means "large spacing" in context
- `--font-heading` means "heading typography" (specific size irrelevant)

If you need to rebrand from blue to purple: change the value once, everywhere understands the intent.

### 7.2 Token Hierarchy (Levels)

**Level 1: Primitive Tokens**
- Base colors, spacing units, font sizes
- Framework-specific (Tailwind defines these; Bootstrap defines these; you define these if greenfield)
- Example: `--color-primary`, `--space-4px`, `--font-base`

**Level 2: Semantic Tokens**
- Composed from primitives, role-based
- `--color-text: var(--color-primary-900)` (semantic: "text should be dark primary")
- `--space-component-gap: var(--space-lg)` (semantic: "components space by large units")
- `--transition-standard: var(--duration-normal)` (semantic: "standard state changes take normal duration")
- **Blazor:** Use in `.razor` components via `style="color: var(--color-text)"` or CSS modules

**Level 3: Component Tokens (Optional)**
- Highly specific to Syncfusion or custom Blazor components
- Example: `--button-padding: var(--space-sm)` (button-specific override)
- Only create if you have many component-specific values

**Why This Hierarchy Matters:**
- Primitives stay stable (platform-specific)
- Semantics stay stable (intent-based, survives design changes)
- Component tokens are rare and explicit

### 7.3 Where Tokens Live

**Your framework determines storage:**

- **Tailwind:** `tailwind.config.js` (extends or replaces defaults) + `wwwroot/css/app.css` for custom overrides
- **Bootstrap:** SCSS variables in `wwwroot/scss/_variables.scss` (before importing Bootstrap), compiled to `wwwroot/css/`
- **Material:** Material Design tokens in `wwwroot/css/material-tokens.css` (CSS custom properties)
- **Greenfield:** `wwwroot/css/tokens.css` in your styles directory

**Decision Point:** Are you using your framework's native token system or creating custom tokens?
- Native: Easier, matches framework ecosystem, less custom CSS
- Custom: More control over Blazor component styling, but requires discipline to maintain

**Output:** Token architecture understood

---

## 8. Syncfusion Component Integration

### 8.1 Theme Registration

**Principle:** Your Syncfusion theme must be imported at app entry before any Blazor components render.

**Blazor WASM vs Blazor WebApp - CRITICAL DIFFERENCE:**

#### For Blazor WebAssembly (WASM):
Import Syncfusion CSS in `<head>` of `wwwroot/index.html`:
```html
<!-- Syncfusion Blazor theme (choose one) -->
<link href="_content/Syncfusion.Blazor.Themes/tailwind3.css" rel="stylesheet" />
<!-- or Bootstrap5: Syncfusion.Blazor.Themes/bootstrap5.css -->
<!-- or Material3: Syncfusion.Blazor.Themes/material3.css -->
```

**Why `index.html`:** In Blazor WASM, `index.html` is the entry point where the Blazor app boots. Themes must be loaded before components render.

#### For Blazor WebApp (Server or Auto-Render):
Import Syncfusion CSS in `Components/App.razor` (or `App.razor` root component):
```razor
@* At the top of Components/App.razor before any child components *@

<head>
    <!-- ... other head content ... -->
    
    <!-- Syncfusion Blazor theme (choose one) -->
    <link href="_content/Syncfusion.Blazor.Themes/tailwind3.css" rel="stylesheet" />
    <!-- or Bootstrap5: Syncfusion.Blazor.Themes/bootstrap5.css -->
    <!-- or Material3: Syncfusion.Blazor.Themes/material3.css -->
</head>

<body>
    @* Blazor app content *@
</body>
```

**Why `App.razor`:** In Blazor WebApp, `App.razor` is the root component that renders the app shell and routes. Theme must be applied at this component level for all pages.

**Alternative for WebApp:** If your layout uses a HeadOutlet component:
```razor
@* In Components/App.razor or Components/Routes.razor *@

<HeadOutlet />  @* This renders content from App.razor's <head> to actual document head *@
```

Then in `Components/App.razor`:
```razor
<head>
    <link href="_content/Syncfusion.Blazor.Themes/tailwind3.css" rel="stylesheet" />
    <!-- other head content -->
</head>
```

This ensures:
- Syncfusion Blazor components use the correct visual system
- Styling cascade works properly
- No flash of unstyled content
- Components render with theme applied immediately

**Decision Point:** Your Syncfusion theme is now locked. Document which theme you chose and why (should match section 2 decision).

**📖 For Component-Specific Import Details:**
Each Syncfusion component skill (Stage 3 Getting Started) includes:
- Individual component theme imports for your selected framework
- Component-specific CSS imports (not global theme)
- Version matching requirements
- Optimized (lite) CSS options per component

### 8.2 Script Registration

**Principle:** Syncfusion Blazor components require JavaScript interop scripts registered **before** Blazor bootstrap. Script order determines initialization success.

**CRITICAL: Script Load Order**
1. CSS Themes (in `<head>`)
2. Syncfusion JavaScript scripts (in `<body>`, early)
3. Blazor bootstrap script (`_framework/blazor.web.js`, last)

**Mismatch = unresponsive components or 404 errors.**

---

#### Blazor WebAssembly (WASM)

**File:** `wwwroot/index.html`

```html
<!DOCTYPE html>
<html>
<head>
    <link href="_content/Syncfusion.Blazor.Themes/tailwind3.css" rel="stylesheet" />
    <!-- other head elements -->
</head>
<body>
    <div id="app"></div>
    
    <!-- 1. Syncfusion scripts (before Blazor) -->
    <script src="_content/Syncfusion.Blazor.Core/scripts/syncfusion-blazor.min.js"></script>
    
    <!-- 2. Blazor bootstrap (last) -->
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
    <!-- other head elements -->
</head>
<body>
    <!-- 1. Syncfusion scripts (before Routes) -->
    <script src="_content/Syncfusion.Blazor.Core/scripts/syncfusion-blazor.min.js"></script>
    
    <!-- 2. App routes -->
    <Routes />
    
    <!-- 3. Blazor bootstrap (last) -->
    <script src="_framework/blazor.web.js"></script>
</body>
</html>
```

---

#### Troubleshooting Quick Reference

| Issue | Cause | Fix |
|-------|-------|-----|
| **404: syncfusion-blazor.min.js not found** | NuGet package missing or project not built | `dotnet add package Syncfusion.Blazor.Core && dotnet build` |
| **Components unresponsive, no errors** | Script loaded after Blazor bootstrap | Move Syncfusion script **before** `blazor.web.js` |
| **Partial component functionality** | Missing component-specific scripts (RichTextEditor, PDFViewer, etc.) | Check component skill documentation for additional scripts |

**Verify:** Open DevTools (F12) → Network tab → Reload → Check `syncfusion-blazor.min.js` loads with status 200 before `blazor.web.js`

**⚠️ CRITICAL: Component-Specific Scripts**

**Some Syncfusion Blazor components require ADDITIONAL scripts beyond the base `syncfusion-blazor.min.js`.** These MUST be added to your app entry point if you're using these components:

| Component | Package | Required Script | Purpose |
|-----------|---------|-----------------|---------|
| **SfPdfViewer2** | Syncfusion.Blazor.SfPdfViewer | `_content/Syncfusion.Blazor.SfPdfViewer/scripts/syncfusion-blazor-sfpdfviewer.min.js` | PDF rendering & annotations |
| **SfSmartPdfViewer** | Syncfusion.Blazor.SfSmartPdfViewer | `_content/Syncfusion.Blazor.SfSmartPdfViewer/scripts/syncfusion-blazor-sfsmartpdfviewer.min.js` | AI-powered PDF features |
| **SfDocumentEditorContainer** | Syncfusion.Blazor.WordProcessor | `_content/Syncfusion.Blazor.WordProcessor/scripts/syncfusion-blazor-documenteditor.min.js` | Document editing & formatting |
| **SfSpreadsheet** | Syncfusion.Blazor.Spreadsheet | `_content/Syncfusion.Blazor.Spreadsheet/scripts/syncfusion-blazor-spreadsheet.min.js` | Spreadsheet calculations & data |

**If you will be using ANY of these components in Stage 3 component mapping, you MUST:**

1. **Plan for these scripts in Stage 4** (this decision-making stage)
2. **Add scripts to your app entry point in Stage 8** (code insertion stage)

**Script Placement (Add AFTER base Syncfusion script):**

**WASM** - `wwwroot/index.html`:
```html
<script src="_content/Syncfusion.Blazor.Core/scripts/syncfusion-blazor.min.js"></script>
<!-- Component-specific scripts (if needed) -->
<script src="_content/Syncfusion.Blazor.SfPdfViewer/scripts/syncfusion-blazor-sfpdfviewer.min.js" type="text/javascript"></script>
<script src="_content/Syncfusion.Blazor.SfSmartPdfViewer/scripts/syncfusion-blazor-sfsmartpdfviewer.min.js" type="text/javascript"></script>
<script src="_content/Syncfusion.Blazor.WordProcessor/scripts/syncfusion-blazor-documenteditor.min.js" type="text/javascript"></script>
<script src="_content/Syncfusion.Blazor.Spreadsheet/scripts/syncfusion-blazor-spreadsheet.min.js" type="text/javascript"></script>
```

**WebApp** - `Components/App.razor`:
```razor
<script src="_content/Syncfusion.Blazor.Core/scripts/syncfusion-blazor.min.js"></script>
<!-- Component-specific scripts (if needed) -->
<script src="_content/Syncfusion.Blazor.SfPdfViewer/scripts/syncfusion-blazor-sfpdfviewer.min.js" type="text/javascript"></script>
<script src="_content/Syncfusion.Blazor.SfSmartPdfViewer/scripts/syncfusion-blazor-sfsmartpdfviewer.min.js" type="text/javascript"></script>
<script src="_content/Syncfusion.Blazor.WordProcessor/scripts/syncfusion-blazor-documenteditor.min.js" type="text/javascript"></script>
<script src="_content/Syncfusion.Blazor.Spreadsheet/scripts/syncfusion-blazor-spreadsheet.min.js" type="text/javascript"></script>
```

**Consequences of Missing Scripts:**
- ❌ Components will not render or be non-functional
- ❌ PDF Viewer: Will not display documents
- ❌ Spreadsheet: Will not calculate formulas or display data
- ❌ Document Editor: Will not allow editing or formatting
- ❌ Smart PDF Viewer: Will not work or show AI features

**Refer to:** `references/nuget-packages.md` → "Component-Specific Script References" section for complete documentation.

**Output:** Script registration strategy implemented and verified (including component-specific scripts if needed)

### 8.3 Custom CSS Coordination

**Non-Obvious Pattern:** Don't override Syncfusion Blazor component colors directly. Instead:

1. Define your color system in tokens (CSS custom properties)
2. Syncfusion theme provides base styling for Blazor components
3. Custom CSS in `wwwroot/css/` layers on top, using your tokens

**Example thinking:**
- ❌ Override SfGrid header color to `#ff0000` in component's `<style>`
- ✅ SfGrid inherits from Syncfusion Blazor theme, which respects your primary color token (CSS custom property)

This keeps styling coordinated and maintainable.

**Material Design Exception:**
If using Material Design, you'll need custom Material-aligned CSS for Syncfusion components. Stage 5 will handle this based on your decisions here.

**📖 For CSS Variable Customization:**
Refer to **Skill: syncfusion-blazor-themes** → **CSS Variables Customization** for:
- CSS variable structure for each theme (Material 3, Fluent 2, Bootstrap 5.3, Tailwind 3.4)
- Customizing primary, success, warning, danger, info colors
- Runtime color modification with JavaScript
- Theme-specific variable formats (RGB vs hex values)

**Output:** Syncfusion integration strategy decided

---

## 9. Load Your Framework Reference (MANDATORY)

**REQUIRED STEP:** Your framework choice from Sections 1-2 now determines your implementation guide.

### Auto-Detected Framework Reference

Based on your **CSS Framework Philosophy** selection in Section 1 and **Syncfusion Theme Alignment** in Section 2, your framework reference is automatically locked:

#### If You Selected: **Tailwind CSS**
→ **Load Reference:** [Tailwind CSS Implementation](tailwind-implementation.md)

**What this reference provides:**
- Tailwind 3.x setup with Syncfusion Blazor components
- `tailwind.config.js` configuration patterns for Blazor projects
- Token architecture using Tailwind's color/spacing scale
- Pure utility-class component generation in `.razor` files
- Responsive design with Tailwind breakpoints (sm, md, lg, xl)
- Syncfusion Tailwind3 theme integration in Blazor
- Stage 6 validation checklist for Tailwind Blazor projects

**Key principle:** You'll define custom colors in `tailwind.config.js`, then extend Syncfusion's Tailwind3 theme using Tailwind utility classes in your `.razor` components. No CSS variables needed.

---

#### If You Selected: **Bootstrap**
→ **Load Reference:** [Bootstrap 5 Implementation](bootstrap-implementation.md)

**What this reference provides:**
- Bootstrap 5 setup with Syncfusion Blazor components
- Semantic color names (primary, secondary, success, danger, warning, info) in your `.razor` files
- 8pt spacing grid and responsive utilities for Blazor layouts
- 12-column grid system with mobile-first breakpoints (xs, sm, md, lg, xl, xxl)
- SCSS configuration and CSS variable setup in `wwwroot/scss/`
- Syncfusion Bootstrap5 theme integration in Blazor
- Stage 6 validation checklist for Bootstrap Blazor projects

**Key principle:** Utility-first approach using Bootstrap semantic colors and spacing scale. You'll use Bootstrap utility classes for layout, spacing, and responsive design in your `.razor` components. Syncfusion Bootstrap5 theme handles component styling automatically.

---

#### If You Selected: **Material Design**
→ **Load Reference:** [Material Design 3 Implementation](material-implementation.md)

**What this reference provides:**
- Material Design 3 system with Syncfusion Material3 theme
- 4dp spacing grid (not 8pt); semantic color tokens (primary, secondary, tertiary, error, surface) in `wwwroot/css/`
- Material typography roles (Display, Headline, Title, Body, Label) for `.razor` components
- Elevation system 0-5 with tonal surfaces for Blazor layouts
- Canonical responsive layouts (Compact < 600dp, Medium 600-840dp, Expanded > 840dp)
- Material token configuration (CSS custom properties in stylesheets)
- Syncfusion Material3 theme integration in Blazor
- Stage 6 validation checklist for Material Design Blazor projects

**Key principle:** Utility-first with Material system rules applied to Blazor components. All spacing from 4dp grid, typography from Material roles, colors from Material semantic tokens. Syncfusion Material3 theme handles component styling automatically.

---

#### If You Selected: **Greenfield / No Framework**
→ **Load Reference:** [Greenfield / Custom CSS Implementation](greenfield-implementation.md)

**What this reference provides:**
- Custom design system architecture from scratch for your Blazor project
- CSS custom properties (CSS variables) token system in `wwwroot/css/tokens.css`
- Syncfusion Tailwind3 theme as visual baseline for Blazor components
- Spacing scale design (4pt or 8pt grid, your choice) for `.razor` components
- Typography scale with modular ratio
- Color system in OKLCH color space (perceptually uniform)
- Custom responsive breakpoint strategy for Blazor layouts
- Dark mode implementation strategy
- Syncfusion Tailwind3 theme integration in Blazor
- Stage 6 validation checklist for greenfield Blazor projects

**Key principle:** You own all design decisions in Blazor. Recommended: use Syncfusion Tailwind3 theme as a visual foundation, then layer custom CSS variables and responsive utilities in your `.razor` components. Maximum flexibility, maximum responsibility.

---

**You cannot proceed to Stage 5 without reviewing your framework reference.**

**Output:** Framework implementation reference locked based on your Section 1-2 decisions

---

## 10. Stage 4 Decision Checklist

**Load Your Framework Reference (MANDATORY)**

**Upon completion, confirm the following decisions are locked:**

### Framework & Theme
- ✅ CSS framework confirmed (Tailwind/Bootstrap/Material/Greenfield)
- ✅ Syncfusion theme selected and documented (Tailwind3/Bootstrap5/Material3)
- ✅ Framework philosophy understood (why this framework for this project)

### Package Installation & Configuration
- ✅ All required Syncfusion packages installed in single command: `dotnet add package Syncfusion.Blazor.Core Syncfusion.Blazor.Themes Syncfusion.Blazor.* && dotnet build`
- ✅ Syncfusion services registered in `Program.cs`: `builder.Services.AddSyncfusionBlazor();`
- ✅ Syncfusion theme CSS imported in project entry point (index.html or App.razor)
- ✅ Syncfusion scripts registered before Blazor bootstrap
- ✅ Project built successfully: `dotnet build` (no errors)

### Color System
- ✅ Color space decided (OKLCH recommended; Material 3 for Material projects)
- ✅ Primary/semantic colors defined
- ✅ Tinted neutrals strategy understood
- ✅ Dark mode decision made (light only / dark only / both)

### Spacing & Typography
- ✅ Spacing scale confirmed (framework default or custom, with rationale)
- ✅ Typography hierarchy locked (modular scale ratio or framework convention)
- ✅ Line height rules applied (readability standards)

### Responsive Design
- ✅ Breakpoint strategy decided (framework defaults or content-driven)
- ✅ Mobile-first thinking confirmed
- ✅ Container queries decision made (yes/no/with fallbacks)

### Accessibility
- ✅ Motion standards applied (100ms / 300ms / 500ms durations)
- ✅ Reduced motion support confirmed
- ✅ Touch targets sized (44x44px minimum)
- ✅ Color contrast verified (WCAG AA or AAA goal)

### Token Architecture
- ✅ Token storage location decided:
  - **Tailwind projects:** `tailwind.config.js` (primary colors only, extend theme)
  - **Bootstrap projects:** SCSS variables or custom CSS
  - **Material projects:** Material Design tokens
- ✅ Semantic token naming understood (not descriptive)
### Implementation approach locked:
  - **Tailwind:** Pure Tailwind utility classes in `.razor` markup (NOT CSS variables)
  - **Bootstrap:** Bootstrap utilities or component classes in `.razor` files
  - **Material:** Material Design tokens (CSS custom properties) + custom CSS in stylesheets

### Syncfusion Integration
- ✅ Theme registration point confirmed (app entry point)
- ✅ Color coordination strategy understood (inherit from tokens, don't override)

### Framework Reference (MANDATORY)
- ✅ Framework-specific reference file loaded (Tailwind/Bootstrap/Material/Greenfield)
- ✅ Implementation guide understood for your framework
- ✅ Ready to proceed to Stage 5 with framework decisions locked

---

## What Stage 5 Does With These Decisions

Stage 5 (Code Generation) uses your Stage 4 decisions to generate Blazor components:
- **Framework setup** with correct Syncfusion Blazor theme imports in `wwwroot/index.html`
- **Token definitions** in framework-appropriate format (`tailwind.config.js`, SCSS variables, Material tokens)
- **Base `.razor` component styles** following your accessibility standards
- **Responsive utilities** aligned to your breakpoint strategy
- **Syncfusion Blazor component integration** (SfGrid, SfButton, SfTextBox, etc.) that respects your token system

Stage 5 generates *implementation*, not *decisions*. The decisions you locked in Stage 4 ensure Stage 5 output is consistent and coherent.

---

### For All Blazor Projects:
- ✅ Syncfusion Blazor imports match locked theme (Tailwind3/Bootstrap5/Material3)
- ✅ WCAG 2.1 AA accessibility (contrast, focus states, reduced-motion) in `.razor` components
- ✅ Responsive design verified on breakpoints (320px, 768px, 1024px, 1280px)
- ✅ C# compilation without errors
- ✅ Build optimization (CSS purging, code splitting)
- ✅ Syncfusion Blazor component configuration and data binding

**Output:** Production-ready `.razor` components aligned with Stage 4 design decisions
