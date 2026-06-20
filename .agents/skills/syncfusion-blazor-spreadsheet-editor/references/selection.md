## Selection

> Select cells, rows, columns, or complete records for data analysis and operations. Supports mouse interaction, keyboard navigation, and programmatic selection.

### SELECTION TYPES

The spreadsheet supports multiple selection modes:

- **Cell Selection**: Select individual cells or range of cells for data manipulation
- **Range Selection**: Select non-adjacent cells or ranges using Ctrl+Click
- **Row Selection**: Select entire rows for row-based operations
- **Column Selection**: Select entire columns for column-based operations

### EVENTS
```csharp
Selected="OnSelected"
```

```csharp
private void OnSelected(SelectedEventArgs e) 
{ 
    // customize your code in based on the requirements and check below table for event argument details
}
```
**Selected Event Arguments**
| Event Arguments | Description |
|---|---|
| `Range` | A string representing the address of the selected cells |

### BUTTON
<!-- Refer the below button for creating button and update the API public method calling. -->
<button @onclick="#MethodName">#Button Name</button>

### API METHOD
```csharp
await SpreadsheetRef.SelectRangeAsync(CELLADDRESS);
```

### ACCESSING SELECTION VIA UI

**Using Mouse Interaction**
- Click a cell to select it
- Click and drag to select a range of cells
- Click a row or column header to select the entire row or column

**Using Keyboard Navigation**
- Use **Arrow** keys to navigate between cells
- Use **Shift + Arrow** keys for range selection
- Use **Ctrl + Click** for non-adjacent selections

**Using Name Box**
- Enter a cell reference (e.g., `C5`) or a range (`A1:E5`)
- Press **Enter** key to select the specified range

**Selecting Non-Adjacent Ranges**
- Select the first cell or range
- Hold **Ctrl** and click additional cells or drag to select additional ranges
- Each selected range is highlighted independently
- The **Name Box** displays the first selected cell reference

**Selecting Rows**
- Click the first row header, then drag to the last desired row header for adjacent rows
- Click the first row header, then hold **Shift** and click the last row header for continuous selection
- Hold **Ctrl** and click individual row headers for non-adjacent rows

**Selecting Columns**
- Click the first column header, then drag to the last desired column header for adjacent columns
- Click the first column header, then hold **Shift** and click the last column header for continuous selection
- Hold **Ctrl** and click individual column headers for non-adjacent columns

### Placeholders

| Placeholder | Description | Example |
|---|---|---|
| `#MethodName` | Name of the method calling when clicking the button | - |
| `#Button Name` | Provide a meaning full name to button which binds to API method | - |
| `CELLADDRESS` | The address of the range to select. The format can be a single range (e.g., "A1:B5") or multiple ranges separated by spaces (e.g., "A1:A10 B1:B10 C1:C10"). If null or empty string is provided, no cells will be selected. |`"A1:C3"`, `"A1:A10 B1:B10 C1:C10"`, `null`|

### Notes
- There is no property for this selection.
- **Important:** API methods should **NOT** be called inside `OnInitialized` or `OnParametersSet` lifecycle methods. Even if you call them, they will not work properly. Call API methods in response to user interactions (like button clicks) or in other appropriate lifecycle methods after the component is fully rendered.

### Documentation link
[Blazor Spreadsheet Selection](https://help.syncfusion.com/document-processing/excel/spreadsheet/blazor/selection)