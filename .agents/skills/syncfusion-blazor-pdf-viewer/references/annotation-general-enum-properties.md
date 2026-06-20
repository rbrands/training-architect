## Bound

| **Property Name** | **Description** | **Data type** |
|-----|-----|-----|
| **Height** | Gets or sets the the height of the annotation or form field. | double |
| **Width** | Gets or sets the the width of the annotation or form field. | double |
| **X** | Gets or sets the the x co-ordinate value of the annotation or form field. | double |
| **Y** | Gets or sets the the y co-orinate value of the annotation or form field. | double |

## PdfAnnotationProperties

| **Property Name** | **Description** | **Data type** |
|-----|-----|-----|
| **Author** | Gets the author value of the annotation. | string |
| **BorderColor** | Gets the border color of the free text annotation. | string |
| **BorderDashArray** | Gets the border dash array of the annotation. | string |
| **BorderWidth** | Gets the border width of the free text annotation. | int |
| **Color** | Gets the color of the textmarkup annotations - highlight, underline and strikethrough and squiggly. | string |
| **Data** | Gets the path data for ink and signature annotation. | string |
| **DefaultText** | Gets the text of freetext annotaton. | string |
| **FillColor** | Gets the fill color of the annotation. | string |
| **FontColor** | Gets font color for label text of shape and measure annotation. | string |
| **FontFamily** | Gets font family for label text of shape and measure annotation. | string |
| **FontSize** | Gets font size for label text of shape and measure annotation. | int |
| **FontStyle** | Gets or sets font style of the freetext annotation. | [FontStyle](#fontstyle) |
| **IsLock** | Gets the value to check whether annotation is editable or not. By default it false. If set as true, can't interact with the annotation. | bool |
| **LabelSettings** | Gets the label settings for the measure and shape annotation. | [PdfViewerShapeLabelSettings](#pdfviewershapelabelsettings) |
| **LineHeadEnd** | Gets the head end style of the line, arrow, distance, perimeter annotations. | [LineHeadStyle](#lineheadstyle) |
| **LineHeadStart** | Gets the head start style of the line, arrow, distance, perimeter annotations. | [LineHeadStyle](#lineheadstyle) |
| **MaxHeight** | Gets the maxHeight value for the annotations. | int |
| **MaxWidth** | Gets the maxWidth value for the annotations. | int |
| **MinHeight** | Gets the minHeight value for the annotations. | int |
| **MinWidth** | Gets the minwidth value for the annotations. | int |
| **ModifiedDate** | Gets the modified date of the annotation. | string |
| **Opacity** | Gets the opacity value of the annotation. | double |
| **StrokeColor** | Gets the stroke color of the annotation. | string |
| **Subject** | Gets the subject value of the annotation. | string |
| **TextAlignment** | Gets text alignment of freetext annotaton. | [TextAlignment](#textalignment) |
| **TextMarkupContent** | Gets or sets the text content of the textmarkup annotation. | string |
| **TextMarkupEndIndex** | Gets the text content end index of the textmarkup annotation. | int |
| **TextMarkupStartIndex** | Gets the text content start index of the textmarkup annotation. | int |
| **Thickness** | Gets the thickness value of the annotation. | int |
| **Type** | Gets or sets the type of the PDF annotation. | [AnnotationType](#annotationtype) |

## FontStyle 

| **Name** | **Description** | **Data type** |
|-----|-----|-----|
| **Bold** | Represents the text content style will be bold. | enum |
| **Italic** | Represents the text content style will be italic. | enum |
| **None** | Represents the text content style does not set. | enum |
| **Strikethrough** | Represents the text content style will be strikethrough. | enum |
| **Underline**	| Represents the text content style will be underline. | enum |

## PdfViewerShapeLabelSettings 

| **Name** | **Description** | **Data type** |
|-----|-----|-----|
| **FillColor** | specifies the fill color of the label. | string |
| **FontColor** | specifies the border color of the label. | string |
| **FontFamily** | specifies the max-width of the label. | string |
| **FontSize** | specifies the font size of the label. | int |
| **LabelContent** | specifies the default content of the label. | string |
| **Notes** | specifies the default content of the label. | string |
| **Opacity** | specifies the opacity of the label. | double |

## LineHeadStyle

| **Name** | **Description** | **Data type** |
|-----|-----|-----|
| **Arrow**	| Represents the line with Arrow head style. | enum |
| **Closed** | Represents the line with closed head style. | enum |
| **ClosedArrow** | Represents the line with Closed Arrow head style. | enum |
| **Diamond** | Represents the line with diamond head style. | enum |
| **None** | Represents the line with no head style. | enum |
| **Open** | Represents the line with open arrow head style. | enum |
| **OpenArrow** | Represents the line with Open Arrow head style. | enum |
| **Round** | Represents the line with round head style. | enum |
| **Square** | Represents the line with square head style. | enum |

## TextAlignment

| **Name** | **Description** | **Data type** |
|-----|-----|-----|
| **Center** | Represents the text alignment in Center. The text content will be shown at center. | enum |
| **Justify** | Represents the text alignment of Justify. The text is aligned along the left margin. | enum |
| **Left** | Represents the text alignment in left. The text content will be shown in left side. | enum |
| **Right** | Represents the text alignment in Right. The text content will be shown in right side. | enum |

## AnnotationType 

| **Name** | **Description** | **Data type** |
|-----|-----|-----|
| **Area** | Represents the Area annotation | enum |
| **Arrow**	| Represents the Arrow annotation | enum |
| **Circle** | Represents the Circle annotation | enum |
| **Distance** | Represents the Distance annotation | enum |
| **FreeText** | Represents the FreeText annotation | enum |
| **HandWrittenSignature** | Represents the HandWrittenSignature annotation | enum |
| **Highlight**	| Represents the Highlight annoation | enum |
| **Image**	| Represents the Image annotation | enum |
| **Ink** | Represents the Ink annotation | enum |
| **Line** | Represents the Line annotation | enum |
| **None** | Represents the annoation type by None | enum |
| **Perimeter** | Represents the Perimeter annotation | enum |
| **Polygon** | Represents the Polygon annotation | enum |
| **Radius** | Represents the Radius annotation | enum |
| **Rectangle** | Represents the Rectangle annotation | enum |
| **Redaction** | Represents the Redaction annotation | enum |
| **Squiggly** | Represents the Squiggly annotation | enum |
| **Stamp** | Represents the Stamp annotation | enum |
| **StickyNotes** | Represents the StickyNotes annotation | enum |
| **Strikethrough** | Represents the Strikethrough annoation | enum |
| **Underline** | Represents the Underline annoation | enum |
| **Volume** | Represents the volume annotation | enum |
