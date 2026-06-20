# Syncfusion Blazor PDF Viewer - Text Interatcion Properties

## Table of Contents
- [Overview](#overview)
- [Properties List](#properties-list)
- [Methods](#methods)
- [Events](#events)

## Overview

The Syncfusion Blazor PDF Viewer provides comprehensive text selection and extraction support from the loaded document. The text selection has been used to select the certain text in a document. The text extraction has been used to extract the text details from the document.

## Properties List

### EnableTextSelection
If set to false, text selection in the PDF Viewer will be disabled. By default it is true. | true | bool |

#### Data Type and default value
`bool` - `true(Default)`

#### Code Example
```razor
<SfPdfViewer2 EnableTextSelection="true"></SfPdfViewer2>
```

### IsExtractText
If it is set as true, the PDF Viewer will extract the text from the document asynchronously after the initial loading.

#### Data Type and default value
`bool` - `false(Default)`

#### Code Example
```razor
<SfPdfViewer2 IsExtractText="true"></SfPdfViewer2>
```

## Methods

### ClearTextSelectionAsync()
Clears all text selection in the PDF document. Removes any highlighted or selected text regions.

#### Parameters
No Parameters

#### Return Type
`Task`

#### Code Example
```csharp
    // We can bind the method for click action based the user requirement.
    await viewer.ClearTextSelectionAsync();
```

### SelectTextRegionAsync(int, List<Bound>)
Selects a specific text region on a page based on provided boundary coordinates asynchronously. Programmatically highlights text without user interaction.

#### Parameters
- `int` - pageNumbers - Page number containing the text to select.
- List<[Bound](./annotation-general-enum-properties.md#bound)> -  bounds - List of boundary rectangles defining the text region.

#### Return Type
`Task`

#### Code Example
```csharp
    // We can bind the method for click action based the user requirement.
    List<Bound> bounds = new List<Bound>() {
        new Bound() {
            X=50,
            Y=50,
            Height=50,
            Width=50
        }
    };
    await viewer.SelectTextRegionAsync(5, bounds);
```

## Events

### OnTextSelectionEnd
Triggers when text selection ends.

#### Value
`EventCallback<TextSelectionEndEventArgs>`

#### OnTextSelectionEnd Events Arguments
| Event Arguments | Data Type | Description |
|-----|-----|-----|
| `TextBounds` | List<[TextBound](./general-enum-properties.md#textbound)> | Defines the bounds of the selected text in the page
| `TextContent` | string | Defines the text content selected in the page. |

#### Code Example
```razor
<SfPdfViewer2>
    <PdfViewerEvents OnTextSelectionEnd="OnTextSelectionEnd"></PdfViewerEvents>
</SfPdfViewer2>
@code {
    private void OnTextSelectionEnd(TextSelectionEndEventArgs args) {
        // customize your code in based on the requirements and check above table for event argument details
    }
}
```

### OnTextSelectionStart
Triggers when text selection starts.

#### Value
`EventCallback<TextSelectionStartEventArgs>`

#### OnTextSelectionStart Events Arguments
| Event Arguments | Data Type | Description |
|-----|-----|-----|
| `PageNumber` | int | Defines the page number in which the text selection is started. |

#### Code Example
```razor
<SfPdfViewer2>
    <PdfViewerEvents OnTextSelectionStart="OnTextSelectionStart"></PdfViewerEvents>
</SfPdfViewer2>
@code {
    private void OnTextSelectionStart(TextSelectionStartEventArgs args) {
        // customize your code in based on the requirements and check above table for event argument details
    }
}
```

### ExtractTextCompleted
Triggers when text extraction is completed in the SfPdfViewer.

#### Value
`EventCallback<ExtractTextCompletedEventArgs>`

#### ExtractTextCompleted Events Arguments
| Event Arguments | Data Type | Description |
|-----|-----|-----|
| `DocumentTextCollection` | List<Dictionary<int, [PageTextContent](./general-enum-properties.md#pagetextcontent)>> | Extracted text organized by page. |

#### Code Example
```razor
<SfPdfViewer2>
    <PdfViewerEvents ExtractTextCompleted="ExtractTextCompleted"></PdfViewerEvents>
</SfPdfViewer2>
@code {
    private void ExtractTextCompleted(ExtractTextCompletedEventArgs args) {
        // customize your code in based on the requirements and check above table for event argument details
    }
}
```
