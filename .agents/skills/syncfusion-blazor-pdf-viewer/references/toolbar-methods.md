# Toolbar Methods in Blazor SfPdfViewer Component

## Table of Contents
- [Overview](#overview)
- [Methods](#methods)

## Overview

The toolbar methods in `SfPdfViewer2` provide programmatic control over toolbar visibility and behavior. These methods enable developers to customize the PDF viewer UI for specific workflows, restrict user actions, and create simplified viewing experiences tailored to application requirements.

## Methods

## ShowToolbarAsync(bool)
Shows or hides the main toolbar with all standard tools. Set to `true` to show, `false` to hide.

#### Parameters
- `bool` - enableToolbar - whether we have to show the toolbar element to UI or hide the element

#### Return Type
`Task`

#### Code Example
```csharp
    // We can bind the method for click action based the user requirement.
    await viewer.ShowToolbarAsync(true);
```

## ShowAnnotationToolbar(bool)
Shows or hides the annotation toolbar for markup and commenting tools. Synchronous operation.
- This method toggles the visibility of the redaction annotation toolbar in the PDF Viewer. It allows the user to control the toolbar's visibility based on their interactions with the viewer.

#### Parameters
- `bool` - enableAnnotationToolbar - whether we have to show the toolbar element to UI or hide the element

#### Return Type
`void`

#### Code Example
```csharp
    // We can bind the method for click action based the user requirement.
    viewer.ShowAnnotationToolbar(true);
```

## ShowRedactionToolbar(bool)
Shows or hides the redaction toolbar for removing sensitive information. Synchronous operation.
- This method toggles the visibility of the redaction redaction toolbar in the PDF Viewer. It allows the user to control the toolbar's visibility based on their interactions with the viewer.

#### Parameters
- `bool` - enableredactionToolbar - A boolean value indicating whether to show or hide the redaction annotation toolbar. Set to true to show the toolbar, or false to hide it.

#### Return Type
`void`

#### Code Example
```csharp
    // We can bind the method for click action based the user requirement.
    viewer.ShowRedactionToolbar(true);
```

## EnableToolbarItemsAsync(List<ToolbarItem>, bool)
Enables or disables specific toolbar items. Items remain visible but appear grayed out when disabled.

#### Parameters
- List<[ToolbarItem](./toolbar-enum-properties.md#toolbaritem)> - items - List of items that we have to enable or disables in the toolbar.
- `bool` - isEnable - A boolean value indicating whether to enable or disables the list of toolbar items.

#### Return Type
`Task`

#### Code Example
```csharp
    // We can bind the method for click action based the user requirement.
    List<ToolbarItem> toolbarItems = new List<ToolbarItem>() {
        ToolbarItem.OpenOption,
        ToolbarItem.DownloadOption
    };
    await viewer.EnableToolbarItemsAsync(toolbarItems, true);
```

## ShowToolbarItemsAsync(List<ToolbarItem>, bool)
Shows or hides specific toolbar items. Removes items from UI entirely when set to `false`. | Yes | Simplified toolbars, workflow-specific UIs, granular control over button visibility |

#### Parameters
- List<[ToolbarItem](./toolbar-enum-properties.md#toolbaritem)> - items - List of items that we have to show or hide in the toolbar.
- `bool` - isVisible - A boolean value indicating whether to show or hide the list of toolbar items.

#### Return Type
`Task`

#### Code Example
```csharp
    // We can bind the method for click action based the user requirement.
    List<ToolbarItem> toolbarItems = new List<ToolbarItem>() {
        ToolbarItem.OpenOption,
        ToolbarItem.DownloadOption
    };
    await viewer.ShowToolbarItemsAsync(toolbarItems, true);
```
