# PDF Metadata

Add and manage XMP metadata in PDF documents using Syncfusion .NET PDF Library.

*Note: For document creation, loading, and save/close patterns, see [document-structure.md](document-structure.md).*

---

**Common namespaces:**

```csharp
using Syncfusion.Pdf;
using Syncfusion.Pdf.Parsing;
using Syncfusion.Pdf.Xmp;
```

---

## Add XMP metadata to new PDF

Embed metadata properties in a new PDF document using the XMP (Extensible Metadata Platform) standard.

```csharp
using Syncfusion.Pdf;
using Syncfusion.Pdf.Xmp;

// Get XMP metadata object
XmpMetadata metaData = pdfDoc.DocumentInformation.XmpMetadata;

// XMP Basic Schema
BasicSchema basic = metaData.BasicSchema;
basic.Advisory.Add("advisory");
basic.BaseURL = new Uri("http://google.com");
basic.CreateDate = DateTime.Now;
basic.CreatorTool = "creator tool";
basic.Identifier.Add("identifier");
basic.Label = "label";
basic.MetadataDate = DateTime.Now;
basic.ModifyDate = DateTime.Now;
basic.Nickname = "nickname";
basic.Rating.Add(-25);
```

---

## Add XMP metadata to existing PDF

Update metadata in an already created PDF document.

```csharp
using Syncfusion.Pdf.Xmp;

// Get XMP metadata object from loaded document
XmpMetadata metaData = pdfDoc.DocumentInformation.XmpMetadata;

// XMP Basic Schema
BasicSchema basic = metaData.BasicSchema;
basic.Advisory.Add("advisory");
basic.BaseURL = new Uri("http://google.com");
basic.CreateDate = DateTime.Now;
basic.CreatorTool = "creator tool";
basic.Identifier.Add("identifier");
basic.Label = "label";
basic.MetadataDate = DateTime.Now;
basic.ModifyDate = DateTime.Now;
basic.Nickname = "nickname";
basic.Rating.Add(-25);
```

---

## Basic Schema

Add basic descriptive information to the PDF using the `BasicSchema` class.

```csharp
XmpMetadata metaData = pdfDoc.DocumentInformation.XmpMetadata;

// XMP Basic Schema
BasicSchema basic = metaData.BasicSchema;
basic.Advisory.Add("advisory");              // Advisory information
basic.BaseURL = new Uri("http://google.com"); // Base URL
basic.CreateDate = DateTime.Now;             // Creation date
basic.CreatorTool = "creator tool";          // Tool that created file
basic.Identifier.Add("identifier");          // Unique identifier
basic.Label = "label";                       // Label for document
basic.MetadataDate = DateTime.Now;           // Metadata date
basic.ModifyDate = DateTime.Now;             // Modification date
basic.Nickname = "nickname";                 // Nickname of document
basic.Rating.Add(-25);                       // Rating
```

---

## Dublin Core Schema

Add standardized metadata properties using the `DublinCoreSchema` class.

```csharp
XmpMetadata metaData = pdfDoc.DocumentInformation.XmpMetadata;

// XMP Dublin Core Schema
DublinCoreSchema dublin = metaData.DublinCoreSchema;
dublin.Creator.Add("Syncfusion");
dublin.Description.Add("Title", "Essential PDF creator");
dublin.Title.Add("Resource name", "Documentation");
dublin.Type.Add("PDF");
dublin.Publisher.Add("Essential PDF");
dublin.Coverage.Add("coverage");
dublin.Date.Add(DateTime.Now);
dublin.Contributor.Add("contributor");
dublin.Format.Add("PDF");
dublin.Language.Add("en");
```

---

## Rights Management Schema

Add copyright and legal restrictions metadata using the `RightsManagementSchema` class.

```csharp
XmpMetadata metaData = pdfDoc.DocumentInformation.XmpMetadata;

// XMP Rights Management Schema
RightsManagementSchema rights = metaData.RightsManagementSchema;
rights.Certificate = new Uri("http://syncfusion.com");
rights.Owner.Add("Syncfusion");
rights.Marked = true;
rights.UsageTerm = "Usage terms";
```

---

## Basic Job Ticket Schema

Add workflow and job information using the `BasicJobTicketSchema` class.

```csharp
XmpMetadata metaData = pdfDoc.DocumentInformation.XmpMetadata;

// XMP Basic Job Ticket Schema
BasicJobTicketSchema basicJob = metaData.BasicJobTicketSchema;
basicJob.JobRef.Add("PDF document creation");
```

---

## Paged-Text Schema

Add properties related to text appearance on pages using the `PagedTextSchema` class.

```csharp
XmpMetadata metaData = pdfDoc.DocumentInformation.XmpMetadata;

// XMP Paged Text Schema
PagedTextSchema pagedText = metaData.PagedTextSchema;
pagedText.MaxPageSize.Width = 500;
pagedText.MaxPageSize.Height = 750;
pagedText.NPages = 1;
pagedText.PlateNames.Add("Sample page");
pagedText.Colorants.Add("Colorant");
```

---

## PDF Schema

Add PDF-specific properties using the `PDFSchema` class.

```csharp
XmpMetadata metaData = pdfDoc.DocumentInformation.XmpMetadata;

// XMP PDF Schema
PDFSchema pdfSchema = metaData.PDFSchema;
pdfSchema.Producer = "Syncfusion";          // PDF producer/creator
pdfSchema.PDFVersion = "1.5";               // PDF version
pdfSchema.Keywords = "Essential PDF";       // Keywords
pdfSchema.Subject = "PDF creation";         // Subject
```

---

## Custom Schema

Define custom metadata properties using the `CustomSchema` class.

```csharp
XmpMetadata metaData = pdfDoc.DocumentInformation.XmpMetadata;

// Create custom schema field
CustomSchema customSchema = new CustomSchema(metaData, "custom", "http://www.syncfusion.com");
customSchema["creationDate"] = DateTime.Now.ToString();
customSchema["DOCID"] = "SYNCSAM001";
customSchema["Encryption"] = "Standard";
customSchema["Project"] = "Data processing";
```

---

## Add custom schema to existing PDF

Add custom schema with custom metadata container to an existing document.

```csharp
// Create XML document container with existing metadata
XmpMetadata metaData = new XmpMetadata(pdfDoc.DocumentInformation.XmpMetadata.XmlData);

// Create custom schema
CustomSchema customSchema = new CustomSchema(metaData, "custom", "http://www.syncfusion.com");
customSchema["Author"] = "Syncfusion";
customSchema["creationDate"] = DateTime.Now.ToString();
customSchema["DOCID"] = "SYNCSAM001";
```

---

## Add custom metadata

Add key-value pairs of custom metadata directly to document information.

```csharp
// Add custom metadata
pdfDoc.DocumentInformation.CustomMetadata["ID"] = "IO1";
pdfDoc.DocumentInformation.CustomMetadata["CompanyName"] = "Syncfusion";
pdfDoc.DocumentInformation.CustomMetadata["Key"] = "DocumentKey";
pdfDoc.DocumentInformation.CustomMetadata["Department"] = "Engineering";
```

---

## Remove custom metadata

Delete custom metadata from an existing PDF document.

```csharp
using Syncfusion.Pdf.Parsing;

// Load the PDF document
PdfLoadedDocument loadedDocument = new PdfLoadedDocument("Input.pdf");

// Remove custom metadata using key name
loadedDocument.DocumentInformation.CustomMetadata.Remove("Key");
loadedDocument.DocumentInformation.CustomMetadata.Remove("ID");
```

---

## Add XMP metadata with image

Embed images with XMP metadata into a PDF document.

```csharp
using Syncfusion.Pdf.Graphics;

// Load image from stream (preserves XMP metadata)
FileStream imageStream = new FileStream("Image.jpg", FileMode.Open, FileAccess.Read);
PdfBitmap image = new PdfBitmap(imageStream, true);  // true = preserve XMP metadata

// Draw the image
page.Graphics.DrawImage(image, 0, 0);
```

---

## Extract XMP metadata from image

Extract metadata from images embedded in a PDF document.

```csharp
using Syncfusion.Pdf.Xmp;

// Extract all images from first page
PdfImageInfo[] imagesInfo = pageBase.GetImagesInfo();

// Extract XMP metadata from first image
XmpMetadata metadata = imagesInfo[0].XmpMetadata;

// Access metadata properties
if (metadata != null)
{
    BasicSchema basic = metadata.BasicSchema;
    DublinCoreSchema dublin = metadata.DublinCoreSchema;
    PDFSchema pdfSchema = metadata.PDFSchema;
    
    // Process metadata...
}
```

---

## Supported Schema Types

| Schema | Purpose | Key Properties |
| --- | --- | --- |
| **Basic Schema** | Basic descriptive info | Creator Tool, Creation Date, Modification Date, Label, Rating |
| **Dublin Core Schema** | Standard metadata | Title, Creator, Subject, Description, Publisher, Date, Language |
| **Rights Management Schema** | Copyright & legal | Owner, Certificate, Marked, Usage Terms |
| **Basic Job Ticket Schema** | Workflow information | Job References |
| **Paged-Text Schema** | Text appearance | Page Size, Plate Names, Colorants, Page Count |
| **PDF Schema** | PDF-specific info | Producer, PDF Version, Keywords |
| **Custom Schema** | User-defined metadata | Arbitrary key-value pairs |

---

## Basic Schema Properties

| Property | Type | Purpose |
| --- | --- | --- |
| `Advisory` | Collection | Advisory information |
| `BaseURL` | Uri | Base URL for document |
| `CreateDate` | DateTime | When document was created |
| `CreatorTool` | string | Tool that created document |
| `Identifier` | Collection | Unique identifier(s) |
| `Label` | string | Display label |
| `MetadataDate` | DateTime | When metadata was modified |
| `ModifyDate` | DateTime | When document was last modified |
| `Nickname` | string | Informal name |
| `Rating` | Collection | Rating value(s) |

---

## XMP Overview

**XMP (Extensible Metadata Platform)** is a standard for embedding metadata in files. It provides:

- **Standardized format** for metadata across applications
- **Extensibility** for custom properties
- **Preservation** when files are processed
- **Interoperability** across different tools and platforms

---

## Use Cases

- **Document Properties**: Author, creation date, modification date, title
- **Copyright Information**: Owner, rights, usage terms
- **Custom Business Data**: Project ID, cost center, department
- **Product Information**: Version, status, category
- **Image Metadata**: Camera settings, copyright, location (for embedded images)
- **Workflow**: Job ticket information for print production
