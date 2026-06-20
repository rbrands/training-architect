# Navigation Toolbar Settings in Blazor SfPdfViewer Component

## Table of Contents
- [Overview](#overview)
- [Settings Properties List](#settings-properties-list)
- [Code Example](#code-example)

## Overview

Defines the settings for page organization features in the PDF Viewer. Use this settings to configure various page manipulation capabilities such as deletion, insertion, rotation, duplication, and more. These settings allow customization of user interactions with pages in the PDF Viewer.

## Settings Properties List

| **Property** | **Type** | **Description** |
|---|---|---|
| **CanDelete** | bool | Enables or disables the ability to delete pages in the page organizer. |
| **CanInsert** | bool | Enables or disables the ability to insert pages in the page organizer. |
| **CanRotate** | bool | Enables or disables the ability to rotate pages in the page organizer. |
| **CanDuplicate** | bool | Enables or disables the ability to duplicate pages in the page organizer. |
| **CanRearrange** | bool | Enables or disables the ability to rearrange pages in the page organizer. |
| **CanImport** | bool | Enables or disables the ability to import pages in the page organizer. |
| **CanExtractPages** | bool | Enables or disables the ability to extract pages from the document. |
| **ImageZoom** | double | Enables or disables zoom functionality for page thumbnails. |
| **FooterButtons** | [FooterButton](./annotation-settings-enum-properties.md#footerbutton) | Specifies which buttons are visible in the Page Organizer dialog footer. |
| **ShowImageZoomingSlider** | bool | Enables or disables the display of the zooming slider for page thumbnails. |
| **ImageZoomMin** | int | Sets the minimum zoom level for page thumbnails. Acceptable values: 2 to 5. |
| **ImageZoomMax** | int | Sets the maximum zoom level for page thumbnails. Acceptable values: 2 to 5. |

## Code Example

- We can customize the options, based on the user requirements.

```razor
<SfPdfViewer2>
    <PageOrganizerSettings CanDelete="true" CanDuplicate="true" CanExtractPages="true" CanImport="true" CanInsert="true" CanRearrange="true" CanRotate="true" FooterButtons="FooterButton.Save" ImageZoom="2" ImageZoomMax="5" ImageZoomMin="2" ShowImageZoomingSlider="true"></PageOrganizerSettings>
</SfPdfViewer2>
```
