# Syncfusion Blazor PDF Viewer - File Save & Export Properties

## Table of Contents
- [Overview](#overview)
- [Properties](#properties)
- [Methods](#methods)
- [Events](#events)

## Overview

The Syncfusion Blazor PDF Viewer provides comprehensive file save and export functionality that allows users to download PDF documents, export annotations, and import previously saved annotations. These methods and events enable complete document management workflows including download tracking, annotation export/import operations, and error handling. The file save properties give developers full control over document persistence and annotation management.

## Properties

### EnableDownload
If set to false, the download option in the toolbar will be disabled. By default it is true.
- `false` - Disabled the download functionalities from the PDF Viewer.
- `true` - Enabled the download functionalities from the PDF Viewer.

#### Data Type and default value
`bool` - `true(Default)`

#### Code Example
```razor
<SfPdfViewer2 EnableDownload="true"></SfPdfViewer2>
```

## Methods

### DownloadAsync()
Downloads the PDF document being loaded in the PDF Viewer to the user's browser. Initiates a file download with the current document state.

#### Parameters
No Parameters

#### Return Type
- `Task`

#### Code Example
```csharp
    // We can bind the method for click action based the user requirement.
    await viewer.DownloadAsync();
```

## Events

### DownloadStart
Triggers an event when the download action is started.

#### Value
`EventCallback<DownloadStartEventArgs>`

#### DownloadStart Events Arguments
| Event Arguments | Data Type | Description |
|-----|-----|-----|
| `FileName` | string | Gets the File name of the currently loaded PDF document in the PDF Viewer. |

#### Code Example
```razor
<SfPdfViewer2>
    <PdfViewerEvents DownloadStart="DownloadStarted"></PdfViewerEvents>
</SfPdfViewer2>
@code {
    private void DownloadStarted(DownloadStartEventArgs args) {
        // customize your code in based on the requirements and check above table for event argument details
    }
}
```

### DownloadEnd
Triggers an event when the download actions is finished.

#### Value
`EventCallback<DownloadEndEventArgs>`

#### DownloadStart Events Arguments
| Event Arguments | Data Type | Description |
|-----|-----|-----|
| `DownloadDocument` | string | Defines the base 64 string of the loaded PDF document data. |
| `FileName` | string | File name of the currently loaded PDF document in the PDF Viewer. |

#### Code Example
```razor
<SfPdfViewer2>
    <PdfViewerEvents DownloadEnd="DownloadEnded"></PdfViewerEvents>
</SfPdfViewer2>
@code {
    private void DownloadEnded(DownloadEndEventArgs args) {
        // customize your code in based on the requirements and check above table for event argument details
    }
}
```
