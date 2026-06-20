## EditingAction

| **Name** | **Description** | **Data type** |
|-----|-----|-----|
| **AnnotationAdded** | Represents an editing action indicating that the document was modified by including an annotation | enum |
| **AnnotationDeleted** | Represents an editing action indicating that the document was modified by removing an existing annotation | enum |
| **AnnotationImported** | Represents an editing action indicating that the document was modified by importing an annotation along with its properties | enum |
| **AnnotationUpdated** | Represents an editing action indicating that the document was modified by update the properties of an annotation | enum |
| **FormFieldImported** | Represents an editing action indicating that the document was modified by updating the values of form fields | enum |
| **FormFieldUpdated** | Represents an editing action indicating that the document was modified by updating the values of form fields | enum |

## ZoomMode

| **Name** | **Description** | **Data type** |
|-----|-----|-----|
| **Default** | Sets the Zoom Mode by Default. It represents the page rendered based on the zoom percentage. | enum |
| **FitToHeight** | Sets the ZoomMode by FitToHeight. It represents the page rendered based on the page container height. | enum |
| **FitToPage** | Sets the ZoomMode by FitToPage. It represents the page rendered based on the page container height. | enum |
| **FitToWidth** | Sets the Zoom Mode by FitToWidth. It represents the page rendered based on the page container width. | enum |

## InteractionMode

| **Name** | **Description** | **Data type** |
|-----|-----|-----|
| **Pan** | Represents the interaction mode with Pan. |
| **TextSelection**	| Represents the interaction mode with TextSelection. |

## LinkTarget

| **Name** | **Description** | **Data type** |
|-----|-----|-----|
| **CurrentTab** | Represents the hyperlink navigation with CurrentTab. | enum |
| **NewTab** | Represents the hyperlink navigation with NewTab. | enum |
| **NewWindow** | Represents the hyperlink navigation with NewWindow. | enum |

## PrintMode

| **Name** | **Description** | **Data type** |
|-----|-----|-----|
| **Default** | Represents the Print mode Default | enum |
| **NewWindow**	| Represents the Print mode in New Window | enum |

## AnnotationDataFormat

| **Name** | **Description** | **Data type** |
|-----|-----|-----|
| Json | Represents the default annotation data format Json. | enum |
| Xfdf | Represents the annotation data format Xfdf | enum |

## FormFieldDataFormat

| **Name** | **Description** | **Data type** |
|-----|-----|-----|
| **Json** | Form fields data in JSON format. | enum |
| **Fdf** | Form fields data in FDF (Forms Data Format). | enum |
| **Xfdf** | Form fields data in XFDF (XML Forms Data Format). | enum |
| **Xml** | Specifies XML file format. | enum |

## ContextMenuAction

| **Name** | **Description** | **Data type** |
|-----|-----|-----|
| **MouseUp** | Represents the ContextMenu Actions by MouseUp. It shows the context menu items when mouse up on the annotation added in the PDF Viewer. | enum |
| **None** | Represents the ContextMenu Actions by None. It doesn't show the context menu items. | enum |
| **RightClick** | Represents the ContextMenu Actions by RightClick. It shows the context menu items when right click on the annotation added in the PDF Viewer. | enum |

## ContextMenuItem

| **Name** | **Description** | **Data type** |
|-----|-----|-----|
| **Comment** |Represents the Comment option in the context menu. | enum |
| **Copy** | Represents the Copy option in the context menu. | enum |
| **Cut** | Represents the Cut option in the context menu. | enum |
| **Delete** | Represents the Delete option in the context menu. | enum |
| **Highlight** | Represents the Highlight option in the context menu. | enum |
| **Paste**	| Represents the Paste option in the context menu. | enum |
| **Properties** | Represents the Properties option in the context menu. | enum |
| **ScaleRatio** | Represents the Scale Ratio option in the context menu. | enum |
| **Squiggly** | Represents the Squiggly option in the context menu. | enum |
| **Strikethrough** | Represents the Strikethrough option in the context menu. | enum |
| **Underline**	| Represents the Underline option in the context menu. | enum |


## Bookmark

| **Name** | **Description** | **Data type** |
|-----|-----|-----|
| **Children** | Gets the child level bookmark collection of the current bookmark item.| List<[Bookmark](#bookmark)> |
| **Destination** | Gets the bookmark destination attributes of the specific bookmark. | [BookmarkDestination](#bookmarkdestination) |
| **HasChild** | Gets the value to check whether the bookmark have child level bookmarks or not. By default it is false. If the bookmark have children it returns true. | bool |
| **Id** | Gets the Id of the bookmark. | string |
| **Title** | Gets the title of bookmark. | string |

## BookmarkDestination

| **Name** | **Description** | **Data type** |
|-----|-----|-----|
| **PageNumber** | Gets the page number of the bookmark. | int |
| **X** | Gets the X co-orinate of the bookmark page. | double |
| **Y** | Gets the Y co-ordinate of the bookmark page. | double |

## RotationDirection

| **Name** | **Description** | **Data type** |
|-----|-----|-----|
| **Clockwise** | Rotates the page 90 degrees clockwise. | enum |
| **CounterClockwise** | Rotates the page 90 degrees counterclockwise. | enum |


## NavigationToolbarItem

| **Item** | **Description** |
|---|---|
| **Thumbnails** | Page thumbnail preview panel |
| **Bookmarks** | Document bookmarks/outline navigation |
| **CommentPanel** | Display annotations and comments |
| **PageOrganizer** | Organize, reorder, rotate, or delete pages |

## CustomNavigationToolbarItem

| **Property** | **Type** | **Description** |
|---|---|---|
| **Name** | string | Unique identifier for the custom toolbar item |
| **HeaderText** | string | Header text displayed at the top of the custom panel |
| **IconCss** | string | CSS class for the toolbar item icon (e.g., "e-icons e-help") |
| **TooltipText** | string | Tooltip text shown on hover |
| **Index** | int | Position within the navigation toolbar (lower index = earlier position) |
| **ItemType** | [NavigationToolbarItemType](#navigationtoolbaritemtype) | Type of item: `Button` or `Separator` |
| **Template** | RenderFragment | Custom Blazor template/component for the toolbar item UI |

## NavigationToolbarItemType

| **Type** | **Description** |
|---|---|
| **Button** | A clickable button that can trigger actions or show panels |
| **Separator** | A non-interactive visual separator to group toolbar items |

## TextBound

| **Name** | **Description** | **Data type** |
|-----|-----|-----|
| **Height** | Gets or sets the the height of the text. | double |
| **Width** | Gets or sets the the width of the text. | double |
| **X** | Gets or sets the the x co-ordinate value of the text. | double |
| **Y** | Gets or sets the the y co-orinate value of the text. | double |
| **Bottom** | Gets the the bottom value of the text. | double |
| **Left** | Gets the the left value of the text. | double |
| **Right** | Gets the the right value of the text. | double |
| **Top** | Gets the the top value of the text. | double |
| **PageNumber** | Defines the page number in which the text selection is started. | int |

## PageTextContent

| **Name** | **Description** | **Data type** |
|-----|-----|-----|
| **PageSize** | Gets the page size for the page. | SizeF |
| **PageText** | Gets the page text for the page. | string |
| **TextData** | Gets the list of text data for the page. | List<[TextContent](#textcontent)> |

## TextContent

| **Name** | **Description** | **Data type** |
|-----|-----|-----|
| **Bound** | Gets the bounds of the text. | RectangleF |
| **Text** | Gets the text. | string |