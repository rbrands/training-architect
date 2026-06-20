## Filtering
Filtering is Excel-like and **enabled by default** via `AllowFiltering`. Users interact using the Ribbon or filter icons. [14](https://help.syncfusion.com/document-processing/excel/spreadsheet/blazor/filtering)

### PROPERTY
```csharp
AllowFiltering="true(Default)/false"
```

### BUTTON
<!-- Refer the below button for creating button and update the API public method calling. -->
<button @onclick="#MethodName">#Button Name</button>

### API METHODS
```csharp
 // This method applies a filter to Column A showing only rows containing value.
await SpreadsheetRef.FilterByCellValueAsync(CELLADDRESS, CELLVALUE);

// This command removes all filtering from Column A (represented by index 0).
await SpreadsheetRef.ClearFilterAsync(COLUMNINDEX);

// Clears all filters applied to the active sheet.
await SpreadsheetRef.ClearAllFiltersAsync();

// Reapplies all active filters to include the updated data.
await SpreadsheetRef.ReapplyFiltersAsync();
```
### Placeholders

| Placeholder | Description | Example |
|---|---|---|
| `#MethodName` | Name of the method calling when clicking the button | - |
| `#Button Name` | Provide a meaning full name to button which binds to API method | - |
| `CELLADDRESS` | specifies the address of the cell that contains the filter criteria. This determines the column to which the filter is applied. For example, "A1" applies the filter to Column A. Using this parameter also updates the used range of the Spreadsheet. For column or row selection, use full range like "A1:A1000" instead of "C:C" or "1:1". |`"A1"`|
| `CELLVALUE` | Defines the value to filter by. This can be a string, number, date, or boolean. The filter displays only the rows where the cell in the specified column matches this value. For example, “New York” displays rows where Column A contains “New York”. Providing an incorrect value results in inaccurate filtered output.
 |`"New York"`|
 | `COLUMNINDEX` | The zero-based index of the column whose filter is to be cleared. For example, “0” refers to the first column (Column A).| 0 |

### Notes
- **AllowFiltering** is enabled by default; include `AllowFiltering="false"` only when you want to **disable** filtering for spreadsheet.
- There is no events available for filtering feature.
- **Important:** API methods should **NOT** be called inside `OnInitialized` or `OnParametersSet` lifecycle methods. Even if you call them, they will not work properly. Call API methods in response to user interactions (like button clicks) or in other appropriate lifecycle methods after the component is fully rendered.

### Documentation link
[Blazor Spreadsheet Filtering](https://help.syncfusion.com/document-processing/excel/spreadsheet/blazor/filtering)