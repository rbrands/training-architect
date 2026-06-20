## Hyperlink
 Edit cell hyperlink by adding, removing in the spreadsheet editor.

### PROPERTY
```csharp
AllowHyperlink="true(Default)/false"
```

### EVENTS
```csharp
HyperlinkCreating="OnHyperlinkCreating"
HyperlinkCreated="OnHyperlinkCreated"
HyperlinkClick="OnHyperlinkClick"
```
```csharp
public void OnHyperlinkCreating(HyperlinkCreatingEventArgs args)
{
    // customize your code in based on the requirements and check below table for event argument details
}
```
**HyperlinkCreating Event Arguments**
| Event Arguments | Description |
|---|---|
| `Uri` | Represents the hyperlink destination, which can be a web URL or an internal sheet reference in the format “SheetName!CellReference”. This value can be modified to redirect the hyperlink to a different location |
| `CellAddress` | Specifies the cell location where the hyperlink will be inserted. The address must be specified using A1 notation (e.g., A1, B5). |
| `DisplayText` | Defines the visible text shown in the cell for the hyperlink. This can be customized to provide a user-friendly label, distinct from the actual hyperlink destination. |
| `Cancel` | Set to `true` to prevents the hyperlink from being added, allowing for conditional validation or restriction logic.. |

```csharp
public void OnHyperlinkCreated(HyperlinkCreatedEventArgs args)
{
    // customize your code in based on the requirements and check below table for event argument details
}
```
**HyperlinkCreated Event Arguments**
| Event Arguments | Description |
|---|---|
| `Uri` | Represents the hyperlink destination, which can be either an external web URL or an internal sheet reference. This value is read-only and reflects the final destination of the hyperlink. |
| `CellAddress` | Specifies the cell location where the hyperlink has been inserted. The address is provided in A1 notation (e.g., A1, B5), and indicates the exact position of the hyperlink in the worksheet. This value is read-only. |
| `DisplayText` | Defines the visible text shown in the cell for the hyperlink. This user-friendly label may differ from the actual hyperlink address and is useful for providing descriptive or meaningful link text. This value is read-only. |


```csharp
public void OnHyperlinkClick(HyperlinkClickEventArgs args)
{
    // customize your code in based on the requirements and check below table for event argument details
}
```
**HyperlinkClick Event Arguments**
| Event Arguments | Description |
|---|---|
| `Uri` | Represents the hyperlink destination, which may be an external web URL or an internal sheet reference. This value reflects the actual navigation target of the hyperlink. This value is read only. |
| `CellAddress` | Specifies the cell location where the hyperlink resides. The address is provided in A1 notation (e.g., A1, B5), indicating the exact position of the hyperlink in the worksheet. This value is read only. |
| `DisplayText` | Defines the visible text shown in the cell for the hyperlink. This user-friendly label may differ from the actual hyperlink address and is useful for identifying the link’s purpose or context. This value is read only. |

### BUTTON
<!-- Refer the below button for creating button and update the API public method calling. -->
<button @onclick="#MethodName">#Button Name</button>

### API METHOD
```csharp
// Adds a hyperlink to the range "A2:A5".
await SpreadsheetRef.AddHyperlinkAsync(CELLADDRESS, HYPERLINK, DISPLAYTEXT);

// Removes all hyperlinks from the specified cell range (A2 to A5) in the active sheet.
await SpreadsheetRef.RemoveHyperlinkAsync(CELLADDRESS);
```

> **⚠️ SECURITY**: When adding hyperlinks programmatically, validate and sanitize all URL inputs to prevent malicious links. Never add hyperlinks from untrusted or user-provided sources without proper validation.

### UI-only Operations

These operations are available only through the user interface. There are no public APIs, events, or programmatic customization points.

**Summary**

Edit Hyperlink is a UI-only action. No public API or event is provided to trigger, intercept, or automate this operation.

**How to Perform**

- **Edit Hyperlink:** Right-click the cell containing the hyperlink and select **Edit Hyperlink** from the context menu. Alternatively, select the cell and click the **Link** option from the **Insert** tab in the **Ribbon** toolbar. Make changes to the hyperlink information in the dialog box and click **Update** to apply the changes.

**Limitations**

- No public API or event to trigger, intercept, or customize this action.
- Cannot be automated or performed programmatically.
- These actions may be disabled when hyperlink functionality is disabled (`AllowHyperlink="false"`).

### Placeholders

| Placeholder | Description | Example |
|---|---|---|
| `#MethodName` | Name of the method calling when clicking the button | - |
| `#Button Name` | Provide a meaning full name to button which binds to API method | - |
| `CELLADDRESS` | Specifies the cell or range of cells where the hyperlink should be added or removed. |`"A2:A5"`|
| `HYPERLINK` | Specifies the destination of the hyperlink. This can be a web URL (with or without a protocol), a cell reference, or a sheet reference. |`"HYPERLINK_URL"` or `"Sheet1!A1"`|
| `DISPLAYTEXT` | This is optional parameter. Specifies the text to display in the cell. If omitted, the hyperlink address is used as the display text. For cells with existing values, this parameter overrides the existing text. |`"Link Text"`|

### Notes
- **AllowHyperlink** is enabled by default; include `AllowHyperlink="false"` only when you want to **disable** hyperlink for the spreadsheet.
- **Important:** API methods should **NOT** be called inside `OnInitialized` or `OnParametersSet` lifecycle methods. Even if you call them, they will not work properly. Call API methods in response to user interactions (like button clicks) or in other appropriate lifecycle methods after the component is fully rendered.
- **Security:** Validate all hyperlink URLs before adding them programmatically. Implement URL allowlists or sanitization to prevent phishing or malicious link injection.

### Documentation link
[Blazor Spreadsheet Hyperlink](https://help.syncfusion.com/document-processing/excel/spreadsheet/blazor/hyperlink)