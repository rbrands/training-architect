## Formula Bar

The **Formula Bar** simplifies editing or entering cell data. Formula bar toggled by `ShowFormulaBar` (default true).

### PROPERTY
```csharp
ShowFormulaBar="true(Default)/false"
```

### Notes
- **ShowFormulaBar** is enabled by default; include `ShowFormulaBar="false"` only when you want to **hide** formula bar for the spreadsheet.
- There is no events and api method available for formula bar.

---

## UI-only Operations

These operations are available only through the user interface. There are no public APIs or programmatic customization points.

### Calculation Mode

**Automatic Mode** - Formulas recalculate instantly when any dependent cell changes.
- Access via the **Formulas** tab in the Ribbon toolbar.
- Select **Calculation Options** → **Automatic**.

**Manual Mode** - Formulas recalculate only when explicitly triggered.
- **Calculate Sheet:** Recalculates formulas for the active sheet only.
- **Calculate Workbook:** Recalculates formulas across all sheets in the workbook.
- Access via the **Formulas** tab in the Ribbon toolbar.

### Named Ranges

**Create Named Range:**
- Select the desired range of cells and enter a name in the **Name Box**.
- Or, select the range and click the **Name Manager** button in the **Formulas** tab.

**Edit Named Range:**
- Open the **Name Manager** dialog.
- Select the Named Range and click the **Edit** icon.
- Modify the name, range, or scope as needed.
- Click **Update Range** → **OK** to save.

**Delete Named Range:**
- Open the **Name Manager** dialog.
- Select the Named Range and click the **Delete** icon.
- Click **OK** to confirm.

### Limitations

- No public API to programmatically change calculation mode or manage named ranges.
- Cannot be automated or performed programmatically.
- Deleting a Named Range used in formulas may cause formula errors.
- Named Ranges can be defined only for cells or ranges that contain values.

---

### Documentation link
[Blazor Spreadsheet Formulas](https://help.syncfusion.com/document-processing/excel/spreadsheet/blazor/formulas)

