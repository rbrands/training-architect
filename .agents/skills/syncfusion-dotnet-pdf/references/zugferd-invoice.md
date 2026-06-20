# ZUGFeRD Invoice

Create, attach, and extract ZUGFeRD electronic invoice documents in PDF/A-3b compliant PDFs using Syncfusion .NET PDF Library.

*Note: For document creation, loading, and save/close patterns, see [document-structure.md](document-structure.md). For PDF/A conformance setup, see [conformance.md](conformance.md). For general file attachments, see [attachments.md](attachments.md).*

---

**Common namespaces:**

```csharp
using Syncfusion.Pdf.Interactive;
using Syncfusion.Pdf;
using Syncfusion.Pdf.Parsing;
```

---

## Overview

ZUGFeRD (Zentraler User Guide des Forums elektronische Rechnung Deutschland) is a standardized electronic invoice format based on the ISO PDF/A-3b standard. A ZUGFeRD invoice embeds both a human-readable PDF and a machine-readable XML within the same document.

---

## ZUGFeRD Versions

| Version | Description |
| --- | --- |
| `ZugferdVersion.ZugferdVersion1_0` | ZUGFeRD 1.0 (default) |
| `ZugferdVersion.ZugferdVersion2_0` | ZUGFeRD 2.0 |
| `ZugferdVersion.FacturX` | Factur-X (French/EU standard) |

---

## ZUGFeRD Conformance Levels

| Level | Description | Supported Versions |
| --- | --- | --- |
| `ZugferdConformanceLevel.Basic` | Structured data for simple invoices; free text allowed | 1.0, 2.0, Factur-X |
| `ZugferdConformanceLevel.Comfort` | Fully automated invoice processing | 1.0, 2.0, Factur-X |
| `ZugferdConformanceLevel.Extended` | Additional data for cross-industry exchange | 1.0, 2.0, Factur-X |
| `ZugferdConformanceLevel.Minimum` | Basic invoice details (French Factur-X) | 2.0 only |
| `ZugferdConformanceLevel.EN16931` | Fully EU-compliant core invoice elements | 2.0 only |
| `ZugferdConformanceLevel.XRechnung` | Germany's e-invoicing regulations (EN 16931) | Factur-X only |

---

## Create a ZUGFeRD Invoice PDF (Basic)

Create a PDF/A-3b document with ZUGFeRD conformance level set to Basic and attach the structured XML invoice data.

```csharp
using Syncfusion.Pdf;
using Syncfusion.Pdf.Interactive;

// Create ZUGFeRD invoice PDF document with PDF/A-3b conformance
PdfDocument document = new PdfDocument(PdfConformanceLevel.Pdf_A3B);

// Set ZUGFeRD conformance level
document.ZugferdConformanceLevel = ZugferdConformanceLevel.Basic;

// Attach the structured XML invoice (ZUGFeRD 1.0: file must be named "ZUGFeRD-invoice.xml")
FileStream invoiceStream = new FileStream("ZUGFeRD-invoice.xml", FileMode.Open, FileAccess.Read);
PdfAttachment attachment = new PdfAttachment("ZUGFeRD-invoice.xml", invoiceStream);
attachment.Relationship = PdfAttachmentRelationship.Alternative;
attachment.ModificationDate = DateTime.Now;
attachment.Description = "ZUGFeRD-invoice";
attachment.MimeType = "application/xml";

// Add attachment to PDF document
document.Attachments.Add(attachment);
```

---

## Create a ZUGFeRD 2.0 Invoice PDF

Specify `ZugferdVersion2_0` to produce a ZUGFeRD 2.0 compliant invoice document.

```csharp
using Syncfusion.Pdf;
using Syncfusion.Pdf.Interactive;

// Create ZUGFeRD invoice PDF document with PDF/A-3b conformance
PdfDocument document = new PdfDocument(PdfConformanceLevel.Pdf_A3B);

// Set ZUGFeRD version to 2.0 (default is ZugferdVersion1_0)
document.ZugferdVersion = ZugferdVersion.ZugferdVersion2_0;

// Set ZUGFeRD conformance level
document.ZugferdConformanceLevel = ZugferdConformanceLevel.Basic;

// Attach the structured XML invoice (ZUGFeRD 2.0: file must be named "zugferd-invoice.xml")
FileStream invoiceStream = new FileStream("zugferd-invoice.xml", FileMode.Open, FileAccess.Read);
PdfAttachment attachment = new PdfAttachment("zugferd-invoice.xml", invoiceStream);
attachment.Relationship = PdfAttachmentRelationship.Alternative;
attachment.ModificationDate = DateTime.Now;
attachment.Description = "ZUGFeRD-invoice";
attachment.MimeType = "application/xml";

// Add attachment to PDF document
document.Attachments.Add(attachment);
```

---

## Create a Factur-X Invoice (XRechnung)

Create a Factur-X invoice using `XRechnung` conformance level, which aligns with Germany's e-invoicing regulations.

```csharp
using Syncfusion.Pdf;
using Syncfusion.Pdf.Interactive;

// Create ZUGFeRD invoice PDF document with PDF/A-3b conformance
PdfDocument document = new PdfDocument(PdfConformanceLevel.Pdf_A3B);

// Set ZUGFeRD version to Factur-X
document.ZugferdVersion = ZugferdVersion.FacturX;

// Set ZUGFeRD conformance level to XRechnung (only supported with Factur-X)
document.ZugferdConformanceLevel = ZugferdConformanceLevel.XRechnung;

// Attach the structured XML invoice (Factur-X XRechnung: file must be named "xrechnung.xml")
FileStream invoiceStream = new FileStream("xrechnung.xml", FileMode.Open, FileAccess.Read);
PdfAttachment attachment = new PdfAttachment("xrechnung.xml", invoiceStream);
attachment.Relationship = PdfAttachmentRelationship.Alternative;
attachment.ModificationDate = DateTime.Now;
attachment.Description = "ZUGFeRD-Xrechnung";
attachment.MimeType = "text/xml";

// Add attachment to PDF document
document.Attachments.Add(attachment);
```

---

## XML File Naming Requirements

> **Important:** As per the ZUGFeRD standard, the XML attachment file name must follow these rules:

| Version | Required File Name |
| --- | --- |
| ZUGFeRD 1.0 | `ZUGFeRD-invoice.xml` |
| ZUGFeRD 2.0 | `zugferd-invoice.xml` |
| Factur-X | `factur-x.xml` |
| Factur-X XRechnung | `xrechnung.xml` |

---

## Extract ZUGFeRD Invoice XML from PDF

Extract the embedded XML invoice data from an existing ZUGFeRD PDF document.

```csharp
using Syncfusion.Pdf.Interactive;
using Syncfusion.Pdf.Parsing;

// Load the ZUGFeRD PDF document
PdfLoadedDocument document = new PdfLoadedDocument("Sample.pdf");

// Iterate through attachments and extract the ZUGFeRD XML
foreach (PdfAttachment attachment in document.Attachments)
{
    // Save the extracted XML invoice to disk
    FileStream outputStream = new FileStream(attachment.FileName, FileMode.Create);
    outputStream.Write(attachment.Data, 0, attachment.Data.Length);
    outputStream.Dispose();
}
```

---

## Key APIs

| Member | Description |
| --- | --- |
| `PdfDocument(PdfConformanceLevel.Pdf_A3B)` | Creates a PDF/A-3b compliant document required for ZUGFeRD |
| `PdfDocument.ZugferdConformanceLevel` | Gets/sets the ZUGFeRD conformance level (`ZugferdConformanceLevel` enum) |
| `PdfDocument.ZugferdVersion` | Gets/sets the ZUGFeRD version (`ZugferdVersion` enum); defaults to `ZugferdVersion1_0` |
| `PdfAttachment(string, Stream)` | Creates a file attachment from a stream with the given file name |
| `PdfAttachment.Relationship` | Relationship type for the attachment — use `PdfAttachmentRelationship.Alternative` for ZUGFeRD |
| `PdfAttachment.MimeType` | MIME type of the attached file (e.g., `"application/xml"`, `"text/xml"`) |
| `PdfAttachment.Description` | Human-readable description of the attachment |
| `PdfAttachment.ModificationDate` | Last modification date of the attachment |
| `PdfDocument.Attachments.Add()` | Adds a `PdfAttachment` to the document |

---

## Notes

- ZUGFeRD invoices require `PdfConformanceLevel.Pdf_A3B` — this is the only conformance level that supports external file attachments.
- The `Minimum` and `EN16931` conformance levels are only available with `ZugferdVersion2_0`.
- The `XRechnung` conformance level is only available with `ZugferdVersion.FacturX`.
- The default ZUGFeRD version is `ZugferdVersion1_0` if not explicitly set.
- ZUGFeRD compliance can be validated using the **Preflight** tool in Adobe Acrobat (Tools > Print Production > Preflight > ZUGFeRD profile).

---

## Related

- [conformance.md](conformance.md)
- [attachments.md](attachments.md)
- [metadata.md](metadata.md)
- ../SKILL.md

## Official documentation

- <https://help.syncfusion.com/document-processing/pdf/pdf-library/net/working-with-zugferd-invoice>
