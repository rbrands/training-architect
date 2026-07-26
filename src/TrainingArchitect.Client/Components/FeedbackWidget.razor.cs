using System.Net.Http.Json;
using Microsoft.AspNetCore.Components;

namespace TrainingArchitect.Client.Components;

public partial class FeedbackWidget : ComponentBase, IDisposable
{
    private const int NegativeSendDebounceMilliseconds = 500;
    private const int ConfirmationDurationMilliseconds = 2000;

    [Inject]
    private HttpClient HttpClient { get; set; } = default!;

    [Inject]
    private ILogger<FeedbackWidget> Logger { get; set; } = default!;

    [Parameter, EditorRequired]
    public string RequestType { get; set; } = string.Empty;

    [Parameter, EditorRequired]
    public string ResponseId { get; set; } = string.Empty;

    private readonly HashSet<string> _selectedTags = [];
    private readonly List<string> _availableTags = [];

    private CancellationTokenSource? _debounceCts;
    private bool _showNegativeTags;
    private bool _showConfirmation;
    private bool _isHidden;
    private bool _isSendingOrSent;

    protected override void OnParametersSet()
    {
        if (_isSendingOrSent)
        {
            return;
        }

        _availableTags.Clear();

        if (FeedbackTags.ByRequestType.TryGetValue(RequestType, out var tags) && tags.Length > 0)
        {
            _availableTags.AddRange(tags);
        }
    }

    private Task OnPositiveClick()
    {
        if (_isSendingOrSent)
        {
            return Task.CompletedTask;
        }

        StartSend("positive", tags: null);
        return Task.CompletedTask;
    }

    private Task OnNegativeClick()
    {
        if (_isSendingOrSent)
        {
            return Task.CompletedTask;
        }

        _showNegativeTags = true;
        return Task.CompletedTask;
    }

    private void ToggleTag(string tag)
    {
        if (_isSendingOrSent)
        {
            return;
        }

        if (_selectedTags.Contains(tag))
        {
            _selectedTags.Remove(tag);
        }
        else
        {
            _selectedTags.Add(tag);
        }

        ScheduleNegativeFeedbackSend();
    }

    private void ScheduleNegativeFeedbackSend()
    {
        _debounceCts?.Cancel();
        _debounceCts?.Dispose();

        if (_selectedTags.Count == 0)
        {
            _debounceCts = null;
            return;
        }

        var cts = new CancellationTokenSource();
        _debounceCts = cts;

        _ = DebouncedNegativeSendAsync(cts.Token);
    }

    private async Task DebouncedNegativeSendAsync(CancellationToken ct)
    {
        try
        {
            await Task.Delay(NegativeSendDebounceMilliseconds, ct);
        }
        catch (OperationCanceledException)
        {
            return;
        }

        if (ct.IsCancellationRequested || _isSendingOrSent || _selectedTags.Count == 0)
        {
            return;
        }

        var tags = string.Join(", ", _selectedTags.Order(StringComparer.Ordinal));
    }

    private void StartSend(string rating, string? tags)
    {
        if (_isSendingOrSent)
        {
            return;
        }

        _isSendingOrSent = true;
        _showNegativeTags = false;
        _showConfirmation = true;

        _debounceCts?.Cancel();
        _debounceCts?.Dispose();
        _debounceCts = null;

        StateHasChanged();

        _ = SendAndFinalizeAsync(rating, tags);
    }

    private async Task SendAndFinalizeAsync(string rating, string? tags)
    {
        try
        {
            var payload = new FeedbackRequest(ResponseId, RequestType, rating, tags);
            using var response = await HttpClient.PostAsJsonAsync("/api/feedback", payload);
            response.EnsureSuccessStatusCode();
        }
        catch (Exception ex)
        {
            Logger.LogWarning(
                ex,
                "Feedback submission failed for RequestType={RequestType}, ResponseId={ResponseId}",
                RequestType,
                ResponseId);

            _showConfirmation = false;
            _isSendingOrSent = false;
            await InvokeAsync(StateHasChanged);
            return;
        }

        try
        {
            await Task.Delay(ConfirmationDurationMilliseconds);
        }
        catch
        {
            // Ignore timing issues during teardown.
        }

        _showConfirmation = false;
        _isHidden = true;

        await InvokeAsync(StateHasChanged);
    }

    public void Dispose()
    {
        _debounceCts?.Cancel();
        _debounceCts?.Dispose();
        _debounceCts = null;
    }

    private sealed record FeedbackRequest(string ResponseId, string RequestType, string Rating, string? Tags);
}