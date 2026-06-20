# PDF Image Extraction

Extract images and image metadata from PDF pages and documents using Syncfusion .NET PDF Library.

*Note: For document loading and close patterns, see [document-structure.md](document-structure.md). For text extraction, see [extract-text.md](extract-text.md).*

---

**NuGet package:**

`Syncfusion.Pdf.Imaging.Net.Core` - (.NET Core / ASP.NET Core)

**Common namespaces:**

```csharp
using Syncfusion.Pdf.Exporting;
using Syncfusion.Pdf.Parsing;
```

---

## Extract images from a page

Use `PdfPageBase.ExtractImages()` to get all images from a specific page as streams.

```csharp
PdfLoadedDocument loadedDocument = new PdfLoadedDocument("Input.pdf");
PdfPageBase page = loadedDocument.Pages[0];

// Returns each image as a Stream
Stream[] extractedImages = page.ExtractImages();
```

---

## Extract image info from a page

Use `PdfPageBase.ImagesInfo` to get image metadata (bounds, index) alongside the image streams.

```csharp
PdfLoadedDocument loadedDocument = new PdfLoadedDocument("Input.pdf");
PdfPageBase page = loadedDocument.Pages[0];

// Returns PdfImageInfo[] with bounds, image index, and stream per image
PdfImageInfo[] imagesInfo = page.GetImagesInfo();
foreach (PdfImageInfo info in imagesInfo)
{
    RectangleF bounds = info.Bounds;
    int index = info.Index;
    Stream imageStream = info.ImageStream;
}
```

---

## Extract images from an entire document (high performance)

Use `PdfDocumentExtractor` for better memory consumption and performance when extracting from large documents.

```csharp
FileStream inputStream = new FileStream("Input.pdf", FileMode.Open, FileAccess.Read);
PdfDocumentExtractor extractor = new PdfDocumentExtractor();
extractor.Load(inputStream);

// Extract images from all pages
Stream[] allImages = extractor.ExtractImages();

// Or extract images from a specific page range (e.g., pages 2–6)
Stream[] rangeImages = extractor.ExtractImages(2, 6);

extractor.Dispose();
```

---

## Key APIs

| Member | Description |
| --- | --- |
| `PdfPageBase.ExtractImages()` | Extracts all images from the page; returns `Stream[]` |
| `PdfPageBase.GetImagesInfo()` | Extracts image metadata (bounds, index, stream) from the page; returns `PdfImageInfo[]` |
| `PdfImageInfo.Bounds` | `RectangleF` representing the position and size of the image on the page |
| `PdfImageInfo.Index` | Zero-based index of the image on the page |
| `PdfImageInfo.ImageStream` | Raw image data as a `Stream` |
| `PdfDocumentExtractor` | High-performance extractor for processing entire documents with lower memory usage |
| `PdfDocumentExtractor.Load(Stream)` | Loads a PDF document stream for extraction |
| `PdfDocumentExtractor.PageCount` | Total number of pages in the loaded document |
| `PdfDocumentExtractor.ExtractImages()` | Extracts images from all pages; returns `Stream[]` |
| `PdfDocumentExtractor.ExtractImages(int, int)` | Extracts images from a page range (start, end); returns `Stream[]` |
| `PdfDocumentExtractor.Dispose()` | Releases all resources held by the extractor |

---

## Notes

- Use `PdfPageBase.ExtractImages()` for extracting from individual pages.
- Use `PdfDocumentExtractor` for full-document extraction — it is more efficient for large or image-heavy PDFs.
- The `Syncfusion.Pdf.Imaging.Net.Core` NuGet package is required for image extraction in .NET Core applications.
- On Ubuntu ARM64, add `SkiaSharp.NativeAssets.Linux` to your project to avoid missing native asset errors.

---

## Related

- [extract-text.md](extract-text.md)
- [pdf-graphics.md](pdf-graphics.md)
- [document-structure.md](document-structure.md)
- ../SKILL.md

## Official documentation

- <https://help.syncfusion.com/document-processing/pdf/pdf-library/net/working-with-image-extraction>
