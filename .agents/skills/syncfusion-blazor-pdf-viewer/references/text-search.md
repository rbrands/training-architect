# Text Search in Blazor SfPdfViewer Component

## Table of Contents
- [Overview](#overview)
- [Properties](#properties)
- [Methods](#methods)
- [Events](#events)

## Overview

Guide users to implement text search when they need to find and navigate through specific content within PDF documents. Text search functionality helps users locate terms, phrases, or data points across single or multi-page PDFs with visual highlighting and navigation controls.

## properties

### EnableTextSearch
If it is set as false, then text search support in the PDF Viewer will be disabled. By default it is true.
- `false` - Disabled the search functionalities from PDF Viewer.
- `true` - Enabled the search functionalities from PDF Viewer.

#### Data Type and default value
`bool` - `true(Default)`

#### Code Example
```razor
<SfPdfViewer2 EnableTextSearch="true"></SfPdfViewer2>
```

### PdfViewerTextSearchColorSettings
Configure the search text and highlight text color for the PDF Viewer.

#### Properties List

| **Name** | **Description** | **Data type** |
|-----|-----|-----|
| **SearchColor** | The color to apply to found search text. Format: hex color (#RRGGBB) or named color. Default: #8b4c12 | string |
| **SearchHighlightColor** | The color to apply to highlighted text during search. Format: hex color (#RRGGBB) or named color. Default: #fdd835 | string |

#### Code Example
```razor
<SfPdfViewer2>
    <PdfViewerTextSearchColorSettings SearchColor="green" SearchHighlightColor="orange"></PdfViewerTextSearchColorSettings>
</SfPdfViewer2>
```

## Methods

### SearchTextAsync(string, bool)
Searches the target text in the PDF document and highlights the occurrences in the PDF pages.

#### Parameters
- `string` - search word that used to start search
- `bool` - is the search with match case or not

#### Return Type
`Task`

#### Code Example
```csharp
    // We can bind the method for click action based the user requirement.
    // example demonstrates how to search word
    await viewer.SearchTextAsync("word", false);
```

### SearchNextAsync()
Searches the next occurrence of the searched text from the current occurrence of the PDF Viewer.

#### Parameters
No Paramters

#### Return Type
`Task`

#### Code Example
```csharp
    // We can bind the method for click action based the user requirement.
    // example demonstrates how to make next search
    await viewer.SearchNextAsync();
```

### SearchPreviousAsync()
Searches the previous occurrence of the searched text from the current occurrence of the PDF Viewer.

#### Parameters
No Paramters

#### Return Type
`Task`

#### Code Example
```csharp
    // We can bind the method for click action based the user requirement.
    // example demonstrates how to make previous search
    await viewer.SearchPreviousAsync();
```

### CancelTextSearchAsync()
Cancels the text search of the PDF Viewer.

#### Parameters
No Paramters

#### Return Type
`Task`

#### Code Example
```csharp
    // We can bind the method for click action based the user requirement.
    // example demonstrates how to make previous search
    await viewer.CancelTextSearchAsync();
```

## Events

### OnTextSearchStart
Event triggered when text search begins in the PDF Viewer. Receives TextSearchStartEventArgs.

#### Value
`EventCallback<TextSearchStartEventArgs>`

#### OnTextSearchStart Events Arguments
| Event Arguments | Data Type | Description |
|-----|-----|-----|
| **SearchText** | string | The text that was searched for in the document. |
| **MatchCase** | bool | Indicates whether the search is case-sensitive. |

#### Code Example
```razor
<SfPdfViewer2>
    <PdfViewerEvents OnTextSearchStart="OnTextSearchStart"></PdfViewerEvents>
</SfPdfViewer2>
@code {
    private void OnTextSearchStart(TextSearchStartEventArgs args) {
        // customize your code in based on the requirements and check above table for event argument details
    }
}
```

### OnTextSearchComplete
Event triggered when text search is completed. Receives TextSearchCompleteEventArgs.

#### Value
`EventCallback<TextSearchCompleteEventArgs>`

#### OnTextSearchComplete Events Arguments
| Event Arguments | Data Type | Description |
|-----|-----|-----|
| **SearchText** | string | The text that was searched for in the document. |
| **MatchCase** | bool | Indicates whether the search is case-sensitive. |

#### Code Example
```razor
<SfPdfViewer2>
    <PdfViewerEvents OnTextSearchComplete="OnTextSearchComplete"></PdfViewerEvents>
</SfPdfViewer2>
@code {
    private void OnTextSearchComplete(TextSearchCompleteEventArgs args) {
        // customize your code in based on the requirements and check above table for event argument details
    }
}
```

### OnTextSearchHighlight
Event triggered when search highlights text matches. Receives TextSearchHighlightEventArgs. | Event | `EventCallback<TextSearchHighlightEventArgs>` |

#### Value
`EventCallback<TextSearchHighlightEventArgs>`

#### OnTextSearchHighlight Events Arguments
| Event Arguments | Data Type | Description |
|-----|-----|-----|
| **PageNumber** | int | Specifies the page number of the highlighted search text. |
| **Bound** | [Bound](./annotation-general-enum-properties.md#bound) | Specifies the bounds of the highlighted searched text. |

#### Code Example
```razor
<SfPdfViewer2>
    <PdfViewerEvents OnTextSearchHighlight="OnTextSearchHighlight"></PdfViewerEvents>
</SfPdfViewer2>
@code {
    private void OnTextSearchHighlight(TextSearchHighlightEventArgs args) {
        // customize your code in based on the requirements and check above table for event argument details
    }
}
```
