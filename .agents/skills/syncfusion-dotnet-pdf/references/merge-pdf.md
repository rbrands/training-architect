# Merge PDFs

Combine multiple PDF files into one document.

*Note: For document creation, loading, and save/close patterns, see [document-structure.md](document-structure.md).*

---

**Common namespaces:**

```csharp
using Syncfusion.Pdf;
using Syncfusion.Pdf.Parsing;
```

---

## Merge multiple PDFs from file paths

```csharp
PdfDocument finalDoc = new PdfDocument();
string[] sources = { "file1.pdf", "file2.pdf", "file3.pdf" };
PdfDocumentBase.Merge(finalDoc, sources);
```

## Merge multiple PDFs from streams

```csharp
PdfDocument finalDoc = new PdfDocument();
Stream[] streams = { stream1, stream2, stream3 };
PdfDocumentBase.Merge(finalDoc, streams);
```

## Merge with PdfMergeOptions

```csharp
PdfDocument finalDoc = new PdfDocument();
string[] sources = { "Src1.pdf", "Src2.pdf" };
var options = new PdfMergeOptions
{
    OptimizeResources = true,
    MergeAccessibilityTags = true
};
PdfDocumentBase.Merge(finalDoc, options, sources);
```

## Import a single page from another PDF

```csharp
PdfLoadedDocument loadedDoc = new PdfLoadedDocument(docStream);
PdfDocument targetDoc = new PdfDocument();
targetDoc.ImportPage(loadedDoc, 1);
```

## Import multiple pages from PDF

```csharp
using Syncfusion.Pdf;
using Syncfusion.Pdf.Parsing;

//Load the PDF document.
PdfLoadedDocument loadedDocument = new PdfLoadedDocument("Input.pdf");
//Create a new PDF document.
PdfDocument document = new PdfDocument();
int startIndex = 0;
int endIndex = loadedDocument.Pages.Count - 1;
//Import all the pages to the new PDF document.
document.ImportPageRange(loadedDocument, startIndex, endIndex);
```
