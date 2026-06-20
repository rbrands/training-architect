## Undo and Redo

> Reverse or reapply recent actions in the spreadsheet. Undo and Redo operations maintain a history of spreadsheet actions, allowing users to safely experiment with data and formatting while preserving the ability to restore previous states.

### Undo

> Reverse the most recent action performed within the spreadsheet, restoring the previous state and enabling safe modifications to content and formatting. End users can perform the undo operation through the user interface (UI) without requiring any programmatic customization.

### Redo

> Reapply an action that was previously undone, allowing end users to move forward through the operation history and restore both data and interface states. Redo actions can be performed via the user interface (UI) without requiring any programmatic customization.

### UI-only Operations

These operations are available only through the user interface. There are no public APIs or events to trigger or customize these operations programmatically.

**Summary**
Undo and Redo are UI-only actions. No public API or event is provided to trigger, intercept, or automate these operations.

**How to Perform**

**Undo:**
- Click the **Undo** button located in the **Home** tab of the **Ribbon** to reverse the latest operation.
- Use the keyboard shortcut **Ctrl + Z** for a quick way to undo the last action.
- The **Undo** button is automatically disabled when there are no reversible operations available.

**Redo:**
- Click the **Redo** button located in the **Home** tab of the **Ribbon** to reapply the most recently undone operation.
- Use the keyboard shortcut **Ctrl + Y** for quick access to redo the last undone action.
- The **Redo** button is automatically disabled when no actions are available to reapply or when a cell is in edit mode.

### Keyboard Shortcuts

| Shortcut | Action |
|---|---|
| **Ctrl + Z** | Undo the most recent action |
| **Ctrl + Y** | Redo the most recently undone action |

### Limitations

- No public API or event to trigger, intercept, or customize these actions.
- Cannot be automated or performed programmatically.
- The undo and redo history is limited to **25 operations** to optimize memory usage; once this limit is reached, older actions are automatically discarded.
- The history is cleared when worksheet protection is enabled.
- The redo history is cleared whenever a new action is performed after an undo operation.

### Documentation link
[Blazor Spreadsheet Undo and Redo](https://help.syncfusion.com/document-processing/excel/spreadsheet/blazor/undo-redo)
