# Syncfusion<sup>®</sup> Blazor DOCX Editor Skill

## Overview

Build MS Word–like document editor applications in Blazor using the Syncfusion DOCX Editor. This skill provides AI-guided code generation and integration support for the Blazor document editor component, enabling editing, formatting, import/export capabilities, and more.

See **[SKILL.md](SKILL.md)** for the full intent-routing guide and rules.

---

## Key Capabilities

- **Document Editing:** Create, edit, and view Word documents.
- **Protection:** Apply read-only mode, allow comments, allow form filling, enforce tracked changes,and define editable ranges.
- **Review:** Enable track changes (revisions) and accept/reject changes.
- **Comments:** Add comments, replies, and author metadata.
- **Find and replace:** Perform programmatic find and replace operations for automation.
- **Form Fields:** Insert and fill text fields, check boxes, and dropdown form fields.

---

## Getting Started

### How to Integrate Skills

**Step 1: Checkout and copy the required skills**

Clone or download the DOCX-Editor-SDK-Skills repository and copy the **syncfusion-blazor-docx-editor** skill from the `skills/` directory.

**Step 2: Install the skill**

Place the copied skill folder in your workspace following this structure:

```
your-workspace/
├── .github/skills/          # or .claude/skills/ or .codestudio/skills/
│   └── syncfusion-blazor-docx-editor/
│       └── SKILL.md
├── SfDocumenteditor/              # Your Blazor project folder
│   ├── Pages/
│   ├── Program.cs
│   └── ...
└── SfDocumenteditor.slnx          # Solution file
```

**Step 3: Verify and manage your skills**

Type `/skills` in the GitHub Copilot or Code Studio chat to quickly access the Configure Skills menu and manage your installed skills.

**Step 4: Use skills in VS Code**

There are two ways to use skills:

1. **Slash commands** - Type `/` in the GitHub Copilot chat to see available skills. For example:
   ```
   /syncfusion-blazor-docx-editor Create a Document Editor with built-in toolbar options
   ```

2. **Automatic loading** - Simply describe your task naturally, and your AI Agent automatically loads the relevant skill:
   ```
   Create a Blazor document editor with a built-in toolbar that allows users to open, edit, and save documents
   ```

When a skill is loaded, AI Agent gains specialized knowledge of the Syncfusion Blazor DOCX Editor and can help you generate code snippets or build complete document editor features.

---

## Prerequisites

### Runnable Blazor project (Server or WebAssembly)

To integrate the Syncfusion Blazor DOCX Editor component directly into your project files, you need a working Blazor project. If you don't have one yet, follow the [Getting Started guide](https://help.syncfusion.com/document-processing/word/word-processor/blazor/getting-started/web-app) to set up a new Blazor project.

**Alternative Options:**
- **No project needed:** You can request code snippets directly in the chat window for learning or reference purposes
- **Separate file generation:** Code can be saved to the skill's output folder (`syncfusion-blazor-docx-editor/output/`) as standalone files

---

## Example Prompts

- "Show me code to create a Document Editor component"
- "Create a Document Editor with Read‑Only restriction mode"

---

## Troubleshooting

| Issue | Solution |
|-------|----------|
| **License watermark appears** | Verify that your license key is properly configured. See [Licensing](https://help.syncfusion.com/document-processing/licensing/overview). |
| **Style/Theme not applied** | Import CSS files in correct order. Ensure theme CSS matches desired theme name. |

---

## Resources

- [Documentation](https://help.syncfusion.com/document-processing/word/word-processor/blazor/overview)
- [API Reference](https://help.syncfusion.com/cr/blazor/syncfusion.blazor.documenteditor.sfdocumenteditorcontainer.html)
- [Online Demo](https://document.syncfusion.com/demos/docx-editor/blazor-server/document-editor/default-functionalities)

---

## License

Syncfusion Blazor DOCX Editor SDK require a [commercial license](https://www.syncfusion.com/docx-editor-sdk/blazor-docx-editor) for production use. A [free Community License](https://www.syncfusion.com/products/communitylicense) is also available for individual developers and small businesses.
