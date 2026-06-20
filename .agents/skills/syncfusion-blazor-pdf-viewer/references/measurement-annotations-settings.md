# Measurement Annotations Settings in Blazor SfPdfViewer Component

## Table of Contents
- [Overview](#overview)
- [Properties List](#properties-list)
- [Code Example](#code-example)

## Overview

Measurement Annotations settings allow you to configure default units and ratios values for Measurement Annotations annotations in the PDF Viewer.

## Properties List

| **Property Name** | **Description** | **Data type** |
|-----|-----|-----|
| **ConversionUnit** | Defines the unit for measuring annotation. By default it is "in". | [CalibrationUnit](./annotation-enum-properties.md#calibrationunit) |
| **Depth** | Defines the value for depth. By default it is 96. | int |
| **DisplayUnit** | Defines the display unit for measuring annotation. By default it is "inch". | [CalibrationUnit](./annotation-enum-properties.md#calibrationunit) |
| **ScaleRatio** | Defines the scale ratio for measuring annotation. By default it is "1". It will be multiplied the actual value of measurement and this multiplied value only displayed in UI. | double |

## Code Example

- We can customize the options, based on the user requirements.

```razor
<SfPdfViewer2>
    <PdfViewerMeasurementSettings ConversionUnit="CalibrationUnit.Mm" Depth="120" DisplayUnit="CalibrationUnit.Mm" ScaleRatio="3"></PdfViewerMeasurementSettings>
</SfPdfViewer2>
```
