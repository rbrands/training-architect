# PDF Watermarks

Add text and image watermarks to PDF documents using Syncfusion .NET PDF Library.

*Note: For document creation, loading, and save/close patterns, see [document-structure.md](document-structure.md).*

---
**Common namespaces:**

```csharp
using Syncfusion.Drawing;
using Syncfusion.Pdf;
using Syncfusion.Pdf.Graphics;
using Syncfusion.Pdf.Parsing;
```

---

## Add text watermark to new PDF

Embed a text watermark with rotation and transparency to a new PDF document.

```csharp
using Syncfusion.Drawing;
using Syncfusion.Pdf.Graphics;

// Get graphics object
PdfGraphics graphics = pdfPage.Graphics;

// Set font
PdfFont font = new PdfStandardFont(PdfFontFamily.Helvetica, 20);

// Add watermark with transparency and rotation
PdfGraphicsState state = graphics.Save();
graphics.SetTransparency(0.25f);      // 25% opacity
graphics.RotateTransform(-40);         // -40 degree rotation
graphics.DrawString("Imported using Essential PDF", font, PdfPens.Red, 
    PdfBrushes.Red, new PointF(-150, 450));
graphics.Restore(state);               // Restore graphics state
```

---

## Add text watermark to existing PDF

Add a watermark to an already created PDF document.

```csharp
using Syncfusion.Drawing;
using Syncfusion.Pdf;
using Syncfusion.Pdf.Graphics;
using Syncfusion.Pdf.Parsing;

// Get graphics for the page
PdfGraphics graphics = loadedPage.Graphics;

// Set font
PdfFont font = new PdfStandardFont(PdfFontFamily.Helvetica, 20);

// Add watermark
PdfGraphicsState state = graphics.Save();
graphics.SetTransparency(0.25f);           // 25% opacity
graphics.RotateTransform(-40);             // -40 degree rotation
graphics.DrawString("Imported using Essential PDF", font, PdfPens.Red, 
    PdfBrushes.Red, new PointF(-150, 450));
graphics.Restore(state);

```

---

## Add image watermark to new PDF

Embed an image as a watermark with transparency control.

```csharp
using Syncfusion.Drawing;
using Syncfusion.Pdf;
using Syncfusion.Pdf.Graphics;


// Get graphics object
PdfGraphics graphics = pdfPage.Graphics;

// Load image
FileStream imageStream = new FileStream("Image.jpeg", FileMode.Open, FileAccess.Read);
PdfImage image = new PdfBitmap(imageStream);

// Add image watermark with transparency
PdfGraphicsState state = graphics.Save();
graphics.SetTransparency(0.25f);           // 25% opacity
graphics.DrawImage(image, new PointF(0, 0), pdfPage.Graphics.ClientSize);
graphics.Restore(state);

```

---

## Add image watermark to existing PDF

Add an image watermark to an already created PDF.

```csharp
using Syncfusion.Drawing;
using Syncfusion.Pdf;
using Syncfusion.Pdf.Graphics;
using Syncfusion.Pdf.Parsing;

// Get graphics for the page
PdfGraphics graphics = loadedPage.Graphics;

// Load image
FileStream imageStream = new FileStream("Image.jpeg", FileMode.Open, FileAccess.Read);
PdfImage image = new PdfBitmap(imageStream);

// Add image watermark with transparency
PdfGraphicsState state = graphics.Save();
graphics.SetTransparency(0.25f);           // 25% opacity
graphics.DrawImage(image, new PointF(0, 0), loadedPage.Graphics.ClientSize);
graphics.Restore(state);

```

---

## Add watermark annotation

Create a watermark annotation that prints at fixed size and position regardless of page dimensions.

```csharp
using Syncfusion.Drawing;
using Syncfusion.Pdf;
using Syncfusion.Pdf.Graphics;
using Syncfusion.Pdf.Interactive;
using Syncfusion.Pdf.Parsing;


// Create watermark annotation
PdfWatermarkAnnotation watermark = new PdfWatermarkAnnotation(
    new RectangleF(100, 100, 200, 50));

// Set transparency
watermark.Opacity = 0.5f;

// Create appearance (custom graphics for watermark)
watermark.Appearance.Normal.Graphics.DrawString("Watermark Text",
    new PdfStandardFont(PdfFontFamily.Helvetica, 20),
    PdfBrushes.Red,
    new RectangleF(0, 0, 200, 50),
    new PdfStringFormat(PdfTextAlignment.Center, PdfVerticalAlignment.Middle));

// Add annotation to page
lpage.Annotations.Add(watermark);

```

---

## Remove watermark annotation

Delete watermark annotations from a PDF document.

```csharp
using Syncfusion.Pdf;
using Syncfusion.Pdf.Parsing;

// Load the PDF document
PdfLoadedDocument loadedDocument = new PdfLoadedDocument("Input.pdf");

// Iterate through pages and remove watermark annotations
foreach (PdfPageBase page in loadedDocument.Pages)
{
    // Iterate backwards to safely remove items
    for (int i = page.Annotations.Count - 1; i >= 0; i--)
    {
        // Check if annotation is a watermark
        if (page.Annotations[i] is PdfLoadedWatermarkAnnotation)
        {
            // Remove the watermark annotation
            page.Annotations.RemoveAt(i);
        }
    }
}

```

---

## Graphics State Management

Use `PdfGraphicsState` to save and restore graphics context for watermarks.

```csharp
// Save current graphics state (transformations, transparency, clipping)
PdfGraphicsState state = graphics.Save();

// Apply changes (rotation, transparency, etc.)
graphics.SetTransparency(0.25f);
graphics.RotateTransform(-40);
graphics.DrawString("Watermark", font, brush, point);

// Restore original state
graphics.Restore(state);

// Subsequent drawing uses original settings
```

---

## Transparency Levels

| Opacity | Transparency | Visibility |
| --- | --- | --- |
| 1.0 | 0% | Fully opaque (not transparent) |
| 0.75 | 25% | Slightly visible |
| 0.5 | 50% | Half transparent |
| 0.25 | 75% | Faintly visible |
| 0.1 | 90% | Very faint |

---

## Rotation Tips

```csharp
// Common rotation angles
graphics.RotateTransform(-45);   // Diagonal (typical watermark angle)
graphics.RotateTransform(-30);   // Moderate angle
graphics.RotateTransform(0);     // No rotation
graphics.RotateTransform(90);    // Vertical
```

---

## Watermark Annotation Properties

| Property | Type | Purpose |
| --- | --- | --- |
| `Opacity` | float | Transparency level (0.0-1.0) |
| `Bounds` | `RectangleF` | Position and size on page |
| `Appearance.Normal.Graphics` | `PdfGraphics` | Custom graphics content for watermark |

---

## Use Cases

- **Document Branding**: Add company logo as watermark
- **Confidentiality**: Mark documents as "CONFIDENTIAL" or "DRAFT"
- **Version Control**: Watermark with version numbers or dates
- **Proof Copies**: Mark "NOT FOR PRODUCTION" on preview PDFs
- **Page Numbers**: Add watermark with page number information
