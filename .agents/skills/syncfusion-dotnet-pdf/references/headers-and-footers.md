# PDF Headers and Footers

Add headers and footers to PDF documents using Syncfusion .NET PDF Library with automatic fields and dynamic content.

*Note: For document creation, loading, and save/close patterns, see [document-structure.md](document-structure.md).*

---

**Common namespaces:**

```csharp
using Syncfusion.Drawing;
using Syncfusion.Pdf;
using Syncfusion.Pdf.Graphics;
```

---

## Add header and footer with automatic fields

Use `PdfPageTemplateElement` to create reusable headers and footers that apply to all pages.

```csharp
using Syncfusion.Drawing;
using Syncfusion.Pdf.Graphics;

// Define header bounds
RectangleF bounds = new RectangleF(0, 0, pdfDocument.Pages[0].GetClientSize().Width, 50);

// Create header template
PdfPageTemplateElement header = new PdfPageTemplateElement(bounds);
FileStream imageStream = new FileStream("Logo.png", FileMode.Open, FileAccess.Read);
PdfImage image = new PdfBitmap(imageStream);
header.Graphics.DrawImage(image, new PointF(0, 0), new SizeF(100, 50));
pdfDocument.Template.Top = header;

// Create footer template
PdfPageTemplateElement footer = new PdfPageTemplateElement(bounds);
PdfFont font = new PdfStandardFont(PdfFontFamily.Helvetica, 7);
PdfBrush brush = new PdfSolidBrush(Color.Black);

// Add page number and page count fields
PdfPageNumberField pageNumber = new PdfPageNumberField(font, brush);
PdfPageCountField count = new PdfPageCountField(font, brush);

// Create composite field: "Page X of Y"
PdfCompositeField compositeField = new PdfCompositeField(font, brush, "Page {0} of {1}", pageNumber, count);
compositeField.Bounds = footer.Bounds;
compositeField.Draw(footer.Graphics, new PointF(470, 40));

// Add footer template
pdfDocument.Template.Bottom = footer;
```

### Page number field only

```csharp
PdfPageNumberField pageNumber = new PdfPageNumberField(font, brush);
PdfCompositeField compositeField = new PdfCompositeField(font, brush, "Page {0}", pageNumber);
```

### Page count field only

```csharp
PdfPageCountField count = new PdfPageCountField(font, brush);
PdfCompositeField compositeField = new PdfCompositeField(font, brush, "Total Pages: {0}", count);
```

### Date/time field

```csharp
PdfDateTimeField dateTime = new PdfDateTimeField(font, brush);
PdfCompositeField compositeField = new PdfCompositeField(font, brush, "Generated: {0:yyyy-MM-dd HH:mm:ss}", dateTime);
```

---

## Add dynamic headers and footers per page

Use the `PageAdded` event to customize headers/footers uniquely for each page.

```csharp
using Syncfusion.Drawing;
using Syncfusion.Pdf.Graphics;

// Subscribe to PageAdded event
document.Pages.PageAdded += (sender, e) => PageAddedHandler(sender, e);

// Define content font and brush
PdfFont contentFont = new PdfStandardFont(PdfFontFamily.TimesRoman, 18);
PdfBrush contentBrush = new PdfSolidBrush(Color.Black);

// Define overflow text for multi-page content
string overflowText = @"Your document content here...";

// Set header and footer heights
float headerHeight = 40f;
float footerHeight = 30f;

// Create a text element with automatic pagination
PdfTextElement textElement = new PdfTextElement(overflowText, contentFont, contentBrush);

// Subscribe to BeginPageLayout event to reserve space for headers/footers
textElement.BeginPageLayout += (sender, args) =>
{
    args.Bounds = new RectangleF(
        0, headerHeight,
        args.Page.GetClientSize().Width,
        args.Page.GetClientSize().Height - headerHeight - footerHeight
    );
};

// Add first page and start drawing content
PdfPage firstPage = document.Pages.Add();
textElement.Draw(firstPage, new PointF(0, headerHeight));

// Event handler: called for each page added
static void PageAddedHandler(object sender, PageAddedEventArgs e)
{
    PdfPage page = e.Page;
    int currentPage = page.Section.Pages.IndexOf(page) + 1;

    // Draw header at the top
    string headerText = $"This is the header - Page {currentPage}";
    page.Graphics.DrawString(
        headerText,
        new PdfStandardFont(PdfFontFamily.Helvetica, 14, PdfFontStyle.Bold),
        new PdfSolidBrush(Color.DimGray),
        new PointF(10, 10)
    );

    // Draw footer at the bottom
    string timestamp = DateTime.Now.ToString("'Date:' yyyy-MM-dd 'Time:' HH:mm:ss");
    string footerText = $"Page {currentPage} {timestamp}";
    page.Graphics.DrawString(
        footerText,
        new PdfStandardFont(PdfFontFamily.Helvetica, 12),
        new PdfSolidBrush(Color.Black),
        new PointF(10, page.GetClientSize().Height - 30)
    );
}
```

---

## Key Classes & Events

| Class/Event | Purpose |
| --- | --- |
| `PdfPageTemplateElement` | Template for applying consistent headers/footers across all pages |
| `PdfPageNumberField` | Automatic current page number |
| `PdfPageCountField` | Automatic total page count |
| `PdfDateTimeField` | Automatic date/time stamp |
| `PdfCompositeField` | Combine multiple fields in one template (e.g., "Page 1 of 10") |
| `PageAdded` event | Triggered when a new page is added; use for custom per-page headers/footers |
| `BeginPageLayout` event | Triggered before page layout; use to reserve space for headers/footers |

---

## Tips

- **Template headers/footers** are applied to all pages automatically; ideal for consistent branding or pagination
- **Dynamic headers/footers** via `PageAdded` event allow unique content per page (e.g., section-specific headers)
- Always reserve space in `BeginPageLayout` if adding content to avoid overlap with headers/footers
- Use `PdfCompositeField` to format multiple fields as a single template string
