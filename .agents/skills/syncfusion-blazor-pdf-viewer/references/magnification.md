# Magnification in Blazor SfPdfViewer Component

## Table of Contents
- [Overview](#overview)
- [Properties](#properties)
- [Methods](#methods)
- [Events](#events)

## Overview

When the user needs to control document zoom levels, implement fit-to-page behaviors, or optimize rendering performance during magnification, guide them to use the PDF Viewer's magnification features. These include programmatic zoom methods, zoom constraints, automatic fit modes for improved performance.

## Properties

### EnableMagnification
If it is set as false, then all the zoom operations in the PDF Viewer will be disabled. By default it is true.
- `false` - Disabled the zoom operations in the PDF Viewer.
- `true` - Enabled the zoom operations in the PDF Viewer.

#### Data Type and default value
`bool` - `true(Default)`

#### Code Example
```razor
<SfPdfViewer2 EnableMagnification="true"></SfPdfViewer2>
```

### EnablePinchZoom
If it is set as false, then pinch zoom of the PDF Viewer will be disabled. By default it is true.
- `false` - Disabled the pinch zoom operations in the PDF Viewer.
- `true` - Enabled the pinch zoom operations in the PDF Viewer.

#### Data Type and default value
`bool` - `true(Default)`

#### Code Example
```razor
<SfPdfViewer2 EnablePinchZoom="true"></SfPdfViewer2>
```

### EnableZoomOptimization
If it is set as false, then page image will be fetched for each zoom percentage values. Otherwise, page image will be fetched only when the zoom percentages are 100 %,200 %,300 %, and 400 %. By default it is true.
- `false` - Disabled the zoom percentage changes in the PDF Viewer.
- `true` - Enabled the zoom percentage changes in the PDF Viewer.

#### Data Type and default value
`bool` - `true(Default)`

#### Code Example
```razor
<SfPdfViewer2 EnableZoomOptimization="true"></SfPdfViewer2>
```

### ZoomMode
Indicates whether the PDF page will be rendered in fit to page, fit to width, or based on the zoom percentage value (default). By default, the value is Default, representing page rendering based on zoom percentage.

#### Data Type and default value
[ZoomMode](./general-enum-properties.md#zoommode)  - `ZoomMode.Default(Default)`

#### Code Example
```razor
<SfPdfViewer2 ZoomMode="ZoomMode.FitToHeight"></SfPdfViewer2>
```

### ZoomValue
Gets or sets the Zoom Percentage value of the PDF Viewer. Range varies from 10 to 400.

#### Data Type
`int`

#### Code Example
```razor
<SfPdfViewer2 ZoomValue="125"></SfPdfViewer2>
```

### ZoomValueChanged
Gets the event callback for ZoomValue. Triggered when the zoom value changes.

#### Data Type
`EventCallback<int>`

#### Code Example
```razor
<SfPdfViewer2 ZoomValueChanged="ZoomValueChanged"></SfPdfViewer2>
@code {
    private void ZoomValueChanged() {
        // Methods implementations can be modified based on users.
    }
}
```

### RestrictZoomRequest
Gets or sets a value indicating whether a single image rendered at 100% zoom should be reused across all zoom levels.

#### Data Type
`int`

#### Code Example
```razor
<SfPdfViewer2 RestrictZoomRequest="true"></SfPdfViewer2>
```

#### Remarks
The RestrictZoomRequest property helps optimize zoom performance in the PDF viewer by reducing the frequency of image regeneration. When enabled (true), the viewer generates a single image at 100% zoom and reuses it for all subsequent zoom operations. This minimizes client-side processing and enhances performance, particularly on low-powered or resource-constrained devices. However, it may slightly reduce image clarity at non-100% zoom levels. When disabled (false, default), page images are regenerated at each zoom level to ensure sharper rendering during zoom interactions.

### MaxZoomValue
Gets or sets the Maximum Zoom value of the PDF Viewer. Default value is 400.

#### Data Type and default value
`int` - `400(Default)`

#### Code Example
```razor
<SfPdfViewer2 MaxZoomValue="250"></SfPdfViewer2>
```

### MinZoomValue
Gets or sets the Minimum Zoom value of the PDF Viewer. Default value is 10.

#### Data Type and default value
`int` - `10(Default)`

#### Code Example
```razor
<SfPdfViewer2 MinZoomValue="50"></SfPdfViewer2>
```

## Methods

### ZoomInAsync()
Scale the page to the next value in the zoom drop down list. Drop down values are 10, 25, 50, 75, 100, 125, 150, 200, 400. | Method

#### Parameters
No Parameters

#### Return Type
- `Task`

#### Code Example
```csharp
    // We can bind the method for click action based the user requirement.
    await viewer.ZoomInAsync();
```

### ZoomOutAsync()
Magnifies the page to the previous value in the zoom drop down list. Drop down values are 10, 25, 50, 75, 100, 125, 150, 200, 400.

#### Parameters
No Parameters

#### Return Type
- `Task`

#### Code Example
```csharp
    // We can bind the method for click action based the user requirement.
    await viewer.ZoomOutAsync();
```

### ZoomAsync(int)
Zoom the PDF document to the given zoom value. Range varies from 10 to 400.

#### Parameters
- `int`

#### Return Type
- `Task`

#### Code Example
```csharp
    // We can bind the method for click action based the user requirement.
    await viewer.ZoomAsync(125);
```

### FitToPageAsync()
Scales the page to fit within the container of the PDF Viewer.

#### Parameters
No Parameters

#### Return Type
- `Task`

#### Code Example
```csharp
    // We can bind the method for click action based the user requirement.
    await viewer.FitToPageAsync();
```

### FitToWidthAsync()
Scales the page to fit the page width within container width of the PDF Viewer. | Method | No parameters - Returns `Task` |

#### Parameters
No Parameters

#### Return Type
- `Task`

#### Code Example
```csharp
    // We can bind the method for click action based the user requirement.
    await viewer.FitToWidthAsync();
```

### FitToHeightAsync()
Scales the page to fit the page width within container height of the PDF Viewer.

#### Parameters
No Parameters

#### Return Type
- `Task`

#### Code Example
```csharp
    // We can bind the method for click action based the user requirement.
    await viewer.FitToHeightAsync();
```

### GetZoomPercentageAsync()
Returns the current zoom percentage of the PdfViewer control. | Method |  |

#### Parameters
No Parameters

#### Return Type
- `Task<int>`

#### Code Example
```csharp
    // We can bind the method for click action based the user requirement.
    int ZoomValue = await viewer.GetZoomPercentageAsync();
```

## Events

### ZoomChanged
Triggers when the magnification value changes.

#### Value
`EventCallback<ZoomChangeEventArgs>`

#### ZoomChanged Events Arguments
| Event Arguments | Data Type | Description |
|-----|-----|-----|
| `CurrentZoomValue` | int | Gets the current zoom percentage value. |
| `PreviousZoomValue` | int | Gets the zoom value before change. |

#### Code Example
```razor
<SfPdfViewer2>
    <PdfViewerEvents ZoomChanged="ZoomChanged"></PdfViewerEvents>
</SfPdfViewer2>
@code {
    private void ZoomChanged(ZoomChangeEventArgs args) {
        // customize your code in based on the requirements and check above table for event argument details
    }
}
```
