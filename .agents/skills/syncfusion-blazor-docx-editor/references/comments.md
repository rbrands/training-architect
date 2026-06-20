# Comments

Add, navigate, and manage comments in documents for collaborative review and feedback.

## Add a comment

Insert a comment on selected text to provide feedback or annotations.

```csharp
await container.DocumentEditor.Editor.InsertCommentAsync("Test comment");
```

**Placeholders**
- 'Test comment' → Replace with {comments}

## Navigate comments

Move between existing comments in the document.

```csharp
// Navigate to next comment
await container.DocumentEditor.Selection.NavigateNextCommentAsync();

// Navigate to previous comment
await container.DocumentEditor.Selection.NavigatePreviousCommentAsync();
```

## Delete current comment

Remove the active comment from the document.

```csharp
await container.DocumentEditor.Editor.DeleteCommentAsync();
```

## Delete all comments

Remove all comments from the entire document

```csharp
// Delete all comments and restore editor state
await container.DocumentEditor.Editor.DeleteAllCommentsAsync();
```

## Protect document in comments-only mode

Restrict editing to comments only, preventing content modifications while allowing users to add or edit comments.

```csharp
@using Syncfusion.Blazor.DocumentEditor

<button @onclick="ProtectDocumentAsync">Protect</button>
<SfDocumentEditorContainer @ref="container" EnableToolbar=true></SfDocumentEditorContainer>

@code {
    SfDocumentEditorContainer container;
    
    protected async Task ProtectDocumentAsync()
    {
        // Enforce comments-only protection
        await container.DocumentEditor.Editor.EnforceProtectionAsync("123", ProtectionType.CommentsOnly);
    }
}
```

**Placeholders**
- '123' → Replace with {password}
- 'ProtectDocumentAsync' → Replace with your event handler name

## Remove document protection

Stop protecting the document to allow full editing again.

```csharp
await container.DocumentEditor.Editor.StopProtectionAsync("123");
```

**Placeholders**
- '123' → Replace with {password}

## Protection types

Available protection modes: `NoProtection`, `ReadOnly`, `FormFieldsOnly`, `CommentsOnly`.

> **Note:** Comments-only protection can also be enabled through the Restrict Editing pane in the UI.
