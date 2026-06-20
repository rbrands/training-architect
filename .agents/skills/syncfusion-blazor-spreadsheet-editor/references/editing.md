## Editing
> Edit cells, handle editing events, and programmatically update cell values in the Spreadsheet Editor.

### PROPERTY
```csharp
AllowEditing="true(Default)/false"
```

### EVENTS
```csharp
CellEditing="OnCellEditing"
CellSaved="OnCellSaved"
```

```csharp
// Event handlers to place inside the @code section
private void OnCellEditing(CellEditingEventArgs args)
{
    // customize your code in based on the requirements and check below table for event argument details
}
```
**CellEditing Event Arguments**
| Event Arguments | Description |
|---|---|
| `RowIndex (read-only)` | The zero-based row index of the cell being edited. |
| `ColumnIndex (read-only)` | The zero-based column index of the cell being edited. |
| `Address (read-only)` | 	The address of the cell being edited (e.g., “Sheet1!A1”).|
| `Value (read-only)` | The current value of the cell before editing. |
| `Cancel` | Set to `true` to cancel the editing operation. |

```csharp
private void OnCellSaved(CellSavedEventArgs args)
{
    // customize your code in based on the requirements and check below table for event arguments details
}
```
**CellSaved Event Arguments**
| Event Arguments | Description |
|---|---|
| `Address (read-only)` | The address of the cell whose value was saved (e.g., “Sheet1!A1”). |
| `Value (read-only)` | The new value of the cell after saving. |
| `OldValue (read-only)` | 	The original value of the cell before saving. |
| `Action (read-only)` | The action that triggered the save (e.g., “Edit”, “Cut”, “Paste”, “Autofill”). |

### BUTTON
<!-- Refer the below button for creating button and update the API public method calling. -->
<button @onclick="#MethodName">#Button Name</button>

### API METHODS
```csharp
// Updates a single cell with a specific value.
await SpreadsheetRef.UpdateCellAsync(CELLADDRESS, CELLVALUE);

// Updates multiple cells in a batch operation using CellUpdateItem objects.
var updates = new List<CellUpdateItem>
{
    new CellUpdateItem { CellAddress = "Sheet1!A1", Value = "Header" },
    new CellUpdateItem { CellAddress = "Sheet1!B1", Value = "Value" },
    new CellUpdateItem { CellAddress = "Sheet1!A2", Value = 100 },
    new CellUpdateItem { CellAddress = "Sheet1!B2", Value = 200 },
    new CellUpdateItem { CellAddress = "Sheet1!C1", Value = "Total" },
    new CellUpdateItem { CellAddress = "Sheet1!C2", Value = "=SUM(A2:B2)" },
    new CellUpdateItem { CellAddress = "Sheet1!A1:A10", Value = "Batch Fill" }  // Supports ranges
};

await SpreadsheetRef.UpdateCellsAsync(updates);
```

## Placeholders

| Placeholder | Description | Example |
|---|---|---|
| `#MethodName` | Name of the method calling when clicking the button | - |
| `#Button Name` | Provide a meaning full name to button which binds to API method | - |
| `CELLADDRESS` | Specifies the address of the cell to update. For column or row selection, use full range like "A1:A1000" instead of "C:C" or "1:1". |`"Sheet1!A1"`, `"Sheet1!B3"`|
| `CELLVALUE` | Defines the new value to assign to the cell. Supported types include strings, numbers, booleans, and formulas |`"Tablet"`, `799`, `=SUM(A3:B3)`|

## Notes
- **AllowEditing** is enabled by default; include `AllowEditing="false"` only when you want to **disable** editing for the sheet.
- **CellEditing** fires *before* a cell enters edit mode and can be canceled via `args.Cancel = true`.
- **CellSaved** fires *after* a cell's value is committed, providing `Address`, `Value`, `OldValue`, and `Action`.
- **Important:** API methods should **NOT** be called inside `OnInitialized` or `OnParametersSet` lifecycle methods. Even if you call them, they will not work properly. Call API methods in response to user interactions (like button clicks) or in other appropriate lifecycle methods after the component is fully rendered.

### Documentation link
[Blazor Spreadsheet Editing](https://help.syncfusion.com/document-processing/excel/spreadsheet/blazor/editing)