# Document Summarizer Configuration

## Basic Setup
```razor
<SfSmartPdfViewer DocumentPath="path/to/document.pdf">
    <AssistViewSettings/>
</SfSmartPdfViewer>
```

## AssistViewSettings Parameters

### ShowPromptSuggestions
Displays suggested prompts to guide user queries (default: `true`).
```razor
<AssistViewSettings ShowPromptSuggestions="true" />
```

### Prompt
Defines query for AI assistant. Supports dynamic binding via `@bind-Prompt`.
```razor
<AssistViewSettings @bind-Prompt="@customPrompt" />
```

### PromptChanged
Callback triggered when user modifies prompt text.
```razor
<AssistViewSettings PromptChanged="@OnPromptChanged" />

@code {
    private void OnPromptChanged(string newPrompt)
    {
        // Handle prompt change
    }
}
```

### Placeholder
Sets input field hint text (default: "Type your prompt for assistance…").
```razor
<AssistViewSettings Placeholder="Enter your query..." />
```

### MinLength
Minimum characters required for AI processing (default: `100`).
```razor
<AssistViewSettings MinLength="100" />
```

### StreamResponse
Enables real-time streaming of AI responses (default: `false`).
```razor
<AssistViewSettings StreamResponse="true" />
```

### MaxRetryAttempts
Maximum retry attempts for AI processing (default: `3`).
```razor
<AssistViewSettings MaxRetryAttempts="3" />
```

### Timeout
Maximum wait time in seconds for AI response (default: `30`).
```razor
<AssistViewSettings Timeout="30" />
```

### Enable
Controls availability of Assist view (default: `true`).
```razor
<AssistViewSettings Enable="true" />
```

## InitialPromptSettings

### Prompt
Sets initial prompt in input field when Assist view opens.
```razor
<AssistViewSettings>
    <InitialPromptSettings Prompt="Explain this document." />
</AssistViewSettings>
```

### SuggestedPrompts
Provides list of predefined prompts to guide user interaction.
```razor
<AssistViewSettings>
    <InitialPromptSettings SuggestedPrompts="@suggestions" />
</AssistViewSettings>

@code {
    string[] suggestions = new[] {
        "What is the main purpose?",
        "Generate an overview.",
        "Identify key points."
    };
}
```

### PageStart
Starting page number for AI analysis.
```razor
<InitialPromptSettings PageStart="1" />
```

### PageEnd
Ending page number for AI analysis.
```razor
<InitialPromptSettings PageEnd="5" />
```

## Templates Customization

### PromptTemplate
Customizes prompt input toolbar.
```razor
@using Syncfusion.Blazor.InteractiveChat;

<AssistViewSettings>
    <PdfViewerTemplates>
        <PromptTemplate>
            <PromptToolbar>
                <PromptToolbarItem IconCss="e-icons e-assist-edit"></PromptToolbarItem>
                <PromptToolbarItem IconCss="e-icons e-assist-copy"></PromptToolbarItem>
            </PromptToolbar>
        </PromptTemplate>
    </PdfViewerTemplates>
</AssistViewSettings>
```

### ResponseTemplate
Customizes AI response toolbar with feedback options.
```razor
<AssistViewSettings>
    <PdfViewerTemplates>
        <ResponseTemplate>
            <ResponseToolbar>
                <ResponseToolbarItem IconCss="e-icons e-assist-like"></ResponseToolbarItem>
                <ResponseToolbarItem IconCss="e-icons e-assist-dislike"></ResponseToolbarItem>
            </ResponseToolbar>
        </ResponseTemplate>
    </PdfViewerTemplates>
</AssistViewSettings>
```

### BannerTemplate
Displays banner at top of Assist view.
```razor
<AssistViewSettings>
    <PdfViewerTemplates>
        <BannerTemplate>
            <div style="font-size: 14px; color:#5D3FD3;">
                AI-powered PDF Summarizer
            </div>
        </BannerTemplate>
    </PdfViewerTemplates>
</AssistViewSettings>
```

## Complete Example
```razor
<SfSmartPdfViewer DocumentPath="https://cdn.syncfusion.com/content/pdf/pdf-succinctly.pdf">
    <AssistViewSettings StreamResponse="true" MaxRetryAttempts="3" Timeout="30">
        <InitialPromptSettings Prompt="Summarize this document" PageStart="1" PageEnd="10" />
        <PdfViewerTemplates>
            <BannerTemplate>
                <div>AI Assistant Enabled</div>
            </BannerTemplate>
        </PdfViewerTemplates>
    </AssistViewSettings>
</SfSmartPdfViewer>
```
