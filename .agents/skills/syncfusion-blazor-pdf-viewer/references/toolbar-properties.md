# Toolbar Settings

## Table of Contents
- [Overview](#overview)
- [Properties]
- [Toolbar Settings](#toolbar-settings)
- [Toolbar Settings Related Events](#toolbar-settings-related-events)
- [Code Example](#toolbar-settings-code-example)

## Overview

The SfPdfViewer component provides comprehensive toolbar customization for the PDF Viewer. It allows developers to control which toolbar buttons appear, customize the toolbar layout, and add custom functionality to meet specific workflow requirements.

## Properties

### EnableToolbar
If it is set as false, then toolbar of the PDF Viewer will not be visible. By default it is true and visible.
- `false` - Hides the default toolbar from the PDF viewer.
- `true` - Shows the default toolbar from the PDF viewer.

#### Data Type and default value
`bool` - `true(Default)`

#### Code Example
```razor
<SfPdfViewer2 EnableToolbar="true"></SfPdfViewer2>
```

### EnableAnnotationToolbar
If it is set as false, then the edit annotation option in the toolbar will be disabled. By default it is true.
- `false` - Disabled the edit annotation option in the toolbar from the PDF viewer.
- `true` - Enabled the edit annotation option in the toolbar from the PDF viewer.

#### Data Type and default value
`bool` - `true(Default)`

#### Code Example
```razor
<SfPdfViewer2 EnableAnnotationToolbar="true"></SfPdfViewer2>
```

## Toolbar Settings

| **Property Name** | **Description** | **Data type** |
|-----|-----|-----|
| **AnnotationToolbarItems** | Provide option to customize the annotation toolbar of the PDF Viewer. PDF Viewer annotation toolbar will be rendered only options configured with this property. | List<[AnnotationToolbarItem](./toolbar-enum-properties.md#annotationtoolbaritem)> |
| **CustomToolbarItems** | Represents a class specifying properties for customize toolbar items. | List<[PdfToolbarItem](./toolbar-enum-properties.md#pdftoolbaritem)> |
| **FormDesignerToolbarItems** | Gets or sets the toolbar items available in the form designer of the PDF Viewer. | List<[FormDesignerToolbarItem](./toolbar-enum-properties.md#formdesignertoolbaritem)> |
| **MobileToolbarItems** | Specifies options for the Mobile Toolbar in the PDF Viewer. The PDF Viewer toolbar in mobile will only display options configured with this property. | List<[MobileToolbarItem](./toolbar-enum-properties.md#mobiletoolbaritem)> |
| **RedactionToolbarItems** | Gets or sets the toolbar items available in the redaction toolbar of the PDF Viewer. | List<[RedactionToolbarItem](./toolbar-enum-properties.md#redactiontoolbaritem)> |
| **ToolbarItems** | Provide option to customize the toolbar of the PDF Viewer. PDF Viewer toolbar will be rendered only options configured with this property. | List<[ToolbarItem](./toolbar-enum-properties.md#toolbaritem)> |
| **ShowTooltip** | Enable or disables the toolbar of PdfViewer. By default it is true. | bool |

## Toolbar Settings Related Events

### ToolbarClicked
Triggers an event when a Custom Toolbar Item is clicked in the Toolbar.

#### Value
`EventCallback<ClickEventArgs>`

#### ToolbarClicked Events Arguments
| Event Arguments | Data Type | Description |
|-----|-----|-----|
| `Cancel` | bool | Gets or sets whether the item click action should be canceled or not. |
| `Item` | [ItemModel](./toolbar-enum-properties.md#item-model) | Gets the clicked ItemModel of toolbar. |
| `Name` | string | Gets name of the event. |
| `OriginalEvent` | MouseEventArgs | Gets the mouse event informations. |

- `MouseEventArgs` - A MouseEventArgs object that represents the mouse event information.

#### Code Example
```razor
<SfPdfViewer2>
    <PdfViewerEvents ToolbarClicked="ToolbarClicked"></PdfViewerEvents>
</SfPdfViewer2>
@code {
    private void ToolbarClicked(ClickEventArgs args) {
        // customize your code in based on the requirements and check above table for event argument details 
    }
}
```

#### Note 
- When we use the `CustomToolbarItems` properties for customized toolbar items, then we have provide the click functionlaities through this event.
- When we used `Syncfusion.Blazor.Navigations` package for the `CustomToolbarItems` template, then we should used the `ToolbarItem` enum value get conflicts. Then we should use like `Syncfusion.Blazor.SfPdfViewer.ToolbarItem`

## Toolbar Settings Code Example

- The below example has been demo with the limited options. We can modifiy the below API as per the user's requirement.

```razor

<SfPdfViewer2 @ref="Viewer">
    <PdfViewerToolbarSettings AnnotationToolbarItems="annotationToolbarItems" CustomToolbarItems="CustomToolbarItems" FormDesignerToolbarItems="formDesignerToolbarItems" MobileToolbarItems="mobileToolbarItems" RedactionToolbarItems="redactionToolbarItems" ShowTooltip="true" ToolbarItems="toolbarItems"></PdfViewerToolbarSettings>
    <PdfViewerEvents ToolbarClicked="ClickAction"></PdfViewerEvents>
</SfPdfViewer2>
@code {
    SfPdfViewer2 Viewer;
    List<Syncfusion.Blazor.SfPdfViewer.ToolbarItem> toolbarItems = new List<Syncfusion.Blazor.SfPdfViewer.ToolbarItem>() {
        Syncfusion.Blazor.SfPdfViewer.ToolbarItem.OpenOption,
        Syncfusion.Blazor.SfPdfViewer.ToolbarItem.DownloadOption
    };
    List<AnnotationToolbarItem> annotationToolbarItems = new List<AnnotationToolbarItem>() {
        AnnotationToolbarItem.CalibrateTool,
        AnnotationToolbarItem.FreeTextAnnotationTool
    };
    List<RedactionToolbarItem> redactionToolbarItems = new List<RedactionToolbarItem>() {
        RedactionToolbarItem.MarkForRedaction,
        RedactionToolbarItem.Redact
    };
    List<MobileToolbarItem> mobileToolbarItems = new List<MobileToolbarItem>() {
        MobileToolbarItem.Open,
        MobileToolbarItem.FormDesigner
    };
    List<FormDesignerToolbarItem> formDesignerToolbarItems = new List<FormDesignerToolbarItem>() {
        FormDesignerToolbarItem.Button,
        FormDesignerToolbarItem.CheckBox
    };
    public List<PdfToolbarItem> CustomToolbarItems = new List<PdfToolbarItem>() 
    {    
        new PdfToolbarItem (){ Index = 0, Template = @GetTemplate("PreviousPage")}, 
        new PdfToolbarItem (){ Index = 1, Template = @GetTemplate("NextPage")}
    };  

    // Get the renderfragment element for the custom toolbaritems in the primary toolbar
    private static RenderFragment GetTemplate(string name)  
    {  
        return __builder => 
        {  
            if (name == "PreviousPage")
            { 
                <ToolbarItem PrefixIcon="e-icons e-chevron-up" 
                            Text="Previous Page" 
                            TooltipText="Previous Page" 
                            Id="previousPage" 
                            Align="ItemAlign.Left"> 
                </ToolbarItem> 
            }  
            else if(name == "NextPage")
            { 
                <ToolbarItem PrefixIcon="e-icons e-chevron-down" 
                            Text="Next Page" 
                            TooltipText="Next Page" 
                            Id="nextPage" 
                            Align="ItemAlign.Left"> 
                </ToolbarItem> 
            }  
        };          
    }  

    // Click for the custom toolbaritems in the primary toolbar
    public async void ClickAction(ClickEventArgs Item) 
    {  
        if (Item.Item.Id == "previousPage") 
        {
            //Navigate to previous page of the PDF document.
            await Viewer.GoToPreviousPageAsync(); 
        }  
        else if (Item.Item.Id == "nextPage") 
        {
            //Navigate to next page page of the PDF document.
            await Viewer.GoToNextPageAsync(); 
        }
    }
}

```

### Note
- `GetTemplate` function should be static when we want to render that customized elements.
