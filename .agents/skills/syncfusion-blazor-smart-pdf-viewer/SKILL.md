---
name: syncfusion-blazor-smart-pdf-viewer
description: Implements Syncfusion Blazor Smart PDF Viewer (SfSmartPdfViewer) for AI-powered document viewing, interactions like summarizer, redaction, form filling and processing in Blazor applications.
metadata:
  author: Syncfusion Inc
  version: "33.1.44"
---

# Syncfusion Blazor Smart PDF Viewer – UI Sample Generator

  **NuGet:** `Syncfusion.Blazor.SfSmartPdfViewer` + `Syncfusion.Blazor.Themes`  
  **Namespace:** `Syncfusion.Blazor.SmartPdfViewer`

## Generate C# Code for the User's Project *(default)*

**Trigger keywords:** "how to", "add smart pdfviewer", "code sample", "show me", "example", "snippet", "integrate", "component", "create sample", "document summarization", "smart redaction", "smart fill".

**Purpose:** Generate minimal, copy-pasteable C# and Razor code that the user can integrate directly into their Blazor project.

**Workflow:**

**⚠️ CRITICAL — Feature Support Policy (STRICT MODE):**

**FUNDAMENTAL RULE:** Only generate code using APIs and properties that are EXPLICITLY listed in the reference files. ANY deviation is a VIOLATION.

- **MANDATORY CHECKS BEFORE GENERATING ANY CODE:**
  1. Search the reference files for the exact API/property name
  2. Verify it appears in the Method Reference, Properties, or Events tables
  3. If NOT found in ANY reference file, STOP immediately
  4. Do NOT generate or suggest undocumented APIs under any circumstances

- **STRICT ENFORCEMENT - ZERO TOLERANCE:**
  - **NO custom properties** - Only use properties from reference file tables
  - **NO invented methods** - Only use methods from reference file tables
  - **NO workarounds with undefined APIs** - Forbidden
  - **NO assumptions** about undocumented behavior - Forbidden
  - **NO alternative implementations** using guess-work - Forbidden
  - **NO pretending support exists** - Forbidden

- **MANDATORY RESPONSE FOR UNSUPPORTED FEATURES:**
  - **If a requested scenario/feature/API is NOT listed in any reference file, you MUST respond with:**
    ```
    "This feature is not supported in the current Syncfusion Blazor Smart PDF Viewer implementation."
    ```
  - **Then list what IS supported** from the appropriate reference file
  - **Never suggest alternatives** unless explicitly documented in reference files

- **REFERENCE FILE HIERARCHY:**
  - Each reference file contains complete, authoritative documentation for its domain
  - The tables (Method Reference, Properties, Events) are the SOURCE OF TRUTH
  - Content outside these tables in reference files is explanatory only
  - Do NOT extend beyond what appears in the reference file tables

- **AUDIT YOUR GENERATION:**
  - Before providing any code, verify EVERY API used appears in a reference file table
  - Document which reference file each API comes from
  - If you cannot cite a reference file table entry, DELETE that code

- **This is a CRITICAL REQUIREMENT.** Violations compromise the skill's integrity and reliability.

### Step 1 — Detect the Application Type *(REQUIRED - DO NOT SKIP)*
- **Use file_search and read_file tools to inspect workspace project files:**
  - `.csproj` file (project configuration)
  - `Program.cs` (startup configuration)
  - `_Imports.razor` (namespace imports)
  - `App.razor` (root component)
  - Any existing `.razor` files in Components/Pages
- **Output:** Confirm the detected application type (e.g., "Blazor WebApp" or "Blazor Server") before proceeding.

### Step 2 — Generate Code from Reference Files Only *(REQUIRED)*
- **Before generating:** Confirm that Steps 1 are complete
- Read the relevant `references/*.md` file(s) for the requested feature
- Cross-reference EVERY API, property, and method against these tables
- **DECLARATIVE-ONLY APPROACH (MANDATORY - NO LIFECYCLE METHODS):**
  - Use ONLY property binding in Razor markup similar lifecycle methods.
  - Configure all Smart PDF Viewer properties directly as component attributes
- **MANDATORY:** Before generating ANY code, verify that reference files exist and are accessible
- **Read the appropriate reference file(s)** for the requested feature:
  - Use `read_file` tool on relevant `references/*.md` files
  - Confirm file contains Methods/Properties/Events tables
  - Verify tables are complete and readable
- **If reference file is missing or cannot be read:**
  - STOP code generation
  - Respond: "Reference file for this feature is not available. Please ensure all reference files are present in the `references/` directory."
  - List the missing reference file name
- **This is a BLOCKER step:** Cannot proceed without reference file validation
- If an API/property does NOT appear in the reference file table, DO NOT USE IT
- Do NOT invent, guess, or suggest any API, method, property, class, or namespace not explicitly present in the reference files

## Code References

All templates and operation snippets live in `references/*.md`. Each file is a focused snippet or template the agent will combine when generating samples.

| File | Purpose |
|---|---|
| [getting-started.md](references/getting-started.md)| Foundation setup and minimal Smart PDF Viewer component. Includes NuGet package installation, service registration, Azure OpenAI configuration, and complete setup prerequisites. |
| [smart-redaction.md](references/smart-redaction.md)| AI-powered smart redaction configuration. Covers enabling/disabling redaction, custom redaction patterns (Person Names, Email Addresses, Phone Numbers, Credit Card Numbers, etc.), and default pattern definitions. |
| [smart-fill.md](references/smart-fill.md)| Smart form filling configuration. Documents the SmartFillSettings class for AI-powered automatic PDF form field population, enable/disable controls, and integration guidelines. |
| [document-summarizer.md](references/document-summarizer.md)| Document summarization and AI assistant configuration. Includes AssistViewSettings parameters like ShowPromptSuggestions, Prompt binding, PromptChanged callbacks, Placeholder, MinLength, and StreamResponse for real-time AI responses. |
| [custom-ai-service.md](references/custom-ai-service.md)| Custom AI service integration guide. Covers IChatInferenceService interface implementation, service registration in Program.cs, error handling with ErrorDialogService, and custom chat client integration patterns. |

## Quick Start Example

```razor
@rendermode InteractiveServer
```

Add component:
```razor
<SfSmartPdfViewer Height="100%" Width="100%" DocumentPath="@DocumentPath">
</SfSmartPdfViewer>
```

---
