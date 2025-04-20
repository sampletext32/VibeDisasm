using System.Text;
using VibeDisasm.Pe.Raw;
using VibeDisasm.Pe.Raw.Structures;

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

// Display section information
Console.WriteLine("Sections:");
Console.WriteLine("{0,-10} {1,-10} {2,-10} {3,-10} {4,-10}", "Name", "VirtAddr", "VirtSize", "RawAddr", "RawSize");
Console.WriteLine(new string('-', 60));

foreach (var section in rawPeFile.SectionHeaders)
{
    Console.WriteLine("{0,-10} 0x{1:X8} 0x{2:X8} 0x{3:X8} 0x{4:X8}", 
        section.Name,
        section.VirtualAddress,
        section.VirtualSize,
        section.PointerToRawData,
        section.SizeOfRawData);
}

// Display export information if available
if (rawPeFile.ExportDirectory != null)
{
    Console.WriteLine("Exports:");
    Console.WriteLine($"Number of Functions: {rawPeFile.ExportDirectory.NumberOfFunctions}");
    Console.WriteLine($"Number of Names: {rawPeFile.ExportDirectory.NumberOfNames}");
}
else
{
    Console.WriteLine("No export directory found.");
}

// Display import information if available
if (rawPeFile.ImportDescriptors != null && rawPeFile.ImportDescriptors.Length > 0)
{
    Console.WriteLine("Imports:");
    Console.WriteLine($"Number of Import Descriptors: {rawPeFile.ImportDescriptors.Length}");
    
    // Create a directory parser to read import names
    var directoryParser = new RawPeDirectoryParser(rawPeFile);
    using var stream = new MemoryStream(rawPeFile.RawData);
    using var reader = new BinaryReader(stream);
    
    foreach (var importDesc in rawPeFile.ImportDescriptors)
    {
        string dllName = directoryParser.ReadAsciiString(reader, importDesc.Name);
        Console.WriteLine($"  {dllName}");
    }
}
else
{
    Console.WriteLine("No import descriptors found.");
}

// Example of getting raw section data
Console.WriteLine("First 16 bytes of .text section (if present):");
var textSection = Array.Find(rawPeFile.SectionHeaders, s => s.Name == ".text");
if (textSection != null)
{
    byte[] textData = rawPeFile.GetSectionData(textSection);
    Console.WriteLine(BitConverter.ToString(textData.Take(16).ToArray()).Replace("-", " "));
}
else
{
    Console.WriteLine("No .text section found.");
}

_ = 5;