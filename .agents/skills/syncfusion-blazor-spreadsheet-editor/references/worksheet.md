## Worksheets

> Users can insert, move, delete and duplicate the worksheet. Many sheet ops are disabled under protection.

### EVENTS
```csharp
WorksheetAdding="OnWorksheetAdding"
```

```csharp
public void OnWorksheetAdding(WorksheetAddingEventArgs args)
{
    // customize your code in based on the requirements and check below table for event argument details
}
```
**WorksheetAdding Event Arguments**
| Event Arguments | Description |
|---|---|
| `Name` | The name of the new worksheet to be added. You can modify this value to change the sheet name. |
| `Index` | The zero-based index position where the new worksheet will be inserted. You can modify this value to change the insertion position. |
| `Cancel` | Set to `true` to cancel the worksheet addition operation. |

### BUTTON
<!-- Refer the below button for creating button and update the API public method calling. -->
<button @onclick="#MethodName">#Button Name</button>

### API METHODS

```csharp
// Insert one or more sheets at specific index.
await SpreadsheetRef.InsertSheetAsync(INDEX, COUNT);

// Insert sheet with custom name at specific index.
await SpreadsheetRef.InsertSheetAsync(INDEX, SHEETNAME);

// Get the active sheet snapshot
var active = SpreadsheetRef.GetActiveWorksheet();

// Get the cellData snapshot
var data = SpreadsheetRef.GetData(CELLADDRESS);

// Remove sheet at specific index.
await SpreadsheetRef.DeleteSheetAsync(DELETEINDEX);

// Remove sheet by name.
await SpreadsheetRef.DeleteSheetAsync(SHEETNAME);

// Move sheet from one index to another index.
await SpreadsheetRef.MoveSheetAsync(SOURCEINDEX, DESTINATIONINDEX);

// Move sheet by names (source sheet name to destination sheet name).
await SpreadsheetRef.MoveSheetAsync(SOURCESHEETNAME, DESTINATIONSHEETNAME);

// Duplicate the sheet at specific index.
await SpreadsheetRef.DuplicateSheetAsync(DUPLICATEINDEX);

// Duplicate sheet by name.
await SpreadsheetRef.DuplicateSheetAsync(SHEETNAME);
```

### UI-only Operations

These operations are available only through the user interface. There are no public APIs, events, or programmatic customization points.

**Summary**
Hide, Unhide, and Rename are UI-only actions. No public API or event is provided to trigger, intercept, or automate these operations.

**How to Perform**

- **Hide:** Right-click the sheet tab and select **Hide** from the context menu. Hidden sheets remain in the workbook but are not visible in the UI.

- **Unhide:** Click on the sheet tab list icon, then select the hidden sheet from the list. The sheet will reappear in the sheet tab collection and become available for editing.

- **Rename:** Right-click the sheet tab and select **Rename** from the context menu, then enter the new name and click **Update**. 

**Limitations**

- No public API or event to trigger, intercept, or customize these actions.
- Cannot be automated or performed programmatically.
- **Hide** is available only if the workbook has more than one visible sheet, ensuring at least one sheet remains visible.
- These actions may be disabled when the workbook is protected.

### Placeholders

| Placeholder | Description | Example |
|---|---|---|
| `#MethodName` | Name of the method calling when clicking the button | - |
| `#Button Name` | Provide a meaning full name to button which binds to API method | - |
| `INDEX` | This is a optional parameter. The zero-based index where the sheets will be inserted. If not specified, sheets are added based on active sheet index. If the specified index is invalid (e.g., negative or beyond the workbook’s sheet count), no action occurs. |`1`, `3`|
| `COUNT` | The number of sheets to add. Defaults to 1 if not specified. |`2`, `4`|
| `CELLADDRESS` | Specifies the cell or range to read. Supports addresses such as “A1”, “A2:B5”, or “Sheet1!A2:B5”. If omitted or invalid, the return value is null. |`"A1"`, `"A2:B5"`, or `"Sheet1!A2:B5"`|
| `DELETEINDEX` | The zero-based index of the sheet to delete. If no index is provided, the active sheet is deleted. If the index is invalid (e.g., negative or beyond the workbook’s sheet count) or the workbook has only one sheet, no action occurs. |`2`, `4`|
| `SHEETNAME` | The name of the sheet to operate on (for insert with name, delete, or duplicate by name operations). |`"Sheet1"`, `"Data"`, `"Report"`|
| `SOURCEINDEX` | The zero-based index of the sheet to move. If invalid (e.g., negative or beyond sheet count), no action occurs. |`5`, `2`|
| `DESTINATIONINDEX` | The zero-based index where the sheet will be moved. If invalid, no action occurs |`0`, `3`|
| `SOURCESHEETNAME` | The name of the source sheet to move. |`"Sheet1"`, `"Data"`|
| `DESTINATIONSHEETNAME` | The name of the destination sheet (used to position the move after this sheet). |`"Sheet3"`, `"Report"`|
| `DUPLICATEINDEX` | The zero-based index of the sheet to duplicate. If no index is provided, the active sheet is duplicated. If the index is invalid (e.g., negative or beyond sheet count), no action occurs. |`0`|

### Notes
- **WorksheetAdding** fires *before* a new worksheet is added and can be canceled via `args.Cancel = true`.
- You can modify the sheet name and index in the **WorksheetAdding** event before the sheet is created.
- **Important:** API methods should **NOT** be called inside `OnInitialized` or `OnParametersSet` lifecycle methods. Even if you call them, they will not work properly. Call API methods in response to user interactions (like button clicks) or in other appropriate lifecycle methods after the component is fully rendered.

### Documentation link
[Blazor Spreadsheet Worksheet](https://help.syncfusion.com/document-processing/excel/spreadsheet/blazor/worksheet)