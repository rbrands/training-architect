# PDF Shapes

Draw various shapes in PDF documents using Syncfusion .NET PDF Library.

*Note: For document creation, loading, and save/close patterns, see [document-structure.md](document-structure.md).*

---

**Common namespaces:**

```csharp
using Syncfusion.Drawing;
using Syncfusion.Pdf;
using Syncfusion.Pdf.Graphics;
```

---

## Draw a line

Use the `DrawLine` method of `PdfGraphics` to create straight lines.

```csharp
using Syncfusion.Drawing;
using Syncfusion.Pdf.Graphics;

// Initialize pen to draw the line
PdfPen pen = new PdfPen(PdfBrushes.Black, 5f);

// Create line points
PointF point1 = new PointF(10, 10);
PointF point2 = new PointF(10, 100);

// Draw the line
page.Graphics.DrawLine(pen, point1, point2);
```

### Draw line in existing PDF

```csharp
PdfPen pen = new PdfPen(PdfBrushes.Black, 5f);
loadedPage.Graphics.DrawLine(pen, new PointF(10, 10), new PointF(10, 100));
```

---

## Draw a rectangle

Use the `DrawRectangle` method to create filled or outlined rectangles.

```csharp
using Syncfusion.Drawing;
using Syncfusion.Pdf;
using Syncfusion.Pdf.Graphics;


// Initialize brush for drawing the rectangle
PdfSolidBrush brush = new PdfSolidBrush(Color.Green);

// Set the bounds for rectangle
RectangleF bounds = new RectangleF(10, 10, 100, 50);

// Draw the rectangle
page.Graphics.DrawRectangle(brush, bounds);

```

---

## Draw an ellipse

Use the `DrawEllipse` method to create filled or outlined ellipses.

```csharp
using Syncfusion.Drawing;
using Syncfusion.Pdf;
using Syncfusion.Pdf.Graphics;

// Initialize brush for drawing the ellipse
PdfSolidBrush brush = new PdfSolidBrush(Color.Red);

// Draw ellipse on the page
page.Graphics.DrawEllipse(brush, new RectangleF(10, 10, 200, 100));
```

---

## Draw a polygon

Use the `DrawPolygon` method to create multi-sided shapes with gradient or solid brushes.

```csharp
using Syncfusion.Drawing;
using Syncfusion.Pdf;
using Syncfusion.Pdf.Graphics;

// Initialize pen to draw the polygon
PdfPen pen = new PdfPen(PdfBrushes.Brown, 10f);

// Initialize gradient brush
PdfLinearGradientBrush brush = new PdfLinearGradientBrush(
    new PointF(10, 100), new PointF(100, 200),
    new PdfColor(Color.Red), new PdfColor(Color.Green));

// Create polygon points
PointF[] points = {
    new PointF(10, 100),
    new PointF(10, 200),
    new PointF(100, 100),
    new PointF(100, 200),
    new PointF(55, 150)
};

// Draw the polygon
page.Graphics.DrawPolygon(pen, brush, points);
```

---

## Draw a circle or arc

Use the `DrawArc` method to create curved shapes.

```csharp
using Syncfusion.Drawing;
using Syncfusion.Pdf;
using Syncfusion.Pdf.Graphics;

// Initialize pen for drawing arc
PdfPen pen = new PdfPen(Color.Brown, 10f);
pen.LineCap = PdfLineCap.Square;

// Set the bounds for arc
RectangleF bounds = new RectangleF(20, 40, 200, 200);

// Draw arc (270 degrees starting angle, 90 degrees sweep)
page.Graphics.DrawArc(pen, bounds, 270, 90);
```

---

## Draw a pie

Use the `DrawPie` method to create pie/wedge shapes.

```csharp
using Syncfusion.Drawing;
using Syncfusion.Pdf;
using Syncfusion.Pdf.Graphics;


// Initialize pen for drawing pie
PdfPen pen = new PdfPen(PdfBrushes.Brown, 5f);
pen.LineJoin = PdfLineJoin.Round;

// Set the bounds for pie
RectangleF rectangle = new RectangleF(10, 50, 200, 200);

// Draw pie (180 degrees starting angle, 60 degrees sweep)
page.Graphics.DrawPie(pen, PdfBrushes.Green, rectangle, 180, 60);

```

---

## Draw a Bezier curve

Use the `DrawBezier` method to create smooth curved lines with control points.

```csharp
using Syncfusion.Drawing;
using Syncfusion.Pdf;
using Syncfusion.Pdf.Graphics;

// Initialize pen to draw the bezier
PdfPen pen = new PdfPen(PdfBrushes.Brown, 1f);

// Draw Bezier with 4 control points
page.Graphics.DrawBezier(pen,
    new PointF(10, 10),    // Start point
    new PointF(10, 50),    // Control point 1
    new PointF(50, 80),    // Control point 2
    new PointF(80, 10));   // End point

```

---

## Draw a path

Use the `DrawPath` method to create complex shapes composed of multiple line and curve segments.

```csharp
using Syncfusion.Drawing;
using Syncfusion.Pdf;
using Syncfusion.Pdf.Graphics;

// Initialize a new PDF path
PdfPath path = new PdfPath();

// Add line path points
path.AddLine(new PointF(10, 100), new PointF(10, 200));
path.AddLine(new PointF(10, 200), new PointF(100, 100));
path.AddLine(new PointF(100, 100), new PointF(100, 200));
path.AddLine(new PointF(100, 200), new PointF(10, 100));

// Draw the path
page.Graphics.DrawPath(PdfPens.Black, path);

```

---

## Draw a curve (Bezier curve element)

Use the `PdfBezierCurve` class with `Draw` method for smooth curves.

```csharp
using Syncfusion.Drawing;
using Syncfusion.Pdf;
using Syncfusion.Pdf.Graphics;


// Create Bezier curve instance
PdfBezierCurve bezier = new PdfBezierCurve(
    new PointF(0, 0),      // Start point
    new PointF(100, 50),   // Control point 1
    new PointF(50, 50),    // Control point 2
    new PointF(100, 100)); // End point

// Draw the Bezier curve
bezier.Draw(page.Graphics, new PointF(10, 10));
```

---

## Apply dash pattern to lines

Use the `DashPattern` property to create dashed or dotted lines.

```csharp
using Syncfusion.Drawing;
using Syncfusion.Pdf;
using Syncfusion.Pdf.Graphics;

// Create custom dashed line pattern (5-point dash, 2-point gap)
float[] dashPattern = { 5, 2 };

// Create pen with dash pattern
PdfPen dashPen = new PdfPen(PdfBrushes.Black, 2);
dashPen.DashStyle = PdfDashStyle.Custom;
dashPen.DashPattern = dashPattern;

// Draw line with dash pattern
page.Graphics.DrawLine(dashPen, new PointF(10, 10), new PointF(300, 10));

```

---

## Paginate large shapes across pages

Use `PdfLayoutFormat` with `PdfLayoutType.Paginate` to break shapes across multiple pages.

```csharp
using Syncfusion.Drawing;
using Syncfusion.Pdf;
using Syncfusion.Pdf.Graphics;


// Set bounds for large ellipse (1000 pixels tall)
RectangleF rect = new RectangleF(0, 0, 100, 1000);

// Create ellipse
PdfEllipse ellipse = new PdfEllipse(rect);

// Set layout property to paginate the ellipse across pages
PdfLayoutFormat format = new PdfLayoutFormat();
format.Break = PdfLayoutBreakType.FitPage;
format.Layout = PdfLayoutType.Paginate;

// Apply brush and draw
ellipse.Brush = PdfBrushes.Brown;
ellipse.Draw(page, 20, 20, format);

```

---

## Pen and Brush Properties

### PdfPen (for outlines)

- `Color` — Line color
- `Width` — Line width in points
- `DashStyle` — Dash pattern style (Solid, Dash, Dot, DashDot, etc.)
- `DashPattern` — Custom dash/gap pattern
- `LineCap` — Line endings (Butt, Round, Square)
- `LineJoin` — Line junction style (Bevel, Miter, Round)

### PdfBrush (for fills)

- `PdfSolidBrush` — Solid color fill
- `PdfLinearGradientBrush` — Gradient fill (linear)
- `PdfRadialGradientBrush` — Gradient fill (radial)
- `PdfTilingBrush` — Pattern fill
- `PdfImageBrush` — Image fill
