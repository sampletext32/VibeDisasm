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

// Extract basic PE information
PeInfo peInfo = PeInfoExtractor.Extract(rawPeFile);

// Display basic information about the PE file
PeInfoPrinter.Print(peInfo, fileName);

// Extract all sections
SectionInfo[] sections = SectionExtractor.Extract(rawPeFile);

// Display section information
SectionInfoPrinter.Print(sections);

// Extract executable sections
SectionInfo[] execSections = CharacteristicsSectionExtractor.Extract(rawPeFile, SectionCharacteristics.Executable);
SectionInfoPrinter.PrintCollection(execSections, "Executable Sections");

// Extract code sections
SectionInfo[] codeSections = CharacteristicsSectionExtractor.Extract(rawPeFile, SectionCharacteristics.ContainsCode);
SectionInfoPrinter.PrintCollection(codeSections, "Code Sections");

// Extract a specific section with data
SectionInfo? textSection = NamedSectionExtractor.Extract(rawPeFile, ".text");
SectionInfoPrinter.PrintDetails(textSection, ".text Section");

// Extract export information
ExportInfo? exportInfo = ExportExtractor.Extract(rawPeFile);
ExportInfoPrinter.Print(exportInfo);

// Extract import information
ImportInfo? importInfo = ImportExtractor.Extract(rawPeFile);
ImportInfoPrinter.Print(importInfo);

// Extract resource information
ResourceInfo? resourceInfo = ResourceExtractor.Extract(rawPeFile);
ResourceInfoPrinter.Print(resourceInfo);

// Extract and print string tables if available
if (resourceInfo != null)
{
    var stringTables = StringTableExtractor.ExtractAll(resourceInfo);
    if (stringTables.Count > 0)
    {
        ResourceInfoPrinter.PrintStringTables(stringTables);
    }
    
    // Extract and print version information if available
    var versionInfos = VersionExtractor.ExtractAll(resourceInfo);
    if (versionInfos.Count > 0)
    {
        VersionInfoPrinter.Print(versionInfos);
    }
}

// Extract and print resource directory information
ResourceDirectoryInfo? resourceDirectoryInfo = ResourceDirectoryExtractor.Extract(rawPeFile);
ResourceDirectoryPrinter.Print(resourceDirectoryInfo);