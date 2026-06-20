# Clipboard Operations

Manage cut, copy, and paste operations within the Document Editor using local or system clipboard modes.

## Clipboard Modes

The Document Editor supports two clipboard modes:

- **Local Clipboard** (default): Optimized for performance when working within the editor; content is isolated to the application and doesn't persist outside it.
- **System Clipboard**: Enables content transfer between the Document Editor and external applications; required for cross-app clipboard integration with a slight performance trade-off.

To switch from local to system clipboard, set `EnableLocalPaste` to `false` on the `SfDocumentEditorContainer`.

## Operations

### Copy

Copies the currently selected content to the clipboard using `CopyAsync()`.

```csharp
await container.DocumentEditor.Selection.CopyAsync();
```

### Cut

Removes the currently selected content and moves it to the clipboard using `CutAsync()`.

```csharp
await container.DocumentEditor.Editor.CutAsync();
```

### Paste

Inserts content from the clipboard at the current cursor position using `PasteAsync()`. The operation uses local or system clipboard based on the `EnableLocalPaste` property.

```csharp
await container.DocumentEditor.Editor.PasteAsync();
```

## Implementation Example

### Local Clipboard (Default)

```cshtml
@using Syncfusion.Blazor.DocumentEditor

<button @onclick="CopyClick">Copy</button>
<button @onclick="CutClick">Cut</button>
<button @onclick="PasteClick">Paste</button>

<SfDocumentEditorContainer @ref="container" EnableToolbar="true">
</SfDocumentEditorContainer>

@code {
    SfDocumentEditorContainer container;
    
    // Copies selected content to clipboard
    protected async void CopyClick()
    {
        await container.DocumentEditor.Selection.CopyAsync();
    }
    
    // Cuts selected content and moves it to clipboard
    protected async void CutClick()
    {
        await container.DocumentEditor.Editor.CutAsync();
    }
    
    // Pastes content from clipboard at cursor position
    protected async void PasteClick()
    {
        await container.DocumentEditor.Editor.PasteAsync();
    }
}
```

### System Clipboard Configuration

To enable system clipboard for cross-application content transfer, set `EnableLocalPaste="false"`:

```cshtml
<SfDocumentEditorContainer @ref="container" 
                           EnableToolbar="true" 
                           EnableLocalPaste="false">
</SfDocumentEditorContainer>
```

## Keyboard Shortcuts

Clipboard operations are also accessible via standard keyboard shortcuts:

- **Copy**: Ctrl+C
- **Cut**: Ctrl+X
- **Paste**: Ctrl+V

These shortcuts work automatically with both toolbar and context menu actions.
