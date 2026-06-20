---
name: syncfusion-dotnet-pdf
description: Create, read, edit, secure, sign, and convert PDF documents (.pdf) using Syncfusion PDF Library for .NET. Use this skill for PDF processing and document automation when the user asks to generate PDF files, modify PDF content, add security or signatures, extract text or images, merge or split PDFs, or perform PDF/A conversion using C# code or CSX execution.
metadata:
  author: Syncfusion Inc
  version: "33.1.44"
---

## PDF Document Processing
 
## Overview
 
Create, read, write, and convert PDF files using the Syncfusion PDF Library. This skill supports two operational modes — generating C# code for the user's project or executing tasks directly through a CSX script.
 
## Key Capabilities
 
- **Create & Edit:** PDF files from scratch, text, images (various formats), tables, shapes, paragraphs, headings, styles, lists, hyperlinks, bookmarks, headers/footers, watermarks, template management, metadata editing
- **Forms & Interactive Elements:** Create, fill, and flatten forms (AcroForms and XFA), bookmarks, annotations, attachments, buttons, and content controls
- **Advanced Features:** Comments, layers (add, remove, flatten), PDF portfolios, JavaScript execution, 3D model embedding and interaction, rich media content (audio/video), optical character recognition (Tesseract engine), text redaction, image redaction, digital signatures and validation
- **Conversion:** XPS to PDF, PDF to PDF/A conformance, extract text and images from PDF documents
- **Security:** Password encryption/decryption, advanced encryption standards, document protection with editable ranges, macro management, digital signing capabilities
- **Barcodes & Standards:** 1D barcodes, 2D barcodes, ZUGFeRD invoice support, PDF/A-1B, PDF/A-1A, PDF/A-2B, PDF/A-2A, PDF/A-2U, PDF/A-3B, PDF/A-3A, PDF/A-3U, PDF/A-4, PDF/A-4E, PDF/A-4F, PDF/X1-A conformances, Accessible PDF/Tagged PDF (PDF/UA) with Section 508 compliance
- **Additional Operations:** Merge and split PDF files, open and modify existing PDF files, compress PDF files, corrupted PDF detection.

## Quick Start Examples

### Example 1: Generate Code (Mode 1)
**User:** "Show me the C# code to create a PDF with a title, a heading, and a paragraph."
**Result:** C# code snippet displayed (no files created)

### Example 2: Execute Task (Mode 2)
**User:** "Generate a meeting agenda document and save it to `output/agenda.pdf`."
**Result:** Physical file created at specified path

## Two Modes — Choose Based on User Intent

### Mode 1: Generate C# Code for the User's Project *(default)*

**Trigger keywords:** "code", "snippet", "how to write", "Program.cs", "show me", "sample", "example code", "generate code for".

**Workflow:**
1. Build the C# code using snippets from `references/*.md` files and replace placeholders with actual content
2. Add the code directly into the user's project files (e.g., `Program.cs`)
3. Do **not** create or run any `.csx` script

Note: Use `Syncfusion.Drawing` namespace for this code for cross-platform projects, not `System.Drawing`. This ensures compatibility across Windows, macOS, and Linux. 

Note: If `Syncfusion.Pdf.Net.Core` package is suggested, then must be use the `Syncfusion.Drawing` namespace for graphics operations instead of `System.Drawing` to ensure cross-platform compatibility. The `Syncfusion.Pdf.Net.Core` package is designed to work with the `Syncfusion.Drawing` namespace, which provides a consistent API for drawing operations across different platforms. Using `System.Drawing` may lead to compatibility issues, especially on non-Windows platforms, as it relies on GDI+ which is not fully supported outside of Windows. Therefore, always use `Syncfusion.Drawing` when working with PDF graphics in a cross-platform context.

Note: If RectangleF, PointF, SizeF, or Color types are needed, use the ones from `Syncfusion.Drawing` namespace for cross-platform compatibility, not the ones from `System.Drawing`. This ensures that the code will work correctly on Windows, macOS, and Linux without relying on GDI+.

---

### Mode 2: Execute via CSX Script *(does not touch project files)*

**Trigger keywords:** "create a pdf document", "make a document", "generate a document", "open", "edit", "modify", "change" a `.pdf` file, "without modifying my project", "run a csx script", or when the user provides a file path (e.g., `output/report.pdf`).

**Workflow:**
1. Create `{skill-root}/syncfusion-dotnet-pdf/scripts/temp-{timestamp}.csx` using `references/template.csx` as the base or `references/template-ocr.csx` for OCR-specific operations. Example, `skill-root` = `.github/skills`.
2. Add required operations from `references/*.md` snippets and replace placeholders with actual content
3. Run: `dotnet script {skill-root}/syncfusion-dotnet-pdf/scripts/temp-{timestamp}.csx`
4. Delete the temp script after execution
5. Report SUCCESS/ERROR and show the output file path

Note: Use `Syncfusion.Drawing` namespace for this csx script, not `System.Drawing`.

---

## Code References

All templates and snippets are in the `references/` folder:

| File | Contents |
|---|---|
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
| **conversions.md** | Conversion guidance for scenarios such as complex HTML-to-PDF conversion that require converter APIs instead of direct drawing APIs |
| **merge-pdf.md** | Merge multiple PDFs into one file |
| **split-pdf.md** | Divide a single PDF into separate files |
| **compress-pdf.md** | Reduce and optimize PDF file size |
| **extract-text.md** | Retrieve text content from PDFs and find text within it|
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

## Prerequisites

- .NET SDK 8+ and `dotnet-script`: `dotnet tool install -g dotnet-script`
- Syncfusion License: `SyncfusionLicense.txt` or env var `SYNCFUSION_LICENSE_KEY`
- Free license: https://www.syncfusion.com/products/communitylicense

## Documentations
- [Syncfusion PDF Docs](https://help.syncfusion.com/document-processing/pdf/pdf-library/net/overview)
- [API Reference](https://help.syncfusion.com/cr/document-processing/Syncfusion.Pdf.html)