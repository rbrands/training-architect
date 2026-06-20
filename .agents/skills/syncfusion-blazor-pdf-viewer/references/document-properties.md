# Syncfusion Blazor PDF Viewer - Document Properties Reference

## Table of Contents
- [Overview](#overview)
- [Properties](#properties)
- [Events](#events)

## Overview
The Syncfusion Blazor PDF Viewer component provides comprehensive document-level properties and events for managing PDF document loading, editing, and interaction. This reference guide covers all available properties, inherited APIs, events, and practical code examples for implementing advanced document handling features.

## Properties

### EnableChunkMessages
Gets or sets a boolean value indicating whether to allow processing large files without increasing the maximum message size.
- `false` - Disables the chunck message functionalities.
- `true` - Enables the chunck message functionalities.

#### Data Type and default value
`bool` - `false(Default)`

#### Code Example
```razor
<SfPdfViewer2 EnableChunkMessages="true"></SfPdfViewer2>
```

## Events

### DocumentEdited
Triggers when the PDF document is edited in the SfPdfViewer.

#### Value
`EventCallback<DocumentEditedEventArgs>`

#### DocumentEdited Events Arguments
| Event Arguments | Data Type | Description |
|-----|-----|-----|
| `EditingAction` | [EditingAction](./general-enum-properties.md#editingaction) | Specifies the action that trigger the DocumentEdit event |

#### Code Example
```razor
<SfPdfViewer2>
    <PdfViewerEvents DocumentEdited="DocumentEdited"></PdfViewerEvents>
</SfPdfViewer2>
@code {
    private void DocumentEdited(DocumentEditedEventArgs args) {
        // customize your code in based on the requirements and check above table for event argument details
    }
}
```

### DocumentLoaded
Triggers when a document is loaded into the PDF Viewer.

#### Value
`EventCallback<LoadEventArgs>`

#### DocumentLoaded Events Arguments
| Event Arguments | Data Type | Description |
|-----|-----|-----|
| `DocumentName` | string | Document name to be loaded into PDF Viewer. | 
| `PageData` | [DocumentInfo](./annotation-settings-enum-properties.md#documentinfo-class) | Gets the pagecount, size for each page of the loaded PDF document. |

#### Code Example
```razor
<SfPdfViewer2>
    <PdfViewerEvents DocumentLoaded="DocumentLoaded"></PdfViewerEvents>
</SfPdfViewer2>
@code {
    private void DocumentLoaded(LoadEventArgs args) {
        // customize your code in based on the requirements and check above table for event argument details
    }
}
```

### DocumentLoadFailed
Triggers when loading a document into the PDF Viewer fails.

#### Value
`EventCallback<LoadFailedEventArgs>`

#### DocumentLoadFailed Events Arguments
| Event Arguments | Data Type | Description |
|-----|-----|-----|
| `DocumentName` | string | Document name to be loaded into PDF Viewer. |
| `IsPasswordRequired` | bool | Defines the document password protected state. |

#### Code Example
```razor
<SfPdfViewer2>
    <PdfViewerEvents DocumentLoadFailed="DocumentLoadFailed"></PdfViewerEvents>
</SfPdfViewer2>
@code {
    private void DocumentLoadFailed(LoadFailedEventArgs args) {
        // customize your code in based on the requirements and check above table for event argument details
    }
}
```

### DocumentUnloaded
Triggers when the document is closed or unloaded.

#### Value
`EventCallback<UnloadEventArgs>`

#### DocumentUnloaded Events Arguments
| Event Arguments | Data Type | Description |
|-----|-----|-----|
| `DocumentName` | string | Document name to be loaded into PDF Viewer. |

#### Code Example
```razor
<SfPdfViewer2>
    <PdfViewerEvents DocumentUnloaded="DocumentUnloaded"></PdfViewerEvents>
</SfPdfViewer2>
@code {
    private void DocumentUnloaded(UnloadEventArgs args) {
        // customize your code in based on the requirements and check above table for event argument details
    }
}
```

### PageMouseover
Triggers when the mouse pointer moves over a page.

#### Value
`EventCallback<PageMouseoverEventArgs>`

#### PageMouseover Events Arguments
| Event Arguments | Data Type | Description |
|-----|-----|-----|
| `PageX` | double | Mouseover x position with respect to page container. |
| `PageY` | double | Mouseover y position with respect to page container. |

#### Code Example
```razor
<SfPdfViewer2>
    <PdfViewerEvents PageMouseover="PageMouseovered"></PdfViewerEvents>
</SfPdfViewer2>
@code {
    private void PageMouseovered(PageMouseoverEventArgs args) {
        // customize your code in based on the requirements and check above table for event argument details
    }
}
```

## OnPageClick
Triggers when a mouse click is performed on a page in the PDF document.

#### Value
`EventCallback<PageClickEventArgs>`

#### OnPageClick Events Arguments
| Event Arguments | Data Type | Description |
|-----|-----|-----|
| `DocumentName` | string | Gets the Document name to be loaded into PDF Viewer. |
| `PageNumber` | int | Page number of the document in which click action is performed. |
| `PageX` | double | Gets the x co-ordinate of the click action location. |
| `PageY` | double | Gets y co-ordinate of the click action location. |

#### Code Example
```razor
<SfPdfViewer2>
    <PdfViewerEvents OnPageClick="OnPageClicked"></PdfViewerEvents>
</SfPdfViewer2>
@code {
    private void OnPageClicked(PageClickEventArgs args) {
        // customize your code in based on the requirements and check above table for event argument details
    }
}
```

