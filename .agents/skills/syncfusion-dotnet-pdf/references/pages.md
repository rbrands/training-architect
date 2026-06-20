# PDF Pages

Add, insert, remove, rotate, rearrange, and configure pages in PDF documents using Syncfusion .NET PDF Library.

*Note: For document creation, loading, and save/close patterns, see [document-structure.md](document-structure.md). For importing pages between documents, see [merge-pdf.md](merge-pdf.md). For splitting a document into separate files, see [split-pdf.md](split-pdf.md).*

---

**Common namespaces:**

```csharp
using Syncfusion.Drawing;
using Syncfusion.Pdf;
using Syncfusion.Pdf.Graphics;
using Syncfusion.Pdf.Parsing;
```

---

## Add a new page to a PDF document

Use `PdfDocument.Pages.Add()` to append a new page to the end of the document.

```csharp
PdfPage page = document.Pages.Add();
```

---

## Insert a page at a specific position in an existing PDF

Use `PdfLoadedDocument.Pages.Insert(index)` to add an empty page at the specified zero-based index.

```csharp
//Insert a new empty page at the beginning of the document.
loadedDocument.Pages.Insert(0);
```

---

## Insert a duplicate page in an existing PDF

Use `Pages.Insert(index, loadedPage)` to clone an existing page and insert it at any position.

```csharp
PdfLoadedPage loadedPage = loadedDocument.Pages[0] as PdfLoadedPage;
//Insert the duplicate page at the beginning of the document.
loadedDocument.Pages.Insert(0, loadedPage);
```

---

## Insert a new page preserving an existing page's size

Use `Pages.Insert(index, size)` to add a new blank page with the same dimensions as an existing page.

```csharp
PdfLoadedPage loadedPage = loadedDocument.Pages[0] as PdfLoadedPage;
//Insert a blank page at position 0 with the same size as the first page.
PdfPageBase page = loadedDocument.Pages.Insert(0, loadedPage.Size);
```

---

## Set margins for all pages

Use `document.PageSettings.Margins` to apply uniform margins to all pages.

```csharp
//Set margin for all pages (default is 40 points).
document.PageSettings.Margins.All = 10;
PdfPage page = document.Pages.Add();
```

> **Note:** The default margin is 40 points. Set `Margins.All = 0` for full-bleed content.

---

## Add sections with different page settings

Use `PdfSection` to group pages that share the same size, orientation, or rotation. Each section can have independent `PageSettings`.

```csharp
//Section 1 – A5, 0° rotation
PdfSection section1 = document.Sections.Add();
section1.PageSettings.Rotate = PdfPageRotateAngle.RotateAngle0;
section1.PageSettings.Size   = PdfPageSize.A5;
section1.PageSettings.Width  = 300;
section1.PageSettings.Height = 400;
PdfPage page1 = section1.Pages.Add();

//Section 2 – 90° rotation
PdfSection section2 = document.Sections.Add();
section2.PageSettings.Rotate = PdfPageRotateAngle.RotateAngle90;
section2.PageSettings.Width  = 300;
section2.PageSettings.Height = 400;
PdfPage page2 = section2.Pages.Add();

//Section 3 – 180° rotation
PdfSection section3 = document.Sections.Add();
section3.PageSettings.Rotate = PdfPageRotateAngle.RotateAngle180;
section3.PageSettings.Width  = 500;
section3.PageSettings.Height = 200;
PdfPage page3 = section3.Pages.Add();

//Section 4 – 270° rotation
PdfSection section4 = document.Sections.Add();
section4.PageSettings.Rotate = PdfPageRotateAngle.RotateAngle270;
section4.PageSettings.Width  = 300;
section4.PageSettings.Height = 200;
PdfPage page4 = section4.Pages.Add();
```

---

## Customize section page numbering style

Use `PdfSectionPageNumberField` with a `PdfNumberStyle` (e.g., lower-roman) to style page numbers within a section.

```csharp
PdfSection section = document.Sections.Add();
PdfFont font = new PdfStandardFont(PdfFontFamily.Helvetica, 12);

PdfSectionPageNumberField sectionPageNumber = new PdfSectionPageNumberField();
sectionPageNumber.NumberStyle = PdfNumberStyle.LowerRoman;
sectionPageNumber.Font        = font;

for (int i = 0; i < 3; i++)
{
    PdfPage page     = section.Pages.Add();
    SizeF   pageSize = page.GetClientSize();
    //Draw the section page number in the footer.
    sectionPageNumber.Draw(page.Graphics, new PointF(10, pageSize.Height - 20));
}
```

---

## Get the page count from an existing PDF

```csharp
int pageCount = loadedDocument.Pages.Count;
```

---

## Remove a page from an existing PDF

Use `Pages.RemoveAt(index)` to delete a page by its zero-based index.

```csharp
loadedDocument.Pages.RemoveAt(0);
```

> **Note:** Shared resources (images, fonts) are only removed when all pages that reference them are deleted.

---

## Rearrange pages in an existing PDF

Use `Pages.ReArrange(int[])` with an array of zero-based page indices in the desired order.

```csharp
//Swap page 0 and page 1.
loadedDocument.Pages.ReArrange(new int[] { 1, 0 });
```

---

## Change page number labels in an existing PDF

Use `PdfPageLabel` to alter how page numbers are displayed (e.g., upper-case Roman numerals).

```csharp
PdfPageLabel pageLabel = new PdfPageLabel();
pageLabel.NumberStyle = PdfNumberStyle.UpperRoman;
pageLabel.StartNumber = 1;
loadedDocument.LoadedPageLabel = pageLabel;
```

---

## Rotate a page in a new PDF

Apply rotation to a section's `PageSettings.Rotate` property before adding pages.

```csharp
PdfSection section = document.Sections.Add();
section.PageSettings.Rotate = PdfPageRotateAngle.RotateAngle90;
PdfPage page = section.Pages.Add();
```

---

## Rotate a page in an existing PDF

Set `PdfPageBase.Rotation` on a loaded page to change its orientation.

```csharp
PdfPageBase loadedPage = loadedDocument.Pages[0] as PdfPageBase;
loadedPage.Rotation = PdfPageRotateAngle.RotateAngle90;
```

---

## Detect blank pages in an existing PDF

Use `PdfPageBase.IsBlank` to check whether a page contains no visible content.

```csharp
PdfPageBase loadedPage = loadedDocument.Pages[0] as PdfPageBase;
bool isEmpty = loadedPage.IsBlank;
```

---

## Import a range of pages from another PDF

Use `ImportPageRange` to copy a contiguous range of pages into a new document.

```csharp
int startIndex = 0;
int endIndex   = loadedDocument.Pages.Count - 1;
document.ImportPageRange(loadedDocument, startIndex, endIndex);
```

---

## Import pages without bookmarks

Pass `false` as the fourth argument to skip importing bookmarks (improves performance on large PDFs).

```csharp
int startIndex = 0;
int endIndex   = loadedDocument.Pages.Count - 1;
//Pass false to skip bookmark import.
document.ImportPageRange(loadedDocument, startIndex, endIndex, false);
```

---

## Split pages into individual PDF documents

Iterate through pages and use `ImportPage` to extract each one into its own file.

```csharp
for (int i = 0; i < loadedDocument.PageCount; i++)
{
    PdfDocument document = new PdfDocument();
    document.ImportPage(loadedDocument, i);
    document.Save(Path.Combine("output", $"Page_{i + 1}.pdf"));
    document.Close(true);
}
```

---

## Span a text element across multiple pages (PageAdded event)

Subscribe to `Pages.PageAdded` to draw content (e.g., a border) on each newly created page during layout overflow.

```csharp
//Subscribe to the PageAdded event before adding any pages.
document.Pages.PageAdded += Pages_PageAdded;
PdfPage page = document.Pages.Add();

PdfTextElement textElement = new PdfTextElement(
    File.ReadAllText("Input.txt", System.Text.Encoding.ASCII),
    new PdfStandardFont(PdfFontFamily.TimesRoman, 14));

PdfLayoutFormat layoutFormat = new PdfLayoutFormat
{
    Layout = PdfLayoutType.Paginate,
    Break  = PdfLayoutBreakType.FitPage
};

//Draw first paragraph; result carries the ending bounds and active page.
PdfLayoutResult result = textElement.Draw(page,
    new RectangleF(0, 0, page.GetClientSize().Width / 2, page.GetClientSize().Height),
    layoutFormat);

//Draw second paragraph immediately below the first.
textElement.Draw(result.Page,
    new RectangleF(0, result.Bounds.Bottom + 10,
                   page.GetClientSize().Width / 2, page.GetClientSize().Height),
    layoutFormat);

//Event handler — runs for every auto-created overflow page.
void Pages_PageAdded(object sender, PageAddedEventArgs args)
{
    PdfPage newPage = args.Page;
    newPage.Graphics.DrawRectangle(PdfPens.Black,
        new RectangleF(0, 0, newPage.GetClientSize().Width, newPage.GetClientSize().Height));
}
```

---

## Add page-level open/close actions

Attach JavaScript or URI actions to a page's `OnOpen` and `OnClose` events.

```csharp
using Syncfusion.Pdf.Interactive;

//Page 1 – JavaScript on open, URI on close.
PdfPage page1 = document.Pages.Add();
page1.Actions.OnOpen  = new PdfJavaScriptAction("app.alert(\"Page 1 opened.\");");
page1.Actions.OnClose = new PdfUriAction("http://www.syncfusion.com");

//Page 2 – Sound action on open, Launch action on close.
PdfPage page2 = document.Pages.Add();
PdfSoundAction soundAction = new PdfSoundAction("Startup.wav");
soundAction.Sound.Bits     = 16;
soundAction.Sound.Channels = PdfSoundChannels.Stereo;
soundAction.Sound.Encoding = PdfSoundEncoding.Signed;
soundAction.Volume         = 0.9f;
page2.Actions.OnOpen  = soundAction;
page2.Actions.OnClose = new PdfLaunchAction("logo.png");

//Remove a specific action or clear all actions on a page.
page1.Actions.OnClose = null;
page2.Actions.Clear(true);
```

---

## Key APIs

| Member | Description |
| --- | --- |
| `PdfDocument.Pages.Add()` | Appends a new blank page and returns it |
| `PdfLoadedDocument.Pages.Insert(int)` | Inserts an empty page at the given zero-based index |
| `PdfLoadedDocument.Pages.Insert(int, PdfLoadedPage)` | Inserts a duplicate of an existing page at the specified index |
| `PdfLoadedDocument.Pages.Insert(int, SizeF)` | Inserts a new blank page with the specified size at the given index |
| `PdfLoadedDocument.Pages.RemoveAt(int)` | Removes the page at the given zero-based index |
| `PdfLoadedDocument.Pages.ReArrange(int[])` | Reorders pages according to the supplied index array |
| `PdfDocument.Pages.Count` / `PdfLoadedDocument.Pages.Count` | Total number of pages in the document |
| `PdfPageBase.IsBlank` | `true` if the page contains no visible content |
| `PdfPageBase.Rotation` | Gets or sets the page rotation (`PdfPageRotateAngle` enum) |
| `PdfPageBase.Size` | Gets the page dimensions as `SizeF` |
| `PdfPageBase.GetClientSize()` | Returns the usable area after margins as `SizeF` |
| `PdfDocument.PageSettings` | Global page settings (margins, size, orientation) applied to all pages |
| `PdfDocument.PageSettings.Margins.All` | Sets equal margins on all four sides in points (default: 40) |
| `PdfSection` | Groups pages that share the same layout settings |
| `PdfSection.PageSettings` | Per-section settings: `Size`, `Width`, `Height`, `Orientation`, `Rotate`, `Margins`, `Transition` |
| `PdfPageSettings.Rotate` | Rotation angle for new pages: `RotateAngle0`, `RotateAngle90`, `RotateAngle180`, `RotateAngle270` |
| `PdfPageSettings.Size` | Page size preset (e.g., `PdfPageSize.A4`, `PdfPageSize.A5`, `PdfPageSize.Letter`) |
| `PdfPageSettings.Orientation` | `PdfPageOrientation.Portrait` or `PdfPageOrientation.Landscape` |
| `PdfPageLabel` | Defines the number style and start number for page labels |
| `PdfPageLabel.NumberStyle` | `PdfNumberStyle` enum: `Numeric`, `UpperRoman`, `LowerRoman`, `UpperAlpha`, `LowerAlpha` |
| `PdfPageLabel.StartNumber` | The logical page number assigned to the first page of the label range |
| `PdfLoadedDocument.LoadedPageLabel` | Assigns a `PdfPageLabel` to an existing document |
| `PdfSectionPageNumberField` | Automatic field that renders the current section page number |
| `PdfSectionPageNumberField.NumberStyle` | Number style for the section page number (same `PdfNumberStyle` enum) |
| `PdfDocumentBase.ImportPageRange(PdfLoadedDocument, int, int)` | Copies a page range from a loaded document into the target document |
| `PdfDocumentBase.ImportPageRange(PdfLoadedDocument, int, int, bool)` | Same as above; `false` skips bookmark import (faster for large documents) |
| `PdfDocumentBase.ImportPage(PdfLoadedDocument, int)` | Copies a single page from a loaded document |
| `PdfDocument.Pages.PageAdded` | Event fired when a new page is created; provides `PageAddedEventArgs.Page` |
| `PageAddedEventArgs.Page` | The newly created `PdfPage` instance inside the event handler |
| `PdfPage.Actions.OnOpen` | Action triggered when the page is opened in a viewer |
| `PdfPage.Actions.OnClose` | Action triggered when the page is closed/navigated away from |
| `PdfPage.Actions.Clear(bool)` | Removes all page-level actions; `true` also clears inherited actions |
| `PdfPageRotateAngle` | Enum: `RotateAngle0`, `RotateAngle90`, `RotateAngle180`, `RotateAngle270` |

---

## Notes

- `Pages.Add()` always appends to the end; use `Pages.Insert(index)` to place a page at a specific position.
- `ReArrange` uses **zero-based** indices; supplying `new int[] { 1, 0 }` swaps the first two pages.
- Shared resources (images, fonts) are only removed when **all** pages referencing them are deleted — removing a single page does not reduce file size if the resource is still used elsewhere.
- `IsBlank` checks for any visible content stream; a page with only invisible objects (e.g., zero-opacity graphics) may still return `false`.
- The default page margin is **40 points**. Set `Margins.All = 0` for full-bleed layouts such as backgrounds or watermarks.
- `PageAdded` is the recommended hook for drawing repeating per-page content (headers, borders) during paginated layout rather than looping after the fact.

---

## Related

- [document-structure.md](document-structure.md)
- [merge-pdf.md](merge-pdf.md)
- [split-pdf.md](split-pdf.md)
- [headers-and-footers.md](headers-and-footers.md)
- [bookmarks.md](bookmarks.md)
- [actions.md](actions.md)
- ../SKILL.md

## Official documentation

- <https://help.syncfusion.com/document-processing/pdf/pdf-library/net/working-with-pages>
