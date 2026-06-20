# Print in Blazor SfPdfViewer Component

## Table of Contents
- [Overview](#overview)
- [Properties](#properties)
- [Methods](#methods)
- [Events](#events)

## Overview

The PDF Viewer provides comprehensive print capabilities with quality control, rotation handling, and event hooks. Use these features to give users control over document printing while maintaining quality and handling edge cases like landscape documents.

## Properties

### EnablePrint
If it is set as false, then the print option in the toolbar of the PDF Viewer will be disabled.
- `false` - Disabled the print functionalities in the pdfviewer.
- `true` - Enabled the print functionalities in the pdfviewer.

#### Data Type and default value
`bool` - `true(Default)`

#### Code Example
```razor
<SfPdfViewer2 EnablePrint="true"></SfPdfViewer2>
```

### EnablePrintRotation
If it is set as FALSE, will suppress the page rotation of Landscape document on print action.
- `false` - Disabled the page rotation of Landscape document in the pdfviewer.
- `true` - Enabled the page rotation of Landscape document in the pdfviewer.

#### Data Type and default value
`bool` - `true(Default)`

#### Code Example
```razor
<SfPdfViewer2 EnablePrint="true"></SfPdfViewer2>
```

### PrintMode
Specifies the print mode in the PDF Viewer. | PrintMode | Default |

#### Data Type and default value
[PrintMode](./general-enum-properties.md#printmode) - `PrintMode.Default(Default)`

#### Code Example
```razor
<SfPdfViewer2 PrintMode="PrintMode.NewWindow"></SfPdfViewer2>
```

### PrintScaleFactor
Specifies the document printing quality. The default printing quality is set to 1.0.

#### Data Type and default value
`double` - `1.0(Default)`

#### Code Example
```razor
<SfPdfViewer2 PrintScaleFactor="2"></SfPdfViewer2>
```

## Methods

### PrintAsync()
Print the PDF document being loaded in the PDF Viewer.

#### Parameters
No Parameters

#### Return Type
`Task`

#### Code Example
```csharp
    // We can bind the method for click action based the user requirement.
    await viewer.PrintAsync();
```

## Events

### PrintStart
Event triggered when the print action starts

#### Value
`EventCallback<PrintStartEventArgs>`

#### PrintStart Events Arguments
| Event Arguments | Data Type | Description |
|-----|-----|-----|
| `Cancel` | bool | If it is true then the print operation will not work. |
| `FileName` | string | File name of the currently loaded PDF document in the PDF Viewer. |

#### Code Example
```razor
<SfPdfViewer2>
    <PdfViewerEvents PrintStart="PrintStarted"></PdfViewerEvents>
</SfPdfViewer2>
@code {
    private void PrintStarted(PrintStartEventArgs args) {
        // customize your code in based on the requirements and check above table for event argument details
    }
}
```

### PrintEnd
Event triggered when the print action ends.

#### Value
`EventCallback<PrintEndEventArgs>`

#### PrintEnd Events Arguments
| Event Arguments | Data Type | Description |
|-----|-----|-----|
| `FileName` | string | File name of the currently loaded PDF document in the PDF Viewer. |

#### Code Example
```razor
<SfPdfViewer2>
    <PdfViewerEvents PrintEnd="PrintEnded"></PdfViewerEvents>
</SfPdfViewer2>
@code {
    private void PrintEnded(PrintEndEventArgs args) {
        // customize your code in based on the requirements and check above table for event argument details
    }
}
```

