# Free text Settings in Blazor SfPdfViewer Component

## Table of Contents
- [Overview](#overview)
- [Properties List](#properties-list)
- [Code Example](#code-example)

## Overview

Free text settings allow you to configure default appearance, behavior, and restrictions for free text annotations in the PDF Viewer. Use these settings to control properties of the free text annotation.

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
| **AnnotationSelectorSettings** | Defines the annotaton selector settings for the annotation. | [PdfViewerAnnotationSelectorSettings](./annotation-settings-enum-properties.md#pdfviewerannotationselectorsettings) |
| **Opacity** | Defines the opacity for the annotations. By default it is 1. It's range varies 0 to 1. | double |
| **Height** | Specified the height of the annotation. | int |
| **Width** | Specified the width of the annotation. | int |
| **AllowEditTextOnly** | If it is set as true, then we can't move , resize the freetext annotations in the PDF Viewer. Just we can modify the existing text content. Otherwise along with modifying text we can resize, move the existing text content.By default it is false. | bool |
| **AutoFit** | Enable or disable auto fit mode for FreeText annotation in the Pdfviewer. FALSE by default. | bool |
| **BorderColor** | Defines the border color for free text or label text for shape and measurement annotation. By default it is "#ffffff00". | string |
| **BorderStyle** | Defines the border style for free text or label text for shape and measurement annotation. By default it is "solid". | string |
| **BorderThickness** | Defines the border width for free text or label text for shape and measurement annotation. By default it is "1". | int |
| **DefaultText** | Defines the fill color for free text or label text for shape and measurement annotation. By default it is "Type Here". | string |
| **FillColor** | Defines the fill color for free text or label text for shape and measurement annotation. By default it is "#ffffff00". | string |
| **FontColor** | Defines the font color for free text or label text for shape and measurement annotation. By default it is "#000". | string |
| **FontFamily** | Defines the font family for free text or label text for shape and measurement annotation. By default it is "Helvetica". | string |
| **FontSize** | Defines the font size for free text or label text for shape and measurement annotation. By default it is "16". | int |
| **FontStyle** | Defines the font style for free text or label text for shape and measurement annotation. By default it is "None". | [FontStyle](./annotation-general-enum-properties.md#fontstyle) |
| **TextAlignment** | Defines the text alignment for free text or label text for shape and measurement annotation. By default it is "Left". | [TextAlignment](./annotation-general-enum-properties.md#textalignment) |

## Code Example

- We can customize the options, based on the user requirements.

```razor
<SfPdfViewer2>
    <PdfViewerFreeTextSettings AllowEditTextOnly="true" AllowedInteractions="allowedInteractions" AnnotationSelectorSettings="selectorSettings" Author="Syncfusion" AutoFit="true" BorderColor="red" BorderStyle="Solid" BorderThickness="3" CustomData="CustomData" DefaultText="Pdf Viewer" FillColor="green" FontColor="black" FontFamily="Symbol" FontSize="20" FontStyle="FontStyle.Italic" Height="100" IsLock="true" IsPrint="false" MaxHeight="100" MaxWidth="100" MinHeight="50" MinWidth="50" Opacity="0.5" SkipDownload="false" SkipPrint="false" TextAlignment="TextAlignment.Center" Width="100"></PdfViewerFreeTextSettings>
</SfPdfViewer2>

@code {
    List<AllowedInteraction> allowedInteractions = new List<AllowedInteraction>() {
        AllowedInteraction.Select,
        AllowedInteraction.Delete
    };
    PdfViewerAnnotationSelectorSettings selectorSettings = new PdfViewerAnnotationSelectorSettings() {
        SelectionBorderColor = "black", 
        ResizerBorderColor = "blue", 
        ResizerCursorType= CursorType.CrossHair, 
        ResizerFillColor="orange", 
        ResizerLocation=AnnotationResizerLocation.Corners, 
        ResizerShape=AnnotationResizerShape.Circle, 
        ResizerSize=4, 
        SelectionBorderThickness=2, 
        SelectorLineDashArray=[1, 2]  
    };
    private object CustomData = new
    {
        Owner = "Syncfusion"
    };
}
```
