# Stage 7: Validation

**Purpose:** Validate generated Blazor code against web standards. Binary pass/fail result.

**AI Should:**

1. **Validate WCAG 2.1 AA Compliance:**
   - Semantic HTML structure used in `.razor` markup? (form, label, input, button tags)
   - ARIA labels on form fields and Syncfusion components?
   - Keyboard navigation supported? (tab order, focus management in Blazor components)
   - Color contrast ≥ 4.5:1 for text?
   - Focus indicator visible on Syncfusion interactive elements?

2. **Check Security:**
   - No XSS vulnerabilities (HTML content properly encoded, no `@Html.Raw` without sanitization)
   - No hardcoded secrets/API keys in `.razor` or C# code-behind
   - User input sanitized if used in bindings or HTML context
   - No vulnerable inline @onclick handlers without validation
   - Data binding uses proper C# types (avoid `dynamic` without validation)

3. **Verify Performance:**
   - `@key` directive used for list rendering in `.razor` files?
   - `shouldRender` method used to prevent unnecessary re-renders?
   - Async operations properly handled (no blocking calls)?
   - Component parameters avoid unnecessary changes?
   - Event handlers use proper delegation without leaks?
   - Code is optimized for Blazor WebAssembly (if applicable)?

4. **Check Responsive Design:**
   - Mobile-first approach (320px+)
   - Flexbox/Grid used for layouts?
   - Media queries for breakpoints?
   - Touch targets ≥ 44x44px?

5. **Validate Syncfusion Code Quality:**
   - **Enum Namespace Qualification**: All Syncfusion enums use full namespace (e.g., `Syncfusion.Blazor.Buttons.ButtonType.Primary`)
   - **@using Directives**: All required component namespaces included
   - **Component Parameters**: Proper C# types and binding syntax
   - **Event Handlers**: Correct Blazor event binding patterns

**Validation Result:**

Binary: **PASS ✓** or **FAIL ✗**

**If PASS:**
```
✓ Validation Result: PASS

All standards met:
  ✓ WCAG 2.1 AA accessibility
  ✓ Security checks
  ✓ Performance standards
  ✓ Responsive design
  ✓ Code quality

Ready to proceed to dependencies...
```

**If FAIL:**
```
✗ Validation Result: FAIL

Issues found:
  ✗ Color contrast on label text (3.2:1, need 4.5:1)
  ✗ Form inputs missing aria-describedby attributes
  ✗ SfTextBox component missing @bind-Value for two-way binding

Auto-fixes applied:
  ✓ Increased font size for contrast in Blazor component
  ✓ Added aria-describedby to Syncfusion inputs
  ✓ Added @bind-Value to SfTextBox components

Remaining issues: 0
Result: PASS (after fixes)
```

**User Interaction:** ⭐ **USER DECISION #2**

If result is PASS:
```
Ready to generate dependencies?
[Proceed] [Review] [Stop]
```

If result is FAIL (after fixing):
```
Validation failed with 2 issues (not auto-fixable):
  - Blazor component requires custom EventCallback for validation errors
  - SfGrid component needs custom RowTemplate for complex styling

Override and proceed anyway?
[Override & Proceed] [Request Manual Fixes] [Stop]
```

**Status:** ⭐ **USER DECISION #2** - User confirms validation result or overrides.

**Blazor-Specific Validation Checks:**
- Component parameters properly defined with `[Parameter]` attribute?
- `OnInitializedAsync` / `OnParametersSetAsync` used correctly?
- Syncfusion component services registered in `Program.cs`?
- Two-way binding (`@bind-*`) used appropriately?
- Event callbacks (`EventCallback`) properly defined for custom events?

**Reference:** See web-standards.md for complete validation rules and Blazor-specific correction methods.
