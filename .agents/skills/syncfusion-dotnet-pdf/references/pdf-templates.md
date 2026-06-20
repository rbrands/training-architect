# PDF Templates

Guide and code snippets for creating and using PDF templates (PdfTemplate, PdfPageTemplateElement, PdfPageTemplate) with Syncfusion .NET PDF Library. Examples are ordered from basic → advanced.

*Note: For document creation, loading, and save/close patterns, see [document-structure.md](document-structure.md).*

---

**Common namespaces:**

```csharp
using Syncfusion.Drawing;
using Syncfusion.Pdf;
using Syncfusion.Pdf.Graphics;
using Syncfusion.Pdf.Interactive;
using Syncfusion.Pdf.Parsing;
```

## Create a template and draw it on a new PDF

```csharp
PdfDocument pdfDocument = new PdfDocument();
PdfPage pdfPage = pdfDocument.Pages.Add();

//Create a template with a fixed size
PdfTemplate template = new PdfTemplate(100, 50);
//Draw a rectangle onto the template
template.Graphics.DrawRectangle(PdfBrushes.BurlyWood, new RectangleF(0, 0, 100, 50));
//Draw text onto the template
PdfFont font = new PdfStandardFont(PdfFontFamily.Helvetica, 14);
template.Graphics.DrawString("Hello World", font, new PdfSolidBrush(Color.Black), 5, 5);

//Stamp the template onto the page at the origin
pdfPage.Graphics.DrawPdfTemplate(template, PointF.Empty);
```

---

## Draw a template on an existing PDF

```csharp
PdfLoadedDocument loadedDocument = new PdfLoadedDocument("Input.pdf");
PdfLoadedPage loadedPage = loadedDocument.Pages[0] as PdfLoadedPage;

PdfTemplate template = new PdfTemplate(100, 50);
template.Graphics.DrawRectangle(PdfBrushes.BurlyWood, new RectangleF(0, 0, 100, 50));
PdfFont font = new PdfStandardFont(PdfFontFamily.Helvetica, 14);
template.Graphics.DrawString("Hello World", font, new PdfSolidBrush(Color.Black), 5, 5);

//Stamp the template onto the loaded page
loadedPage.Graphics.DrawPdfTemplate(template, PointF.Empty);
```

---

## Create a template from an existing page

Capture an existing page as a reusable template and draw it (optionally scaled) onto a new document.

```csharp
PdfLoadedDocument loadedDocument = new PdfLoadedDocument("Input.pdf");
PdfLoadedPage loadedPage = loadedDocument.Pages[0] as PdfLoadedPage;

//Capture the page content into a template
PdfTemplate template = loadedPage.CreateTemplate();

//Draw into a new document at half width
PdfDocument document = new PdfDocument();
PdfPage page = document.Pages.Add();
page.Graphics.DrawPdfTemplate(template, PointF.Empty,
    new SizeF(page.Size.Width / 2, page.Size.Height));
```

---

## Create a document overlay (merge two pages via templates)

```csharp
PdfLoadedDocument doc1 = new PdfLoadedDocument("Input1.pdf");
PdfLoadedDocument doc2 = new PdfLoadedDocument("Input2.pdf");
PdfDocument document = new PdfDocument();
PdfPage page = document.Pages.Add();

//Draw page 0 of doc1 as background
PdfTemplate template1 = doc1.Pages[0].CreateTemplate();
page.Graphics.DrawPdfTemplate(template1, new PointF(0, 0), new SizeF(500, 700));

//Overlay page 0 of doc2 at an offset
PdfTemplate template2 = doc2.Pages[0].CreateTemplate();
page.Graphics.DrawPdfTemplate(template2, new PointF(10, 10), new SizeF(400, 500));

doc1.Close(true);
doc2.Close(true);
```

---

## Add a PdfPageTemplate from an existing PDF

```csharp
PdfLoadedDocument loadedDocument = new PdfLoadedDocument("Input.pdf");
PdfPageBase page = loadedDocument.Pages[0];

PdfPageTemplate pageTemplate = new PdfPageTemplate(page);
pageTemplate.Name = "pageTemplate";
pageTemplate.IsVisible = true;
loadedDocument.PdfPageTemplates.Add(pageTemplate);
```

---

## Key APIs

| Member | Description |
| --- | --- |
| `PdfTemplate(float width, float height)` | Creates a blank drawing surface of the given size |
| `PdfTemplate.Graphics` | `PdfGraphics` surface — draw text, images, shapes onto the template |
| `PdfPageBase.CreateTemplate()` | Captures an existing page's content as a `PdfTemplate` |
| `PdfGraphics.DrawPdfTemplate(PdfTemplate, PointF)` | Stamps a template onto a page at the given point |
| `PdfGraphics.DrawPdfTemplate(PdfTemplate, PointF, SizeF)` | Stamps a template with explicit scaling |
| `PdfPageTemplateElement(RectangleF)` | Creates a positioned template element for headers, footers, or stamps |
| `PdfPageTemplateElement.Graphics` | `PdfGraphics` surface for the template element |
| `PdfDocument.Template.Top` | Sets the document-wide header template element |
| `PdfDocument.Template.Bottom` | Sets the document-wide footer template element |
| `PdfDocument.Template.Left` | Sets the document-wide left-margin template element |
| `PdfDocument.Template.Right` | Sets the document-wide right-margin template element |
| `PdfPageNumberField(PdfFont, PdfBrush)` | Auto-field that renders the current page number |
| `PdfPageCountField(PdfFont, PdfBrush)` | Auto-field that renders the total page count |
| `PdfCompositeField(PdfFont, PdfBrush, string, params PdfAutomaticField[])` | Combines text and auto-fields (e.g. "Page {0} of {1}") |
| `PdfPageTemplate(PdfPageBase)` | Wraps a loaded page as a named, reusable page template |
| `PdfPageTemplate.Name` | Identifier string for the page template |
| `PdfPageTemplate.IsVisible` | Controls whether the template is visible when rendered |
| `PdfLoadedDocument.PdfPageTemplates` | Collection of `PdfPageTemplate` on an existing document |

---

## Notes

- `PdfTemplate` acts like an off-screen canvas; draw any content (text, images, shapes, forms) then stamp it anywhere with `DrawPdfTemplate`.
- `PdfPageTemplateElement` set on `PdfDocument.Template.Top/Bottom` repeats automatically on every page — ideal for consistent headers and footers.
- `CreateTemplate()` on a loaded page captures its visual content; use `DrawPdfTemplate` with a `SizeF` argument to scale it when drawing onto another page.
- See [headers-and-footers.md](headers-and-footers.md) for dedicated header/footer patterns including page number formatting.
- See [watermarks.md](watermarks.md) for stamp/watermark use cases built on top of templates.

---
