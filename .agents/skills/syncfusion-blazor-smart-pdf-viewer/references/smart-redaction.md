# Smart Redaction Configuration

## Basic Implementation

```razor
<SfSmartPdfViewer DocumentPath="https://cdn.syncfusion.com/content/pdf/pdf-succinctly.pdf">
    <SmartRedactSettings/>
</SfSmartPdfViewer>
```

## Enable/Disable Smart Redaction

```razor
<SfSmartPdfViewer DocumentPath="https://cdn.syncfusion.com/content/pdf/pdf-succinctly.pdf">
    <SmartRedactSettings Enable="false" />
</SfSmartPdfViewer>
```

## Custom Redaction Patterns

```razor
<SfSmartPdfViewer DocumentPath="https://cdn.syncfusion.com/content/pdf/pdf-succinctly.pdf">
    <SmartRedactSettings RedactPatterns="@redactPatterns" />
</SfSmartPdfViewer>

@code {
    string[] redactPatterns = new string[] {
        "Person Names",
        "Email Addresses",
        "Phone Numbers",
        "Credit Card Numbers"
    };
}
```

## Default Patterns

- Person Names
- Organization Names
- Email Addresses
- Phone Numbers
- Addresses
- Dates
- Account Numbers
- Credit Card Numbers
