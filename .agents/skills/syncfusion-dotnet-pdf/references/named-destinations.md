# PDF Named Destinations

Guide and code snippets for adding, modifying, removing, and linking named destinations in PDF documents using Syncfusion .NET PDF Library. Examples are ordered from basic → advanced.

*Note: For document creation, loading, and save/close patterns, see [document-structure.md](document-structure.md).*

---

**Common namespaces:**

```csharp
using Syncfusion.Drawing;
using Syncfusion.Pdf;
using Syncfusion.Pdf.Graphics;
using Syncfusion.Pdf.Interactive;
using Syncfusion.Pdf.Parsing;
```

## Add a named destination to a new PDF

```csharp
PdfDocument doc = new PdfDocument();
PdfPage page = doc.Pages.Add();

//Create a named destination
PdfNamedDestination destination = new PdfNamedDestination("TOC");
destination.Destination = new PdfDestination(page);
//Set the location
destination.Destination.Location = new PointF(0, 500);
//Set zoom factor to 400%
destination.Destination.Zoom = 4;
doc.NamedDestinationCollection.Add(destination);

//Draw content at the destination point
page.Graphics.DrawString("Hello World!!", new PdfStandardFont(PdfFontFamily.Helvetica, 10), PdfBrushes.Black, new PointF(0, 500));
```

---

## Add a named destination to an existing PDF

```csharp
PdfLoadedDocument loadedDocument = new PdfLoadedDocument("Input.pdf");
PdfPageBase page = loadedDocument.Pages[0];

PdfNamedDestination destination = new PdfNamedDestination("TOC");
destination.Destination = new PdfDestination(page);
destination.Destination.Location = new PointF(0, 500);
destination.Destination.Zoom = 4;
loadedDocument.NamedDestinationCollection.Add(destination);
```

---

## Remove a named destination

```csharp
PdfLoadedDocument lDoc = new PdfLoadedDocument("Input.pdf");
PdfNamedDestinationCollection destinationCollection = lDoc.NamedDestinationCollection;

//Remove by title
destinationCollection.Remove("TOC");
```

---

## Modify an existing named destination

```csharp
PdfLoadedDocument lDoc = new PdfLoadedDocument("Input.pdf");
PdfNamedDestinationCollection destinationCollection = lDoc.NamedDestinationCollection;

//Rename the first destination
PdfNamedDestination destination = destinationCollection[0];
destination.Title = "POC";
```

---

## Link a named destination to a bookmark

```csharp
PdfDocument doc = new PdfDocument();
PdfPage page = doc.Pages.Add();

//Create named destination
PdfNamedDestination destination = new PdfNamedDestination("TOC");
destination.Destination = new PdfDestination(page);
destination.Destination.Location = new PointF(0, 800);
destination.Destination.Zoom = 4;
doc.NamedDestinationCollection.Add(destination);

//Create a bookmark and assign the named destination
PdfBookmark bookmark = doc.Bookmarks.Add("TOC");
bookmark.NamedDestination = destination;
```

---

## Iterate named destinations from an existing PDF

```csharp
PdfLoadedDocument loadedDocument = new PdfLoadedDocument("Input.pdf");
PdfNamedDestinationCollection destinationCollection = loadedDocument.NamedDestinationCollection;

for (int i = 0; i < destinationCollection.Count; i++)
{
    PdfNamedDestination dest = destinationCollection[i];
    Console.WriteLine($"Title: {dest.Title}, Zoom: {dest.Destination.Zoom}, Location: {dest.Destination.Location}");
}
```

---

## Key APIs

| Member | Description |
| --- | --- |
| `PdfNamedDestination(string)` | Creates a named destination with the given title |
| `PdfNamedDestination.Destination` | Gets or sets the `PdfDestination` (target page and position) |
| `PdfNamedDestination.Title` | Unique string identifier for the destination (max 32 chars; no `=`, `#`, `&`) |
| `PdfDestination(PdfPageBase)` | Creates a destination pointing to the given page |
| `PdfDestination.Location` | `PointF` — X, Y coordinates on the target page |
| `PdfDestination.Zoom` | Magnification factor (1 = 100%, 2 = 200%, 4 = 400%) |
| `PdfDestination.PageIndex` | Zero-based index of the target page |
| `PdfDocument.NamedDestinationCollection` | Collection of `PdfNamedDestination` on a new document |
| `PdfLoadedDocument.NamedDestinationCollection` | Collection of `PdfNamedDestination` on an existing document |
| `PdfNamedDestinationCollection.Add(PdfNamedDestination)` | Adds a named destination to the collection |
| `PdfNamedDestinationCollection.Remove(string)` | Removes a named destination by title |
| `PdfBookmark.NamedDestination` | Assigns a `PdfNamedDestination` to a bookmark for integrated navigation |

---

## Notes

- Named destinations enable opening a PDF at a specific page and zoom level via a URL: `document.pdf#nameddest=Chapter3`.
- Individual parameter values (title + separators) must not exceed **32 characters**.
- The characters `=`, `#`, and `&` are reserved and cannot be used in destination titles.
- See [bookmarks.md](bookmarks.md) for full bookmark navigation patterns.

---
