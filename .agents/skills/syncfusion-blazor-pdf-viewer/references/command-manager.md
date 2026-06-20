# Command Manager in Blazor SfPdfViewer Component

## Table of Contents
- [Command Manager](#command-manager)
- [API Properties](#api-properties)
- [Code Example](#code-example)

---

## Command Manager

The PDF Viewer supports mapping keyboard gestures to named commands so that pressing a defined key combination triggers a viewer action.

The PdfViewerCommandManager enables defining custom commands that execute when the specified key gesture is recognized.

## API Properties

| **Property** | **Purpose** | **Type** |
|-----|-----|-----|
| **Commands** | Collection of keyboard command mappings (key gesture → action name). Add `<KeyboardCommand>` elements here. | List<[KeyboardCommand](./keyboard-enum-properties.md#keyboardcommand)> |

### Command Executed
Represents the method to be called when the designated cusotmized command is invoked.

#### Value
`EventCallback<CommandExecutedEventArgs>`

#### CommandExecuted Events Arguments
| Event Arguments | Data Type | Description |
|-----|-----|-----|
| `ActionName` | string | Represents the Name of the Action to be performed. |
| `Key` | [PdfKeys](./keyboard-enum-properties.md#pdfkeys) | Represents the PDF key commonly used in Pdf-related operations. |
| `Modifiers` | [PdfModifierKeys](./keyboard-enum-properties.md#pdfmodifierkeys) | Represents the enumeration value for the modifier key used in keyboard combinations in the PDF Viewer.

## Code Example

- We can customize the options, based on the user requirements.

```razor

<SfPdfViewer2 @ref="@pdfViewer">
    <PdfViewerEvents CommandExecuted="@CommandExecute"></PdfViewerEvents>
    <PdfViewerCommandManager Commands="@command" ></PdfViewerCommandManager>                
</SfPdfViewer2>

@code {
    SfPdfViewer2 pdfViewer;

    /// <summary> 
    /// Defines the list of custom commands 
    /// </summary> 
    public List<KeyboardCommand> command = new List<KeyboardCommand>() 
    { 
        new KeyboardCommand() 
        { 
            ActionName = "FitToWidth", // Define the action name as per user requirement
            Gesture = new KeyGesture() { Key = PdfKeys.W, Modifiers = PdfModifierKeys.Shift } // Define the action name as per user requirement
        }
    }; 

    /// <summary> 
    /// Custom command execution. 
    /// </summary> 

    public void CommandExecute(CommandExecutedEventArgs args) 
    { 
        if(args.Modifiers == PdfModifierKeys.Shift && args.Key == PdfKeys.W) 
        { 
            pdfViewer.FitToWidthAsync(); // Define the action name as per user requirement
        }
    } 
}
```

