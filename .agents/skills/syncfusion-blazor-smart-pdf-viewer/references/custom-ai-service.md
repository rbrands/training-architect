# Custom AI Service Integration

## IChatInferenceService Interface

```csharp
public interface IChatInferenceService
{
    Task<string> GenerateResponseAsync(ChatParameters options);
}
```

## Service Registration

Register in `Program.cs`:

```csharp
using Syncfusion.Blazor.AI;
builder.Services.AddSingleton<IChatInferenceService, YourCustomService>();
```

## Error Handling

### Step 1: Create ErrorDialogService

```csharp
public class ErrorDialogService 
{
    public event Action OnDialogOpen;
    public string DialogMessage { get; set; }

    internal void RaiseDialogOpen()
    {
        OnDialogOpen?.Invoke();
    }
}
```

### Step 2: Use in Custom Service

```csharp
private readonly ErrorDialogService _errorDialogService;

public MyCustomService(IChatClient client, ErrorDialogService errorDialogService)
{
    this._errorDialogService = errorDialogService;
}

public async Task<string> GenerateResponseAsync(ChatParameters options)
{
    try
    {
        // Custom AI service logic
    }
    catch (Exception ex)
    {
        _errorDialogService.DialogMessage = ex.Message;
        _errorDialogService.RaiseDialogOpen();
        return "";
    }
}
```

### Step 3: Configure Program.cs

```csharp
builder.Services.AddScoped<ErrorDialogService>();
builder.Services.AddScoped<SfDialogService>();
builder.Services.AddScoped<IChatInferenceService, MyCustomService>(sp =>
{
    ErrorDialogService errorDialogService = sp.GetRequiredService<ErrorDialogService>();
    return new MyCustomService("YourChatClient", errorDialogService);
});
```

### Step 4: Add SfDialogProvider

In `Pages/Home.razor`:

```html
@page "/"

<Syncfusion.Blazor.Popups.SfDialogProvider/>
```

### Step 5: Handle Dialog Display

In `Pages/Home.razor.cs`:

```csharp
using Syncfusion.Blazor.Popups;

public partial class Home : IDisposable
{
    [Inject]
    public ErrorDialogService ErrorDialogService { get; set; }

    [Inject]
    public SfDialogService DialogService { get; set; }

    public async void OpenDialog()
    {
        string dialogText = ErrorDialogService.DialogMessage;
        await DialogService.AlertAsync(dialogText, "Error", new DialogOptions() 
        { 
            ShowCloseIcon = true, 
            Width = "400px" 
        });
    }

    protected override void OnInitialized()
    {
        ErrorDialogService.OnDialogOpen += OpenDialog;
    }

    public void Dispose()
    {
        ErrorDialogService.OnDialogOpen -= OpenDialog;
    }
}
```
