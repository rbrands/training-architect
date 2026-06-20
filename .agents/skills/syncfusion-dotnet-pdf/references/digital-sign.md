# Digital Signatures

Guide and code snippets for adding digital signatures to PDFs using Syncfusion .NET PDF Library. Examples are ordered from basic → advanced.

*Note: For document creation, loading, and save/close patterns, see [document-structure.md](document-structure.md).*

---

**Common namespaces:**

```csharp
using Syncfusion.Drawing;
using Syncfusion.Pdf;
using Syncfusion.Pdf.Graphics;
using Syncfusion.Pdf.Parsing;
using Syncfusion.Pdf.Security;
using System.Security.Cryptography.X509Certificates;
using System.IO;
```

## Add a visible digital signature to an existing PDF

```csharp
PdfLoadedPage page = loadedDocument.Pages[0] as PdfLoadedPage;

// Load certificate from PFX file
using FileStream certStream = new FileStream("Data/PDF.pfx", FileMode.Open, FileAccess.Read);
PdfCertificate pdfCert = new PdfCertificate(certStream, "syncfusion");

// Create a visible signature field on the page
PdfSignature signature = new PdfSignature(loadedDocument, page, pdfCert, "Signature");
signature.Bounds = new RectangleF(10, 10, 160, 80);
signature.ContactInfo = "support@example.com";
signature.LocationInfo = "Office";
signature.Reason = "Document approval";
```

---

## Add a digital signature to a new PDF

```csharp
PdfPage page = document.Pages.Add();

using FileStream certStream = new FileStream("Data/PDF.pfx", FileMode.Open, FileAccess.Read);
PdfCertificate pdfCert = new PdfCertificate(certStream, "syncfusion");

PdfSignature signature = new PdfSignature(document, page, pdfCert, "Signature");
signature.Bounds = new RectangleF(200, 200, 150, 60);
```

---

## Set digest algorithm and cryptographic standard

```csharp
PdfSignature signature = new PdfSignature(loadedDocument, page, pdfCert, "Signature");

PdfSignatureSettings settings = signature.Settings;
settings.DigestAlgorithm = DigestAlgorithm.SHA256;           // or SHA512
settings.CryptographicStandard = CryptographicStandard.CADES; // or CMS
```

---

## Add a timestamp (TSA) to a signature

```csharp
PdfSignature signature = new PdfSignature(loadedDocument, page, pdfCert, "Signature");
signature.TimeStampServer = new TimeStampServer(new Uri("http://time.certum.pl/"));
```

---

## Customize signature appearance (overlay text or image)

```csharp
PdfSignature signature = new PdfSignature(loadedDocument, page, pdfCert, "Signature");
signature.Bounds = new RectangleF(10, 10, 160, 80);

// Draw text overlay
PdfFont font = new PdfStandardFont(PdfFontFamily.Helvetica, 10);
signature.Appearance.Normal.Graphics.DrawString("Signed by: Example", font, PdfBrushes.Black, new PointF(5, 5));

// Or draw an image overlay
using FileStream imageStream = new FileStream("Data/signature.png", FileMode.Open, FileAccess.Read);
PdfBitmap signatureImage = new PdfBitmap(imageStream);
signature.Appearance.Normal.Graphics.DrawImage(signatureImage, new RectangleF(0, 0, signature.Bounds.Width, signature.Bounds.Height));
```

---

## Enable validation appearance

```csharp
signature.EnableValidationAppearance = true;
```

---

## Reserve estimated signature size

Reserve bytes upfront for external or large signatures.

```csharp
signature.EstimatedSignatureSize = 20000; // bytes
```

---

## Sign an existing signature field

```csharp
PdfLoadedPage page = loadedDocument.Pages[0] as PdfLoadedPage;
PdfLoadedSignatureField field = loadedDocument.Form.Fields[0] as PdfLoadedSignatureField;

using FileStream certStream = new FileStream("Data/PDF.pfx", FileMode.Open, FileAccess.Read);
PdfCertificate pdfCert = new PdfCertificate(certStream, "syncfusion");

field.Signature = new PdfSignature(loadedDocument, page, pdfCert, "Signature", field);
```

---

## Add a signature field (unsigned placeholder)

```csharp
PdfLoadedPage page = loadedDocument.Pages[0] as PdfLoadedPage;
PdfSignatureField field = new PdfSignatureField(loadedDocument);
field.Bounds = new RectangleF(10, 10, 160, 80);
page.Form.Fields.Add(field);
```

---

## Remove a signature field

```csharp
PdfLoadedSignatureField signatureField = loadedDocument.Form.Fields[0] as PdfLoadedSignatureField;
loadedDocument.Form.Fields.Remove(signatureField);
```

---

## Use an X509 certificate from the Windows certificate store

```csharp
X509Store store = new X509Store(StoreName.My, StoreLocation.CurrentUser);
store.Open(OpenFlags.ReadOnly | OpenFlags.OpenExistingOnly);
X509Certificate2Collection found = store.Certificates.Find(X509FindType.FindByThumbprint, "THUMBPRINT", true);
X509Certificate2 digitalID = found[0];

PdfCertificate pdfCert = new PdfCertificate(digitalID);
PdfLoadedPage page = loadedDocument.Pages[0] as PdfLoadedPage;
PdfSignature signature = new PdfSignature(loadedDocument, page, pdfCert, "DigitalSignature");
signature.Settings.CryptographicStandard = CryptographicStandard.CADES;
signature.Settings.DigestAlgorithm = DigestAlgorithm.SHA512;

store.Close();
```

---

## Externally sign a PDF (ComputeHash handler)

```csharp
PdfSignature signature = new PdfSignature(loadedDocument, loadedDocument.Pages[0] as PdfLoadedPage, null, "DigitalSignature");
signature.ComputeHash += Signature_ComputeHash;

void Signature_ComputeHash(object sender, PdfSignatureEventArgs ars)
{
    // ars.Data contains the byte range to sign
    SignedCms signedCms = new SignedCms(new ContentInfo(ars.Data), detached: true);
    X509Certificate2 certificate = new X509Certificate2("Data/PDF.pfx", "syncfusion");
    CmsSigner cmsSigner = new CmsSigner(certificate);
    cmsSigner.DigestAlgorithm = new Oid("2.16.840.1.101.3.4.2.1"); // SHA256
    signedCms.ComputeSignature(cmsSigner);
    ars.SignedData = signedCms.Encode();
}
```

---

## Create Long-Term Validation (LTV)

After external signing, reopen the signed PDF and enable LTV.

```csharp
PdfLoadedSignatureField sigField = loadedDocument.Form.Fields[0] as PdfLoadedSignatureField;
X509Certificate2 x509 = new X509Certificate2("Data/PDF.pfx", "syncfusion");
sigField.Signature.CreateLongTermValidity(new List<X509Certificate2> { x509 });
```

---

## Validate a signature and check LTV

```csharp
//Get signature field.
            PdfLoadedSignatureField lSigFld = ldoc.Form.Fields[0] as PdfLoadedSignatureField;

            //X509Certificate2Collection to check the signer's identity using root certificates.
            X509CertificateCollection collection = new X509CertificateCollection();

            //Read the certificate file.
            FileStream pfxFile = new FileStream(dataPath + @"Root.cer", FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            
            byte[] data = new byte[pfxFile.Length];

            pfxFile.Read(data, 0, data.Length);

            X509Certificate2 certificate = new X509Certificate2(data);

            //Add the certificate to the collection.
            collection.Add(certificate);

            pfxFile = new FileStream(dataPath + @"Intermediate0.cer", FileMode.Open, FileAccess.Read, FileShare.ReadWrite);

            data = new byte[pfxFile.Length];

            pfxFile.Read(data, 0, data.Length);

            certificate = new X509Certificate2(data);

            //Add the certificate to the collection.
            collection.Add(certificate);

            pfxFile = new FileStream(dataPath + @"Intermediate1.cer", FileMode.Open, FileAccess.Read, FileShare.ReadWrite);

            data = new byte[pfxFile.Length];

            pfxFile.Read(data, 0, data.Length);

            certificate = new X509Certificate2(data);

            //Add the certificate to the collection.
            collection.Add(certificate);

            //Validate signature and get the validation result
            PdfSignatureValidationResult result = lSigFld.ValidateSignature(collection);

            StringBuilder builder = new StringBuilder();

            builder.AppendLine("Signature is " + result.SignatureStatus);

            builder.AppendLine("----------Validation Summary----------");

            //Checks whether the document is modified or not
            bool isModified = result.IsDocumentModified;
            if (isModified)
                builder.AppendLine("The document has been altered or corrupted since the signature was applied.");
            else
                builder.AppendLine("The document has not been modified since the signature was applied.");

            //Signature details
            builder.AppendLine("Digitally signed by " + lSigFld.Signature.Certificate.IssuerName);
            builder.AppendLine("Valid From : " + lSigFld.Signature.Certificate.ValidFrom);
            builder.AppendLine("Valid To : " + lSigFld.Signature.Certificate.ValidTo);
            builder.AppendLine("Signature Algorithm : " + result.SignatureAlgorithm);
            builder.AppendLine("Hash Algorithm : " + result.DigestAlgorithm);

            //Revocation validation details
            builder.AppendLine("OCSP revocation status : " + result.RevocationResult.OcspRevocationStatus);
            if (result.RevocationResult.OcspRevocationStatus == RevocationStatus.None && result.RevocationResult.IsRevokedCRL)
                builder.AppendLine("CRL is revoked.");

            builder.AppendLine();
            builder.AppendLine("--------Revocation Information---------");
            builder.AppendLine();

            foreach (PdfSignerCertificate signerCertificate in result.SignerCertificates)
            {
                if (signerCertificate.OcspCertificate != null)
                {
                    builder.AppendLine("------------OCSP Certificate-------------");
                    builder.AppendLine();
                    foreach (X509Certificate2 item in signerCertificate.OcspCertificate.Certificates)
                    {
                        builder.AppendLine("The OCSP Response was signed by " + item.SubjectName.Name);
                    }
                    builder.AppendLine("Is Embedded: " + signerCertificate.OcspCertificate.IsEmbedded);
                    builder.AppendLine("ValidFrom: " + signerCertificate.OcspCertificate.ValidFrom);
                    builder.AppendLine("ValidTo: " + signerCertificate.OcspCertificate.ValidTo);
                    builder.AppendLine();
                    continue;
                }
                if (signerCertificate.CrlCertificate != null)
                {
                    builder.AppendLine("------------CRL Certificate--------------");
                    builder.AppendLine();
                    foreach (X509Certificate2 item in signerCertificate.CrlCertificate.Certificates)
                    {
                        builder.AppendLine("The CRL was signed by " + item.SubjectName.Name);
                    }
                    builder.AppendLine("Is Embedded: " + signerCertificate.CrlCertificate.IsEmbedded);
                    builder.AppendLine("ValidFrom: " + signerCertificate.CrlCertificate.ValidFrom);
                    builder.AppendLine("ValidTo: " + signerCertificate.CrlCertificate.ValidTo);
                    break;
                }
            }
			Console.WriteLine("DigitalSignatureValidation" + builder.ToString());

```

---

## Retrieve signature details from a signed PDF

```csharp
PdfLoadedSignatureField sigField = loadedDocument.Form.Fields[0] as PdfLoadedSignatureField;
Console.WriteLine($"Signed by: {sigField.Signature.SignedName} ({sigField.Signature.Certificate.IssuerName}) on {sigField.Signature.SignedDate}");
```

---

## Externally sign the PDF document using IPdfExternalSigner

```csharp
//Create an external signer.
IPdfExternalSigner externalSignature = new ExternalSigner("SHA1");

//Add public certificates.
List<X509Certificate2> certificates = new List<X509Certificate2>();
certificates.Add(new X509Certificate2(Convert.FromBase64String(PublicCert)));
signature.AddExternalSigner(externalSignature, certificates, null);
```
```Helper Class 
		// Create the external signer class and sign the document hash
        class ExternalSigner : IPdfExternalSigner
        {
            private string _hashAlgorithm;

            public string HashAlgorithm
            {
                get { return _hashAlgorithm; }
            }

            public ExternalSigner(string hashAlgorithm)
            {
                _hashAlgorithm = hashAlgorithm;
            }

            public byte[] Sign(byte[] message, out byte[] timeStampResponse)
            {
                timeStampResponse = null;
                X509Certificate2 digitalID = new X509Certificate2(Path.GetFullPath(@"../../../Data/PDF.pfx"), "password123");

                if (digitalID.PrivateKey is RSACryptoServiceProvider rsaProvider)
                {
                    return rsaProvider.SignData(message, HashAlgorithm);
                }
                else if (digitalID.PrivateKey is RSACng rsaCng)
                {
                    return rsaCng.SignData(message, HashAlgorithmName.SHA1, RSASignaturePadding.Pkcs1);
                }
                else if (digitalID.PrivateKey is RSAOpenSsl rsaOpenSsl)
                {
                    return rsaOpenSsl.SignData(message, HashAlgorithmName.SHA1, RSASignaturePadding.Pkcs1);
                }

                return null;
            }
        }
```
---

## Deferred signing in PDF document
````csharp
		//Creates a digital signature with empty sign.
        PdfSignature signature = new PdfSignature(loadedDocument, loadedDocument.Pages[0], null, "Signature");
        //Sets the signature information.
        signature.Bounds = new RectangleF(new PointF(0, 0), new SizeF(100, 30));
        signature.Settings.CryptographicStandard = CryptographicStandard.CADES;
        signature.Settings.DigestAlgorithm = DigestAlgorithm.SHA1;
        //Create an external signer.
        IPdfExternalSigner externalSignature = new SignEmpty("SHA1");
        //Add public certificates.
        System.Collections.Generic.List<X509Certificate2> certificates = new System.Collections.Generic.List<X509Certificate2>();
        certificates.Add(new X509Certificate2(Convert.FromBase64String(PublicCert)));
        signature.AddExternalSigner(externalSignature, certificates, null);
```
```csharp
 //Create an external signer with a signed hash message.
    IPdfExternalSigner externalSigner = new ExternalSigner("SHA1", signedHash);
    //Add public certificates.
    System.Collections.Generic.List<X509Certificate2> publicCertificates = new System.Collections.Generic.List<X509Certificate2>();
    publicCertificates.Add(new X509Certificate2(Convert.FromBase64String(PublicCert)));

    //Create an output file stream.
    MemoryStream outputFileStream = new MemoryStream();
    //Get the stream from the document.
    FileStream inputFileStream = new FileStream("EmptySignature.pdf", FileMode.Open, FileAccess.Read);
    string pdfPassword = string.Empty;
    //Replace an empty signature.
    PdfSignature.ReplaceEmptySignature(inputFileStream, pdfPassword, outputFileStream, signatureName, externalSigner, publicCertificates);
```

```Helper Class 
/// <summary>
    /// Represents to sign an empty signature from the external signer.
    /// </summary>
    class SignEmpty : IPdfExternalSigner
    {
        private string _hashAlgorithm;

        public string HashAlgorithm
        {
            get { return _hashAlgorithm; }
        }

        public SignEmpty(string hashAlgorithm)
        {
            _hashAlgorithm = hashAlgorithm;
        }

        public byte[] Sign(byte[] message, out byte[] timeStampResponse)
        {
            //Send document hash for signing using the external services.
            SignDocumentHash(message);
            //Set a null value to create an empty signed document.
            byte[] signedBytes = null;
            timeStampResponse = null;
            return signedBytes;
        }
        //Example for signed docuement hash 
        private void SignDocumentHash(byte[] documentHash)
        {
            X509Certificate2 digitalID = new X509Certificate2(new X509Certificate2(Path.GetFullPath(@"Data/PDF.pfx"), "password123"));
            if (digitalID.PrivateKey is System.Security.Cryptography.RSACryptoServiceProvider)
            {
                System.Security.Cryptography.RSACryptoServiceProvider rsa = (System.Security.Cryptography.RSACryptoServiceProvider)digitalID.PrivateKey;
                Program.SignedHash = rsa.SignData(documentHash, HashAlgorithm);
            }
            else if (digitalID.PrivateKey is RSACng)
            {
                RSACng rsa = (RSACng)digitalID.PrivateKey;
                Program.SignedHash = rsa.SignData(documentHash, System.Security.Cryptography.HashAlgorithmName.SHA1, RSASignaturePadding.Pkcs1);
            }
            else if (digitalID.PrivateKey is System.Security.Cryptography.RSAOpenSsl)
            {
                System.Security.Cryptography.RSAOpenSsl rsa = (System.Security.Cryptography.RSAOpenSsl)digitalID.PrivateKey;
                Program.SignedHash = rsa.SignData(documentHash, System.Security.Cryptography.HashAlgorithmName.SHA1, RSASignaturePadding.Pkcs1);
            }
        }
    }
	/// <summary>
    /// Represents to replace an empty signature from an external signer.
    /// </summary>
    class ExternalSigner : IPdfExternalSigner
    {
        private string _hashAlgorithm;
        private byte[] _signedHash;
        public string HashAlgorithm
        {
            get { return _hashAlgorithm; }
        }
        public ExternalSigner(string hashAlgorithm, byte[] hash)
        {
            _hashAlgorithm = hashAlgorithm;
            _signedHash = hash;
        }
        public byte[] Sign(byte[] message, out byte[] timeStampResponse)
        {
            //Set the signed hash message to replace an empty signature.
            byte[] signedBytes = _signedHash;
            timeStampResponse = null;
            return signedBytes;
        }
    }
```

---


## Key APIs

| Member | Description |
| --- | --- |
| `PdfSignature(PdfDocumentBase, PdfPageBase, PdfCertificate, string)` | Creates a named digital signature field with the given certificate |
| `PdfSignature.Bounds` | Gets or sets the visible bounds of the signature field |
| `PdfSignature.Certificate` | Certificate used for signing (`PdfCertificate` or `X509Certificate2`) |
| `PdfSignature.ContactInfo` | Signer contact information embedded in the signature |
| `PdfSignature.LocationInfo` | Geographic location embedded in the signature |
| `PdfSignature.Reason` | Reason for signing embedded in the signature |
| `PdfSignature.Settings.DigestAlgorithm` | Hash algorithm — `SHA256`, `SHA512`, etc. |
| `PdfSignature.Settings.CryptographicStandard` | Standard — `CryptographicStandard.CADES` or `Pkcs7` |
| `PdfSignature.EnableValidationAppearance` | Shows validity state in the signature appearance |
| `PdfSignature.TimeStampServer` | Attaches a TSA server for trusted timestamping |
| `PdfSignature.EstimatedSignatureSize` | Reserves bytes for external/large signature values |
| `PdfSignature.Appearance.Normal.Graphics` | `PdfGraphics` surface to draw text or image overlays |
| `PdfSignature.ComputeHash` | Event for external/detached signing flows |
| `PdfSignature.CreateLongTermValidity(List<X509Certificate2>)` | Embeds LTV data into a previously signed document |
| `PdfLoadedSignatureField.Signature` | Gets or sets the `PdfSignature` on an existing field |
| `PdfLoadedSignatureField.ValidateSignature(X509Certificate2Collection)` | Validates signature and returns `PdfSignatureValidationResult` |
| `PdfSignatureValidationResult.SignatureStatus` | `SignatureStatus.Valid` if signature is intact and trusted |
| `PdfSignatureValidationResult.LtvVerificationInfo` | LTV details including `IsLtvEnabled` |
| `PdfCertificate(Stream, string)` | Loads a certificate from a PFX/PKCS#12 stream with password |
| `PdfCertificate(X509Certificate2)` | Wraps a Windows `X509Certificate2` for use with Syncfusion |

---

## Notes

- Always call `loadedDocument.Save()` **after** configuring the signature (see [document-structure.md](document-structure.md)).
- Replace `cert.pfx` and passwords with secure key storage in production.
- For CAdES/TSA/PAdES advanced scenarios, use `ComputeHash` event with an external CMS signer.

---
