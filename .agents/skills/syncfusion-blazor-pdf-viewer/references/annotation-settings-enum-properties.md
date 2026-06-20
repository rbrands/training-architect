## PdfViewerAnnotationSelectorSettings

| **Property Name** | **Description** | **Data type** |
|-----|-----|-----|
| **ResizerBorderColor** | Defines the annotation resizer border color. By default it is black. | string |
| **ResizerCursorType** | Defines the annotation resizer Type. By default it is null. | [CursorType](#cursortype) |
| **ResizerFillColor** | Defines the annotation resizer fill color. | string |
| **ResizerLocation** | Defines the location for the resizer of the annotation. It is used to customize the resizer location of the annotation. | [AnnotationResizerLocation](#annotationresizerlocation) |
| **ResizerShape** | Defines the shape of the resizer. By default it is Square. Different shapes of resizer are circle and square. | [AnnotationResizerShape](#annotationresizershape) |
| **ResizerSize** | Defines the size of the resizer used for annotations. | int |
| **SelectionBorderColor** | Defines the selection border color for the annotation. By default it is empty. It is used to customize the selection border color for the annotation. | string |
| **SelectionBorderThickness** | Defines the selection border thickness for the annotation. By default it is 1. It is used to customize the selection border thickness for the annotation. It's range varies from 1 to 10. | double |
| **SelectorLineDashArray** | Defines the selector line dash array. By default it is empty. | double[] |

#### CursorType

| **Property Name** | **Description** | **Data type** |
|-----|-----|-----|
| **Auto** | Represents the default cursor type Auto. | enum |
| **CrossHair**	| Represents the cursor type CrossHair. | enum |
| **E_resize** | The cursor indicates that an edge of a box is to be moved right (east). | enum |
| **EastResize** | The cursor indicates that an edge of a box is to be moved right (east). | enum |
| **EastWestResize** | Represents a bidirectional resize cursor. | enum |
| **Ew_resize** | Represents a bidirectional resize cursor. | enum |
| **Grab** | Represents a grab cursor. | enum |
| **GrabHandle** | Represents GrabHandle cursor type. | enum |
| **Grabbing** | Represents a grabbing cursor. | enum |
| **Move** | Represents a Move cursor when moving on something. | enum |
| **N_resize** | The cursor indicates that an edge of a box is to be moved up (north). | enum |
| **Ne_resize** | The cursor indicates that an edge of a box is to be moved up and right (north/east). | enum |
| **NorthEastResize** | The cursor indicates that an edge of a box is to be moved up and right (north/east). | enum |
| **NorthResize** | The cursor indicates that an edge of a box is to be moved up (north). | enum |
| **NorthSouthResize** | Represents a bidirectional resize cursor. | enum |
| **NorthWestResize** | The cursor indicates that an edge of a box is to be moved up and left (north/west). | enum |
| **Ns_resize**	| Represents a bidirectional resize cursor. | enum |
| **Nw_resize**	| The cursor indicates that an edge of a box is to be moved up and left (north/west). | enum |
| **Pointer** | Represents Pointer cursor type. | enum |
| **S_resize** | The cursor indicates that an edge of a box is to be moved down (south). | enum |
| **Se_resize** | The cursor indicates that an edge of a box is to be moved down and right (south/east). | enum |
| **SouthEastResize** | The cursor indicates that an edge of a box is to be moved down and right (south/east). | enum |
| **SouthResize** | The cursor indicates that an edge of a box is to be moved down (south). | enum |
| **SouthWestResize** | The cursor indicates that an edge of a box is to be moved down and left (south/west). | enum |
| **Sw_resize** | The cursor indicates that an edge of a box is to be moved down and left (south/west). | enum |
| **Text** | The cursor indicates text that may be selected. | enum |
| **W_resize** | The cursor indicates that an edge of a box is to be moved left (west). | enum |
| **WestResize** | The cursor indicates that an edge of a box is to be moved left (west). | enum |

#### AnnotationResizerLocation

| **Property Name** | **Description** | **Data type** |
|-----|-----|-----|
| **Corners** | When resizing annotation, Resizer location is represented by corners. | enum |
| **Edges**	| When resizing annotation, Resizer location is represented by Edges. | enum |

#### AnnotationResizerShape

| **Property Name** | **Description** | **Data type** |
|-----|-----|-----|
| **Circle** | Represent the Resizer shape by Circle when resizing annotations. | enum |
| **Square** | Represent the Resizer shape by Square when resizing annotations. | enum |

### PageInfo Class

| **Property** | **Type** | **Description** |
|---|---|---|
| **PageNumber** | int | The 1-based page number (first page is 1, not 0) |
| **Width** | double | The width of the page in pixels |
| **Height** | double | The height of the page in pixels |
| **RotationAngle** | [PageRotation](#pagerotation) | The current rotation angle applied to the page |

### DocumentInfo Class

| **Property** | **Type** | **Description** |
|---|---|---|
| **PageCount** | int | Total number of pages in the document |
| **PageSizes** | Dictionary<int, SizeF> | Dictionary containing page sizes for each page in the document |

## PageRotation

| **Property** | **Type** | **Description** |
|---|---|---|
| **RotateAngle0** | No rotation applied to the page (0 degrees). | enum |
| **RotateAngle180** | Rotates the page by 180 degrees. | enum |
| **RotateAngle270** | Rotates the page by 270 degrees clockwise (or 90 degrees counterclockwise). | enum |
| **RotateAngle90** | Rotates the page by 90 degrees clockwise. | enum |

## FooterButton

| **Name** | **Description** | **Data type** |
|-----|-----|-----|
| **None** | No buttons are shown in the Page Organizer dialog footer. |
| **Save** | Show the "Save" button in the Page Organizer dialog footer. |
| **SaveAs** | Show the "SaveAs" button in the Page Organizer dialog footer. |
