## Open and save

> Open and save the workbook in the spreadsheet editor. Supports loading Excel files from local paths, Base64 strings, JSON data (local or remote), and Google Drive.

### EVENTS
```csharp
BeforeSave="OnBeforeSave"
```

```csharp
private void OnBeforeSave(BeforeSaveEventArgs args)
{
    // customize your code in based on the requirements and check below table for event argument details
}
```
**BeforeSave Event Arguments**
| Event Arguments | Description |
|---|---|
| `FileName` | Gets or sets the file name to be used when saving the workbook (e.g., "Report.xlsx"). You can modify this value to change the output file name. |
| `Cancel` | Set to `true` to cancel the save operation. |
| `SaveType (read-only)` | The file format type for saving (e.g., "Xlsx"). |

### BUTTON
<!-- Refer the below button for creating button and update the API public method calling. -->
<button @onclick="#MethodName">#Button Name</button>

### API METHODS

```csharp

// Save: Exports the workbook as "MonthlyReport.xlsx"
await SpreadsheetInstance.SaveAsync(new SaveOptions
{
    SaveType = SaveType.Xlsx,
    FileName = "MonthlyReport"
});

// SaveAsStreamAsync: Returns the spreadsheet content as a MemoryStream
var stream = await SpreadsheetInstance.SaveAsStreamAsync();

```

## Placeholders

| Placeholder | Description | Example |
|---|---|---|
| `#MethodName` | Name of the method calling when clicking the button | SaveWorkbookHandler |
| `#Button Name` | Provide a meaningful name to button which binds to API method | Save as Excel |

### OPEN FROM LOCAL JSON FILE

Loads JSON data from a local file, converts it to Excel format using XlsIO, and binds it to the Spreadsheet as a byte array.

```csharp
@using System.Text.Json
@using Syncfusion.XlsIO
@using Syncfusion.Blazor.Spreadsheet

<SfSpreadsheet DataSource="DataSourceBytes">
    <SpreadsheetRibbon></SpreadsheetRibbon>
</SfSpreadsheet>

@code {
    public byte[] DataSourceBytes { get; set; }

    protected override void OnInitialized()
    {
        // Build the file path to the JSON data source
        // Note: Replace "wwwroot" and "sample.json" with the actual folder and file name where your JSON is stored.
        string jsonFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "sample.json");

        // Read the entire JSON file content as a string
        string jsonData = File.ReadAllText(jsonFilePath);

        // Convert the JSON content to an Excel byte array for Spreadsheet binding
        DataSourceBytes = ConvertJsonToExcel(jsonData);
    }

    // Converts a JSON string into an Excel workbook byte array using Syncfusion XlsIO
    private byte[] ConvertJsonToExcel(string jsonData)
    {
        using JsonDocument jsonDocument = JsonDocument.Parse(jsonData);
        JsonElement rootJsonElement = jsonDocument.RootElement;
        List<Dictionary<string, JsonElement>> dataRows = NormalizeJsonToRows(rootJsonElement);
        List<string> columnHeaders = dataRows.SelectMany(row => row.Keys).Distinct().ToList();

        using ExcelEngine excelEngine = new ExcelEngine();
        IApplication excelApplication = excelEngine.Excel;
        IWorkbook workbook = excelApplication.Workbooks.Create(1);
        IWorksheet worksheet = workbook.Worksheets[0];

        // Write header row with column names
        int columnCount = columnHeaders.Count;
        for (int columnIndex = 0; columnIndex < columnCount; columnIndex++)
        {
            IRange headerCell = worksheet.Range[1, columnIndex + 1];
            headerCell.Text = columnHeaders[columnIndex];
            headerCell.CellStyle.Font.Bold = true;
        }

        // Write data rows starting from the second row
        int currentRowIndex = 2;
        foreach (var dataRow in dataRows)
        {
            for (int columnIndex = 0; columnIndex < columnCount; columnIndex++)
            {
                string columnKey = columnHeaders[columnIndex];
                if (dataRow.TryGetValue(columnKey, out var cellValue))
                {
                    worksheet.Range[currentRowIndex, columnIndex + 1].Value2 = cellValue;
                }
            }
            currentRowIndex++;
        }

        using MemoryStream memoryStream = new MemoryStream();
        workbook.SaveAs(memoryStream);
        return memoryStream.ToArray();
    }

    // Normalizes various JSON structures into a uniform list of row dictionaries
    private List<Dictionary<string, JsonElement>> NormalizeJsonToRows(JsonElement rootJsonElement)
    {
        if (rootJsonElement.ValueKind == JsonValueKind.Array)
        {
            return rootJsonElement.EnumerateArray().Select(JsonToDictionaryList).ToList();
        }

        if (rootJsonElement.ValueKind == JsonValueKind.Object)
        {
            foreach (var property in rootJsonElement.EnumerateObject())
            {
                if (property.Value.ValueKind == JsonValueKind.Array)
                {
                    return property.Value.EnumerateArray().Select(JsonToDictionaryList).ToList();
                }
            }
            return new List<Dictionary<string, JsonElement>> { JsonToDictionaryList(rootJsonElement) };
        }

        return new List<Dictionary<string, JsonElement>>
        {
            new Dictionary<string, JsonElement> { ["value"] = rootJsonElement }
        };
    }

    // Converts a JsonElement to a dictionary of property names and values
    private Dictionary<string, JsonElement> JsonToDictionaryList(JsonElement jsonElement)
    {
        if (jsonElement.ValueKind != JsonValueKind.Object)
        {
            return new Dictionary<string, JsonElement> { ["value"] = jsonElement };
        }

        return jsonElement.EnumerateObject()
        .ToDictionary(
            property => property.Name,
            property => property.Value,
            StringComparer.OrdinalIgnoreCase
        );
    }
}
```

### OPEN FROM REMOTE JSON URL

Retrieves JSON asynchronously from a remote endpoint using HttpClient, converts it to an Excel workbook through XlsIO, and binds the resulting byte array to the Spreadsheet.

> **⚠️ SECURITY**: Only fetch data from **trusted sources**. Use an allowlist of approved URLs and validate all external data to prevent injection attacks. Never use arbitrary user-provided URLs.

```csharp
@using System.Text.Json
@using Syncfusion.XlsIO
@using Syncfusion.Blazor.Spreadsheet
@inject HttpClient HttpClient

@if (IsDataLoaded)
{
    <SfSpreadsheet DataSource="DataSourceBytes">
        <SpreadsheetRibbon></SpreadsheetRibbon>
    </SfSpreadsheet>
}

@code {
    public byte[] DataSourceBytes { get; set; }

    // Flag to indicate whether the data has been loaded
    public bool IsDataLoaded { get; set; }

    protected override async Task OnInitializedAsync()
    {
        // SECURITY: Validate URL against allowlist of trusted domains before use
        string jsonData = await HttpClient.GetStringAsync("URL"); // Note: Replace "URL" with your actual JSON endpoint URL
        
        // Transform the JSON data to an Excel byte array for Spreadsheet binding
        DataSourceBytes = ConvertJsonToExcel(jsonData);

        // Set flag to indicate data is loaded
        IsDataLoaded = true;
    }

    // Transforms a JSON string into an Excel workbook byte array using Syncfusion XlsIO
    private byte[] ConvertJsonToExcel(string jsonData)
    {
        using JsonDocument jsonDocument = JsonDocument.Parse(jsonData);
        JsonElement rootJsonElement = jsonDocument.RootElement;
        List<Dictionary<string, JsonElement>> dataRows = NormalizeJsonToRows(rootJsonElement);
        List<string> columnHeaders = dataRows.SelectMany(row => row.Keys).Distinct().ToList();

        using ExcelEngine excelEngine = new ExcelEngine();
        IApplication excelApplication = excelEngine.Excel;
        IWorkbook workbook = excelApplication.Workbooks.Create(1);
        IWorksheet worksheet = workbook.Worksheets[0];

        // Write header row
        int columnCount = columnHeaders.Count;
        for (int columnIndex = 0; columnIndex < columnCount; columnIndex++)
        {
            IRange headerCell = worksheet.Range[1, columnIndex + 1];
            headerCell.Text = columnHeaders[columnIndex];
        }

        // Write data rows starting from the second row
        int currentRowIndex = 2;
        foreach (var dataRow in dataRows)
        {
            for (int columnIndex = 0; columnIndex < columnCount; columnIndex++)
            {
                string columnKey = columnHeaders[columnIndex];
                if (dataRow.TryGetValue(columnKey, out JsonElement cellValue))
                {
                    worksheet.Range[currentRowIndex, columnIndex + 1].Value2 = cellValue;
                }
            }
            currentRowIndex++;
        }

        using MemoryStream memoryStream = new MemoryStream();
        workbook.SaveAs(memoryStream);
        return memoryStream.ToArray();
    }

    // Normalizes various JSON structures into a uniform list of row dictionaries
    private List<Dictionary<string, JsonElement>> NormalizeJsonToRows(JsonElement rootJsonElement)
    {
        if (rootJsonElement.ValueKind == JsonValueKind.Array)
        {
            return rootJsonElement.EnumerateArray().Select(JsonToDictionaryList).ToList();
        }

        if (rootJsonElement.ValueKind == JsonValueKind.Object)
        {
            foreach (var property in rootJsonElement.EnumerateObject())
            {
                if (property.Value.ValueKind == JsonValueKind.Array)
                {
                    return property.Value.EnumerateArray().Select(JsonToDictionaryList).ToList();
                }
            }
            return new List<Dictionary<string, JsonElement>> { JsonToDictionaryList(rootJsonElement) };
        }

        return new List<Dictionary<string, JsonElement>>
        {
            new Dictionary<string, JsonElement> { ["value"] = rootJsonElement }
        };
    }

    // Parses a JsonElement into a dictionary of property names and values
    private Dictionary<string, JsonElement> JsonToDictionaryList(JsonElement jsonElement)
    {
        if (jsonElement.ValueKind != JsonValueKind.Object)
        {
            return new Dictionary<string, JsonElement> { ["value"] = jsonElement };
        }

        return jsonElement.EnumerateObject()
        .ToDictionary(
        property => property.Name,
        property => property.Value,
        StringComparer.OrdinalIgnoreCase
        );
    }
}
```

### OPEN FROM GOOGLE DRIVE

Downloads an Excel file from Google Drive using the Drive API with service account authentication, converts the stream to a byte array, and binds it to the Spreadsheet.

> **⚠️ SECURITY**: Only download files from **verified sources**. Validate file IDs against an approved list and verify ownership before downloading. Never accept arbitrary user-provided file IDs.

**Prerequisites:**
- Google Cloud project in the Google Cloud Console
- Service account within the GCP project
- Service account key (JSON) available on disk
- Google Drive API enabled for the project
- Google Drive account with access to the file to download
- `Google.Apis.Drive.v3` NuGet package installed in your project

```csharp

@using Google.Apis.Auth.OAuth2
@using Google.Apis.Drive.v3
@using Google.Apis.Services
@using Syncfusion.Blazor.Spreadsheet
@using System.IO

@if (IsSpreadsheetDataLoaded)
{
    <SfSpreadsheet DataSource="DataSourceBytes">
        <SpreadsheetRibbon></SpreadsheetRibbon>
    </SfSpreadsheet>
}

@code {
    public byte[] DataSourceBytes { get; set; }

    // Flag to indicate whether the spreadsheet data has been loaded and is ready for rendering
    public bool IsSpreadsheetDataLoaded { get; set; }

    protected override async Task OnInitializedAsync()
    {
        // Download the document from Google Drive
        MemoryStream stream = await GetDocumentFromGoogleDrive();

        // Set the position to the beginning of the stream
        stream.Position = 0;

        // Convert the MemoryStream to a byte array to be used as the DataSource
        DataSourceBytes = stream.ToArray();

        // Set the flag to indicate that the spreadsheet data is ready
        IsSpreadsheetDataLoaded = true;
    }

    // Download file from Google Drive using service account credentials
    public async Task<MemoryStream> GetDocumentFromGoogleDrive()
    {
        // Define the path to the service account key file
        string serviceAccountKeyPath = "Your_service_account_key_path";

        // Specify the file ID of the file to download
        string fileID = "Your_file_id";

        try
        {
            // Authenticate the Google Drive API access using the service account key
            GoogleCredential credential = GoogleCredential.FromFile(serviceAccountKeyPath)
                .CreateScoped(DriveService.ScopeConstants.Drive);

            // Create the Google Drive service
            DriveService service = new DriveService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential
            });

            // Create a request to get the file from Google Drive
            var request = service.Files.Get(fileID);

            // Download the file into a MemoryStream
            MemoryStream stream = new MemoryStream();
            // SECURITY: Validate third-party downloaded file before processing to prevent content injection
            await request.DownloadAsync(stream);

            return stream;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error retrieving document from Google Drive: {ex.Message}");
            throw;
        }
    }
}
```

### Notes
- The Spreadsheet component accepts data only as a byte array through the `DataSource` property.
- To load JSON data, first convert it to Excel format using XlsIO, then convert to byte array.
- For Remote JSON, the component renders only after data is fetched (`IsDataLoaded` flag pattern).
- Supported file formats for opening: `.xlsx`, `.xls`.
- Supported file format for saving: `.xlsx`.
- **BeforeSave** fires *before* the workbook is saved and allows you to customize the file name or cancel the operation.
- `SaveAsync()` supports `SaveOptions` with `FileName` and `SaveType` customization.
- `SaveAsStreamAsync()` returns a `MemoryStream` for further processing or storage.
- For Google Drive: replace `Your_file_id` with the actual file ID from your Google Drive file URL.
  - URL format: `[GOOGLE_DRIVE_URL]/file/d/[FILE_ID]/view` → Extract the `[FILE_ID]` portion.
- For Google Drive: replace `Your_service_account_key_path` with the actual path to your service account JSON key file.
- **Important:** API methods should **NOT** be called inside `OnInitialized` or `OnParametersSet` lifecycle methods. Even if you call them, they will not work properly. Call API methods in response to user interactions (like button clicks) or in other appropriate lifecycle methods after the component is fully rendered.
- **Security:** When loading external data (remote JSON, Google Drive), always validate sources using allowlists, sanitize inputs, and implement proper authentication. Never accept arbitrary user-provided URLs or file IDs.

### Documentation link
[Blazor Spreadsheet Open and Save](https://help.syncfusion.com/document-processing/excel/spreadsheet/blazor/open-and-save)
