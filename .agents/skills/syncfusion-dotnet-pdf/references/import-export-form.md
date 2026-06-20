# Import and Export Form Fields

Import pre-filled values into PDF AcroForm fields and export user-entered form data using the Syncfusion .NET PDF Library. Supported formats: **FDF**, **XFDF**, and **JSON**.

*Note: For document creation, loading, and save/close patterns, see [document-structure.md](document-structure.md).*

---

**Common namespaces:**

```csharp
using Syncfusion.Pdf.Parsing;
```

---
## Import form data from FDF

FDF (Forms Data Format) is a compact binary format for form field data.
Use `ImportDataFDF` on `PdfLoadedForm` to import values from an FDF stream into matching fields.

```csharp
// Import form field values from an FDF file stream
using FileStream fdfStream = new FileStream("FormData.fdf", FileMode.Open, FileAccess.Read);
loadedDocument.Form.ImportDataFDF(fdfStream, true); // true = regenerate field appearance after import

loadedDocument.Save("output/Output.pdf");
```

---

## Import form data from XFDF

XFDF (XML Forms Data Format) is the XML-based version of FDF — human-readable and standards-compliant.
Use `ImportDataXFDF` on `PdfLoadedForm` to import values from an FDF stream into matching fields.

```csharp
// Import form field values from an XFDF file stream
using FileStream xfdfStream = new FileStream("FormData.xfdf", FileMode.Open, FileAccess.Read);
loadedDocument.Form.ImportDataXFDF(xfdfStream);

loadedDocument.Save("output/Output.pdf");
```

---

## Import form data from JSON

JSON is the most portable format and works well for web/API integration.
Use `ImportDataJson` on `PdfLoadedForm` to import values from an FDF stream into matching fields.

```csharp
// Import form field values from a JSON file stream
using FileStream jsonStream = new FileStream("FormData.json", FileMode.Open, FileAccess.Read);
loadedDocument.Form.ImportDataJson(jsonStream);

loadedDocument.Save("output/Output.pdf");
```

---

## Import from a MemoryStream (e.g., from a database or HTTP request)

```csharp
// Assume fdfBytes was previously stored in a database or received over HTTP
byte[] fdfBytes = File.ReadAllBytes("FormData.fdf");

using MemoryStream fdfStream = new MemoryStream(fdfBytes);
loadedDocument.Form.ImportDataFDF(fdfStream, true);

loadedDocument.Save("output/Output.pdf");
```

---

## Export form data to FDF

Use `ExportData` on `PdfLoadedForm` to write all field values to an FDF stream.

```csharp
// Export form data to an FDF file
using FileStream fdfStream = new FileStream("output/FormData.fdf", FileMode.Create, FileAccess.ReadWrite);
loadedDocument.Form.ExportData(fdfStream, DataFormat.Fdf, "SourceForm.pdf");
```

---

## Export form data to XFDF

```csharp
// Export form data to an XFDF file
using FileStream xfdfStream = new FileStream("output/FormData.xfdf", FileMode.Create, FileAccess.ReadWrite);
loadedDocument.Form.ExportData(xfdfStream, DataFormat.XFdf, "SourceForm.pdf");
```

---

## Export form data to JSON

```csharp
// Export form data to an JSON file
using FileStream jsonStream = new FileStream("output/FormData.json", FileMode.Create, FileAccess.ReadWrite);
loadedDocument.Form.ExportData(jsonStream, DataFormat.Json, "SourceForm.pdf");
```

---

## Export form data to JSON

```csharp
// Export form data to a JSON file
loadedDocument.Form.ExportData("output/FormData.json", DataFormat.Json, "SourceForm.pdf");
```

---

## Export form data to a MemoryStream (for database or HTTP response)

```csharp
using MemoryStream exportStream = new MemoryStream();
loadedDocument.Form.ExportData(exportStream, DataFormat.Fdf, "SourceForm.pdf");

byte[] fdfBytes = exportStream.ToArray();
// Store fdfBytes in a database or send via HTTP response
```

---

## Fill form fields programmatically before exporting

Fill specific fields by name or index, then export the populated values.

```csharp
PdfLoadedForm loadedForm = loadedDocument.Form;

// Fill fields by index
(loadedForm.Fields[0] as PdfLoadedTextBoxField).Text = "John Doe";
(loadedForm.Fields[1] as PdfLoadedTextBoxField).Text = "john.doe@example.com";

// Or fill by name using TryGetField
if (loadedForm.Fields.TryGetField("FirstName", out PdfLoadedField loadedField))
    (loadedField as PdfLoadedTextBoxField).Text = "John";

// Ensure values are visible in all PDF viewers
loadedForm.SetDefaultAppearance(false);

// Export the filled values to JSON
loadedDocument.Form.ExportData("output/FilledFormData.json", DataFormat.Json, "SourceForm.pdf");

loadedDocument.Save("output/Output_Filled.pdf");
```

---

## Round-trip: export → import (full workflow)

Export form data from a filled PDF and import it into a blank copy of the same form — useful for pre-filling, archiving, and restoring form state.

```csharp
// Step 1: Export form data from the filled source document
byte[] fdfBytes;
using (PdfLoadedDocument sourceDoc = new PdfLoadedDocument("source_filled.pdf"))
{
    using MemoryStream fdfStream = new MemoryStream();
    sourceDoc.Form.ExportData(fdfStream, DataFormat.Fdf, "SourceForm.pdf");
    fdfBytes = fdfStream.ToArray();
}

// Step 2: Import the exported data into a blank copy of the form
PdfLoadedDocument targetDoc = new PdfLoadedDocument("source_blank.pdf");
using MemoryStream importStream = new MemoryStream(fdfBytes);
targetDoc.Form.ImportDataFDF(importStream, true);

targetDoc.Form.SetDefaultAppearance(false);
targetDoc.Save("output/Output_Imported.pdf");
targetDoc.Close(true);
```

---

## Flatten form fields after import (burn values into the page)

Flattening converts interactive fields into static page graphics — useful for archiving or printing where fields should not be editable.

```csharp
// Import form data
using FileStream fdfStream = new FileStream("FormData.fdf", FileMode.Open, FileAccess.Read);
loadedDocument.Form.ImportDataFDF(fdfStream, true);

// Flatten all fields (non-editable after save)
loadedDocument.Form.Flatten = true;

loadedDocument.Save("output/Output_Flattened.pdf");
```

---

## Key APIs

| Member | Description |
| --- | --- |
| `PdfLoadedForm.ImportDataFDF(Stream, bool)` | Imports form field values from an FDF stream; second param = regenerate appearance |
| `PdfLoadedForm.ImportDataXFDF(Stream)` | Imports form field values from an XFDF stream; |
| `PdfLoadedForm.ImportDataJson(Stream)` | Imports form field values from an Json stream; |
| `PdfLoadedForm.ExportData(Stream, DataFormat, string)` | Exports all form field values to a stream in JSON, FDF, or XFDF format; third param = PDF source filename |
| `DataFormat.Fdf` | FDF format for `ExportData` |
| `DataFormat.XFdf` | XFDF format for `ExportData` |
| `DataFormat.Json` | JSON format for `ExportData` |
| `PdfLoadedForm.SetDefaultAppearance(false)` | Ensures field values are visible in all PDF viewers after import |
| `PdfLoadedForm.Flatten` | When `true`, converts all fields to static graphics on next `Save()` |
| `PdfLoadedForm.FlattenFields()` | Flattens fields immediately without waiting for `Save()` |
| `PdfLoadedFormFieldCollection.TryGetField(string, out PdfLoadedField)` | Gets a form field by name safely (returns `false` if not found) |
| `PdfLoadedFormFieldCollection.TryGetValue(string, out string)` | Gets a field's current value by name |

---

## Notes

- **FDF vs XFDF vs JSON**: FDF is compact binary; XFDF is XML (human-readable, standards-compliant); JSON is the most portable and recommended for REST API workflows.
- **Appearance after import**: Always call `loadedForm.SetDefaultAppearance(false)` after importing — otherwise imported values may appear blank in some viewers.
- **`ImportDataFDF` second parameter**: Pass `true` to regenerate the appearance stream for each field so filled values are visible immediately.
- **`ExportData` source filename**: The third parameter in `ExportData()` is the original PDF filename embedded in the FDF/XFDF header — it does not need to be an actual path, but conventionally matches the source PDF name.
- **Stream position**: When using a `MemoryStream` for import, ensure `stream.Position = 0` before calling import methods.
- **Flattening is irreversible**: Once flattened and saved, field interactivity is permanently removed.
