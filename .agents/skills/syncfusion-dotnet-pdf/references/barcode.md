# PDF Barcodes

Add one-dimensional and two-dimensional barcodes to a PDF document using Syncfusion .NET PDF Library.

*Note: For document creation, loading, and save/close patterns, see [document-structure.md](document-structure.md).*

---

**Common namespaces:**

```csharp
using Syncfusion.Drawing;
using Syncfusion.Pdf;
using Syncfusion.Pdf.Barcode;
```

---

## Add a 1D barcode — Code 39

```csharp
PdfDocument doc = new PdfDocument();
PdfPage page = doc.Pages.Add();

PdfCode39Barcode barcode = new PdfCode39Barcode();
barcode.BarHeight = 45;
barcode.Text = "CODE39$";

//Draw barcode on the page
barcode.Draw(page, new PointF(25, 70));
```

## Add a 1D barcode — EAN-13

```csharp
PdfEan13Barcode barcode = new PdfEan13Barcode();
barcode.BarHeight = 50;
barcode.Text = "400638133393";

PdfDocument document = new PdfDocument();
PdfPage page = document.Pages.Add();
barcode.Draw(page, new PointF(25, 70));
```

## Add a 1D barcode — EAN-8

```csharp
PdfEan8Barcode barcode = new PdfEan8Barcode();
barcode.BarHeight = 50;
barcode.Text = "1234567";

barcode.Draw(page, new PointF(25, 70));
```

## Add a 2D barcode — QR code

```csharp
PdfQRBarcode barcode = new PdfQRBarcode();
barcode.ErrorCorrectionLevel = PdfErrorCorrectionLevel.High;
barcode.XDimension = 3;
barcode.Text = "http://www.syncfusion.com";

PdfDocument doc = new PdfDocument();
PdfPage page = doc.Pages.Add();
barcode.Draw(page, new PointF(25, 70));
```

## Add a 2D barcode — PDF417

```csharp
Pdf417Barcode barcode = new Pdf417Barcode();
barcode.ErrorCorrectionLevel = Pdf417ErrorCorrectionLevel.Auto;
barcode.XDimension = 2;
barcode.Text = "http://www.syncfusion.com";

barcode.Draw(page, new PointF(25, 70));
```

## Set location and size

```csharp
PdfCodabarBarcode barcode = new PdfCodabarBarcode();
barcode.Location = new PointF(100, 100);
barcode.Size = new SizeF(200, 100);
barcode.Text = "123456789$";

//When Location/Size are set, Draw without a PointF argument
barcode.Draw(page);
```

## Hide barcode text

```csharp
PdfCode39Barcode barcode = new PdfCode39Barcode();
barcode.Location = new PointF(10, 10);
barcode.Text = "123456789";
barcode.TextDisplayLocation = TextLocation.None;

barcode.Draw(page);
```

## Export barcode as image

```csharp
//1D barcode to image
PdfCode39Barcode barcode = new PdfCode39Barcode();
barcode.BarHeight = 45;
barcode.Text = "CODE39$";
Stream imageStream = barcode.ToImage(new SizeF(300, 300));

//2D (QR) barcode to image
PdfQRBarcode qrBarcode = new PdfQRBarcode();
qrBarcode.XDimension = 3;
qrBarcode.Text = "http://www.google.com";
Stream qrStream = qrBarcode.ToImage(new Syncfusion.Drawing.SizeF(300, 300));
```

## QR code with logo

```csharp
PdfQRBarcode qrBarcode = new PdfQRBarcode();
qrBarcode.Text = "https://www.syncfusion.com/";
qrBarcode.XDimension = 5;

FileStream imageStream = new FileStream("logo.png", FileMode.Open, FileAccess.Read);
qrBarcode.Logo = new QRCodeLogo(imageStream);

qrBarcode.Draw(page);
```

## Customize appearance

```csharp
//1D — change height and bar color
PdfCode93Barcode code93 = new PdfCode93Barcode("CODE93");
code93.BarHeight = 40;
code93.BarColor = Color.Blue;
code93.Draw(page, new PointF(25, 500));

//2D — change XDimension and background color
PdfQRBarcode barcode = new PdfQRBarcode();
barcode.XDimension = 3;
barcode.BackColor = Color.Green;
barcode.Text = "http://www.syncfusion.com";
barcode.Draw(page, new PointF(25, 70));
```

### Supported barcode types

| Type | Valid characters | Length |
| --- | --- | --- |
| QR Code | [0-9] [A-Z] [space $ % * + - . / , :] [Shift JIS] | variable |
| DataMatrix | All ASCII | variable |
| Code 39 | [0-9] [A-Z] [- . $ / + % SPACE] | variable |
| Code 39 Extended | [0-9] [A-Z] [a-z] | variable |
| Code 11 | [0-9] [-] | variable |
| Codabar | [0-9] [- $ : / . +] | variable |
| Code 32 | [0-9] | 8 |
| Code 93 | [0-9] [A-Z] [- . $ / + % SPACE] | variable |
| Code 93 Extended | All 128 ASCII | variable |
| Code 128A / 128B / 128C | See UG | variable |
| PDF417 | [0-9] [A-Z] [a-z] Mixed Punctuation Byte | variable |
| EAN-8 | [0-9] | 7 or 8 |
| EAN-13 | [0-9] | 12 or 13 |

### NuGet note

To export barcodes as images on .NET Core, reference [Syncfusion.Pdf.Imaging.Net.Core](https://www.nuget.org/packages/Syncfusion.Pdf.Imaging.Net.Core/).

## Related

- [pdf-graphics.md](pdf-graphics.md)
- [document-structure.md](document-structure.md)
- ../SKILL.md

## Official documentation

- <https://help.syncfusion.com/document-processing/pdf/pdf-library/net/working-with-barcode>

---
