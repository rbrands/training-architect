# Spell Check

Enable real-time spelling verification in the Document Editor with multi-language support, custom dictionaries, and contextual correction suggestions.

## Enable Spell Check

Activate spell checking by setting `EnableSpellCheck="true"` and configuring the server endpoint.

```csharp
<SfDocumentEditorContainer @ref="container" 
    Width="85%" 
    Height="590px" 
    ServiceUrl="https://localhost:5001/api/documenteditor/" 
    EnableSpellCheck="true" 
    EnableToolbar="true">
    <DocumentEditorContainerEvents Created="OnCreated"></DocumentEditorContainerEvents>
</SfDocumentEditorContainer>

@code {
    SfDocumentEditorContainer container;

    public async void OnCreated(object args)
    {
        var spellchecker = container.DocumentEditor.SpellChecker;
        // Configure spell check settings after component creation
        spellchecker.SetLanguageIDAsync(1033);
        spellchecker.SetRemoveUnderlineAsync(false);
        spellchecker.SetAllowSpellCheckAndSuggestionAsync(true);
    }
}
```

**Placeholders**
- `https://localhost:5001/api/documenteditor/` → Replace with `{serviceUrl}`

## Configure Language

Set the language for spell checking using Language Locale ID (LCID).

```csharp
await spellChecker.SetLanguageIDAsync(1033); // en-US
```

**Placeholders**
- `1033` → Replace with `{languageID}` (1033=en-US, 1036=fr-FR, 1031=de-DE)

## Control Underline Display

Show or hide the red squiggly underline that indicates misspelled words.

```csharp
await spellChecker.SetRemoveUnderlineAsync(false); // Show underline
```

## Enable Suggestions

Enable correction suggestions when spell checking detects errors.

```csharp
await spellChecker.SetAllowSpellCheckAndSuggestionAsync(true);
```

## Context Menu Actions

Right-click any misspelled word to access these options:
- **Suggestions** – Select a recommended spelling correction
- **Add to Dictionary** – Add the word permanently to the user's dictionary
- **Ignore Once** – Skip this instance without dictionary changes
- **Ignore All** – Skip all instances of this word in the current session
- **Spelling** – Open the spell check dialog for comprehensive review

## Server Architecture

The Document Editor client communicates with an ASP.NET Core Web API backend that hosts Syncfusion SpellChecker with Hunspell dictionaries. The server manages multi-language support, custom dictionaries, and performance optimization through dictionary caching.

### Server Endpoints

- **POST `/api/documenteditor/SpellCheck`** – Returns spelling errors and suggestions for text
- **POST `/api/documenteditor/SpellCheckByPage`** – Performs incremental page-by-page spell checking for optimized performance

### Server Setup

**Step 1: Create ASP.NET Core Web API Project**

```bash
dotnet new webapi -n SpellCheckServer
cd SpellCheckServer
```

**Step 2: Install Required NuGet Packages**

```bash
dotnet add package Syncfusion.EJ2.SpellChecker.AspNet.Core
dotnet add package Syncfusion.EJ2.WordEditor.AspNet.Core
dotnet add package Newtonsoft.Json
```

**Step 3: Configure Dictionary Files**

Create a `Data` folder in the project root and download Hunspell dictionaries from [wooorm/dictionaries](https://github.com/wooorm/dictionaries):
- `en/index.dic` – Valid words dictionary
- `en/index.aff` – Affix rules for word variations
- (Optional) `customDict.dic` – Personal dictionary for domain-specific terms

Create `Data/spellcheck.json` configuration:

```json
[
  {
    "LanguadeID": 1033,
    "DictionaryPath": "index.dic",
    "AffixPath": "index.aff",
    "PersonalDictPath": "customDict.dic"
  }
]
```

Ensure dictionary files copy to output directory. Add to `.csproj`:

```xml
<ItemGroup>
  <None Update="Data\**\*">
    <CopyToOutputDirectory>PreserveNewest</CopyToOutputDirectory>
  </None>
</ItemGroup>
```

**Step 4: Create Spell Check Controller**

Create `Controllers/DocumentEditorController.cs`:

```csharp
using Newtonsoft.Json;
using Syncfusion.EJ2.SpellChecker;
using Microsoft.AspNetCore.Cors;

[Route("api/[controller]")]
[ApiController]
[EnableCors("AllowAllOrigins")]
public class DocumentEditorController : ControllerBase
{
    [HttpPost("SpellCheck")]
    public string SpellCheck([FromBody] SpellCheckJsonData spellCheckData)
    {
        try
        {
            SpellChecker spellCheck = new SpellChecker();
            spellCheck.GetSuggestions(
                spellCheckData.LanguageID,
                spellCheckData.TexttoCheck,
                spellCheckData.CheckSpelling,
                spellCheckData.CheckSuggestion,
                spellCheckData.AddWord
            );
            return JsonConvert.SerializeObject(spellCheck);
        }
        catch
        {
            return "{\"SpellCollection\":[],\"HasSpellingError\":false,\"Suggestions\":null}";
        }
    }

    [HttpPost("SpellCheckByPage")]
    public string SpellCheckByPage([FromBody] SpellCheckJsonData spellCheckData)
    {
        try
        {
            SpellChecker spellCheck = new SpellChecker();
            spellCheck.CheckSpelling(spellCheckData.LanguageID, spellCheckData.TexttoCheck);
            return JsonConvert.SerializeObject(spellCheck);
        }
        catch
        {
            return "{\"SpellCollection\":[],\"HasSpellingError\":false,\"Suggestions\":null}";
        }
    }

    public class SpellCheckJsonData
    {
        public int LanguageID { get; set; }
        public string TexttoCheck { get; set; }
        public bool CheckSpelling { get; set; }
        public bool CheckSuggestion { get; set; }
        public bool AddWord { get; set; }
    }
}
```

**Step 5: Configure Program.cs**

Initialize spell check dictionaries at application startup:

```csharp
using Newtonsoft.Json;
using Syncfusion.EJ2.SpellChecker;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers().AddNewtonsoftJson();
builder.Services.AddMemoryCache();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins", policy =>
    {
        policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
    });
});
builder.Services.AddResponseCompression();

var app = builder.Build();

// Initialize dictionaries at startup for optimal performance
var env = app.Environment;
string dataPath = Path.Combine(env.ContentRootPath, "Data");
string jsonPath = Path.Combine(dataPath, "spellcheck.json");

if (File.Exists(jsonPath))
{
    var jsonContent = File.ReadAllText(jsonPath);
    var spellChecks = JsonConvert.DeserializeObject<List<DictionaryData>>(jsonContent);
    var spellDictCollection = new List<DictionaryData>();
    string personalDictPath = string.Empty;

    foreach (var check in spellChecks)
    {
        spellDictCollection.Add(
            new DictionaryData(
                check.LanguageID,
                Path.Combine(dataPath, check.DictionaryPath),
                Path.Combine(dataPath, check.AffixPath)
            )
        );
        personalDictPath = Path.Combine(dataPath, check.PersonalDictPath);
    }

    SpellChecker.InitializeDictionaries(spellDictCollection, personalDictPath, 2);
}

app.UseCors("AllowAllOrigins");
app.UseAuthorization();
app.MapControllers();
app.Run();
```

**Step 6: Build and Run**

```bash
dotnet build
dotnet run
```

The API server will start at `https://localhost:5001` (or your configured port). Update the `ServiceUrl` in your Blazor client to match.

## File-Based Changes

- `Components/DocEditor.razor` → Add `SfDocumentEditorContainer` with spell check enabled
- `appsettings.json` → Configure server endpoint URL
- Backend `Program.cs` → Initialize spell check dictionaries at startup
- Backend `Controllers/DocumentEditorController.cs` → Add spell check endpoints
