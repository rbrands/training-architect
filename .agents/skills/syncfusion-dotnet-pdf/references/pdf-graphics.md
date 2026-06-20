# PDF Graphics

Guidelines for how to draw text, shapes, images on the PDF page using PDF graphics.

*Note: For document creation, loading, and save/close patterns, see [document-structure.md](document-structure.md).*

---

**Common namespaces:**

```csharp
using Syncfusion.Drawing;
using Syncfusion.Pdf;
using Syncfusion.Pdf.Graphics;
```

---

## Get graphics from PDF page

```csharp
PdfGraphics graphics = page.Graphics;
```

### Draw text

```csharp
PdfFont font = new PdfStandardFont(PdfFontFamily.Helvetica, 20);
//Draw text
graphics.DrawString("Hello World!!!", font, PdfBrushes.Black, new Syncfusion.Drawing.PointF(0, 0));
```

### Draw line

```csharp
//Initialize pen to draw the line
PdfPen pen = new PdfPen(PdfBrushes.Black, 5f);
//Create the line points
PointF point1 = new PointF(10, 10);
PointF point2 = new PointF(10, 100);
//Draw the line on the page
graphics.DrawLine(pen, point1, point2);
```

### Draw Curve (BezierCurve)

```csharp
PdfBezierCurve bezier = new PdfBezierCurve(new PointF(0, 0), new PointF(100, 50), new PointF(50, 50), new PointF(100, 100));
bezier.Draw(graphics, new PointF(10, 10));
```

### Draw Path

```csharp
PdfPath path = new PdfPath();
path.AddLine(new PointF(10, 100), new PointF(10, 200));
path.AddLine(new PointF(10, 200), new PointF(100, 100));
path.AddLine(new PointF(100, 100), new PointF(100, 200));
path.AddLine(new PointF(100, 200), new PointF(10, 100));
page.Graphics.DrawPath(PdfPens.Black, path);
```

### Draw Rectangle

```csharp
//Initialize PdfSolidBrush for drawing the rectangle
PdfSolidBrush brush = new PdfSolidBrush(Color.Green);
//Create PdfPen (Optional)
PdfPen pen = new PdfPen(Color.Blue, 10f);
//Set the bounds for rectangle
RectangleF bounds = new RectangleF(10, 10, 100, 50);
//Draw the rectangle on PDF document
page.Graphics.DrawRectangle(pen, brush, bounds); 
```

## Draw polygon

```csharp
PdfPen polyPen = new PdfPen(PdfBrushes.Brown, 10f);
PdfLinearGradientBrush gradient = new PdfLinearGradientBrush(
    new PointF(10, 100), new PointF(100, 200),
    new PdfColor(Color.Red), new PdfColor(Color.Green));
PointF[] points = {
    new PointF(10, 100), new PointF(10, 200), new PointF(100, 100),
    new PointF(100, 200), new PointF(55, 150)
};
page.Graphics.DrawPolygon(polyPen, gradient, points);
```

## Draw pie

```csharp
PdfPen piePen = new PdfPen(PdfBrushes.Brown, 5f);
piePen.LineJoin = PdfLineJoin.Round;
RectangleF pieRect = new RectangleF(10, 50, 200, 200);
page.Graphics.DrawPie(piePen, PdfBrushes.Green, pieRect, 180, 60);
```

## Draw arc

```csharp
PdfPen arcPen = new PdfPen(Color.Brown, 10f);
arcPen.LineCap = PdfLineCap.Square;
RectangleF arcBounds = new RectangleF(20, 40, 200, 200);
page.Graphics.DrawArc(arcPen, arcBounds, 270, 90);
```

## Draw bezier (direct)

```csharp
PdfPen bezPen = new PdfPen(PdfBrushes.Brown, 1f);
page.Graphics.DrawBezier(
    bezPen,
    new PointF(10, 10), new PointF(10, 50),
    new PointF(50, 80), new PointF(80, 10));
```

## Draw ellipse

```csharp
PdfSolidBrush ellBrush = new PdfSolidBrush(Color.Red);
page.Graphics.DrawEllipse(ellBrush, new RectangleF(10, 10, 200, 100));
```

## Dash patterns (dashed/dotted)

```csharp
PdfPen dashed = new PdfPen(Color.Black, 2f)
{
    DashStyle = PdfDashStyle.Custom,
    DashPattern = new float[] { 3, 2 }
};
page.Graphics.DrawLine(dashed, new PointF(20, 20), new PointF(200, 20));
page.Graphics.DrawRectangle(dashed, new RectangleF(20, 40, 180, 80));
```

## Paginate large shapes across pages

```csharp
RectangleF tall = new RectangleF(0, 0, 100, 1000);
PdfEllipse ellipse = new PdfEllipse(tall)
{
    Brush = PdfBrushes.Brown
};
PdfLayoutFormat format = new PdfLayoutFormat
{
    Break = PdfLayoutBreakType.FitPage,
    Layout = PdfLayoutType.Paginate
};
ellipse.Draw(page, 20, 20, format);
```

## Draw images

```csharp
//Load the image from the disk
FileStream imageStream = new FileStream("Autumn Leaves.jpg", FileMode.Open, FileAccess.Read);
PdfBitmap image = new PdfBitmap(imageStream);
//Draw the image
graphics.DrawImage(image, 0, 0);
```

### Apply image mask

```csharp
//Load the TIFF image
FileStream imageStream = new FileStream("image.tif", FileMode.Open, FileAccess.Read);
PdfTiffImage image = new PdfTiffImage(imageStream);
//Create masking image
FileStream maskStream = new FileStream("mask.bmp", FileMode.Open, FileAccess.Read);
PdfImageMask mask = new PdfImageMask(new PdfTiffImage(maskStream));
image.Mask = mask;
//Draw the image
graphics.DrawImage(image, 0, 0);
```

### Image pagination

```csharp
PdfBitmap image = new PdfBitmap(imageStream);
//Set layout property to make the element break across the pages
PdfLayoutFormat format = new PdfLayoutFormat();
format.Break = PdfLayoutBreakType.FitPage;
format.Layout = PdfLayoutType.Paginate;
//Draw image
image.Draw(page, 20, 400, format);
```

## Graphics state

The graphics state captures the current drawing context on a PDF page: current transformation matrix (CTM), clipping region, and other drawing parameters. In Syncfusion .NET PDF, calling PdfGraphics.Save() snapshots this state into a PdfGraphicsState object; calling PdfGraphics.Restore(state) rolls back to that snapshot. Use this pattern to apply transforms or clipping to only a portion of drawing without affecting subsequent operations.

### Typical pattern (scoped transforms)

```csharp
// 1) Snapshot
PdfGraphicsState state = page.Graphics.Save();
// 2) Apply transforms *only* for the following draw calls
page.Graphics.TranslateTransform(100, 100);
page.Graphics.RotateTransform(90);
// 3) Issue drawing
page.Graphics.DrawString("Rotated here", new PdfStandardFont(PdfFontFamily.Helvetica, 12), PdfBrushes.Black, new PointF(0, 0));
// 4) Revert to the previous context
page.Graphics.Restore(state);
```

### Common operations that modify state

#### Translate (move origin)

Moves the drawing origin; helpful to localize coordinates before drawing.

```csharp
page.Graphics.TranslateTransform(120, 60);
```

#### Rotate (degrees)

Rotates the axes; often used for watermarks or slanted labels.

```csharp
page.Graphics.RotateTransform(-40);
```

#### Scale (uniform or non-uniform)

Resizes subsequent drawings in X and Y. Combine with Save/Restore to avoid affecting later content,

```csharp
page.Graphics.ScaleTransform(0.75f, 1.25f);
```
