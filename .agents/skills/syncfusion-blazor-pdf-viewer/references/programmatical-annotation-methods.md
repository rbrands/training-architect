# Programmatical Annotation Methods in Blazor SfPdfViewer Component

## Table of Contents
- [Overview](#overview)
- [Methods List](#methods-list)

## Overview

Use programmatic annotation methods when you need code-driven control over annotations instead of relying solely on UI interactions. These methods enable automated document review workflows, bulk annotation management, and integration with external systems.

## Methods List

### AddAnnotationAsync(PdfAnnotation)
Adds a PDF annotation to the PDF Viewer.

#### Parameters
- [PdfAnnotation](./pdf-annotation-enum-properties.md#pdfannotation) - `annotation`	- The PDF annotation to be added.

#### Return Type
`Task`

#### Code Example
```csharp
    // We can bind the method for click action based the user requirement.
    // example demonstrates how to create a new annotation of type AnnotationType.Line
    public async void AddAnnotation() 
    { 
        PdfAnnotation annotation = new PdfAnnotation(); 
        annotation.Type = AnnotationType.Line; 
        await viewer.AddAnnotationAsync(annotation); 
    }
```

### AddAnnotationAsync(PdfAnnotation, DynamicStampItem)
Adds a dynamic stamp PDF annotation to the PDF viewer.

#### Parameters
- [PdfAnnotation](./pdf-annotation-enum-properties.md#pdfannotation) - `annotation`	- The PDF annotation to be added.
- [DynamicStampItem](./annotation-enum-properties.md#dynamicstampitem) - `dynamicStampItem` - The dynamic stamp item associated with the annotation.

#### Return Type
`Task`

#### Code Example
```csharp
    // We can bind the method for click action based the user requirement.
    // example demonstrates how to create a new dynamic stamp annotation
    public async void AddAnnotation()
    {
        PdfAnnotation annotation = new PdfAnnotation();
        annotation.Type = AnnotationType.Stamp;
        await viewer.AddAnnotationAsync(annotation, DynamicStampItem.Approved); 
    }
```

### AddAnnotationAsync(PdfAnnotation, SignStampItem)
Adds a sign stamp PDF annotation to the PDF viewer.

#### Parameters
- [PdfAnnotation](./pdf-annotation-enum-properties.md#pdfannotation) - `annotation`	- The PDF annotation to be added.
- [SignStampItem](./annotation-enum-properties.md#signstampitem) - `signStampItem` - The sign stamp item associated with the annotation.

#### Return Type
`Task`

#### Code Example
```csharp
    // We can bind the method for click action based the user requirement.
    // example demonstrates how to create a new sign here stamp annotation
    public async void AddAnnotation()
    {
        PdfAnnotation annotation = new PdfAnnotation();
        annotation.Type = AnnotationType.Stamp;
        await viewer.AddAnnotationAsync(annotation, SignStampItem.Accepted);
    }
```

### AddAnnotationAsync(PdfAnnotation, StandardBusinessStampItem)
Adds a standard business stamp PDF annotation to the PDF viewer.

#### Parameters
- [PdfAnnotation](./pdf-annotation-enum-properties.md#pdfannotation) - `annotation`	- The PDF annotation to be added.
- [StandardBusinessStampItem](./annotation-enum-properties.md#standardbusinessstampitem) - `standardBusinessStampItem` - The standard business stamp item associated with the annotation.

#### Return Type
`Task`

#### Code Example
```csharp
    // We can bind the method for click action based the user requirement.
    // example demonstrates how to create a new standard business stamp annotation
    public async void AddAnnotation()
    {
        PdfAnnotation annotation = new PdfAnnotation();
        annotation.Type = AnnotationType.Stamp;
        await viewer.AddAnnotationAsync(annotation, StandardBusinessStampItem.Approved); 
    }
```

### AddAnnotationsAsync(List<PdfAnnotation>)
Adds a collection of PDF annotations to the PDF Viewer.

#### Parameters
- List<[PdfAnnotation](./pdf-annotation-enum-properties.md#pdfannotation)> - `annotationCollection` - The collection of PDF annotations to be added.

#### Return Type
`Task`

#### Code Example
```csharp
    // We can bind the method for click action based the user requirement.
    // example demonstrates how to create an annotation collection
    public async void AddAnnotations()
    {
        List<PdfAnnotation> annotations = new List<PdfAnnotation>();
        PdfAnnotation annotation = new PdfAnnotation();
        annotation.Type = AnnotationType.Line;
        annotations.Add(annotation);

        PdfAnnotation annotation1 = new PdfAnnotation();
        annotation1.Type = AnnotationType.Circle;
        annotations.Add(annotation1);

        await viewer.AddAnnotationsAsync(annotations);
    }
```

### AddPageRedactionsAsync(List<int>)
Asynchronously adds redaction annotations that cover the entire content of the specified pages in the PDF document.

#### Parameters
- `List<int>` - `PageNumbers` - A list of page numbers to which redaction annotations will be applied.

#### Return Type
`Task`

#### Code Example
```csharp
    // We can bind the method for click action based the user requirement.
    // example demonstrates how to add redaction annotation
    public async Task RedactSelectedPagesAsync()
    {
        List<int> pages = new List<int> { 1, 3, 5 };
        await viewer.AddPageRedactionsAsync(pages); // Adds redaction to pages 1, 3, and 5
    }
```

### EditAnnotationAsync(PdfAnnotation)
Edits a PDF annotation in the PDF Viewer.

#### Parameters
- [PdfAnnotation](./pdf-annotation-enum-properties.md#pdfannotation) - `annotation` - The PDF annotation to be edited.

#### Return Type
`Task`

#### Code Example
```csharp
    // We can bind the method for click action based the user requirement.
    // example demonstrates how to edit annotation
    public async void EditAnnotation() 
    { 
        List <PdfAnnotation> pdfAnnotations = await viewer.GetAnnotationsAsync();
        // Modify the properties of the annotation (e.g., opacity).
        pdfAnnotations[0].Opacity = 0.5; // Assuming that opacity is a value between 0 and 1.
        await viewer.EditAnnotationAsync(pdfAnnotations[0]); 
    }
```

### DeleteAnnotationAsync()
Deletes the selected annotation in the PDF Viewer.

#### Parameters
No Parameters

#### Return Type
`Task`

#### Code Example
```csharp
    // We can bind the method for click action based the user requirement.
    // example demonstrates how to selected delete annotation
    await viewer.DeleteAnnotationAsync();
```

### DeleteAnnotationAsync(PdfAnnotation)
Deletes a PDF annotation by its object.

#### Parameters
- [PdfAnnotation](./pdf-annotation-enum-properties.md#pdfannotation) - `annotation` - The PDF annotation to be deleted.

#### Return Type
`Task`

#### Code Example
```csharp
    // We can bind the method for click action based the user requirement.
    // example demonstrates how to selected delete annotation
    public async void DeleteAnnotation()
    {
        List<PdfAnnotation> pdfAnnotations = await viewer.GetAnnotationsAsync();
        await viewer.DeleteAnnotationAsync(pdfAnnotations[0]);
    }
```

### DeleteAnnotationAsync(string)
Deletes a PDF annotation by its identifier.

#### Parameters
- `string` - `annotationId`	- The identifier of the annotation to be deleted.

#### Return Type
`Task`

#### Code Example
```csharp
    // We can bind the method for click action based the user requirement.
    // example demonstrates how to selected delete annotation
    public async void DeleteAnnotation()
    {
        List<PdfAnnotation> pdfAnnotations = await viewer.GetAnnotationsAsync();
        string deleteAnnotationId = pdfAnnotations[0].Id;
        await viewer.DeleteAnnotationAsync(deleteAnnotationId);
    }
```

### DeleteAnnotationsAsync()
Deletes all annotation collections in the PDF Document.

#### Parameters
No Parameters

#### Return Type
`Task`

#### Code Example
```csharp
    // We can bind the method for click action based the user requirement.
    // example demonstrates how to selected delete annotations
    await viewer.DeleteAnnotationsAsync();
```

### DeleteAnnotationsAsync(List<PdfAnnotation>)
Deletes a collection of PDF annotations from the PDF Viewer.

#### Parameters
- List<[PdfAnnotation](./pdf-annotation-enum-properties.md#pdfannotation)> - `annotationCollection` - The collection of PDF annotations to be deleted.

#### Return Type
`Task`

#### Code Example
```csharp
    // We can bind the method for click action based the user requirement.
    // example demonstrates how to selected delete annotations
    public async void DeleteAnnotations()
    {
        List<PdfAnnotation> annotationCollection = await viewer.GetAnnotationsAsync();
        await viewer.DeleteAnnotationsAsync(annotationCollection);
    }
```

### DeleteAnnotationsAsync(List<string>)
Deletes a list of annotations by their IDs.

#### Parameters
- `List<string>` - `annotationsId` - A list of string IDs representing the annotations to be deleted. Each ID corresponds to a specific annotation in the PDF document.

#### Return Type
`Task`

#### Code Example
```csharp
    // We can bind the method for click action based the user requirement.
    // example demonstrates how to selected delete annotations
    public async void DeleteAnnotations()
    {
        List<PdfAnnotation> pdfAnnotations = await viewer.GetAnnotationsAsync();
        List<string> annotationsId = new List<string> { pdfAnnotations[0].Id, pdfAnnotations[1].Id, pdfAnnotations[2].Id };
        await viewer.DeleteAnnotationsAsync(annotationsId);
    }
```

### GetAnnotationsAsync()
Gets the annotations collection from the PDF document.

#### Parameters
No Parameters

#### Return Type
`Task`

#### Code Example
```csharp
    // We can bind the method for click action based the user requirement.
    // example demonstrates how to get annotations
    List<PdfAnnotation> pdfAnnotations = await viewer.GetAnnotationsAsync();
```

### SelectAnnotationAsync(PdfAnnotation)
Selects an annotation using its object.

#### Parameters
- [PdfAnnotation](./pdf-annotation-enum-properties.md#pdfannotation) - `annotation` - Annotation object to be select.

#### Return Type
`Task`

#### Code Example
```csharp
    // We can bind the method for click action based the user requirement.
    // example demonstrates how to select annotation
    public async void DeleteAnnotation()
    {
        List<PdfAnnotation> pdfAnnotations = await viewer.GetAnnotationsAsync();
        await viewer.SelectAnnotationAsync(pdfAnnotations[0]);
    }
```

### SelectAnnotationAsync(string)
Selects an annotation using its identifier.

#### Parameters
- `string` - `annotationId` - Annotation object to be select by using id.

#### Return Type
`Task`

#### Code Example
```csharp
    // We can bind the method for click action based the user requirement.
    // example demonstrates how to select annotations
    public async void DeleteAnnotation()
    {
        List<PdfAnnotation> pdfAnnotations = await viewer.GetAnnotationsAsync();
        string deleteAnnotationId = pdfAnnotations[0].Id;
        await viewer.SelectAnnotationAsync(deleteAnnotationId);
    }
```

### ClearSelectionAsync()
Clears the annotation selection in the PDF Viewer.

#### Parameters
No Parameters

#### Return Type
`Task`

#### Code Example
```csharp
    // We can bind the method for click action based the user requirement.
    // example demonstrates how to clear select annotations
    await viewer.ClearSelectionAsync();
```

### RedactAsyn()
Asynchronously applies redaction to all Redaction annotations in the PDF document.

#### Parameters
No Parameters

#### Return Type
`Task`

#### Code Example
```csharp
    // We can bind the method for click action based the user requirement.
    // example demonstrates how to clear select annotations
    await viewer.RedactAsyn();
```

### ExportAnnotationAsync(AnnotationDataFormat)
Exports annotations from the PDF document.

#### Parameters
- [AnnotationDataFormat](./general-enum-properties.md#annotationdataformat) - `annotationDataFormat` - set the annotation format that need to be exported

#### Return Type
`Task`

#### Code Example
```csharp
    // We can bind the method for click action based the user requirement.
    // example demonstrates how to export annotation
    await viewer.ExportAnnotationAsync(AnnotationDataFormat.Json);
```

### ExportAnnotationAsStreamAsync(AnnotationDataFormat)
Exports annotations as a stream.

#### Parameters
- [AnnotationDataFormat](./general-enum-properties.md#annotationdataformat) - `annotationDataFormat` - set the annotation format that need to be exported

#### Return Type
`Task<Stream>`

#### Code Example
```csharp
    // We can bind the method for click action based the user requirement.
    // example demonstrates how to export annotation
    Stream stream = await viewer.ExportAnnotationAsStreamAsync(AnnotationDataFormat.Json);
```

### ExportAnnotationsAsObjectAsync()
Exports annotations as an object.

#### Parameters
No Parameters

#### Return Type
`Task<object>`

#### Code Example
```csharp
    // We can bind the method for click action based the user requirement.
    // example demonstrates how to export annotation
    object annotationObject = await viewer.ExportAnnotationsAsObjectAsync();
```

### ImportAnnotationAsync(Stream, AnnotationDataFormat)
Imports annotations from a stream.

#### Parameters
- `Stream` - `importData` - Imported annotation data.
- [AnnotationDataFormat](./general-enum-properties.md#annotationdataformat) - `annotationDataFormat` - set the annotation format that need to be import

#### Return Type
`Task`

#### Code Example
```csharp
    // We can bind the method for click action based the user requirement.
    // example demonstrates how to import annotation
    await viewer.ImportAnnotationAsync(annotationStream, AnnotationDataFormat.Json);
```

### ImportAnnotationAsync(object, AnnotationDataFormat)
Imports annotations from an object.

#### Parameters
- `object` - `importData` - Imported annotation data.
- [AnnotationDataFormat](./general-enum-properties.md#annotationdataformat) - `annotationDataFormat` - set the annotation format that need to be import

#### Return Type
`Task`

#### Code Example
```csharp
    // We can bind the method for click action based the user requirement.
    // example demonstrates how to import annotation
    await viewer.ImportAnnotationAsync(annotationObject, AnnotationDataFormat.Json);
```
