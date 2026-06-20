# Annotation Selector Settings in Blazor SfPdfViewer Component

## Table of Contents
- [Overview](#overview)
- [Properties List](#properties-list)
- [Code Example](#code-example)

## Overview

Annotation Selector settings allow you to configure default appearance of the selection for all the types of annotations in the PDF Viewer.

When we need to set the selector settings for all the annotations as common, then we can use this properties. Then we can use this settings for common for all the annotation not for individual annotation.

## Properties List

| **Property Name** | **Description** | **Data type** |
|-----|-----|-----|
| **ResizerBorderColor** | Defines the annotation resizer border color. By default it is black. | string |
| **ResizerCursorType** | Defines the annotation resizer Type. By default it is null. | [CursorType](./annotation-settings-enum-properties.md#cursortype) |
| **ResizerFillColor** | Defines the annotation resizer fill color. | string |
| **ResizerLocation** | Defines the location for the resizer of the annotation. It is used to customize the resizer location of the annotation. | [AnnotationResizerLocation](./annotation-settings-enum-properties.md#annotationresizerlocation) |
| **ResizerShape** | Defines the shape of the resizer. By default it is Square. Different shapes of resizer are circle and square. | [AnnotationResizerShape](./annotation-settings-enum-properties.md#annotationresizershape) |
| **ResizerSize** | Defines the size of the resizer used for annotations. | int |
| **SelectionBorderColor** | Defines the selection border color for the annotation. By default it is empty. It is used to customize the selection border color for the annotation. | string |
| **SelectionBorderThickness** | Defines the selection border thickness for the annotation. By default it is 1. It is used to customize the selection border thickness for the annotation. It's range varies from 1 to 10. | double |
| **SelectorLineDashArray** | Defines the selector line dash array. By default it is empty. | double[] |

## Code Example

- We can customize the options, based on the user requirements.

```razor
<SfPdfViewer2>
    <PdfViewerAnnotationSelectorSettings ResizerBorderColor="black" ResizerCursorType="CursorType.CrossHair" ResizerFillColor="blue" ResizerLocation="AnnotationResizerLocation.Corners" ResizerShape="AnnotationResizerShape.Circle" ResizerSize="4" SelectionBorderColor="green" SelectionBorderThickness="3" SelectorLineDashArray="[2, 3]"></PdfViewerAnnotationSelectorSettings>
</SfPdfViewer2>
```
