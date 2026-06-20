# Perimeter Settings in Blazor SfPdfViewer Component

## Table of Contents
- [Overview](#overview)
- [Properties List](#properties-list)
- [Code Example](#code-example)

## Overview

Perimeter settings allow you to configure default appearance, behavior, and restrictions for Perimeter annotations in the PDF Viewer. Use these settings to control properties of the Perimeter annotation.

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
| **BorderDashArray** | Defines the border dash array. | int[] |
| **FillColor** | Specifies the fill color of the annotation. | string |
| **Opacity** | Defines the opacity for the annotations. By default it is 1. It's range varies 0 to 1. | double |
| **StrokeColor** | Defines the stroke color of the annotations | string |
| **Thickness** | Defines the thickness of the annotations. By default it is 1. It's range varies 1 to 10. | int |

## Code Example

- We can customize the options, based on the user requirements.

```razor
<SfPdfViewer2>
    <PdfViewerPerimeterSettings AllowedInteractions="allowedInteractions" AnnotationSelectorSettings="selectorSettings" Author="Syncfusion" BorderDashArray="[1, 3]" CustomData="CustomData" FillColor="green" IsLock="true" IsPrint="false" MaxHeight="100" MaxWidth="100" MinHeight="50" MinWidth="50" Opacity="0.5" SkipDownload="false" SkipPrint="false" StrokeColor="yellow" Thickness="3"></PdfViewerPerimeterSettings>
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
