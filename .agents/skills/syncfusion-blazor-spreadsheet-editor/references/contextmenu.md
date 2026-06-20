## Context Menu
> Display context-sensitive options when right-clicking on cells, rows, columns, and sheet tabs to perform operations like cut, copy, paste, insert, delete, and more.

### PROPERTY
```csharp
EnableContextMenu="true(Default)/false"
```

### Features Accessible via Context Menu

**Cell Context Menu**
- Cut, Copy, Paste
- Hyperlink creation
- Sort operations
- Clear Contents
- Filter operations

**Row Header Context Menu**
- Cut, Copy, Paste
- Insert Rows Above / Below

**Column Header Context Menu**
- Cut, Copy, Paste
- Insert Column to the Left / Right

**Sheet Tab Context Menu**
- Insert, Delete, Duplicate
- Rename, Protect/Unprotect Sheet
- Move Left / Right
- Hide Sheet

### Related Properties that Control Context Menu Options

| Property | Default | Effect when set to "false" |
|---|---|---|
| `EnableClipboard` | true | Removes Cut, Copy, Paste from all context menus |
| `AllowSorting` | true | Removes Sort option from context menus |
| `AllowFiltering` | true | Removes Filter option from context menus |
| `AllowHyperlink` | true | Removes Hyperlink options from context menus |

### When Context Menu is Disabled

When `EnableContextMenu` is set to **false**, the following features become unavailable through the UI:
- Quick access to Cut, Copy, Paste operations
- Fast row/column insertion via header context menu
- Quick sheet management (insert, delete, rename, move)
- Filter and Sort quick options
- Hyperlink creation shortcuts

**Note:** These operations may still be available through the ribbon menu, toolbar buttons, or API methods, depending on the Spreadsheet configuration.

### Sheet Protection Impact on Context Menu

When a sheet is protected:
- **Cut**, **Paste**, **Clear Contents** are restricted to unlocked cells only
- **Insert Rows/Columns** options are available only if explicitly enabled in protection settings
- **Hyperlink**, **Sort**, **Filter** options are available only if explicitly enabled in protection settings

When a workbook is protected:
- Only **Protect Sheet** / **Unprotect Sheet** option remains active
- All other sheet tab options (**Insert**, **Delete**, **Rename**, **Move**, **Hide**, **Duplicate**) are disabled

### Notes
- **EnableContextMenu** is enabled by default; include `EnableContextMenu="false"` only when you want to **disable** context menu for the sheet.
- There are no events and API methods available for context menu feature.
- Context menu options are dynamically adjusted based on sheet and workbook protection status.
- Clipboard, sorting, filtering, and hyperlink features can be independently controlled via their respective properties.

### Documentation link
[Blazor Spreadsheet Context Menu](https://help.syncfusion.com/document-processing/excel/spreadsheet/blazor/contextmenu)