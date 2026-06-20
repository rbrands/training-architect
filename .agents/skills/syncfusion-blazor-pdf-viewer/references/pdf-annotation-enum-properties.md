## PdfAnnotation

| **Name** | **Description** | **Data type** |
|-----|-----|-----|
| **AllowedInteractions** | Gets or sets the allowed interactions for the locked area annotations. IsLock can be configured using area settings. | [AllowedInteraction](#allowedinteraction) |
| **AnnotationMode** | Gets the mode of annotation to be added in the PDF document. Three different modes are Drawn, Import and Existing. | [AnnotationMode](#annotationmode) |
| **AnnotationProperties** | Gets or sets the redaction-specific information for this annotation. | [AnnotationProperties](#redactionproperties--annotationproperties) (This property provides access to redaction-specific settings such as overlay text, text repetition, and marker appearance properties. It is only applicable for annotations of type Redaction.) |
| **AnnotationSettings** | Gets the annotation settings. | [PdfViewerAnnotationSettings](#pdfviewerannotationsettings) |
| **Bound** | Gets or sets the bound of the PDF annotation. | [Bound](#bound) |
| **Bounds** | Gets or sets the bounds details of the text markup annotation. | List<[Bound](#bound)> |
| **Comments** | Gets or sets the collection of comments details of the PDF annotation. | List<[Comment](#comment)> |
| **CustomStampSource** | Gets or sets the custom stamp source for the PDF annotation. | string |
| **DynamicText** | Gets default text of freetext annotaton. | string |
| **EnableShapeLabel** | Gets the value to check whether the label for the shape and measure annotation is enabled or not. By default it is false. | bool |
| **Id** | Gets the unique Id for the annotation. | string |
| **IdCollection** | Gets the Id's collection of the textmarkup annotation. | List<string> |
| **Indent** | Gets the intent of the measure annotation. | string |
| **IsMultiSelect** | Gets the value to check whether the textmarkup annotation added with multiple pages. By default it is false. If we add textmarkup annotation with multiple page region it will return true. | bool |
| **IsPrint** | Gets or sets the value for the individual annotations are included or not in print actions. | bool |
| **LabelBorderColor** | Gets or sets border color for the shape and measure annotation label. | string |
| **LabelBound** | Gets bound for label text of shape and measure annotation. | [Bound](#bound) |
| **LabelContent** | Gets or sets content of the shape label. | string |
| **LabelFillColor** | Gets or sets fillcolor for the shape and measure annotation label. | string |
| **LeaderLength** | Gets or sets the leader length of the annotation. | int |
| **LeaderLineExtension** | Gets the leader line extension of the measure annotation. | int |
| **LeaderLineOffset** | Gets the leader line offset of the measure annotation. | int |
| **Note** | Gets the note of the annotation. It represents comment text added for this annotation. | string |
| **PageNumber** | Gets or sets the page number of the PDF annotation. | int |
| **PageNumbers** | Gets the page number collection of the textmarkup annotation. | List<int> |
| **Rect** | Gets the rectangle of the textmarkup annotation. | Rect |
| **Review** | Gets or sets the review status details of the PDF annotation Comments. | [Review](#review) |
| **UniqueKey** | Gets or sets uniquekey for annotation. | string |
| **VertexPoints** | Gets or sets the list of vertex points defining the annotation shape. | List<[VertexPoint](#vertexpoint)> |

## AllowedInteraction

| **Name** | **Description** | **Data type** |
|-----|-----|-----|
| **Delete** | Represents to allow the delete interaction for the locked annotations. | enum |
| **Move** | Represents to allow the move interaction for the locked annotations. | enum |
| **None** | Represents to disallow the annotation interaction for the locked annotations. | enum |
| **PropertyChange** | Represents to allow the property change interaction for the locked annotations. | enum |
| **Resize** | Represents to allow the resize interaction for the locked annotations. | enum |
| **Select** | Represents to allow the select interaction for the locked annotations. | enum |

## AnnotationMode

| **Name** | **Description** | **Data type** |
|-----|-----|-----|
| **Drawn** | Reperesent the annotation added through user interactively. | enum |
| **Existing** | Reperesent the annotations are already existing in the PDF document. | enum |
| **Imported** | Reperesent the annotations are imported. | enum |

## RedactionProperties : AnnotationProperties

| **Name** | **Description** | **Data type** |
|-----|-----|-----|
| **IsRepeat** | Gets or sets a value indicating whether the overlay text should be repeated within the redaction area. | bool |
| **MarkerBorderColor** | Gets or sets the border color of the redaction marker. | string |
| **MarkerFillColor** | Gets or sets the fill color of the redaction marker. | string |
| **MarkerOpacity** | Gets or sets the opacity level of the redaction marker. | double |
| **OverlayText** | Gets or sets the overlay text displayed within the redaction annotation. | string |

## PdfViewerAnnotationSettings

| **Name** | **Description** | **Data type** |
|-----|-----|-----|
| **AllowedInteractions** | Gets or sets the allowed interactions for the locked area annotations. IsLock can be configured using area settings. | List<[AllowedInteraction](#allowedinteraction)> |
| **Author** | Specifies the author's name to add annotation or review the PDF document. By default it is Guest. | string |
| **CustomData** | Specifies the user's defined information related to the annotations. By default it is null. | object |
| **IsLock** | If it is set as true, can't interact with annotation. Otherwise can interact the annotations. By default it is false. | bool |
| **IsPrint** | Gets or sets the value for the individual annotations are included or not in print actions. | bool |
| **MaxHeight** | Sets the maximum height of annotations. It prevents the height of the annotation becoming larger than the values provided in MaxHeight.By default it is 0. | int |
| **MaxWidth** | Sets the maximum width of annotations. It prevents the width of the annotation becoming larger than values provided in MaxWidth.By default it is 0. | int |
| **MinHeight** | Sets the minimum height of annotations. It prevents the height of the annotation becoming smaller than values provided in MinHeight.By default it is 0. | int |
| **MinWidth** | Sets the minimum width of annotations. It prevents the width of the annotation becoming smaller than values provided in MinWidth.By default it is 0. | int |
| **SkipDownload** | If it is set as true, newly added annotations won't be included in downloaded file. By default it is false. | bool |
| **SkipPrint** | If it is set as true, newly added annotations won't be included in printing. By default it is false. | bool |

## Comment

| **Name** | **Description** | **Data type** |
|-----|-----|-----|
| **Id** | Gets the unique id of the annotation. | string |
| **Note** | Gets the note of the annotation. It represents comment text added for this annotation. | string |
| **PageNumber** | Gets the page number of the comment. | int |
| **ParentId** | Gets the id of the parent annotation. | string |
| **ReplyCollection** | Gets the reply collections of the comment. | List<[Comment](#comment)> |
| **Review** | Gets the review status details of the comment. | [Review](#review) |
| **ShapeAnnotationType** | Gets the type of the annotation. | string |

## Review

| **Name** | **Description** | **Data type** |
|-----|-----|-----|
| **Author** | Gets the author value of the annotation. | string |
| **Id** | Gets the unique id of the annotation. | string |
| **ModifiedDate** | Gets the modified date of the review comment. | string |
| **State** | Gets the state of the comment. | string |
| **StateModel** | Gets the state model of the comment. | string |

## Rect

| **Name** | **Description** | **Data type** |
|-----|-----|-----|
| **Bottom** | Gets the the bottom value of the annotation. | double |
| **Left** | Gets the the left value of the annotation. | double |
| **Right** | Gets the the right value of the annotation. | double |
| **Top** | Gets the the top value of the annotation. | double |

## VertexPoint

| **Name** | **Description** | **Data type** |
|-----|-----|-----|
| **X** | Gets or sets the X-coordinate of the vertex point. | double |
| **Y** | Gets or sets the Y-coordinate of the vertex point. | double |

## Bound

| **Property Name** | **Description** | **Data type** |
|-----|-----|-----|
| **Height** | Gets or sets the the height of the annotation or form field. | double |
| **Width** | Gets or sets the the width of the annotation or form field. | double |
| **X** | Gets or sets the the x co-ordinate value of the annotation or form field. | double |
| **Y** | Gets or sets the the y co-orinate value of the annotation or form field. | double |
