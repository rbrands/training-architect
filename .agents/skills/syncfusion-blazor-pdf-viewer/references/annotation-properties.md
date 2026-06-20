# Syncfusion Blazor PDF Viewer - Annotation Properties

## Table of Contents
- [Overview](#overview)
- [Properties List](#properties-list)

## Overview

The Syncfusion Blazor PDF Viewer provides comprehensive annotation support that allows users to interact with PDF documents through various annotation tools. The annotation properties control which annotation features are enabled or disabled, and how they behave within the PDF Viewer component.

## Properties List
The following APIs provides the annotation-related properties available in the Syncfusion Blazor PDF Viewer:

### EnableAnnotation
If set to false, annotation support in the PDF Viewer will be disabled. By default it is true.
- `false` - Disabled the annotation module from the PDF viewer and don't get interact with the annotation. It won't hide the annotation button from UI, it disables the button.
- `true` - Enabled the annotation module from the PDF Viewer and also enables the button in the UI.

#### Data Type and default value
`bool` - `true(Default)`

#### Code Example
```razor
<SfPdfViewer2 EnableAnnotation="true"></SfPdfViewer2>
```

### EnableFreeText
If set to false, the free text annotation tool in the annotation toolbar will be disabled. By default it is true.
- `false` - Disabled the free text annotation from the PDF viewer and don't get interact with the annotation. It won't hide the free text annotation button from UI, it disables the button.
- `true` - Enabled the free text annotation from the PDF Viewer and also enables the button in the UI.

#### Data Type and default value
`bool` - `true(Default)`

#### Code Example
```razor
<SfPdfViewer2 EnableFreeText="true"></SfPdfViewer2>
```

### EnableHandwrittenSignature
If set to false, the handwritten signature tool in the annotation toolbar will be disabled. By default it is true.
- `false` - Disabled the handwritten signature annotation from the PDF viewer and don't get interact with the annotation. It won't hide the handwritten signature annotation button from UI, it disables the button.
- `true` - Enabled the handwritten signature annotation from the PDF Viewer and also enables the button in the UI.

#### Data Type and default value
`bool` - `true(Default)`

#### Code Example
```razor
<SfPdfViewer2 EnableHandwrittenSignature="true"></SfPdfViewer2>
```

### EnableImportAnnotationMeasurement
Enable or disables the customization of measure values in PdfViewer.

#### Data Type and default value
`bool` - `true(Default)`

#### Code Example
```razor
<SfPdfViewer2 EnableImportAnnotationMeasurement="true"></SfPdfViewer2>
```

### EnableInkAnnotation
If set to false, ink annotation support in the PDF Viewer will be disabled. By default it is true.
- `false` - Disabled the ink annotation from the PDF viewer and don't get interact with the annotation. It won't hide the ink annotation button from UI, it disables the button.
- `true` - Enabled the ink annotation from the PDF Viewer and also enables the button in the UI.

#### Data Type and default value
`bool` - `true(Default)`

#### Code Example
```razor
<SfPdfViewer2 EnableInkAnnotation="true"></SfPdfViewer2>
```

### EnableMeasureAnnotation
If set to false, the measurement annotation tool in the annotation toolbar will be disabled. By default it is true.
- `false` - Disabled the measurement annotation from the PDF viewer and don't get interact with the annotation. It won't hide the measurement annotation button from UI, it disables the button.
- `true` - Enabled the measurement annotation from the PDF Viewer and also enables the button in the UI.

#### Data Type and default value
`bool` - `true(Default)`

#### Code Example
```razor
<SfPdfViewer2 EnableMeasureAnnotation="true"></SfPdfViewer2>
```

### EnableShapeAnnotation
If set to false, the shape annotation tool in the annotation toolbar will be disabled. By default it is true.
- `false` - Disabled the shape annotation from the PDF viewer and don't get interact with the annotation. It won't hide the shape annotation button from UI, it disables the button.
- `true` - Enabled the shape annotation from the PDF Viewer and also enables the button in the UI.

#### Data Type and default value
`bool` - `true(Default)`

#### Code Example
```razor
<SfPdfViewer2 EnableShapeAnnotation="true"></SfPdfViewer2>
```

### EnableStampAnnotations
If set to false, the stamp annotation tool in the annotation toolbar will be disabled. By default it is true.
- `false` - Disabled the stamp annotation from the PDF viewer and don't get interact with the annotation. It won't hide the stamp annotation button from UI, it disables the button.
- `true` - Enabled the stamp annotation from the PDF Viewer and also enables the button in the UI.

#### Data Type and default value
`bool` - `true(Default)`

#### Code Example
```razor
<SfPdfViewer2 EnableStampAnnotations="true"></SfPdfViewer2>
```

### EnableStickyNotesAnnotation
If set to false, the sticky notes annotation tool in the toolbar will be disabled. By default it is true.
- `false` - Disabled the sticky notes annotation from the PDF viewer and don't get interact with the annotation. It won't hide the sticky notes annotation button from UI, it disables the button.
- `true` - Enabled the sticky notes annotation from the PDF Viewer and also enables the button in the UI.

#### Data Type and default value
`bool` - `true(Default)`

#### Code Example
```razor
<SfPdfViewer2 EnableStickyNotesAnnotation="true"></SfPdfViewer2>
```

### EnableTextMarkupAnnotation
If set to false, the text markup tools (highlight, strikethrough, squiggly and underline) are disabled in annotation toolbar. By default it is true.
- `false` - Disabled the text markup annotation from the PDF viewer and don't get interact with the annotation. It won't hide the text markup annotation button from UI, it disables the button.
- `true` - Enabled the text markup annotation from the PDF Viewer and also enables the button in the UI.

#### Data Type and default value
`bool` - `true(Default)`

#### Code Example
```razor
<SfPdfViewer2 EnableTextMarkupAnnotation="true"></SfPdfViewer2>
```

### ExportAnnotationFileName
Gets or sets the file name of annotations to be exported. When we set the file name through this API, it will set the file name for the exported file.

#### Data Type
`string`

#### Code Example
```razor
<SfPdfViewer2 ExportAnnotationFileName="ExportFile"></SfPdfViewer2>
```

### IsSignatureEditable
If it is set as true, the signature can editable in the PDF Viewer after saved as ink annotation on download action.
- `false` - It disables the edit functionalities after saved on the downloaded document.
- `true` - It enables the edit functionalities after saved on the downloaded document.

#### Data Type and default value
`bool` - `false(Default)`

#### Code Example
```razor
<SfPdfViewer2 IsSignatureEditable="true"></SfPdfViewer2>
```

### ShowDigitalSignatureAppearance
Gets or sets a value that indicates whether to show the digital signature appearance in the document.

#### Data Type and default value
`bool` - `true(Default)`

#### Code Example
```razor
<SfPdfViewer2 ShowDigitalSignatureAppearance="true"></SfPdfViewer2>
```

