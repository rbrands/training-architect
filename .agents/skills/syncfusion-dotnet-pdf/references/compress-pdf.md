# PDF Compression

Reduce PDF file size using Syncfusion .NET PDF Library

*Note: For document creation, loading, and save/close patterns, see [document-structure.md](document-structure.md).*

---

**Common namespaces:**

```csharp
using Syncfusion.Pdf;
using Syncfusion.Pdf.Parsing;
```

---

## Compress an existing PDF

```csharp
// Load existing PDF (stream or path already available)
PdfLoadedDocument loaded = new PdfLoadedDocument("input.pdf");

// Configure compression options
var options = new PdfCompressionOptions
{
    CompressImages = true,   // downsample & recompress images
    ImageQuality   = 50,     // 10–100 (100 = original quality)
    OptimizeFont   = true,   // remove unused glyphs/tables
    OptimizePageContents = true, // minify/pack content streams
    RemoveMetadata = true    // strip XMP & extra doc info
};

// Apply
loaded.Compress(options);
```

### Image‑only compression (quick win)

```csharp
var options = new PdfCompressionOptions { CompressImages = true, ImageQuality = 35 };
```

### Font optimization only

```csharp
var options = new PdfCompressionOptions { OptimizeFont = true };
```

### Page content optimization only

```csharp
var options = new PdfCompressionOptions { OptimizePageContents = true };
```

### Remove metadata only

```csharp
var options = new PdfCompressionOptions { RemoveMetadata = true };
```

### (Optional) Advanced reductions

Depending on your workflow, you may also disable incremental updates (forces full rewrite) or flatten/remove forms & annotations to shrink size further.

#### Disable incremental updates

```csharp
PdfLoadedDocument loadedDocument = new PdfLoadedDocument("Input.pdf");
//Disable the incremental update
loadedDocument.FileStructure.IncrementalUpdate = false;
```

#### Flatten annotations and form fields

```csharp
using Syncfusion.Pdf;
using Syncfusion.Pdf.Parsing;

//Load the PDF document
PdfLoadedDocument loadedDocument = new PdfLoadedDocument("Input.pdf");
//Flatten all the annotations
loadedDocument.FlattenAnnotations();
// Flatten the form fields.
loadedDocument.Form.FlattenFields();
```

#### Remove annotations and form fields

```csharp
//Load the PDF document
PdfLoadedDocument loadedDocument = new PdfLoadedDocument("Input.pdf");
//Clear annotations
foreach(PdfLoadedPage lpage in loadedDocument.Pages){
    lpage.Annotations.Clear();
}
//Clear form fields
loadedDocument.Form.Fields.Clear();
```

### Platform & NuGet notes (for .NET Core / Linux)

For image‑based operations (compression/rendering) on .NET Core/Linux, include [Syncfusion.Pdf.Imaging.Net.Core](https://www.nuget.org/packages/Syncfusion.Pdf.Imaging.Net.Core) and SkiaSharp native assets (e.g., SkiaSharp.NativeAssets.Linux).
