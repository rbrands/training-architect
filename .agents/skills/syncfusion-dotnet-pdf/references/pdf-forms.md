# PDF Forms (AcroForm)

Create, fill, modify, flatten, and manage interactive AcroForm fields in PDF documents using Syncfusion .NET PDF Library. Examples are ordered from basic → advanced.

*Note: For document creation, loading, and save/close patterns, see [document-structure.md](document-structure.md). For import/export of form data, see [import-export-form.md](import-export-form.md). For digital signatures, see [digital-sign.md](digital-sign.md). For JavaScript actions on form fields, see [actions.md](actions.md).*

---

**Common namespaces:**

```csharp
using Syncfusion.Drawing;
using Syncfusion.Pdf;
using Syncfusion.Pdf.Graphics;
using Syncfusion.Pdf.Interactive;
using Syncfusion.Pdf.Parsing;
```

---

## Add a text box field to a new PDF

```csharp
PdfDocument document = new PdfDocument();
PdfPage page = document.Pages.Add();

PdfTextBoxField textBoxField = new PdfTextBoxField(page, "FirstName");
textBoxField.Bounds = new RectangleF(0, 0, 100, 20);
textBoxField.ToolTip = "First Name";
document.Form.Fields.Add(textBoxField);
```

---

## Add a text box field to an existing PDF

```csharp
PdfLoadedDocument loadedDocument = new PdfLoadedDocument("Input.pdf");
if (loadedDocument.Form == null)
    loadedDocument.CreateForm();
PdfLoadedPage loadedPage = loadedDocument.Pages[0] as PdfLoadedPage;

PdfTextBoxField textBoxField = new PdfTextBoxField(loadedPage, "FirstName");
textBoxField.Bounds = new RectangleF(0, 0, 100, 20);
textBoxField.ToolTip = "First Name";
loadedDocument.Form.Fields.Add(textBoxField);
```

---

## Add a combo box field to a new PDF

```csharp
PdfDocument document = new PdfDocument();
PdfPage page = document.Pages.Add();

PdfComboBoxField comboBoxField = new PdfComboBoxField(page, "JobTitle");
comboBoxField.Bounds = new RectangleF(0, 40, 100, 20);
comboBoxField.ToolTip = "Job Title";
comboBoxField.Items.Add(new PdfListFieldItem("Development", "dev"));
comboBoxField.Items.Add(new PdfListFieldItem("Support", "sup"));
comboBoxField.Items.Add(new PdfListFieldItem("Documentation", "doc"));
document.Form.Fields.Add(comboBoxField);
```

---

## Add a radio button field to a new PDF

```csharp
PdfDocument document = new PdfDocument();
PdfPage page = document.Pages.Add();

PdfRadioButtonListField employeesRadioList = new PdfRadioButtonListField(page, "employeesRadioList");
document.Form.Fields.Add(employeesRadioList);

PdfRadioButtonListItem radioItem1 = new PdfRadioButtonListItem("1-9");
radioItem1.Bounds = new RectangleF(100, 140, 20, 20);
PdfRadioButtonListItem radioItem2 = new PdfRadioButtonListItem("10-49");
radioItem2.Bounds = new RectangleF(100, 170, 20, 20);

employeesRadioList.Items.Add(radioItem1);
employeesRadioList.Items.Add(radioItem2);
```

---

## Set default selected index for radio button field

```csharp
PdfDocument document = new PdfDocument();
PdfPage page = document.Pages.Add();

PdfRadioButtonListField radioList = new PdfRadioButtonListField(page, "gender");
PdfFont font = new PdfStandardFont(PdfFontFamily.Helvetica, 12);

PdfRadioButtonListItem maleItem = new PdfRadioButtonListItem("Male");
maleItem.Bounds = new RectangleF(90, 203, 15, 15);
page.Graphics.DrawString("Male", font, PdfBrushes.Black, new RectangleF(110, 204, 180, 20));
radioList.Items.Add(maleItem);

PdfRadioButtonListItem femaleItem = new PdfRadioButtonListItem("Female");
femaleItem.Bounds = new RectangleF(205, 203, 15, 15);
page.Graphics.DrawString("Female", font, PdfBrushes.Black, new RectangleF(225, 204, 180, 20));
radioList.Items.Add(femaleItem);

// Select "Female" as the default
radioList.SelectedIndex = 1;
document.Form.Fields.Add(radioList);
```

---

## Add a list box field to a new PDF

```csharp
PdfDocument document = new PdfDocument();
PdfPage page = document.Pages.Add();

PdfListBoxField listBoxField = new PdfListBoxField(page, "list1");
listBoxField.Bounds = new RectangleF(100, 60, 100, 50);
listBoxField.Items.Add(new PdfListFieldItem("English", "English"));
listBoxField.Items.Add(new PdfListFieldItem("French", "French"));
listBoxField.Items.Add(new PdfListFieldItem("German", "German"));
listBoxField.SelectedIndex = 0;
listBoxField.MultiSelect = true;
document.Form.Fields.Add(listBoxField);
```

---

## Add a check box field to a new PDF

```csharp
PdfDocument document = new PdfDocument();
PdfPage page = document.Pages.Add();

PdfCheckBoxField checkBoxField = new PdfCheckBoxField(page, "CheckBox");
checkBoxField.ToolTip = "Check Box";
checkBoxField.Bounds = new RectangleF(0, 20, 10, 10);
document.Form.Fields.Add(checkBoxField);
```

---

## Add a signature field to a new PDF

```csharp
PdfDocument document = new PdfDocument();
PdfPage page = document.Pages.Add();

PdfSignatureField signatureField = new PdfSignatureField(page, "Signature");
signatureField.Bounds = new RectangleF(0, 400, 90, 20);
signatureField.ToolTip = "Signature";
document.Form.Fields.Add(signatureField);
```

---

## Add a button field to a new PDF

```csharp
PdfDocument document = new PdfDocument();
PdfPage page = document.Pages.Add();

PdfButtonField buttonField = new PdfButtonField(page, "Click");
buttonField.Bounds = new RectangleF(0, 150, 90, 20);
buttonField.Text = "Click";
document.Form.Fields.Add(buttonField);
```

---

## Add a date field (text box with JavaScript formatting)

```csharp
PdfDocument document = new PdfDocument();
PdfPage page = document.Pages.Add();

PdfTextBoxField dateField = new PdfTextBoxField(page, "DateField");
dateField.Bounds = new RectangleF(10, 40, 70, 20);
dateField.ToolTip = "Date";
dateField.Text = "12/01/1995";
// Enforce date format via JavaScript actions
dateField.Actions.KeyPressed = new PdfJavaScriptAction("AFDate_KeystrokeEx(\"mm/dd/yyyy\")");
dateField.Actions.Format    = new PdfJavaScriptAction("AFDate_FormatEx(\"mm/dd/yyyy\")");
dateField.Actions.Validate  = new PdfJavaScriptAction("AFDate_Validate(\"mm/dd/yyyy\")");
document.Form.Fields.Add(dateField);
```

---

## Fill form fields in an existing PDF

```csharp
PdfLoadedDocument loadedDocument = new PdfLoadedDocument("Input.pdf");
PdfLoadedForm loadedForm = loadedDocument.Form;

// Fill text box
PdfLoadedTextBoxField loadedTextBox = loadedForm.Fields[0] as PdfLoadedTextBoxField;
loadedTextBox.Text = "First Name";

// Fill combo box by index
PdfLoadedComboBoxField loadedCombo = loadedForm.Fields[1] as PdfLoadedComboBoxField;
loadedCombo.SelectedIndex = 1;

// Fill list box (multi-select)
PdfLoadedListBoxField loadedListBox = loadedForm.Fields[2] as PdfLoadedListBoxField;
loadedListBox.SelectedIndex = new int[] { 1, 2 };

// Fill radio button
PdfLoadedRadioButtonListField loadedRadio = loadedForm.Fields[3] as PdfLoadedRadioButtonListField;
loadedRadio.SelectedIndex = 1;

// Fill check box
PdfLoadedCheckBoxField loadedCheckBox = loadedForm.Fields[4] as PdfLoadedCheckBoxField;
loadedCheckBox.Checked = true;
```

---

## Fill XFA form fields via AcroForm API

```csharp
PdfLoadedDocument loadedDocument = new PdfLoadedDocument("Input.pdf");
PdfLoadedForm acroform = loadedDocument.Form;
// Allow AcroForm API to also fill XFA fields in static XFA documents
acroform.EnableXfaFormFill = true;

PdfLoadedTextBoxField firstName = acroform.Fields["FirstName"] as PdfLoadedTextBoxField;
firstName.Text = "Simon";
PdfLoadedTextBoxField lastName = acroform.Fields["LastName"] as PdfLoadedTextBoxField;
lastName.Text = "Bistro";
```

---

## Enumerate and fill all form fields

```csharp
PdfLoadedDocument document = new PdfLoadedDocument("Input.pdf");
PdfLoadedFormFieldCollection fields = document.Form.Fields;

for (int i = 0; i < fields.Count; i++)
{
    if (fields[i] is PdfLoadedTextBoxField textBox)
        textBox.Text = "Text";
}
```

---

## Retrieve a field by name (TryGetField)

```csharp
PdfLoadedDocument doc = new PdfLoadedDocument("Input.pdf");
PdfLoadedFormFieldCollection fieldCollection = doc.Form.Fields as PdfLoadedFormFieldCollection;

PdfLoadedField loadedField = null;
if (fieldCollection.TryGetField("f1-1", out loadedField))
    (loadedField as PdfLoadedTextBoxField).Text = "1";
```

---

## Retrieve a field value by name (TryGetValue)

```csharp
PdfLoadedDocument loadedDocument = new PdfLoadedDocument("Input.pdf");
PdfLoadedFormFieldCollection fieldCollection = loadedDocument.Form.Fields as PdfLoadedFormFieldCollection;

string fieldValue = string.Empty;
fieldCollection.TryGetValue("FirstName", out fieldValue);
Console.WriteLine("Value: " + fieldValue);
```

---

## Modify an existing form field

```csharp
PdfLoadedDocument loadedDocument = new PdfLoadedDocument("Input.pdf");
PdfLoadedForm loadedForm = loadedDocument.Form;

PdfLoadedTextBoxField loadedTextBox = loadedForm.Fields[0] as PdfLoadedTextBoxField;
loadedTextBox.Bounds = new RectangleF(100, 100, 150, 50);
loadedTextBox.Text = "New text of the field.";
loadedTextBox.SpellCheck = true;
loadedTextBox.Password = false;
```

---

## Retrieve and modify fore/back color of a form field

```csharp
PdfLoadedDocument loadedDocument = new PdfLoadedDocument("Input.pdf");
PdfLoadedTextBoxField loadedTextBox = loadedDocument.Form.Fields[0] as PdfLoadedTextBoxField;

// Get existing colors
PdfColor foreColor = loadedTextBox.ForeColor;
PdfColor backColor = loadedTextBox.BackColor;

// Apply new colors
loadedTextBox.ForeColor = new PdfColor(Color.Red);
loadedTextBox.BackColor = new PdfColor(Color.Green);
```

---

## Customize checkbox and radio button indicator colors

```csharp
PdfLoadedDocument loadedDocument = new PdfLoadedDocument("Input.pdf");
PdfLoadedForm form = loadedDocument.Form;

foreach (PdfLoadedField field in form.Fields)
{
    if (field is PdfLoadedCheckBoxField checkBox)
        checkBox.ForeColor = Color.Red;
    else if (field is PdfLoadedRadioButtonListField radioButton)
        foreach (PdfLoadedRadioButtonItem item in radioButton.Items)
            item.ForeColor = Color.Blue;
}
// Disable default appearance to allow custom rendering
form.SetDefaultAppearance(false);
```

---

## Retrieve option values from a radio button field

```csharp
PdfLoadedDocument doc = new PdfLoadedDocument("Input.pdf");
PdfLoadedForm form = doc.Form;
form.SetDefaultAppearance(false);

PdfLoadedRadioButtonListField radioField = form.Fields["Gender"] as PdfLoadedRadioButtonListField;
foreach (PdfLoadedRadioButtonItem item in radioField.Items)
{
    if (item.OptionValue == "Male")
        item.Selected = true;
}
```

---

## Retrieve widget annotation from a page

```csharp
PdfLoadedDocument loadedDocument = new PdfLoadedDocument("Input.pdf");
foreach (PdfLoadedPage page in loadedDocument.Pages)
{
    int widgetCount = 0;
    foreach (PdfLoadedAnnotation annot in page.Annotations)
    {
        if (annot is PdfLoadedWidgetAnnotation widget)
        {
            RectangleF bounds = widget.Bounds;
            Console.WriteLine($"Index: {widgetCount}, Bounds: {bounds}");
            widgetCount++;
        }
    }
    Console.WriteLine($"Total widgets on page: {widgetCount}");
}
```

---

## Get the page number of each form field

```csharp
PdfLoadedDocument document = new PdfLoadedDocument("Input.pdf");
PdfLoadedFormFieldCollection fieldCollection = document.Form.Fields;

// Build page-number lookup map
Dictionary<PdfPageBase, int> pageMap = new Dictionary<PdfPageBase, int>();
for (int i = 0; i < document.Pages.Count; i++)
    pageMap[document.Pages[i]] = i + 1;

foreach (PdfLoadedField field in fieldCollection)
{
    if (field.Page != null && pageMap.TryGetValue(field.Page, out int pageNumber))
        Console.WriteLine($"{field.Name} — Page: {pageNumber}");
}
```

---

## Remove form fields from an existing PDF

```csharp
PdfLoadedDocument loadedDocument = new PdfLoadedDocument("Input.pdf");
PdfLoadedForm loadedForm = loadedDocument.Form;

// Remove by reference
PdfLoadedTextBoxField loadedTextBox = loadedForm.Fields[0] as PdfLoadedTextBoxField;
loadedForm.Fields.Remove(loadedTextBox);

// Remove by index
loadedForm.Fields.RemoveAt(0);
```

---

## Flatten form fields (make non-editable)

```csharp
// Flatten all fields in a new document
PdfDocument document = new PdfDocument();
PdfPage page = document.Pages.Add();
PdfTextBoxField textBox = new PdfTextBoxField(page, "FirstName");
textBox.Bounds = new RectangleF(0, 0, 100, 20);
document.Form.Flatten = true;
document.Form.Fields.Add(textBox);
```

```csharp
// Flatten all fields in an existing document
PdfLoadedDocument loadedDocument = new PdfLoadedDocument("Input.pdf");
PdfLoadedForm loadedForm = loadedDocument.Form;
(loadedForm.Fields[0] as PdfLoadedTextBoxField).Text = "Text";
loadedForm.Flatten = true;
```

```csharp
// Flatten fields immediately (before save, using FlattenFields)
PdfLoadedDocument loadedDocument = new PdfLoadedDocument("Input.pdf");
loadedDocument.Form.FlattenFields();
```

---

## Set form fields as read-only

```csharp
// New document — entire form read-only
PdfDocument document = new PdfDocument();
PdfPage page = document.Pages.Add();
document.Form.ReadOnly = true;

PdfTextBoxField textBox = new PdfTextBoxField(page, "FirstName");
textBox.Bounds = new RectangleF(0, 0, 100, 20);
textBox.Text = "john";
document.Form.Fields.Add(textBox);
```

```csharp
// Existing document — entire form read-only
PdfLoadedDocument loadedDocument = new PdfLoadedDocument("Input.pdf");
loadedDocument.Form.ReadOnly = true;
```

---

## Control form field visibility

```csharp
PdfDocument document = new PdfDocument();
PdfPage page = document.Pages.Add();
PdfFont font = new PdfStandardFont(PdfFontFamily.Courier, 12f);

PdfTextBoxField textBox = new PdfTextBoxField(page, "firstNameTextBox");
textBox.Bounds = new RectangleF(100, 20, 200, 20);
textBox.Font = font;
textBox.Text = "Text Box";
// Options: Visible, Hidden, HiddenPrintable, VisibleNotPrintable
textBox.Visibility = PdfFormFieldVisibility.Visible;
document.Form.Fields.Add(textBox);
```

---

## Enable field auto-naming (group fields with the same name)

```csharp
PdfDocument document = new PdfDocument();
PdfPage page = document.Pages.Add();
PdfForm form = document.Form;

// true = each same-named field acts independently (default)
// false = same-named fields share a single group value
form.FieldAutoNaming = true;

PdfTextBoxField field1 = new PdfTextBoxField(page, "Name");
field1.Bounds = new RectangleF(0, 0, 100, 20);
field1.Text = "John";
document.Form.Fields.Add(field1);

PdfTextBoxField field2 = new PdfTextBoxField(page, "Name");
field2.Bounds = new RectangleF(0, 50, 100, 20);
field2.Text = "Doe";
document.Form.Fields.Add(field2);
```

---

## Auto-resize text in a text box field

```csharp
PdfLoadedDocument loadedDocument = new PdfLoadedDocument("Input.pdf");
PdfLoadedTextBoxField loadedField = loadedDocument.Form.Fields[0] as PdfLoadedTextBoxField;
loadedField.AutoResizeText = true;
loadedDocument.Form.Flatten = true;
```

---

## Add complex script (non-Latin) text support

```csharp
PdfDocument document = new PdfDocument();
PdfPage page = document.Pages.Add();

PdfTextBoxField textField = new PdfTextBoxField(page, "textBox");
textField.Bounds = new RectangleF(10, 10, 200, 30);
textField.Text = "สวัสดีชาวโลก";

FileStream fontStream = new FileStream("tahoma.ttf", FileMode.Open, FileAccess.Read);
textField.Font = new PdfTrueTypeFont(fontStream, 10);
textField.ComplexScript = true;
document.Form.Fields.Add(textField);
document.Form.SetDefaultAppearance(false);
```

```csharp
// Enable complex script for all supported fields at once
document.Form.ComplexScript = true;
```

---

## Set appearance dictionary (fix empty fields in viewers)

```csharp
// Call SetDefaultAppearance(false) so PDF viewers render filled values correctly
PdfLoadedDocument loadedDocument = new PdfLoadedDocument("Input.pdf");
PdfLoadedForm loadedForm = loadedDocument.Form;
loadedForm.SetDefaultAppearance(false);

(loadedForm.Fields[0] as PdfLoadedTextBoxField).Text = "First Name";
```

---

## Import FDF data into a PDF form

```csharp
PdfLoadedDocument loadedDocument = new PdfLoadedDocument("Input.pdf");
FileStream fdfStream = new FileStream("ImportFDF.fdf", FileMode.Open, FileAccess.Read);
loadedDocument.Form.ImportDataFDF(fdfStream, true);
```

---

## Export form data to FDF

```csharp
PdfLoadedDocument loadedDocument = new PdfLoadedDocument("Input.pdf");
PdfLoadedForm loadedForm = loadedDocument.Form;
FileStream stream = new FileStream("Export.fdf", FileMode.Create, FileAccess.ReadWrite);
loadedForm.ExportData(stream, DataFormat.Fdf, "SourceForm.pdf");
```

---

## Preserve form fields when creating a template from an existing page

```csharp
// Flatten first so form field visuals carry over into the template
PdfLoadedDocument loadedDocument = new PdfLoadedDocument("Form.pdf");
loadedDocument.Form.FlattenFields();

PdfTemplate template = (loadedDocument.Pages[0] as PdfLoadedPage).CreateTemplate();

PdfDocument newDocument = new PdfDocument();
newDocument.PageSettings.Margins.All = 0;
PdfPage newPage = newDocument.Pages.Add();
newPage.Graphics.DrawPdfTemplate(template, PointF.Empty,
    new SizeF(newPage.Size.Width, newPage.Size.Height));

newDocument.Save("Output.pdf");
loadedDocument.Close(true);
newDocument.Close(true);
```

---

## Key APIs

| Member | Description |
| --- | --- |
| `PdfForm` | Represents the AcroForm of a new PDF document; accessed via `PdfDocument.Form` |
| `PdfLoadedForm` | Represents the AcroForm of a loaded PDF document; accessed via `PdfLoadedDocument.Form` |
| `PdfFormFieldCollection` | Collection of all fields in a form; use `Fields.Add()` to append |
| `PdfLoadedFormFieldCollection` | Collection of fields in a loaded form; supports `TryGetField` and `TryGetValue` |
| `PdfTextBoxField(PdfPage, string)` | Creates a single-line or multiline text box field |
| `PdfComboBoxField(PdfPage, string)` | Creates a drop-down combo box field; add items via `Items.Add(PdfListFieldItem)` |
| `PdfRadioButtonListField(PdfPage, string)` | Creates a named radio button group; add items via `Items.Add(PdfRadioButtonListItem)` |
| `PdfListBoxField(PdfPage, string)` | Creates a scrollable list box; supports multi-select via `MultiSelect` |
| `PdfCheckBoxField(PdfPage, string)` | Creates a check box field; toggle state via `Checked` |
| `PdfSignatureField(PdfPage, string)` | Creates an empty signature field placeholder |
| `PdfButtonField(PdfPage, string)` | Creates a push button; set label via `Text` |
| `PdfListFieldItem(string, string)` | Item used in `PdfComboBoxField` and `PdfListBoxField` (display text, export value) |
| `PdfRadioButtonListItem(string)` | Item used in `PdfRadioButtonListField`; set position via `Bounds` |
| `PdfField.Bounds` | Gets or sets the position and size of the field as `RectangleF` |
| `PdfField.ToolTip` | Tooltip text shown when hovering over the field |
| `PdfField.ReadOnly` | Prevents user edits when `true` |
| `PdfField.Flatten` | Converts field to static graphics on save when `true` |
| `PdfField.Visibility` | Controls display/print visibility via `PdfFormFieldVisibility` enum |
| `PdfForm.Flatten` | Flattens all fields in the form on save |
| `PdfLoadedForm.FlattenFields()` | Flattens all fields immediately (before save) |
| `PdfForm.ReadOnly` | Makes the entire form read-only |
| `PdfForm.FieldAutoNaming` | `true` (default) = each same-named field is unique; `false` = grouped |
| `PdfForm.ComplexScript` | Enables complex script (non-Latin) rendering for all supported fields |
| `PdfForm.SetDefaultAppearance(bool)` | Pass `false` to create appearance dict; required for fields to display in all viewers |
| `PdfLoadedForm.EnableXfaFormFill` | Allows static XFA fields to be filled via AcroForm API |
| `PdfLoadedFormFieldCollection.TryGetField(string, out PdfLoadedField)` | Safely looks up a field by name; returns `false` if not found |
| `PdfLoadedFormFieldCollection.TryGetValue(string, out string)` | Safely retrieves a field's string value by name |
| `PdfLoadedTextBoxField.Text` | Gets or sets the text value of a loaded text box |
| `PdfLoadedTextBoxField.AutoResizeText` | Auto-scales text to fit the field bounds |
| `PdfLoadedTextBoxField.ForeColor` / `.BackColor` | Gets or sets the foreground/background color of a text box |
| `PdfLoadedComboBoxField.SelectedIndex` / `.SelectedValue` | Gets or sets the active selection in a combo box |
| `PdfLoadedRadioButtonListField.SelectedIndex` / `.SelectedValue` | Gets or sets the active item in a radio button group |
| `PdfLoadedRadioButtonItem.OptionValue` | The export value of a radio button item |
| `PdfLoadedCheckBoxField.Checked` | Gets or sets the checked state |
| `PdfLoadedCheckBoxItem.ExportValue` | The value exported when the check box is checked |
| `PdfLoadedListBoxField.SelectedIndex` | Gets or sets selected indices (supports `int[]` for multi-select) |
| `PdfLoadedWidgetAnnotation` | Low-level representation of a form field widget on a page |
| `PdfLoadedDocument.CreateForm()` | Creates a new AcroForm in a loaded PDF that has no form |
| `PdfLoadedForm.ImportDataFDF(Stream, bool)` | Imports field data from an FDF stream |
| `PdfLoadedForm.ExportData(Stream, DataFormat, string)` | Exports field data to FDF, XFDF, or JSON stream |

---

## Notes

- Always call `SetDefaultAppearance(false)` after filling fields so values are visible in all PDF viewers.
- Use `PdfForm.Flatten = true` to flatten on save; use `FlattenFields()` to flatten immediately before save.
- `FieldAutoNaming = false` groups fields with the same name so they share a single value — useful for repeating fields across pages.
- `ComplexScript` must be enabled for non-Latin languages (Thai, Arabic, Hebrew, etc.) in text boxes, combo boxes, list boxes, and buttons.
- For signature field filling (with certificate), refer to [digital-sign.md](digital-sign.md).
- For FDF/XFDF/JSON round-trip workflows, refer to [import-export-form.md](import-export-form.md).

---

## Related

- [import-export-form.md](import-export-form.md)
- [digital-sign.md](digital-sign.md)
- [actions.md](actions.md)
- [annotations.md](annotations.md)
- [document-structure.md](document-structure.md)
- ../SKILL.md

## Official documentation

- <https://help.syncfusion.com/document-processing/pdf/pdf-library/net/working-with-forms>
