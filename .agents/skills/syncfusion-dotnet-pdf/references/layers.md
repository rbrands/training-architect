# PDF Layers

Create, manage, and toggle optional content (layers) in PDF documents using Syncfusion .NET PDF Library.

*Note: For document creation, loading, and save/close patterns, see [document-structure.md](document-structure.md).*

---

**Common namespaces:**

```csharp
using Syncfusion.Drawing;
using Syncfusion.Pdf;
using Syncfusion.Pdf.Graphics;
using Syncfusion.Pdf.Parsing;
using Syncfusion.Pdf.Interactive;
```

---

## Add layers to new PDF

Create multiple layers with different visual content in a new PDF document.

```csharp
using Syncfusion.Drawing;
using Syncfusion.Pdf.Graphics;

// Add first layer
PdfPageLayer layer = page.Layers.Add("Layer1");
PdfGraphics graphics = layer.Graphics;
graphics.TranslateTransform(100, 60);

// Draw arc in first layer
PdfPen pen = new PdfPen(Color.Red, 50);
RectangleF bounds = new RectangleF(0, 0, 50, 50);
graphics.DrawArc(pen, bounds, 360, 360);

// Add second layer
PdfPageLayer layer2 = page.Layers.Add("Layer2");
graphics = layer2.Graphics;
graphics.TranslateTransform(100, 180);

// Draw ellipse in second layer
graphics.DrawEllipse(pen, bounds);
```

---

## Add layers to existing PDF

Add new layers to an already created PDF document.

```csharp
using Syncfusion.Drawing;
using Syncfusion.Pdf.Graphics;

// Add first layer
PdfPageLayer layer = loadedPage.Layers.Add("Layer1");
PdfGraphics graphics = layer.Graphics;
graphics.TranslateTransform(100, 60);

// Draw arc
PdfPen pen = new PdfPen(Color.Gray, 50);
RectangleF bounds = new RectangleF(0, 0, 50, 50);
graphics.DrawArc(pen, bounds, 360, 360);

// Add second layer
PdfPageLayer layer2 = loadedPage.Layers.Add("Layer2");
graphics = layer2.Graphics;
graphics.TranslateTransform(100, 180);
graphics.DrawEllipse(pen, bounds);
```

---

## Create nested layers

Build hierarchical layer structure with parent and child layers.

```csharp
using Syncfusion.Drawing;
using Syncfusion.Pdf.Graphics;

// Add parent layer
PdfLayer layer = document.Layers.Add("Layer1");
PdfGraphics graphics = layer.CreateGraphics(page);
graphics.TranslateTransform(100, 60);

// Draw arc in parent layer
PdfPen pen = new PdfPen(Color.Red, 50);
RectangleF bounds = new RectangleF(0, 0, 50, 50);
graphics.DrawArc(pen, bounds, 360, 360);

// Add child layer to parent
PdfLayer layer2 = layer.Layers.Add("Layer2");
graphics = layer2.CreateGraphics(page);
graphics.TranslateTransform(100, 180);

// Draw ellipse in child layer
graphics.DrawEllipse(pen, bounds);
```

---

## Add annotation to layer

Create annotations and assign them to specific layers.

```csharp
using Syncfusion.Drawing;
using Syncfusion.Pdf.Graphics;
using Syncfusion.Pdf.Interactive;

// Add layer
PdfLayer layer = document.Layers.Add("Layer");
PdfGraphics graphics = layer.CreateGraphics(page);

// Draw ellipse on layer
graphics.DrawEllipse(PdfPens.Red, new RectangleF(50, 50, 40, 40));

// Create square annotation
PdfSquareAnnotation annotation = new PdfSquareAnnotation(
    new RectangleF(200, 260, 50, 50), "Square annotation");
annotation.Color = new PdfColor(Color.Red);

// Assign annotation to layer
annotation.Layer = layer;

// Add annotation to page
page.Annotations.Add(annotation);
```

---

## Remove layers from PDF

Delete layers from an existing PDF document.

```csharp
using Syncfusion.Pdf.Graphics;

// Get layer collection
PdfPageLayerCollection layers = loadedPage.Layers;

// Remove layer by index
layers.RemoveAt(0);

// Or remove by name
layers.Remove("Layer1");
```

---

## Flatten layers in PDF

Remove layers from visibility and merge them (flatten) into the page content.

```csharp
using Syncfusion.Pdf.Graphics;

// Get the layer collection
PdfDocumentLayerCollection layers = loadedDocument.Layers;

// Flatten a layer (remove from layer tree)
layers.RemoveAt(0);
```

---

## Toggle layer visibility

Show or hide layers in the PDF viewer.

```csharp
using Syncfusion.Drawing;
using Syncfusion.Pdf;
using Syncfusion.Pdf.Graphics;


// Add layer 1 with visibility enabled
PdfPageLayer layer = page.Layers.Add("Layer1", true);
PdfGraphics graphics = layer.Graphics;
graphics.TranslateTransform(100, 60);

// Draw arc
PdfPen pen = new PdfPen(Color.Red, 50);
RectangleF bounds = new RectangleF(0, 0, 50, 50);
graphics.DrawArc(pen, bounds, 360, 360);

// Add layer 2 with visibility disabled (hidden by default)
PdfPageLayer layer2 = page.Layers.Add("Layer2", false);
graphics = layer2.Graphics;
graphics.TranslateTransform(100, 180);
graphics.DrawEllipse(pen, bounds);
```

---

## Toggle visibility in existing PDF

Change layer visibility in a loaded PDF document.

```csharp
using Syncfusion.Pdf;
using Syncfusion.Pdf.Parsing;

// Load the PDF document
PdfLoadedDocument document = new PdfLoadedDocument("Input.pdf");

// Get first layer
PdfLayer layer = document.Layers[0];

// Disable visibility (hide layer)
layer.Visible = false;

// Or enable visibility (show layer)
layer.Visible = true;

```

---

## Lock or unlock layers

Prevent users from toggling layer visibility or editing layer content.

```csharp
using Syncfusion.Drawing;
using Syncfusion.Pdf;
using Syncfusion.Pdf.Graphics;

// Create layer
PdfLayer layer = document.Layers.Add("Layer");

// Lock the layer (user cannot toggle visibility)
layer.Locked = true;

// Create graphics for layer
PdfGraphics graphics = layer.CreateGraphics(page);

// Draw ellipse
graphics.DrawEllipse(PdfPens.Red, new RectangleF(50, 50, 40, 40));
```

---

## Layer Properties

| Property | Type | Purpose |
| --- | --- | --- |
| `Name` | string | Layer/group identifier |
| `Visible` | bool | Show/hide layer content |
| `Locked` | bool | Prevent user interaction with layer |
| `Layers` | `PdfLayerCollection` | Child layers (for nested structure) |

---

## Layer View Modes

Layers are typically useful for:

| Use Case | Benefit |
| --- | --- |
| **CAD Drawings** | Show/hide construction lines, dimensions, notes |
| **Maps** | Toggle road layers, boundary layers, annotations |
| **Artwork** | Separate design elements for editing flexibility |
| **Multi-Language** | Show different language layers |
| **Design Drafts** | Display different versions or alternatives |
| **Print vs Screen** | Different content for print vs digital viewing |

---

## Layer Hierarchy Example

```csharp
// Example structure:
// Document Layers
// └── Layer1 (parent)
//     ├── Layer1.1 (child)
//     └── Layer1.2 (child)
// └── Layer2 (parent)
//     └── Layer2.1 (child)

document.Layers.Add("Layer1");
document.Layers[0].Layers.Add("Layer1.1");  // Child of Layer1
document.Layers[0].Layers.Add("Layer1.2");  // Child of Layer1
document.Layers.Add("Layer2");
document.Layers[1].Layers.Add("Layer2.1");  // Child of Layer2
```
