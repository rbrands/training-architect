# General Settings in Blazor SfPdfViewer Component

## Table of Contents
- [Overview](#overview)
- [List of Settings](#list-of-settings)

## Overview

When the user needs to customize the PDF Viewer's as context menu settings.

## List of Settings

### ContextMenuSettings
Defines the settings of context menu for the PDF Viewer. It is used to customize the context menu options to be visible in the PDF Viewer. 

#### Propeties List

| **Property Name** | **Description** | **Data type** |
|-----|-----|-----|-----|
| **EnableContextMenu** | Enable or disable context menu in PDF Viewer. By default it is true. | bool |
| **ContextMenuAction** | Defines the context menu action. By default it is RightClick. If it is set as MouseUp, context menu will open on mouse up action instead of right click action. | [ContextMenuAction](./general-enum-properties.md#contextmenuaction) |
| **ContextMenuItems** | Defines the context menu items that should be visible in the PDF Viewer. | List<[ContextMenuItem](./general-enum-properties.md#contextmenuitem)> |

#### Code Example

```razor
<SfPdfViewer2>
    <PdfViewerContextMenuSettings ContextMenuAction="ContextMenuAction.MouseUp" ContextMenuItems="contextMenuItems" EnableContextMenu="true"></PdfViewerContextMenuSettings>
</SfPdfViewer2>
@code {
    List<ContextMenuItem> contextMenuItems = new List<ContextMenuItem>() {
        ContextMenuItem.Copy,
        ContextMenuItem.Paste
    };
}
```
