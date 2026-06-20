# Annotation Events in Blazor SfPdfViewer Component

## Table of Contents
- [Overview](#overview)
- [When to Use Annotation Events](#when-to-use-annotation-events)
- [Complete Event List](#complete-event-list)

## Overview

Annotation events notify your application when users interact with annotations—adding, selecting, moving, resizing, modifying, or removing them. Use these events to track annotation changes, implement custom workflows, validate modifications, or synchronize annotation state with your application data.

## When to Use Annotation Events

**Guide users to subscribe to annotation events when they need to:**
- It allowing users to handle interactions, changes, and actions performed within the viewer for annotation (Except Signature Annotation).
- When we do such action while annotation interaction process, the we can use the annotation events to handle to acheive those scenaios.

## Complete Event List

**Below is the comprehensive reference of all annotation events, their arguments, and properties. Use this as a lookup when you know which event you need.**

### AnnotationAdded
Triggers when an annotation is added to a page in the PDF document.

#### Value
`EventCallback<AnnotationAddEventArgs>`

#### AnnotationAdded Events Arguments
| Event Arguments | Data Type | Description |
|-----|-----|-----|
| `AnnotationProperties` | [PdfAnnotationProperties](./annotation-general-enum-properties.md#pdfannotationproperties) | Gets the settings of the annotation added to the PDF document. |
| `Bound` | [Bound](./annotation-general-enum-properties.md#bound) | Gets the bound of the annotations. |
| `Bounds` | List<[Bound](./annotation-general-enum-properties.md#Bound)> | Gets the textmark annotation bounds details. |
| `CustomStampName` | string | Defines the name of the custom stamp added to the PDF page. |
| `LabelSettings` | [PdfViewerShapeLabelSettings](./annotation-general-enum-properties.md#PdfViewerShapeLabelSettings) | LabelSettings for shape and measure annotation. |
| `MultiplePageCollection` | List<[PdfAnnotation](./pdf-annotation-enum-properties.md#PdfAnnotation)> | Defines the multi page annotation collections. |
| `Name` | string | Specifies name of the event. |
| `AnnotationId` | string | Defines the id of the annotation added in the page of the PDF document. |
| `AnnotationType` | [AnnotationType](./annotation-general-enum-properties.md#AnnotationType) | Define the type of the annotation added in the page of the PDF document.
| `PageNumber` | int | Defines the page number in which the annotation is added. |

#### Code Example
```razor
<SfPdfViewer2>
    <PdfViewerEvents AnnotationAdded="AnnotationAdded"></PdfViewerEvents>
</SfPdfViewer2>
@code {
    private void AnnotationAdded(AnnotationAddEventArgs args) {
        // customize your code in based on the requirements and check above table for event argument details
    }
}
```

### AnnotationMouseover
Triggers when the mouse pointer moves over an annotation object.

#### Value
`EventCallback<AnnotationMouseoverEventArgs>`

#### AnnotationMouseover Events Arguments
| Event Arguments | Data Type | Description |
|-----|-----|-----|
| `AnnotationProperties` | [PdfAnnotationProperties](./annotation-general-enum-properties.md#PdfAnnotationProperties) | Defines the settings of the annotation added to the PDF document. |
| `Bound` | [Bound](./annotation-general-enum-properties.md#Bound) | Gets the bound of the annotations. |
| `Bounds` | List<[Bound](./annotation-general-enum-properties.md#Bound)> - Gets the textmark annotation bounds details. |
| `PageX` | double | Gets the pageX co-ordinate. |
| `PageY` | double | Gets the PageY co-ordinate. |
| `X` | double | Gets the X co-orinate of the annotation. |
| `Y` | double | Gets the Y co-orinate of the annotation. |
| `Name` | string | Specifies name of the event. |
| `AnnotationId` | string | Defines the id of the annotation added in the page of the PDF document. |
| `AnnotationType` | [AnnotationType](./pdf-annotation-enum-properties.md#AnnotationType) | Define the type of the annotation added in the page of the PDF document.
| `PageNumber` | int | Defines the page number in which the annotation is added. |

#### Code Example
```razor
<SfPdfViewer2>
    <PdfViewerEvents AnnotationMouseover="AnnotationMouseovered"></PdfViewerEvents>
</SfPdfViewer2>
@code {
    private void AnnotationMouseOvered(AnnotationMouseoverEventArgs args) {
        // customize your code in based on the requirements and check above table for event argument details
    }
}
```

### AnnotationMoved
Triggers when an annotation is moved on a page in the PDF document.

#### Value
`EventCallback<AnnotationMoveEventArgs>`

#### AnnotationMoved Events Arguments
| Event Arguments | Data Type | Description |
|-----|-----|-----|
| `AnnotationProperties` | [PdfAnnotationProperties](./annotation-general-enum-properties.md#PdfAnnotationProperties) | Gets the settings of the annotation moved in the PDF document. |
| `CurrentPosition` | [Bound](./annotation-general-enum-properties.md#bound) | Current position of annotations in the page text content. |
| `PreviousPosition` | [Bound](./annotation-general-enum-properties.md#bound) | Previous position of annotations in the page text content. |
| `Name` | string | Specifies name of the event. |
| `AnnotationId` | string | Defines the id of the annotation added in the page of the PDF document. |
| `AnnotationType` | [AnnotationType](./annotation-general-enum-properties.md#annotationtype) | Define the type of the annotation added in the page of the PDF document.
| `PageNumber` | int | Defines the page number in which the annotation is added. |

#### Code Example
```razor
<SfPdfViewer2>
    <PdfViewerEvents AnnotationMoved="AnnotationMove"></PdfViewerEvents>
</SfPdfViewer2>
@code {
    private void AnnotationMove(AnnotationMoveEventArgs args) {
        // customize your code in based on the requirements and check above table for event argument details
    }
}
```

### AnnotationPropertiesChanged
Triggers when annotation properties are modified on a PDF page.

#### Value
`EventCallback<AnnotationPropertiesChangeEventArgs>`

#### AnnotationPropertiesChanged Events Arguments
| Event Arguments | Data Type | Description |
|-----|-----|-----|
| `IsBorderDashArrayChanged` | bool | It returns true when the border dash array of the annotation is changed.By default it is false. |
| `IsColorChanged` | bool | It returns true when the color of the annotation is changed.By default it is false. |
| `IsCommentsChanged` | bool | It returns true when the comment of the annotation is changed.By default it is false. |
| `IsLineHeadEndStyleChanged` | bool | It returns true when the line head end style of the annotation is changed.By default it is false. |
| `IsLineHeadStartStyleChanged` | bool | Specifies that the line head start style of the annotation is changed. |
| `IsOpacityChanged` | bool | Specifies that the opacity of the annotation is changed. |
| `IsStrokeColorChanged` | bool | Specifies that the stroke color of the annotation is changed. |
| `IsTextChanged` | bool | Specifies that the Text of the annotation is changed. |
| `IsThicknessChanged` | bool | Specifies that the thickness of the annotation is changed. |
| `MultiplePageCollection` | List<[PdfAnnotation](./pdf-annotation-enum-properties.md#pdfannotation)> - Gets the multipage text markup annotation collection. |
| `TextMarkupContent` | string | Gets the selected text content in the text markup annotation. |
| `TextMarkupEndIndex` | int | Gets the end index of text markup annotation in the page text content. |
| `TextMarkupStartIndex` | int | Gets the starting index of text markup annotation in the page text content. |
| `Name` | string | Specifies name of the event. |
| `AnnotationId` | string | Defines the id of the annotation added in the page of the PDF document. |
| `AnnotationType` | [AnnotationType](./annotation-general-enum-properties.md#annotationtype) | Define the type of the annotation added in the page of the PDF document.
| `PageNumber` | int | Defines the page number in which the annotation is added. |

#### Code Example
```razor
<SfPdfViewer2>
    <PdfViewerEvents AnnotationPropertiesChanged="AnnotationModified"></PdfViewerEvents>
</SfPdfViewer2>
@code {
    private void AnnotationModified(AnnotationPropertiesChangeEventArgs args) {
        // customize your code in based on the requirements and check above table for event argument details
    }
}
```

### AnnotationRemoved
Triggers when an annotation is removed / deleted from a page in the PDF document.

#### Value
`EventCallback<AnnotationRemoveEventArgs>`

#### AnnotationRemoved Events Arguments
| Event Arguments | Data Type | Description |
|-----|-----|-----|
| `Bound` | [Bound](./annotation-general-enum-properties.md#bound) | Gets the bound of the annotations. |
| `Bounds` | List<[Bound](./annotation-general-enum-properties.md#bound)> | Gets the textmark annotation bounds details. |
| `MultiplePageCollection` | List<[PdfAnnotation](./pdf-annotation-enum-properties.md#pdfannotation)> | Defines the multi page annotation collections. |
| `TextMarkupContent` | string | Defines the selected text content in the text markup annotation. |
| `TextMarkupEndIndex` | int | End index of text markup annotation in the page text content. |
| `TextMarkupStartIndex` | int | Starting index of text markup annotation in the page text content. |
| `Name` | string | Specifies name of the event. |
| `AnnotationId` | string | Defines the id of the annotation added in the page of the PDF document. |
| `AnnotationType` | [AnnotationType](./annotation-general-enum-properties.md#annotationtype) | Define the type of the annotation added in the page of the PDF document.
| `PageNumber` | int | Defines the page number in which the annotation is added. |

#### Code Example
```razor
<SfPdfViewer2>
    <PdfViewerEvents AnnotationRemoved="AnnotationDeleted"></PdfViewerEvents>
</SfPdfViewer2>
@code {
    private void AnnotationDeleted(AnnotationRemoveEventArgs args) {
        // customize your code in based on the requirements and check above table for event argument details
    }
}
```

### AnnotationResized
Triggers when an annotation is resized on a page in the PDF document.

#### Value
`EventCallback<AnnotationResizeEventArgs>`

#### AnnotationResized Events Arguments
| Event Arguments | Data Type | Description |
|-----|-----|-----|
| `AnnotationProperties` | [PdfAnnotationProperties](./annotation-general-enum-properties.md#pdfannotationproperties) | Gets the settings of the annotation added to the PDF document. |
| `Bound` | [Bound](./annotation-general-enum-properties.md#bound) | Gets the bound of the annotations. |
| `Bounds` | List<[Bound](./annotation-general-enum-properties.md#bound)> | Gets the textmark annotation bounds details. |
| `CustomStampName` | string | Defines the name of the custom stamp added to the PDF page. |
| `LabelSettings` | [PdfViewerShapeLabelSettings](./annotation-general-enum-properties.md#pdfviewershapelabelsettings) | LabelSettings for shape and measure annotation. |
| `MultiplePageCollection` | List<[PdfAnnotation](./pdf-annotation-enum-properties.md#pdfannotation)> | Defines the multi page annotation collections. |
| `Name` | string | Specifies name of the event. |
| `AnnotationId` | string | Defines the id of the annotation added in the page of the PDF document. |
| `AnnotationType` | [AnnotationType](./annotation-general-enum-properties.md#annotationtype) | Define the type of the annotation added in the page of the PDF document.
| `PageNumber` | int | Defines the page number in which the annotation is added. |
| `TextMarkupContent` | string | Gets the selected text content in the text markup annotation. |
| `TextMarkupEndIndex` | int | Gets the end index of text markup annotation in the page text content. |
| `TextMarkupStartIndex` | int | Gets the Starting index of text markup annotation in the page text content. |

#### Code Example
```razor
<SfPdfViewer2>
    <PdfViewerEvents AnnotationResized="AnnotationResized"></PdfViewerEvents>
</SfPdfViewer2>
@code {
    private void AnnotationResized(AnnotationResizeEventArgs args) {
        // customize your code in based on the requirements and check above table for event argument details
    }
}
```

### AnnotationSelected
Triggers when an annotation is selected on a page in the PDF document.

#### Value
`EventCallback<AnnotationSelectEventArgs>`

#### AnnotationSelected Events Arguments
| Event Arguments | Data Type | Description |
|-----|-----|-----|
| `AnnotationCollection` | List<[PdfAnnotation](./pdf-annotation-enum-properties.md#pdfannotation)> | Defines the overlapped annotations of the selected annotation. |
| `AnnotationId` | string | Defines the id of the annotation selected in the page of the PDF document. |
| `AnnotationProperties` | [PdfAnnotationProperties](./annotation-general-enum-properties.md#pdfannotationproperties) |  Defines the annotation selected in the PDF document. |
| `IsProgrammaticSelection` | bool | Defines the annotation selection by mouse. |
| `MultiplePageCollection` | List<[PdfAnnotation](./pdf-annotation-enum-properties.md#pdfannotation)> | Gets the multi page annotation collections. |
| `PageNumber` | int | Gets the page number in which the annotation is selected. |
| `Name` | string | Specifies name of the event. |

#### Code Example
```razor
<SfPdfViewer2>
    <PdfViewerEvents AnnotationSelected="AnnotationSelected"></PdfViewerEvents>
</SfPdfViewer2>
@code {
    private void AnnotationSelected(AnnotationSelectEventArgs args) {
        // customize your code in based on the requirements and check above table for event argument details
    }
}
```

### AnnotationUnselected
Triggers when an annotation is unselected on a page in the PDF document.

#### Value
`EventCallback<AnnotationUnselectEventArgs>`

#### AnnotationUnselected Events Arguments
| Event Arguments | Data Type | Description |
|-----|-----|-----|
| `Annotation` | [PdfAnnotation](./pdf-annotation-enum-properties.md#pdfannotation) | Gets the instance of Annotation which is unselected by the user. |
| `AnnotationId` | string | Gets the ID of Annotation which is unselected by the user. |
| `PageNumber` | int | Gets the page number in which the annotation is unselected. |
| `Name` | string | Specifies name of the event. |

#### Code Example
```razor
<SfPdfViewer2>
    <PdfViewerEvents AnnotationUnselected="AnnotationUnselected"></PdfViewerEvents>
</SfPdfViewer2>
@code {
    private void AnnotationUnselected(AnnotationUnselectEventArgs args) {
        // customize your code in based on the requirements and check above table for event argument details
    }
}
```

### OnAnnotationDoubleClick
Triggers when an annotation is double-clicked.

#### Value
`EventCallback<AnnotationDoubleClickEventArgs>`

#### OnAnnotationDoubleClick Events Arguments
| Event Arguments | Data Type | Description |
|-----|-----|-----|
| `AnnotationId` | string | Gets the id of the annotation double clicked in the page of the PDF document. | 
| `AnnotationProperties` | [PdfAnnotationProperties](./annotation-general-enum-properties.md#pdfannotationproperties) | Gets the annotation double clicked in the PDF document. |
| `PageNumber` | int | Defines the page number in which the annotation is double clicked. |
| `Name` | string | Specifies name of the event. |

#### Code Example
```razor
<SfPdfViewer2>
    <PdfViewerEvents OnAnnotationDoubleClick="OnAnnotationDoubleClicked"></PdfViewerEvents>
</SfPdfViewer2>
@code {
    private void OnAnnotationDoubleClicked(AnnotationDoubleClickEventArgs args) {
        // customize your code in based on the requirements and check above table for event argument details
    }
}
```

### ExportStarted
Triggers when exporting annotations starts in the SfPdfViewer. | ExportStartEventArgs | No Properties | Show loading indicators or prevent UI interactions during export |

#### Value
`EventCallback<ExportStartEventArgs>`

#### ExportStarted Events Arguments
| Event Arguments | Data Type | Description |
|-----|-----|-----|
| `Name` | string | Specifies name of the event. |

#### Code Example
```razor
<SfPdfViewer2>
    <PdfViewerEvents ExportStarted="ExportStarted"></PdfViewerEvents>
</SfPdfViewer2>
@code {
    private void ExportStarted(ExportStartEventArgs args) {
        // customize your code in based on the requirements and check above table for event argument details
    }
}
```

### ExportSucceed
Triggers when exporting annotations succeeds in the SfPdfViewer.

#### Value
`EventCallback<ExportSuccessEventArgs>`

#### ExportSucceed Events Arguments
| Event Arguments | Data Type | Description |
|-----|-----|-----|
| `Name` | string | Specifies name of the event. |
| `FileName` | string | Gets the exported annotations json file name. |

#### Code Example
```razor
<SfPdfViewer2>
    <PdfViewerEvents ExportSucceed="ExportSucceed"></PdfViewerEvents>
</SfPdfViewer2>
@code {
    private void ExportSucceed(ExportSuccessEventArgs args) {
        // customize your code in based on the requirements and check above table for event argument details
    }
}
```

### ExportFailed
Triggers when exporting annotations fails in the SfPdfViewer.

#### Value
`EventCallback<ExportFailureEventArgs>`

#### ExportFailed Events Arguments
| Event Arguments | Data Type | Description |
|-----|-----|-----|
| `Name` | string | Specifies name of the event. |
| `ErrorDetails` | string | Gets the error details for export annotations. |

#### Code Example
```razor
<SfPdfViewer2>
    <PdfViewerEvents ExportFailed="ExportFailed"></PdfViewerEvents>
</SfPdfViewer2>
@code {
    private void ExportFailed(ExportFailureEventArgs args) {
        // customize your code in based on the requirements and check above table for event argument details
    }
}
```

### ImportStarted
Triggers when importing annotations starts in the PDF document. |  | No Properties | Show loading indicators or prevent UI interactions during import |

#### Value
`EventCallback<ImportStartEventArgs>`

#### ImportStarted Events Arguments
| Event Arguments | Data Type | Description |
|-----|-----|-----|
| `Name` | string | Specifies name of the event. |

#### Code Example
```razor
<SfPdfViewer2>
    <PdfViewerEvents ImportStarted="ImportStarted"></PdfViewerEvents>
</SfPdfViewer2>
@code {
    private void ImportStarted(ImportStartEventArgs args) {
        // customize your code in based on the requirements and check above table for event argument details
    }
}
```

### ImportSucceed
Triggers when importing annotations succeeds in the PDF document.

#### Value
`EventCallback<ImportSuccessEventArgs>`

#### ImportSucceed Events Arguments
| Event Arguments | Data Type | Description |
|-----|-----|-----|
| `Name` | string | Specifies name of the event. |

#### Code Example
```razor
<SfPdfViewer2>
    <PdfViewerEvents ImportSucceed="ImportSucceed"></PdfViewerEvents>
</SfPdfViewer2>
@code {
    private void ImportSucceed(ImportSuccessEventArgs args) {
        // customize your code in based on the requirements and check above table for event argument details
    }
}
```

### ImportFailed
Triggers when importing annotations fails in the PDF document.

#### Value
`EventCallback<ImportFailureEventArgs>`

#### ImportFailed Events Arguments
| Event Arguments | Data Type | Description |
|-----|-----|-----|
| `Name` | string | Specifies name of the event. |
| `ErrorDetails` | string | Error details for import annotations. |

#### Code Example
```razor
<SfPdfViewer2>
    <PdfViewerEvents ImportFailed="ImportFailed"></PdfViewerEvents>
</SfPdfViewer2>
@code {
    private void ImportFailed(ImportFailureEventArgs args) {
        // customize your code in based on the requirements and check above table for event argument details
    }
}
```
