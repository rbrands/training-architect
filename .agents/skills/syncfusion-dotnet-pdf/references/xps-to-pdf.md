# XPS to PDF Conversion

Convert XPS (XML Paper Specification) documents to PDF using Syncfusion .NET PDF Library.

*Note: For document save and close patterns, see [document-structure.md](document-structure.md).*

---

**NuGet package:**

`Syncfusion.XpsToPdfConverter.Net.Core` - (.NET Core / ASP.NET Core)

**Common namespaces:**

```csharp
using Syncfusion.XPS;
using Syncfusion.Pdf;
```

---

## Convert an XPS file to PDF

```csharp
XPSToPdfConverter converter = new XPSToPdfConverter();
FileStream xpsStream = new FileStream("Input.xps", FileMode.Open, FileAccess.Read);
PdfDocument document = converter.Convert(xpsStream);
```

---

## Key APIs

| Member | Description |
| --- | --- |
| `XPSToPdfConverter` | Converts XPS documents to PDF |
| `XPSToPdfConverter.Convert(Stream)` | Accepts an XPS file stream and returns a `PdfDocument` |

---

## Notes

- The returned `PdfDocument` is a standard Syncfusion PDF document — apply further operations (security, watermarks, etc.) before saving.
- Use `Syncfusion.XpsToPdfConverter.Net.Core` NuGet package for .NET Core / ASP.NET Core projects.

---

## Related

- [document-structure.md](document-structure.md)
- [merge-pdf.md](merge-pdf.md)
- [conformance.md](conformance.md)
- ../SKILL.md

## Official documentation

- <https://help.syncfusion.com/document-processing/pdf/pdf-library/net/converting-xps-to-pdf>
