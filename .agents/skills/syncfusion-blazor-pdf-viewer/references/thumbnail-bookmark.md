# General Methods in Blazor SfPdfViewer Component

## Table of Contents
- [Overview](#overview)
- [When to Use General Methods](#when-to-use-general-methods)
- [Common Scenarios](#common-scenarios)
- [Implementation Examples](#implementation-examples)
- [Method Reference](#method-reference)
- [Supporting Types](#supporting-types)

## Overview

When the user needs to implement document navigation, text selection management, undo/redo functionality, or form field interactions in the Blazor PDF Viewer, guide them to use these general utility methods. These methods provide programmatic control over viewer state and user interactions beyond basic document loading.

## Properties

### EnableBookmarkPanel
If set to false, the bookmark panel on the left side of the PDF Viewer will be disabled. By default it is true.
- `false` - Disabled the bookmark panel from PDF Viewer.
- `true` - Enabled the bookmark panel from PDF Viewer.

#### Data Type and default value
`bool` - `true(Default)`

#### Code Example
```razor
<SfPdfViewer2 EnableBookmarkPanel="true"></SfPdfViewer2>
```

### EnableThumbnailPanel
If set to false, the thumbnail panel on the left side of the PDF Viewer will be disabled. By default it is true. 
- `false` - Disabled the thumbnail panel from PDF Viewer.
- `true` - Enabled the thumbnail panel from PDF Viewer.

#### Data Type and default value
`bool` - `true(Default)`

#### Code Example
```razor
<SfPdfViewer2 EnableThumbnailPanel="true"></SfPdfViewer2>
```

### IsThumbnailPanelOpen
Indicates whether the thumbnail panel is in expanded or collapsed state.

#### Data Type
`bool`

#### Code Example
```razor
<SfPdfViewer2 IsThumbnailPanelOpen="isThumbnailPanelOpen">
</SfPdfViewer2>
@code {
    private bool isThumbnailPanelOpen;
}
```

## Methods

### GetBookmarksAsync()
Retrieves all bookmarks from the PDF document asynchronously. Returns a list of bookmark objects containing navigation information.

#### Parameters
No Parameters

#### Return Type
`Task<List<Bookmark>>`

- [Bookmark](./general-enum-properties.md#bookmark)

#### Code Example
```csharp
    // We can bind the method for click action based the user requirement.
    List<Bookmark> bookMarks = await viewer.GetBookmarksAsync();
```

### GoToBookmarkAsync(int, double)
Navigates to a specific bookmark location in the PDF document. Jumps to specified page and vertical position.

#### Parameters
- `int` - pageNumber - Represents the bookmark pageindex of the loaded PDF document.
- `double` - y - Represents the Y co-ordinate of the bookmark page of the loaded PDF document.

#### Return Type
`Task`

#### Code Example
```csharp
    // We can bind the method for click action based the user requirement.
    await viewer.GetBookmarksAsync(5, 100);
```

### OpenThumbnailPaneAsync()
Opens the thumbnail panel on the left side of the PDF Viewer asynchronously. 

#### Parameters
No Parameters

#### Return Type
`Task`

#### Code Example
```csharp
    // We can bind the method for click action based the user requirement.
    await viewer.OpenThumbnailPaneAsync();
```

## Events

### OnThumbnailClick
Triggers when a thumbnail is clicked in the thumbnail panel of the SfPdfViewer.|****|**PageNumber** - (int) - Gets the Page number of the thumbnail in which click action is performed.|

#### Value
`EventCallback<ThumbnailClickEventArgs>`

#### OnThumbnailClick Events Arguments
| Event Arguments | Data Type | Description |
|-----|-----|-----|
| **PageNumber** | int | Gets the Page number of the thumbnail in which click action is performed. |

#### Code Example
```razor
<SfPdfViewer2>
    <PdfViewerEvents OnThumbnailClick="OnThumbnailClick"></PdfViewerEvents>
</SfPdfViewer2>
@code {
    private void OnThumbnailClick(ThumbnailClickEventArgs args) {
        // customize your code in based on the requirements and check above table for event argument details
    }
}
```
