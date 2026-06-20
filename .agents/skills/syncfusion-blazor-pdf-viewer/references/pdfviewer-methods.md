# PDFViewer Methods

## Table of Contents
- [Overview](#overview)
- [Methods](#methods)

## Overview

This document combines the core methods and general properties available on the Blazor `SfPdfViewer` component. Use the methods to control lifecycle and document operations, and configure properties to control appearance, loading, fonts, and interaction behavior.

## Methods

### Dispose()
Dispose the PDF Viewer and its dependencies. Releases unmanaged resources immediately.

#### Parameters
No Parameters

#### Return Type
`void`

#### Code Example
```csharp
    // We can bind the method for click action based the user requirement.
    viewer.Dispose();
```

### Dispose(bool)
Dispose the unmanaged resources from PDF Viewer component. Protected disposal method with disposing parameter for managing both managed and unmanaged resources.

#### Parameters
- `bool` - `disposing` - Indicates whether to dispose managed resources.

#### Return Type
`void`

#### Code Example
```csharp
    // We can bind the method for click action based the user requirement.
    viewer.Dispose(true);
```

### GetDocumentAsync()
Gets the loaded PDF document with the changes.

#### Parameters
No Parameters

#### Return Type
`Task`

#### Code Example
```csharp
    // We can bind the method for click action based the user requirement.
    byte[] documentBytes = await viewer.GetDocumentAsync();
```

### LoadAsync(byte[], string)
Load a PDF Document from the byte array. All pages from the byte array are loaded into the viewer.

#### Parameters
- `byte[]` - `bytes` - Byte array of the PDF document to be loaded.
- `string` - `password` - Optional password for encrypted PDF (null if not applicable)

#### Return Type
`Task`

#### Code Example
```csharp
    // We can bind the method for click action based the user requirement.
    await viewer.LoadAsync(documentbytes);
```

### LoadAsync(Stream, string)
Load a PDF Document from the stream. All pages from the stream are loaded into the viewer.

#### Parameters
- `Stream` - `stream` - Stream of the PDF document to be loaded
- `string` - `password` - Optional password for encrypted PDF (null if not applicable)

#### Return Type
`Task`

#### Code Example
```csharp
    // We can bind the method for click action based the user requirement.
    await viewer.LoadAsync(documentstream);
```

### LoadAsync(string, string)
Loads the given PDF document in the PDF Viewer by document path. Supports both file paths and web URLs.

#### Parameters
- `string` - `document` - File path or URL of the PDF document to load
- `string` - `password` - Optional password for encrypted PDF (null if not applicable)

#### Return Type
`Task`

#### Code Example
```csharp
    // We can bind the method for click action based the user requirement.
    await viewer.LoadAsync(documentstring);
```

### SaveAsync(string)
Save the loaded document with the changes in the given file path. The file path must include the .pdf extension.

#### Parameters
- `string` - `filePath` - File path where the document will be saved.

#### Return Type
`Task`

#### Code Example
```csharp
    // We can bind the method for click action based the user requirement.
    await viewer.SaveAsync(documentsavepath);
```

### UnloadAsync()
Unloads the PDF document being displayed in the PDF viewer. Clears the current document from memory.

#### Parameters
No Parameters

#### Return Type
`Task`

#### Code Example
```csharp
    // We can bind the method for click action based the user requirement.
    await viewer.UnloadAsync();
```

### UpdateViewerContainerAsync()
Updates the PDF Viewer container width and height from external changes. Useful when container dimensions change dynamically.

#### Parameters
No Parameters

#### Return Type
`Task`

#### Code Example
```csharp
    // We can bind the method for click action based the user requirement.
    await viewer.UpdateViewerContainerAsync();
```

