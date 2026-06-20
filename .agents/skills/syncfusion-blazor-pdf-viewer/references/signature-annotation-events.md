# Annotation Events in Blazor SfPdfViewer Component

## Table of Contents
- [Overview](#overview)
- [When to Use Annotation Events](#when-to-use-annotation-events)
- [Complete Event List](#complete-event-list)

## Overview

Signature Annotation events notify your application when users interact with annotations—adding, selecting, moving, resizing, modifying, or removing them. Use these events to track signature annotation changes, implement custom workflows, validate modifications, or synchronize annotation state with your application data.

## When to Use Annotation Events

**Guide users to subscribe to annotation events when they need to:**
- It allowing users to handle interactions, changes, and actions performed within the viewer for signature annotation only.
- When we do such action while signature annotation interaction process, the we can use the signature annotation events to handle to acheive those scenaios.

## Complete Event List

**Below is the comprehensive reference of all annotation events, their arguments, and properties. Use this as a lookup when you know which event you need.**

### AddSignature 
Triggers when a signature is added to a page in the PDF document.

#### Value
`EventCallback<AddSignatureEventArgs>`

#### AddSignature Events Arguments
| Event Arguments | Data Type | Description |
|-----|-----|-----|
| `Bound` | [Bound](./annotation-general-enum-properties.md#bound) | Gets the bounds of the signature added in the page of the PDF document. |
| `Opacity` | double | Gets the opacity of the signature added in the page of the PDF document. |
| `StrokeColor` | string | Define the stroke color of the signature added in the page of the PDF document. |
| `Thickness` | int | Define the thickness of the signature added in the page of the PDF document. |
| `Name` | string | Specifies name of the event. |
| `Id` | string | Gets the id of the signature added in the page of the PDF document. |
| `PageNumber` | int | Defines the page number in which the signature is added. |

#### Code Example
```razor
<SfPdfViewer2>
    <PdfViewerEvents AddSignature="SignatureAdded"></PdfViewerEvents>
</SfPdfViewer2>
@code {
    private void SignatureAdded(AddSignatureEventArgs args) {
        // customize your code in based on the requirements and check above table for event argument details
    }
}
```

### MoveSignature
Triggers when a signature is moved on a page in the PDF document.

#### Value
`EventCallback<MoveSignatureEventArgs>`

#### MoveSignature Events Arguments
| Event Arguments | Data Type | Description |
|-----|-----|-----|
| `CurrentPosition` | [Bound](./annotation-general-enum-properties.md#bound) | Current position of signature in the page text content. |
| `Opacity` | double | Get the opacity of the signature added in the page of the PDF document. |
| `PreviousPosition` | [Bound](./annotation-general-enum-properties.md#bound) | Gets the previous position of signature in the page text content. |
| `StrokeColor` | string | Gets the stroke color of the signature added in the page of the PDF document. |
| `Thickness` | int | Gets the thickness of the signature added in the page of the PDF document. |
| `Name` | string | Specifies name of the event. |
| `Id` | string | Gets the id of the signature added in the page of the PDF document. |
| `PageNumber` | int | Defines the page number in which the signature is added. |


#### Code Example
```razor
<SfPdfViewer2>
    <PdfViewerEvents MoveSignature="MoveSignatured"></PdfViewerEvents>
</SfPdfViewer2>
@code {
    private void MoveSignatured(MoveSignatureEventArgs args) {
        // customize your code in based on the requirements and check above table for event argument details
    }
}
```

### RemoveSignature
Triggers when a signature is removed from a page in the PDF document.

#### Value
`EventCallback<RemoveSignatureEventArgs>`

#### RemoveSignature Events Arguments
| Event Arguments | Data Type | Description |
|-----|-----|-----|
| `Name` | string | Specifies name of the event. |
| `Bound` | [Bound](./annotation-general-enum-properties.md#bound) | Gets the bounds of the signature removed in the page of the PDF document. |
| `Id` | string | Gets the id of the signature added in the page of the PDF document. |
| `PageNumber` | int | Defines the page number in which the signature is added. |

#### Code Example
```razor
<SfPdfViewer2>
    <PdfViewerEvents RemoveSignature="SignatureRemoved"></PdfViewerEvents>
</SfPdfViewer2>
@code {
    private void SignatureRemoved(RemoveSignatureEventArgs args) {
        // customize your code in based on the requirements and check above table for event argument details
    }
}
```

### ResizeSignature
Triggers when a signature is resized on a page in the PDF document.

#### Value
`EventCallback<ResizeSignatureEventArgs>`

#### ResizeSignature Events Arguments
| Event Arguments | Data Type | Description |
|-----|-----|-----|
| `CurrentPosition` | [Bound](./annotation-general-enum-properties.md#bound) | Gets the current Position of the signature added in the page of the PDF document. |
| `Opacity` | double | Define the opacity of the signature added in the page of the PDF document. |
| `PreviousPosition` | [Bound](./annotation-general-enum-properties.md#bound) | Gets the previous position of the signature added in the page of the PDF document. |
| `StrokeColor` | string | Gets the stroke color of the signature added in the page of the PDF document. |
| `Thickness` | int | Gets the thickness of the signature added in the page of the PDF document. |
| `Name` | string | Specifies name of the event. |
| `Id` | string | Gets the id of the signature added in the page of the PDF document. |
| `PageNumber` | int | Defines the page number in which the signature is added. |

#### Code Example
```razor
<SfPdfViewer2>
    <PdfViewerEvents ResizeSignature="SignatureResized"></PdfViewerEvents>
</SfPdfViewer2>
@code {
    private void SignatureResized(ResizeSignatureEventArgs args) {
        // customize your code in based on the requirements and check above table for event argument details
    }
}
```

### SignaturePropertiesChange
Triggers when the properties of a signature are changed on a page in the PDF document.

#### Value
`EventCallback<SignaturePropertiesChangeEventArgs>`

#### SignaturePropertiesChange Events Arguments
| Event Arguments | Data Type | Description |
|-----|-----|-----|
| `IsOpacityChanged` | bool | It returns true that the opacity of the signature is changed. |
| `IsStrokeColorChanged` | bool | It returns true that the stroke color of the signature is changed. |
| `IsThicknessChanged` | bool | It returns true that the thickness of the signature is changed. |
| `NewValue` | string | Gets the new property value of the signature. |
| `OldValue` | string | Gets the old property value of the signature. |
| `Name` | string | Specifies name of the event. |
| `Id` | string | Gets the id of the signature added in the page of the PDF document. |
| `PageNumber` | int | Defines the page number in which the signature is added. |

#### Code Example
```razor
<SfPdfViewer2>
    <PdfViewerEvents SignaturePropertiesChange="SignaturePropertiesChanged"></PdfViewerEvents>
</SfPdfViewer2>
@code {
    private void SignaturePropertiesChanged(SignaturePropertiesChangeEventArgs args) {
        // customize your code in based on the requirements and check above table for event argument details
    }
}
```

### SignatureSelected
Triggers when a signature is selected on a page in the PDF document.

#### Value
`EventCallback<SignatureSelectEventArgs>`

#### SignatureSelected Events Arguments
| Event Arguments | Data Type | Description |
|-----|-----|-----|
| `Name` | string | Specifies name of the event. |
| `Id` | string | Gets the id of the signature added in the page of the PDF document. |
| `PageNumber` | int | Defines the page number in which the signature is added. |

#### Code Example
```razor
<SfPdfViewer2>
    <PdfViewerEvents SignatureSelected="SignatureSelected"></PdfViewerEvents>
</SfPdfViewer2>
@code {
    private void SignatureSelected(SignatureSelectEventArgs args) {
        // customize your code in based on the requirements and check above table for event argument details
    }
}
```

### SignatureUnselected
Triggers when a signature is unselected on a page in the PDF document.

#### Value
`EventCallback<SignatureSelectEventArgs>`

#### SignatureUnselected Events Arguments
| Event Arguments | Data Type | Description |
|-----|-----|-----|
| `Name` | string | Specifies name of the event. |
| `Id` | string | Gets the id of the signature added in the page of the PDF document. |
| `PageNumber` | int | Defines the page number in which the signature is added. |

#### Code Example
```razor
<SfPdfViewer2>
    <PdfViewerEvents SignatureUnselected="SignatureUnselected"></PdfViewerEvents>
</SfPdfViewer2>
@code {
    private void SignatureUnselected(SignatureSelectEventArgs args) {
        // customize your code in based on the requirements and check above table for event argument details
    }
}
```
