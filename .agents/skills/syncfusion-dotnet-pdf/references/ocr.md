# OCR (Optical Character Recognition)

Guide and code snippets for performing OCR on scanned PDF documents and images using Syncfusion .NET OCR Processor Library (powered by Tesseract). Examples are ordered from basic → advanced.

*Note: For document creation, loading, and save/close patterns, see [document-structure.md](document-structure.md). For text extraction from non-scanned PDFs, see [extract-text.md](extract-text.md).*

---

**NuGet package:**

`Syncfusion.PDF.OCR.Net.Core` - (.NET Core / ASP.NET Core / Blazor)
`Syncfusion.Pdf.OCR.WinForms` - (Windows Forms / .NET Framework)
`Syncfusion.Pdf.OCR.Wpf` - (WPF)

**Common namespaces:**

```csharp
using Syncfusion.OCRProcessor;
using Syncfusion.Pdf.Parsing;
```

---

## Perform OCR on an entire PDF document

Convert a scanned PDF into a searchable PDF using `OCRProcessor.PerformOCR`.

```csharp
using (OCRProcessor processor = new OCRProcessor())
{
    PdfLoadedDocument document = new PdfLoadedDocument("Input.pdf");
    processor.Settings.Language = Languages.English;
    processor.PerformOCR(document);
}
```

---

## Perform OCR on a region of a page

Restrict OCR to specific rectangular regions on a page using `PageRegion` and `OCRSettings.Regions`.

```csharp
// Define a region on page 0
PageRegion region = new PageRegion();
region.PageIndex = 0;
region.PageRegions = new RectangleF[] { new RectangleF(0, 100, 950, 150) };

processor.Settings.Regions = new List<PageRegion> { region };
processor.PerformOCR(document);
```

---

## Perform OCR on a rotated page

Use `PageSegMode.AutoOsd` to correctly detect and process text on rotated pages.

```csharp
// AutoOsd detects orientation and script before OCR
processor.Settings.PageSegment = PageSegMode.AutoOsd;
processor.PerformOCR(document);
```

---

## Perform OCR with Tesseract version

Switch the Tesseract engine version using `OCRSettings.TesseractVersion`. Default is version 5.0.

```csharp
// Version3_05 | Version4_0 | Version5_0 (default)
processor.Settings.TesseractVersion = TesseractVersion.Version5_0;
processor.PerformOCR(document);
```

---

## Perform OCR on large PDF documents

Pass `isMemoryOptimized: true` to reduce memory usage on large or image-heavy documents.

```csharp
// Reduces memory footprint for large/image-heavy PDFs
processor.PerformOCR(document, isMemoryOptimized: true);
```

> **Note:** Memory optimization is not supported on ASP.NET Core platform.

---

## Optimize OCR performance with tessdata variants

Point `TessDataPath` to `tessdata_fast` or `tessdata_best` trained data to tune speed vs. accuracy.

```csharp
// Use tessdata_fast (speed) or tessdata_best (accuracy)
processor.TessDataPath = @"/path/to/tessdata-fast";
processor.PerformOCR(document);
```

---

## Get OCR layout result (text and bounds)

Use `OCRLayoutResult` to extract OCRed text along with per-line bounding rectangles.

```csharp
processor.PerformOCR(document, @"Tessdata/", out OCRLayoutResult layoutResult);

foreach (Line line in layoutResult.Pages[0].Lines)
{
    string text = line.Text;
    RectangleF bounds = line.Rectangle;
}
```

---

## Set page segmentation mode

Control how Tesseract analyses page layout using `OCRSettings.PageSegment`.

```csharp
// OsdOnly | AutoOsd | Auto (default) | SingleColumn | SparseText | RawLine | ...
processor.Settings.PageSegment = PageSegMode.AutoOsd;
processor.PerformOCR(document);
```

---

## Set OCR engine mode

Choose the OCR engine backend using `OCRSettings.OCREngineMode`. Default is `Default`.

```csharp
// TesseractOnly | LSTMOnly | TesseractAndLSTM | Default
processor.Settings.OCREngineMode = OCREngineMode.LSTMOnly;
processor.PerformOCR(document);
```

> **Note:** OCR Engine Mode requires Tesseract version 4.0 or above.

---

## Set image enhancement mode

Control how images are pre-processed before OCR using `ImageEnhancementMode`.

```csharp
// EnhanceForRecognitionOnly (default) | EnhanceAndIncludeInOutput | None
processor.ImageEnhancementMode = OcrImageEnhancementMode.EnhanceForRecognitionOnly;
processor.PerformOCR(document);
```

---

## Set image enhancement options

Fine-tune individual enhancement steps using `OcrImageEnhancementOptions`.

```csharp
OcrImageEnhancementOptions options = new OcrImageEnhancementOptions();
options.IsGrayscaleEnabled = true;  // Remove color noise
options.IsDeskewEnabled = true;     // Correct tilted text
options.IsDenoiseEnabled = true;    // Remove speckles
options.IsConstrastEnabled = true;  // Enhance contrast
options.IsBinarizeEnabled = true;   // Convert to black-and-white
processor.PerformOCR(document);
```

---

## White list

Restrict recognized characters to a specific set using `OCRSettings.WhiteList`.

```csharp
// Only recognise characters in this set
processor.Settings.WhiteList = "0123456789";
processor.PerformOCR(document);
```

---

## Black list

Exclude specific characters from recognition using `OCRSettings.BlackList`.

```csharp
// Exclude these characters from OCR results
processor.Settings.BlackList = "!@#$%";
processor.PerformOCR(document);
```

---

## OCR an image to PDF

Perform OCR on an image file and produce a searchable PDF document.

```csharp
FileStream imageStream = new FileStream("Input.jpg", FileMode.Open);
processor.Settings.Language = Languages.English;
processor.Settings.Conformance = PdfConformanceLevel.Pdf_A1B; // optional
PdfDocument document = processor.PerformOCR(imageStream);
```

---

## Perform OCR with Unicode characters

Preserve Unicode characters in the output PDF by supplying a TrueType Unicode font.

```csharp
// Supply a Unicode TrueType font to embed multi-language characters
FileStream fontStream = new FileStream("ARIALUNI.ttf", FileMode.Open);
processor.UnicodeFont = new PdfTrueTypeFont(fontStream, 8);
processor.PerformOCR(document);
```

---

## Key APIs

| Member | Description |
| --- | --- |
| `OCRProcessor()` | Initialises the OCR processor (disposable; always use inside `using`) |
| `OCRProcessor.PerformOCR(PdfLoadedDocument)` | Performs OCR on all pages of the loaded PDF and makes it searchable |
| `OCRProcessor.PerformOCR(Stream)` | Performs OCR on an image stream and returns the recognised text string |
| `OCRProcessor.PerformOCR(Stream) → PdfDocument` | Converts an image to a searchable PDF document |
| `OCRProcessor.TessDataPath` | Path to the Tesseract language data folder (`tessdata`) |
| `OCRProcessor.UnicodeFont` | `PdfTrueTypeFont` used to embed Unicode characters in the output |
| `OCRProcessor.ImageEnhancementMode` | `OcrImageEnhancementMode` — controls pre-processing (Enhance, EnhanceAndInclude, None) |
| `OCRSettings.Language` | `Languages` enum — sets the OCR recognition language (e.g., `Languages.English`) |
| `OCRSettings.TesseractVersion` | `TesseractVersion` enum — selects engine version (3.05, 4.0, 5.0) |
| `OCRSettings.PageSegment` | `PageSegMode` enum — controls page layout analysis mode |
| `OCRSettings.OCREngineMode` | `OCREngineMode` enum — selects OCR backend (LSTMOnly, TesseractOnly, Default) |
| `OCRSettings.Regions` | `List<PageRegion>` — restricts OCR to specific rectangular regions per page |
| `OCRSettings.WhiteList` | String of characters the OCR engine is allowed to recognise |
| `OCRSettings.BlackList` | String of characters to exclude from OCR recognition |
| `OCRSettings.Conformance` | `PdfConformanceLevel` — conformance of the output PDF when converting image to PDF |
| `OCRSettings.TempFolder` | Custom path for temporary files created during OCR processing |
| `OCRSettings.Performance` | `Performance` enum — `Rapid` (fast), `Fast` (balanced), `Slow` (best accuracy) |
| `OcrImageEnhancementOptions.IsGrayscaleEnabled` | Convert image to grayscale before OCR |
| `OcrImageEnhancementOptions.IsDeskewEnabled` | Correct skewed/tilted text before OCR |
| `OcrImageEnhancementOptions.IsDenoiseEnabled` | Remove noise/speckles from image before OCR |
| `OcrImageEnhancementOptions.IsConstrastEnabled` | Enhance image contrast before OCR |
| `OcrImageEnhancementOptions.IsBinarizeEnabled` | Binarize image to black-and-white before OCR |
| `OCRLayoutResult` | Holds the structured OCR output: pages → lines → words with bounding rectangles |
| `OCRLayoutResult.Pages` | Collection of `OCRPage` objects, one per PDF page |
| `OCRLineCollection` | Collection of `Line` objects on a page |
| `Line.Text` | The recognised text for a single line |
| `Line.Rectangle` | Bounding `RectangleF` of the recognised line on the page |
| `PageRegion.PageIndex` | Zero-based index of the page to apply the region to |
| `PageRegion.PageRegions` | Array of `RectangleF` that define the zones to OCR on that page |

---

## Page Segmentation Modes

| Mode | Description |
| --- | --- |
| `OsdOnly` | Detect orientation and script only; no OCR |
| `AutoOsd` | Automatic layout analysis with orientation and script detection |
| `AutoOnly` | Automatic layout analysis without orientation detection |
| `Auto` | Fully automatic page layout analysis (default) |
| `SingleColumn` | Process as a single column of text |
| `SingleBlock` | Process as a single block of text and graphics |
| `SingleLine` | Process as a single line |
| `SingleWord` | Process as a single word |
| `SparseText` | Recognise sparse text scattered across the page |
| `RawLine` | Single line with no layout analysis |

> **Note:** Page segmentation mode requires Tesseract version 4.0 or above.

---

## Notes

- Always wrap `OCRProcessor` in a `using` block — it holds unmanaged Tesseract resources.
- Starting with v21.1.x, TesseractBinaries and Tessdata paths are added automatically from the NuGet package; explicit paths are not required.
- `PerformOCR` only returns text added by OCR; pre-existing text in the PDF is not included — use [extract-text.md](extract-text.md) for that.
- OCR accuracy is best with images at 300 DPI or higher. Use lossless compression (ZIP or CCITT Group 4) for scanned images.
- For multi-language OCR, set `OCRSettings.Language` to a `+`-separated list of language codes (e.g., `"eng+deu+fra"`).

---

## Related

- [extract-text.md](extract-text.md)
- [conformance.md](conformance.md)
- [document-structure.md](document-structure.md)
- ../SKILL.md

## Official documentation

- <https://help.syncfusion.com/document-processing/pdf/pdf-library/net/working-with-ocr/working-with-ocr>
- <https://help.syncfusion.com/document-processing/pdf/pdf-library/net/working-with-ocr/features>
