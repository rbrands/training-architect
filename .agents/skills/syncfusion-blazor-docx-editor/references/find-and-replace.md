# Find and Replace

## Open or close the navigation pane
Programmatically toggle the search/find navigation pane to allow users to search for text within documents.

```csharp
@using Syncfusion.Blazor.DocumentEditor

<button @onclick="OpenOptionsPane">OpenOptionsPane</button>
<SfDocumentEditorContainer @ref="container" EnableToolbar=true></SfDocumentEditorContainer>

@code {
    SfDocumentEditorContainer container;
    protected void OpenOptionsPane(object args)
    {
        container.DocumentEditor.OpenOptionsPane();
    }
}
```

## Search for text
Find all instances of a word or phrase within the document with optional search filters.

```csharp
await container.DocumentEditor.Search.FindAllAsync("text to find", FindOption.None);
```

**Placeholders**
- `"text to find"` → Replace with {searchTerm}
- `FindOption.None` → Replace with {searchOptions} (e.g., `FindOption.MatchCase`, `FindOption.WholeWords`)

## Wire up search button
Attach the search functionality to a toolbar button or custom control.

```csharp
@using Syncfusion.Blazor.DocumentEditor

<button @onclick="SearchDocument">Find Text</button>
<SfDocumentEditorContainer @ref="container" EnableToolbar=true></SfDocumentEditorContainer>

@code {
    SfDocumentEditorContainer container;
    
    protected async Task SearchDocument()
    {
        await container.DocumentEditor.Search.FindAllAsync("search term", FindOption.None);
    }
}
```

## Keyboard shortcuts
- **Ctrl+F** – Open navigation pane
- **ESC** – Close navigation pane