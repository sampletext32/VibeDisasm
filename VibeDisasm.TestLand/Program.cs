using System.Text;
using VibeDisasm.Pe.Extractors;
using VibeDisasm.Pe.Raw;

// Path to the PE file to analyze
const string filePath = @"C:\Program Files (x86)\Nikita\Iron Strategy\Terrain.dll";

// Read the file bytes
byte[] fileData = File.ReadAllBytes(filePath);

// Parse the PE file using the raw parser
RawPeFile rawPeFile = RawPeFactory.FromBytes(fileData);

// Display basic information about the PE file
Console.WriteLine($"PE File: {Path.GetFileName(filePath)}");
Console.WriteLine($"Architecture: {(rawPeFile.IsPe32Plus ? "64-bit" : "32-bit")}");
Console.WriteLine($"Entry Point RVA: 0x{rawPeFile.OptionalHeader.AddressOfEntryPoint:X8}");
Console.WriteLine($"Number of Sections: {rawPeFile.FileHeader.NumberOfSections}");

// Create extractors for different section types (without including the raw data for brevity)
var allSectionsExtractor = new SectionExtractor { IncludeData = false };
var executableSectionsExtractor = new ExecutableSectionExtractor { IncludeData = false };
var codeSectionsExtractor = new CodeSectionExtractor { IncludeData = false };
var dataSectionsExtractor = new DataSectionExtractor { IncludeData = false };
var textSectionExtractor = new NamedSectionExtractor(".text") { IncludeData = false };

// Extract all sections
SectionInfo[] sections = allSectionsExtractor.Extract(rawPeFile);

// Display section information
Console.WriteLine("All Sections:");
Console.WriteLine("{0,-10} {1,-10} {2,-10} {3,-10} {4,-15}", "Name", "VirtAddr", "VirtSize", "RawAddr", "Properties");
Console.WriteLine(new string('-', 65));

foreach (var section in sections)
{
    string properties = string.Empty;
    if (section.IsExecutable) properties += "X";
    if (section.IsReadable) properties += "R";
    if (section.IsWritable) properties += "W";
    if (section.ContainsCode) properties += " Code";
    if (section.ContainsInitializedData) properties += " Data";
    
    Console.WriteLine("{0,-10} 0x{1:X8} 0x{2:X8} 0x{3:X8} {4,-15}", 
        section.Name,
        section.VirtualAddress,
        section.VirtualSize,
        section.RawDataAddress,
        properties.Trim());
}

// Extract executable sections
SectionInfo[] execSections = executableSectionsExtractor.Extract(rawPeFile);

Console.WriteLine("\nExecutable Sections:");
if (execSections.Length > 0)
{
    foreach (var section in execSections)
    {
        Console.WriteLine($"  {section.Name} (0x{section.VirtualAddress:X8})");
    }
}
else
{
    Console.WriteLine("  No executable sections found.");
}

// Extract code sections
SectionInfo[] codeSections = codeSectionsExtractor.Extract(rawPeFile);

Console.WriteLine("\nCode Sections:");
if (codeSections.Length > 0)
{
    foreach (var section in codeSections)
    {
        Console.WriteLine($"  {section.Name} (0x{section.VirtualAddress:X8})");
    }
}
else
{
    Console.WriteLine("  No code sections found.");
}

// Extract a specific section by name
SectionInfo? textSection = textSectionExtractor.Extract(rawPeFile);

Console.WriteLine("\n.text Section Details:");
if (textSection != null)
{
    Console.WriteLine($"  Virtual Address: 0x{textSection.VirtualAddress:X8}");
    Console.WriteLine($"  Virtual Size: 0x{textSection.VirtualSize:X8}");
    Console.WriteLine($"  Raw Data Address: 0x{textSection.RawDataAddress:X8}");
    Console.WriteLine($"  Raw Data Size: 0x{textSection.RawDataSize:X8}");
    Console.WriteLine($"  Is Executable: {textSection.IsExecutable}");
    Console.WriteLine($"  Is Readable: {textSection.IsReadable}");
    Console.WriteLine($"  Is Writable: {textSection.IsWritable}");
    Console.WriteLine($"  Contains Code: {textSection.ContainsCode}");
}
else
{
    Console.WriteLine("  .text section not found.");
}

// Extract a specific section with data
var textSectionWithDataExtractor = new NamedSectionExtractor(".text") { IncludeData = true };
textSection = textSectionWithDataExtractor.Extract(rawPeFile);

Console.WriteLine("\nFirst 16 bytes of .text section:");
if (textSection != null && textSection.Data.Length > 0)
{
    Console.WriteLine(BitConverter.ToString(textSection.Data.Take(16).ToArray()).Replace("-", " "));
}
else
{
    Console.WriteLine("  No data available.");
}

_ = 5;