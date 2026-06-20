# PDF Conformance (PDF/A, PDF/X)

Short reference for producing PDF files that meet PDF/A or PDF/X conformance using Syncfusion .NET PDF Library.

*Note: For document creation, loading, and save/close patterns, see [document-structure.md](document-structure.md).*

---
**Common namespaces:**

```csharp
using Syncfusion.Pdf;
using Syncfusion.Pdf.Graphics;
using Syncfusion.Pdf.Interactive;
using Syncfusion.Pdf.Parsing;
using SkiaSharp;
using Syncfusion.Drawing;
```
**Common Instruction for PDF to PDF/A:**
```
To convert an existing PDF document to the PDFA document in .NET Core, you need to substitute the non-embedded fonts in the input document.
``` 

## Overview

PDF conformance profiles (PDF/A, PDF/X) impose restrictions on fonts, color spaces, metadata, and embedded resources to ensure long-term reproducibility or print readiness. Syncfusion provides save options to produce conformance-compliant output when the required resources (embedded fonts, ICC profiles, metadata) are supplied.

Official user guide: <https://help.syncfusion.com/document-processing/pdf/pdf-library/net/working-with-pdf-conformance>

## Save as PDF/A (example)

```csharp
using Syncfusion.Pdf;
using Syncfusion.Pdf.Graphics;

// Create document with PDF/A-1b conformance
PdfDocument document = new PdfDocument(PdfConformanceLevel.Pdf_A1B);
// Add a page to the document
PdfPage page = document.Pages.Add();
// Create PDF graphics for the page
PdfGraphics graphics = page.Graphics;

// Load the TrueType font from the local file
FileStream fontStream = new FileStream("Arial.ttf", FileMode.Open, FileAccess.Read);
PdfFont font = new PdfTrueTypeFont(fontStream, 14);

// Draw the text
graphics.DrawString("Hello World!!!", font, PdfBrushes.Black, new Syncfusion.Drawing.PointF(0, 0));

// Save and close the document
document.Save("output-pdfa.pdf");
document.Close(true);
```

## Convert an existing PDF to PDF/A on save

```csharp
using Syncfusion.Pdf;
using Syncfusion.Pdf.Parsing;

// Load the PDF document
PdfLoadedDocument loadedDocument = new PdfLoadedDocument("input.pdf");

// Convert to PDF/A conformance
loadedDocument.ConvertToPDFA(PdfConformanceLevel.Pdf_A1B);

// Save and close the document
loadedDocument.Save("converted-pdfa.pdf");
loadedDocument.Close(true);
```

**Note:** To convert PDFs to PDF/A on cross-platform environments, include the [Syncfusion.Pdf.Imaging.Net.Core](https://www.nuget.org/packages/Syncfusion.Pdf.Imaging.Net.Core) package.

## Font subsetting during PDF to PDF/A conversion

Optimize PDF/A document size by embedding only the required font glyphs during conversion using the `SubsetFonts` property:

```csharp
using Syncfusion.Pdf;
using Syncfusion.Pdf.Parsing;
using SkiaSharp;

// Load an existing PDF document
PdfLoadedDocument loadedDocument = new PdfLoadedDocument("input.pdf");

// Subscribe to font substitution event for .NET Core environments
loadedDocument.SubstituteFont += LoadedDocument_SubstituteFont;

// Create conformance options with font subsetting
PdfConformanceOptions options = new PdfConformanceOptions();
options.ConformanceLevel = PdfConformanceLevel.Pdf_A1B;
options.SubsetFonts = true;

// Convert to PDF/A conformance
loadedDocument.ConvertToPDFA(options);

// Font substitution event handler for .NET Core
static void LoadedDocument_SubstituteFont(object sender, PdfFontEventArgs args)
{
    // Get the font name
    string fontName = args.FontName.Split(',')[0];
    // Get the font style
    PdfFontStyle fontStyle = args.FontStyle;
    SKFontStyle sKFontStyle = SKFontStyle.Normal;

    if (fontStyle != PdfFontStyle.Regular)
    {
        if (fontStyle == PdfFontStyle.Bold)
            sKFontStyle = SKFontStyle.Bold;
        else if (fontStyle == PdfFontStyle.Italic)
            sKFontStyle = SKFontStyle.Italic;
        else if (fontStyle == (PdfFontStyle.Italic | PdfFontStyle.Bold))
            sKFontStyle = SKFontStyle.BoldItalic;
    }

    // Create typeface and get font stream
    SKTypeface typeface = SKTypeface.FromFamilyName(fontName, sKFontStyle);
    SKStreamAsset typeFaceStream = typeface.OpenStream();
    MemoryStream memoryStream = null;

    if (typeFaceStream != null && typeFaceStream.Length > 0)
    {
        // Create font data from typeface stream
        byte[] fontData = new byte[typeFaceStream.Length];
        typeFaceStream.Read(fontData, typeFaceStream.Length);
        typeFaceStream.Dispose();
        
        // Create memory stream from font data
        memoryStream = new MemoryStream(fontData);
    }

    // Set the font stream to the event args
    args.FontStream = memoryStream;
}
```

**Notes:**

- Font subsetting is supported only in .NET Core for PDF/A conversion
- The `SubsetFonts` property reduces file size by including only glyphs actually used in the document
- For .NET Core environments, implement the `SubstituteFont` event handler to handle non-embedded fonts
- Requires `SkiaSharp` package for font handling in .NET Core

## PDF/A to PDF conversion

Convert an existing PDF/A conformance document back to a standard PDF document using the `RemoveConformance()` method:

```csharp
using Syncfusion.Pdf;
using Syncfusion.Pdf.Parsing;

// Load a PDF/A document
PdfLoadedDocument document = new PdfLoadedDocument("input-pdfa.pdf");

// Remove PDF/A conformance
document.RemoveConformance();
```

**Notes:**

- Removes all PDF/A conformance restrictions and metadata
- Converts PDF/A back to a standard PDF format
- Useful when you need to modify restricted PDF/A documents

## PDF/X and printing profiles

Set the appropriate conformance level (PDF/X-1a, PDF/X-3) where supported and supply output intent ICC profiles. APIs and enum names vary by Syncfusion versions; consult the UG link above for exact option names.

## Notes

- Pass the `PdfConformanceLevel` directly to the `PdfDocument` constructor when creating new PDF/A documents.
- Enum values use underscores: `Pdf_A1B`, `Pdf_A2B`, `Pdf_A3B`, etc.
- Always embed TrueType fonts using `PdfTrueTypeFont` for PDF/A compliance.
- Use `ConvertToPDFA()` method on loaded documents to convert existing PDFs to PDF/A.
- Call `.Close(true)` after saving to properly cleanup resources.
- PDF/A requires embedded fonts, appropriate color spaces/ICC profiles, and specific metadata (XMP).
- API names and properties may vary between Syncfusion package versions — refer to the official user guide for exact signatures.
