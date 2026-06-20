# PDF Tables (PdfGrid)

Create and render PDF tables using PdfGrid in the Syncfusion .NET PDF Library.

*Note: For document creation, loading, and save/close patterns, see [document-structure.md](document-structure.md).*

---

**Common namespaces:**

```csharp
using Syncfusion.Pdf;
using Syncfusion.Pdf.Graphics;
using Syncfusion.Pdf.Grid;
```

---

## Create a table from a data source

PdfGrid supports binding from `DataTable`, `IEnumerable`, arrays, and more.

```csharp
PdfGrid grid = new PdfGrid();

// Bind data (e.g., IEnumerable)
IEnumerable<object> data = new List<object>
{
    new { ID = "E01", Name = "Clay" },
    new { ID = "E02", Name = "Thomas" }
};

grid.DataSource = data;

// Draw on page (page available elsewhere)
grid.Draw(page, new PointF(10, 10));

```

## Create a table without a data source (manual rows & columns)

```csharp
PdfGrid grid = new PdfGrid();

// Define columns
grid.Columns.Add(3);

// Header row
PdfGridRow header = grid.Headers.Add(1)[0];
header.Cells[0].Value = "Employee ID";
header.Cells[1].Value = "Name";
header.Cells[2].Value = "Salary";

// Data row
PdfGridRow row = grid.Rows.Add();
row.Cells[0].Value = "E01";
row.Cells[1].Value = "Clay";
row.Cells[2].Value = "$10,000";

// Render
grid.Draw(page, PointF.Empty);
```

## Apply built‑in table styles

```csharp
PdfGrid grid = new PdfGrid();
// ... set DataSource or rows

grid.ApplyBuiltinStyle(PdfGridBuiltinStyle.GridTable4Accent1);
```

## Customize columns (widths)

```csharp
PdfGrid grid = new PdfGrid();

PdfGridColumn c1 = grid.Columns.Add();
c1.Width = 100;
PdfGridColumn c2 = grid.Columns.Add();
c2.Width = 200;
PdfGridColumn c3 = grid.Columns.Add();
c3.Width = 100;   
```

## Customize grid appearance (style)

```csharp
PdfGridStyle style = new PdfGridStyle
{
    BackgroundBrush = PdfBrushes.LightGray,
    CellSpacing = 0.5f,
    TextBrush = PdfBrushes.Black,
    AllowHorizontalOverflow = true
};

grid.Style = style;
```

## Apply row and column span

```csharp
// Assume grid has been created and rows added
PdfGridRow row = grid.Rows.Add();
row.Cells[0].Value = "Spanned Cell";
// Column span: merge current cell across the next two columns (total 3 columns width) 
row.Cells[0].ColumnSpan = 3;
// Optionally, center text in spanned cell 
row.Cells[0].StringFormat = new PdfStringFormat { Alignment = PdfTextAlignment.Center, LineAlignment = PdfVerticalAlignment.Middle };
// Row span:
PdfGridRow row1 = grid.Rows[0]; 
header.Cells[0].RowSpan = 2; // spans cell into the next row
```

## Automatic pagination across pages

```csharp
PdfGridLayoutFormat layout = new PdfGridLayoutFormat
{
    Layout = PdfLayoutType.Paginate
};

grid.Draw(page, new RectangleF(0, 0, page.GetClientSize().Width, page.GetClientSize().Height), layout);
```

## Repeat header row on each page

```csharp
grid.RepeatHeader = true;
```

## Nested tables

```csharp
// Create parent grid
PdfGrid parentGrid = new PdfGrid();
parentGrid.Columns.Add(1);
PdfGridRow parentRow = parentGrid.Rows.Add();
// Create nested grid 
PdfGrid nestedGrid = new PdfGrid(); 
nestedGrid.Columns.Add(2); 
PdfGridRow nRow = nestedGrid.Rows.Add(); 
nRow.Cells[0].Value = "Inner 1"; 
nRow.Cells[1].Value = "Inner 2";
// Assign nested grid to a cell 
parentRow.Cells[0].Value = nestedGrid;
```

## Add background image to a cell

```csharp
PdfGridRow row = grid.Rows.Add();
row.Cells[0].Value = "Text over image";
// Set background image 
PdfBitmap image = new PdfBitmap(imageStream); 
row.Cells[0].Style.BackgroundImage = image; 
row.Cells[0].Style.BackgroundImageLayout = PdfGridBackgroundImageLayout.Stretch;
```

## Draw borderless (no‑line) table

```csharp
// Remove all borders
foreach (PdfGridRow row in grid.Rows) 
{ 
    foreach (PdfGridCell cell in row.Cells) 
    { 
        cell.Style.Borders.All = PdfPens.Transparent; 
    }
}
```

## Add hyperlinks inside table cells

```csharp
PdfGridRow row = grid.Rows.Add();
// Create a URL link 
PdfUriAnnotation link = new PdfUriAnnotation( new RectangleF(0, 0, 100, 20), "https://www.syncfusion.com");
row.Cells[0].Value = link;
```

## Table events

Use events to customize layout during rendering.

### BeginCellLayout event

The event raised on starting cell lay outing.

```csharp
grid.BeginCellLayout += new PdfGridBeginCellLayoutEventHandler(grid_BeginCellLayout);

//Cell layout event handler
void table_BeginCellLayout(object sender,PdfGridBeginCellLayoutEventArgs args)
{
    if (args.RowIndex == 1)
    {
        args.Graphics.DrawRectangle(new PdfPen(PdfBrushes.Red, 2), PdfBrushes.White,args.Bounds);
    }
}
```

### EndCellLayout event

The event raised on finished cell layout.

```csharp
// Subscribe the cell layout event 
grid.EndCellLayout += new PdfGridEndCellLayoutEventHandler(table_EndCellLayout);

// Cell layout event handler
oid table_EndCellLayout(object sender, PdfGridEndCellLayoutEventArgs args)
{
    if (args.RowIndex == 1)
    {
        args.Graphics.DrawRectangle(new PdfPen(PdfBrushes.Red, 2), PdfBrushes.White, args.Bounds);
    }
}
```
