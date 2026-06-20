# Customize Toolbar

Customize the DocumentEditor toolbar by adding custom items, controlling visibility, and managing item states.

## Add Custom Toolbar Item

Add new toolbar items using `CustomToolbarItemModel` alongside existing items in the `ToolbarItems` property.

```csharp
<SfDocumentEditorContainer @ref="container" EnableToolbar=true ToolbarItems="@Items">
</SfDocumentEditorContainer>

@code {
    SfDocumentEditorContainer container;
    List<Object> Items = new List<Object>
    {
        new CustomToolbarItemModel() { Id = "save", Text = "Save" },
        "New", "Undo", "Redo", "Separator", "Image", "Table",
        "Hyperlink", "Bookmark", "TableOfContents", "Separator",
        "Header", "Footer", "PageSetup", "PageNumber", "Break"
    };
}
```

Handle custom item clicks by binding to the `OnToolbarClick` event:

```csharp
<SfDocumentEditorContainer @ref="container" EnableToolbar=true ToolbarItems="@Items">
    <DocumentEditorContainerEvents OnToolbarClick="@ToolbarClickHandler"></DocumentEditorContainerEvents>
</SfDocumentEditorContainer>

@code {
    SfDocumentEditorContainer container;
    internal string DocumentName { get; set; }
    List<Object> Items = new List<Object>
    {
        new CustomToolbarItemModel() { Id = "save", Text = "Save" },
        "New", "Undo", "Redo", "Separator", "Image", "Table",
        "Hyperlink", "Bookmark", "TableOfContents", "Separator",
        "Header", "Footer", "PageSetup", "PageNumber", "Break"
    };
    private async Task ToolbarClickHandler(ClickEventArgs args)
    {
        if (args.Item.Id == "save")
        {
            // Sample custom action
            await container.DocumentEditor.SaveAsync(DocumentName, FormatType.Docx);
        }
    }
}
```

## Show or Hide Toolbar Items

Control visibility of toolbar items by including or excluding them from the `ToolbarItems` collection.

### On Creation update toolbar items

```csharp
<SfDocumentEditorContainer @ref="container" EnableToolbar=true ToolbarItems="@Items">
</SfDocumentEditorContainer>

@code {
    SfDocumentEditorContainer container;
    string[] Items = new string[] { "New", "Undo", "Redo", "Comments" };
}
```

### Dynamically update toolbar items

Control toolbar visibility dynamically at runtime by updating the `ToolbarItems` collection:

```csharp

<SfButton CssClass="e-outline" @onclick="@Customize">Customize Toolbar</SfButton>

<SfDocumentEditorContainer @ref="Container" EnableToolbar=true ToolbarItems="@SelectedItems">
</SfDocumentEditorContainer>

@code {
    SfDocumentEditorContainer Container;
    string[] SelectedItems = new string[] { "New", "Undo", "Redo", "Separator", "Image", "Table",
        "Hyperlink", "Bookmark", "TableOfContents", "Separator",
        "Header", "Footer", "PageSetup", "PageNumber", "Break" };
    public string[] ToolbarOptions { get; set; } = new string[] { "New", "Open", "Undo", "Redo", "Comments", "TrackChanges", "Image", "Table" };
    internal string DocumentName { get; set; }

    public async Task Customize()
    {
        string[] selected = new string[] { "New", "Open", "Undo", "Redo", "Comments", "TrackChanges", "Image", "Table" };  // Get the selected toolbar items
        Container.ToolbarItems = selected;  // Dynamically update the toolbar with selected items
    }
}
```


## Enable or Disable Toolbar Items

Toggle the enabled state of toolbar items at runtime using `EnableItemAsync`.

```csharp
// Disable item at index 2
container.Toolbar.EnableItemAsync(2, false);
// Enable item at index 2
container.Toolbar.EnableItemAsync(2, true);
```

**Placeholders**
- `2` → Replace with {itemIndex} (0-based index of the toolbar item)

---

**Note:** Default toolbar items include: `'New'`, `'Open'`, `'Separator'`, `'Undo'`, `'Redo'`, `'Separator'`, `'Image'`, `'Table'`, `'Hyperlink'`, `'Bookmark'`, `'TableOfContents'`, `'Separator'`, `'Header'`, `'Footer'`, `'PageSetup'`, `'PageNumber'`, `'Break'`, `'InsertFootnote'`, `'InsertEndnote'`, `'Separator'`, `'Find'`, `'Separator'`, `'Comments'`, `'TrackChanges'`, `'Separator'`, `'LocalClipboard'`, `'RestrictEditing'`, `'Separator'`, `'FormFields'`, `'UpdateFields'`.
 