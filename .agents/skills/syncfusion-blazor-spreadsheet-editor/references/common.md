## Component Configuration

> Configure the Spreadsheet component's basic layout properties and appearance.

### PROPERTIES
```csharp
Height="auto(Default)/CSS height value"
Width="auto(Default)/CSS width value"
ID="Unique component ID"
CssClass="CSS class names"
ActiveSheetIndex="0(Default)/valid sheet index"
AllowImage="true(Default)/false"
```

### Placeholders

| Placeholder | Description | Example |
|---|---|---|
| `CSS height value` | Any valid CSS height value | `"500px"`, `"50vh"`, `"100%"` |
| `CSS width value` | Any valid CSS width value | `"800px"`, `"75vw"`, `"100%"` |
| `Unique component ID` | A custom identifier for the component | `"MySpreadsheet"`, `"DataEditor"` |
| `CSS class names` | One or more CSS class names separated by spaces | `"dark-theme"`, `"my-style another-style"` |
| `valid sheet index` | Zero-based index of the worksheet to activate (0 = first sheet, 1 = second sheet, etc.) | `0`, `1`, `2` |

### Notes
- **Height** defaults to `auto`; set a specific value to control vertical sizing.
- **Width** defaults to `auto`; set a specific value to control horizontal sizing.
- **ID** is auto-generated if not specified; set a custom ID for targeted styling or JavaScript access.
- **CssClass** allows applying custom CSS styles to the root element for appearance customization.
- **ActiveSheetIndex** uses zero-based indexing; invalid indices are automatically corrected to 0 (first sheet).
- Changing **ActiveSheetIndex** programmatically switches the active sheet in the spreadsheet.
- **AllowImage** is enabled by default; include `AllowImage="false"` only when you want to **disable** image insertion for the spreadsheet.
- Existing images in imported Excel files are always displayed regardless of the **AllowImage** setting; this property only controls the ability to insert new images.

### Documentation link
[Blazor Spreadsheet Component](https://help.syncfusion.com/document-processing/excel/spreadsheet/blazor/getting-started)
````
