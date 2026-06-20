# Navigation Methods in Blazor SfPdfViewer Component

## Table of Contents
- [Overview](#overview)
- [Methods](#methods)
- [Events](#events)

## Overview

Navigation methods in `SfPdfViewer2` provide programmatic control over page navigation and document information retrieval. These methods enable developers to build custom navigation UI, implement keyboard shortcuts, create guided workflows, and access page metadata without relying on the default toolbar.

## Methods

### GoToFirstPageAsync()
Navigate to first page of the PDF document loaded in the PDF Viewer.

#### Parameters
No Parameters

#### Return Type
- `Task`

#### Code Example
```csharp
    // We can bind the method for click action based the user requirement.
    await viewer.GoToFirstPageAsync();
```

### GoToLastPageAsync()
Navigate to last page of the PDF document loaded in the PDF Viewer.

#### Parameters
No Parameters

#### Return Type
- `Task`

#### Code Example
```csharp
    // We can bind the method for click action based the user requirement.
    await viewer.GoToLastPageAsync();
```

### GoToNextPageAsync()
Navigate to Next page of the PDF document loaded in the PDF Viewer.

#### Parameters
No Parameters

#### Return Type
- `Task`

#### Code Example
```csharp
    // We can bind the method for click action based the user requirement.
    await viewer.GoToNextPageAsync();
```

### GoToPreviousPageAsync()
Navigate to Previous page of the PDF document.

#### Parameters
No Parameters

#### Return Type
- `Task`

#### Code Example
```csharp
    // We can bind the method for click action based the user requirement.
    await viewer.GoToPreviousPageAsync();
```

### GoToPageAsync(int)
Navigate to given page number in loaded document of the PDF Viewer.

#### Parameters
- `int` - pageNumber

#### Return Type
- `Task`

#### Code Example
```csharp
    // We can bind the method for click action based the user requirement.
    await viewer.GoToPageAsync(4);
```

### GetPageCountAsync()
Returns the page count of the document loaded in the PdfViewer control.

#### Parameters
- `int`

#### Return Type
- `Task<int>`

#### Code Example
```csharp
    // We can bind the method for click action based the user requirement.
    int PageCount = await viewer.GetPageCountAsync();
```

### GetPageDetails()
Retrieves details of all pages in the PDF document.

#### Parameters
No Parameters

#### Return Type
- List<[PageInfo](./annotation-settings-enum-properties.md#pageinfo-class)>

#### Code Example
```csharp
    // We can bind the method for click action based the user requirement.
    List<PageInfo> PageDetails = viewer.GetPageDetails();
```

### ShowNavigationToolbarAsync(bool)
Shows or hides the Navigation toolbar in the PDF Viewer.

#### Parameters
- `bool` - whether enableNavigationToolbar or not.

#### Return Type
- `Task`

#### Code Example
```csharp
    // We can bind the method for click action based the user requirement.
    await viewer.ShowNavigationToolbarAsync(true);
```

### ToggleItemByIndex(int)
Toggles the visibility of the custom panel associated with the specified index in the navigation toolbar.

#### Parameters
- `int` - The zero-based index of the custom toolbar item whose corresponding panel should be toggled.

#### Return Type
- `Task`

#### Code Example
```csharp
    // We can bind the method for click action based the user requirement.
    await viewer.ToggleItemByIndex(4);
```

## Events

### PageChanged
Triggers when there is change in current page number.

#### Value
`EventCallback<PageChangeEventArgs>`

#### PageChanged Events Arguments
| Event Arguments | Data Type | Description |
|-----|-----|-----|
| `CurrentPageNumber` | int | Gets the current page number of the document. |
| `DocumentName` | string | Gets the Document name to be loaded into PdfViewer. |
| `PreviousPageNumber` | int | Gets the previous page number of the document. |

#### Code Example
```razor
<SfPdfViewer2>
    <PdfViewerEvents PageChanged="PageChanged"></PdfViewerEvents>
</SfPdfViewer2>
@code {
    private void PageChanged(PageChangeEventArgs args) {
        // customize your code in based on the requirements and check above table for event argument details
    }
}
```


