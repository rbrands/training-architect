# Syncfusion .NET PDF Library Skill

Create, read, edit, secure, sign, and convert PDF documents (.pdf) using Syncfusion PDF Library for .NET. Always use this skill for PDF processing and document automation when the user asks to generate PDF files, modify PDF content, add security or signatures, extract text or images, merge or split PDFs, or perform PDF/A conversion using C# code or CSX execution.

See **[SKILL.md](SKILL.md)** for the full intent-routing guide and rules.

---

## Two Modes

### Mode 1: Generate C# Code for the User's Project *(default)*

Produces production-ready C# code and adds it directly into the user's project files (e.g., `Program.cs`). No `.csx` scripts are created or run.

**Trigger keywords:** "code", "snippet", "how to write", "Program.cs", "show me", "sample", "example code", "generate code for".

### Mode 2: Execute via CSX Script

Creates a temporary `.csx` script, runs it with `dotnet script`, and produces an output file — without touching the user's project.

**Trigger keywords:** "create a pdf document", "make a document", "generate a document", "open", "edit", "modify", "change" a `.pdf` file, "without modifying my project", "run a csx script", or when a file path is provided (e.g., `output/report.pdf`).

**Workflow:**

1. Create `scripts/temp-{timestamp}.csx` using `references/template.csx` as the base
2. Add required operations from `references/*.md` snippets and replace placeholders with actual content
3. Run: `dotnet script scripts/temp-{timestamp}.csx`
4. Delete the temp script after execution
5. Report SUCCESS/ERROR and show the output file path

---

## Quick Start

### Prerequisites

- **.NET SDK 8+**
- **dotnet-script** (required for Mode 2):

  ```bash
  dotnet tool install -g dotnet-script
  ```

- **Syncfusion License** — place your key in `SyncfusionLicense.txt` at the workspace root, or set the `SYNCFUSION_LICENSE_KEY` environment variable.
  Free license: [Syncfusion Community License](https://www.syncfusion.com/products/communitylicense)

### NuGet Packages (Mode 1 — user's project)

```bash
dotnet add package Syncfusion.Pdf.Net.Core
dotnet add package Syncfusion.Pdf.Imaging.Net.Core  # For Advanced PDF functions such as compress, redact, convert to PDF/A  and image processing related.
dotnet add package Syncfusion.XpsToPdfConverter.Net.Core # For XPS to PDF conversion.
```

---

## Code References

All templates and snippets used by the skill are in the `references/` folder:

| File | Contents |
| --- | --- |
| **template.csx** | Core CSX script template (used in Mode 2) |
| **document-structure.md** | Document lifecycle: create, save, close; sections and page setup |
| **open-pdf.md** | Open existing PDFs from file stream, byte array, encrypted files, corrupted documents, and cloud storage (Azure Blob, AWS S3, Google Drive, Google Cloud Storage, Dropbox) |
| **save-pdf.md** | Save new and loaded PDFs to file path, MemoryStream, byte array, and cloud storage (Azure Blob, AWS S3, Google Drive, Google Cloud Storage, Dropbox) |
| **pages.md** | Add, insert, remove, rotate, rearrange, import, and configure pages; page count, blank detection, page labels, section numbering, PageAdded event, and page-level actions |
| **pdf-graphics.md** | Working with PdfGraphics: text, images, and shapes |
| **shapes.md** | Draw shapes: lines, rectangles, ellipses, polygons, arcs, bezier curves, paths |
| **brushes.md** | Fill shapes with solid, linear gradient, radial gradient, tiling, and hatch brushes; PdfBrushes static colors; combine pen and brush |
| **images.md** | Insert, draw, replace, remove, clip, transform, and paginate raster images (JPEG, PNG, BMP, GIF, TIFF, ICO); image masking; multi-page TIFF to PDF; unit conversion for image placement |
| **text.md** | Draw text with standard, TrueType, OpenType, and CJK fonts; alignment, RTL, complex scripts, HTML styled text, multi-column, paginated text flow, ordered/unordered lists, string measurement, text clipping detection, and unit conversion |
| **merge-pdf.md** | Merge multiple PDFs into one file |
| **split-pdf.md** | Divide a single PDF into separate files |
| **compress-pdf.md** | Reduce and optimize PDF file size |
| **extract-text.md** | Retrieve text content from PDFs and find text within it |
| **extract-image.md** | Extract images and image metadata (bounds, index) from PDF pages and entire documents using PdfPageBase and PdfDocumentExtractor |
| **ocr.md** | Perform OCR on scanned PDFs and images using Tesseract; supports region OCR, rotated pages, layout result, page segmentation modes, engine modes, image enhancement, whitelist/blacklist, Unicode, and image-to-PDF conversion |
| **tables.md** | Build tables in PDFs using PdfGrid |
| **headers-and-footers.md** | Add headers and footers with automatic fields and dynamic content |
| **bookmarks.md** | Create and manage PDF bookmarks (outline navigation) |
| **attachments.md** | Add, manage, and extract file attachments in PDFs |
| **security.md** | Encrypt and protect PDFs with passwords and permissions |
| **actions.md** | Add interactive actions, triggers, and JavaScript to PDFs |
| **hyperlinks.md** | Add web URL links, internal document navigation, and external file links using PdfTextWebLink, PdfDocumentLinkAnnotation, and PdfFileLinkAnnotation |
| **watermarks.md** | Add text and image watermarks with transparency and rotation |
| **portfolio.md** | Create PDF portfolios embedding multiple files |
| **layers.md** | Create and manage layers (optional content) in PDFs |
| **metadata.md** | Work with document and image XMP metadata; properties schemas and custom fields |
| **redact.md** | Redaction examples and usage (text/image/pattern/regex-based redaction) |
| **digital-sign.md** | Digital signature examples: basic signing → advanced (TSA, LTV, external sign) |
| **import-export-annotation.md** | Import and export annotations (FDF, XFDF, JSON) — file, stream, and round-trip workflows |
| **pdf-forms.md** | Create, fill, modify, flatten, and manage AcroForm fields (text box, combo box, radio button, list box, check box, signature, button); covers field properties, visibility, read-only, auto-naming, complex script, and appearance |
| **import-export-form.md** | Import and export AcroForm field data (FDF, XFDF, JSON) — fill, export, round-trip, and flatten |
| **annotations.md** | Add, modify, remove, flatten PDF annotations (popup, free text, line, stamp, ink, markup, URI, redaction, cloud border, and more) |
| **barcode.md** | Add 1D (Code 39, EAN-13, EAN-8, Codabar, Code 93, Code 128, PDF417) and 2D (QR, DataMatrix) barcodes; export barcodes as images |
| **colorspace.md** | Work with DeviceGray, DeviceRGB, DeviceCMYK, and ICC-based color spaces for drawing and images |
| **conformance.md** | Produce PDF/A (1B, 2B, 3B) and PDF/X compliant files; convert existing PDFs to conformance standards |
| **named-destinations.md** | Add, modify, remove, and link named destinations for in-document and URL-based navigation |
| **pdf-templates.md** | Create and use PdfTemplate, PdfPageTemplateElement (header/footer), and PdfPageTemplate; create overlays and capture pages as templates |
| **accessible-pdf.md** | Create tagged PDFs, PDF/UA-2, Well-Tagged PDFs and Extract accessiblity tag elements |
| **tagged-pdf.md** | Create tagged (accessible/structured) PDFs with logical structure trees for screen-reader and reflow support |
| **zugferd-invoice.md** | Create ZUGFeRD electronic invoice PDFs (PDF/A-3b) with embedded XML; supports ZUGFeRD 1.0, 2.0, Factur-X, and XRechnung conformance levels; extract XML from existing ZUGFeRD PDFs |
| **xps-to-pdf.md** | Convert XPS (XML Paper Specification) documents to PDF using XPSToPdfConverter |

---

## Rules

- Output files go in `./output/` directory
- Temp `.csx` scripts must be created inside `{skill-root}/syncfusion-dotnet-pdf/scripts/` — never in the workspace root or customer `scripts/` folder
- Use license key from `SyncfusionLicense.txt` at workspace root
- Never use Python libraries (e.g., python-pdf)
- Never leave temp `.csx` files after execution
- Always use the latest NuGet package version
---

## Integration with GitHub Copilot

This skill is designed to work with GitHub Copilot in VS Code. Place the skill folder in `.github/skills/` of your repository.

When working with PDF documents, Copilot can automatically:

1. Route between Mode 1 (code generation) and Mode 2 (CSX execution)
2. Generate Syncfusion PDF code using the reference snippets
3. Execute CSX scripts to produce `.pdf` files on demand

### Example Prompts

#### Mode 1 / Code-Generation Prompts (C# snippets for your project)

*Use these when you want C# code to embed in your project (e.g., Program.cs).*

**Examples:**

- "Show me the C# code to create a PDF with a title, a heading, and a paragraph."
- "Generate a C# snippet to add a 3×4 table to a PDF document using Syncfusion PDF Library."
- "How do I add a header and footer with page numbers using Syncfusion PDF Library?"
- "Provide C# code to merge three PDF files into one using Syncfusion."
- "Show me how to extract text from all pages of a PDF using Syncfusion PDF Library."

#### Mode 2 / Document Generation Prompts (creates output PDF file)

*Use these when you want the skill to create/modify a PDF file immediately (via `.csx` script and `dotnet script`), or when you mention a file path like `output/report.pdf`.*

**Examples:**

- "Create a PDF document about the top 5 programming languages in 2025 and save it to output/."
- "Generate a meeting agenda document and save it to `output/agenda.pdf`."
- "Open `output/report.pdf` and apply a watermark to it."
- "Convert `output/report.pdf` to PDF/A-1B."
- "Merge `output/part1.pdf` and `output/part2.pdf` into `output/combined.pdf`."
- "Extract all images from `output/source.pdf` and save them to `output/images/`."

#### Complex / Multi-Step Prompt

- "Create a PDF report that includes a title page, a summary paragraph, and a table of product inventory (columns: ProductCode, ProductName, Category, Price, StockStatus).
Then:

1. Add a header with the document title and a footer with page numbers.
2. Add a semi‑transparent watermark on every page.
3. Save the result as output/inventory-report.pdf.
4. Also include a brief paragraph explaining that this is an automatically generated inventory report, and highlight that the data comes from a 'dynamic source'.”

**Key trigger distinction (for routing Mode 1 vs Mode 2):**
**Mode 1 triggers:** "show me", "code", "snippet", "how to", "example".
**Mode 2 triggers:** "create", "generate", "open", "modify", "convert", file paths (e.g. output/…), or explicit "run a csx script".

---

## Troubleshooting

| Issue | Solution |
| ----- | -------- |
| Missing NuGet package | `dotnet add package Syncfusion.Pdf.Net.Core` |
| License error | Add key to `SyncfusionLicense.txt` or register via `SyncfusionLicenseProvider.RegisterLicense()` |
| File access error | Check path, permissions, and ensure the file isn't open elsewhere |
| `dotnet script` not found | `dotnet tool install -g dotnet-script` |

---

## Resources

- [Syncfusion PDF Docs](https://help.syncfusion.com/document-processing/pdf/pdf-library/net/overview)
- [API Reference](https://help.syncfusion.com/cr/document-processing/Syncfusion.Pdf.html)
- [Syncfusion Community License](https://www.syncfusion.com/products/communitylicense)

---

## License

Syncfusion .NET PDF library requires a commercial license for production use. A [free community license](https://www.syncfusion.com/products/communitylicense) is available for qualifying organizations.
