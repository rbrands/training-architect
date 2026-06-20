# PDF Hyperlinks

Add web, internal document, and external file navigation links to PDFs using Syncfusion .NET PDF Library.

*Note: For document creation, loading, and save/close patterns, see [document-structure.md](document-structure.md). For annotation-based links, see [annotations.md](annotations.md). For named destination navigation, see [named-destinations.md](named-destinations.md).*

---

**Common namespaces:**

```csharp
using Syncfusion.Drawing;
using Syncfusion.Pdf;
using Syncfusion.Pdf.Graphics;
using Syncfusion.Pdf.Interactive;
using Syncfusion.Pdf.Parsing;
```

---

## Add a web hyperlink to a new PDF

Navigate to a URL from a PDF page using `PdfTextWebLink`.

```csharp
using Syncfusion.Pdf.Graphics;
using Syncfusion.Pdf.Interactive;

// Create the font
PdfFont font = new PdfStandardFont(PdfFontFamily.Helvetica, 12f);

// Create a text web link
PdfTextWebLink textLink = new PdfTextWebLink();
textLink.Url = "http://www.syncfusion.com";
textLink.Text = "Syncfusion .NET components and controls";
textLink.Font = font;

// Draw the hyperlink on the page
textLink.DrawTextWebLink(page, new PointF(10, 40));
```

---

## Add a web hyperlink to an existing PDF

Add a clickable web link to a page in an already existing PDF document.

```csharp
using Syncfusion.Pdf.Graphics;
using Syncfusion.Pdf.Interactive;
using Syncfusion.Pdf.Parsing;

// Load the PDF document and page
PdfLoadedDocument loadedDocument = new PdfLoadedDocument("Input.pdf");
PdfLoadedPage loadedPage = loadedDocument.Pages[0] as PdfLoadedPage;

// Create a text web link
PdfFont font = new PdfStandardFont(PdfFontFamily.Helvetica, 12f);
PdfTextWebLink textLink = new PdfTextWebLink();
textLink.Url = "http://www.syncfusion.com";
textLink.Text = "Syncfusion .NET components and controls";
textLink.Font = font;

// Draw the hyperlink on the loaded page graphics
textLink.DrawTextWebLink(loadedPage.Graphics, new PointF(10, 40));
```

---

## Add internal document navigation link

Navigate to another page within the same document using `PdfDocumentLinkAnnotation`.

```csharp
using Syncfusion.Pdf.Interactive;

// Create bounds for the link area
RectangleF bounds = new RectangleF(10, 40, 30, 30);

// Create a document link annotation
PdfDocumentLinkAnnotation documentLinkAnnotation = new PdfDocumentLinkAnnotation(bounds);
documentLinkAnnotation.AnnotationFlags = PdfAnnotationFlags.NoRotate;
documentLinkAnnotation.Text = "Document link annotation";
documentLinkAnnotation.Color = new PdfColor(Color.Navy);

// Set the destination page and location
PdfPage navigationPage = document.Pages.Add();
documentLinkAnnotation.Destination = new PdfDestination(navigationPage);
documentLinkAnnotation.Destination.Location = new PointF(10, 0);
documentLinkAnnotation.Destination.Zoom = 1;

// Add annotation to the source page
page.Annotations.Add(documentLinkAnnotation);
```

---

## Add internal navigation link to an existing PDF

Add an internal page-to-page link annotation to an existing PDF document.

```csharp
using Syncfusion.Pdf.Interactive;
using Syncfusion.Pdf.Parsing;

// Load the PDF document and source page
PdfLoadedDocument loadedDocument = new PdfLoadedDocument("Input.pdf");
PdfLoadedPage loadedPage = loadedDocument.Pages[0] as PdfLoadedPage;

// Create a document link annotation
RectangleF bounds = new RectangleF(10, 40, 30, 30);
PdfDocumentLinkAnnotation documentLinkAnnotation = new PdfDocumentLinkAnnotation(bounds);
documentLinkAnnotation.Text = "Document link annotation";

// Set the destination to an existing page
PdfLoadedPage navigationPage = loadedDocument.Pages[1] as PdfLoadedPage;
documentLinkAnnotation.Destination = new PdfDestination(navigationPage);
documentLinkAnnotation.Destination.Location = new PointF(10, 0);

// Add annotation to the source page
loadedPage.Annotations.Add(documentLinkAnnotation);
```

---

## Add external file navigation link

Open an external file (image, text, PDF, etc.) using `PdfFileLinkAnnotation`.

```csharp
using Syncfusion.Pdf.Interactive;

// Create bounds for the link area
RectangleF bounds = new RectangleF(10, 40, 30, 30);

// Create a file link annotation pointing to an external file path
PdfFileLinkAnnotation fileLinkAnnotation = new PdfFileLinkAnnotation(bounds, "C:/Files/sample.pdf");

// Add annotation to the page
page.Annotations.Add(fileLinkAnnotation);
```

> **Note:** `PdfFileLinkAnnotation` uses an absolute file path. Moving the files to another machine may result in "file not found" errors in PDF reader applications.

---

## Key APIs

| Member | Description |
| --- | --- |
| `PdfTextWebLink` | Renders clickable text that navigates to a URL |
| `PdfTextWebLink.Url` | The URL to navigate to when the link is clicked |
| `PdfTextWebLink.Text` | The display text of the hyperlink |
| `PdfTextWebLink.Font` | Font used to render the link text |
| `PdfTextWebLink.DrawTextWebLink(PdfPageBase, PointF)` | Draws the web link on a new or loaded PDF page |
| `PdfDocumentLinkAnnotation(RectangleF)` | Creates an annotation for navigating within the same document |
| `PdfDocumentLinkAnnotation.Destination` | `PdfDestination` that specifies the target page and location |
| `PdfDestination(PdfPageBase)` | Defines a navigation target by page reference |
| `PdfDestination.Location` | `PointF` specifying the scroll position on the destination page |
| `PdfDestination.Zoom` | Zoom level applied when navigating to the destination |
| `PdfFileLinkAnnotation(RectangleF, string)` | Creates an annotation that opens an external file by path |
| `PdfAnnotationFlags.NoRotate` | Prevents the annotation from rotating with the page |

---

## Notes

- `PdfTextWebLink` is the simplest way to render a clickable URL with visible link text on a page.
- Use `PdfDocumentLinkAnnotation` for in-document navigation (e.g., table of contents).
- `PdfFileLinkAnnotation` uses absolute file paths — links may break if files are moved to another machine.
- For named destination–based navigation, see [named-destinations.md](named-destinations.md).

---

## Related

- [annotations.md](annotations.md)
- [named-destinations.md](named-destinations.md)
- [actions.md](actions.md)
- ../SKILL.md

## Official documentation

- <https://help.syncfusion.com/document-processing/pdf/pdf-library/net/working-with-hyperlinks>
