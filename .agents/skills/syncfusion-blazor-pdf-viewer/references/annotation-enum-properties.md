## DynamicStampItem

| **Name** | **Description** | **Data type** |
|-----|-----|-----|
| **Approved** | Represents a stamp indicating the document is approved.  | enum |
| **Confidential** | Represents a stamp indicating the document is confidential. | enum |
| **NotApproved** | Represents a stamp indicating the document is not approved. | enum |
| **Received** | Represents a stamp indicating the document has been received. | enum |
| **Reviewed** | Represents a stamp indicating the document has been reviewed. | enum |
| **Revised** | Represents a stamp indicating the document has been revised. | enum |

## SignStampItem

| **Name** | **Description** | **Data type** |
|-----|-----|-----|
| **Accepted** | Represents a stamp indicating the document is accepted. | enum |
| **InitialHere** | Represents a stamp indicating the initial placement here. | enum |
| **Rejected** | Represents a stamp indicating the document is rejected. | enum |
| **SignHere** | Represents a stamp indicating where the sign is needed. | enum |
| **Witness** | Represents a stamp indicating a witness is required. | enum |

## StandardBusinessStampItem

| **Name** | **Description** | **Data type** |
|-----|-----|-----|
| **Approved** | Represents a stamp indicating the document is approved. | enum |
| **Completed** | Represents a stamp indicating the document is completed. | enum |
| **Confidential** | Represents a stamp indicating the document is confidential. | enum |
| **Draft** | Represents a stamp indicating the document is a draft. | enum |
| **Final** | Represents a stamp indicating the document is final. | enum |
| **ForComment** | Represents a stamp indicating the document is for comment. | enum |
| **ForPublicRelease** | Represents a stamp indicating the document is for public release. | enum |
| **InformationOnly** | Represents a stamp indicating the document is for information only. | enum |
| **NotApproved** | Represents a stamp indicating the document is not approved. | enum |
| **NotForPublicRelease** | Represents a stamp indicating the document is not for public release. | enum |
| **PreliminaryResults** | Represents a stamp indicating the document contains preliminary results. | enum |
| **Void** | Represents a stamp indicating the document is void. | enum |

## PdfViewerCustomStamp

| **Name** | **Description** | **Data type** |
|-----|-----|-----|
| **CustomStampImageSource** | Defines the custom stamp images source to be added in stamp menu of the PDF Viewer toolbar. | string |
| **CustomStampName** | Defines the custom stamp name to be added in stamp menu of the PDF Viewer toolbar. | string |

## PdfViewerSignatureDialogSettings

| **Name** | **Description** | **Data type** |
|-----|-----|-----|
| **DisplayMode** | Get or set the required signature options will be enabled in the signature dialog. | [DisplayMode](#displaymode) |
| **HideSaveSignature** | Hide the save signature checkbox in the signature dialog box. | bool |
| **SignatureFonts** | Gets or sets the array of font families that will be available in the signature dialog's "Type" tab for signature fields. | string[] |

## DisplayMode

| **Name** | **Description** | **Data type** |
|-----|-----|-----|
| **Draw** | Display only the draw option in the signature dialog. | enum |
| **Text** | Display only the type option in the signature dialog. | enum |
| **Upload** | Display only the upload option in the signature dialog. | enum |

## CalibrationUnit

| **Name** | **Description** | **Data type** |
|-----|-----|-----|
| **Cm** | Represents the unit of centi meter. | enum |
| **Ft** | Represents the unit of feet. | enum |
| **In** | Represents the unit of inch. | enum |
| **Mm** | Represents the unit of meter | enum |
| **P**	| Represents the unit of points | enum |
| **Pt** | Represents the unit of points | enum |