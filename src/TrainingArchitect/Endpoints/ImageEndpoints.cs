using Azure.Identity;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Sas;

namespace TrainingArchitect.Endpoints;

/// <summary>
/// Minimal API endpoint for uploading editor images to Azure Blob Storage.
/// Images selected through the editor's file dialog are resized in the browser
/// before upload. Each blob is stored under a sanitized GUID-based name.
/// </summary>
public static class ImageEndpoints
{
    private const int MaxFileSizeBytes = 10 * 1024 * 1024;
    private const int WarningFileSizeBytes = 2 * 1024 * 1024;

    private static readonly HashSet<string> AllowedContentTypes = new(StringComparer.OrdinalIgnoreCase)
    {
        "image/jpeg", "image/jpg", "image/png", "image/webp", "image/gif"
    };

    public static IEndpointRouteBuilder MapImageEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapPost("/api/images/upload",
            async (HttpContext context, IConfiguration config, ILoggerFactory loggerFactory) =>
            {
                var logger = loggerFactory.CreateLogger("ImageEndpoints");
                if (context.Request.ContentLength > MaxFileSizeBytes + 64 * 1024)
                    return Results.StatusCode(StatusCodes.Status413PayloadTooLarge);

                var file = context.Request.Form.Files.FirstOrDefault();

                if (file is null || file.Length == 0)
                    return Results.BadRequest("No file received.");

                if (file.Length > MaxFileSizeBytes)
                    return Results.StatusCode(StatusCodes.Status413PayloadTooLarge);

                var endpoint = config["Storage:BlobEndpoint"];
                if (string.IsNullOrEmpty(endpoint))
                    return Results.Problem("Storage endpoint is not configured.");

                try
                {
                    if (!AllowedContentTypes.Contains(file.ContentType))
                        return Results.BadRequest("File type not allowed.");

                    var contentType = await GetValidatedContentTypeAsync(file);
                    if (contentType is null)
                        return Results.BadRequest("The uploaded file is not a supported image.");

                    var extension = contentType switch
                    {
                        "image/png" => ".png",
                        "image/webp" => ".webp",
                        "image/gif" => ".gif",
                        _ => ".jpg"
                    };
                    var blobName = $"{Guid.NewGuid():N}{extension}";

                    var credential = new DefaultAzureCredential();
                    var blobServiceClient = new BlobServiceClient(new Uri(endpoint), credential);
                    var containerClient = blobServiceClient.GetBlobContainerClient("images");
                    var blobClient = containerClient.GetBlobClient(blobName);

                    await using var uploadStream = file.OpenReadStream();
                    await blobClient.UploadAsync(uploadStream, new BlobUploadOptions
                    {
                        HttpHeaders = new BlobHttpHeaders { ContentType = contentType }
                    });

                    if (file.Length > WarningFileSizeBytes)
                    {
                        logger.LogWarning(
                            "Uploaded image exceeds {LimitMB} MB: {ActualKB} KB.",
                            WarningFileSizeBytes / 1024 / 1024,
                            file.Length / 1024);
                    }

                    var blobUrl = blobClient.Uri.ToString();

                    // X-Blob-Url is used by the Syncfusion upload success handler.
                    context.Response.Headers["X-Blob-Url"] = blobUrl;

                    return Results.Ok(new ImageUploadResponse(
                        Success: true,
                        Name: blobName,
                        Url: blobUrl,
                        File: new ImageFileInfo(
                            Name: blobName,
                            Size: file.Length,
                            Type: contentType)));
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Image upload to Azure Blob Storage failed.");
                    return Results.Problem("Image upload failed. Check server logs for details.");
                }
            })
            .RequireAuthorization(p => p.RequireRole("SiteAdmin"))
            .DisableAntiforgery();

        // Returns a short-lived write SAS URI for direct browser-to-blob uploads.
        // Requires the Web App Managed Identity to have the 'Storage Blob Delegator' role
        // on the storage account (in addition to the existing 'Storage Blob Data Contributor').
        // Assign with: az role assignment create --assignee <principalId> \
        //   --role "Storage Blob Delegator" --scope /subscriptions/.../storageAccounts/<name>
        app.MapGet("/api/images/sas", async (IConfiguration config) =>
        {
            var endpoint = config["Storage:BlobEndpoint"];
            if (string.IsNullOrEmpty(endpoint))
                return Results.Problem("Storage endpoint is not configured.");

            var credential = new DefaultAzureCredential();
            var blobServiceClient = new BlobServiceClient(new Uri(endpoint), credential);
            var containerClient = blobServiceClient.GetBlobContainerClient("images");

            var blobName = $"{Guid.NewGuid():N}.png";
            var blobClient = containerClient.GetBlobClient(blobName);

            // User Delegation Key: required when using Managed Identity (no account key available).
            // Slightly back-dated start time to tolerate minor clock skew between servers.
            var delegationKey = await blobServiceClient.GetUserDelegationKeyAsync(
                new BlobGetUserDelegationKeyOptions(DateTimeOffset.UtcNow.AddMinutes(5))
                {
                    StartsOn = DateTimeOffset.UtcNow.AddMinutes(-5)
                });

            var sasBuilder = new BlobSasBuilder
            {
                BlobContainerName = "images",
                BlobName = blobName,
                Resource = "b",
                StartsOn = DateTimeOffset.UtcNow.AddMinutes(-5),
                ExpiresOn = DateTimeOffset.UtcNow.AddMinutes(5)
            };
            sasBuilder.SetPermissions(BlobSasPermissions.Write | BlobSasPermissions.Create);

            var sasParams = sasBuilder.ToSasQueryParameters(delegationKey, blobServiceClient.AccountName);
            var uriBuilder = new Azure.Storage.Blobs.BlobUriBuilder(blobClient.Uri) { Sas = sasParams };

            return Results.Ok(new
            {
                Sas = uriBuilder.ToUri().ToString(),
                PublicLink = blobClient.Uri.ToString(),
                BlobName = blobName
            });
        })
        .RequireAuthorization(p => p.RequireRole("SiteAdmin"));

        return app;
    }

    private static async Task<string?> GetValidatedContentTypeAsync(IFormFile file)
    {
        if (file.Length < 12)
            return null;

        var header = new byte[12];
        await using var stream = file.OpenReadStream();
        await stream.ReadExactlyAsync(header);

        if (file.ContentType.Equals("image/png", StringComparison.OrdinalIgnoreCase)
            && header[0] == 137
            && header[1] == 80
            && header[2] == 78
            && header[3] == 71
            && header[4] == 13
            && header[5] == 10
            && header[6] == 26
            && header[7] == 10)
            return "image/png";

        if ((file.ContentType.Equals("image/jpeg", StringComparison.OrdinalIgnoreCase)
             || file.ContentType.Equals("image/jpg", StringComparison.OrdinalIgnoreCase))
            && header[0] == 0xFF
            && header[1] == 0xD8
            && header[2] == 0xFF)
            return "image/jpeg";

        if (file.ContentType.Equals("image/gif", StringComparison.OrdinalIgnoreCase)
            && (System.Text.Encoding.ASCII.GetString(header, 0, 6) == "GIF87a"
                || System.Text.Encoding.ASCII.GetString(header, 0, 6) == "GIF89a"))
            return "image/gif";

        if (file.ContentType.Equals("image/webp", StringComparison.OrdinalIgnoreCase)
            && System.Text.Encoding.ASCII.GetString(header, 0, 4) == "RIFF"
            && System.Text.Encoding.ASCII.GetString(header, 8, 4) == "WEBP")
            return "image/webp";

        return null;
    }
}

/// <summary>Response returned by the image upload endpoint.</summary>
public record ImageUploadResponse(bool Success, string Name, string Url, ImageFileInfo File);

/// <summary>File metadata included in <see cref="ImageUploadResponse"/>.</summary>
public record ImageFileInfo(string Name, long Size, string Type);
