## Cell Formatting

> Apply cell styles including fonts, colors, alignment, and text decoration. Control cell formatting permissions and programmatically format cells.

### Number formatting

> Display numeric, date, and time values with built-in Excel-style formats or custom patterns. Control number formatting permissions and programmatically apply formats to cells.

### Borders

> Apply borders to cells with customizable styles, colors, and regions. Visually separate cells and define table boundaries. Control border permissions and programmatically apply borders to cell ranges.

### PROPERTIES
```csharp
AllowCellFormatting="true(Default)/false"
AllowNumberFormatting="true(Default)/false"
```

### BUTTON
<!-- Refer the below button for creating button and update the API public method calling. -->
<button @onclick="#MethodName">#Button Name</button>

### API METHODS
```csharp
// Apply custom format to range
await SpreadsheetInstance.NumberFormatAsync(FORMAT, CELLADDRESS);

// Apply custom cell styles to specific range
await SpreadsheetInstance.CellFormatAsync(new CellFormat
{
    BackgroundColor = "#FFEB3B",
    FontStyle = FontStyle.Italic
}, CELLADDRESS);

/// <summary>
/// Applies cell borders programmatically to a cell or range.
/// </summary>
/// <param name="borderType">
/// The border region to apply (e.g., <see cref="BorderType.OutsideBorders"/>,
/// <see cref="BorderType.AllBorders"/>, <see cref="BorderType.TopBorder"/>).
/// </param>
/// <param name="lineStyle">
/// Border line style (e.g., <c>ExcelLineStyle.Thin</c>, <c>ExcelLineStyle.Medium</c>,
/// <c>ExcelLineStyle.Dashed</c>, <c>ExcelLineStyle.Dotted</c>, <c>ExcelLineStyle.Double</c>).
/// </param>
/// <param name="borderColor">
/// Border color in CSS/hex format (e.g., "#000000", "red", "#2196F3").
/// </param>
/// <param name="cellAddress">
/// (Optional) Target cell or range in A1 notation (e.g., "A1", "A1:C5", "Sheet2!B2:D4").
/// If omitted, applies to the current selection.
/// </param>
/// <remarks>
/// Requires <c>AllowCellFormatting = true</c>. Supports common border presets
/// (Top/Left/Right/Bottom/No/All/Horizontal/Vertical/Outside/Inside) with
/// customizable style and color. Use named ranges or A1 addresses for scope.
/// </remarks>
SpreadsheetRef.SetBordersAsync(BorderType.AllBorders, ExcelLineStyle.Dashed, "#0000FF", "B2:D4");
```

### Placeholders

| Placeholder | Description | Example |
|---|---|---|
| `#MethodName` | Name of the method calling when clicking the button | - |
| `#Button Name` | Provide a meaning full name to button which binds to API method | - |
| `FORMAT` | The built-in format or a supported custom pattern. | `"0.00%"`, `"mm/dd/yyyy"` |
| `CELLADDRESS` | The address of the target range where the format is applied (e.g., "Sheet1!A2:A5" or "A2:A5"). If the sheet name is not specified, the format is applied to the specified range in the active sheet. When cellAddress is omitted, the current selection is formatted. For column or row selection, use full range like "A1:A1000" instead of "C:C" or "1:1". | `"Sheet1!A2:A5"`, `"A2:A5"`, `"D1"` |

### CellFormat Class Properties

| Property | Type | Description |
|---|---|---|
| `BackgroundColor` | string | Cell background color in hex or named CSS format (e.g., "#4B5366", "red") |
| `Color` | string | Font color in hex or named CSS format (e.g., "#FFFFFF", "blue") |
| `FontFamily` | FontFamily enum | Font family (e.g., FontFamily.Arial, FontFamily.TimesNewRoman) |
| `FontSize` | string | Font size with unit (e.g., "14pt", "12px") |
| `FontWeight` | FontWeight enum | Font weight (e.g., FontWeight.Bold, FontWeight.Normal) |
| `FontStyle` | FontStyle enum | Font style (e.g., FontStyle.Italic, FontStyle.Normal) |
| `TextDecoration` | TextDecoration enum | Text decoration (e.g., TextDecoration.Underline, TextDecoration.LineThrough) |
| `TextAlign` | TextAlign enum | Horizontal alignment (e.g., TextAlign.Center, TextAlign.Left, TextAlign.Right) |
| `VerticalAlign` | VerticalAlign enum | Vertical alignment (e.g., VerticalAlign.Middle, VerticalAlign.Top, VerticalAlign.Bottom) |

### Notes
- **AllowCellFormatting** is enabled by default; include `AllowCellFormatting="false"` only when you want to **disable** cell formatting for spreadsheet.
- **AllowNumberFormatting** is enabled by default; include `AllowNumberFormatting="false"` only when you want to **disable** number format for spreadsheet.
- **ExcelLineStyle for borders**: When using `ExcelLineStyle` for borders, you must add `@using Syncfusion.XlsIO` directive at the top of your Razor component file.
- **Wrap Text**: Wrap text is not supported through API methods. It can only be applied through UI actions in the spreadsheet(Ribbon > Home tab > Wrap Text button).
- **Important:** API methods should **NOT** be called inside `OnInitialized` or `OnParametersSet` lifecycle methods. Even if you call them, they will not work properly. Call API methods in response to user interactions (like button clicks) or in other appropriate lifecycle methods after the component is fully rendered.

### Documentation link
[Blazor Spreadsheet formatting](https://help.syncfusion.com/document-processing/excel/spreadsheet/blazor/formatting)