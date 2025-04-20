using System.Text;
using VibeDisasm.Pe.Extractors;
using VibeDisasm.Pe.Raw;
using VibeDisasm.TestLand.Printers;

// Path to the PE file to analyze
const string filePath = @"C:\Program Files (x86)\Nikita\Iron Strategy\Terrain.dll";
string fileName = Path.GetFileName(filePath);

// Read the file bytes
byte[] fileData = File.ReadAllBytes(filePath);

// Parse the PE file using the raw parser
RawPeFile rawPeFile = RawPeFactory.FromBytes(fileData);

// Create extractors for different PE information
var peInfoExtractor = new PeInfoExtractor(fileName);

// Extract basic PE information
PeInfo peInfo = peInfoExtractor.Extract(rawPeFile);

// Display basic information about the PE file
PeInfoPrinter.Print(peInfo);

// Create extractors for different section types (without including the raw data for brevity)
var allSectionsExtractor = new SectionExtractor { IncludeData = false };
var executableSectionsExtractor = new ExecutableSectionExtractor { IncludeData = false };
var codeSectionsExtractor = new CodeSectionExtractor { IncludeData = false };
var dataSectionsExtractor = new DataSectionExtractor { IncludeData = false };
var textSectionExtractor = new NamedSectionExtractor(".text") { IncludeData = false };

// Extract all sections
SectionInfo[] sections = allSectionsExtractor.Extract(rawPeFile);

// Display section information
SectionInfoPrinter.Print(sections);

// Extract executable sections
SectionInfo[] execSections = executableSectionsExtractor.Extract(rawPeFile);
SectionInfoPrinter.PrintCollection(execSections, "Executable Sections");

// Extract code sections
SectionInfo[] codeSections = codeSectionsExtractor.Extract(rawPeFile);
SectionInfoPrinter.PrintCollection(codeSections, "Code Sections");

// Extract a specific section with data
var textSectionWithDataExtractor = new NamedSectionExtractor(".text") { IncludeData = true };
SectionInfo? textSection = textSectionWithDataExtractor.Extract(rawPeFile);
SectionInfoPrinter.PrintDetails(textSection, ".text Section");

// Extract export information
var exportExtractor = new ExportExtractor();
ExportInfo? exportInfo = exportExtractor.Extract(rawPeFile);
ExportInfoPrinter.Print(exportInfo);

// Extract import information
var importExtractor = new ImportExtractor();
ImportInfo? importInfo = importExtractor.Extract(rawPeFile);
ImportInfoPrinter.Print(importInfo);

// Extract resource information
var resourceExtractor = new ResourceExtractor(includeData: true);
ResourceInfo? resourceInfo = resourceExtractor.Extract(rawPeFile);
ResourceInfoPrinter.Print(resourceInfo);

// Debug resource directory
Console.WriteLine("\r\nResource Directory Debug:");
if (rawPeFile.ResourceDirectory != null)
{
    Console.WriteLine($"  Resource Directory Found: Yes");
    Console.WriteLine($"  Named Entries: {rawPeFile.ResourceDirectory.NumberOfNamedEntries}");
    Console.WriteLine($"  ID Entries: {rawPeFile.ResourceDirectory.NumberOfIdEntries}");
}
else
{
    Console.WriteLine($"  Resource Directory Found: No");
}