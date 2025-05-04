using System.Diagnostics;
using System.Text;
using VibeDisasm.DecompilerEngine;
using VibeDisasm.DecompilerEngine.ControlFlow;
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

// ========== Decompiler Engine Code Analysis ==========
Console.WriteLine("\r\nDecompiler Engine - Code Offset Analysis:");
Console.WriteLine("===========================================");

// Create a list to store all definite code offsets
var definiteCodeOffsets = new List<(uint FileOffset, uint RVA, string Source, string Description)>();

// 1. Add entry point as definite code
if (peInfo.EntryPointRva > 0)
{
    uint entryPointOffset = Util.RvaToOffset(rawPeFile, peInfo.EntryPointRva);
    definiteCodeOffsets.Add((entryPointOffset, peInfo.EntryPointRva, "Entry Point", "Program entry point"));
}

// 2. Add exported functions as definite code
if (exportInfo != null && exportInfo.Functions.Count > 0)
{
    foreach (var exportedFunction in exportInfo.Functions)
    {
        // Skip forwarded exports (they don't have code in this file)
        if (exportedFunction.IsForwarded)
            continue;
            
        uint exportOffset = Util.RvaToOffset(rawPeFile, exportedFunction.RelativeVirtualAddress);
        definiteCodeOffsets.Add((exportOffset, exportedFunction.RelativeVirtualAddress, "Export", 
            $"Exported function: {exportedFunction.Name} (Ordinal: {exportedFunction.Ordinal})"));
    }
}

// 3. Add TLS callbacks as definite code (if present)
// TLS callbacks are executed before the entry point
if (rawPeFile.OptionalHeader.DataDirectories.Length > 9 && 
    rawPeFile.OptionalHeader.DataDirectories[9].VirtualAddress > 0)
{
    uint tlsDirectoryRva = rawPeFile.OptionalHeader.DataDirectories[9].VirtualAddress;
    uint tlsDirectoryOffset = Util.RvaToOffset(rawPeFile, tlsDirectoryRva);
    
    // TLS directory has a different structure in 32-bit and 64-bit PE files
    bool is64Bit = rawPeFile.OptionalHeader.Magic == 0x20B;
    
    // Read the TLS directory
    uint callbackArrayRva;
    if (is64Bit)
    {
        // In 64-bit PE files, the callback array RVA is at offset 24 in the TLS directory
        if (tlsDirectoryOffset + 24 + 8 <= fileData.Length)
        {
            callbackArrayRva = BitConverter.ToUInt32(fileData, (int)tlsDirectoryOffset + 24);
        }
        else
        {
            callbackArrayRva = 0;
        }
    }
    else
    {
        // In 32-bit PE files, the callback array RVA is at offset 12 in the TLS directory
        if (tlsDirectoryOffset + 12 + 4 <= fileData.Length)
        {
            callbackArrayRva = BitConverter.ToUInt32(fileData, (int)tlsDirectoryOffset + 12);
        }
        else
        {
            callbackArrayRva = 0;
        }
    }
    
    // If we have a callback array, read the callbacks
    if (callbackArrayRva > 0)
    {
        uint callbackArrayOffset = Util.RvaToOffset(rawPeFile, callbackArrayRva);
        int callbackIndex = 0;
        
        // Read each callback until we find a null entry
        while (true)
        {
            uint callbackRva;
            if (is64Bit)
            {
                // 64-bit PE files use 8-byte pointers
                if (callbackArrayOffset + (callbackIndex * 8) + 8 > fileData.Length)
                    break;
                    
                ulong callback = BitConverter.ToUInt64(fileData, (int)callbackArrayOffset + (callbackIndex * 8));
                if (callback == 0)
                    break;
                    
                // Convert the VA to an RVA by subtracting the image base
                callbackRva = (uint)(callback - rawPeFile.OptionalHeader.ImageBase);
            }
            else
            {
                // 32-bit PE files use 4-byte pointers
                if (callbackArrayOffset + (callbackIndex * 4) + 4 > fileData.Length)
                    break;
                    
                uint callback = BitConverter.ToUInt32(fileData, (int)callbackArrayOffset + (callbackIndex * 4));
                if (callback == 0)
                    break;
                    
                // Convert the VA to an RVA by subtracting the image base
                callbackRva = callback - (uint)rawPeFile.OptionalHeader.ImageBase;
            }
            
            uint callbackOffset = Util.RvaToOffset(rawPeFile, callbackRva);
            definiteCodeOffsets.Add((callbackOffset, callbackRva, "TLS Callback", $"TLS Callback #{callbackIndex}"));
            
            callbackIndex++;
        }
    }
}

// 4. Add exception handlers as definite code (if present)
if (rawPeFile.OptionalHeader.DataDirectories.Length > 3 && 
    rawPeFile.OptionalHeader.DataDirectories[3].VirtualAddress > 0)
{
    uint exceptionDirectoryRva = rawPeFile.OptionalHeader.DataDirectories[3].VirtualAddress;
    uint exceptionDirectorySize = rawPeFile.OptionalHeader.DataDirectories[3].Size;
    uint exceptionDirectoryOffset = Util.RvaToOffset(rawPeFile, exceptionDirectoryRva);
    
    // Exception directory contains an array of runtime function entries
    // Each entry is 12 bytes (3 DWORDs)
    int entryCount = (int)(exceptionDirectorySize / 12);
    
    for (int i = 0; i < entryCount; i++)
    {
        uint entryOffset = exceptionDirectoryOffset + (uint)(i * 12);
        
        // Make sure we don't read past the end of the file
        if (entryOffset + 12 > fileData.Length)
            break;
            
        // Read the function start RVA (first DWORD)
        uint functionStartRva = BitConverter.ToUInt32(fileData, (int)entryOffset);
        
        // Read the handler RVA (third DWORD)
        uint handlerRva = BitConverter.ToUInt32(fileData, (int)entryOffset + 8);
        
        // Add the function start as definite code
        uint functionStartOffset = Util.RvaToOffset(rawPeFile, functionStartRva);
        definiteCodeOffsets.Add((functionStartOffset, functionStartRva, "Exception Handler", 
            $"Function start for exception handler #{i}"));
            
        // Add the handler as definite code (if it's not 0)
        if (handlerRva > 0)
        {
            uint handlerOffset = Util.RvaToOffset(rawPeFile, handlerRva);
            definiteCodeOffsets.Add((handlerOffset, handlerRva, "Exception Handler", 
                $"Exception handler #{i}"));
        }
    }
}

// 5. Add import thunks as definite code (for delay-loaded imports)
if (rawPeFile.OptionalHeader.DataDirectories.Length > 13 && 
    rawPeFile.OptionalHeader.DataDirectories[13].VirtualAddress > 0)
{
    uint delayImportDirectoryRva = rawPeFile.OptionalHeader.DataDirectories[13].VirtualAddress;
    uint delayImportDirectoryOffset = Util.RvaToOffset(rawPeFile, delayImportDirectoryRva);
    
    // Process each delay-load import descriptor
    uint currentOffset = delayImportDirectoryOffset;
    while (currentOffset + 32 <= fileData.Length) // Each descriptor is 32 bytes
    {
        // Read the descriptor
        uint attributesField = BitConverter.ToUInt32(fileData, (int)currentOffset);
        uint nameRva = BitConverter.ToUInt32(fileData, (int)currentOffset + 4);
        uint moduleHandleRva = BitConverter.ToUInt32(fileData, (int)currentOffset + 8);
        uint delayImportAddressTableRva = BitConverter.ToUInt32(fileData, (int)currentOffset + 12);
        uint delayImportNameTableRva = BitConverter.ToUInt32(fileData, (int)currentOffset + 16);
        uint boundDelayImportTableRva = BitConverter.ToUInt32(fileData, (int)currentOffset + 20);
        uint unloadDelayImportTableRva = BitConverter.ToUInt32(fileData, (int)currentOffset + 24);
        uint timestampField = BitConverter.ToUInt32(fileData, (int)currentOffset + 28);
        
        // If all fields are 0, we've reached the end of the descriptor array
        if (nameRva == 0 && delayImportAddressTableRva == 0 && delayImportNameTableRva == 0)
            break;
            
        // The delay import address table contains thunks that are executed
        if (delayImportAddressTableRva > 0)
        {
            uint delayImportAddressTableOffset = Util.RvaToOffset(rawPeFile, delayImportAddressTableRva);
            
            // Read the module name
            string moduleName = "Unknown";
            if (nameRva > 0)
            {
                uint nameOffset = Util.RvaToOffset(rawPeFile, nameRva);
                moduleName = ReadNullTerminatedString(fileData, (int)nameOffset);
            }
            
            // Add the delay import address table as definite code
            definiteCodeOffsets.Add((delayImportAddressTableOffset, delayImportAddressTableRva, "Delay Import", 
                $"Delay import address table for {moduleName}"));
        }
        
        // Move to the next descriptor
        currentOffset += 32;
    }
}

// Print all definite code offsets
Console.WriteLine($"Found {definiteCodeOffsets.Count} definite code locations:");
Console.WriteLine("{0,-12} {1,-12} {2,-15} {3,-60}", "File Offset", "RVA", "Source", "Description");
Console.WriteLine(new string('-', 80));

foreach (var codeOffset in definiteCodeOffsets.OrderBy(o => o.FileOffset))
{
    Console.WriteLine("{0,-12:X8} {1,-12:X8} {2,-15} {3,-60}", 
        codeOffset.FileOffset, codeOffset.RVA, codeOffset.Source, codeOffset.Description);
}

var entryPointCodeOffset = definiteCodeOffsets.FirstOrDefault(x => x.Source == "Entry Point");

// Disassemble the entry point into basic blocks (control flow function)
var controlFlowFunction = ControlFlowBlockDisassembler.DisassembleBlock(fileData, entryPointCodeOffset.FileOffset);

Console.WriteLine("\nEntry point instruction blocks:");
foreach (var (offset, block) in controlFlowFunction.Blocks.OrderBy(x => x.Key))
{
    Console.WriteLine("{0,-10:X8}", offset);
    foreach (var blockInstruction in block.Instructions)
    {
        Console.WriteLine("\t{0}", blockInstruction);
    }
}

var cfgEdges = ControlFlowEdgesBuilder.Build(controlFlowFunction);

var diagram = MermaidDiagramGenerator.GenerateDiagram(controlFlowFunction, cfgEdges);

Console.WriteLine("Mermaid diagram----\n\n");

Console.WriteLine(diagram);

static string ReadNullTerminatedString(byte[] data, int offset)
{
    int length = 0;
    while (offset + length < data.Length && data[offset + length] != 0)
    {
        length++;
    }
    
    return Encoding.ASCII.GetString(data, offset, length);
}