# Stamp Settings in Blazor SfPdfViewer Component

## Table of Contents
- [Overview](#overview)
- [Properties List](#properties-list)
- [Code Example](#code-example)

## Overview

Stamp settings allow you to configure default appearance, behavior, and restrictions for stamp / Image annotations in the PDF Viewer. Use these settings to control properties of the stamp / Image annotation.

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
| **Height** | Specified the height of the annotation. | int |
| **Width** | Specified the width of the annotation. | int |
| **DateTimeFormat** | Customize desired date and time format | string |
| **DynamicStamps** | Provide option to define the required dynamic stamp items to be displayed in annotation toolbar menu. | List<[DynamicStampItem](./annotation-enum-properties.md#dynamicstampitem)> |
| **SignStamps** | Provide option to define the required sign stamp items to be displayed in annotation toolbar menu. | List<[SignStampItem](./annotation-enum-properties.md#signstampitem)> |
| **StandardBusinessStamps** | Provide option to define the required standrd business stamp items to be displayed in annotation toolbar menu. | List<[StandardBusinessStampItem](./annotation-enum-properties.md#standardbusinessstampitem)> |

## Code Example

- We can customize the options, based on the user requirements.

```razor
<SfPdfViewer2>
    <PdfViewerStampSettings AllowedInteractions="allowedInteractions" AnnotationSelectorSettings="selectorSettings" Author="Syncfusion" BorderDashArray="[1, 3]" CustomData="CustomData" DateTimeFormat="dd:MM:yyyy" DynamicStamps="dynamicStampItems" FillColor="green" Height="150" IsLock="true" IsPrint="false" MaxHeight="100" MaxWidth="100" MinHeight="50" MinWidth="50" Opacity="0.5" SignStamps="signStampItems" SkipDownload="false" SkipPrint="false" StandardBusinessStamps="standardBusinessStampItems" StrokeColor="yellow" Thickness="3" Width="150"></PdfViewerStampSettings>
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
    List<DynamicStampItem> dynamicStampItems = new List<DynamicStampItem>() {
        DynamicStampItem.Revised,
        DynamicStampItem.Approved  
    };
    List<SignStampItem> signStampItems = new List<SignStampItem>() {
        SignStampItem.InitialHere,
        SignStampItem.SignHere
    };
    List<StandardBusinessStampItem> standardBusinessStampItems = new List<StandardBusinessStampItem>() {
        StandardBusinessStampItem.Approved,
        StandardBusinessStampItem.Completed
    };
}
```
