# Syncfusion Blazor PDF Viewer - Undo/Redo Properties

## Table of Contents
- [Overview](#overview)
- [Properties List](#properties-list)
- [Methods](#methods)

## Overview

The Syncfusion Blazor PDF Viewer provides comprehensive undo and redo functionality that allows users to reverse or reapply changes made to PDF documents. The undo/redo system tracks all user actions including annotations, form field entries, edits, and other modifications. The undo/redo properties and methods provide developers with complete control over the document's edit history and enable users to navigate through their changes seamlessly.

## Properties List

### CanUndo
Gets a value indicating whether the user can undo the previous operation in the PDF Viewer control.

#### Data Type
`bool`

#### Code Example
```razor
<SfPdfViewer2 CanUndo="CanUndo">
</SfPdfViewer2>
@code {
    private bool CanUndo;
}
```

### CanUndoChanged
Gets or sets a callback that updates the CanUndo bound value.

#### Data Type
`EventCallback<bool>`

#### Code Example
```razor
<SfPdfViewer2 CanUndoChanged="CanUndoChanged">
</SfPdfViewer2>
@code {
    private void CanUndoChanged() {
        // customize your code in based on the requirements
    }
}
```

### CanRedo
Gets a value that indicates whether the most recent action can be undone.

#### Data Type
`bool`

#### Code Example
```razor
<SfPdfViewer2 CanRedo="CanRedo">
</SfPdfViewer2>
@code {
    private bool CanRedo;
}
```

### CanRedoChanged
Gets or sets a callback that updates the CanRedo bound value.

#### Data Type
`EventCallback<bool>`

#### Code Example
```razor
<SfPdfViewer2 CanRedoChanged="CanRedoChanged">
</SfPdfViewer2>
@code {
    private void CanRedoChanged() {
        // customize your code in based on the requirements
    }
}
```

## Methods

### UndoAsync()
Performs an undo operation for the last action asynchronously. Reverses the most recent edit, annotation, or form field change.

#### Parameters
No Parameters

#### Return Type
`Task`

#### Code Example
```csharp
    // We can bind the method for click action based the user requirement.
    await viewer.UndoAsync();
```

### RedoAsync()
Performs a redo operation for the last undone action asynchronously. Reverses the effect of the most recent undo action.

#### Parameters
No Parameters

#### Return Type
`Task`

#### Code Example
```csharp
    // We can bind the method for click action based the user requirement.
    await viewer.RedoAsync();
```

