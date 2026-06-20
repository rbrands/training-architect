# Form Fields

Manage text, checkbox, and dropdown fields within documents for data collection and form-filling workflows.

> **Note:** All examples assume `using Syncfusion.Blazor.DocumentEditor;` is included in your Blazor component.

## Insert Form Field

Add a new form field (Text, CheckBox, or DropDown) at the current cursor position.

```csharp
// Insert Text form field
await container.DocumentEditor.Editor.InsertFormFieldAsync(FormFieldType.Text);

// Insert Checkbox form field
await container.DocumentEditor.Editor.InsertFormFieldAsync(FormFieldType.CheckBox);

// Insert Drop down form field
await container.DocumentEditor.Editor.InsertFormFieldAsync(FormFieldType.DropDown);
```

## Get Form Field Names

Retrieve all form field names in the current document.

```csharp
List<string> formFieldNames = await container.DocumentEditor.GetFormFieldNamesAsync();
```

## Export Form Field Data

Extract all form field values from the document.

```csharp
List<FormFieldData> formFieldData = await container.DocumentEditor.ExportFormDataAsync();
```

## Import Form Field Data

Pre-populate form fields with specific values programmatically.

```csharp
// Create individual form field data objects with field names and values
var textField = new FormFieldData { FieldName = "Text1", Value = "Hello World" };
var checkboxField = new FormFieldData { FieldName = "Check1", Value = true };
var dropdownField = new FormFieldData { FieldName = "Drop1", Value = 1 };

// Aggregate form data into a collection and import into document
List<FormFieldData> formData = new List<FormFieldData> { textField, checkboxField, dropdownField };
await container.DocumentEditor.ImportFormDataAsync(formData);
```

**Placeholders**
- `Text1` → Replace with {textFieldName}
- `Check1` → Replace with {checkboxFieldName}
- `Drop1` → Replace with {dropdownFieldName}
- `Hello World` → Replace with {textValue}
- `true` → Replace with {checkboxValue}
- `1` → Replace with {dropdownValue}

## Reset Form Fields

Clear all form fields in the document to their default values.

```csharp
await container.DocumentEditor.ResetFormFieldsAsync();
```

## Protect Document for Form Filling

Lock the document so users can only edit form fields, preventing changes to other content.

```csharp
// Enforce form-filling protection with password
await container.DocumentEditor.Editor.EnforceProtectionAsync("123", ProtectionType.FormFieldsOnly);

// Remove protection with password
await container.DocumentEditor.Editor.StopProtectionAsync("123");
```

```csharp
@using Syncfusion.Blazor.DocumentEditor

<button @onclick="protectDocument">Protection</button>
<SfDocumentEditorContainer @ref="container" EnableToolbar=true></SfDocumentEditorContainer>

@code {
    SfDocumentEditorContainer container;
    protected async void protectDocument(object args)
    {
        //enforce protection
        await container.DocumentEditor.Editor.EnforceProtectionAsync("123", ProtectionType.FormFieldsOnly);
        //stop the document protection
        await container.DocumentEditor.Editor.StopProtectionAsync("123");
    }
}
```

**Placeholders**
- `123` → Replace with {protectionPassword}

## Protection Type Options

- `NoProtection` – Document is fully editable
- `ReadOnly` – Document cannot be edited
- `FormFieldsOnly` – Only form fields can be edited
- `CommentsOnly` – Only comments can be added/edited
