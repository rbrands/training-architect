# PDF Bookmarks

Add, manage, and modify bookmarks (outline navigation) in PDF documents using Syncfusion .NET PDF Library.

*Note: For document creation, loading, and save/close patterns, see [document-structure.md](document-structure.md).*

---
**Common namespaces:**

```csharp
using Syncfusion.Drawing;
using Syncfusion.Pdf;
using Syncfusion.Pdf.Graphics;
using Syncfusion.Pdf.Interactive;
```

---

## Add bookmarks to a new PDF

Use the `PdfBookmark` class to create navigable bookmarks with destinations.

```csharp
using Syncfusion.Drawing;
using Syncfusion.Pdf.Interactive;

// Create document bookmarks
PdfBookmark bookmark = document.Bookmarks.Add("Page 1");

// Set the destination page
bookmark.Destination = new PdfDestination(page);

// Set the destination location
bookmark.Destination.Location = new PointF(20, 20);

// Set the text style and color
bookmark.TextStyle = PdfTextStyle.Bold;
bookmark.Color = Color.Red;
```

---

## Add bookmarks to existing PDF

Load an existing PDF and add new bookmarks to it.

```csharp
using Syncfusion.Drawing;
using Syncfusion.Pdf.Interactive;

// Create bookmarks
PdfBookmark bookmark = document.Bookmarks.Add("Page 1");

// Set the destination page
bookmark.Destination = new PdfDestination(document.Pages[0]);

// Set the destination location
bookmark.Destination.Location = new PointF(20, 20);

// Set the text style and color
bookmark.TextStyle = PdfTextStyle.Bold;
bookmark.Color = Color.Red;
```

---

## Add child bookmarks (nested)

Create hierarchical bookmark structure by adding child bookmarks.

```csharp
using Syncfusion.Drawing;
using Syncfusion.Pdf.Interactive;

// Create parent bookmark
PdfBookmark bookmark = document.Bookmarks.Add("Chapter 1");
bookmark.Destination = new PdfDestination(page);
bookmark.Destination.Location = new PointF(20, 20);

// Add child bookmark (nested)
PdfBookmark childBookmark = bookmark.Insert(0, "Section 1.1");
childBookmark.Destination = new PdfDestination(page);
childBookmark.Destination.Location = new PointF(400, 300);
childBookmark.Destination.Zoom = 2F;

// Set styles
bookmark.TextStyle = PdfTextStyle.Bold;
bookmark.Color = Color.Red;
```

---

## Insert bookmarks in existing PDF

Insert new bookmarks at specific positions in the existing bookmark collection.

```csharp
using Syncfusion.Pdf.Interactive;
using Syncfusion.Pdf.Parsing;

// Insert a new bookmark at index 1
PdfBookmark bookmark = document.Bookmarks.Insert(1, "New Page 2");

// Set the destination page and location
bookmark.Destination = new PdfDestination(document.Pages[1]);
bookmark.Destination.Location = new PointF(0, 300);

```

---

## Remove bookmarks from existing PDF

Delete bookmarks by name or index.

```csharp
using Syncfusion.Pdf.Interactive;

// Get all bookmarks
PdfBookmarkBase bookmarks = document.Bookmarks;

// Remove bookmark by name
bookmarks.Remove("Page 1");

// Remove bookmark by index
bookmarks.RemoveAt(1);
```

---

## Modify bookmarks in existing PDF

Change bookmark properties: destination, color, style, and title.

```csharp
using Syncfusion.Pdf.Interactive;

// Get all bookmarks
PdfBookmarkBase bookmarks = document.Bookmarks;

// Get the first bookmark and modify properties
PdfLoadedBookmark bookmark = bookmarks[0] as PdfLoadedBookmark;
bookmark.Destination = new PdfDestination(document.Pages[0]);
bookmark.Color = Color.Green;
bookmark.TextStyle = PdfTextStyle.Bold;
bookmark.Title = "Changed Title";
```

---

## Get bookmark page index

Retrieve the page index associated with a bookmark in an existing PDF.

```csharp
using Syncfusion.Pdf.Interactive;

// Get all bookmarks
PdfBookmarkBase bookmark = loadedDocument.Bookmarks;

// Get the bookmark page index
int index = bookmark[0].Destination.PageIndex;
```

---

## Bookmark Properties

| Property | Type | Purpose |
| --- | --- | --- |
| `Title` | string | Bookmark display text |
| `Destination` | `PdfDestination` | Target page and location |
| `Color` | `Color` | Text color in PDF viewer outline |
| `TextStyle` | `PdfTextStyle` | Font style (Bold, Italic, BoldItalic, None) |
| `Destination.Location` | `PointF` | X, Y coordinates on target page |
| `Destination.Zoom` | float | Magnification factor (1 = 100%, 2 = 200%, etc.) |
| `Destination.PageIndex` | int | Zero-based page index (read-only for loaded bookmarks) |

---

## Bookmark Hierarchy

```csharp
// Example hierarchy:
// - Chapter 1 (bookmark)
//   - Section 1.1 (child)
//   - Section 1.2 (child)
// - Chapter 2 (bookmark)
//   - Section 2.1 (child)

document.Bookmarks.Add("Chapter 1");  // Root level
document.Bookmarks[0].Insert(0, "Section 1.1");  // Child of Chapter 1
document.Bookmarks[0].Insert(1, "Section 1.2");  // Child of Chapter 1
document.Bookmarks.Add("Chapter 2");  // Root level
document.Bookmarks[1].Insert(0, "Section 2.1");  // Child of Chapter 2
```
