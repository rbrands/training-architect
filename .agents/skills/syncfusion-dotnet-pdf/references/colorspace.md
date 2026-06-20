# PDF Color Spaces

Short reference for working with color spaces in Syncfusion .NET PDF Library.

*Note: For document creation, loading, and save/close patterns, see [document-structure.md](document-structure.md).*

---

**Common namespaces:**

```csharp
using Syncfusion.Drawing;
using Syncfusion.Pdf;
using Syncfusion.Pdf.ColorSpace;
using Syncfusion.Pdf.Graphics;
```

---

## Overview

Color spaces control how colors are represented in PDF content and images. Typical device color spaces include DeviceGray, DeviceRGB, and DeviceCMYK; PDF also supports calibrated and ICC-based color spaces for precise color management.

Official user guide: <https://help.syncfusion.com/document-processing/pdf/pdf-library/net/working-with-colorspace>

## Create common color spaces

```csharp
// Device Gray (example)
PdfDocument document = new PdfDocument();
PdfPage page = document.Pages.Add();
PdfGraphics g = page.Graphics;

// Use a gray brush / pen when drawing
PdfBrush grayBrush = new PdfSolidBrush(PdfColor.Gray);
g.DrawString("Gray text", new PdfStandardFont(PdfFontFamily.Helvetica, 12), grayBrush, new PointF(10, 10));
```

```csharp
// Device RGB (example)
PdfBrush rgbBrush = new PdfSolidBrush(new PdfColor(255, 0, 0)); // red
g.DrawRectangle(rgbBrush, new RectangleF(10, 30, 100, 40));
```

```csharp
// Device CMYK (example)
// Many drawing APIs accept CMYK via color converters or specialized constructors.
PdfColor cmyk = PdfColor.FromCmyk(0f, 1f, 1f, 0f); // example CMYK color
PdfBrush cmykBrush = new PdfSolidBrush(cmyk);
g.DrawString("CMYK text", new PdfStandardFont(PdfFontFamily.Helvetica, 10), cmykBrush, new PointF(10, 80));
```

## Images and color conversion

When importing images, you can inspect or convert the image color space (API names vary by library version). Typical steps:

- Load image (Bitmap, PdfBitmap, or PdfImage)
- Check image color space / pixel format
- Convert to desired color space or re-encode with different color profile

```csharp
// Pseudo-code: inspect and save image with desired encoding
PdfBitmap image = new PdfBitmap("input.jpg");
// If conversion APIs are available, convert image to DeviceGray/DeviceRGB/DeviceCMYK
// image = image.ConvertToGrayscale(); // example helper (API varies)
page.Graphics.DrawImage(image, new RectangleF(10, 110, 200, 150));
```

## Notes

- Use ICC/Calibrated color spaces for color-managed workflows.
- Some APIs differ between Syncfusion package versions; consult the official guide above for exact class/method names and overloads.

## Related

- [pdf-graphics.md](pdf-graphics.md)
- [compress-pdf.md](compress-pdf.md)
- ../SKILL.md

## Official documentation

- <https://help.syncfusion.com/document-processing/pdf/pdf-library/net/working-with-colorspace>

<!-- Examples are concise and may need exact API names from your installed Syncfusion package/version -->
