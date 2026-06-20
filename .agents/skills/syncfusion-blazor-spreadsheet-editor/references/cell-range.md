## Cell range / Autofill

> Update the cell range by using autofill in the Spreadsheet Editor

### PROPERTY
```csharp
AllowAutofill="true(Default)/false"
```

### EVENTS

```csharp
AutofillActionBegin="OnAutofillActionBegin"
AutofillActionEnd="OnAutofillActionEnd"
```

```csharp
// Event handlers to place inside the @code section
public void OnAutofillActionBegin(AutofillActionBeginEventArgs args)
{
    // customize your code in based on the requirements and check below table for event argument details
}
```
**AutofillActionBegin Event Arguments**
| Event Arguments | Description |
|---|---|
| `FillRange (read-only)` | The address of the target range for the autofill operation (e.g., “Sheet1!A2:A5”) |
| `DataRange (read-only)` | The source data range for the autofill operation (e.g., “Sheet1!A1:A1”) |
| `Direction (read-only)` | The direction of the autofill operation (“Down”, “Right”, “Up”, or “Left”) |
| `Cancel` | Set to `true` to cancel the autofill operation. |


```csharp
public void OnAutofillActionEnd(AutofillActionEndEventArgs args)
{
    // customize your code in based on the requirements and check below table for event argument details
}
```
**AutofillActionEnd Event Arguments**
| Event Arguments | Description |
|---|---|
| `FillRange (read-only)` | The address of the target range for the autofill operation (e.g., “Sheet1!A2:A5”) |
| `DataRange (read-only)` | The source data range for the autofill operation (e.g., “Sheet1!A1:A1”) |
| `Direction (read-only)` | The direction of the autofill operation (“Down”, “Right”, “Up”, or “Left”) |

### BUTTON
<!-- Refer the below button for creating button and update the API public method calling. -->
<button @onclick="{Method Name}">{Button Name}</button>

### API METHOD
```csharp
/// <summary>
/// Performs Excel-style autofill from a source range into a target range.
/// </summary>
/// <param name="fillRange">
/// Target cells to fill (A1 notation), e.g., "A1:A10".
/// </param>
/// <param name="dataRange">
/// Source cells providing the pattern or values, e.g., "A1:A2".
/// </param>
/// <param name="direction">
/// Fill direction: "Up", "Right", "Down", or "Left".
/// </param>
/// <remarks>
/// Requires AllowAutofill = true. Autofill follows the source pattern.
/// UI autofill menu options (Copy Cells, Fill Series, Fill Without Formatting,
/// Fill Formatting Only) apply only to drag-fill; they are not parameters of AutofillAsync.
/// </remarks>
public Task AutofillAsync(string fillRange, string dataRange, string direction);
```

### Autofill Options

The autofill feature supports multiple behaviors that control how adjacent cells are populated when using the fill handle:

#### Copy Cells
Copies the source cell content and formatting to the destination range. This replicates both values and presentation. When the source contains formulas, relative references are adjusted accordingly.

#### Fill Series
Extends a recognizable pattern—such as numbers (1, 2, 3), days or months (Mon, Tue; Jan, Feb), or dates—into the destination range while preserving the source formatting.

#### Fill Formatting Only
Applies only the source styling (number format, font, fill color, borders, and alignment) to the destination range, leaving existing values unchanged. This unifies appearance without altering the data.

#### Fill Without Formatting
Continues the detected series into the destination range but retains the destination's existing formatting. This applies only the new values while keeping the target style intact.

### Notes
- **AllowAutofill** is enabled by default; include `AllowAutofill="false"` only when you want to **disable** autofill functionality in spreadsheet
- **Important:** API methods should **NOT** be called inside `OnInitialized` or `OnParametersSet` lifecycle methods. Even if you call them, they will not work properly. Call API methods in response to user interactions (like button clicks) or in other appropriate lifecycle methods after the component is fully rendered.

### Documentation link
[Blazor Spreadsheet Cell Range](https://help.syncfusion.com/document-processing/excel/spreadsheet/blazor/cell-range)