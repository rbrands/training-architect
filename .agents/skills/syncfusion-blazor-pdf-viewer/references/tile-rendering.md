# Tile Rendering in Blazor SfPdfViewer Component

## Table of Contents
- [Overview](#overview)
- [Properties List](#properties-list)
- [Code Example](#code-example)

## Overview

Represents the tile rendering settings for the PDF Viewer component. The below properties provides configuration options for enabling tile rendering and setting tile coordinates.

## Properties List

| **Property Name** | **Description** | **Data type** |
|-----|-----|-----|
| **EnableTileRendering** | Gets or sets a value indicating whether tile rendering is enabled. | bool |
| **X** | Defines the x co ordinates tiles rendered for the image. | int |
| **Y** | Defines the y co ordinates tiles rendered for the image. | int |

## Code Example

- We can customize the options, based on the user requirements.

```razor
<SfPdfViewer2>
    <PdfViewerTileRenderingSettings EnableTileRendering="true" X="4" Y="4"></PdfViewerTileRenderingSettings>
</SfPdfViewer2>
```
