
## Create Custom Data Using XlsIO
- Use **Syncfusion XlsIO** to create Excel data programmatically.
- Generate simple tabular data (rows and columns only).
- Convert the created workbook to a byte array and load it into the Blazor Spreadsheet.
- Avoid XlsIO features not supported in Blazor Spreadsheet (example: conditional formatting).

## XlsIO Overview
Before generating Excel data, you can review the official XlsIO documentation:

**XlsIO Overview**  
https://help.syncfusion.com/document-processing/excel/excel-library/net/overview

This overview helps understand workbook creation, worksheet manipulation, supported features, and more.

### Important Note
Blazor Spreadsheet **does not support all features** available in XlsIO.  
Example:  
- **Conditional Formatting** → ✔ Supported in XlsIO  
- **Conditional Formatting** → ✘ NOT supported in Blazor Spreadsheet

Blazor Spreadsheet supported features:  
https://help.syncfusion.com/document-processing/excel/spreadsheet/blazor/getting-started-webapp

### CUSTOM CUSTOMER DATA EXAMPLE

````csharp
// Example: Create custom customer data programmatically using XlsIO
private byte[] CreateCustomerSpreadsheet()
{
    // Initialize the Excel engine
    using ExcelEngine excelEngine = new ExcelEngine();
    IApplication excelApplication = excelEngine.Excel;

    // Create a new workbook with one worksheet
    IWorkbook workbook = excelApplication.Workbooks.Create(1);
    IWorksheet worksheet = workbook.Worksheets[0];
  
    //Enter values to the cells
    worksheet.Range["A3"].Text = "46036 Michigan Ave";
    worksheet.Range["A4"].Text = "Canton, USA";
    worksheet.Range["A5"].Text = "Phone: +1 231-231-2310";
    
    //Make the text bold
    worksheet.Range["A3:A5"].CellStyle.Font.Bold = true;
    
    //Merge cells
    worksheet.Range["D1:E1"].Merge();
    
    //Enter text to the cell D1 and apply formatting.
    worksheet.Range["D1"].Text = "INVOICE";
    worksheet.Range["D1"].CellStyle.Font.Bold = true;
    worksheet.Range["D1"].CellStyle.Font.RGBColor = Color.FromArgb(42, 118, 189);
    worksheet.Range["D1"].CellStyle.Font.Size = 35;
    
    //Apply alignment in the cell D1
    worksheet.Range["D1"].CellStyle.HorizontalAlignment = ExcelHAlign.HAlignRight;
    worksheet.Range["D1"].CellStyle.VerticalAlignment = ExcelVAlign.VAlignTop;
    
    //Enter values to the cells from D5 to E8
    worksheet.Range["D5"].Text = "INVOICE#";
    worksheet.Range["E5"].Text = "DATE";
    worksheet.Range["D6"].Number = 1028;
    worksheet.Range["E6"].Value = "12/31/2018";
    worksheet.Range["D7"].Text = "CUSTOMER ID";
    worksheet.Range["E7"].Text = "TERMS";
    
    // To add more formatting like:
    
    // 1. //Setting Font Color
	worksheet.Range["B6"].CellStyle.Font.Color = ExcelKnownColors.Green;
    
    // 2. Add borders
    worksheet.Range["A1:F10"].CellStyle.Borders.LineStyle = ExcelLineStyle.Thin;
    
    // 3. Add a SUM formula
    worksheet.Range["F10"].Formula = "=SUM(F2:F9)";

    //4. Set the background color
    worksheet.Range["B6"].CellStyle.Color = Color.FromArgb(0, 51, 105);

    // Auto-fit columns
    worksheet.UsedRange.AutofitColumns();

    // Save the workbook to a memory stream and return as byte array
    using MemoryStream memoryStream = new MemoryStream();
    workbook.SaveAs(memoryStream);
    return memoryStream.ToArray();
}

// Use in OnInitialized:
protected override void OnInitialized()
{
    DataSourceBytes = CreateCustomerSpreadsheet();
}

````
### Notes
- There is no property and events available for custom workbook creation.
- using only `Syncfusion.Drawing` namespace for referring the Color
