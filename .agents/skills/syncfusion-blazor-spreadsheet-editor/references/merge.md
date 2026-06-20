## Merge and Unmerge cells

> Merge cells into a single larger cell and unmerge them back in the Spreadsheet Editor.

### PROPERTY
```csharp
AllowMerge="true(Default)/false"
```

### MERGE TYPES

The Spreadsheet supports the following merge operations:

| Merge Type | Description |
|---|---|
| **Cells** | Combines all selected cells into one single cell. The value from the top-left cell is kept. |
| **Center** | Combines all selected cells into one single cell and centers the content horizontally. The value from the top-left cell is kept. |
| **Across** | Merges cells row by row across columns in the selection. Each row keeps its first cell value. |

### BUTTON
<!-- Refer the below button for creating button and update the API public method calling. -->
<button @onclick="#MethodName">#Button Name</button>

### API METHOD
```csharp
// Merge the current selection into a single cell and method contains two parameters: MergeType and Cell range(Optional)
await _spreadsheetRef.MergeAsync(#MERGETYPE, "#CELLRANGE");

// Unmerge the specified merged region. Cell range is optional if it is not specified it will merge the current selected range.
await _spreadsheetRef.UnmergeAsync("#CELLRANGE");
```

### Placeholders

| Placeholder | Description | Example |
|---|---|---|
| `#MethodName` | Name of the method calling when clicking the button | `MergeCells`, `UnmergeCells` |
| `#Button Name` | Provide a meaningful name to button which binds to API method | `Merge`, `Unmerge` |
| `#CELLRANGE` | Cell range which should be merged/unmerged. |`"A1:D4"`, `"D5:G10"`|
| `MergeType` | Type of merge to perform |`MergeType.Center`, `MergeType.Cells`, `MergeType.Across`|

### LIMITATIONS

When merging cells in the Spreadsheet, the following constraints apply:

- **Sorting with merged cells** - Sorting a range containing merged cells requires all merged cells to be consistent in size. A validation dialog will appear if this condition is not met.

- **Autofill on merged cells** - When performing autofill on merged cells, all merged cells in the range must be the same size. A validation dialog will appear otherwise.

- **Protected sheets** - Merge operations are disabled when the worksheet is protected.

- **Single cell selection** - The Merge Cell button is disabled when a single unmerged cell is selected.

### Notes
- **AllowMerge** is enabled by default; include `AllowMerge="false"` only when you want to **disable** merge and unmerge for the sheet.
- When `AllowMerge` is set to false, merge options are disabled in the Ribbon and API methods related to merging will be inactive.
- There are no events available for merge and unmerge feature.
- **Important:** API methods should **NOT** be called inside `OnInitialized` or `OnParametersSet` lifecycle methods. Even if you call them, they will not work properly. Call API methods in response to user interactions (like button clicks) or in other appropriate lifecycle methods after the component is fully rendered.

### Documentation link
[Blazor Spreadsheet Merge Cell](https://help.syncfusion.com/document-processing/excel/spreadsheet/blazor/merge-cell)

