# Split PDFs

Divide one PDF into multiple PDF documents.

*Note: For document creation, loading, and save/close patterns, see [document-structure.md](document-structure.md).*

---

**Common namespaces:**

```csharp
using Syncfusion.Pdf.Parsing;
```

---

## Split each page into a separate PDF

```csharp
PdfLoadedDocument loaded = new PdfLoadedDocument("Input.pdf");
loaded.Split("Output{0}.pdf");
```

## Split by page ranges

```csharp
PdfLoadedDocument loaded = new PdfLoadedDocument("Input.pdf");
int[,] ranges = new int[,] { { 2, 5 }, { 8, 10 } };
loaded.SplitByRanges("Output{0}.pdf", ranges);
```

## Split by fixed number of pages

```csharp
PdfLoadedDocument loaded = new PdfLoadedDocument("Input.pdf");
loaded.SplitByFixedNumber("Output{0}.pdf", 4);
```

## Split with PdfSplitOptions

```csharp
PdfLoadedDocument loaded = new PdfLoadedDocument("Input.pdf");
var options = new PdfSplitOptions
{
    RemoveUnusedResources = true,
    SplitTags = true
};
loaded.Split("Output{0}.pdf", options);
```
