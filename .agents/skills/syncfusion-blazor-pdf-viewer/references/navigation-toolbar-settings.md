# Navigation Toolbar Settings in Blazor SfPdfViewer Component

## Table of Contents
- [Overview](#overview)
- [Related Properties](#related-properties)
- [Settings Properties List](#settings-properties-list)
- [Code Example](#code-example)

## Overview

It describes about the enable disable rpoperties of the navigation toolbar and also provides configuration options for customizing the navigation toolbar in the Blazor PDF Viewer.

## Related Properties

### EnableNavigationToolbar
Gets or sets whether the left-side navigation toolbar is visible.
- `false` - Hide the left-side navigation toolbar in the PDF Viewer.
- `true` - Shows the left-side navigation toolbar in the PDF Viewer.

#### Data Type and default value
`bool` - `true(Default)`

#### Code Example
```razor
<SfPdfViewer2 EnableNavigationToolbar="true"></SfPdfViewer2>
```

### EnableNavigationPanel
Gets or sets whether the modern navigation panel is enabled in the PDF Viewer. When Customize the options in the navigation toolbar, then this property required.

#### Data Type
`bool`

#### Code Example
```razor
<SfPdfViewer2 EnableNavigationPanel="true"></SfPdfViewer2>
```

## Settings Properties List

| **Property** | **Type** | **Description** |
|---|---|---|
| **BuiltInItems** | List<[NavigationToolbarItem](./general-enum-properties.md#navigationtoolbaritem)> | Collection of built-in navigation items to display in the navigation toolbar |
| **CustomItems** | List<[CustomNavigationToolbarItem](./general-enum-properties.md#customnavigationtoolbaritem)> | Collection of custom toolbar items to inject into the navigation toolbar |

## Code Example

- We can customize the options, based on the user requirements.

It is an example code for add panel with set annotation mode APIs.

```razor
<SfPdfViewer2 EnableNavigationPanel="true">
    <NavigationToolbarSettings BuiltInItems="navigationToolbarItems" CustomItems="customNavigationToolbarItems"></NavigationToolbarSettings>
</SfPdfViewer2>

@code {
    SfPdfViewer2 pdfViewer;
    // List of built-in navigation panel items.
    List<NavigationToolbarItem> navigationToolbarItems = new List<NavigationToolbarItem>()
    {
        NavigationToolbarItem.Thumbnails,
        NavigationToolbarItem.Bookmarks,
        NavigationToolbarItem.CommentPanel,
    };
    // List to hold the custom navigation toolbar items.
    List<CustomNavigationToolbarItem> customNavigationToolbarItems;

    // Initializes the component and sets up the custom navigation items.
    protected override void OnInitialized()
    {
        customNavigationToolbarItems = new List<CustomNavigationToolbarItem>()
        {
            new CustomNavigationToolbarItem()
            {
                Name = "Edit Annotation",
                HeaderText = "Edit Annotation",
                IconCss = "e-pv-annotation-icon e-pv-icon",
                Index = 4,
                TooltipText = "Edit Annotation",
                ItemType = NavigationToolbarItemType.Button,
                Template = EditAnnotationTemplate()
            }
        };
    }

    // Defines the RenderFragment for the custom panel's content.
    private RenderFragment EditAnnotationTemplate()
    {
        return @<div style="padding: 16px 24px; background-color: #f9fafb; border-top: 1px solid #e5e7eb; display: flex; flex-direction: column; align-items: center; gap: 12px; border-bottom-left-radius: 12px; border-bottom-right-radius: 12px;">
            <button style="background: #ffffff; color: #374151; border: 1px solid #d1d5db; padding: 10px 18px; border-radius: 6px; font-size: 14px; font-weight: 500; cursor: pointer;" @onclick="AddRectangle">
                Add Rectangle
            </button>
            <button style="background: #ffffff; color: #374151; border: 1px solid #d1d5db; padding: 10px 18px; border-radius: 6px; font-size: 14px; font-weight: 500; cursor: pointer;" @onclick="AddRadius">
                Add Radius
            </button>
            <button style="background: #ffffff; color: #374151; border: 1px solid #d1d5db; padding: 10px 18px; border-radius: 6px; font-size: 14px; font-weight: 500; cursor: pointer;" @onclick="AddFreeText">
                Add Free text
            </button>
        </div>;
    }

    // Sets the annotation mode to Rectangle.
    private async Task AddRectangle()
    {
        await pdfViewer.SetAnnotationModeAsync(AnnotationType.Rectangle);
    }

    // Sets the annotation mode to Radius.
    private async Task AddRadius()
    {
        await pdfViewer.SetAnnotationModeAsync(AnnotationType.Radius);
    }

    // Sets the annotation mode to FreeText.
    private async Task AddFreeText()
    {
        await pdfViewer.SetAnnotationModeAsync(AnnotationType.FreeText);
    }
}
```
