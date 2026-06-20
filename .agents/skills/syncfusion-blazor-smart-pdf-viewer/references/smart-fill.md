# Smart Fill Configuration

## Basic Setup

```razor
<SfSmartPdfViewer DocumentPath="https://cdn.syncfusion.com/content/pdf/pdf-succinctly.pdf">
    <SmartFillSettings />
</SfSmartPdfViewer>
```

## Enable Parameter

Controls Smart Fill button visibility and accessibility:

```razor
<SfSmartPdfViewer DocumentPath="document.pdf">
    <SmartFillSettings Enable="false" />
</SfSmartPdfViewer>
```

- **Default**: `true`
- **Active Only**: When PDF contains form fields
- **Use Case**: Toggle based on user roles or document content

## SmartFillSettings Class

The `SmartFillSettings` class configures AI-powered automatic form field population. It:
- Automates PDF form filling from clipboard or specified data
- Reduces manual input and errors
- Allows users to review and adjust values before finalizing

## Integration

Include `SmartFillSettings` component within `SfSmartPdfViewer`. Ensure the PDF document contains form fields for AI-powered filling.