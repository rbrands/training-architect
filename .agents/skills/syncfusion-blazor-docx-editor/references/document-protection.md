# Restrict Editing

Control document editability by making content read-only or permitting changes only to specific portions.

## Read-only mode

Prevents all users from editing the entire document while preserving view access.

```tsx
@using Syncfusion.Blazor.DocumentEditor

<button @onclick="ReadOnly">Read Only</button>
<SfDocumentEditorContainer @ref="container" EnableToolbar=true></SfDocumentEditorContainer>

@code {
    SfDocumentEditorContainer container;

    public void ReadOnly(object args)
    {
        container.RestrictEditing = true;
    }
}
```

## Partial editing (restricted regions)

Allow users to edit only designated portions while protecting the rest of the document. Use the same UI patterns available in Microsoft Word for managing edit restrictions on specific sections.
