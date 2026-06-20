# Page Organizer (Properties & Methods)

## Table of Contents
- [Overview](#overview)
- [Properties](#properties)
- [Methods](#methods)
- [Events](#events)

## Overview

The Page Organizer lets you programmatically manage PDF pages: delete, duplicate, extract, insert, move, and rotate. Use it when building document editing tools, batch operations, or automation workflows that modify PDF structure.

## Properties

### EnablePageOrganizer
Gets or sets a value indicating whether the page organizer feature is enabled in the PDF Viewer.
- `false` - Disabled the page organizer functionlities in the PDF Viewer.
- `true` - Enabled the page organizer functionlities in the PDF Viewer.

#### Data Type and default value
`bool` - `true(Default)`

#### Code Example
```razor
<SfPdfViewer2 EnablePageOrganizer="true"></SfPdfViewer2>
```

### PageOrganizerVisibility
Gets or sets a value indicating whether the Page Organizer dialog is visible in the PDF Viewer.

#### Data Type
`bool`

#### Code Example
```razor
<SfPdfViewer2 PageOrganizerVisibility="PageOrganizerVisibility"></SfPdfViewer2>

@code {
    bool PageOrganizerVisibility;
}
```

### PageOrganizerVisibilityChanged
Gets or sets the callback that is triggered when the Page Organizer dialog visibility changes in the PDF Viewer.

#### Data Type
`EventCallback<bool>`

#### Code Example
```razor
<SfPdfViewer2 PageOrganizerVisibilityChanged="PageOrganizerVisibilityChanged"></SfPdfViewer2>

@code {
    private void PageOrganizerVisibilityChanged() {
        // Method implementation based on the users requirement.
    }
}
```

## Methods

### DeletePagesAsync(int[])
Asynchronously deletes the pages at the specified indices from the document. Changes are applied client-side and persisted on save.

#### Parameters
- `int[]` - `pageIndices` - (int[]) 0-based page indices to delete.

#### Return Type
- `Task`

#### Code Example
```csharp
    // We can bind the method for click action based the user requirement.
    await viewer.DeletePagesAsync(new int[] { 5, 6 });
```

### DuplicatePagesAsync(int[])
Asynchronously duplicates the pages at the specified indices. Duplicates are inserted immediately after each original page.

#### Parameters
- `int[]` - `pageIndices` - (int[]) 0-based page indices to duplicate.

#### Return Type
- `Task`

#### Code Example
```csharp
    // We can bind the method for click action based the user requirement.
    await viewer.DuplicatePagesAsync(new int[] { 5, 6 });
```

### ExtractPagesAsStreamAsync(int[])
Extracts the specified pages and returns a `Stream` containing a new PDF with only those pages.

#### Parameters
- `int[]` - `pageIndices` - (int[]) 0-based page indices to extract.

#### Return Type
- `Task<Stream>`

#### Code Example
```csharp
    // We can bind the method for click action based the user requirement.
    Stream stream = await viewer.ExtractPagesAsStreamAsync(new int[] { 5, 6 });
```

### ExtractPagesAsync(int[])
Extracts the specified pages and initiates download of a new PDF containing only those pages.

#### Parameters
- `int[]` - `pageIndices` - (int[]) 0-based page indices to extract.

#### Return Type
- `Task`

#### Code Example
```csharp
    // We can bind the method for click action based the user requirement.
    await viewer.ExtractPagesAsync(new int[] { 5, 6 });
```

### InsertBlankPagesAsync(int, int)
Inserts one or more blank pages at the specified index.

#### Parameters
- `int` - `insertAtIndex` - (int) 0-based index at which to insert pages (use `PageCount` to append).
- `int` - `count` - (int) number of blank pages to insert (>=1)..

#### Return Type
- `Task`

#### Code Example
```csharp
    // We can bind the method for click action based the user requirement.
    await viewer.InsertBlankPagesAsync(2, 6);
```

### InsertPagesAsync(int, byte[], string)
Inserts all pages from the provided PDF byte array at the specified index.

#### Parameters
- `int` - `insertAtIndex` - (int) 0-based insertion index.
- `byte[]` - `documentBytes` - (byte[]) source PDF bytes.
- `string` - `password` - (string) optional password for source PDF. 

#### Return Type
- `Task`

#### Code Example
```csharp
    // We can bind the method for click action based the user requirement.
    await viewer.InsertPagesAsync(3, documentBytes);
```

### InsertPagesAsync(int, Stream, string)
Inserts all pages from the provided PDF `Stream` at the specified index.

#### Parameters
- `int` - `insertAtIndex` - (int) 0-based insertion index.
- `stream` - `documentStream` - (`Stream`) readable stream of source PDF.
- `string` - `password` - (string) optional password for source PDF. 

#### Return Type
- `Task`

#### Code Example
```csharp
    // We can bind the method for click action based the user requirement.
    await viewer.InsertPagesAsync(3, documentStream);
```

### MovePagesAsync(int[], int)
Moves/rearranges the specified pages to start at the given target index. The order of `pageIndices` is preserved in the final arrangement.

#### Parameters
- `int[]` - `pageIndices` - (int[]) 0-based indices of pages to move (unique, within page count).
- `int` - `targetIndex` - (int) 0-based destination index where pages will be placed. 

#### Return Type
- `Task`

#### Code Example
```csharp
    // We can bind the method for click action based the user requirement.
    await viewer.MovePagesAsync(new int[] { 0, 2 }, 4);
```

### RotatePagesAsync(int[], RotationDirection)
Rotates the specified pages in the given `RotationDirection` (Clockwise/CounterClockwise).

#### Parameters
- `int[]` - `pageIndices` - (int[]) 0-based indices to rotate.
- [RotationDirection](./general-enum-properties.md#rotationdirection) - `direction` - (`RotationDirection`) enum: `Clockwise` or `CounterClockwise`.

#### Return Type
- `Task`

#### Code Example
```csharp
    // We can bind the method for click action based the user requirement.
    await viewer.RotatePagesAsync(new int[] { 0, 1 }, RotationDirection.Clockwise);
```

### RotatePagesToAngleAsync(int[], PageRotation)
Rotates the specified pages to a fixed `PageRotation` angle (e.g., 0, 90, 180, 270).

#### Parameters
- `int[]` - `pageIndices` - (int[]) 0-based indices to rotate.
- [PageRotation](./annotation-settings-enum-properties.md#pagerotation) - `angle` - (`PageRotation`) enum: `RotateAngle0`, `RotateAngle90`, `RotateAngle180`, `RotateAngle270`.

#### Return Type
- `Task`

#### Code Example
```csharp
    // We can bind the method for click action based the user requirement.
    await viewer.RotatePagesToAngleAsync(new int[] { 0 }, PageRotation.RotateAngle90);
```

## Events

### PageOrganizerSaveRequested
Provides event data for the save action triggered in the SfPdfViewer2 Page Organizer dialog.

#### Value
`EventCallback<PageOrganizerSaveEventArgs>`

#### PageOrganizerSaveRequested Events Arguments
| Event Arguments | Data Type | Description |
|-----|-----|-----|
| `Cancel` | bool | Gets or sets a value indicating whether the save operation should be canceled in the SfPdfViewer2 Page Organizer dialog. |
| `DownloadDocument` | Stream | Gets the stream representing the document to be downloaded in the SfPdfViewer2 Page Organizer dialog. |
| `FileName` | string | Gets the file name used when saving the document in the SfPdfViewer2 Page Organizer dialog. |

#### Code Example
```razor
<SfPdfViewer2>
    <PdfViewerEvents PageOrganizerSaveRequested="PageOrganizerSaveRequested"></PdfViewerEvents>
</SfPdfViewer2>
@code {
    private void PageOrganizerSaveRequested(PageOrganizerSaveEventArgs args) {
        // customize your code in based on the requirements and check above table for event argument details
    }
}
```

### PageOrganizerZoomChanged
Triggered when the zoom level changes in the SfPdfViewer2 Page Organizer view.

#### Value
`EventCallback<PageOrganizerZoomChangedEventArgs>`

#### PageOrganizerZoomChanged Events Arguments
| Event Arguments | Data Type | Description |
|-----|-----|-----|
| `CurrentZoom` | double | Gets or sets the current zoom level after the change in the SfPdfViewer2 Page Organizer view. |
| `PreviousZoom` | double | Gets or sets the previous zoom level before the change in the SfPdfViewer2 Page Organizer view. |

#### Code Example
```razor
<SfPdfViewer2>
    <PdfViewerEvents PageOrganizerZoomChanged="PageOrganizerZoomChanged"></PdfViewerEvents>
</SfPdfViewer2>
@code {
    private void PageOrganizerZoomChanged(PageOrganizerZoomChangedEventArgs args) {
        // customize your code in based on the requirements and check above table for event argument details
    }
}
```

### PageSelected
Triggered when one or more pages are selected in the Page Organizer view.

#### Value
`EventCallback<PageSelectEventArgs>`

#### PageSelected Events Arguments
| Event Arguments | Data Type | Description |
|-----|-----|-----|
| `SelectedPages` | List<[PageInfo](./annotation-settings-enum-properties.md#pageinfo-class)> | Gets or sets the list of selected pages in the SfPdfViewer2 Page Organizer view.

#### Code Example
```razor
<SfPdfViewer2>
    <PdfViewerEvents PageSelected="PageSelected"></PdfViewerEvents>
</SfPdfViewer2>
@code {
    private void PageSelected(PageSelectEventArgs args) {
        // customize your code in based on the requirements and check above table for event argument details
    }
}
```
