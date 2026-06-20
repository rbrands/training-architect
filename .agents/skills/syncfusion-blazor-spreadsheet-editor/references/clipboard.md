## Clipboard
> Perform clipboard actions like Cut, Copy and paste in the Spreadsheet Editor.

### PROPERTY
```csharp
EnableClipboard="true(default)/false"
``` 

### EVENTS

```csharp
CutCopyActionBegin="OnCutCopyActionBegin"
Pasting="OnPasting"
```

```csharp
public void OnCutCopyActionBegin(CutCopyActionBeginEventArgs args)
{
    // customize your code in based on the requirements and check below table for event argument details
}
```
**CutCopyActionBegin Event Arguments**
| Event Arguments | Description |
|---|---|
| `ClipboardAction` | Specifies the type of clipboard operation in progress. Returns a value from the ClipboardAction enumeration, such as ClipboardAction.Cut or ClipboardAction.Copy |
| `CopiedRange` | Represents the full address of the cell range involved in the clipboard operation. Includes the worksheet name and range in A1 notation (e.g., “Sheet1!A1:B5”). |
| `Cancel` | Set to `true` to cancel cut or copy action from proceeding.. |

```csharp
public void OnPasting(PastingEventArgs args)
{
    // customize your code in based on the requirements and check below table for event argument details
}
```
**Pasting Event Arguments**
| Event Arguments | Description |
|---|---|
| `ExternalClipboardData` | An array of strings containing raw text data from external sources (like Excel or Google Sheets), with each element representing a row of data. Set to null when copying from within the workbook. |
| `CopiedRange` | A string in the format “SheetName!Range” (e.g., “Sheet1!A1:A10”) representing the source location of the copied or cut content. Set to null when pasting external content. |
| `PasteRange` | A string in the format “SheetName!Range” specifying the target cell range where content will be pasted. |
| `Cancel` | Set to `true` to cancel paste action from proceeding.. |

### BUTTON
<!-- Refer the below button for creating button and update the API public method calling. -->
<button @onclick="#MethodName">#Button Name</button>

### API METHODS
```csharp
// Cuts content from the currently active cell or selected range in the active worksheet.
await _spreadsheetRef.CutCellAsync(CELLADDRESS);

// Copies content from the currently active cell or selected range in the active worksheet.
await _spreadsheetRef.CopyCellAsync(CELLADDRESS);

// Pastes the content into the currently active cell or range.
await _spreadsheetRef.PasteCellAsync(CELLADDRESS);
```

### Placeholders

| Placeholder | Description | Example |
|---|---|---|
| `#MethodName` | Name of the method calling when clicking the button | - |
| `#Button Name` | Provide a meaning full name to button which binds to API method | - |
| `CELLADDRESS` | This is a optional string parameter which Specifies the target cell or range of cells for cut, copy and pasting clipboard content. Accepts either a single cell reference (for example, "A1") or a range of cells (for example, "A1:B5") from the active worksheet. For column or row selection, use full range like "A1:A1000" instead of "C:C" or "1:1". |`"F3:F9"`, `"A5"`|

### Notes
- **EnableClipboard** is enabled by default; include `EnableClipboard="false"` only when you want to **disable** cut, copy and paste for the sheet.
- **OnCutCopyActionBegin** fires *before* a cell or range of value is cut or copy and can be canceled via `args.Cancel = true`.
- **OnPasting** fires *before* a cell or range of value is paste and can be canceled via `args.Cancel = true`.
- **Important:** API methods should **NOT** be called inside `OnInitialized` or `OnParametersSet` lifecycle methods. Even if you call them, they will not work properly. Call API methods in response to user interactions (like button clicks) or in other appropriate lifecycle methods after the component is fully rendered.

### Documentation link
[Blazor Spreadsheet Clipboard](https://help.syncfusion.com/document-processing/excel/spreadsheet/blazor/clipboard)