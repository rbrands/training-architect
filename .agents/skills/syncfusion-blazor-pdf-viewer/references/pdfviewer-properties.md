# PDFViewer Properties

## Table of Contents
- [Overview](#overview)
- [Properties](#properties)

## Overview

This document has general properties available on the Blazor `SfPdfViewer` component. It describe the UI appearance of viewer, it's attributes, interatcion mode, custom font families supports for free text and text documents, document path and file name for download.

## Properties

### Height
Gets or sets the scrollable height of the PDF Viewer.

#### Data Type
`string`

#### Code Example
```razor
<SfPdfViewer2 Height="600px"></SfPdfViewer2>
```

### Width
Gets or sets the scrollable width of the PDF Viewer.

#### Data Type
`string`

#### Code Example
```razor
<SfPdfViewer2 Width="600px"></SfPdfViewer2>
```

### Id
Gets or sets the ID for the PDF Viewer parent DOM element div.

#### Data Type
`string`

#### Code Example
```razor
<SfPdfViewer2 ID="pdfviewer"></SfPdfViewer2>
```

### CssClass
Defines single or multiple classes (separated by space) to be used for customization styles of viewer.

#### Data Type
`string`

#### Code Example
```razor
<SfPdfViewer2 CssClass="pdfviewer-class"></SfPdfViewer2>
```

### DocumentPath
Sets the PDF document path for initial loading.

#### Data Type
`string`

#### Code Example
```razor
<SfPdfViewer2 DocumentPath="wwwroot/test.pdf"></SfPdfViewer2>
```

### DownloadFileName
Sets the name of the file to be downloaded.

#### Data Type
`string`

#### Code Example
```razor
<SfPdfViewer2 DownloadFileName="Pdfviewer"></SfPdfViewer2>
```

### CustomFonts
Sets the Custom fonts path locations for initial loading.
- `CustomFonts` it should be refered from the local paths and it support for the document text.

#### Data Type
`List<string>`

#### Code Example
```razor
<SfPdfViewer2 CustomFonts="customFonts"></SfPdfViewer2>
@code {
    public List<string> customFonts = new List<string> { "wwwroot/Custom-Fonts/arialbd.ttf", "wwwroot/Custom-Fonts/arial.ttf" };
}
```

### FallbackFontCollection
It is used to import the custom fonts into PDF Viewer.
- It supports loading, editing, and saving custom fonts in FreeText annotations

#### Data Type
`Dictionary<string, Stream>`

#### Code Example
```razor
<SfPdfViewer2 @ref="pdfViewer">
    <PdfViewerEvents Created="@Created"></PdfViewerEvents>
</SfPdfViewer2>

@code {
    private SfPdfViewer2? pdfViewer;

    public void Created()
    {
        // Use FallbackFontCollection to save the custom font
        // Maps the font family name to its corresponding TTF file as a memory stream  
        pdfViewer.FallbackFontCollection.Add("Arial", new MemoryStream(System.IO.File.ReadAllBytes("wwwroot/Fonts/ARIAL.ttf")));
        pdfViewer.FallbackFontCollection.Add("Arial Black", new MemoryStream(System.IO.File.ReadAllBytes("wwwroot/Fonts/ARIBLK.ttf")));
        pdfViewer.FallbackFontCollection.Add("Courier New", new MemoryStream(System.IO.File.ReadAllBytes("wwwroot/Fonts/COUR.ttf")));
    }
}
```

### Locale
Overrides the global culture and localization value for this component. Default global culture is 'en-US'. | string | en-US |

#### Data Type
`string`

#### Code Example
```razor
<SfPdfViewer2 Locale="en-US"></SfPdfViewer2>
```

### InteractionMode
Indicates whether the interaction is in selection(select tool) or pan mode. By default it is selection. | InteractionMode | Selection |

#### Data Type
[InteractionMode](./general-enum-properties.md#interactionmode)

#### Code Example
```razor
<SfPdfViewer2 InteractionMode="InteractionMode.Pan"></SfPdfViewer2>
```

### IsDocumentEdited
Gets the value to check whether the document has been edited or not. By default it is false. If the document edited, it returns true.

#### Data Type
`bool`

#### Code Example
```razor
<SfPdfViewer2 IsDocumentEdited="isDocumentEdited"></SfPdfViewer2>
@code {
    private bool isDocumentEdited;
}
```

### FontFamilies
Defines the list of preferred font families specified by the user.
- It supports loading, editing, and saving custom fonts in FreeText annotations

#### Data Type
`string[]`

#### Code Example
```razor
<SfPdfViewer2 FontFamilies="@FontFamilies"></SfPdfViewer2>

@code {
    internal string[] FontFamilies { get; set; } = { "Helvetica", "Courier", "Symbol", "Times New Roman", "Arial Black", "Courier New", "Arial" };
}
```

### RetryCount
Gets or sets the retry count for PDF Viewer service call. By default retrycount is 1.

#### Data Type
`double`

#### Code Example
```razor
<SfPdfViewer2 RetryCount="2"></SfPdfViewer2>
```

### HyperlinkOpenState
Indicates whether the URL is opened in current tab, new tab, new window. By default it is current tab. | LinkTarget | CurrentTab |

#### Data Type
[LinkTarget](./general-enum-properties.md#linktarget)

#### Code Example
```razor
<SfPdfViewer2 HyperlinkOpenState="LinkTarget.NewTab"></SfPdfViewer2>
```
