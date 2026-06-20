# PDF Annotations

Guide and code snippets for adding, modifying, removing, flattening, and importing/exporting PDF annotations using Syncfusion .NET PDF Library. Examples are ordered from basic → advanced.

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

## Add a popup annotation to a new PDF

```csharp
PdfDocument document = new PdfDocument();
PdfPage page = document.Pages.Add();
RectangleF rectangle = new RectangleF(10, 40, 30, 30);

PdfPopupAnnotation popupAnnotation = new PdfPopupAnnotation(rectangle, "Test popup annotation");
popupAnnotation.Border.Width = 4;
popupAnnotation.Border.HorizontalRadius = 20;
popupAnnotation.Border.VerticalRadius = 30;
popupAnnotation.Icon = PdfPopupIcon.NewParagraph;
page.Annotations.Add(popupAnnotation);
```

---

## Add a popup annotation to an existing PDF

```csharp
PdfLoadedDocument document = new PdfLoadedDocument("Input.pdf");
RectangleF rectangle = new RectangleF(10, 40, 30, 30);

PdfPopupAnnotation popupAnnotation = new PdfPopupAnnotation(rectangle, "Test popup annotation");
popupAnnotation.Border.Width = 4;
popupAnnotation.Icon = PdfPopupIcon.NewParagraph;
document.Pages[0].Annotations.Add(popupAnnotation);
```

---

## Add a free text annotation

```csharp
PdfDocument document = new PdfDocument();
PdfPage page = document.Pages.Add();

PdfFreeTextAnnotation freeText = new PdfFreeTextAnnotation(new RectangleF(50, 100, 100, 50));
freeText.MarkupText = "Free Text with Callout";
freeText.TextMarkupColor = new PdfColor(Color.Black);
freeText.Font = new PdfStandardFont(PdfFontFamily.Helvetica, 7f);
freeText.Color = new PdfColor(Color.Yellow);
freeText.BorderColor = new PdfColor(Color.Red);
freeText.Border = new PdfAnnotationBorder(.5f);
freeText.LineEndingStyle = PdfLineEndingStyle.OpenArrow;
freeText.Text = "Free Text";
freeText.Opacity = 0.5f;
PointF[] points = { new PointF(100, 450), new PointF(100, 200), new PointF(100, 150) };
freeText.CalloutLines = points;
page.Annotations.Add(freeText);
```

---

## Add a line annotation

```csharp
PdfDocument document = new PdfDocument();
PdfPage page = document.Pages.Add();
int[] points = new int[] { 80, 420, 150, 420 };

PdfLineAnnotation lineAnnotation = new PdfLineAnnotation(points, "Line Annotation");
LineBorder lineBorder = new LineBorder();
lineBorder.BorderStyle = PdfBorderStyle.Solid;
lineBorder.BorderWidth = 1;
lineAnnotation.lineBorder = lineBorder;
lineAnnotation.BeginLineStyle = PdfLineEndingStyle.Butt;
lineAnnotation.EndLineStyle = PdfLineEndingStyle.Diamond;
lineAnnotation.InnerLineColor = new PdfColor(Color.Green);
lineAnnotation.BackColor = new PdfColor(Color.Green);
lineAnnotation.LineCaption = true;
lineAnnotation.CaptionType = PdfLineCaptionType.Inline;
page.Annotations.Add(lineAnnotation);
```

---

## Add a rubber stamp annotation

```csharp
PdfDocument document = new PdfDocument();
PdfPage page = document.Pages.Add();

RectangleF rectangle = new RectangleF(40, 60, 80, 20);
PdfRubberStampAnnotation rubberStampAnnotation = new PdfRubberStampAnnotation(rectangle, "Text Rubber Stamp Annotation");
rubberStampAnnotation.Icon = PdfRubberStampAnnotationIcon.Draft;
rubberStampAnnotation.Text = "Text Properties Rubber Stamp Annotation";
page.Annotations.Add(rubberStampAnnotation);
```

---

## Add an ink annotation

```csharp
PdfDocument document = new PdfDocument();
PdfPage page = document.Pages.Add();
List<float> linePoints = new List<float> { 40, 300, 60, 100, 40, 50, 40, 300 };

PdfInkAnnotation inkAnnotation = new PdfInkAnnotation(new RectangleF(0, 0, 300, 400), linePoints);
inkAnnotation.Color = new PdfColor(Color.Red);
page.Annotations.Add(inkAnnotation);
```

---

## Add a text markup annotation (highlight)

```csharp
PdfDocument document = new PdfDocument();
PdfPage page = document.Pages.Add();
PdfFont pdfFont = new PdfTrueTypeFont(new FileStream("arial.ttf", FileMode.Open, FileAccess.Read), 14);
page.Graphics.DrawString("Text Markup", pdfFont, new PdfSolidBrush(Color.Black), new RectangleF(175, 40, 100, 20));

PdfTextMarkupAnnotation markupAnnotation = new PdfTextMarkupAnnotation("Markup annotation", "Highlight demo", "Text Markup", new PointF(175, 40), pdfFont);
markupAnnotation.TextMarkupColor = new PdfColor(Color.BlueViolet);
markupAnnotation.TextMarkupAnnotationType = PdfTextMarkupAnnotationType.Highlight;
page.Annotations.Add(markupAnnotation);
```

---

## Add a URI annotation

```csharp
PdfDocument document = new PdfDocument();
PdfPage page = document.Pages.Add();

PdfUriAnnotation uriAnnotation = new PdfUriAnnotation(new RectangleF(10, 40, 30, 30), "http://www.google.com");
uriAnnotation.Text = "Uri Annotation";
page.Annotations.Add(uriAnnotation);
```

---

## Add a document link annotation

```csharp
PdfDocument document = new PdfDocument();
PdfPage page = document.Pages.Add();
PdfPage page2 = document.Pages.Add();

PdfDocumentLinkAnnotation documentLinkAnnotation = new PdfDocumentLinkAnnotation(new RectangleF(10, 40, 30, 30));
documentLinkAnnotation.Text = "Document link annotation";
documentLinkAnnotation.Destination = new PdfDestination(page2);
documentLinkAnnotation.Destination.Location = new PointF(10, 0);
documentLinkAnnotation.Destination.Zoom = 5;
page.Annotations.Add(documentLinkAnnotation);
```

---

## Add a watermark annotation

```csharp
PdfLoadedDocument loadedDocument = new PdfLoadedDocument("Input.pdf");
PdfLoadedPage lpage = loadedDocument.Pages[0] as PdfLoadedPage;

PdfWatermarkAnnotation watermark = new PdfWatermarkAnnotation(new RectangleF(50, 100, 100, 50));
watermark.Opacity = 0.5f;
watermark.Appearance.Normal.Graphics.DrawString("Watermark Text",
    new PdfStandardFont(PdfFontFamily.Helvetica, 20), PdfBrushes.Red,
    new RectangleF(0, 0, 200, 50),
    new PdfStringFormat(PdfTextAlignment.Center, PdfVerticalAlignment.Middle));
lpage.Annotations.Add(watermark);
```

---

## Add a redaction annotation

```csharp
PdfDocument document = new PdfDocument();
PdfPage page = document.Pages.Add();

PdfRedactionAnnotation annot = new PdfRedactionAnnotation();
annot.Bounds = new Rectangle(100, 120, 100, 100);
annot.InnerColor = Color.Black;
annot.BorderColor = Color.Yellow;
annot.TextColor = Color.Blue;
annot.Font = new PdfStandardFont(PdfFontFamily.Helvetica, 10);
annot.OverlayText = "REDACTION";
annot.TextAlignment = PdfTextAlignment.Right;
annot.RepeatText = true;
annot.SetAppearance(true);
page.Annotations.Add(annot);
```

---

## Add a rectangle annotation with cloud border

```csharp
PdfDocument document = new PdfDocument();
PdfPage page = document.Pages.Add();

PdfRectangleAnnotation annotation = new PdfRectangleAnnotation(new RectangleF(0, 0, 200, 100), "rectangle");
annotation.Border.BorderWidth = 1;
annotation.Color = Color.Red;
annotation.InnerColor = Color.Blue;

PdfBorderEffect bordereffect = new PdfBorderEffect();
bordereffect.Intensity = 2;
bordereffect.Style = PdfBorderEffectStyle.Cloudy;
annotation.BorderEffect = bordereffect;
page.Annotations.Add(annotation);
```

---

## Modify an annotation in an existing PDF

```csharp
PdfLoadedDocument lDoc = new PdfLoadedDocument("Input.pdf");
PdfLoadedPage page = lDoc.Pages[0] as PdfLoadedPage;
PdfLoadedAnnotationCollection annotations = page.Annotations;

PdfLoadedPopupAnnotation popUp = annotations[0] as PdfLoadedPopupAnnotation;
popUp.Border = new PdfAnnotationBorder(4, 0, 0);
popUp.Color = new PdfColor(Color.Red);
popUp.Text = "Modified annotation";
// Rebuild appearance so changes are visible in viewers
popUp.SetAppearance(true);
```

---

## Remove an annotation from an existing PDF

```csharp
PdfLoadedDocument lDoc = new PdfLoadedDocument("Input.pdf");
PdfLoadedPage page = lDoc.Pages[0] as PdfLoadedPage;
PdfLoadedAnnotationCollection annotations = page.Annotations;
//Removes the first annotation
annotations.RemoveAt(0);
```

---

## Flatten all annotations

```csharp
PdfLoadedDocument loadedDocument = new PdfLoadedDocument("Input.pdf");
foreach (PdfLoadedPage loadedPage in loadedDocument.Pages)
{
    loadedPage.Annotations.Flatten = true;
}
```

## Flatten a specific annotation type

```csharp
foreach (PdfLoadedPage loadedPage in loadedDocument.Pages)
{
    foreach (PdfLoadedAnnotation annotation in loadedPage.Annotations)
    {
        if (annotation is PdfLoadedCircleAnnotation)
            annotation.Flatten = true;
    }
}
```

## Flatten annotations using FlattenAnnotations()

```csharp
PdfLoadedDocument loadedDocument = new PdfLoadedDocument("Input.pdf");
//Flatten without pop-ups
loadedDocument.FlattenAnnotations();
//Or flatten including pop-ups
loadedDocument.FlattenAnnotations(true);
```

---

## Add comments and review status to an annotation

```csharp
PdfDocument document = new PdfDocument();
PdfPage page = document.Pages.Add();

PdfRectangleAnnotation rectangleAnnotation = new PdfRectangleAnnotation(new RectangleF(0, 0, 100, 50), "Rectangle Annotation");
rectangleAnnotation.Author = "Syncfusion";
rectangleAnnotation.Border.BorderWidth = 1;
rectangleAnnotation.Color = Color.Red;
rectangleAnnotation.ModifiedDate = DateTime.Now;

//Add a comment
PdfPopupAnnotation comment = new PdfPopupAnnotation();
comment.Author = "John";
comment.Text = "This is first comment";
comment.ModifiedDate = DateTime.Now;
comment.Subject = "Annotation Comments";
rectangleAnnotation.Comments.Add(comment);

//Add a review status
PdfPopupAnnotation review = new PdfPopupAnnotation();
review.Author = "John";
review.StateModel = PdfAnnotationStateModel.Review;
review.State = PdfAnnotationState.Accepted;
review.ModifiedDate = DateTime.Now;
rectangleAnnotation.ReviewHistory.Add(review);
page.Annotations.Add(rectangleAnnotation);
```

---

## Retrieve annotation type from an existing PDF

```csharp
using (PdfLoadedDocument document = new PdfLoadedDocument("Input.pdf"))
{
    for (int i = 0; i < document.PageCount; i++)
    {
        PdfLoadedPage page = document.Pages[i] as PdfLoadedPage;
        foreach (PdfLoadedAnnotation annotation in page.Annotations)
            Console.WriteLine($"Page {i} — Type: {annotation.Type}");
    }
}
```

---

## Retrieve annotation creation date

```csharp
using (PdfLoadedDocument document = new PdfLoadedDocument("Input.pdf"))
{
    PdfLoadedAnnotation annotation = (document.Pages[0] as PdfLoadedPage).Annotations[0] as PdfLoadedAnnotation;
    Console.WriteLine("Created: " + annotation.CreationDate);
}
```

---

## Key APIs

| Member | Description |
| --- | --- |
| `PdfPopupAnnotation(RectangleF, string)` | Creates a popup/comment annotation with the given bounds and text |
| `PdfFreeTextAnnotation(RectangleF)` | Creates a free text annotation displayed directly on the page |
| `PdfLineAnnotation(int[], string)` | Creates a line annotation with the given endpoints and note text |
| `PdfRubberStampAnnotation(RectangleF, string)` | Creates a rubber stamp annotation |
| `PdfInkAnnotation(RectangleF, List<float>)` | Creates a freehand ink annotation from a list of points |
| `PdfTextMarkupAnnotation` | Highlights, underlines, or strikes through text on the page |
| `PdfUriAnnotation(RectangleF, string)` | Creates a clickable URI link annotation |
| `PdfDocumentLinkAnnotation(RectangleF)` | Creates an in-document navigation link annotation |
| `PdfWatermarkAnnotation(RectangleF)` | Creates a fixed-size watermark annotation |
| `PdfRedactionAnnotation` | Marks content for permanent removal; apply via `Flatten` + `Redact()` |
| `PdfRectangleAnnotation` / `PdfCircleAnnotation` | Shape annotations; support cloud border via `PdfBorderEffect` |
| `PdfAnnotation.Opacity` | Transparency of the annotation (0 = fully transparent, 1 = opaque) |
| `PdfAnnotation.AnnotationFlags` | Flags controlling visibility, print, read-only, locked, etc. |
| `PdfLoadedAnnotation.SetAppearance(true)` | Rebuilds appearance stream — required after modifying annotation properties |
| `PdfLoadedAnnotationCollection.Flatten` | Set `true` to flatten all annotations on a page during save |
| `PdfLoadedAnnotation.Flatten` | Set `true` to flatten a specific annotation during save |
| `PdfLoadedDocument.FlattenAnnotations()` | Flatten all annotations without needing a separate save call |
| `PdfAnnotation.Comments` | Collection of popup comment annotations attached to this annotation |
| `PdfAnnotation.ReviewHistory` | Collection of review-state popup annotations attached to this annotation |
| `PdfLoadedAnnotation.CreationDate` | Retrieves the creation date of an existing annotation |
| `PdfLoadedAnnotation.Type` | Returns the `PdfLoadedAnnotationTypes` enum value for the annotation |

---

## Notes

- Always call `SetAppearance(true)` after modifying annotation properties so the visual representation updates in viewers.
- To flatten redaction annotations in ASP.NET Core, reference `Syncfusion.Pdf.Imaging.Portable`.
- When exporting annotations that use `PdfTrueTypeFont`, save the document first to ensure font resources are embedded before export.
- `PdfAnnotationFlags.Print` must be set for an annotation to appear in print output.

---
