# Save PDF

Save new and existing PDF documents to file paths, streams, byte arrays, and cloud storage providers using Syncfusion .NET PDF Library.

*Note: For document creation and loading patterns, see [document-structure.md](document-structure.md). For opening existing PDFs, see [open-pdf.md](open-pdf.md).*

---

**Common namespaces:**

```csharp
using Syncfusion.Pdf;
using Syncfusion.Pdf.Parsing;
```

---

## Save a new PDF to a file path

```csharp
document.Save("Output.pdf");
document.Close(true);
```

---

## Save a new PDF to a MemoryStream

```csharp
MemoryStream stream = new MemoryStream();
document.Save(stream);
stream.Position = 0;
document.Close(true);
```

---

## Save an existing (loaded) PDF to a MemoryStream

```csharp
MemoryStream stream = new MemoryStream();
loadedDocument.Save(stream);
stream.Position = 0;
loadedDocument.Close(true);
```

---

## Get PDF as a byte array

```csharp
MemoryStream stream = new MemoryStream();
document.Save(stream);
document.Close(true);
byte[] pdfBytes = stream.ToArray();
```

---

## Close a document

`Close(true)` releases all PDF DOM memory and also disposes the source stream. Use `Close(false)` to keep the source stream open.

```csharp
document.Close(true);       // disposes document + source stream
loadedDocument.Close(true); // disposes loaded document + source stream
```

> **Note:** Always call `Close` after saving to free resources. Skipping it causes memory leaks in long-running processes.

---

## Save to Azure Blob Storage

**NuGet:** `Microsoft.Azure.Storage.Blob`

```csharp
using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Blob;

//Save the PDF to a MemoryStream first.
MemoryStream stream = new MemoryStream();
document.Save(stream);
document.Close(true);

//Upload the stream to Azure Blob Storage.
CloudStorageAccount storageAccount = CloudStorageAccount.Parse(connectionString);
CloudBlobClient     blobClient     = storageAccount.CreateCloudBlobClient();
CloudBlobContainer  container      = blobClient.GetContainerReference(containerName);
container.CreateIfNotExists();
CloudBlockBlob blockBlob = container.GetBlockBlobReference(blobName);

stream.Position = 0;
blockBlob.UploadFromStream(stream);
```

---

## Save to AWS S3

**NuGet:** `AWSSDK.S3`

```csharp
using Amazon;
using Amazon.S3;
using Amazon.S3.Transfer;

//Save the PDF to a MemoryStream first.
MemoryStream stream = new MemoryStream();
document.Save(stream);
document.Close(true);

string accessKey  = "YOUR_ACCESS_KEY";
string secretKey  = "YOUR_SECRET_KEY";
RegionEndpoint region     = RegionEndpoint.USEast1; // Change to your region
string bucketName = "YOUR_BUCKET_NAME";
string objectKey  = "Output.pdf";

stream.Position = 0;
using (var s3Client = new AmazonS3Client(accessKey, secretKey, region))
using (var transferUtility = new TransferUtility(s3Client))
{
    transferUtility.Upload(stream, bucketName, objectKey);
}
```

---

## Save to Google Drive

**NuGet:** `Google.Apis.Drive.v3`

```csharp
using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using File = Google.Apis.Drive.v3.Data.File;

//Save the PDF to a MemoryStream first.
MemoryStream stream = new MemoryStream();
document.Save(stream);
document.Close(true);

//Authenticate with Google Drive.
UserCredential credential;
string[] scopes = { DriveService.Scope.Drive };

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

//Upload the PDF to Google Drive.
var fileMetadata = new File { Name = "Output.pdf", MimeType = "application/pdf" };
stream.Position = 0;
var request = service.Files.Create(fileMetadata, stream, "application/pdf");
request.Upload();
```

---

## Save to Google Cloud Storage

**NuGet:** `Google.Cloud.Storage.V1`

```csharp
using Google.Apis.Auth.OAuth2;
using Google.Cloud.Storage.V1;

//Save the PDF to a MemoryStream first.
MemoryStream stream = new MemoryStream();
document.Save(stream);
document.Close(true);

GoogleCredential credential = GoogleCredential.FromFile("credentials.json");
StorageClient    storage    = StorageClient.Create(credential);

stream.Position = 0;
storage.UploadObject("your-bucket-name", "Output.pdf", "application/pdf", stream);
```

---

## Save to Dropbox

**NuGet:** `Dropbox.Api`

```csharp
using Dropbox.Api;
using Dropbox.Api.Files;

//Save the PDF to a MemoryStream first.
MemoryStream stream = new MemoryStream();
document.Save(stream);
document.Close(true);

string accessToken       = "YOUR_ACCESS_TOKEN";
string filePathInDropbox = "/path/to/save/Output.pdf";

stream.Position = 0;
using (var dbx = new DropboxClient(accessToken))
{
    await dbx.Files.UploadAsync(
        filePathInDropbox,
        WriteMode.Overwrite.Instance,
        body: new MemoryStream(stream.ToArray()));
}
```

---

## Key APIs

| Member | Description |
| --- | --- |
| `PdfDocument.Save(string)` | Saves a new document to the specified file path |
| `PdfDocument.Save(Stream)` | Saves a new document to a stream (e.g., `MemoryStream`) |
| `PdfLoadedDocument.Save(Stream)` | Saves a loaded (and modified) document to a stream |
| `PdfDocument.Close(bool)` | Releases all resources; `true` also disposes the source stream |
| `PdfLoadedDocument.Close(bool)` | Same as above for loaded documents |
| `MemoryStream.ToArray()` | Returns the saved PDF as a `byte[]` for further use or upload |
| `MemoryStream.Position` | Must be reset to `0` before reading or uploading the stream |

---

## Notes

- Always reset `stream.Position = 0` before reading or uploading a `MemoryStream` — skipping this results in an empty or truncated file.
- For cloud targets, save to a `MemoryStream` first, then upload the stream directly rather than writing a temporary file to disk.
- `Close(true)` is the recommended call after saving — it disposes both the PDF DOM and the source input stream. Use `Close(false)` only when you need the source stream to remain open after the operation.
- Saving to the same source file (`loadedDocument.Save("Input.pdf")`) is **not supported** on .NET cross-platform; always save to a new path or stream.

---

## Related

- [document-structure.md](document-structure.md)
- [open-pdf.md](open-pdf.md)
- [security.md](security.md)
- [compress-pdf.md](compress-pdf.md)
- ../SKILL.md

## Official documentation

- <https://help.syncfusion.com/document-processing/pdf/pdf-library/net/open-and-save-pdf-file-in-c-sharp-vb-net>
- <https://help.syncfusion.com/document-processing/pdf/pdf-library/net/save-pdf-files/to-azure-blob-storage>
- <https://help.syncfusion.com/document-processing/pdf/pdf-library/net/save-pdf-files/to-aws-s3>
- <https://help.syncfusion.com/document-processing/pdf/pdf-library/net/save-pdf-files/to-google-drive>
- <https://help.syncfusion.com/document-processing/pdf/pdf-library/net/save-pdf-files/to-google-cloud-storage>
- <https://help.syncfusion.com/document-processing/pdf/pdf-library/net/save-pdf-files/to-dropbox-cloud-file-storage>
