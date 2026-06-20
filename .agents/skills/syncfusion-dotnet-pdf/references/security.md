# PDF Security

Encrypt, protect, and manage security in PDF documents using Syncfusion .NET PDF Library.

*Note: For document creation, loading, and save/close patterns, see [document-structure.md](document-structure.md).*

---

**Common namespaces:**

```csharp
using Syncfusion.Drawing;
using Syncfusion.Pdf;
using Syncfusion.Pdf.Graphics;
using Syncfusion.Pdf.Security;
```

---

## RC4 encryption with user password

Encrypt a PDF with RC4 algorithm (40-bit or 128-bit key) and require a user password to open.

```csharp
using Syncfusion.Pdf.Security;

// Configure security
PdfSecurity security = document.Security;
security.KeySize = PdfEncryptionKeySize.Key128Bit;
security.Algorithm = PdfEncryptionAlgorithm.RC4;
security.UserPassword = "password";  // Password required to open document
```

---

## RC4 encryption with owner password

Restrict document permissions (printing, editing, copying) with an owner password.

```csharp
using Syncfusion.Pdf.Security;

// Configure security
PdfSecurity security = document.Security;
security.KeySize = PdfEncryptionKeySize.Key128Bit;
security.Algorithm = PdfEncryptionAlgorithm.RC4;
security.OwnerPassword = "syncfusion";  // Password to change permissions
security.UserPassword = "password";      // Password to open document
security.Permissions = PdfPermissionsFlags.Print | PdfPermissionsFlags.AccessibilityCopyContent;

document.Save("Output.pdf");
document.Close(true);
```

---

## AES encryption with 256-bit key

Use modern AES-256 encryption for strong security.

```csharp
using Syncfusion.Pdf.Security;


// Configure security with AES-256
PdfSecurity security = document.Security;
security.KeySize = PdfEncryptionKeySize.Key256Bit;
security.Algorithm = PdfEncryptionAlgorithm.AES;
security.UserPassword = "password";

```

---

## AES-GCM encryption (PDF 2.0)

Use AES-GCM algorithm for authenticated encryption (PDF 2.0 only).

```csharp
using Syncfusion.Pdf.Security;


// Set document version to 2.0 (required for AES-GCM)
document.FileStructure.Version = PdfVersion.Version2_0;

PdfPage page = document.Pages.Add();
PdfGraphics graphics = page.Graphics;

// Configure AES-GCM security
PdfSecurity security = document.Security;
security.KeySize = PdfEncryptionKeySize.Key256Bit;
security.Algorithm = PdfEncryptionAlgorithm.AESGCM;
security.OwnerPassword = "ownerPassword";
security.UserPassword = "userPassword";

// Add content
graphics.DrawString("Encrypted document with AES-GCM 256bit", 
    new PdfStandardFont(PdfFontFamily.TimesRoman, 15f), 
    PdfBrushes.Black, new PointF(0, 40));

```

---

## Encryption options

Control what content gets encrypted in the PDF.

```csharp
using Syncfusion.Pdf.Security;

PdfSecurity security = document.Security;
security.KeySize = PdfEncryptionKeySize.Key256Bit;
security.Algorithm = PdfEncryptionAlgorithm.AES;

// Option 1: Encrypt all contents (default)
security.EncryptionOptions = PdfEncryptionOptions.EncryptAllContents;

// Option 2: Encrypt all except metadata
security.EncryptionOptions = PdfEncryptionOptions.EncryptAllContentsExceptMetadata;

// Option 3: Encrypt only attachments
security.EncryptionOptions = PdfEncryptionOptions.EncryptOnlyAttachments;
security.UserPassword = "password";  // Mandatory for this option

```

---

## Decrypt an encrypted PDF

Remove encryption and restore default permissions.

```csharp
using Syncfusion.Pdf.Parsing;
using Syncfusion.Pdf.Security;

// Load encrypted PDF with password
PdfLoadedDocument loadedDocument = new PdfLoadedDocument("Input.pdf", "syncfusion");

// Reset permissions to default (removes restrictions)
loadedDocument.Security.Permissions = PdfPermissionsFlags.Default;

// Clear passwords to decrypt
loadedDocument.Security.OwnerPassword = string.Empty;
loadedDocument.Security.UserPassword = string.Empty;

```

---

## Protect existing PDF document

Add encryption to an already created PDF.

```csharp
using Syncfusion.Pdf.Parsing;
using Syncfusion.Pdf.Security;

// Configure security
PdfSecurity security = document.Security;
security.KeySize = PdfEncryptionKeySize.Key256Bit;
security.Algorithm = PdfEncryptionAlgorithm.AES;
security.OwnerPassword = "ownerPassword256";
security.UserPassword = "userPassword256";

```

---

## Change PDF password

Modify the user password of an encrypted PDF.

```csharp
using Syncfusion.Pdf.Parsing;
using Syncfusion.Pdf.Security;

// Load encrypted PDF with current password
PdfLoadedDocument loadedDocument = new PdfLoadedDocument("Input.pdf", "password");

// Change the user password
loadedDocument.Security.UserPassword = "NewPassword";
```

---

## Change document permissions

Modify what users are allowed to do with the PDF.

```csharp
using Syncfusion.Pdf.Parsing;
using Syncfusion.Pdf.Security;

// Change permissions
loadedDocument.Security.Permissions = 
    PdfPermissionsFlags.CopyContent | PdfPermissionsFlags.AssembleDocument;

```

---

## Permission Flags

| Permission | Allowed Action |
| --- | --- |
| `Print` | Print the document |
| `EditContent` | Modify document content |
| `CopyContent` | Copy/extract text and images |
| `EditAnnotations` | Add/modify annotations and comments |
| `FillFields` | Fill interactive form fields |
| `AssembleDocument` | Rotate pages and modify file structure |
| `AccessibilityCopyContent` | Copy content for accessibility tools |
| `FullQualityPrint` | Print at full quality (no degradation) |
| `Default` | No restrictions |

---

## Encryption Algorithms & Key Sizes

| Algorithm | Key Sizes | Notes |
| --- | --- | --- |
| RC4 | 40-bit, 128-bit | Legacy; not recommended for new documents |
| AES | 128-bit, 256-bit | Strong encryption; widely supported |
| AES-GCM | 256-bit | Authenticated encryption; PDF 2.0 only; most secure |

---

## Detect password protection

Check if a PDF requires a password to open.

```csharp
using Syncfusion.Pdf;
using Syncfusion.Pdf.Parsing;

try
{
    PdfLoadedDocument loadedDocument = new PdfLoadedDocument("Input.pdf");
}
catch (PdfDocumentException exception)
{
    if (exception.Message == "Can't open an encrypted document. The password is invalid.")
    {
        Console.WriteLine("Document is password protected");
    }
}
```
