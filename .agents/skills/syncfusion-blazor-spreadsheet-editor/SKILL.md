---
name: syncfusion-blazor-spreadsheet-editor
description: Use this skill for Blazor apps needing Excel-like UI using the Syncfusion Spreadsheet Component. Trigger for creating, viewing, editing Excel (.xlsx, .xls) files; embedding spreadsheet editors; using formulas, filtering, sorting, or cell formatting. Also trigger when users reference spreadsheet files ("open xlsx", "load Excel file", "add Syncfusion spreadsheet"). Do NOT trigger for standalone file processing without UI components.
metadata:
  author: "Syncfusion Inc"
  version: "33.1.44"
---

# Syncfusion Blazor Spreadsheet Editor

## Overview

Generates production-ready C# and Razor code for integrating the Syncfusion Blazor Spreadsheet component into Blazor Server or WebAssembly applications. The skill produces copy-pasteable code snippets with Excel-like functionality including cell editing, formatting, formulas, data operations, and worksheet management.

## Key Capabilities

- **Data & Binding:** Load from `byte[]`, local JSON file, remote JSON URL, or Google Drive; save via Ribbon
- **Cell & Range Operations:** Editing with events, UpdateCell, set formulas, autofill, range actions, merge/unmerge cells, cell formatting, number formats
- **Rows, Columns & Worksheets:** Insert/resize rows and columns; insert, delete, move, and duplicate worksheets
- **Data Operations:** filtering, sorting, hyperlink add/edit/remove, cut/copy/paste, cell and range selection
- **UI & Interactivity:** Hide/show Formula bar , Hide/show Context Menu, custom data rendered via XlsIO
- **Protection & History:** Sheet and workbook protection/unprotection, undo/redo operations
- **Images:** Insert, resize, and move images through UI (enabled by default)

## Quick Start Examples

### Example 1: Display a basic spreadsheet
**User:** "Add a basic Spreadsheet to my Blazor page"
**Result:** C# + Razor code snippet displayed (no files created)

### Example 2: Disable filtering and sorting
**User:** "Create a spreadsheet with filtering and sorting disabled"
**Result:** Basic sample merged with `filtering.md` and `sorting.md` snippets

---

## Generate C# Code for the User's Project *(default)*

**Trigger keywords:** "how to", "add spreadsheet", "code sample", "show me", "example", "snippet", "integrate", "component", "create sample", "code", "sample code", "generate code", "implement", "add to project", "configure spreadsheet"

### Step 1 - Analyze Request & Select Snippets

- Parse the user's intent and identify the requested features.
- Always start with `references/basic-sample.md` as the base.
- Map each requested feature to its corresponding `references/*.md` file.
- If no feature keywords match, output only the basic sample.

### Step 2 - Consent & Destination Gate (MUST ASK FIRST - STOP HERE UNTIL USER RESPONDS)

Before generating or writing any files, ask the user:

```
I'm ready to generate your Syncfusion Blazor Spreadsheet sample.

Where should I place it?
1) Create a new Razor file in the skill's "output" folder (recommended for quick tryout)
2) Add/modify an existing file in your project (please provide full path; choose append or replace)
Or, say "Just show me the code" to get the snippet here without changing files.
```

- **Do not proceed** until the user explicitly chooses an option or says "Just show me the code."
- If the user declines file changes, **only output the code in chat** (no files created/modified).

**For Option 2 only:** Before generating code, check if the project has the prerequisites from `references/basic-sample.md` (Prerequisites and Setup Requirements section). If missing, ask user consent and add them.

### Step 3 - Generate Code (Dry-Run)

Build the code in-memory by merging:
- Base: `references/basic-sample.md`
- Merge matched feature snippets into its anchors: `PROPERTY`, `EVENTS`, `BUTTON`, `API METHOD`
- Do **not** invent or modify any property, event, or API beyond what the reference files contain

### Step 4 - Apply Changes (Only After Consent)

**Option A - Create new file in skill's folder:**
- Create a `output/` folder at the skill's root and place the file there
- Default file name: `SpreadsheetSample.razor` (configurable via user reply)
- If the file already exists, ask:
  ```
  A file named "SpreadsheetSample.razor" already exists. Overwrite, append, or use a different name?
  ```
  Stop and wait for the user's decision before proceeding.
- Write the generated Razor/C# content.

**Option B - Add to user's project file:**
- Ask the user for the **full path** of the target file and whether to **append** or **replace**
- If the path does not exist, ask: `I can create this file. Proceed with creation?`
- If replacing, make a backup copy (e.g., `.bak`) before writing (if the environment supports it)
- Apply the change per the user's instruction.

### Step 5 - Report Outcome

```
Done
- Location: {path}
- Action: {created|overwritten|appended}
- Notes: {any warnings or unsupported features}
```

If file operations were skipped, confirm that the code was only displayed in chat.

---

## Code References

All templates and feature snippets live in `references/*.md`. Each file is a focused snippet the agent combines when generating samples.

**Flow:** Always start with `references/basic-sample.md`, then merge matched feature snippets into its anchors (`PROPERTY`, `EVENTS`, `BUTTON`, `API METHOD`). If no feature keywords match, return only the basic sample.

**Supported Features:** Editing, Filtering, Sorting, Cell merging, Insert row/column, Hyperlink, Clipboard actions, Cell formatting, open and save, selection

| File | Purpose |
|---|---|
| **basic-sample.md** | Minimal Spreadsheet sample with `byte[]` DataSource and Ribbon |
| **open-save.md** | Open (`byte[]`, local JSON, remote JSON URL, Google Drive) and Save via Ribbon |
| **worksheet.md** | Worksheet insert, delete, move, and duplicate |
| **cell-range.md** | Autofill & range actions |
| **merge.md** | Merge cells, unmerge cells |
| **editing.md** | Bind editing events, UpdateCell, set formula |
| **formula.md** | Formula bar toggle |
| **formatting.md** | Number formats and cell formatting |
| **contextmenu.md** | Toggle context menu |
| **rows-columns.md** | Insert rows/columns, resizing rows/columns |
| **filtering.md** | Column filtering |
| **sorting.md** | Multi-column sorting |
| **hyperlink.md** | Hyperlink add and remove |
| **clipboard.md** | Cut, Copy, Paste |
| **selection.md** | Cell and range selection |
| **undo-redo.md** | Undo and redo actions |
| **protection.md** | Sheet and workbook protection and unprotection |
| **custom-data.md** | Create a custom data spreadsheet using XlsIO |
| **common.md** | Height, Width, ID, CssClass, ActiveSheetIndex, Image |

---

## Rules

1. **Use Only Reference Snippets**
   - Generate code **exclusively from** the Markdown files under `references/`
   - **Do not invent/guess/include** any properties, events, API methods, component names, or parameters not present in `references/*.md`

2. **UI interaction feature**
   - The following features support **only UI interaction** and have **no customization options** (no methods, events, or properties):
   - **Gridlines** - Toggle visibility through UI only
   - **Clear** - Clear content through UI context menu only
   - **Named Ranges** - Create and manage through UI only
   - **Protection** - Protect/unprotect sheets through UI only
   - **Undo/Redo** - Undo and redo actions through UI only
   - **Images** - Image insert, resize and move actions through UI only
   - **Do not generate** methods, events, or properties for these features
   - **Only add** these features if they are explicitly mentioned in `references/*.md` files
   - If a user requests customization for these features, inform them:
     `This feature supports only UI interaction and does not have programmatic customization options in the Blazor Spreadsheet component.`

3. **Unsupported Feature Handling**
   - If the user requests a feature with no corresponding snippet in `references/*.md`, respond with:
     `That feature is not currently supported by the Syncfusion Blazor Spreadsheet component.`
   - Suggest the closest supported features **only if** they have snippets
   - **Explicitly list** unsupported items and **do not synthesize code** for them

4. **Validation Before Write**
   - Re-validate before writing that **all code blocks** originate from `references/*.md` files
   - If validation fails, stop and inform the user

5. **Transparency**
   - Include a **Supported vs. Unsupported** summary in the final report when applicable
   
---