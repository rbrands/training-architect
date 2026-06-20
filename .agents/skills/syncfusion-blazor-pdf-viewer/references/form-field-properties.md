# Syncfusion Blazor PDF Viewer - Form Field Properties

## Table of Contents
- [Overview](#overview)
- [Properties](#properties)
- [Methods](#methods)

## Overview

The Syncfusion Blazor PDF Viewer provides robust form field handling capabilities that allow users to interact with, edit, and validate PDF forms. Form field properties control whether form designers can be used, how form fields are interacted with, and whether validation is performed. These properties enable comprehensive form management and design workflows within the PDF Viewer component.

## Properties

The following table provides a comprehensive list of form field-related properties available in the Syncfusion Blazor PDF Viewer:

### EnableFormDesigner
Gets or sets a value indicating whether the form designer is enabled in the PDF Viewer.
- `false` - Disabled the form designer functionalities from the PDF Viewer.
- `true` - Enabled the form designer functionalities from the PDF Viewer.

#### Data Type and default value
`bool` - `true(Default)`

#### Code Example
```razor
<SfPdfViewer2 EnableFormDesigner="true"></SfPdfViewer2>
```

### EnableFormFields
If set to false, form fields in the PDF document can't be interacted or edited. By default it is true.
- `false` - Disabled the interactions between the form fields in the PDF Viewer.
- `true` - Enabled the interactions between the form fields in the PDF Viewer.

#### Data Type and default value
`bool` - `true(Default)`

#### Code Example
```razor
<SfPdfViewer2 EnableFormFields="true"></SfPdfViewer2>
```

### EnableFormFieldsValidation
If set to true, validate form fields will be triggered. By default it is false.

#### Data Type and default value
`bool` - `false(Default)`

#### Code Example
```razor
<SfPdfViewer2 EnableFormFieldsValidation="true"></SfPdfViewer2>
```

### IsFormFieldDocument
Gets the value to check whether the loaded document have the form fields or not. We can bind the properties to get the values of the this API.

#### Data Type
`bool`

#### Code Example
```razor
<SfPdfViewer2 IsFormFieldDocument="IsFormFieldDocument"></SfPdfViewer2>
@code {
    private bool IsFormFieldDocument;
}
```

### EnableAutoComplete
If it is set as false, auto complete behavior of form fields will be disabled. Be default it is true.

#### Data Type and default value
`bool` - `true(Default)`

#### Code Example
```razor
<SfPdfViewer2 EnableAutoComplete="true"></SfPdfViewer2>
```

### Methods

### SetFormDrawingModeAsync
Asynchronously sets or clears the form field type in the PDF Viewer. If no argument is provided or null is passed, the current form field type will be cleared, otherwise with the respected type values then it add that form fields in the PDF Viewer.

#### Parameters
- [FormFieldType](./form-fields-enum-properties.md#formfieldtype)

#### Return Type
- `Task`

#### Code Example
```csharp
    // We can bind the method for click action based the user requirement.
    await viewer.SetFormDrawingModeAsync(FormFieldType.Button);

    // If no argument is provided or null is passed, the current form field type will be cleared.
    await viewer.SetFormDrawingModeAsync(null);
```

