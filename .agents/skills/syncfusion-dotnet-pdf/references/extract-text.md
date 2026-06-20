# PDF Text Extraction

Extract text from PDF pages using the Syncfusion .NET PDF Library

*Note: For document creation, loading, and save/close patterns, see [document-structure.md](document-structure.md).*

---

**Common namespaces:**

```csharp
using Syncfusion.Pdf;
using Syncfusion.Pdf.Interactive;
using Syncfusion.Pdf.Parsing;
```

## Extract text from a single page

```csharp
 PdfPageBase page = loadedDocument.Pages[pageIndex];
 string text = page.ExtractText();
 ```

## Extract text from the entire document

```csharp
string allText = string.Empty;
foreach (PdfLoadedPage p in loadedDocument.Pages)
{
    allText += p.ExtractText();
}
```

## Layout‑based extraction (as seen in the viewer)

```csharp
ent.Pages[0];
string laidOut = page.ExtractText(true); // layout mode
```

## Extract text with bounds (lines/words/characters)

Note: On .NET Core (cross-platform), use `TextLineCollection`. On classic .NET Framework, the overloads return List<TextLine> / List<TextData>.

```csharp
PdfPageBase page = loadedDocument.Pages[0];
TextLineCollection lines; // contains lines → words → glyphs
string pageText = page.ExtractText(out lines);

// Example: iterate lines and get text + bounds
foreach (TextLine line in lines.TextLine)
{
    RectangleF lineBounds = line.Bounds;
    string lineText = line.Text;
    // Gets collection of the words in the line
    List<TextWord> textWordCollection = line.WordCollection;
    foreach(TextWord textWord in textWordCollection)
    {
        // Get Glyph details of the word
        List<TextGlyph> textGlyphCollection = textWord.Glyphs;
        foreach(TextGlyph textGlyph in textGlyphCollection)
        {
            // Get bounds of the character
            RectangleF glyphBounds = textGlyph.Bounds;
            // Get font name of the character
            string glyphFontName = textGlyph.FontName;
            // Get font size of the character
            float glyphFontSize = textGlyph.FontSize;
            // Get font style of the character
            FontStyle glyphFontStyle = textGlyph.FontStyle;
            // Get the character in the word
            char glyphText = textGlyph.Text;
            // Get the color of the character
            Color glyphColor = textGlyph.TextColor;
        }
    }
}
```

## Find text across all pages

Returns all matching text occurrences with page index and bounds.

```csharp
PdfLoadedDocument loadedDocument = new PdfLoadedDocument("input.pdf");
// Key: Page index
// Value: List of bounding rectangles where text is found
Dictionary<int, List<RectangleF>> matchRects =
    new Dictionary<int, List<RectangleF>>();

loadedDocument.FindText("invoice", out matchRects);
```

## Find text in a specific page

Search only within a single page.

```csharp
List<RectangleF> bounds;
loadedDocument.FindText("Total Amount", pageIndex: 0, out bounds);
```

## Find multiple text values at once

Efficiently search for multiple keywords in a single pass.

```csharp
List<string> keywords = new List<string> { "Invoice", "Date", "Amount" };
TextSearchResultCollection results;
loadedDocument.FindText(keywords, out results);
```

## Find text with search options

Customize search behavior such as case sensitivity and whole-word matching.

```csharp
TextSearchOptions options = new TextSearchOptions
{
    CaseSensitive = false,
    WholeWord = true
};

TextSearchResultCollection results;
loadedDocument.FindText(
    new List<string> { "invoice" },
    options,
    out results
);
```
