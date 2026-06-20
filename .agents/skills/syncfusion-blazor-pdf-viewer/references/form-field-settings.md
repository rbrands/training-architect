# Form Field Settings in Blazor SfPdfViewer Component

## Table of Contents
- [Overview](#overview)
- [Properties List](#properties-list)
- [Code Example](#code-example)

## Overview

Use form field settings when users need to customize the appearance and behavior of interactive form fields in PDF documents. These settings control visual properties like colors, fonts, borders, and text styling for text boxes, dropdowns, checkboxes, and other form elements.

### Properties List

| **Property Name** | **Description** | **Data type** |
|-----|-----|-----|
| **BackgroundColor** | Gets or sets the background color of the form field. Defines the background color of the form field to enhance its visual appearance. Accepts any valid HEX color code, RGB value, or CSS color name. | string |
| **BorderColor** | Gets or sets the border color of the form field. Specifies the color of the border surrounding the form field to enhance its visual appearance and make it easily identifiable. | string |
| **Color** | Gets or sets the text color for the form field. Defines the color of the text inside the form field. Applicable to text-based fields such as Button, Text Box, Password, List Box, and Drop Down fields. | string |
| **FontFamily** | Gets or sets the font family of the text in the form field. Defines the font style used for displaying text inside the form field. Applicable to text-based fields. If the specified font is unavailable, the browser may use a default fallback font. | string |
| **FontSize** | Gets or sets the font size of the text in the form field. Defines the size of the text displayed inside the form field. Applicable to text-based fields such as Button, Text Box, Password, List Box, and Drop Down fields. | double |
| **FontStyle** | Gets or sets the font style of the text in the form field. Defines the style of the text displayed inside the form field (e.g., Bold, Italic). Applicable to text-based fields. | [FontStyle](./annotation-general-enum-properties.md#fontstyle) |
| **Thickness** | Gets or sets the thickness of the border surrounding the form field. Specifies the border thickness of the form field measured in pixels. Default value is 1 pixel. Setting to 0 hides the border. | double |

## Code Example

```razor
<SfPdfViewer2>
    <FormFieldSettings BackgroundColor="green" BorderColor="blue" Color="red" FontFamily="Courier" FontSize="20" FontStyle="FontStyle.Bold" Thickness="3"></FormFieldSettings>
</SfPdfViewer2>
```
