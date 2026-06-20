## PdfKeys

| **Name** | **Description** |
|-----|-----|
| **A** | Represents the key value A when the A key is pressed in the Keyboard  |
| **ArrowDown** | Represents the key value ArrowDown when the ArrowDown key is pressed in the Keyboard  |
| **ArrowLeft** | Represents the key value ArrowLeft when the ArrowLeft key is pressed in the Keyboard |
| **ArrowRight** | Represents the key value ArrowRight when the ArrowRight key is pressed in the Keyboard |
| **ArrowUp** | Represents the key value ArrowUp when the ArrowUp key is pressed in the Keyboard |
| **B** | Represents the key value B when the B key is pressed in the Keyboard |
| **BackSpace** | Represents the key value BackSpace when the BackSpace key is pressed in the Keyboard |
| **C** | Represents the key value C when the C key is pressed in the Keyboard |
| **D** | Represents the key value D when the D key is pressed in the Keyboard |
| **Delete** | Represents the key value Delete when the Delete key is pressed in the Keyboard |
| **E** | Represents the key value E when the E key is pressed in the Keyboard |
| **End** | Represents the key value End when the End key is pressed in the Keyboard |
| **Enter** | Represents the key value Enter when the Enter key is pressed in the Keyboard |
| **Escape** | Represents the key value Escape when the Escape key is pressed in the Keyboard |
| **F** | Represents the key value F when the F key is pressed in the Keyboard |
| **F1** | Represents the key value F1 when the F1 key is pressed in the Keyboard |
| **F10** | Represents the key value F10 when the F10 key is pressed in the Keyboard |
| **F11** | Represents the key value F11 when the F11 key is pressed in the Keyboard |
| **F12** | Represents the key value F12 when the F12 key is pressed in the Keyboard |
| **F2** | Represents the key value F2 when the F2 key is pressed in the Keyboard |
| **F3** | Represents the key value F3 when the F3 key is pressed in the Keyboard |
| **F4** | Represents the key value F4 when the F4 key is pressed in the Keyboard |
| **F5** | Represents the key value F5 when the F5 key is pressed in the Keyboard |
| **F6** | Represents the key value F6 when the F6 key is pressed in the Keyboard |
| **F7** | Represents the key value F7 when the F7 key is pressed in the Keyboard |
| **F8** | Represents the key value F8 when the F8 key is pressed in the Keyboard |
| **F9** | Represents the key value F9 when the F9 key is pressed in the Keyboard |
| **G** | Represents the key value G when the G key is pressed in the Keyboard |
| **H** | Represents the key value H when the H key is pressed in the Keyboard |
| **Home** | Represents the key value Home when the Home key is pressed in the Keyboard |
| **I** | Represents the key value I when the I key is pressed in the Keyboard |
| **J** | Represents the key value J when the J key is pressed in the Keyboard |
| **K** | Represents the key value K when the K key is pressed in the Keyboard |
| **L** | Represents the key value L when the L key is pressed in the Keyboard |
| **M** | Represents the key value M when the M key is pressed in the Keyboard |
| **Minus** | Represents the key value Minus when the Minus key is pressed in the Keyboard |
| **N** | Represents the key value N when the N key is pressed in the Keyboard |
| **None** | Represents a null key value when no keys are pressed |
| **Number0** | Represents the key value 0 when the 0 key is pressed in the Keyboard |
| **Number1** | Represents the key value 1 when the 1 key is pressed in the Keyboard |
| **Number2** | Represents the key value 2 when the 2 key is pressed in the Keyboard |
| **Number3** | Represents the key value 3 when the 3 key is pressed in the Keyboard |
| **Number4** | Represents the key value 4 when the 4 key is pressed in the Keyboard |
| **Number5** | Represents the key value 5 when the 5 key is pressed in the Keyboard |
| **Number6** | Represents the key value 6 when the 6 key is pressed in the Keyboard |
| **Number7** | Represents the key value 7 when the 7 key is pressed in the Keyboard |
| **Number8** | Represents the key value 8 when the 8 key is pressed in the Keyboard |
| **Number9** | Represents the key value 9 when the 9 key is pressed in the Keyboard |
| **O** | Represents the key value O when the O key is pressed in the Keyboard |
| **P** | Represents the key value P when the P key is pressed in the Keyboard |
| **PageDown** | Sets the key value as PageDown when page down key is pressed |
| **PageUp** | Represents the key value PageUp when the PageUp key is pressed in the Keyboard |
| **Plus** | Represents the key value Plus when the Plus key is pressed in the Keyboard |
| **Q** | Represents the key value Q when the Q key is pressed in the Keyboard |
| **R** | Represents the key value R when the R key is pressed in the Keyboard |
| **S** | Represents the key value S when the S key is pressed in the Keyboard |
| **Space** | Represents the key value Space when the Space key is pressed in the Keyboard |
| **Star** | Represents the key value Star when the Star key is pressed in the Keyboard |
| **T** | Represents the key value T when the T key is pressed in the Keyboard |
| **Tab** | Represents the key value Tab when the Tab key is pressed in the Keyboard |
| **U** | Represents the key value U when the U key is pressed in the Keyboard |
| **V** | Represents the key value V when the V key is pressed in the Keyboard |
| **W** | Represents the key value W when the W key is pressed in the Keyboard |
| **X** | Represents the key value X when the X key is pressed in the Keyboard |
| **Y** | Represents the key value Y when the Y key is pressed in the Keyboard |
| **Z** | Represents the key value Z when the Z key is pressed in the Keyboard |

## PdfModifierKeys

| **Modifier** | **When to Use** |
|-----|-----|
| **Control** (Ctrl) | Standard Windows/Linux shortcuts (Ctrl+P, Ctrl+S) |
| **Alt** | Alternative commands, menu access keys |
| **Shift** | Capitalization, reverse actions (Shift+Tab for reverse navigation) |
| **Meta** (Cmd on Mac) | macOS-style shortcuts |
| **None** | Single-key shortcuts without modifiers (F1, Escape) |

## KeyboardCommand

| **Property** | **Purpose** | **Type** |
|-----|-----|-----|
| **ActionName** | Name of the custom action to execute. Must match your C# handler method name. | string |
| **Gesture** | Key combination (key + modifiers) that triggers this command. | [KeyGesture](#keygesture) |

## KeyGesture

| **Property** | **Purpose** | **Type** |
|-----|-----|-----|
| **Key** | Primary key from PdfKeys enum (A-Z, F1-F12, arrows, etc.). | [PdfKeys](#pdfkeys) |
| **Modifiers** | Optional modifier keys (Ctrl, Alt, Shift, Meta) from PdfModifierKeys enum. | [PdfModifierKeys](#pdfmodifierkeys) |