# Validation Rules Reference — Blazor Edition

**Purpose:** Comprehensive checklist for Stage 7 validation. Used to validate Blazor `.razor` components against web standards (WCAG 2.1 AA, security, performance).

## Binary Validation Result

Each component receives a **PASS ✓** or **FAIL ✗** result against these rules.

---

## Accessibility (WCAG 2.1 AA) - Blocking

| Rule | Check | Pass/Fail |
|------|-------|-----------|
| **Semantic HTML** | Form elements use semantic tags: `<input>`, `<label>`, `<button>`, not divs with roles | |
| **Blazor Inputs** | Syncfusion/HTML inputs have `@bind-*`, proper `id`, associated `<label>` | |
| **ARIA Attributes** | Inputs have `aria-label` or `<label>` with `for="id"` matching | |
| **Error Messages** | Invalid inputs have `aria-invalid="true"` linked via `aria-describedby` | |
| **Focus Indicator** | All interactive elements have visible focus via `:focus-visible` | |
| **Keyboard Navigation** | Tab order logical (left→right, top→bottom), no keyboard traps | |
| **Color Contrast** | Text ≥ 4.5:1, focus indicator ≥ 3:1, icons ≥ 3:1 contrast | |
| **Touch Targets** | Interactive elements ≥ 44×44px (buttons, links, inputs) | |
| **@key Directive** | Lists use `@key` for proper rendering/accessibility in loops | |

---

## Security - Blocking

| Rule | Check | Pass/Fail |
|------|-------|-----------|
| **No XSS** | No `@((MarkupString)userInput)` without sanitization; user input auto-escaped | |
| **Input Validation** | All user input validated server-side before processing | |
| **No Secrets** | No hardcoded API keys, JWT tokens, or database connection strings | |
| **Config Security** | Secrets in `appsettings.{Environment}.json` (gitignored) or user secrets | |
| **Syncfusion License** | License key from `appsettings.json` or `appsettings.{Environment}.json`, never hardcoded | |
| **Null Safety** | Nullable reference types enabled; no implicit null-forgiving operators (!) | |

---

## Performance - Warning (Auto-fixable)

| Rule | Check | Status |
|------|-------|--------|
| **ShouldRender()** | Heavy components override `ShouldRender()` to prevent unnecessary renders | ⚠️ Can auto-fix |
| **Event Handlers** | Event handlers use stable method references (not new delegates) | ⚠️ Can auto-fix |
| **Bundle Size** | No unnecessary NuGet packages or large libraries | ⚠️ Can warn |
| **Async Operations** | Data loading in `OnInitializedAsync()`, not in render logic | ⚠️ Can auto-fix |
| **List Virtualization** | Large lists (>100 items) use `<Virtualize>` component | ⚠️ Can auto-fix |

---

## Responsive Design - Warning (Auto-fixable)

| Rule | Check | Status |
|------|-------|--------|
| **Mobile-First** | CSS/Tailwind starts mobile (320px), then expands with media queries | ⚠️ Can auto-fix |
| **Flexbox/Grid** | No fixed widths in `.razor` markup; uses responsive layouts | ⚠️ Can auto-fix |
| **Media Queries** | CSS breakpoints at 320px, 768px, 1024px (or framework defaults) | ⚠️ Can auto-fix |
| **Touch Targets** | All interactive elements ≥ 48px on mobile (not just 44px) | ⚠️ Can auto-fix |
| **Viewport Meta** | `<meta name="viewport" content="width=device-width, initial-scale=1">` in `wwwroot/index.html` | ⚠️ Can add |

---

## Code Quality - Warning

| Rule | Check | Status |
|------|-------|--------|
| **Nullable Types** | `<Nullable>enable</Nullable>` in `.csproj`; no implicit nulls | ⚠️ Can auto-fix |
| **XML Comments** | Component parameters and public methods documented with `///` | ⚠️ Can add |
| **Error Handling** | Try-catch on async operations; user-friendly error messages | ⚠️ Can auto-fix |
| **Async/Await** | All async operations properly awaited; no `Task` without `await` | ⚠️ Can auto-fix |
| **No Dynamic** | No use of `dynamic` keyword; static typing enforced | ⚠️ Can warn |

---

## Validation Logic (Stage 7)

### Step 1: Check Blocking Rules
If ANY blocking rule fails → **FAIL ✗**
- Inaccessible form (unsemantic HTML, missing ARIA, missing `@bind-*`)
- XSS vulnerability (`@((MarkupString)userInput)` without sanitization)
- Hardcoded secrets or license keys in `.razor` code
- Security: No nullable type checking (`#nullable enable`)

**Action:** Auto-fix if possible. If not auto-fixable, ask user to override or request fixes.

### Step 2: Check Auto-Fixable Warnings
Apply auto-fixes:
- Missing color contrast → Adjust CSS variables in design system
- Missing ARIA attributes → Add `aria-label`, `aria-describedby` to Syncfusion components
- Poor keyboard navigation → Fix `@onkeydown` handler, verify tab order
- Missing touch target size → Increase `SfButton`, input sizes in `.razor`
- Missing responsive styles → Add `@media (max-width: 768px)` queries in CSS

### Step 3: Check Non-Auto-Fixable Warnings
Report to user:
- Missing XML documentation (`///` comments)
- Could benefit from `ShouldRender()` override
- No error handling for `OnInitializedAsync()` async operations

**Action:** Warn but allow proceeding.

### Step 4: Output Result

**If all blocking rules pass + warnings auto-fixed:**
```
✓ VALIDATION PASS

All standards met:
  ✓ WCAG 2.1 AA accessibility (Razor component)
  ✓ Security checks (no XSS, secrets in config)
  ✓ Performance optimizations (ShouldRender, async patterns)
  ✓ Responsive design (media queries, flexbox)
  
Auto-fixes applied: 4
  - Fixed color contrast on SfButton
  - Added aria-describedby to SfTextBox inputs
  - Added @key directive to @foreach loops
  - Updated media queries for mobile breakpoint

Ready to proceed to Stage 8...
```

**If blocking rule fails (not auto-fixable):**
```
✗ VALIDATION FAIL

Critical issues:
  ✗ Form inputs in MyComponent.razor missing semantic HTML (using <div> instead of <input>)
  ✗ @((MarkupString)userInput) without sanitization in MyComponent.razor line 42

Auto-fixes NOT available for these issues.

Options:
  [Override & Proceed] [Request Manual Fixes] [Cancel]
```

---

## Override Behavior

If user overrides failed validation:
```
⚠️  Proceeding with known accessibility/security issues in MyComponent.razor:
  - Color contrast: 3.8:1 (need 4.5:1)
  - Missing aria-label on 2 SfTextBox inputs
  - Missing @key in @foreach loop (line 28)

Code will be generated but flagged as non-compliant.
User assumes responsibility for fixing before production deployment.
```

---

## Blazor Component Structure Checklist

Verify generated `.razor` components contain:
- ✓ `@page "/path"` directive (if routable page)
- ✓ `@using` directives for Syncfusion and app namespaces
- ✓ Semantic HTML or Syncfusion component markup
- ✓ ARIA attributes for accessibility
- ✓ `@key` directive in `@foreach` loops
- ✓ `@onclick`, `@onchange`, `@bind-*` event bindings
- ✓ `@code { }` block with `[Parameter]` attributes and lifecycle methods
- ✓ `OnInitializedAsync()` for data loading (not in render)
- ✓ `ShouldRender()` override for performance-critical components
- ✓ No hardcoded secrets (use `IConfiguration` from `appsettings.json`)
- ✓ Error handling in async operations
- ✓ XML documentation (`///`) for public parameters

---

All validation rules ensure generated Blazor code is accessible, secure, and production-ready. Stage 7 validation gates deployment to production.
