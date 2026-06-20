# Squiggly Settings in Blazor SfPdfViewer Component

## Table of Contents
- [Overview](#overview)
- [Properties List](#properties-list)
- [Code Example](#code-example)

## Overview

Squiggly settings allow you to configure default appearance, behavior, and restrictions for Squiggly annotations in the PDF Viewer. Use these settings to control properties of the Squiggly annotation.

## Properties List

| **Property Name** | **Description** | **Data type** |
|-----|-----|-----|
| **AllowedInteractions** | Gets or sets the allowed interactions for the locked annotations. IsLock can be configured using settings. | List<[AllowedInteraction](./pdf-annotation-enum-properties.md#allowedinteraction)> |
| **Author** | Specifies the author's name to add annotation or review the PDF document. By default it is Guest. | string |
| **CustomData** | Specifies the user's defined information related to the annotations. By default it is null. | object |
| **IsLock** | If it is set as true, can't interact with annotation. Otherwise can interact the annotations. By default it is false. | bool |
| **IsPrint** | Gets or sets the value for the individual annotations are included or not in print actions. | bool |
| **MaxHeight** | Sets the maximum height of annotations. It prevents the height of the annotation becoming larger than the values provided in MaxHeight.By default it is 0. | int |
| **MaxWidth** | Sets the maximum width of annotations. It prevents the width of the annotation becoming larger than values provided in MaxWidth.By default it is 0. | int |
| **MinHeight** | Sets the minimum height of annotations. It prevents the height of the annotation becoming smaller than values provided in MinHeight.By default it is 0. | int |
| **MinWidth** | Sets the minimum width of annotations. It prevents the width of the annotation becoming smaller than values provided in MinWidth.By default it is 0. | int |
| **SkipDownload** | If it is set as true, newly added annotations won't be included in downloaded file. By default it is false. | bool |
| **SkipPrint** | If it is set as true, newly added annotations won't be included in printing. By default it is false. | bool |
| **Opacity** | Defines the opacity for the annotations. By default it is 1. It's range varies 0 to 1. | double |
| **Color** | Defines the color for Text markup annotations like highlight or underline or strikethrough or squiggly. | string |
| **EnableMultiPageAnnotation** | If it is set as true, then can add text markup annotation with multiple pages. Otherwise can add text markup annotation only with in the page. By default it is false. | bool |
| **EnableTextMarkupResizer** | If it is set as true, resizer for text markup annotation will be enabled. By default it is false. | bool |

## Code Example

- We can customize the options, based on the user requirements.

```razor
<SfPdfViewer2>
    <PdfViewerSquigglySettings AllowedInteractions="allowedInteractions" Author="Syncfusion" Color="green" CustomData="CustomData" EnableMultiPageAnnotation="false" EnableTextMarkupResizer="true" IsLock="true" IsPrint="false" MaxHeight="100" MaxWidth="100" MinHeight="50" MinWidth="50" Opacity="0.5" SkipDownload="false" SkipPrint="false"></PdfViewerSquigglySettings>
</SfPdfViewer2>

@code {
    List<AllowedInteraction> allowedInteractions = new List<AllowedInteraction>() {
        AllowedInteraction.Select,
        AllowedInteraction.Delete
    };
    private object CustomData = new
    {
        Owner = "Syncfusion"
    };
}
```
