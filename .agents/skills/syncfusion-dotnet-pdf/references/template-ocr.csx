#!/usr/bin/env dotnet-script
#r "nuget: Syncfusion.PDF.OCR.Net.Core, 33.1.43"
#r "nuget: Syncfusion.Licensing"

using System;
using System.IO;
using System.Runtime.InteropServices;
using Syncfusion.OCRProcessor;
using Syncfusion.Pdf.Parsing;
using Syncfusion.Licensing;

//  1. Register Syncfusion license
var licenseKey = Environment.GetEnvironmentVariable("SYNCFUSION_LICENSE_KEY");
if (string.IsNullOrWhiteSpace(licenseKey))
{
   var licenseFile = Path.Combine(Directory.GetCurrentDirectory(), "SyncfusionLicense.txt");
   if (File.Exists(licenseFile))
   {
       licenseKey = File.ReadAllText(licenseFile).Trim();
   }
}

if (!string.IsNullOrWhiteSpace(licenseKey))
{
   Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense(licenseKey);
}

// 2. Resolve Syncfusion OCR native runtime path from NuGet cache
string nugetRoot = Path.Combine(
    Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
    ".nuget",
    "packages");

// Change only if you update the NuGet version
string ocrPackageName = "syncfusion.pdf.ocr.net.core";
string ocrPackageVersion = "26.2.14";

// Resolve runtime identifier
string rid =
    RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "win-x64" :
    RuntimeInformation.IsOSPlatform(OSPlatform.Linux)   ? "linux-x64" :
    "osx-x64";

// Native OCR binaries path
string ocrNativePath = Path.Combine(
    nugetRoot,
    ocrPackageName,
    ocrPackageVersion,
    "runtimes",
    rid,
    "native");

// Get the Tessdata path (contains OCR language data files)
string tessdataPath = Path.Combine(
    nugetRoot,
    ocrPackageName,
    ocrPackageVersion,
    "runtimes",    
    "tessdata");

// Validate runtime path
if (!Directory.Exists(ocrNativePath))
{
    throw new DirectoryNotFoundException(
        $"OCR native runtime path not found: {ocrNativePath}");
}

// Validate tessdata path
if (!Directory.Exists(tessdataPath))
{
    throw new DirectoryNotFoundException(
        $"OCR native tessdata path not found: {tessdataPath}");
}

// Ensure output directory exists
Directory.CreateDirectory(Path.Combine(Directory.GetCurrentDirectory(), "output"));

try
{
    // *** INSERT OPERATION CODE HERE ***
    //Make sure to refer the path of ocrNativePath and tessdataPath in the code for OCR processing. as follows
    // var processor = new OCRProcessor(ocrNativePath)
    // {
    // necessary ocr processing code here, and set the tessdata path as well
    //processor.PerformOCR(document, tessdataPath);
    
    Console.WriteLine("SUCCESS: {output-file-path}");
}
catch (Exception ex)
{
    Console.Error.WriteLine($"ERROR: {ex.Message}");
    Environment.Exit(1);
}
