# Programmatical Form Field Methods in Blazor SfPdfViewer Component

## Table of Contents
- [Overview](#overview)
- [Methods List](#methods-list)

## Overview

Use programmatic form field methods when you need code-driven control over PDF form fields instead of relying solely on pre-existing form templates. These methods enable dynamic form creation, data binding, bulk field management, and integration with external data sources.

## Methods List

### AddFormFieldsAsync(List<FormFieldInfo>)
Asynchronously adds a list of form fields in the PDF Viewer.

#### Parameters
- List<[FormFieldInfo](./form-fields-enum-properties.md#formfieldinfo)> - `formFields` - Form fields need to be add in this method.

#### Return Type
`Task`

#### Code Example
```csharp
    // We can bind the method for click action based the user requirement.
    // example demonstrates how to add form fields.
    private async Task AddFormFields()
    {
        List<FormFieldInfo>  formFields = new List<FormFieldInfo>
        {
            new TextBoxField() { Name = "TextBox1", PageNumber = 1, Value = "text" },
            new CheckBoxField { Name = "CheckBox1", PageNumber = 1, IsChecked = true }
        };
        await pdfViewer.AddFormFieldsAsync(formFields);
    }
```

### DeleteFormFieldsAsync(bool)
Asynchronously deletes form fields in the PDF Viewer. If `deleteAllFields` is true, all form fields will be deleted; if false, only the selected form field will be deleted.

#### Parameters
- `bool` - `deleteAllFields` - A boolean value indicating whether to delete all form fields. If true, all form fields will be removed; if false or null, only the selected form field will be deleted.

#### Return Type
`Task`

#### Code Example
```csharp
    // We can bind the method for click action based the user requirement.
    // example demonstrates how to delete form fields.
    async Task DeleteAllFormFields()
    {
        await pdfViewer.DeleteFormFieldsAsync(true);
    }

    async Task DeleteSelectedFormField()
    {
        await pdfViewer.DeleteFormFieldsAsync(false);
    }
```

### DeleteFormFieldsAsync(List<FormFieldInfo>)
Asynchronously deletes specified form fields in the PDF Viewer.

#### Parameters
- List<[FormFieldInfo](./form-fields-enum-properties.md#formfieldinfo)> - `formFields` - form fields that need to be deleted.

#### Return Type
`Task`

#### Code Example
```csharp
    // We can bind the method for click action based the user requirement.
    // example demonstrates how to delete form fields.
    async Task DeleteFormFields()
    {
        List<FormFieldInfo> formFields = await pdfViewer.GetFormFieldsAsync();
        await pdfViewer.DeleteFormFieldsAsync(formFields);
    }
```

### DeleteFormFieldsAsync(List<string>)
Asynchronously deletes specified form fields in the PDF Viewer.

#### Parameters
- `List<[string]>` - `formFieldIds` - form fields that need to be deleted using the form fields id.

#### Return Type
`Task`

#### Code Example
```csharp
    // We can bind the method for click action based the user requirement.
    // example demonstrates how to delete form fields.
    List<string> formFieldIds = new List<string> { "field1", "field2" };

    async Task DeleteFormFields()
    {
        await pdfViewer.DeleteFormFieldsAsync(formFieldIds);
    }
```

### GetFormFieldsAsync()
Asynchronously retrieves form fields from the PDF Viewer.

#### Parameters
No Parameters

#### Return Type
`Task<List<FormFieldInfo>>`

#### Code Example
```csharp
    // We can bind the method for click action based the user requirement.
    // example demonstrates how to get form fields.
    async Task GetFormFields()
    {
        List<FormFieldInfo> formFields = await pdfViewer.GetFormFieldsAsync();
    }
```

### UpdateFormFieldsAsync(FormField)
Asynchronously updates specified form fields in the PDF Viewer.

#### Parameters
- [FormField](./form-fields-enum-properties.md#formfield) - `formField` - formField that needed to be edited

#### Return Type
`Task`

#### Code Example
```csharp
    // We can bind the method for click action based the user requirement.
    // example demonstrates how to edit form fields.
    async Task UpdateFormFields()
    {
       List<FormFieldInfo> formFields = await viewer.GetFormFieldsAsync();
       TextBoxField textBoxField = formFields[0] as TextBoxField;
       textBoxField.Value = "Updated";
       await viewer.UpdateFormFieldsAsync(new List<FormFieldInfo>(){
              textBoxField
       });
    }
```

### UpdateFormFieldsAsync(List<FormFieldInfo>)
Asynchronously updates specified form fields in the PDF Viewer.

#### Parameters
- List<[FormFieldInfo](./form-fields-enum-properties.md#formfieldinfo)> - `formField` - formField that needed to be edited

#### Return Type
`Task`

#### Code Example
```csharp
    // We can bind the method for click action based the user requirement.
    // example demonstrates how to edit form fields.
    async Task UpdateFormFields()
    {
       List<FormFieldInfo> formFields = await viewer.GetFormFieldsAsync();
       TextBoxField textBoxField = formFields[0] as TextBoxField;
       textBoxField.Value = "Updated";
       await viewer.UpdateFormFieldsAsync(new List<FormFieldInfo>(){
              textBoxField
       });
    }
```

### SelectFormFieldAsync(FormFieldInfo)
Asynchronously selects a form field in the PDF Viewer using a FormFieldInfo object.

#### Parameters
- [FormFieldInfo](./form-fields-enum-properties.md#formfieldinfo) - `formField` - formField that needed to be select.

#### Return Type
`Task`

#### Code Example
```csharp
    // We can bind the method for click action based the user requirement.
    // example demonstrates how to select form fields.
    async Task SelectFormField()
    {
       List<FormFieldInfo> formFields = await viewer.GetFormFieldsAsync();
       if (formFields.Count > 0)
       {
          FormFieldInfo formField = formFields[0];
          await pdfViewer.SelectFormFieldAsync(formField);
       }
    }
```

### SelectFormFieldAsync(string)
Asynchronously selects a form field in the PDF Viewer by its identifier.

#### Parameters
- `string` - `formFieldId` - formField that needed to be select using the form field id.

#### Return Type
`Task`

#### Code Example
```csharp
    // We can bind the method for click action based the user requirement.
    // example demonstrates how to select form fields.
    async Task SelectFormField()
    {
        await pdfViewer.SelectFormFieldAsync("formField1");
    }
```

### ExportFormFieldsAsync(FormFieldDataFormat)
Exports the form fields data as stream.

#### Parameters
- [FormFieldDataFormat](./general-enum-properties.md#formfielddataformat) - `formattype` - Represents the form fields data format.

#### Return Type
`Task<Stream>`

#### Code Example
```csharp
    // We can bind the method for click action based the user requirement.
    // example demonstrates how to export form fields.
    Stream stream = await pdfViewer.ExportFormFieldsAsync(FormFieldDataFormat.Json);
```

### ExportFormFieldsAsync(string)
Perform export action in the PDF Viewer.

#### Parameters
- `string` - `path` - Represents the form fields file exported to certain path.

#### Return Type
`Task`

#### Code Example
```csharp
    // We can bind the method for click action based the user requirement.
    // example demonstrates how to export form fields.
    await pdfViewer.ExportFormFieldsAsync(pathToSave);
```

### ExportFormFieldsAsObjectAsync()
Perform export form fields action in the PDF Viewer.

#### Parameters
No Parameters

#### Return Type
`Task<Dictionary<string, string>>`

#### Code Example
```csharp
    // We can bind the method for click action based the user requirement.
    // example demonstrates how to export form fields.
    Dictionary<string, string> formFields = await viewer.ExportFormFieldsAsObjectAsync();
```

### ImportFormFieldsAsync(Dictionary<string, string>)
Import the form fields.

#### Parameters
- `Dictionary<string, string>` - `formFields` - Form fields details name and value will be in Dictionary format.

#### Return Type
`Task`

#### Code Example
```csharp
    // We can bind the method for click action based the user requirement.
    // example demonstrates how to export form fields.
    await viewer.ExportFormFieldsAsObjectAsync(formFieldsDetails);
```

### ImportFormFieldsAsync(Stream, FormFieldDataFormat)
Import the form fields data into the current PDF document.

#### Parameters
- `Stream` - `data` - Represents the stream instance of form fields data.
- [FormFieldDataFormat](./general-enum-properties.md#formfielddataformat) - `formattype` - Represents the form fields data format.

#### Return Type
`Task`

#### Code Example
```csharp
    // We can bind the method for click action based the user requirement.
    // example demonstrates how to export form fields.
    await viewer.ExportFormFieldsAsObjectAsync(formFieldStream, FormFieldDataFormat.Json);
```

### RetrieveFormFieldsAsync()
To retrieve the form fields in the loaded PDF Document. | None | Task<List<FormField>> |

#### Parameters
No Parameters

#### Return Type
`Task`

#### Code Example
```csharp
    // We can bind the method for click action based the user requirement.
    // example demonstrates how to export form fields.
    List<FormField> formFields = await viewer.RetrieveFormFieldsAsync(formFieldStream, FormFieldDataFormat.Json);
```

