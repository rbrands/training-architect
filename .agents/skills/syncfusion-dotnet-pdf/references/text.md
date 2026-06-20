# PDF Text

Draw text in PDF documents using standard, TrueType, CJK, and OpenType fonts with alignment, RTL support, HTML styled text, multi-column layouts, pagination, lists, and complex scripts using Syncfusion .NET PDF Library.

*Note: For document creation, loading, and save/close patterns, see [document-structure.md](document-structure.md). For extracting text from existing PDFs, see [extract-text.md](extract-text.md). For headers/footers with text, see [headers-and-footers.md](headers-and-footers.md).*

---

**Common namespaces:**

```csharp
using Syncfusion.Drawing;
using Syncfusion.Pdf;
using Syncfusion.Pdf.Graphics;
using Syncfusion.Pdf.Lists;
using Syncfusion.Pdf.Parsing;
```

---

## Draw text in a new PDF (standard font)

Use `PdfGraphics.DrawString` with `PdfStandardFont` for the 14 built-in PDF fonts.

```csharp
PdfFont font = new PdfStandardFont(PdfFontFamily.Helvetica, 20);
graphics.DrawString("Hello World!!!", font, PdfBrushes.Black, new PointF(0, 0));
```

---

## Draw text in an existing PDF

```csharp
// page.Graphics comes from a PdfLoadedPage
PdfFont font = new PdfStandardFont(PdfFontFamily.Helvetica, 20);
graphics.DrawString("Hello World!!!", font, PdfBrushes.Black, new PointF(0, 0));
```

---

## Draw text using a TrueType font (file path)

```csharp
PdfFont font = new PdfTrueTypeFont("Arial.ttf", 14);
graphics.DrawString("Hello World!!!", font, PdfBrushes.Black, new PointF(0, 0));
```

---

## Draw text using a TrueType font (stream)

```csharp
FileStream fontStream = new FileStream("Arial.ttf", FileMode.Open, FileAccess.Read);
PdfFont font = new PdfTrueTypeFont(fontStream, 14);
graphics.DrawString("Hello World!!!", font, PdfBrushes.Black, new PointF(0, 0));
```

---

## Draw text using an OpenType font (.otf)

`PdfTrueTypeFont` also accepts `.otf` font files.

```csharp
PdfFont font = new PdfTrueTypeFont("Font.otf", 14);
graphics.DrawString(
    "Syncfusion Essential PDF is a .NET Core PDF library",
    font,
    new PdfSolidBrush(new PdfColor(0, 0, 0)),
    new RectangleF(0, 0, page.GetClientSize().Width, page.GetClientSize().Height));
```

---

## Draw text using a CJK font

Use `PdfCjkStandardFont` for Chinese, Japanese, and Korean text.

```csharp
PdfFont font = new PdfCjkStandardFont(PdfCjkFontFamily.HeiseiMinchoW3, 20);
graphics.DrawString("こんにちは世界", font, PdfBrushes.Black, new PointF(0, 0));
```

---

## Customize TrueType font settings (PdfFontSettings)

Use `PdfFontSettings` to control size, style, embedding, and subsetting in one call.

```csharp
FileStream fontStream = new FileStream("Arial.ttf", FileMode.Open, FileAccess.Read);
// PdfFontSettings(size, style, unicode, embed, subset)
PdfFontSettings fontSettings = new PdfFontSettings(10, PdfFontStyle.Bold, true, true, true);
PdfFont font = new PdfTrueTypeFont(fontStream, fontSettings);
graphics.DrawString("Hello World!!!", font, PdfBrushes.Black, new PointF(0, 0));
```

---

## Add text encoding to a standard font

Register code-page encoding providers (required for .NET Core), then call `SetTextEncoding`.

```csharp
// Required once at startup in .NET Core
System.Text.Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

PdfStandardFont font = new PdfStandardFont(PdfFontFamily.Helvetica, 12);
font.SetTextEncoding(Encoding.GetEncoding("Windows-1250"));
graphics.DrawString("äÖíßĆŇ", font, PdfBrushes.Black, PointF.Empty);
```

---

## Measure a string

Use `PdfFont.MeasureString` to get the rendered size of a string before drawing it.

```csharp
PdfFont font = new PdfStandardFont(PdfFontFamily.Helvetica, 12);
string text = "Hello World!";

// Returns the SizeF (width, height) the string occupies at this font size
SizeF size = font.MeasureString(text);
graphics.DrawString(text, font, PdfBrushes.Black, new RectangleF(PointF.Empty, size));
```

---

## Measure tilting space for italic text

Enable `MeasureTiltingSpace` in `PdfStringFormat` for accurate measurement of italic strings.

```csharp
PdfFont font = new PdfTrueTypeFont("arial.ttf", 14, PdfFontStyle.Italic);

PdfStringFormat format = new PdfStringFormat();
format.MeasureTiltingSpace = true;

string text = "Hello World!";
SizeF size = font.MeasureString(text, format);
graphics.DrawString(text, font, PdfBrushes.Black, new RectangleF(0, 0, size.Width, size.Height));
```

---

## Apply text alignment and spacing

Control horizontal/vertical alignment, word spacing, and character spacing via `PdfStringFormat`.

```csharp
PdfStringFormat format = new PdfStringFormat();
format.Alignment        = PdfTextAlignment.Right;
format.LineAlignment    = PdfVerticalAlignment.Middle;
format.WordSpacing      = 2f;
format.CharacterSpacing = 1f;

PdfFont font = new PdfStandardFont(PdfFontFamily.Helvetica, 12);
graphics.DrawString("Right-Alignment", font, PdfBrushes.Red,
    new RectangleF(10, 10, 200, 20), format);
```

---

## Draw text with baseline alignment

Set `EnableBaseline = true` to align text drawn in different fonts/sizes to a common baseline.

```csharp
PdfFont font1 = new PdfTrueTypeFont("tahoma.ttf", 8);
PdfFont font2 = new PdfTrueTypeFont("Arial.ttf", 20);
PdfFont font3 = new PdfStandardFont(PdfFontFamily.Helvetica, 16);
PdfFont font4 = new PdfTrueTypeFont("Calibri.ttf", 25);

PdfStringFormat format = new PdfStringFormat();
format.LineAlignment  = PdfVerticalAlignment.Bottom;
format.EnableBaseline = true;

graphics.DrawString("Hello World!", font1, PdfBrushes.Black, new PointF(0,   50), format);
graphics.DrawString("Hello World!", font2, PdfBrushes.Black, new PointF(65,  50), format);
graphics.DrawString("Hello World!", font3, PdfBrushes.Black, new PointF(220, 50), format);
graphics.DrawString("Hello World!", font4, PdfBrushes.Black, new PointF(320, 50), format);
```

---

## Draw Right-To-Left (RTL) text

Set `TextDirection` to `PdfTextDirection.RightToLeft` for Arabic, Hebrew, Persian, Urdu, etc.

```csharp
PdfFont font = new PdfTrueTypeFont("arial.ttf", 14);

PdfStringFormat format = new PdfStringFormat();
format.TextDirection   = PdfTextDirection.RightToLeft;
format.Alignment       = PdfTextAlignment.Right;
format.ParagraphIndent = 35f;

string text = File.ReadAllText("Arabic.txt", Encoding.Unicode);
graphics.DrawString(text, font, PdfBrushes.Black,
    new RectangleF(0, 0, page.GetClientSize().Width, page.GetClientSize().Height), format);
```

---

## Draw complex script language text

Set `PdfStringFormat.ComplexScript = true` for Thai, Arabic, Devanagari, and similar scripts.

```csharp
PdfFont font = new PdfTrueTypeFont("tahoma.ttf", 10);

PdfStringFormat format = new PdfStringFormat();
format.ComplexScript = true;

graphics.DrawString("สวัสดีชาวโลก", font, PdfBrushes.Black,
    new RectangleF(0, 0, page.GetClientSize().Width, page.GetClientSize().Height), format);
```

---

## Add HTML styled text

Use `PdfHTMLTextElement` to render basic inline HTML tags (font, bold, italic, underline, line break, paragraph) in a PDF.

```csharp
PdfFont font = new PdfStandardFont(PdfFontFamily.TimesRoman, 14);
string htmlText = "<font color='#0000F8' face='TimesRoman' size='14'>" +
                  "<i><b><u>Essential PDF</u></b></i></font> is a <u><i>.NET</i></u> library";

PdfHTMLTextElement richText = new PdfHTMLTextElement(htmlText, font, PdfBrushes.Black);

PdfLayoutFormat format = new PdfLayoutFormat();
format.Layout = PdfLayoutType.Paginate;
format.Break  = PdfLayoutBreakType.FitPage;

richText.Draw(page,
    new RectangleF(0, 20, page.GetClientSize().Width, page.GetClientSize().Height),
    format);
```

---

## Flow long text across multiple pages (PdfTextElement)

`PdfTextElement` with `PdfLayoutFormat` lets text overflow onto new pages automatically. Use `PdfLayoutResult.Bounds` to position the next element below the previous one.

```csharp
string text = File.ReadAllText("Input.txt", Encoding.ASCII);
const int paragraphGap = 10;

PdfTextElement textElement = new PdfTextElement(text,
    new PdfStandardFont(PdfFontFamily.TimesRoman, 14));

PdfLayoutFormat layoutFormat = new PdfLayoutFormat();
layoutFormat.Layout = PdfLayoutType.Paginate;
layoutFormat.Break  = PdfLayoutBreakType.FitPage;

// Draw first paragraph; result.Bounds.Bottom gives the Y end position
PdfLayoutResult result = textElement.Draw(page,
    new RectangleF(0, 0, page.GetClientSize().Width / 2, page.GetClientSize().Height),
    layoutFormat);

// Draw second paragraph starting just below the first
result = textElement.Draw(page,
    new RectangleF(0, result.Bounds.Bottom + paragraphGap,
                   page.GetClientSize().Width / 2, page.GetClientSize().Height),
    layoutFormat);
```

---

## Unit conversion for precise text placement

Use `PdfUnitConverter` to convert inches (or other units) to PDF points before defining text bounds.

```csharp
PdfUnitConverter converter = new PdfUnitConverter();
// Convert 1 inch → points (72 pt = 1 in)
float margin = converter.ConvertUnits(1f, PdfGraphicsUnit.Inch, PdfGraphicsUnit.Point);

RectangleF textBounds = new RectangleF(
    margin, margin,
    page.Graphics.ClientSize.Width  - 2 * margin,
    page.Graphics.ClientSize.Height - 2 * margin);

PdfTextElement textElement = new PdfTextElement(
    "Adventure Works Cycles, a large multinational manufacturing company.",
    new PdfStandardFont(PdfFontFamily.TimesRoman, 14),
    PdfBrushes.Black);

PdfLayoutFormat layoutFormat = new PdfLayoutFormat
{
    Break  = PdfLayoutBreakType.FitPage,
    Layout = PdfLayoutType.Paginate
};

textElement.Draw(page, textBounds, layoutFormat);
```

---

## Create a multi-column layout

Draw `PdfTextElement` into side-by-side bounds to simulate columns.

```csharp
string text = "Adventure Works Cycles manufactures and sells bicycles to North American, " +
              "European and Asian commercial markets.";

float colWidth  = page.GetClientSize().Width / 2;
float colHeight = page.GetClientSize().Height;
PdfFont font = new PdfStandardFont(PdfFontFamily.TimesRoman, 14);

// Left column
new PdfTextElement(text, font).Draw(page, new RectangleF(0, 0, colWidth, colHeight));

// Right column
new PdfTextElement(text, font).Draw(page, new RectangleF(colWidth, 0, colWidth, colHeight));
```

---

## Detect text clipping (PdfStringLayouter)

Use `PdfStringLayouter` to check whether text overflows its bounds before drawing, and capture the remainder.

```csharp
PdfFont font = new PdfStandardFont(PdfFontFamily.Helvetica, 30);
string text = "Adventure Works Cycles, a large multinational manufacturing company " +
              "that manufactures and sells bicycles across global markets.";

RectangleF border = new RectangleF(0, 0, page.GetClientSize().Width, 150);
graphics.DrawRectangle(PdfPens.Black, border);

PdfStringLayouter layouter = new PdfStringLayouter();
PdfStringLayoutResult result = layouter.Layout(
    text, font,
    new PdfStringFormat(PdfTextAlignment.Center),
    new SizeF(border.Width, border.Height));

if (result.Remainder != null)
    Console.WriteLine("Clipped text: " + result.Remainder);
```

---

## Control LineLimit and NoClip

`LineLimit` restricts text to the exact bounds height; `NoClip` prevents words from being cut off.

```csharp
PdfStringFormat format = new PdfStringFormat();
format.NoClip    = true;   // show full words even if they exceed the bounds
format.LineLimit = false;  // allow partial lines to use remaining space

PdfFont font = new PdfStandardFont(PdfFontFamily.Helvetica, 12);
graphics.DrawRectangle(PdfPens.Red, new RectangleF(100, 100, 100, 20));
graphics.DrawString("PDF text line 1\r\nPDF text line 2", font, PdfBrushes.Black,
    new RectangleF(100, 100, 100, 20), format);
```

---

## Save and restore graphics state around text transforms

Use `Save`/`Restore` to scope transforms (translate, rotate) to a specific drawing block.

```csharp
PdfFont font = new PdfStandardFont(PdfFontFamily.Helvetica, 16);

PdfGraphicsState state = graphics.Save();
graphics.TranslateTransform(100, 50);
graphics.RotateTransform(45);
graphics.DrawString("Hello, World!", font, PdfBrushes.Black, new PointF(0, 0));
graphics.Restore(state);

// Drawn without any rotation or translation
graphics.DrawString("This text is not rotated.", font, PdfBrushes.Black, new PointF(0, 100));
```

---

## Add an ordered list

Use `PdfOrderedList` (numbered or alphabetical) with `PdfListItem` entries.

```csharp
PdfFont font = new PdfStandardFont(PdfFontFamily.TimesRoman, 10, PdfFontStyle.Italic);

PdfStringFormat format = new PdfStringFormat();
format.LineSpacing = 10f;

PdfOrderedList pdfList = new PdfOrderedList();
pdfList.Marker.Brush = PdfBrushes.Black;
pdfList.Indent       = 20;
pdfList.Font         = font;
pdfList.StringFormat = format;

foreach (string product in new[] { "PDF", "XlsIO", "DocIO", "Chart", "Diagram" })
    pdfList.Items.Add("Essential " + product);

pdfList.Draw(page, new RectangleF(0, 20, page.GetClientSize().Width, page.GetClientSize().Height));
```

---

## Add an unordered list

Use `PdfUnorderedList` with a `PdfUnorderedMarkerStyle` (Disk, Circle, Square, or Image).

```csharp
PdfUnorderedList list = new PdfUnorderedList();
list.Marker.Style = PdfUnorderedMarkerStyle.Disk;

PdfStringFormat format = new PdfStringFormat();
format.LineSpacing = 10f;

list.Font         = new PdfStandardFont(PdfFontFamily.Helvetica, 12);
list.StringFormat = format;
list.Indent       = 10;
list.TextIndent   = 10;

list.Items.Add("PDF");
list.Items.Add("XlsIO");
list.Items.Add("DocIO");
list.Items.Add("PPT");

list.Draw(page, new RectangleF(0, 10, page.GetClientSize().Width, page.GetClientSize().Height));
```

---

## Key APIs

| Member | Description |
| --- | --- |
| `PdfGraphics.DrawString(string, PdfFont, PdfBrush, PointF)` | Draws text at a point using the font's natural size |
| `PdfGraphics.DrawString(string, PdfFont, PdfBrush, RectangleF, PdfStringFormat)` | Draws text inside a rectangle with optional format (alignment, spacing, direction) |
| `PdfStandardFont(PdfFontFamily, float)` | Creates one of the 14 built-in PDF fonts; no embedding required |
| `PdfStandardFont.SetTextEncoding(Encoding)` | Sets a specific code-page encoding for the standard font |
| `PdfTrueTypeFont(string, float)` | Creates a TrueType/OpenType font from a file path |
| `PdfTrueTypeFont(Stream, float)` | Creates a TrueType font from a stream |
| `PdfTrueTypeFont(Stream, PdfFontSettings)` | Creates a TrueType font with granular control over size, style, embedding, and subsetting |
| `PdfCjkStandardFont(PdfCjkFontFamily, float)` | Creates a CJK font for Chinese, Japanese, or Korean text |
| `PdfFontSettings(float, PdfFontStyle, bool, bool, bool)` | Configures size, style, unicode, embed, and subset for `PdfTrueTypeFont` |
| `PdfFontFamily` | Enum: `Helvetica`, `TimesRoman`, `Courier`, `Symbol`, `ZapfDingbats` |
| `PdfCjkFontFamily` | Enum: `HeiseiKakuGothicW5`, `HeiseiMinchoW3`, `HanyangSystemsGothicMedium`, and more |
| `PdfFont.MeasureString(string)` | Returns the `SizeF` (width × height) that the string occupies at its current size |
| `PdfFont.MeasureString(string, PdfStringFormat)` | Measures with the given format; use `MeasureTiltingSpace = true` for italic accuracy |
| `PdfStringFormat` | Controls alignment, line spacing, character/word spacing, direction, and rendering flags |
| `PdfStringFormat.Alignment` | Horizontal text alignment: `Left`, `Center`, `Right`, `Justify` |
| `PdfStringFormat.LineAlignment` | Vertical text alignment: `Top`, `Middle`, `Bottom` |
| `PdfStringFormat.TextDirection` | `RightToLeft` or `LeftToRight`; use for Arabic, Hebrew, and other RTL languages |
| `PdfStringFormat.ComplexScript` | `true` enables complex-script shaping for Thai, Devanagari, Arabic, etc. |
| `PdfStringFormat.WordSpacing` | Extra space added between words in points |
| `PdfStringFormat.CharacterSpacing` | Extra space added between characters in points |
| `PdfStringFormat.LineSpacing` | Additional spacing between lines in points |
| `PdfStringFormat.ParagraphIndent` | Indent applied to the first line of each paragraph |
| `PdfStringFormat.EnableBaseline` | Aligns text of mixed fonts/sizes to a common baseline when `true` |
| `PdfStringFormat.MeasureTiltingSpace` | Improves measurement accuracy for italic fonts when `true` |
| `PdfStringFormat.LineLimit` | `true` (default) = text confined to bounds height; `false` = fills remaining space |
| `PdfStringFormat.NoClip` | `true` = full words shown even if they exceed bounds; `false` = clip at edge |
| `PdfTextElement(string, PdfFont)` | Layoutable text element supporting multi-page flow via `Draw` |
| `PdfTextElement.Draw(PdfPage, RectangleF, PdfLayoutFormat)` | Draws text and returns `PdfLayoutResult` with final bounds |
| `PdfLayoutResult.Bounds` | Bounding rectangle of the last rendered portion; use `.Bottom` to stack elements |
| `PdfLayoutFormat` | Controls pagination: set `Layout = Paginate` and `Break = FitPage` for overflow |
| `PdfHTMLTextElement(string, PdfFont, PdfBrush)` | Renders basic inline HTML (font, b, i, u, br, p) as PDF text |
| `PdfHTMLTextElement.Draw(PdfPage, RectangleF, PdfLayoutFormat)` | Draws HTML text with optional pagination |
| `PdfStringLayouter` | Low-level layouter; use `Layout()` to preview fit/overflow without drawing |
| `PdfStringLayoutResult.Remainder` | The portion of text that did not fit in the specified bounds (`null` if all fit) |
| `PdfOrderedList` | Numbered or alphabetical list; add items via `Items.Add(string)` |
| `PdfUnorderedList` | Bulleted list; set marker style via `Marker.Style` (`Disk`, `Circle`, `Square`) |
| `PdfList.Indent` | Left indent for the list markers in points |
| `PdfList.TextIndent` | Gap between the marker and the item text in points |
| `PdfUnitConverter` | Converts between PDF units (Point, Pixel, Inch, Millimeter, Centimeter) |
| `PdfUnitConverter.ConvertUnits(float, PdfGraphicsUnit, PdfGraphicsUnit)` | Converts a value from one unit to another |
| `PdfGraphics.Save()` / `.Restore(PdfGraphicsState)` | Snapshot and revert graphics state (transforms, clip) |

---

## Notes

- Use `PdfTrueTypeFont` instead of `PdfStandardFont` for Unicode, international characters, or culture-specific symbols (e.g., `€`, umlauts).
- Call `System.Text.Encoding.RegisterProvider(CodePagesEncodingProvider.Instance)` once at startup in .NET Core before using `SetTextEncoding`.
- `PdfHTMLTextElement` supports only basic inline HTML; for complex HTML with CSS and URLs use the HTML-to-PDF converter — see [conversions.md](conversions.md) and the [official HTML-to-PDF documentation](https://help.syncfusion.com/document-processing/pdf/conversions/html-to-pdf/net).
- RTF-to-image (`PdfImage.FromRtf`) is Windows-only; use the RTF-to-PDF conversion path for cross-platform support.
- Always wrap `RotateTransform`/`TranslateTransform` between `Save()` and `Restore()` to avoid affecting subsequent drawing operations.
- `PdfStringLayouter.Layout()` is useful for pre-flight checks — measure how much text fits before committing to a draw call.

---

## Related

- [extract-text.md](extract-text.md)
- [pdf-graphics.md](pdf-graphics.md)
- [brushes.md](brushes.md)
- [images.md](images.md)
- [headers-and-footers.md](headers-and-footers.md)
- [document-structure.md](document-structure.md)
- [conversions.md](conversions.md)
- ../SKILL.md

## Official documentation

- <https://help.syncfusion.com/document-processing/pdf/pdf-library/net/working-with-text>
- <https://help.syncfusion.com/document-processing/pdf/conversions/html-to-pdf/net>
