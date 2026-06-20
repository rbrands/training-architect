## Sorting
Users sort an range ascending/descending in the spreadasheet editor.

### PROPERTY
```csharp
AllowSorting="true(Default)/false"
```

### BUTTON
<!-- Refer the below button for creating button and update the API public method calling. -->
<button @onclick="#MethodName">#Button Name</button>

### API METHOD
```csharp
/// <param name="selectedRange">
/// (Optional) The cell range to sort, in A1 format (e.g., "B2:C5").
/// If omitted, the currently selected range in the active sheet is used.
/// Sorting is performed using the first column of this range.
/// </param>
/// <param name="sortDirection">
/// (Optional) Specifies the sort direction.
/// Uses the SortDirection enum: Ascending or Descending.
/// Defaults to Ascending.
/// </param>

// Sorts the range B2:D5 in ascending order based on values in "Column B".
await SpreadsheetRef.SortRangeAsync("B2:D5", SortDirection.Ascending);
```

### Placeholders

| Placeholder | Description | Example |
|---|---|---|
| `#MethodName` | Name of the method calling when clicking the button | - |
| `#Button Name` | Provide a meaning full name to button which binds to API method | - |

### Notes
- **AllowSorting** is enabled by default; include `AllowSorting="false"` only when you want to **disable** sorting for the spreadsheet.
- There is no events available for sorting feature.
- **Important:** API methods should **NOT** be called inside `OnInitialized` or `OnParametersSet` lifecycle methods. Even if you call them, they will not work properly. Call API methods in response to user interactions (like button clicks) or in other appropriate lifecycle methods after the component is fully rendered.

### Documentation link
[Blazor Spreadsheet Sorting](https://help.syncfusion.com/document-processing/excel/spreadsheet/blazor/sorting)