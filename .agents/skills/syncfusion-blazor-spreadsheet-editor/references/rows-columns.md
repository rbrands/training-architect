## Rows & Columns

> perform operations like resizing row/column, inserting row/column in spreadsheet editor

### PROPERTIES
```csharp
RowCount="1000(Default)"
ColumnCount="200(Default)"
AllowResizing="true(Default)/false"
```

### EVENTS
```csharp
RowResizing="OnRowResizing"
RowResized="OnRowResized"
ColumnResized="OnColumnResized"
ColumnResizing="OnColumnResizng"
```
```csharp
private void OnRowResizing(RowResizingEventArgs args)
{
    // customize your code in based on the requirements and check below table for event argument details
}
```
**RowResizing Event Arguments**
| Event Arguments | Description |
|---|---|
| `Cancel` | Set to `true` to cancel the row resizing operation. |
| `RowHeight (read-only)` | A `double` representing the height of the resized row. |
| `RowIndex (read-only)` | An `int` representing the zero-based index of the resized row. For example, 0 represents the first row, 1 represents the second row, and so on.|

```csharp
private void OnRowResized(RowResizedEventArgs args)
{
    // customize your code in based on the requirements and check below table for event argument details
}
```

**RowResized Event Arguments**
| Event Arguments | Description |
|---|---|
| `RowHeight (read-only)` | A `double` representing the height of the resized row. |
| `RowIndex (read-only)` | An `int` representing the zero-based index of the resized row. For example, 0 represents the first row, 1 represents the second row, and so on.|

```csharp
private void OnColumnResizng(ColumnResizingEventArgs args)
{
    
}
```
**ColumnResizing Event Arguments**
| Event Arguments | Description |
|---|---|
| `Cancel` | Set to `true` to cancel the column resizing operation. |
| `ColumnIndex (read-only)` | An `int` representing the zero-based index of the resizing column. For example, 0 represents column A, 1 represents column B, and so on. |
| `ColumnName (read-only)` | A `string` representing the name of the resizing column.|
| `ColumnWidth (read-only)` | A `double` representing the width of the resizing column.|

```csharp
private void OnColumnResized(ColumnResizedEventArgs args)
{

}
```
**ColumnResized Event Arguments**
| Event Arguments | Description |
|---|---|
| `ColumnIndex (read-only)` | An `int` representing the zero-based index of the resized column. For example, 0 represents column A, 1 represents column B, and so on. |
| `ColumnName (read-only)` | A `string` representing the name of the resized column.|
| `ColumnWidth (read-only)` | A `double` representing the width of the resized column.|

### BUTTON
<!-- Refer the below button for creating button and update the API public method calling. -->
<button @onclick="#MethodName">#Button Name</button>

### API METHOD
```csharp
// Insert 2 rows above row index 0 in the active sheet
await SpreadsheetInstance.InsertRowAsync(ROWINDEX, COUNT, SHEET, ROWPOSITION);

// Insert 2 columns to the right of column index 2
await SpreadsheetInstance.InsertColumnAsync(COLUMNINDEX, COUNT, SHEET, COLUMNPOSITION);
```

### Placeholders

| Placeholder | Description | Example |
|---|---|---|
| `#MethodName` | Name of the method calling when clicking the button | - |
| `#Button Name` | Provide a meaning full name to button which binds to API method | - |
| `ROWINDEX` | The zero-based index where the rows should be inserted. If position is Below, rows will be inserted after this index. If position is Above, rows will be inserted at this index. |`2`, `5`|
| `COUNT` | The number of rows OR COLUMNS to insert. Must be greater than zero. |`1`, `5`|
| `SHEET` | The target worksheet where rows will be inserted. Accepts either a sheet index (integer) or sheet name (string). sheet name is case-insensitive. If null, the active sheet will be used. When using an index, note that it is zero-based (i.e., the first sheet is index 0). | `null`, `Sheet2`, `3`|
| `ROWPOSITION` |Specifies the position relative to rowIndex where rows should be inserted. Valid values are Above or Below. The default value is Above. |`RowPosition.Above`, `RowPosition.Below`|
| `COLUMNINDEX` | The zero-based index where the columns should be inserted. If position is Right, columns will be inserted after this index. If position is Left, columns will be inserted at this index. |`2`, `5`|
| `COLUMNPOSITION` | Specifies the position relative to columnIndex where columns should be inserted. Valid values are Right or Left. The default value is Right. |`ColumnPosition.Left`, `ColumnPosition.Right`|

### Notes
- **RowCount** default value is 1000; you can change the number of row count in the spreadsheet.
- **ColumnCount** default value is 200; you can change the number of column count in the spreadsheet.
- **AllowResizing** is enabled by default; include `AllowResizing="false"` only when you want to **disable** row and column resizing for the spreadsheet.
- **Hiding rows and columns is not supported** in the Blazor Spreadsheet component (neither through UI interactions nor through API methods or events).
- **Important:** API methods should **NOT** be called inside `OnInitialized` or `OnParametersSet` lifecycle methods. Even if you call them, they will not work properly. Call API methods in response to user interactions (like button clicks) or in other appropriate lifecycle methods after the component is fully rendered.

### Documentation link
[Blazor Spreadsheet rows and columns](https://help.syncfusion.com/document-processing/excel/spreadsheet/blazor/rows-and-columns)
