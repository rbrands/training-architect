# PDF Actions

Add interactive actions and triggers to PDF documents using Syncfusion .NET PDF Library.

*Note: For document creation, loading, and save/close patterns, see [document-structure.md](document-structure.md).*

---

**Common namespaces:**

```csharp
using Syncfusion.Pdf;
using Syncfusion.Pdf.Interactive;
using Syncfusion.Drawing;
using Syncfusion.Pdf.Graphics;
using Syncfusion.Pdf.Parsing;
```

## Add launch action

Execute an external application or open a file when the PDF loads.

```csharp
using Syncfusion.Pdf.Interactive;

// Create and add launch action
PdfLaunchAction action = new PdfLaunchAction("logo.png");
document.Actions.AfterOpen = action;
```

---

## Add JavaScript action

Execute JavaScript code when the document opens or on user interaction.

```csharp
using Syncfusion.Pdf.Interactive;

// Create JavaScript action
PdfJavaScriptAction scriptAction = new PdfJavaScriptAction("app.alert(\"Hello World!!!\")");

// Add the action to document (executes on open)
document.Actions.AfterOpen = scriptAction;
```

---

## Add URI/URL action

Create a hyperlink that opens a web URL.

```csharp
using Syncfusion.Pdf.Interactive;

// Create URI action
PdfUriAction uriAction = new PdfUriAction("http://www.google.com");

// Add the action
document.Actions.AfterOpen = uriAction;
```

---

## Add GoTo action

Navigate to a specific page in the same PDF document.

```csharp
using Syncfusion.Drawing;
using Syncfusion.Pdf.Interactive;

// Create GoTo action to jump to page 2
PdfGoToAction gotoAction = new PdfGoToAction(page2);
gotoAction.Destination = new PdfDestination(page2, new PointF(0, 100));

// Add the action
document.Actions.AfterOpen = gotoAction;
```

---

## Add named action

Jump to predefined locations (first, last, next, previous page).

```csharp
using Syncfusion.Pdf.Interactive;

// Create named action (go to last page)
PdfNamedAction namedAction = new PdfNamedAction(PdfActionDestination.LastPage);

// Add the action
document.Actions.AfterOpen = namedAction;
```

---

## Add sound action

Play an audio file when the PDF opens or on user interaction.

```csharp
using Syncfusion.Pdf.Interactive;

// Create sound action
PdfSoundAction soundAction = new PdfSoundAction("Startup.wav");
soundAction.Sound.Bits = 16;
soundAction.Sound.Channels = PdfSoundChannels.Stereo;
soundAction.Sound.Encoding = PdfSoundEncoding.Signed;
soundAction.Volume = 0.9f;  // 90% volume

// Set as document action
document.Actions.AfterOpen = soundAction;
```

---

## Add submit action

Submit form data to a web server when a button is clicked.

```csharp
using Syncfusion.Drawing;
using Syncfusion.Pdf;
using Syncfusion.Pdf.Interactive;

// Create a Submit button field
PdfButtonField submitButton = new PdfButtonField(page, "Submit data");
submitButton.Bounds = new RectangleF(100, 60, 50, 20);
submitButton.ToolTip = "Submit";

// Create submit action
PdfSubmitAction submitAction = new PdfSubmitAction("http://www.example.com/submit");
submitAction.DataFormat = SubmitDataFormat.Html;
submitButton.Actions.GotFocus = submitAction;

// Add button to form
document.Form.Fields.Add(submitButton);
```

---

## Add reset action

Reset all form fields to their default values.

```csharp
using Syncfusion.Drawing;
using Syncfusion.Pdf;
using Syncfusion.Pdf.Interactive;

// Create a text field
PdfTextBoxField textBoxField = new PdfTextBoxField(page, "FirstName");
textBoxField.BorderColor = new PdfColor(Color.Gray);
textBoxField.Bounds = new RectangleF(80, 0, 100, 20);
textBoxField.Text = "First Name";
document.Form.Fields.Add(textBoxField);

// Create a Clear button
PdfButtonField clearButton = new PdfButtonField(page, "Clear");
clearButton.Bounds = new RectangleF(100, 60, 50, 20);
clearButton.ToolTip = "Clear";

// Create reset action
PdfResetAction resetAction = new PdfResetAction();
clearButton.Actions.MouseDown = resetAction;

// Add button to form
document.Form.Fields.Add(clearButton);

```

---

## Add action to form field

Attach actions to specific form field events.

```csharp
using Syncfusion.Drawing;
using Syncfusion.Pdf;
using Syncfusion.Pdf.Interactive;


// Create a button field
PdfButtonField submitButton = new PdfButtonField(page, "submitButton");
submitButton.Bounds = new RectangleF(25, 160, 100, 20);
submitButton.Text = "Apply";
submitButton.BackColor = new PdfColor(181, 191, 203);

// Create JavaScript action
PdfJavaScriptAction scriptAction = new PdfJavaScriptAction(
    "app.alert(\"You are looking at Form field action of PDF \")");

// Attach to field event
submitButton.Actions.MouseDown = scriptAction;

// Add button to form
document.Form.Fields.Add(submitButton);

```

---

## Add action to bookmark

Assign actions to bookmarks so clicking them performs custom actions.

```csharp
using Syncfusion.Drawing;
using Syncfusion.Pdf;
using Syncfusion.Pdf.Interactive;


// Create bookmark
PdfBookmark bookmark = document.Bookmarks.Add("Page 1");
bookmark.TextStyle = PdfTextStyle.Bold;
bookmark.Color = Color.Red;

// Create URI action
PdfUriAction uriAction = new PdfUriAction("http://www.google.com");

// Attach action to bookmark
bookmark.Action = uriAction;
```

---

## Add document-level JavaScript actions

Define JavaScript functions at the document level that run on open.

```csharp
using Syncfusion.Pdf;
using Syncfusion.Pdf.Interactive;

// Get document JavaScript collection
PdfDocumentJavaScriptCollection javaScriptCollection = document.DocumentJavaScripts;

// Create JavaScript action
PdfJavaScriptAction javaScriptAction = new PdfJavaScriptAction(
    "app.alert(\"Hello World!!!\")");

// Set name for the action
javaScriptAction.Name = "Test";

// Add to document
javaScriptCollection.Add(javaScriptAction);
```

---

## Remote GoTo action

Navigate to a specific page in a different PDF file.

```csharp
using Syncfusion.Drawing;
using Syncfusion.Pdf;
using Syncfusion.Pdf.Interactive;

// Create button field
PdfButtonField submitButton = new PdfButtonField(page, "submitButton");
submitButton.Bounds = new RectangleF(25, 160, 100, 20);
submitButton.Font = new PdfStandardFont(PdfFontFamily.Helvetica, 12f, PdfFontStyle.Bold);
submitButton.Text = "Open file";
submitButton.BackColor = new PdfColor(181, 191, 203);

// Create remote destination (page 3 of external PDF)
PdfRemoteDestination remoteDestination = new PdfRemoteDestination();
remoteDestination.RemotePageNumber = 3;
remoteDestination.Mode = PdfDestinationMode.FitToPage;

// Create remote GoTo action
PdfRemoteGoToAction goToAction = new PdfRemoteGoToAction("input.pdf", remoteDestination);
goToAction.IsNewWindow = true;

// Attach action to button
submitButton.Actions.GotFocus = goToAction;

// Add button to form
document.Form.Fields.Add(submitButton);

```

---

## Supported Action Types

| Action Class | Triggers | Purpose |
| --- | --- | --- |
| `PdfLaunchAction` | On event | Execute external application or file |
| `PdfJavaScriptAction` | On event | Run embedded JavaScript code |
| `PdfUriAction` | On event | Open URL in web browser |
| `PdfGoToAction` | On event | Jump to page in same PDF |
| `PdfNamedAction` | On event | Jump to predefined location (first/last/next/prev) |
| `PdfSoundAction` | Document open | Play audio file |
| `PdfSubmitAction` | Button click | Submit form data to server |
| `PdfResetAction` | Button click | Reset form fields |
| `PdfRemoteGoToAction` | On event | Jump to page in different PDF |

---

## Field Action Events

| Event | Trigger |
| --- | --- |
| `MouseUp` | User releases mouse button |
| `MouseDown` | User presses mouse button |
| `GotFocus` | Field receives focus |
| `LostFocus` | Field loses focus |
| `KeyDown` | User presses key in field |
| `KeyUp` | User releases key in field |
