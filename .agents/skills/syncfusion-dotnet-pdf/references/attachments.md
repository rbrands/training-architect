# PDF Attachments

Add, manage, and extract file attachments in PDF documents using Syncfusion .NET PDF Library.

*Note: For document creation, loading, and save/close patterns, see [document-structure.md](document-structure.md).*

---

## Add attachment to a new PDF

Embed files as attachments in a new PDF document using the `PdfAttachment` class.

```csharp
using Syncfusion.Pdf.Interactive;

// Create an attachment from a file stream
Stream fileStream = new FileStream("Input.txt", FileMode.Open, FileAccess.Read);
PdfAttachment attachment = new PdfAttachment("Input.txt", fileStream);
attachment.ModificationDate = DateTime.Now;
attachment.Description = "Input.txt";
attachment.MimeType = "application/txt";

// Add the attachment to the document
document.Attachments.Add(attachment);
```

---

## Add attachment to existing PDF

Add attachments to an already created PDF document.

```csharp
using Syncfusion.Pdf.Interactive;

// Create an attachment
Stream fileStream = new FileStream("Input.txt", FileMode.Open, FileAccess.Read);
PdfAttachment attachment = new PdfAttachment("Input.txt", fileStream);
attachment.ModificationDate = DateTime.Now;
attachment.Description = "Input.txt";
attachment.MimeType = "application/txt";

// Create attachments section if needed
if (loadedDocument.Attachments == null)
    loadedDocument.CreateAttachment();

// Add the attachment
loadedDocument.Attachments.Add(attachment);
```

---

## Remove attachment from PDF

Delete attachments from an existing PDF document by index or reference.

```csharp
using Syncfusion.Pdf.Interactive;

// Remove attachment by index
document.Attachments.RemoveAt(0);

// Or remove by reference
// PdfAttachment attachment = document.Attachments[0];
// document.Attachments.Remove(attachment);
```

---

## Extract and save attachment to disk

Extract embedded attachments and save them to the file system.

```csharp
using Syncfusion.Pdf.Interactive;

// Iterate through attachments and save to disk
foreach (PdfAttachment attachment in document.Attachments)
{
    // Extract and save attachment
    FileStream s = new FileStream(attachment.FileName, FileMode.Create);
    s.Write(attachment.Data, 0, attachment.Data.Length);
    s.Dispose();
    
    // Access attachment metadata
    string mimeType = attachment.MimeType;
    DateTime creationDate = attachment.CreationDate;
    string description = attachment.Description;
}
```

---

## Add interactive launch buttons for attachments

Create button fields that trigger opening of attached PDFs using JavaScript actions.

```csharp
using Syncfusion.Drawing;
using Syncfusion.Pdf.Interactive;

// Create a PDF attachment
PdfAttachment attachment = new PdfAttachment("Attachment.pdf", 
    File.ReadAllBytes("Attachment.pdf"));
attachment.Description = "Attachment";

// Create attachments section if needed
if (loadedDocument.Attachments == null)
    loadedDocument.CreateAttachment();

loadedDocument.Attachments.Add(attachment);

// Create a button field
PdfButtonField buttonField = new PdfButtonField(lpage, "Button");
buttonField.Bounds = new RectangleF(100, 100, 100, 20);
buttonField.BorderColor = new PdfColor(Color.Black);
buttonField.BackColor = new PdfColor(Color.LightGray);
buttonField.Text = "Click Me";
buttonField.Font = new PdfStandardFont(PdfFontFamily.Helvetica, 12);

// Add JavaScript action to open the attachment
buttonField.Actions.MouseUp = new PdfJavaScriptAction(
    "this.exportDataObject({ cName: \"Attachment.pdf\", nLaunch: 2 });");

// Create form if needed
if (loadedDocument.Form == null)
    loadedDocument.CreateForm();

// Add button to form
loadedDocument.Form.Fields.Add(buttonField);
loadedDocument.Form.SetDefaultAppearance(false);
```

---

## Attachment Properties

| Property | Type | Purpose |
|---|---|---|
| `FileName` | string | Name of the embedded file |
| `Description` | string | Human-readable description of the attachment |
| `ModificationDate` | DateTime | Last modification date of the attachment |
| `CreationDate` | DateTime | Creation date of the attachment |
| `MimeType` | string | MIME type (e.g., "application/pdf", "image/png") |
| `Data` | byte[] | Binary content of the attachment |
| `Relationship` | `PdfAttachmentRelationship` | Relationship type (Unspecified, Source, Data, Alternative) |

---

## MIME Type Examples

| File Type | MIME Type |
|---|---|
| PDF | `application/pdf` |
| Text | `application/txt` or `text/plain` |
| Word | `application/msword` |
| Excel | `application/vnd.ms-excel` |
| Image (PNG) | `image/png` |
| Image (JPEG) | `image/jpeg` |
| ZIP | `application/zip` |
