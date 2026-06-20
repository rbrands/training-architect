# PDF Portfolio

Create PDF portfolios that embed multiple files within a single PDF container using Syncfusion .NET PDF Library.

*Note: For document creation, loading, and save/close patterns, see [document-structure.md](document-structure.md).*

---

**Common namespaces:**

```csharp
using Syncfusion.Pdf.Interactive;
using Syncfusion.Pdf.Parsing;
```

---


## Create a PDF portfolio

Embed multiple documents and files into a single portfolio PDF with metadata.

```csharp
using Syncfusion.Pdf.Interactive;

// Create portfolio information
document.PortfolioInformation = new PdfPortfolioInformation();
document.PortfolioInformation.ViewMode = PdfPortfolioViewMode.Tile;

// Create PDF attachment
FileStream pdfStream = new FileStream("CorporateBrochure.pdf", FileMode.Open, FileAccess.Read);
PdfAttachment pdfFile = new PdfAttachment("CorporateBrochure.pdf", pdfStream);
pdfFile.FileName = "CorporateBrochure.pdf";
pdfFile.Description = "This is a PDF document";
pdfFile.CreationDate = DateTime.Now;
pdfFile.ModificationDate = DateTime.Now;
pdfFile.MimeType = "application/pdf";
pdfFile.Relationship = PdfAttachmentRelationship.Unspecified;

// Add attachment to portfolio
document.Attachments.Add(pdfFile);

// Set as startup document (opens first when portfolio is opened)
document.PortfolioInformation.StartupDocument = pdfFile;
```

---

## Add multiple files to portfolio

Include various file types in a single portfolio.

```csharp
using Syncfusion.Pdf.Interactive;

// Create portfolio
document.PortfolioInformation = new PdfPortfolioInformation();
document.PortfolioInformation.ViewMode = PdfPortfolioViewMode.Tile;

// Add PDF file
FileStream pdfStream = new FileStream("Document.pdf", FileMode.Open);
PdfAttachment pdfFile = new PdfAttachment("Document.pdf", pdfStream);
pdfFile.MimeType = "application/pdf";
pdfFile.CreationDate = DateTime.Now;
document.Attachments.Add(pdfFile);

// Add Word document
FileStream docStream = new FileStream("Report.docx", FileMode.Open);
PdfAttachment wordFile = new PdfAttachment("Report.docx", docStream);
wordFile.MimeType = "application/vnd.openxmlformats-officedocument.wordprocessingml.document";
wordFile.CreationDate = DateTime.Now;
document.Attachments.Add(wordFile);

// Add Excel spreadsheet
FileStream xlsStream = new FileStream("Data.xlsx", FileMode.Open);
PdfAttachment excelFile = new PdfAttachment("Data.xlsx", xlsStream);
excelFile.MimeType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
excelFile.CreationDate = DateTime.Now;
document.Attachments.Add(excelFile);

// Set first file as startup
document.PortfolioInformation.StartupDocument = pdfFile;
```

---

## Extract files from portfolio

Extract and save all embedded files from an existing portfolio.

```csharp
using Syncfusion.Pdf.Interactive;

// Iterate through all attachments
foreach (PdfAttachment attachment in document.Attachments)
{
    // Create file stream to save attachment
    using (FileStream s = new FileStream(attachment.FileName, FileMode.Create))
    {
        // Write attachment data to file
        s.Write(attachment.Data, 0, attachment.Data.Length);
    }

    // Access attachment metadata
    string mimeType = attachment.MimeType;
    DateTime creationDate = attachment.CreationDate;
    DateTime modificationDate = attachment.ModificationDate;
    string description = attachment.Description;
    PdfAttachmentRelationship relationship = attachment.Relationship;

    // Log or process metadata
    Console.WriteLine($"Saved: {attachment.FileName}");
    Console.WriteLine($"MIME Type: {mimeType}");
    Console.WriteLine($"Description: {description}");
    Console.WriteLine($"Created: {creationDate}, Modified: {modificationDate}");
    Console.WriteLine($"Relationship: {relationship}");
}
```

---

## Remove files from portfolio

Delete specific files from an existing portfolio.

```csharp
using Syncfusion.Pdf.Interactive;

// Remove attachment by index
document.Attachments.RemoveAt(0);

// Or remove by reference
// PdfAttachment attachment = document.Attachments[0];
// document.Attachments.Remove(attachment);
```

---

## Portfolio View Modes

Control how files are displayed in the portfolio.

```csharp
using Syncfusion.Pdf;
using Syncfusion.Pdf.Interactive;

PdfDocument document = new PdfDocument();
document.PortfolioInformation = new PdfPortfolioInformation();

// Available view modes:
document.PortfolioInformation.ViewMode = PdfPortfolioViewMode.Tile;      // Tile view
document.PortfolioInformation.ViewMode = PdfPortfolioViewMode.Details;   // Detail list
document.PortfolioInformation.ViewMode = PdfPortfolioViewMode.Hidden;    // Hidden
```

---

## Attachment Metadata

| Property | Type | Purpose |
| --- | --- | --- |
| `FileName` | string | Name of the embedded file |
| `Description` | string | Human-readable description |
| `CreationDate` | DateTime | When file was created |
| `ModificationDate` | DateTime | Last modification date |
| `MimeType` | string | File type (e.g., "application/pdf") |
| `Relationship` | `PdfAttachmentRelationship` | File relationship type |
| `Data` | byte[] | Binary file contents |

---

## Relationship Types

| Relationship | Use Case |
| --- | --- |
| `Unspecified` | No specific relationship defined |
| `Source` | Source file for the portfolio |
| `Data` | Data file associated with main content |
| `Alternative` | Alternative version of document |

---

## Common MIME Types

| File Type | MIME Type |
| --- | --- |
| PDF | `application/pdf` |
| Word (.docx) | `application/vnd.openxmlformats-officedocument.wordprocessingml.document` |
| Excel (.xlsx) | `application/vnd.openxmlformats-officedocument.spreadsheetml.sheet` |
| PowerPoint (.pptx) | `application/vnd.openxmlformats-officedocument.presentationml.presentation` |
| Image (PNG) | `image/png` |
| Image (JPEG) | `image/jpeg` |
| Text | `text/plain` |
| ZIP Archive | `application/zip` |

---

## Portfolio Use Cases

- **Document Bundles**: Combine related documents into one file
- **Project Packages**: Bundle project files, specs, and artwork
- **Report Collections**: Group multiple reports and supporting files
- **Submission Packages**: Submit various documents as single attachment
- **Reference Archives**: Distribute collections of reference materials
