# PDF Images

Insert, draw, replace, remove, clip, transform, and paginate images in PDF documents using Syncfusion .NET PDF Library. Covers raster images (BMP, JPEG, PNG, GIF, TIFF, ICO), image masking, TIFF-to-PDF conversion, and unit conversion.

*Note: For document creation, loading, and save/close patterns, see [document-structure.md](document-structure.md). For extracting images from existing PDFs, see [extract-image.md](extract-image.md). For graphics state and transforms, see [pdf-graphics.md](pdf-graphics.md).*

---

**NuGet packages:**

`Syncfusion.Pdf.Net.Core` - (PNG, JPEG only — .NET Core / ASP.NET Core)
`Syncfusion.Pdf.Imaging.Net.Core` - (BMP, GIF, TIFF, ICO, image masking, remove image — .NET Core / ASP.NET Core)

**Common namespaces:**

```csharp
using Syncfusion.Drawing;
using Syncfusion.Pdf;
using Syncfusion.Pdf.Graphics;
using Syncfusion.Pdf.Parsing;
```

---

## Insert an image in a new PDF

Load an image from a file stream and draw it onto the page using `PdfGraphics.DrawImage`.

```csharp
PdfDocument doc = new PdfDocument();
PdfPage page = doc.Pages.Add();
PdfGraphics graphics = page.Graphics;

// Load image from disk
FileStream imageStream = new FileStream("Autumn Leaves.jpg", FileMode.Open, FileAccess.Read);
PdfBitmap image = new PdfBitmap(imageStream);

// Draw at origin with the image's natural size
graphics.DrawImage(image, 0, 0);
```

---

## Insert an image in an existing PDF

```csharp
PdfLoadedDocument doc = new PdfLoadedDocument("Input.pdf");
PdfLoadedPage page = doc.Pages[0] as PdfLoadedPage;
PdfGraphics graphics = page.Graphics;

FileStream imageStream = new FileStream("Autumn Leaves.jpg", FileMode.Open, FileAccess.Read);
PdfBitmap image = new PdfBitmap(imageStream);

// Draw at a specific position
graphics.DrawImage(image, 0, 0);
```

---

## Draw an image with explicit bounds

Pass a `RectangleF` to scale or position the image precisely on the page.

```csharp
PdfDocument doc = new PdfDocument();
PdfPage page = doc.Pages.Add();

FileStream imageStream = new FileStream("Input.png", FileMode.Open, FileAccess.Read);
PdfBitmap image = new PdfBitmap(imageStream);

// Draw at (x=50, y=100) scaled to 200 × 150 points
page.Graphics.DrawImage(image, new RectangleF(50, 100, 200, 150));
```

---

## Image masking (TIFF with mask)

Use `PdfImageMask` to apply a binary or soft mask to a TIFF image. Requires `Syncfusion.Pdf.Imaging.Net.Core`.

```csharp
PdfDocument doc = new PdfDocument();
PdfPage page = doc.Pages.Add();
PdfGraphics graphics = page.Graphics;

// Load the source TIFF image
FileStream imageStream = new FileStream("image.tif", FileMode.Open, FileAccess.Read);
PdfTiffImage image = new PdfTiffImage(imageStream);

// Load the mask image and attach it
FileStream maskStream = new FileStream("mask.bmp", FileMode.Open, FileAccess.Read);
PdfImageMask mask = new PdfImageMask(new PdfTiffImage(maskStream));
image.Mask = mask;

graphics.DrawImage(image, 0, 0);
```

---

## Replace an image in an existing PDF

Use `PdfPageBase.ReplaceImage(int index, PdfImage newImage)` to swap an image by its zero-based index on the page.

```csharp
PdfLoadedDocument loadedDocument = new PdfLoadedDocument("Input.pdf");

FileStream imageStream = new FileStream("Autumn Leaves.jpg", FileMode.Open, FileAccess.Read);
PdfBitmap bmp = new PdfBitmap(imageStream);

// Replace the first image (index 0) on the first page
loadedDocument.Pages[0].ReplaceImage(0, bmp);
```

---

## Remove an image from an existing PDF

Use `PdfPageBase.RemoveImage(PdfImageInfo)` to delete an image. Requires `Syncfusion.Pdf.Imaging.Net.Core`.

```csharp
PdfLoadedDocument loadedDocument = new PdfLoadedDocument("Input.pdf");
PdfPageBase pageBase = loadedDocument.Pages[0];

// Get image metadata from the page
PdfImageInfo[] imageInfo = loadedDocument.Pages[0].GetImagesInfo();

// Remove the first image
pageBase.RemoveImage(imageInfo[0]);
```

---

## Image pagination across pages

Use `PdfLayoutFormat` with `PdfLayoutType.Paginate` so a tall image automatically continues on the next page.

```csharp
PdfDocument doc = new PdfDocument();
PdfPage page = doc.Pages.Add();

FileStream imageStream = new FileStream("Autumn Leaves.jpg", FileMode.Open, FileAccess.Read);
PdfBitmap image = new PdfBitmap(imageStream);

PdfLayoutFormat format = new PdfLayoutFormat();
format.Break  = PdfLayoutBreakType.FitPage;
format.Layout = PdfLayoutType.Paginate;

// Start drawing at y=400; overflows automatically paginate
image.Draw(page, 20, 400, format);
```

---

## Clipping an image to a region

Use `PdfGraphics.SetClip` with `Save`/`Restore` to render only the portion of an image that falls inside the clip rectangle.

```csharp
PdfDocument document = new PdfDocument();
PdfPage page = document.Pages.Add();
PdfGraphics graphics = page.Graphics;

FileStream imageStream = new FileStream("Input.png", FileMode.Open, FileAccess.Read);
PdfBitmap image = new PdfBitmap(imageStream);

// Save graphics state before clipping
PdfGraphicsState state = graphics.Save();

// Apply clip region — only content inside this rectangle will be drawn
RectangleF clipRect = new RectangleF(50, 50, 200, 100);
graphics.SetClip(clipRect);

// Only the clipped portion of the image is visible
graphics.DrawImage(image, new RectangleF(40, 60, 150, 80));

// Restore state to remove the clip for subsequent drawing
graphics.Restore(state);

// Draw the full image (no clip active)
graphics.DrawImage(image, new RectangleF(60, 160, 150, 80));
```

---

## Apply transparency and rotation to an image

Use `SetTransparency` and `RotateTransform` (wrapped in `Save`/`Restore`) to render a semi-transparent rotated image.

```csharp
PdfDocument doc = new PdfDocument();
PdfPage page = doc.Pages.Add();

FileStream imageStream = new FileStream("input.jpg", FileMode.Open, FileAccess.Read);
PdfBitmap image = new PdfBitmap(imageStream);

// Save state
PdfGraphicsState state = page.Graphics.Save();

// Move origin, then apply transparency and rotation
page.Graphics.TranslateTransform(20, 100);
page.Graphics.SetTransparency(0.5f);   // 50% opacity
page.Graphics.RotateTransform(-45);    // rotate 45° counter-clockwise

image.Draw(page, 0, 0);

// Restore state
page.Graphics.Restore(state);
```

---

## Unit conversion for image placement

Use `PdfUnitConverter` to translate pixel dimensions into PDF points so images are placed at exact sizes.

```csharp
PdfDocument document = new PdfDocument();

FileStream stream = new FileStream("Image.png", FileMode.Open, FileAccess.Read);
PdfBitmap image = new PdfBitmap(stream);

// Add a section sized to match the image exactly
PdfSection section = document.Sections.Add();

PdfUnitConverter converter = new PdfUnitConverter();
// Convert the image's pixel dimensions to PDF points
SizeF size = converter.ConvertFromPixels(image.PhysicalDimension, PdfGraphicsUnit.Point);

section.PageSettings.Size = size;
section.PageSettings.Margins.All = 0;

// Set landscape if the image is wider than tall
if (image.Width > image.Height)
    section.PageSettings.Orientation = PdfPageOrientation.Landscape;

PdfPage page = section.Pages.Add();
page.Graphics.DrawImage(image, 0, 0);
```

---

## Convert multi-page TIFF to PDF

Iterate over each frame of a multi-frame TIFF and render it to a separate PDF page. Requires `Syncfusion.Pdf.Imaging.Net.Core`.

```csharp
PdfDocument document = new PdfDocument();
document.PageSettings.Margins.All = 0;

FileStream imageStream = new FileStream("image.tiff", FileMode.Open, FileAccess.Read);
PdfTiffImage tiffImage = new PdfTiffImage(imageStream);

int frameCount = tiffImage.FrameCount;
for (int i = 0; i < frameCount; i++)
{
    PdfPage page = document.Pages.Add();
    PdfGraphics graphics = page.Graphics;

    // Activate the current TIFF frame
    tiffImage.ActiveFrame = i;

    // Stretch frame to fill the full page
    graphics.DrawImage(tiffImage, 0, 0,
        page.GetClientSize().Width,
        page.GetClientSize().Height);
}
```

---

## Key APIs

| Member | Description |
| --- | --- |
| `PdfBitmap(Stream)` | Loads a raster image (JPEG, PNG, BMP, GIF, ICO) from a stream |
| `PdfTiffImage(Stream)` | Loads a TIFF image (single or multi-frame) from a stream |
| `PdfMetafile` | Loads a vector EMF/WMF image; graphics are converted to native PDF (Windows only) |
| `PdfImage` | Abstract base class for `PdfBitmap` and `PdfMetafile` |
| `PdfGraphics.DrawImage(PdfImage, float, float)` | Draws an image at (x, y) using the image's natural size |
| `PdfGraphics.DrawImage(PdfImage, RectangleF)` | Draws an image scaled to fit the given rectangle |
| `PdfBitmap.Draw(PdfPage, float, float, PdfLayoutFormat)` | Draws an image with pagination support |
| `PdfBitmap.Width` / `.Height` | Image dimensions in pixels |
| `PdfBitmap.PhysicalDimension` | Image size as a `SizeF` in its native unit (used with `PdfUnitConverter`) |
| `PdfTiffImage.FrameCount` | Number of frames in a multi-frame TIFF |
| `PdfTiffImage.ActiveFrame` | Gets or sets the zero-based index of the currently active TIFF frame |
| `PdfImageMask(PdfTiffImage)` | Creates a binary/soft mask from a TIFF image |
| `PdfTiffImage.Mask` | Assigns a `PdfImageMask` to a TIFF image before drawing |
| `PdfPageBase.ReplaceImage(int, PdfImage)` | Replaces an existing image at the given zero-based index on the page |
| `PdfPageBase.RemoveImage(PdfImageInfo)` | Removes an image identified by its `PdfImageInfo` from the page |
| `PdfPageBase.GetImagesInfo()` | Returns `PdfImageInfo[]` with bounds, index, and stream for each image on the page |
| `PdfLayoutFormat` | Controls pagination; set `Layout = PdfLayoutType.Paginate` and `Break = PdfLayoutBreakType.FitPage` |
| `PdfGraphics.SetClip(RectangleF)` | Restricts subsequent drawing to the specified rectangle |
| `PdfGraphics.SetTransparency(float)` | Sets drawing opacity; `1.0` = fully opaque, `0.0` = fully transparent |
| `PdfGraphics.RotateTransform(float)` | Rotates the coordinate system by the given angle in degrees |
| `PdfGraphics.TranslateTransform(float, float)` | Shifts the drawing origin by (dx, dy) |
| `PdfGraphics.Save()` | Snapshots the current graphics state; returns `PdfGraphicsState` |
| `PdfGraphics.Restore(PdfGraphicsState)` | Reverts to a previously saved graphics state |
| `PdfUnitConverter` | Converts between pixels, points, inches, and other units |
| `PdfUnitConverter.ConvertFromPixels(SizeF, PdfGraphicsUnit)` | Converts a pixel-based size to the target graphics unit (e.g., `PdfGraphicsUnit.Point`) |
| `PdfGraphicsUnit` | Enum: `Point`, `Pixel`, `Inch`, `Millimeter`, `Centimeter` |

---

## Notes

- For image formats beyond PNG and JPEG in .NET Core, add `Syncfusion.Pdf.Imaging.Net.Core` to your project.
- `PdfMetafile` (EMF/WMF) is a Windows-only feature; it is not supported in cross-platform (.NET Core) builds.
- Image masking and `RemoveImage` both require `Syncfusion.Pdf.Imaging.Net.Core` in ASP.NET Core.
- Always wrap `SetClip` / `SetTransparency` / `RotateTransform` between `Save()` and `Restore()` to avoid affecting subsequent drawing operations.
- Use `PdfUnitConverter` when image positioning must match real-world measurements (e.g., millimetre-accurate layouts).
- On Ubuntu ARM64, add `SkiaSharp.NativeAssets.Linux` to avoid missing native asset errors.

---

## Related

- [extract-image.md](extract-image.md)
- [pdf-graphics.md](pdf-graphics.md)
- [shapes.md](shapes.md)
- [brushes.md](brushes.md)
- [watermarks.md](watermarks.md)
- [document-structure.md](document-structure.md)
- ../SKILL.md

## Official documentation

- <https://help.syncfusion.com/document-processing/pdf/pdf-library/net/working-with-images>
