# Syncfusion<sup>®</sup> Blazor Spreadsheet Editor Skill

## Overview

Integrate and configure the Syncfusion Blazor Spreadsheet — an interactive Excel-like UI — into Blazor projects.
This skill generates minimal, copy-pasteable C# and Razor code that you can drop directly into your Blazor application.

See **[SKILL.md](SKILL.md)** for the full intent-routing guide and rules.

---

## Key Capabilities

- **Data & Binding:** Load from `byte[]`, local JSON file, remote JSON URL, or Google Drive; save via Ribbon or programmatically
- **Cell & Range Operations:** Editing with events, UpdateCell, set formulas, autofill, range actions, merge/unmerge cells
- **Cell Formatting:** Background colors, fonts, alignment, text decoration, number formats, borders
- **Rows, Columns & Worksheets:** Insert/resize rows and columns; insert, delete, move, and duplicate worksheets
- **Data Operations:** Filtering, sorting, hyperlink add/edit/remove, cut/copy/paste, cell and range selection
- **UI & Interactivity:** Show/hide Formula bar, show/hide Context Menu, custom data rendering via XlsIO
- **Protection & History:** Sheet and workbook protection/unprotection, undo/redo operations
- **Images:** Insert, resize, and move images through UI (enabled by default)

---

## Getting Started

### How to Integrate Skills

**Step 1: Checkout and copy the required skills**

Clone or download the Spreadsheet-Editor-SDK-Skills repository and copy the **blazor-spreadsheet-editor** skill from the `skills/` directory.

**Step 2: Install the skill**

Place the copied skill folders in your workspace following this structure:

```
your-workspace/
├── .github/skills/          # or .claude/skills/ or .codestudio/skills/
│   └── blazor-spreadsheet-editor/
│       └── SKILL.md
├── SfSpreadsheet/              # Your Blazor project folder
│   ├── Pages/
│   ├── Program.cs
│   └── ...
└── SfSpreadsheet.slnx          # Solution file
```

**Step 3: Verify and manage your skills**

Type `/skills` in the GitHub Copilot or Code Studio chat to quickly access the Configure Skills menu and manage your installed skills.

**Step 4: Use skills in VS Code**

There are two ways to use skills:

1. **Slash commands** - Type `/` in the GitHub Copilot chat to see available skills. For example:
   ```
   /blazor-spreadsheet-editor Create a spreadsheet with editing disabled
   ```

2. **Automatic loading** - Simply describe your task naturally, and your AI Agent automatically loads the relevant skill:
   ```
   Create a spreadsheet without the ribbon
   ```

When a skill is loaded, AI Agent gains specialized knowledge of Syncfusion Blazor Spreadsheet and can help you generate code for your Blazor project efficiently.

### Prerequisites

### Runnable Blazor project (Server or WebAssembly)

To integrate the Syncfusion Blazor Spreadsheet component directly into your project files, you need a working Blazor project. If you don't have one yet, follow the [Getting Started guide](https://help.syncfusion.com/document-processing/excel/spreadsheet/blazor/getting-started-webapp) to set up a new Blazor project.

**Alternative Options:**
- **No project needed:** You can request code snippets directly in the chat window for learning or reference purposes
- **Separate file generation:** Code can be saved to the skill's output folder (`syncfusion-blazor-spreadsheet-editor/output/`) as standalone files

## Example Prompts

*Use these when you want C# and Razor code snippets for your Blazor project.*

- "Add a basic Blazor Spreadsheet component to my page"
- "Show me how to load a local Excel file using filepath into the Spreadsheet"
- "Generate code to load JSON data from a URL into the Spreadsheet"
- "Add cell editing events and update cells programmatically"
- "How do I add filtering and sorting to the Spreadsheet?"
- "Show me how to insert rows, columns, and manage worksheets"
- "Generate code to handle the BeforeSave event"

---

## Troubleshooting

| Issue | Solution |
|-------|----------|
| License Watermark | Add key to `SyncfusionLicense.txt` or use env var `SYNCFUSION_LICENSE_KEY` |
| Missing NuGet package | `dotnet add package Syncfusion.Blazor.Spreadsheet`, `dotnet add package Syncfusion.Blazor.Themes` |
| File access error | Ensure the Excel file path is correct and accessible |
| Google Drive error | Verify service account credentials and file ID |
| Component not rendering | Ensure Syncfusion script and stylesheet are referenced in your layout |

---

## Resources

- [Syncfusion Blazor Spreadsheet Documentation](https://help.syncfusion.com/document-processing/excel/spreadsheet/blazor/overview)
- [API Reference](https://help.syncfusion.com/cr/blazor/Syncfusion.Blazor.Spreadsheet.html)
- [Demo & Examples](https://document.syncfusion.com/demos/spreadsheet-editor/blazor-server/spreadsheet/overview)

---

## License

Syncfusion Blazor Spreadsheet requires a commercial license for production use. A [free community license](https://www.syncfusion.com/products/communitylicense) is available for qualifying organizations.
