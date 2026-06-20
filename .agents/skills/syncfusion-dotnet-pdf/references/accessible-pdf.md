# Tagged PDF (Accessible PDFs)

Create and manage **tagged/accessible PDFs** (PDF/UA / Section 508 / WCAG) by adding semantic structure, alternate text, reading order, artifacts, and tagged tables/lists/images with the Syncfusion .NET PDF Library.

*Note: For document creation, loading, and save/close patterns, see [document-structure.md](document-structure.md).*

---

**Common namespaces:**

```csharp
using Syncfusion.Drawing;
using Syncfusion.Pdf;
using Syncfusion.Pdf.Graphics;
using Syncfusion.Pdf.Interactive;
using Syncfusion.Pdf.Grid;
using Syncfusion.Pdf.Parsing;
```

## Enable auto‑tagging

Automatically tag elements as they are added (quick start for accessible PDFs).

```csharp
// On a new PdfDocument: enable the property before adding content to the page or document
document.AutoTag = true;
```

## Tag text as a paragraph

Attach a semantic Paragraph tag to a drawn text element (and provide fallback/ActualText).

```csharp
PdfStructureElement p = new PdfStructureElement(PdfTagType.Paragraph)
{
    ActualText = "Simple paragraph element" // replacement text for AT
};

PdfTextElement text = new PdfTextElement(loremIpsum);
text.PdfTag = p;

text.Draw(page, bounds);
```

## Tag headings (H1…H6)

Use structural heading tags for proper navigation.

```csharp
var h = new PdfStructureElement(PdfTagType.Heading);
var h1 = new PdfStructureElement(PdfTagType.HeadingLevel1) { Parent = h };

var title = new PdfTextElement("Quarterly Report");
title.PdfTag = h1;

title.Draw(page, point);
```

## Tag an image with alternate text

Ensure non‑text content has Alt text and the correct tag type (Figure).

```csharp
//Load the image as stream
FileStream imageStream = new FileStream("syncfusion.jpg", FileMode.Open, FileAccess.Read);
PdfBitmap img = new PdfBitmap(stream);
//Create structure element with the tag type of figure
PdfStructureElement imageElement  = new PdfStructureElement(PdfTagType.Figure)
{
    AlternateText = "Product photo: Model X in blue"
};
img.PdfTag = imageElement ;
img.Draw(page, new RectangleF(100, 100, 300, 200));
```

## Tag an shapes with alternative text

Describes vector shapes for accessibility.

```csharp
//Initialize structure element with tag type as figure
PdfStructureElement element = new PdfStructureElement(PdfTagType.Figure);
//Set alternate text
element.AlternateText = "Line Sample";
//Initialize the line shape
PdfLine line = new PdfLine(100, 100, 100, 300);
line.Pen = new PdfPen(Color.Red);
//Adding tag to the line element
line.PdfTag = element;
//Draws the line
line.Draw(page.Graphics);
```

## Adding tag to hyperlink

Exposes links as interactive elements.

```csharp
//Creates new PDF structure element with tag type link
PdfStructureElement linkStructureElement = new PdfStructureElement(PdfTagType.Link);
//Create the text web link
PdfTextWebLink textLink = new PdfTextWebLink();
//Adding tag to text web link
textLink.PdfTag = linkStructureElement;
//Set the hyperlink
textLink.Url = "http://www.syncfusion.com";
//Set the link text
textLink.Text = "Syncfusion .NET components and controls";

## Adding tag to templates

Tags reusable visual elements.

```csharp
//Create a PDF template
PdfTemplate template = new PdfTemplate(100, 50);
//Initialize the structure element with tag type figure
PdfStructureElement structureElement = new PdfStructureElement(PdfTagType.Figure);
//Set alternative description for figure
structureElement.AlternateText = "Template Figure";
//Adding tag to the template element
template.PdfTag = structureElement;
```

## Mark decorative content as Artifact

Exclude headers/footers/lines/watermarks from the logical structure.

```csharp
//Creating artifact type for the header
PdfArtifact headerArtifact = new PdfArtifact(PdfArtifactType.Pagination, new RectangleF(30, 40, 100, 100), new PdfAttached(PdfEdge.Top), PdfArtifactSubType.Header);
PdfPageTemplateElement header = new PdfPageTemplateElement(bounds);
//Adding artifact to the header
header.PdfTag = headerArtifact;
```

## Control reading order (structure parenting)

Re‑parent structure elements to enforce a logical order for assistive tech.

```csharp
//Initialize the structure element
PdfStructureElement paraStruct1 = new PdfStructureElement(PdfTagType.Paragraph);
//Order the tag in first position
paraStruct1.Order = 1;
```

## Tag a list (UL/OL) with items

Represent lists semantically.

```csharp
string[] products = { "Tools", "Grid", "Chart", "Edit", "Diagram", "XlsIO", "Grouping", "Calculate", "PDF", "HTMLUI", "DocIO" };

//Initialize new structure element with tag type List.
PdfStructureElement listElement = new PdfStructureElement(PdfTagType.List);

//Create ordered list
PdfOrderedList pdfList = new PdfOrderedList();
//Adding tag for list element
pdfList.PdfTag = listElement;

for (int i = 0; i < products.Length; i++)
{
    pdfList.Items.Add(string.Concat("Essential ", products[i]));
    //Adding tag for the list item
    pdfList.Items[i].PdfTag = new PdfStructureElement(PdfTagType.ListItem);
}

//Draw the list
pdfList.Draw(page, new RectangleF(0, 20, size.Width, size.Height));
```

## Tag a table (Table/TH/TD) for accessibility

Apply proper table semantics (header/data cells).

```csharp
// Grid/table already prepared
//Initialize the new structure element with tag type table.
PdfStructureElement element = new PdfStructureElement(PdfTagType.Table);

//Create a new PdfGrid.
PdfGrid pdfGrid = new PdfGrid();

//Adding tag to PDF grid.
pdfGrid.PdfTag = element;

//Add three columns.
pdfGrid.Columns.Add(3);

//Add header.
pdfGrid.Headers.Add(1);

//Set table header.
PdfGridRow pdfGridHeader = pdfGrid.Headers[0];
//Adding tag for each row with tag type TR.
pdfGridHeader.PdfTag = new PdfStructureElement(PdfTagType.TableRow);

//Set the cell value.  
pdfGridHeader.Cells[0].Value = "Employee ID";

//Adding tag for header cell with tag type TH
pdfGridHeader.Cells[0].PdfTag = new PdfStructureElement(PdfTagType.TableHeader) { Scope = ScopeType.Column };

//Set the cell value. 
pdfGridHeader.Cells[1].Value = "Employee Name";

//Adding tag for header cell with tag type TH.
pdfGridHeader.Cells[1].PdfTag = new PdfStructureElement(PdfTagType.TableHeader) { Scope = ScopeType.Column };

//Set the cell value. 
pdfGridHeader.Cells[2].Value = "Salary";

//Adding tag for header cell with tag type TH.
pdfGridHeader.Cells[2].PdfTag = new PdfStructureElement(PdfTagType.TableHeader) { Scope = ScopeType.Column };

//Add rows.
PdfGridRow pdfGridRow = pdfGrid.Rows.Add();

//Add tag to table row.
pdfGridRow.PdfTag = new PdfStructureElement(PdfTagType.TableRow);

//Set the cell values. 
pdfGridRow.Cells[0].Value = "E01";
pdfGridRow.Cells[1].Value = "Clay";
pdfGridRow.Cells[2].Value = "$10,000";

//Adding tag for each cell with tag type TD.
pdfGridRow.Cells[0].PdfTag = new PdfStructureElement(PdfTagType.TableDataCell);
pdfGridRow.Cells[1].PdfTag = new PdfStructureElement(PdfTagType.TableDataCell);
pdfGridRow.Cells[2].PdfTag = new PdfStructureElement(PdfTagType.TableDataCell);

//Draw the PdfGrid
pdfGrid.Draw(pdfPage, PointF.Empty);
```

## Tag form fields and annotations

Expose widgets/notes to assistive technology.

```csharp
//Create a text box field
PdfTextBoxField textBoxField = new PdfTextBoxField(page, "This is form field text box");

// Example for a text box (widget)
PdfStructureElement formField = new PdfStructureElement(PdfTagType.Form);
textBoxField.PdfTag = formField;

//Adding tag for the annotation
PdfPopupAnnotation popupAnnotation = new PdfPopupAnnotation(new RectangleF(10, 40, 30, 30), "Test popup annotation");

// Example for an annotation
PdfStructureElement annotation = new PdfStructureElement(PdfTagType.Annotation);
popupAnnotation.PdfTag = annotation;
```

## Document‑level accessibility metadata

Set language and standard metadata that screen readers leverage.

```csharp
document.DocumentInformation.Title = "Annual Accessibility Report";
document.Language = "en-US"; // primary document language (BCP 47)
```

## Custom role mapping

Maps custom tags to PDF/UA roles.

```csharp
// Create a new PDF document 
PdfDocument doc = new PdfDocument(); 

// Create a custom structure element with a specified role 
PdfStructureElement structureElement = new PdfStructureElement("WorkBook"); 

// Create a text element and associate it with the structure element 
PdfTextElement element = new PdfTextElement(text); 
element.PdfTag = structureElement; 

// Create a role map to define custom structure roles 
PdfRoleMap roleMap = new PdfRoleMap(); 
roleMap.Add("WorkBook", "Document"); // Mapping "WorkBook" to "Document" 
roleMap.Add("WorkSheet", "Sect"); // Mapping "WorkSheet" to "Sect" 

doc.StructureRoleMap = roleMap; // Assign role map to the document 
```

## Get accessibility tags from existing PDF

Reads structure information from a tagged PDF.

```csharp
//Load the existing PDF document.
PdfLoadedDocument document = new PdfLoadedDocument("Input.pdf");
//Get the structure element root from the document.
PdfStructureElement rootElement = document.StructureElement;
//Get the child elements for the element.
PdfStructureElement[] child = rootElement.ChildElements;
//Get the first element from the child element.
PdfStructureElement element = child[0];
//Get the element properties.
string abbrevation = element.Abbrevation;
string ActualText = element.ActualText;
string AlternateText = element.AlternateText;
string Language = element.Language;
int Order = element.Order;
PdfTagType TagType = element.TagType;
string Title = element.Title;
ScopeType scope = element.Scope;
RectangleF bounds = element.Bounds;
//Get the parent of the child element.
PdfStructureElement parent = element.Parent;
```

### Get page-wise accessibility tags

Gets accessibility data per page.

```csharp
//Get the first page from the document.
PdfLoadedPage loadedPage = document.Pages[0] as PdfLoadedPage;
//Get the structure elements associated with the page.
PdfStructureElement[] pageElements = loadedPage.StructureElements;
//Get the first element from the page.
PdfStructureElement element = pageElements[0];
//Get the element properties.
string abbrevation = element.Abbrevation;
string ActualText = element.ActualText;
string AlternateText = element.AlternateText;
string Language = element.Language;
int Order = element.Order;
PdfTagType TagType = element.TagType;
string Title = element.Title;
ScopeType scope = element.Scope;
RectangleF bounds = element.Bounds;
//Get the tagged text in a paragraph or header tags.
string taggedText = element.Text;
//Get the parent element for the element.
PdfStructureElement parent = element.Parent;
//Get the child elements for the element.
PdfStructureElement[] child = element.ChildElements;
```

## PDF for Universal Accessibility (PDF/UA-2)

PDF/UA-2 ensures that PDF 2.0 files conform to the Web Content Accessibility Guidelines (WCAG), making them accessible to all users.

```csharp
//Create a new PDF document 
PdfDocument document = new PdfDocument();     

//Set PDF File version 2.0 
document.FileStructure.Version = PdfVersion.Version2_0; 

//Set true to auto tag all elements in document 
document.AutoTag = true; 

//your code here. By setting File version and AutoTag is create the PDF/UA-2 documents.
```

## Well-Tagged PDF (WTPDF)

Well-Tagged PDF (WTPDF) enables the creation of fully reusable and accessible PDF 2.0 files in an interoperable manner. WTPDF is essentially identical to PDF/UA-2. A PDF file can be compliant with PDF/UA-2, WTPDF, or both.

```csharp
//Create a new PDF document 
PdfDocument document = new PdfDocument(PdfConformanceLevel.Pdf_A4);     

//Set PDF File version 2.0 
document.FileStructure.Version = PdfVersion.Version2_0; 

//Set true to auto tag all elements in document 
document.AutoTag = true; 

//your code here. By setting File version, AutoTag and the conformance level is create the WTPDF documents.
```
