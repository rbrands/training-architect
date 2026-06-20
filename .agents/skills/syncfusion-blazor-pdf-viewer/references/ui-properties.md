# Syncfusion Blazor PDF Viewer - UI Properties Reference

## Table of Contents
- [Overview](#overview)
- [Properties](#properties)
- [Events](#events)

## Overview
The Syncfusion Blazor PDF Viewer component provides a comprehensive set of properties and events to customize the viewing and interaction experience. This document outlines the key properties, inherited APIs, and events available for the PDF Viewer component.

## Properties

### EnableDesktopMode
Gets or sets a boolean value to show or hide desktop toolbar in mobile devices.

#### Data Type and default value
`bool` - `false(Default)`

#### Code Example
```razor
<SfPdfViewer2 EnableDesktopMode="false"></SfPdfViewer2>
```

### EnableErrorDialog
If set to false, error dialogue display in PDF Viewer will be disabled. By default it is true. 

#### Data Type and default value
`bool` - `true(Default)`

#### Code Example
```razor
<SfPdfViewer2 EnableErrorDialog="true"></SfPdfViewer2>
```

### EnableHyperlink
If set to false, hyperlink in the PDF Viewer will be disabled. By default it is true.

#### Data Type and default value
`bool` - `true(Default)`

#### Code Example
```razor
<SfPdfViewer2 EnableHyperlink="true"></SfPdfViewer2>
```

### EnableRtl
If set to true, PDF Viewer will render in right to left direction. By default it renders in left to right direction.

#### Data Type and default value
`bool` - `false(Default)`

#### Code Example
```razor
<SfPdfViewer2 EnableRtl="true"></SfPdfViewer2>
```

## Events

### OnHyperlinkClick
Triggers when a hyperlink in the PDF document is clicked.

#### Value
`EventCallback<HyperlinkClickEventArgs>`

#### OnHyperlinkClick Events Arguments
| Event Arguments | Data Type | Description |
|-----|-----|-----|
| `Cancel` | bool | Hyperlink navigation will not work if it is set to TRUE. The value is set to FALSE by default. |
| `Hyperlink` | string | Gets the current clicked hyperlink. |
| `HyperlinkElement` | DOM | Gets or set the URL to navigate. |

#### Code Example
```razor
<SfPdfViewer2>
    <PdfViewerEvents OnHyperlinkClick="OnHyperlinkClick"></PdfViewerEvents>
</SfPdfViewer2>
@code {
    private void OnHyperlinkClick(HyperlinkClickEventArgs args) {
        // customize your code in based on the requirements and check above table for event argument details 
    }
}
```

### OnHyperlinkMouseOver
Triggers when a hyperlink in the PDF document is hovered. |  | **HyperlinkElement** - (DOM) - . |

#### Value
`EventCallback<HyperlinkMouseOverArgs>`

#### OnHyperlinkMouseOver Events Arguments
| Event Arguments | Data Type | Description |
|-----|-----|-----|
| `HyperlinkElement` | DOM | Gets the current hyperlink element |

#### Code Example
```razor
<SfPdfViewer2>
    <PdfViewerEvents OnHyperlinkMouseOver="OnHyperlinkMouseOver"></PdfViewerEvents>
</SfPdfViewer2>
@code {
    private void OnHyperlinkMouseOver(HyperlinkMouseOverArgs args) {
        // customize your code in based on the requirements and check above table for event argument details 
    }
}
```

### Created
Triggers when the PDF Viewer component is created.

#### Value
`EventCallback<object>`

#### OnHyperlinkMouseOver Events Arguments
No Arguments

#### Code Example
```razor
<SfPdfViewer2>
    <PdfViewerEvents Created="Created"></PdfViewerEvents>
</SfPdfViewer2>
@code {
    private void Created() {
        // customize your code in based on the requirements and check above table for event argument details 
    }
}
```
