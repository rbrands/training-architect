# Open PDF

Open existing PDF documents from file paths, streams, byte arrays, encrypted files, and cloud storage providers using Syncfusion .NET PDF Library.

*Note: For save/close patterns, see [document-structure.md](document-structure.md). For PDF security and passwords, see [security.md](security.md).*

---

**Common namespaces:**

```csharp
using Syncfusion.Pdf.Parsing;
```

---

## Open from a file path (stream)

```csharp
FileStream inputStream = new FileStream("Input.pdf", FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
PdfLoadedDocument loadedDocument = new PdfLoadedDocument(inputStream);
```

---

## Open from a byte array

```csharp
byte[] inputBytes = File.ReadAllBytes("Input.pdf");
PdfLoadedDocument loadedDocument = new PdfLoadedDocument(inputBytes);
```

---

## Open a password-protected PDF (stream)

```csharp
FileStream inputStream = new FileStream("Input.pdf", FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
PdfLoadedDocument loadedDocument = new PdfLoadedDocument(inputStream, "password");
```

---

## Open a password-protected PDF (byte array)

```csharp
PdfLoadedDocument loadedDocument = new PdfLoadedDocument(inputBytes, "password");
```

---

## Handle an invalid password exception

```csharp
FileStream inputStream = new FileStream("Input.pdf", FileMode.Open, FileAccess.Read);
PdfLoadedDocument loadedDocument = null;
try
{
    loadedDocument = new PdfLoadedDocument(inputStream, "password");
}
catch (PdfInvalidPasswordException)
{
    //Password is incorrect or document was opened without a password.
}
```

---

## Open a corrupted PDF (attempt repair — stream)

Pass `true` to enable the open-and-repair mode, which resolves basic cross-reference offset issues.

```csharp
FileStream inputStream = new FileStream("Input.pdf", FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
PdfLoadedDocument loadedDocument = new PdfLoadedDocument(inputStream, true);
```

> **Note:** The open-and-repair overload only resolves basic cross-reference offset issues. It cannot recover from complex corruption and may be slower than the standard overloads.

---

## Open a corrupted PDF (attempt repair — byte array)

```csharp
PdfLoadedDocument loadedDocument = new PdfLoadedDocument(inputBytes, true);
```

---

## Handle general PDF loading exceptions

```csharp
FileStream inputStream = new FileStream("Input.pdf", FileMode.Open, FileAccess.Read);
PdfLoadedDocument loadedDocument = null;
try
{
    loadedDocument = new PdfLoadedDocument(inputStream, true);
}
catch (PdfException ex)
{
    //Handles: invalid signature, bad format, corrupted cross-reference,
    //missing EOF, bad input stream, or fatal parse errors.
    Console.WriteLine(ex.Message);
}
```

---

## Save to a MemoryStream

```csharp
MemoryStream outputStream = new MemoryStream();
loadedDocument.Save(outputStream);
outputStream.Position = 0;
loadedDocument.Close(true);
```

---

## Open from Azure Blob Storage

**NuGet:** `Microsoft.Azure.Storage.Blob`

```csharp
using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Blob;

CloudStorageAccount storageAccount = CloudStorageAccount.Parse(connectionString);
CloudBlobClient    blobClient      = storageAccount.CreateCloudBlobClient();
CloudBlobContainer container       = blobClient.GetContainerReference(containerName);
CloudBlockBlob     blockBlob       = container.GetBlockBlobReference(blobName);

using (var fileStream = File.OpenWrite("sample.pdf"))
{
    blockBlob.DownloadToStream(fileStream);
}
//Load the downloaded file with Syncfusion.
PdfLoadedDocument loadedDocument = new PdfLoadedDocument("sample.pdf");
```

---

## Open from AWS S3

**NuGet:** `AWSSDK.S3`

```csharp
using Amazon;
using Amazon.S3;
using Amazon.S3.Transfer;

string accessKey  = "YOUR_ACCESS_KEY";
string secretKey  = "YOUR_SECRET_KEY";
RegionEndpoint region     = RegionEndpoint.USEast1; // Change to your region
string bucketName = "YOUR_BUCKET_NAME";
string objectKey  = "YOUR_OBJECT_KEY";
string localPath  = "Output.pdf";

using (var s3Client = new AmazonS3Client(accessKey, secretKey, region))
using (var transferUtility = new TransferUtility(s3Client))
{
    transferUtility.Download(localPath, bucketName, objectKey);
}
//Load the downloaded file with Syncfusion.
PdfLoadedDocument loadedDocument = new PdfLoadedDocument(localPath);
```

---

## Open from Google Drive

**NuGet:** `Google.Apis.Drive.v3`

```csharp
using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using Google.Apis.Util.Store;

UserCredential credential;
string[] scopes = { DriveService.Scope.DriveReadonly };

using (var credStream = new FileStream("credentials.json", FileMode.Open, FileAccess.Read))
{
    credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
        GoogleClientSecrets.Load(credStream).Secrets,
        scopes, "user", CancellationToken.None,
        new FileDataStore("token.json", true)).Result;
}

var service = new DriveService(new BaseClientService.Initializer
{
    HttpClientInitializer = credential,
    ApplicationName       = "YourAppName"
});

string fileId = "YOUR_FILE_ID"; // Replace with actual Google Drive file ID
var downloadStream = new MemoryStream();
service.Files.Get(fileId).Download(downloadStream);
downloadStream.Position = 0;

//Load the stream directly with Syncfusion.
PdfLoadedDocument loadedDocument = new PdfLoadedDocument(downloadStream);
```

---

## Open from Google Cloud Storage

**NuGet:** `Google.Cloud.Storage.V1`

```csharp
using Google.Apis.Auth.OAuth2;
using Google.Cloud.Storage.V1;

GoogleCredential credential = GoogleCredential.FromFile("credentials.json");
StorageClient    storage    = StorageClient.Create(credential);

using (var memStream = new MemoryStream())
{
    storage.DownloadObject("your-bucket-name", "Sample.pdf", memStream);
    memStream.Position = 0;
    //Load the stream directly with Syncfusion.
    PdfLoadedDocument loadedDocument = new PdfLoadedDocument(memStream);
}
```

---

## Open from Dropbox

**NuGet:** `Dropbox.Api`

```csharp
using Dropbox.Api;

string accessToken      = "YOUR_ACCESS_TOKEN";
string filePathInDropbox = "/path/to/Sample.pdf";

using (var dbx = new DropboxClient(accessToken))
using (var response = await dbx.Files.DownloadAsync(filePathInDropbox))
{
    var contentStream = await response.GetContentAsStreamAsync();
    //Load the stream directly with Syncfusion.
    PdfLoadedDocument loadedDocument = new PdfLoadedDocument(contentStream);
}
```

---

## Key APIs

| Member | Description |
| --- | --- |
| `PdfLoadedDocument(Stream)` | Opens an existing PDF from a file stream or memory stream |
| `PdfLoadedDocument(byte[])` | Opens an existing PDF from a byte array |
| `PdfLoadedDocument(Stream, string)` | Opens a password-protected PDF from a stream |
| `PdfLoadedDocument(byte[], string)` | Opens a password-protected PDF from a byte array |
| `PdfLoadedDocument(Stream, bool)` | Opens and attempts to repair a corrupted PDF from a stream (`true` = repair mode) |
| `PdfLoadedDocument(byte[], bool)` | Opens and attempts to repair a corrupted PDF from a byte array |
| `PdfLoadedDocument.Save(Stream)` | Saves the loaded (and modified) document to a stream |
| `PdfLoadedDocument.Close(bool)` | Releases all resources; `true` also disposes the source stream |
| `PdfInvalidPasswordException` | Thrown when a password is missing or incorrect |
| `PdfException` | Base exception for all PDF parsing and load errors |

---

## Notes

- Always pass `FileShare.ReadWrite` when opening a `FileStream` to avoid locking the source file during processing.
- Set `stream.Position = 0` before passing a `MemoryStream` to `PdfLoadedDocument` to avoid empty-document errors.
- `Close(true)` disposes the underlying source stream. If you need to keep the source stream open after processing, call `Close(false)` and manage the stream lifetime yourself.
- The open-and-repair overload (`bool = true`) cannot fix complex corruption — see the error message list in the official docs for repairable vs. non-repairable cases.
- For cloud sources, prefer downloading to a `MemoryStream` and passing that directly to `PdfLoadedDocument` rather than writing to disk first.

---

## Related

- [document-structure.md](document-structure.md)
- [security.md](security.md)
- [merge-pdf.md](merge-pdf.md)
- [split-pdf.md](split-pdf.md)
- ../SKILL.md

## Official documentation

- <https://help.syncfusion.com/document-processing/pdf/pdf-library/net/open-and-save-pdf-file-in-c-sharp-vb-net>
- <https://help.syncfusion.com/document-processing/pdf/pdf-library/net/open-pdf-files/from-azure-blob-storage>
- <https://help.syncfusion.com/document-processing/pdf/pdf-library/net/open-pdf-files/from-aws-s3>
- <https://help.syncfusion.com/document-processing/pdf/pdf-library/net/open-pdf-files/from-google-drive>
- <https://help.syncfusion.com/document-processing/pdf/pdf-library/net/open-pdf-files/from-google-cloud-storage>
- <https://help.syncfusion.com/document-processing/pdf/pdf-library/net/open-pdf-files/from-dropbox-cloud-file-storage>
