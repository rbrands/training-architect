# Comment Panel in Blazor SfPdfViewer Component

## Table of Contents
- [Command Panel](#command-manager)
- [Properties](#properties)
- [Code Example](#code-example)

## Command Panel
Comments, replies, and status can be added to a PDF document using the comment panel. Annotation comments can be added using the comment panel. We can modifiy the command panel visibility, module injection and it's settings properties.

## Properties

### EnableCommentPanel
If set to false, the comment panel will be disabled from the PDF Viewer functionalities. By default it is true.
- `false` - Disables the command panel funtionalities from the PDF Viewer.
- `true` - Enables the command panel funtionalities from the PDF Viewer.

#### Data Type and default value
`bool` - `true(Default)`

#### Code Example
```razor
<SfPdfViewer2 EnableCommentPanel="true"></SfPdfViewer2>
```

### CommentPanelVisible
Defines the visibility of the comment panel in the PDF Viewer. It is UI based reflection, that hide or show the command panel in the PDF Viewer.
- `false` - Hides the command panel from the PDF Viewer.
- `true` - Shows the command panel from the PDF Viewer.

#### Data Type and default value
`bool` - `false(Default)`

#### Code Example
```razor
<SfPdfViewer2 CommentPanelVisible="true"></SfPdfViewer2>
```

### CommentPanelVisibleChanged
EventCallback to update the CommentPanelVisible value in the parent component.

#### Data Type
`EventCallback<bool>`

#### Code Example
```razor
<SfPdfViewer2 CommentPanelVisibleChanged="CommentPanelVisibleChanged"></SfPdfViewer2>

@code {
    private void CommentPanelVisibleChanged() {
        // Method implementation based on the user requirement
    }
}
```

### CommentPanelSettings
Configuration object for comment panel behavior (multiline, date format). Use this to customize annotation comment UX. 

#### Properties List
| **Property Name** | **Description** | **Data type** |
|-----|-----|-----|
| `Multiline` | Enable or disable the use of multiline text boxes in the comment panel. By default it is false. | bool |
| `DateTimeFormat` | Allows the customization of dates within the comment panel. | string |

#### Code Example
```razor
<SfPdfViewer2>
    <PdfViewerCommentPanelSettings DateTimeFormat="dd:MM:yyyy" Multiline="true"></PdfViewerCommentPanelSettings>
</SfPdfViewer2>
```

