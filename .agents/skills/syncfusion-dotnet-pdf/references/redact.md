# PDF Redaction

Permanently remove sensitive or confidential content from PDF pages using the Syncfusion .NET PDF Library.
Redaction is **irreversible** — once applied and saved, the original content cannot be recovered.

*Note: For document creation, loading, and save/close patterns, see [document-structure.md](document-structure.md).*

---
**NuGet package:**

`Syncfusion.Pdf.Imaging.Net.Core` - (.NET Core / ASP.NET Core)

**Common namespaces:**

```csharp
using Syncfusion.Drawing;
using Syncfusion.Pdf;
using Syncfusion.Pdf.Parsing;
using Syncfusion.Pdf.Redaction;
```

---

## Create a basic redaction

Redact a rectangular area with a solid black fill.

```csharp
// new PdfRedaction(bounds, fillColor)
PdfRedaction redaction = new PdfRedaction(new RectangleF(343, 147, 60, 17), Color.Black);
page.AddRedaction(redaction);
```

## Set fill color on the redacted area

Replace the redacted region with a custom fill color instead of the default black.

```csharp
PdfRedaction redaction = new PdfRedaction(new RectangleF(343, 147, 60, 17));
redaction.FillColor = Color.Red;
page.AddRedaction(redaction);
```

## Display overlay text on the redacted area

Draw custom text (e.g., "Redacted", "Confidential") inside the redacted region using the appearance graphics.

```csharp
PdfRedaction redaction = new PdfRedaction(new RectangleF(343, 147, 60, 17), Color.Black);
PdfFont font = new PdfStandardFont(PdfFontFamily.Courier, 10);
redaction.Appearance.Graphics.DrawString("Redacted", font, PdfBrushes.White, new PointF(5, 5));
page.AddRedaction(redaction);
```

## Draw an image on the redacted area

Replace the redacted region with an image (e.g., a company logo or a stamp).

```csharp
PdfRedaction redaction = new PdfRedaction(new RectangleF(63, 57, 182, 157));
using var imageStream = new FileStream(Path.GetFullPath(@"Data/image.jpg"), FileMode.Open, FileAccess.Read);
PdfImage image = new PdfBitmap(imageStream);
redaction.Appearance.Graphics.DrawImage(image, new RectangleF(0, 0, 182, 157));
page.AddRedaction(redaction);
```

## Draw a tiling pattern on the redacted area

Fill the redacted region with a repeating tile pattern for a visually distinctive mask.

```csharp
PdfRedaction redaction = new PdfRedaction(new RectangleF(341, 149, 64, 14));
var tile = new RectangleF(0, 0, 8, 8);
var tiling = new PdfTilingBrush(tile);
tiling.Graphics.DrawRectangle(PdfBrushes.Gray,      new RectangleF(0, 0, 2, 2));
tiling.Graphics.DrawRectangle(PdfBrushes.White,     new RectangleF(2, 0, 2, 2));
tiling.Graphics.DrawRectangle(PdfBrushes.LightGray, new RectangleF(4, 0, 2, 2));
tiling.Graphics.DrawRectangle(PdfBrushes.DarkGray,  new RectangleF(6, 0, 2, 2));
redaction.Appearance.Graphics.DrawRectangle(tiling, new RectangleF(0, 0, 64, 14));
page.AddRedaction(redaction);
```

## Redact text content only (preserve graphics)

Use `TextOnly = true` to remove only text within the bounds while leaving images and vector graphics intact.

```csharp
PdfRedaction redaction = new PdfRedaction(new RectangleF(150, 150, 60, 24), Color.Transparent)
{
    TextOnly = true
};
page.AddRedaction(redaction);
```

## Find text by regular expression and redact

Extract text with word-level bounds, match a regex pattern, and redact every match on a page.

```csharp
// Requires: using System.Text.RegularExpressions;
TextLineCollection lineCollection = new TextLineCollection();
page.ExtractText(out lineCollection);

foreach (TextLine line in lineCollection.TextLine)
{
    foreach (TextWord word in line.WordCollection)
    {
        // Example pattern: dates in MM/DD/YYYY format
        MatchCollection matches = Regex.Matches(word.Text, @"\b\d{1,2}\/\d{1,2}\/\d{4}\b");
        foreach (Match m in matches)
        {
            page.AddRedaction(new PdfRedaction(word.Bounds, Syncfusion.Drawing.Color.Black));
        }
    }
}
```

## Find a specific keyword and redact all occurrences

Search across all pages for a keyword and redact every occurrence.

```csharp
foreach (PdfLoadedPage loadedPage in loadedDocument.Pages)
{
    TextLineCollection lineCollection = new TextLineCollection();
    loadedPage.ExtractText(out lineCollection);

    foreach (TextLine line in lineCollection.TextLine)
    {
        foreach (TextWord word in line.WordCollection)
        {
            if (word.Text.Contains("Confidential", StringComparison.OrdinalIgnoreCase))
            {
                loadedPage.AddRedaction(new PdfRedaction(word.Bounds, Syncfusion.Drawing.Color.Black));
            }
        }
    }
}
```

## Redact multiple regions on a page

Add several redaction rectangles in one pass for batch redaction of known bounding coordinates.

```csharp
var regions = new[]
{
    new RectangleF(100, 100, 120, 20),  // e.g., name field
    new RectangleF(100, 130, 160, 20),  // e.g., address field
    new RectangleF(100, 160, 100, 20),  // e.g., phone number
};

foreach (var bounds in regions)
{
    page.AddRedaction(new PdfRedaction(bounds, Color.Black));
}
```

## Track redaction progress

Subscribe to `RedactionProgress` to monitor progress when processing large documents.

```csharp
loadedDocument.RedactionProgress += (sender, e) =>
{
    Console.WriteLine($"Redaction progress: {e.Progress}%");
};
```

## Get redaction results

`Redact()` returns a list of `PdfRedactionResult` objects that describe what was removed on each page.

```csharp
List<PdfRedactionResult> results = loadedDocument.Redact();

foreach (PdfRedactionResult result in results)
{
    Console.WriteLine($"Page {result.PageNumber}: IsContentRemoved = {result.IsContentRemoved}");
}
```

## Apply redactions and save (full workflow)

Complete end-to-end example: load, configure redactions, apply, and save.

```csharp
var outputPath = Path.Combine(Directory.GetCurrentDirectory(), "output", "Output_Redacted.pdf");

// Load the document
PdfLoadedDocument loadedDocument = new PdfLoadedDocument("input.pdf");
PdfLoadedPage page = loadedDocument.Pages[0] as PdfLoadedPage;

// Redaction 1: solid black fill
PdfRedaction redaction1 = new PdfRedaction(new RectangleF(343, 147, 60, 17), Color.Black);
page.AddRedaction(redaction1);

// Redaction 2: dark fill with overlay text
PdfRedaction redaction2 = new PdfRedaction(new RectangleF(100, 200, 150, 20));
redaction2.FillColor = Color.DarkGray;
PdfFont font = new PdfStandardFont(PdfFontFamily.Helvetica, 8);
redaction2.Appearance.Graphics.DrawString("REDACTED", font, PdfBrushes.White, new PointF(4, 4));
page.AddRedaction(redaction2);

// Track progress
loadedDocument.RedactionProgress += (s, e) => Console.WriteLine($"Progress: {e.Progress}%");

// Apply and capture results
List<PdfRedactionResult> results = loadedDocument.Redact();

// Save and close
loadedDocument.Save(outputPath);
loadedDocument.Close(true);
Console.WriteLine($"SUCCESS: {outputPath}");
```

---

## Key APIs

| Member | Description |
| --- | --- |
| `PdfRedaction(RectangleF bounds)` | Creates a redaction for the given bounds with a transparent fill |
| `PdfRedaction(RectangleF bounds, Color fillColor)` | Creates a redaction with the specified solid fill color |
| `PdfRedaction.FillColor` | Gets or sets the fill color applied to the redacted region after `Redact()` |
| `PdfRedaction.TextOnly` | When `true`, removes only text within bounds; images and graphics are preserved |
| `PdfRedaction.Appearance.Graphics` | `PdfGraphics` surface to draw overlay text, images, or patterns onto the redacted region |
| `PdfLoadedPage.AddRedaction(PdfRedaction)` | Registers a redaction on the page — must be called before `Redact()` |
| `PdfLoadedDocument.Redact()` | Permanently applies all registered redactions; returns `List<PdfRedactionResult>` |
| `PdfLoadedDocument.RedactionProgress` | Event raised with progress percentage during `Redact()` on large documents |
| `PdfRedactionResult.PageNumber` | 1-based page index where the redaction was applied |
| `PdfRedactionResult.IsContentRemoved` | `true` if content was found and removed within the redacted bounds |

---

## Notes

- Always call `page.AddRedaction(redaction)` **before** calling `loadedDocument.Redact()`.
- `Redact()` must be called **before** `Save()` — order matters.
- Use `TextOnly = true` when only text needs removal and vector graphics should remain untouched.
- Bounds are in PDF user-space units (points, 72 DPI). Match coordinates precisely to actual content position.
- For search-based redaction across all pages, use `ExtractText(out TextLineCollection)` to get word-level bounds (see `extract-text.md`).
- Redaction is **permanent and irreversible** — once saved, the original content cannot be recovered.
