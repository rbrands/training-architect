# Import and Export Annotations

Save and restore PDF annotations by exporting them to FDF, XFDF, or JSON format and importing them back. Use these snippets to persist annotations to disk, transfer them between documents, or round-trip them through a database.

*Note: For document creation, loading, and save/close patterns, see [document-structure.md](document-structure.md).*

---

**Common namespaces:**

```csharp
using Syncfusion.Pdf.Parsing;
```

---

## Export annotations to FDF

FDF (Forms Data Format) is a compact binary format for annotation data.

```csharp
// Export all annotations to an FDF file
loadedDocument.ExportAnnotations("output/Annotations.fdf", AnnotationDataFormat.Fdf);
```

---

## Export annotations to XFDF

XFDF (XML Forms Data Format) is the XML-based version of FDF — human-readable and suitable for interchange.

```csharp
// Export all annotations to an XFDF file
loadedDocument.ExportAnnotations("output/Annotations.xfdf", AnnotationDataFormat.XFdf);
```

---

## Export annotations to JSON

JSON is the most portable format and is compatible with the Syncfusion PDF Viewer import API.

```csharp
// Export all annotations to a JSON file
loadedDocument.ExportAnnotations("output/Annotations.json", AnnotationDataFormat.Json);
```

---

## Export to a stream (instead of a file path)

Use a stream overload when you need to write to memory (e.g., for a database or HTTP response).

```csharp
using MemoryStream exportStream = new MemoryStream();
loadedDocument.ExportAnnotations(exportStream, AnnotationDataFormat.Json);

// Read the exported JSON bytes
byte[] jsonBytes = exportStream.ToArray();
string json = System.Text.Encoding.UTF8.GetString(jsonBytes);
Console.WriteLine(json);
```

---

## Import annotations from FDF

```csharp
// Import annotation data from an FDF file stream
using FileStream fdfStream = new FileStream("Annotations.fdf", FileMode.Open, FileAccess.Read);
loadedDocument.ImportAnnotations(fdfStream, AnnotationDataFormat.Fdf);

loadedDocument.Save("output/Output.pdf");
```

---

## Import annotations from XFDF

```csharp
// Import annotation data from an XFDF file stream
using FileStream xfdfStream = new FileStream("Annotations.xfdf", FileMode.Open, FileAccess.Read);
loadedDocument.ImportAnnotations(xfdfStream, AnnotationDataFormat.XFdf);

loadedDocument.Save("output/Output.pdf");
```

---

## Import annotations from JSON

```csharp
// Import annotation data from a JSON file stream
using FileStream jsonStream = new FileStream("Annotations.json", FileMode.Open, FileAccess.Read);
loadedDocument.ImportAnnotations(jsonStream, AnnotationDataFormat.Json);

loadedDocument.Save("output/Output.pdf");
```

---

## Import annotations from a MemoryStream (e.g., from a database)

```csharp
// Assume jsonBytes was previously stored in a database or received over HTTP
byte[] jsonBytes = File.ReadAllBytes("Annotations.json");

using MemoryStream importStream = new MemoryStream(jsonBytes);
loadedDocument.ImportAnnotations(importStream, AnnotationDataFormat.Json);

loadedDocument.Save("output/Output.pdf");
```

---

## Export newly added annotations before saving

When you add new annotations to a loaded document, export them **before** calling `Save()` so appearance resources are embedded. If you use `PdfTrueTypeFont`, save first, then export.

```csharp
// Add a new annotation
PdfFreeTextAnnotation freeText = new PdfFreeTextAnnotation(new RectangleF(10, 0, 150, 50));
freeText.MarkupText = "Review comment";
freeText.Font = new PdfStandardFont(PdfFontFamily.Helvetica, 8f);
freeText.Color = new PdfColor(Color.Yellow);
freeText.BorderColor = new PdfColor(Color.Red);
freeText.Border = new PdfAnnotationBorder(0.5f);
loadedDocument.Pages[0].Annotations.Add(freeText);

// Export before saving to ensure appearance resources are available
loadedDocument.ExportAnnotations("output/Annotations.json", AnnotationDataFormat.Json);

loadedDocument.Save("output/Output.pdf");
```

---

## Round-trip: export → import (full workflow)

Export annotations from one document and import them into another — useful for copying annotations between documents.

```csharp
// Step 1: Export annotations from source document
using MemoryStream annotationStream = new MemoryStream();
using (PdfLoadedDocument sourceDoc = new PdfLoadedDocument("source.pdf"))
{
    sourceDoc.ExportAnnotations(annotationStream, AnnotationDataFormat.Json);
}

// Step 2: Import annotations into target document
annotationStream.Position = 0;
PdfLoadedDocument targetDoc = new PdfLoadedDocument("target.pdf");
targetDoc.ImportAnnotations(annotationStream, AnnotationDataFormat.Json);

targetDoc.Save("output/Output_Imported.pdf");
targetDoc.Close(true);
```

---

## Key APIs

| Member | Description |
| --- | --- |
| `PdfLoadedDocument.ExportAnnotations(string filePath, AnnotationDataFormat)` | Exports all annotations to a file on disk in the specified format |
| `PdfLoadedDocument.ExportAnnotations(Stream stream, AnnotationDataFormat)` | Exports all annotations to a stream in the specified format |
| `PdfLoadedDocument.ImportAnnotations(Stream stream, AnnotationDataFormat)` | Imports annotations from a stream into the loaded document |
| `AnnotationDataFormat.Fdf` | FDF (Forms Data Format) — compact binary format |
| `AnnotationDataFormat.XFdf` | XFDF (XML Forms Data Format) — XML, human-readable |
| `AnnotationDataFormat.Json` | JSON — portable; compatible with Syncfusion PDF Viewer import API |

---

## Notes

- **Export before Save**: When exporting newly added annotations, call `ExportAnnotations()` before `Save()` so all annotation data is available. Exception: if the annotation uses `PdfTrueTypeFont`, call `Save()` first to embed font resources, then export.
- **JSON compatibility**: The JSON format exported by `ExportAnnotations()` is directly compatible with the Syncfusion PDF Viewer's `importAnnotation()` JavaScript API, enabling server-to-viewer annotation transfer.
- **Stream position**: When using a `MemoryStream` for import, always reset `stream.Position = 0` before calling `ImportAnnotations()`.
- **Appearance streams**: After importing annotations, viewers may not render them if the appearance dictionary is missing. Call `annotation.SetAppearance(true)` on each loaded annotation if visual fidelity is needed across all viewers.
- **Formats summary**: FDF is most compact; XFDF is XML and human-readable; JSON is the most portable and recommended for web/API workflows.
