# Form Designer Events in Blazor SfPdfViewer Component

## Table of Contents
- [Overview](#overview)
- [When to Use Form Designer Events](#when-to-use-form-designer-events)
- [Complete Event List](#complete-event-list)

## Overview

Form Designer events notify your application when users interact with form fields in the PDF document. Use these events to track form field creation, modification, deletion, validation, focus changes, and import/export operations. These events enable custom workflows, validation rules, data binding, and audit logging for form interactions.

## When to Use Form Designer Events

**Guide users to subscribe to form designer events when they need to:**
- It allowing users to handle interactions, changes, and actions performed within the viewer for form fields.
- When we do such action while form fields interaction process, the we can use the form fields events to handle to acheive those scenaios.

## Complete Event List

**Below is the comprehensive reference of all form designer events, their arguments, and properties. Use this as a lookup when you know which event you need.**

### FormFieldAdding
Triggered before a new form field is added, allowing validation before insertion.

#### Value
`EventCallback<FormFieldAddEventArgs>`

#### FormFieldAdding Events Arguments
| Event Arguments | Data Type | Description |
|-----|-----|-----|
| `Field` | [FormFieldInfo](./form-fields-enum-properties.md#formfieldinfo) | Gets the form field object that was added or is being added in the SfPdfViewer2 component. |
| `PageNumber` | int | Gets the page number where the form field was added or is being added in the SfPdfViewer2 component. |
| `Cancel` | bool | Gets or sets a value indicating whether the addition of a form field should be canceled in the SfPdfViewer2 component. The default value is false. |

#### Code Example
```razor
<SfPdfViewer2>
    <PdfViewerEvents FormFieldAdding="FormFieldAdding"></PdfViewerEvents>
</SfPdfViewer2>
@code {
    private void FormFieldAdding(FormFieldAddEventArgs args) {
        // customize your code in based on the requirements and check above table for event argument details
    }
}
```

### FormFieldAdded
Triggered when a form field is added to the PDF document.

#### Value
`EventCallback<FormFieldAddedEventArgs>`

#### FormFieldAdded Events Arguments
| Event Arguments | Data Type | Description |
|-----|-----|-----|
| `Field` | [FormFieldInfo](./form-fields-enum-properties.md#formfieldinfo) | Gets the form field object that was added or is being added in the SfPdfViewer2 component. |
| `PageNumber` | int | Gets the page number where the form field was added or is being added in the SfPdfViewer2 component. |

#### Code Example
```razor
<SfPdfViewer2>
    <PdfViewerEvents FormFieldAdded="FormFieldAdded"></PdfViewerEvents>
</SfPdfViewer2>
@code {
    private void FormFieldAdded(FormFieldAddedEventArgs args) {
        // customize your code in based on the requirements and check above table for event argument details
    }
}
```

### FormFieldDeleted
Triggered when a form field is removed from the document.

#### Value
`EventCallback<FormFieldDeletedEventArgs>`

#### FormFieldDeleted Events Arguments
| Event Arguments | Data Type | Description |
|-----|-----|-----|
| `Field` | [FormFieldInfo](./form-fields-enum-properties.md#formfieldinfo) | Gets the form field object that was deleted in the SfPdfViewer2 component. |
| `PageNumber` | int | Gets the page number where the form field was deleted in the SfPdfViewer2 component. |

#### Code Example
```razor
<SfPdfViewer2>
    <PdfViewerEvents FormFieldDeleted="FormFieldDeleted"></PdfViewerEvents>
</SfPdfViewer2>
@code {
    private void FormFieldDeleted(FormFieldDeletedEventArgs args) {
        // customize your code in based on the requirements and check above table for event argument details
    }
}
```

### FormFieldClick
Triggered when a user clicks on a form field while designer mode is off.

#### Value
`EventCallback<FormFieldClickArgs>`

#### FormFieldClick Events Arguments
| Event Arguments | Data Type | Description |
|-----|-----|-----|
| `Cancel` | bool | If it is set as true, signature panel does not open for signature field. By default it is false. |
| `Field` | [FormField](./form-fields-enum-properties.md#formfield) | Gets the form field object. |
| `FormField` | [FormFieldInfo](./form-fields-enum-properties.md#formfieldinfo) | Gets information about the clicked form field in SfPdfViewer2. |
| `Name` | string | Specifies name of the event. |

#### Code Example
```razor
<SfPdfViewer2>
    <PdfViewerEvents FormFieldClick="FormFieldClicked"></PdfViewerEvents>
</SfPdfViewer2>
@code {
    private void FormFieldClicked(FormFieldClickArgs args) {
        // customize your code in based on the requirements and check above table for event argument details
    }
}
```

### FormFieldDoubleClick
Triggered when a form field is double-clicked.

#### Value
`EventCallback<FormFieldDoubleClickEventArgs>`

#### FormFieldDoubleClick Events Arguments
| Event Arguments | Data Type | Description |
|-----|-----|-----|
| `Cancel` | bool | Gets or sets a value indicating whether the form field property panel should be prevented from opening in the SfPdfViewer2 component. |
| `Field` | [FormFieldInfo](./form-fields-enum-properties.md#formfieldinfo) | Gets the form field object that was double-clicked in the SfPdfViewer2 component. |

#### Code Example
```razor
<SfPdfViewer2>
    <PdfViewerEvents FormFieldDoubleClick="FormFieldDoubleClicked"></PdfViewerEvents>
</SfPdfViewer2>
@code {
    private void FormFieldDoubleClicked(FormFieldDoubleClickEventArgs args) {
        // customize your code in based on the requirements and check above table for event argument details
    }
}
```

### FormFieldSelected
Triggered when a form field is selected.

#### Value
`EventCallback<FormFieldSelectedEventArgs>`

#### FormFieldSelected Events Arguments
| Event Arguments | Data Type | Description |
|-----|-----|-----|
| `Field` | [FormFieldInfo](./form-fields-enum-properties.md#formfieldinfo) | Gets the form field object that was selected in the SfPdfViewer2 component. |
| `IsProgrammaticSelection` | bool | Gets a value indicating whether the form field selection was triggered programmatically in the SfPdfViewer2 component. |
| `PageNumber` | int | Gets the page number where the form field was selected in the SfPdfViewer2 component. |

#### Code Example
```razor
<SfPdfViewer2>
    <PdfViewerEvents FormFieldSelected="FormFieldSelected"></PdfViewerEvents>
</SfPdfViewer2>
@code {
    private void FormFieldSelected(FormFieldSelectedEventArgs args) {
        // customize your code in based on the requirements and check above table for event argument details
    }
}
```

### FormFieldUnselected
Triggered when a form field is unselected.

#### Value
`EventCallback<FormFieldUnselectedEventArgs>`

#### FormFieldUnselected Events Arguments
| Event Arguments | Data Type | Description |
|-----|-----|-----|
| `Field` | [FormFieldInfo](./form-fields-enum-properties.md#formfieldinfo) - Gets the form field object that was unselected in the SfPdfViewer2 component. |
| `PageNumber` | int | Gets the page number where the form field was unselected in the SfPdfViewer2 component. |

#### Code Example
```razor
<SfPdfViewer2>
    <PdfViewerEvents FormFieldUnselected="FormFieldUnselected"></PdfViewerEvents>
</SfPdfViewer2>
@code {
    private void FormFieldUnselected(FormFieldUnselectedEventArgs args) {
        // customize your code in based on the requirements and check above table for event argument details
    }
}
```

### FormFieldResized
Triggered when a form field is resized.

#### Value
`EventCallback<FormFieldResizedEventArgs>`

#### FormFieldResized Events Arguments
| Event Arguments | Data Type | Description |
|-----|-----|-----|
| `Field` | [FormFieldInfo](./form-fields-enum-properties.md#formfieldinfo) - Gets the form field object that was resized in the SfPdfViewer2 component. |
| `NewBounds` | [Bound](./annotation-general-enum-properties.md#bound) | Gets the updated bounds of the form field after resized in the SfPdfViewer2 component. |
| `OldBounds` | [Bound](./annotation-general-enum-properties.md#bound) | Gets the previous bounds of the form field before resizing in the SfPdfViewer2 component. |
| `PageNumber` | int | Gets the page number where the form field was resized in the SfPdfViewer2 component. |

#### Code Example
```razor
<SfPdfViewer2>
    <PdfViewerEvents FormFieldResized="FormFieldResized"></PdfViewerEvents>
</SfPdfViewer2>
@code {
    private void FormFieldResized(FormFieldResizedEventArgs args) {
        // customize your code in based on the requirements and check above table for event argument details
    }
}
```

### ValidateFormFields
Triggered when form fields are validated.

#### Value
`EventCallback<ValidateFormFieldsArgs>`

#### ValidateFormFields Events Arguments
| Event Arguments | Data Type | Description |
|-----|-----|-----|
| `DocumentName` | string | Gets the document name to be loaded into PdfViewer. |
| `Fields` | List<[FormFieldInfo](./form-fields-enum-properties.md#formfieldinfo)> | Gets the collection of form fields to be validated in SfPdfViewer2 component. |
| `FormFields` | List<[FormField](./form-fields-enum-properties.md#formfield)> | The form fields object from PDF document being loaded. |
| `UnfilledFields` | Dictionary<string, object> | Gets a collection of form fields that have not been filled in the SfPdfViewer2 component. |
| `Name` | string | Specifies name of the event. |

#### Code Example
```razor
<SfPdfViewer2>
    <PdfViewerEvents ValidateFormFields="ValidatedFormFields"></PdfViewerEvents>
</SfPdfViewer2>
@code {
    private void ValidatedFormFields(ValidateFormFieldsArgs args) {
        // customize your code in based on the requirements and check above table for event argument details
    }
}
```

### FormFieldFocusIn
Triggered when focus enters a form field.

#### Value
`EventCallback<FormFieldFocusInEventArgs>`

#### FormFieldFocusIn Events Arguments
| Event Arguments | Data Type | Description |
|-----|-----|-----|
| `Field` | [FormFieldInfo](./form-fields-enum-properties.md#formfieldinfo) - Gets the form field object that received focus in the SfPdfViewer2 component. |
| `PageNumber` | int | Gets the page number where the form field received focus in the SfPdfViewer2 component. |

#### Code Example
```razor
<SfPdfViewer2>
    <PdfViewerEvents FormFieldFocusIn="FormFieldFocusedIn"></PdfViewerEvents>
</SfPdfViewer2>
@code {
    private void FormFieldFocusedIn(FormFieldFocusInEventArgs args) {
        // customize your code in based on the requirements and check above table for event argument details
    }
}
```

### FormFieldFocusOut
Triggered when focus leaves a form field.

#### Value
`EventCallback<FormFieldFocusOutEventArgs>`

#### FormFieldFocusOut Events Arguments
| Event Arguments | Data Type | Description |
|-----|-----|-----|
| `Field` | [FormFieldInfo](./form-fields-enum-properties.md#formfieldinfo) | Gets the form field object that lost focus in the SfPdfViewer2 component. |
| `PageNumber` | int | Gets the page number where the form field lost focus in the SfPdfViewer2 component. |

#### Code Example
```razor
<SfPdfViewer2>
    <PdfViewerEvents FormFieldFocusOut="FormFieldFocusedOut"></PdfViewerEvents>
</SfPdfViewer2>
@code {
    private void FormFieldFocusedOut(FormFieldFocusOutEventArgs args) {
        // customize your code in based on the requirements and check above table for event argument details
    }
}
```

### FormFieldMouseEnter
Triggered when the mouse hovers over a form field.

#### Value
`EventCallback<FormFieldMouseEnterEventArgs>`

#### FormFieldMouseEnter Events Arguments
| Event Arguments | Data Type | Description |
|-----|-----|-----|
| `Field` | [FormFieldInfo](./form-fields-enum-properties.md#formfieldinfo) | Gets the form field object that is being mouser enter in the SfPdfViewer2 component. |
| `PageNumber` | int | Gets the page number of the form field being mouse entered in the SfPdfViewer2 component. |
| `PageX` | double | Gets the X-coordinate of the mouse pointer relative to the page container in the SfPdfViewer2 component. |
| `PageY` | double | Gets the Y-coordinate of the mouse pointer relative to the page container in the SfPdfViewer2 component. |
| `X` | double | Gets the X-coordinate of the mouse pointer relative to the viewer container in the SfPdfViewer2 component. |
| `Y` | double | Gets the Y-coordinate of the mouse pointer relative to the viewer container in the SfPdfViewer2 component. |

#### Code Example
```razor
<SfPdfViewer2>
    <PdfViewerEvents FormFieldMouseEnter="FormFieldMouseEntered"></PdfViewerEvents>
</SfPdfViewer2>
@code {
    private void FormFieldMouseEntered(FormFieldMouseEnterEventArgs args) {
        // customize your code in based on the requirements and check above table for event argument details
    }
}
```

### FormFieldMouseLeave
Triggered when the mouse leaves a form field.

#### Value
`EventCallback<FormFieldMouseLeaveEventArgs>`

#### FormFieldMouseLeave Events Arguments
| Event Arguments | Data Type | Description |
|-----|-----|-----|
| `Field` | [FormFieldInfo](./form-fields-enum-properties.md#formfieldinfo) | Gets the form field object that the mouse pointer exited in the SfPdfViewer2 component. |
| `PageNumber` | int | Gets the page number where the form field was located when the mouse pointer exited in the SfPdfViewer2 component. |

#### Code Example
```razor
<SfPdfViewer2>
    <PdfViewerEvents FormFieldMouseLeave="FormFieldMouseLeave"></PdfViewerEvents>
</SfPdfViewer2>
@code {
    private void FormFieldMouseLeave(FormFieldMouseLeaveEventArgs args) {
        // customize your code in based on the requirements and check above table for event argument details
    }
}
```

### FormFieldPropertyChanged
Triggered when a form field’s properties are modified.

#### Value
`EventCallback<FormFieldPropertyChangedEventArgs>`

#### FormFieldPropertyChanged Events Arguments
| Event Arguments | Data Type | Description |
|-----|-----|-----|
| `ChangedProperties` | List<string> | Gets the list of form field properties that have changed in the SfPdfViewer2 component. |
| `NewValue` | [FormFieldInfo](./form-fields-enum-properties.md#formfieldinfo) | Gets the updated state of the form field after the property change in the SfPdfViewer2 component. |
| `OldValue` | [FormFieldInfo](./form-fields-enum-properties.md#formfieldinfo) | Gets the previous state of the form field before the property change in the SfPdfViewer2 component. |

#### Code Example
```razor
<SfPdfViewer2>
    <PdfViewerEvents FormFieldPropertyChanged="FormFieldPropertyChanged"></PdfViewerEvents>
</SfPdfViewer2>
@code {
    private void FormFieldPropertyChanged(FormFieldPropertyChangedEventArgs args) {
        // customize your code in based on the requirements and check above table for event argument details
    }
}
```

### FormFieldsExporting
Triggered before form fields are exported, allowing customization of the export process.

#### Value
`EventCallback<FormFieldsExportEventArgs>`

#### FormFieldsExporting Events Arguments
| Event Arguments | Data Type | Description |
|-----|-----|-----|
| `Cancel` | bool | Gets or sets a value indicating whether the form fields export operation should be canceled in the SfPdfViewer2 component. |

#### Code Example
```razor
<SfPdfViewer2>
    <PdfViewerEvents FormFieldsExporting="FormFieldsExporting"></PdfViewerEvents>
</SfPdfViewer2>
@code {
    private void FormFieldsExporting(FormFieldsExportEventArgs args) {
        // customize your code in based on the requirements and check above table for event argument details
    }
}
```

### FormFieldsImporting
Triggered before form fields are imported, allowing validation or modifications.

#### Value
`EventCallback<FormFieldsImportEventArgs>`

#### FormFieldsImporting Events Arguments
| Event Arguments | Data Type | Description |
|-----|-----|-----|
| `Cancel` | bool | Gets or sets a value indicating whether the form field import operation should be canceled in the SfPdfViewer2 component. |

#### Code Example
```razor
<SfPdfViewer2>
    <PdfViewerEvents FormFieldsImporting="FormFieldsImporting"></PdfViewerEvents>
</SfPdfViewer2>
@code {
    private void FormFieldsImporting(FormFieldsImportEventArgs args) {
        // customize your code in based on the requirements and check above table for event argument details
    }
}
```

### FormFieldsExported
Triggered when form fields are successfully exported.

#### Value
`EventCallback<FormFieldsExportedEventArgs>`

#### FormFieldsImporting Events Arguments
No Event Arguments

#### Code Example
```razor
<SfPdfViewer2>
    <PdfViewerEvents FormFieldsExported="FormFieldsExported"></PdfViewerEvents>
</SfPdfViewer2>
@code {
    private void FormFieldsExported(FormFieldsExportedEventArgs args) {
        // customize your code in based on the requirements and check above table for event argument details
    }
}
```

### FormFieldsImported
Triggered when form fields are successfully imported. 

#### Value
`EventCallback<FormFieldsImportedEventArgs>`

#### FormFieldsImporting Events Arguments
No Event Arguments

#### Code Example
```razor
<SfPdfViewer2>
    <PdfViewerEvents FormFieldsImported="FormFieldsImported"></PdfViewerEvents>
</SfPdfViewer2>
@code {
    private void FormFieldsImported(FormFieldsImportedEventArgs args) {
        // customize your code in based on the requirements and check above table for event argument details
    }
}
```

### FormFieldsExportFailed
Triggered when form fields export operation fails.

#### Value
`EventCallback<FormFieldsExportFailedEventArgs>`

#### FormFieldsExportFailed Events Arguments
| Event Arguments | Data Type | Description |
|-----|-----|-----|
| `ErrorDetails` | string | Gets the error details associated with the failure of the form fields export process in the SfPdfViewer2 component. |

#### Code Example
```razor
<SfPdfViewer2>
    <PdfViewerEvents FormFieldsExportFailed="FormFieldsExportFailed"></PdfViewerEvents>
</SfPdfViewer2>
@code {
    private void FormFieldsExportFailed(FormFieldsExportFailedEventArgs args) {
        // customize your code in based on the requirements and check above table for event argument details
    }
}
```

### FormFieldsImportFailed
Triggered when form fields import operation fails.

#### Value
`EventCallback<FormFieldsImportFailedEventArgs>`

#### FormFieldsImportFailed Events Arguments
| Event Arguments | Data Type | Description |
|-----|-----|-----|
| `ErrorDetails` | string | Gets the error details associated with the failure of the form fields import process in the SfPdfViewer2 component. |

#### Code Example
```razor
<SfPdfViewer2>
    <PdfViewerEvents FormFieldsImportFailed="FormFieldsImportFailed"></PdfViewerEvents>
</SfPdfViewer2>
@code {
    private void FormFieldsImportFailed(FormFieldsImportFailedEventArgs args) {
        // customize your code in based on the requirements and check above table for event argument details
    }
}
```

### IsDesignerModeChanged 
Triggered when the designer mode state changes in the PDF Viewer. This event should declared in the `SfPdfViewer2` tag.

#### Value
`EventCallback<bool>`

#### FormFieldsImportFailed Events Arguments
No Event Arguments

#### Code Example
```razor
<SfPdfViewer2 IsDesignerModeChanged="IsDesignerModeChanged"> 
</SfPdfViewer2>
@code {
    private void IsDesignerModeChanged() {
        // customize your code in based on the requirements.
    }
}
```
