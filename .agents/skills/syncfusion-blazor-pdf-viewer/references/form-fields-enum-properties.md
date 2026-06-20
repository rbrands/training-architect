## FormFieldInfo

| **Name** | **Description** | **Data type** |
|-----|-----|-----|
| **BackgroundColor** | Gets or sets the background color of the form field in hexadecimal string format. | string |
| **BorderColor** | Gets or sets the border color of the form field in hexadecimal string format. | string |
| **Bounds** | Gets or sets the bounds of the form field in the PDF Viewer. | [Bound](#bound) |
| **Color** | Gets or sets the color of the form field in the PDF Viewer. | string |
| **CustomData** | Gets or sets custom data associated with the form field, allowing the storage of key-value pairs in dictionary format. | object |
| **FontFamily** | Gets or sets the font family used for rendering text within a form field in the PDF Viewer. | string |
| **FontSize** | Gets or sets the font size of the text in the form field. | double |
| **FontStyle** | Gets or sets the font style applied to the text content within a form field in the PDF document. | [FontStyle](#fontstyle) |
| **Id** | Gets the unique identifier of a form field in the PDF Viewer. | string |
| **IsReadOnly** | Gets or sets a value indicating whether the form field is read-only in the PDF Viewer. | bool |
| **IsRequired** | Gets or sets a value indicating whether the form field is required to be filled before exporting or downloading the PDF document. | bool |
| **Name** | Gets or sets the name of the form field in the PDF Viewer. | string |
| **PageNumber** | Gets or sets the page number of the form field in the PDF Viewer. | int |
| **TextAlignment** | Gets or sets the text alignment for a form field in the PDF Viewer. | [TextAlignment](#textalignment) |
| **Thickness** | Gets or sets the border thickness of the form field. | double |
| **TooltipText** | Gets or sets the tooltip text that provides additional information about the form field. | string |
| **Type** | Gets or sets the type of the form field, specified using an enumeration. | [FormFieldType](#formfieldtype) |
| **Visibility** | Gets or sets the visibility of the form field in the PDF Viewer. | [VisibilityMode](#visibilitymode) |

## VisibilityMode

| **Name** | **Description** | **Data type** |
|-----|-----|-----|
| **Hidden** | Represents a form field that remains completely hidden in both the PDF Viewer and print mode. | enum |
| **HiddenPrintable** | Represents a form field that is hidden in the PDF Viewer but visible in print mode. | enum |
| **Visible** | Represents a form field that is always visible in the PDF Viewer and in the print mode. | enum |
| **VisibleNotPrintable** | Represents a form field that is visible in the PDF Viewer but does not appear in the printed document. | enum |

## FormFieldType

| **Name** | **Description** | **Data type** |
|-----|-----|-----|
| **Button** | Represents the button field type. | enum |
| **CheckBox** | Represents the checkbox field type. | enum |
| **DropDown** | Represents the dropdown field type. | enum |
| **ListBox** | Represents the listbox field type. | enum |
| **Password** | Represents the password field type. | enum |
| **RadioButton** | Represents the radio button field type. | enum |
| **SignatureField** | Represents the signature field type. | enum |
| **Textbox** | Represents the textbox field type. | enum |

## FormField

| **Name** | **Description** | **Data type** |
|-----|-----|-----|
| **FontName** | specifies the fontName of the signature. | string |
| **Id** | Gets the id of the form field. | string |
| **IsChecked** | Gets or sets the state of the checkbox. | bool |
| **IsReadOnly** | If it is set as true, can't edit the form field in the PDF document. By default it is false. | bool |
| **IsSelected** | Gets or sets the selection state of the radio button. | bool |
| **Name** | Gets the name of the form field. | string |
| **SignatureType** | specifies the type of the signature. | List<[SignatureType](#signaturetype)> |
| **Type** | Gets the type of the form field. | [FormFieldType](#formfieldtype) |
| **Value** | Gets or sets the value of the form field. | string |

## SignatureType

| **Name** | **Description** | **Data type** |
|-----|-----|-----|
| **Draw** | Represents the default signature type. | enum |
| **Image** | Represents the image signature type. | enum |
| **Type** | Represents the text signature type. | enum |

## Bound

| **Property Name** | **Description** | **Data type** |
|-----|-----|-----|
| **Height** | Gets or sets the the height of the annotation or form field. | double |
| **Width** | Gets or sets the the width of the annotation or form field. | double |
| **X** | Gets or sets the the x co-ordinate value of the annotation or form field. | double |
| **Y** | Gets or sets the the y co-orinate value of the annotation or form field. | double |

## FontStyle 

| **Name** | **Description** | **Data type** |
|-----|-----|-----|
| **Bold** | Represents the text content style will be bold. | enum |
| **Italic** | Represents the text content style will be italic. | enum |
| **None** | Represents the text content style does not set. | enum |
| **Strikethrough** | Represents the text content style will be strikethrough. | enum |
| **Underline**	| Represents the text content style will be underline. | enum |

## TextAlignment

| **Name** | **Description** | **Data type** |
|-----|-----|-----|
| **Center** | Represents the text alignment in Center. The text content will be shown at center. | enum |
| **Justify** | Represents the text alignment of Justify. The text is aligned along the left margin. | enum |
| **Left** | Represents the text alignment in left. The text content will be shown in left side. | enum |
| **Right** | Represents the text alignment in Right. The text content will be shown in right side. | enum |