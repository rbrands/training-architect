# PDF Brushes

Fill shapes and content in PDF documents using solid, gradient, tiling, and hatch brushes with Syncfusion .NET PDF Library.

*Note: For document creation, loading, and save/close patterns, see [document-structure.md](document-structure.md). For drawing shapes and graphics, see [pdf-graphics.md](pdf-graphics.md) and [shapes.md](shapes.md).*

---

**Common namespaces:**

```csharp
using Syncfusion.Drawing;
using Syncfusion.Pdf;
using Syncfusion.Pdf.Graphics;
```

---

## Solid brush

Use `PdfSolidBrush` to fill shapes with a single flat color.

```csharp
PdfDocument doc = new PdfDocument();
PdfPage page = doc.Pages.Add();
PdfGraphics graphics = page.Graphics;

// Create a solid red brush
PdfSolidBrush brush = new PdfSolidBrush(Color.Red);

// Fill an ellipse with the solid brush
graphics.DrawEllipse(brush, new RectangleF(0, 0, 200, 100));
```

---

## Linear gradient brush (two colors)

Use `PdfLinearGradientBrush` to fill a shape with a smooth two-color transition along a straight line.

```csharp
PdfDocument doc = new PdfDocument();
PdfPage page = doc.Pages.Add();
PdfGraphics graphics = page.Graphics;

// Gradient flows from (0,0) → (200,100): Red → Blue
PdfLinearGradientBrush brush = new PdfLinearGradientBrush(
    new PointF(0, 0),
    new PointF(200, 100),
    Color.Red,
    Color.Blue);

graphics.DrawEllipse(brush, new RectangleF(0, 0, 200, 100));
```

---

## Linear gradient brush (multiple colors)

Use `PdfColorBlend` with `InterpolationColors` to define a gradient that transitions through more than two colors.

```csharp
PdfDocument document = new PdfDocument();
PdfPage page = document.Pages.Add();
PdfGraphics graphics = page.Graphics;

// Create a horizontal linear gradient brush
PdfLinearGradientBrush brush = new PdfLinearGradientBrush(
    new RectangleF(new PointF(0, 0), new SizeF(200, 100)),
    Color.Red,
    Color.Blue,
    PdfLinearGradientMode.Horizontal);

// Define multi-stop color blend
PdfColorBlend colorBlend = new PdfColorBlend(4)
{
    Colors = new PdfColor[]
    {
        Color.Red,
        Color.Yellow,
        Color.Green,
        Color.Blue
    },
    // Position values must start at 0 and end at 1
    Positions = new float[] { 0f, 0.3f, 0.7f, 1f }
};

brush.InterpolationColors = colorBlend;

graphics.DrawRectangle(brush, new RectangleF(0, 0, 200, 100));
```

---

## Radial gradient brush

Use `PdfRadialGradientBrush` to fill a shape with a color that radiates outward from a center point.

```csharp
PdfDocument doc = new PdfDocument();
PdfPage page = doc.Pages.Add();
PdfGraphics graphics = page.Graphics;

// Inner circle: center (50,50), radius 0  →  Outer circle: center (50,50), radius 50
// Color transitions: Red (center) → Blue (edge)
PdfRadialGradientBrush brush = new PdfRadialGradientBrush(
    new PointF(50, 50), 0,          // inner center + radius
    new PointF(50, 50), 50,         // outer center + radius
    Color.Red,
    Color.Blue);

graphics.DrawEllipse(brush, new RectangleF(0, 0, 100, 100));
```

---

## Tiling brush

Use `PdfTilingBrush` to fill a region by repeating a small tile graphic. Draw any content into the tile's `Graphics`, then use the brush to fill a larger area.

```csharp
PdfDocument doc = new PdfDocument();
PdfPage page = doc.Pages.Add();
PdfGraphics graphics = page.Graphics;

// Define the tile size (11 × 11 units)
PdfTilingBrush brush = new PdfTilingBrush(new RectangleF(0, 0, 11, 11));

// Draw a small red ellipse inside the tile
brush.Graphics.DrawEllipse(PdfPens.Red, new RectangleF(0, 0, 10, 10));

// Fill a larger ellipse by tiling the brush
graphics.DrawEllipse(brush, new RectangleF(0, 0, 200, 100));
```

---

## Hatch brush

Use `PdfHatchBrush` to fill a shape with a predefined hatch pattern. Choose from `PdfHatchStyle` values such as `Cross`, `DiagonalCross`, `Plaid`, `Trellis`, and more.

```csharp
PdfDocument doc = new PdfDocument();
PdfPage page = doc.Pages.Add();
PdfGraphics graphics = page.Graphics;

// Define foreground (pattern) and background colors
Color foreColor = Color.FromArgb(255, 255, 255, 0);   // Yellow
Color backColor = Color.FromArgb(255, 78, 167, 46);   // Green

// Create hatch brush with Plaid style
PdfHatchBrush brush = new PdfHatchBrush(
    PdfHatchStyle.Plaid,
    new PdfColor(foreColor),
    new PdfColor(backColor));

graphics.DrawRectangle(PdfPens.Black, brush, new Rectangle(100, 100, 300, 200));
```

---

## Use PdfBrushes static colors

`PdfBrushes` exposes named solid brushes for common colors — no instantiation needed.

```csharp
// Use a named brush directly
graphics.DrawString("Hello PDF", new PdfStandardFont(PdfFontFamily.Helvetica, 14),
    PdfBrushes.DarkBlue, new PointF(10, 10));

graphics.DrawRectangle(PdfBrushes.LightGray, new RectangleF(10, 40, 200, 60));

graphics.DrawEllipse(PdfBrushes.Tomato, new RectangleF(10, 120, 100, 60));
```

---

## Combine brush with pen (outlined + filled shapes)

Pass both a `PdfPen` (outline) and a `PdfBrush` (fill) to draw a shape with a border and a filled interior.

```csharp
PdfPen pen = new PdfPen(Color.Black, 2f);
PdfSolidBrush brush = new PdfSolidBrush(Color.LightSkyBlue);

// Outlined + filled rectangle
graphics.DrawRectangle(pen, brush, new RectangleF(20, 20, 180, 80));

// Outlined + filled ellipse
graphics.DrawEllipse(pen, brush, new RectangleF(20, 120, 180, 80));
```

---

## Key APIs

| Member | Description |
| --- | --- |
| `PdfSolidBrush(Color)` | Creates a brush that fills with a single flat color |
| `PdfLinearGradientBrush(PointF, PointF, Color, Color)` | Two-color gradient along the line from start point to end point |
| `PdfLinearGradientBrush(RectangleF, Color, Color, PdfLinearGradientMode)` | Two-color gradient bounded by a rectangle; direction set via `PdfLinearGradientMode` |
| `PdfLinearGradientBrush.InterpolationColors` | Assigns a `PdfColorBlend` for multi-stop color transitions |
| `PdfColorBlend(int)` | Defines a multi-color gradient; set `Colors` (`PdfColor[]`) and `Positions` (`float[]`) |
| `PdfColorBlend.Colors` | Array of `PdfColor` values at each gradient stop |
| `PdfColorBlend.Positions` | Array of `float` positions (0 – 1) matching each color stop |
| `PdfLinearGradientMode` | Enum: `Horizontal`, `Vertical`, `ForwardDiagonal`, `BackwardDiagonal` |
| `PdfRadialGradientBrush(PointF, float, PointF, float, Color, Color)` | Radial gradient: inner center + radius, outer center + radius, start color, end color |
| `PdfTilingBrush(RectangleF)` | Creates a repeating tile of the given size; draw content via `brush.Graphics` |
| `PdfTilingBrush.Graphics` | `PdfGraphics` for drawing the tile content that will be repeated |
| `PdfHatchBrush(PdfHatchStyle, PdfColor, PdfColor)` | Pattern brush with foreground (pattern) and background colors |
| `PdfHatchStyle` | Enum of hatch patterns: `Cross`, `DiagonalCross`, `Plaid`, `Trellis`, `Horizontal`, `Vertical`, `ForwardDiagonal`, `BackwardDiagonal`, and more |
| `PdfBrushes` | Static class exposing named `PdfSolidBrush` instances (e.g., `PdfBrushes.Red`, `PdfBrushes.Black`) |
| `PdfColor(Color)` | Wraps a `System.Drawing.Color` / `Syncfusion.Drawing.Color` as a PDF color value |

---

## Notes

- `PdfBrushes` static brushes are singletons — do not dispose them.
- For `PdfColorBlend`, the `Positions` array must start at `0` and end at `1`; the array length must match `Colors`.
- `PdfTilingBrush` draws tiles in the page coordinate space; the tile size should be chosen relative to the target fill area.
- `PdfHatchBrush` is available in `Syncfusion.Pdf.Graphics` and works with all standard drawing methods (`DrawRectangle`, `DrawEllipse`, etc.).
- Use `Syncfusion.Drawing` namespace (not `System.Drawing`) when targeting .NET Core / CSX scripts.

---

## Related

- [pdf-graphics.md](pdf-graphics.md)
- [shapes.md](shapes.md)
- [colorspace.md](colorspace.md)
- [watermarks.md](watermarks.md)
- [document-structure.md](document-structure.md)

## Official documentation

- <https://help.syncfusion.com/document-processing/pdf/pdf-library/net/working-with-brushes>
