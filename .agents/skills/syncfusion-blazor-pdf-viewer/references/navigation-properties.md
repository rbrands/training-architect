# Navigation in Blazor SfPdfViewer Component

## Table of Contents
- [Overview](#overview)
- [Properties](#properties)

## Overview

Navigation properties in `SfPdfViewer2` provide state management for page tracking, event callbacks for UI synchronization. These properties enable developers to track document position, react to page changes and optimize scrolling performance.

## Properties

### EnableNavigation
Gets or sets whether page navigation is enabled in the PDF Viewer.
- `false` - Disabled the page navigation interactions in the PDF Viewer.
- `true` - Enabled the page navigation interactions in the PDF Viewer.

#### Data Type and default value
`bool` - `true(Default)`

#### Code Example
```razor
<SfPdfViewer2 EnableNavigation="true"></SfPdfViewer2>
```

### CurrentPageNumber
Gets or sets the current page number of the PDF Viewer (1-based indexing).

#### Data Type and default value
`int` - `1(Default)`

#### Code Example
```razor
<SfPdfViewer2 CurrentPageNumber="CurrentPageNumber"></SfPdfViewer2>
@code {
    private int CurrentPageNumber;
}
```

### CurrentPageNumberChanged
Event callback that fires when the current page number changes 

#### Data Type
`EventCallback<int>`

#### Code Example
```razor
<SfPdfViewer2 CurrentPageNumberChanged="CurrentPageNumberChanged"></SfPdfViewer2>
@code {
    private void CurrentPageNumberChanged() {
        // Method implementation based on the user query
    }
}
```

### PageCount
Gets the total page count of the loaded PDF document (read-only).

#### Data Type and default value
`int` - `0(Default)`

#### Code Example
```razor
<SfPdfViewer2 PageCount="PageCount"></SfPdfViewer2>
@code {
    private int PageCount;
}
```

### PageCountChanged
Event callback that fires when a new document is loaded and page count changes.

#### Data Type
`EventCallback<int>`

#### Code Example
```razor
<SfPdfViewer2 PageCountChanged="PageCountChanged"></SfPdfViewer2>
@code {
    private void PageCountChanged() {
        // Method implementation based on the user query
    }
}
```
