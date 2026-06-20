# Redaction Settings in Blazor SfPdfViewer Component

## Table of Contents
- [Overview](#overview)
- [Property Reference](#property-reference)
- [Code Example](#code-example)

## Overview

Guide users to configure redaction settings when they need to permanently remove sensitive information from PDFs. Redaction settings control how redacted areas appear, what text overlays display, and how users can interact with redactions before finalizing them.

## Property Reference

| **Property Name** | **Description** | **Data type** |
|-----|-----|-----|-----|
| **OverlayText** | Gets or sets the text to be displayed as an overlay in the redaction annotation. Specifies the string that will appear over the redacted area. If `IsRepeat` is `true`, this text will repeat to fill the redaction bounds; otherwise it appears only once. | string |
| **MarkerFillColor** | Gets or sets the fill color of the redaction marker. This property defines the color used to fill the redaction area. The fill color can be set using any valid CSS color string (e.g., `"#FFFFFF"`, `"green"`, or `"rgba(255, 255, 255, 1)"`). | string |
| **MarkerBorderColor** | Gets or sets the border color of the redaction marker. This property defines the color of the border surrounding the redaction area. You can specify the border color using a valid CSS color string (e.g., `"#FF0000"`, `"blue"`, or `"rgba(255, 0, 0, 1)"`). | string |
| **MarkerOpacity** | Gets or sets the opacity of the redaction marker. This property controls the transparency of the redaction marker's fill and border. A value of `1` represents full opacity (no transparency), while a value of `0` represents full transparency. | double |
| **FontColor** | Gets or sets the font color of the overlay text in the redaction annotation. Specifies the color used for the overlay text displayed within the redacted area. The value must be a valid CSS color string. | string |
| **FontSize** | Gets or sets the font size of the overlay text in the redaction annotation. This property determines the size of the overlay text displayed within the redacted area. Larger values result in bigger text, while smaller values produce smaller text. The unit is in points (pt). | double |
| **FontFamily** | Gets or sets the font family used for the overlay text in the redaction annotation. Defines the font style of the overlay text that appears on the redacted area. You can use standard font family names such as `Arial`, `Courier`, `Helvetica`, or any other supported font. | string |
| **IsRepeat** | Gets or sets a value indicating whether the overlay text should repeat to fill the redaction area. When `true`, the specified OverlayText will repeat across the entire bounds of the redaction annotation. When `false`, the overlay text will be displayed only once. | bool |
| **TextAlignment** | Gets or sets the alignment of the overlay text displayed in the redaction annotation. This property defines how the overlay text is aligned within the bounds of the redaction area. Supported alignment options are `Left`, `Center`, and `Right`. | [TextAlignment](./annotation-general-enum-properties.md#textalignment) |
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
| **Opacity** | Defines the opacity for the shape annotations. By default it is 1. It's range varies 0 to 1. | double |
| **StrokeColor** | Defines the stroke color of the shape annotations | string |
| **Thickness** | Defines the thickness of the shape annotations. By default it is 1. It's range varies 1 to 10. | int |

## Code Example

```razor
<SfPdfViewer2>
    <PdfViewerRedactionSettings AllowedInteractions="allowedInteractions" AnnotationSelectorSettings="selectorSettings" Author="Syncfusion" BorderDashArray="[1, 3]" CustomData="CustomData" FillColor="green" FontColor="black" FontFamily="Times New Roman" FontSize="20" IsLock="true" IsPrint="false" IsRepeat="true" MarkerBorderColor="red" MarkerFillColor="yellow" MarkerOpacity="3" MaxHeight="100" MaxWidth="100" MinHeight="50" MinWidth="50" Opacity="0.5" OverlayText="Syncfusion" SkipDownload="false" SkipPrint="false" StrokeColor="blue" TextAlignment="TextAlignment.Center" Thickness="4"></PdfViewerRedactionSettings>
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
