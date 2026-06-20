
### ToolbarItem

| **Item** | **Description** |
|---|---|
| **OpenOption** | Load documents from file system |
| **PrintOption** | Print the PDF document |
| **DownloadOption** | Download the PDF document |
| **SearchOption** | Search within the PDF document |
| **MagnificationTool** | Zoom controls |
| **PageNavigationTool** | Bookmark and thumbnail navigation |
| **SelectionTool** | Text selection tool |
| **PanTool** | Pan mode for document navigation |
| **AnnotationEditTool** | Edit existing annotations |
| **CommentTool** | View annotations and comments |
| **FormDesigner** | Access form designer toolbar |
| **SubmitForm** | Export form data |
| **UndoRedoTool** | Undo and redo actions |
| **Redaction** | Access redaction toolbar |

### AnnotationToolbarItem

| **Item** | **Description** |
|---|---|
| **HighlightTool** | Highlight text |
| **UnderlineTool** | Underline text |
| **StrikethroughTool** | Strikethrough text |
| **SquigglyTool** | Squiggly underline text |
| **FreeTextAnnotationTool** | Add free text annotations |
| **HandWrittenSignatureTool** | Add handwritten signatures |
| **StampAnnotationTool** | Add stamp annotations |
| **ShapeTool** | Add shapes (line, circle, rectangle, polygon) |
| **InkAnnotationTool** | Add ink annotations |
| **CalibrateTool** | Add measurements (distance, perimeter, area, radius, volume) |
| **ColorEditTool** | Change annotation color |
| **StrokeColorEditTool** | Change stroke color |
| **ThicknessEditTool** | Change annotation thickness |
| **OpacityEditTool** | Change annotation opacity |
| **FontFamilyAnnotationTool** | Change text font family |
| **FontSizeAnnotationTool** | Change text font size |
| **FontStylesAnnotationTool** | Change text font style |
| **FontColorAnnotationTool** | Change text color |
| **FontAlignAnnotationTool** | Align text |
| **CommentPanelTool** | Show comment panel |
| **AnnotationDeleteTool** | Delete annotations |
| **CloseTool** | Close annotation toolbar |

### FormDesignerToolbarItem

| **Item** | **Description** |
|---|---|
| **TextBox** | Add text input field |
| **Password** | Add password field |
| **CheckBox** | Add checkbox field |
| **RadioButton** | Add radio button field |
| **DropDown** | Add dropdown field |
| **ListBox** | Add list box field |
| **Button** | Add button field |
| **Signature** | Add signature field |
| **Delete** | Delete form fields |

### MobileToolbarItem

| **Item** | **Description** |
|---|---|
| **Open** | Load documents |
| **Search** | Search in PDF |
| **EditAnnotation** | Edit annotations |
| **FormDesigner** | Access form designer |
| **PageOrganizer** | Reorder, rotate, or delete pages |
| **Redaction** | Access redaction tools |
| **UndoRedo** | Undo and redo actions |

### RedactionToolbarItem

| **Item** | **Description** |
|---|---|
| **MarkForRedaction** | Mark content for redaction |
| **Redact** | Apply redactions |
| **RedactPages** | Redact entire pages |
| **RemoveAnnotation** | Remove redaction marks |
| **RedactionPanel** | Show redaction panel |
| **CommentPanel** | Show comment panel |
| **Close** | Close redaction toolbar |

### PdfToolbarItem

| **Property** | **Type** | **Description** |
|---|---|---|
| **Index** | int | Position of the custom item in the toolbar |
| **Template** | RenderFragment | Custom UI for the toolbar item |

## Item Model

| **Property** | **Type** | **Description** |
|---|---|---|
| **CssClass** | string | Gets or sets the classes for toolbar item to customize the toolbar item. |
| **Id** | string | Gets or sets the unique ID for toolbar button or input element. |
| **PrefixIcon** | string | Gets or sets the classes to display an icon for toolbar button item. |