# Document Structure

Document lifecycle & page layout — creating, saving, closing documents and configuring sections.

---
##Common instruction

Don't call properties or methods on an object after calling close statement.

## Create add page and save PDF document

```csharp
var outputPath = Path.Combine(Directory.GetCurrentDirectory(), "output", "document.pdf");
//Create a new PDF document.
PdfDocument document = new PdfDocument();
//Add a page to the document.
PdfPage page = document.Pages.Add();

//Add your content here

//Save the document.
document.Save(outputPath);
//Close the document.
document.Close(true);
Console.WriteLine($"SUCCESS: {outputPath}");
```

## Add sections with page settings

```csharp
//Add new section to the document
PdfSection section = document.Sections.Add();
//Set page size
section.PageSettings.Size = new SizeF(565, 845);
//Set margins
section.PageSettings.Margins.All = 40;
//Add a new page to the section
PdfPage page = section.Pages.Add();

//Add your content here
```

## Add document properties

```csharp
//Set document information.
document.DocumentInformation.Author = "Syncfusion";
document.DocumentInformation.CreationDate = DateTime.Now;
document.DocumentInformation.Creator = "Essential PDF";
document.DocumentInformation.Keywords = "PDF";
document.DocumentInformation.Subject = "Document information DEMO";
document.DocumentInformation.Title = "Essential PDF Sample";
```

## Load existing PDF file

```csharp
PdfLoadedDocument loadedDocument = new PdfLoadedDocument("input.pdf"); //it may direct file path or stream
```

## Get existing page

```csharp
PdfLoadedPage loadedPage = loadedDocument.Pages[0] as PdfLoadedPage;
//Draw a simple text
//Create font
PdfFont font = new PdfStandardFont(PdfFontFamily.Helvetica, 20);
//Draw the text.
loadedPage.Graphics.DrawString("Hello World!!!", font, PdfBrushes.Black, new Syncfusion.Drawing.PointF(0, 0));
```

## Save and close the exising PDF file

```csharp
loadedDocument.Save(outputPath);
loadedDocument.Close(true);
```
