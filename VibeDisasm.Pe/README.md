# VibeDisasm.Pe

## Overview
VibeDisasm.Pe is a .NET library for parsing and analyzing Portable Executable (PE) files. It provides a comprehensive set of tools for extracting and working with various PE file components including sections, resources, imports, exports, and version information.

## Features

### PE File Parsing
- Complete PE header parsing (DOS header, NT headers, optional header)
- Section table analysis
- Data directory access

### Resource Extraction
- Resource directory traversal
- Resource entry extraction with file offset tracking
- Support for various resource types (icons, string tables, version info, etc.)

### String Table Processing
- Extract string tables from resources
- Access individual strings with their file offsets
- Support for different language IDs

### Import/Export Analysis
- Extract imported modules and functions
- Process exported functions
- Track ordinals and hints

### Version Information
- Extract file version information
- Access string file info (company name, product name, etc.)
- Parse fixed file info (file version, product version, etc.)

## Project Structure

### Extractors
The `Extractors` namespace contains classes for extracting specific information from PE files:

- `BaseSectionExtractor`: Base class for section extractors
- `ExportExtractor`: Extracts exported functions
- `ImportExtractor`: Extracts imported modules and functions
- `PeInfoExtractor`: Extracts basic PE file information
- `ResourceExtractor`: Extracts resources from the resource section
- `ResourceDirectoryExtractor`: Extracts resource directory information
- `SectionExtractor`: Extracts section information
- `StringTableExtractor`: Extracts string tables from resources
- `VersionExtractor`: Extracts version information

### Raw
The `Raw` namespace contains classes for working with raw PE file data:

- Data structures that match the PE file format
- Utilities for reading PE file structures
- Low-level access to PE file components

## File Offset Tracking
A key feature of VibeDisasm.Pe is its ability to track absolute file offsets for resources and strings. This allows for precise location of data within PE files, which is useful for:

- Forensic analysis
- Binary patching
- Detailed PE file mapping
- Resource extraction and modification

## Usage Examples

### Basic PE File Analysis
```csharp
// Load a PE file
var rawPeFile = RawPeFile.FromFile("example.exe");

// Extract basic information
var peInfo = PeInfoExtractor.Extract(rawPeFile);
Console.WriteLine($"Machine Type: {peInfo.Machine}");
Console.WriteLine($"Timestamp: {peInfo.TimeDateStamp}");
```

### Extracting Resources with File Offsets
```csharp
// Extract resources
var resourceInfo = ResourceExtractor.Extract(rawPeFile);

// Access resources with their file offsets
foreach (var resource in resourceInfo.Resources)
{
    Console.WriteLine($"Resource Type: {resource.Type}");
    Console.WriteLine($"Resource ID: {resource.Id}");
    Console.WriteLine($"File Offset: 0x{resource.FileOffset:X8}");
}
```

### Working with String Tables
```csharp
// Extract string tables from resources
var stringTables = StringTableExtractor.ExtractAll(resourceInfo);

// Access strings with their file offsets
foreach (var table in stringTables)
{
    Console.WriteLine($"String Table ID: {table.Id}");
    Console.WriteLine($"Table File Offset: 0x{table.FileOffset:X8}");
    
    foreach (var entry in table.Strings)
    {
        uint offset = table.StringFileOffsets[entry.Key];
        Console.WriteLine($"String ID: {entry.Key}, Offset: 0x{offset:X8}, Value: {entry.Value}");
    }
}
```

## Requirements
- .NET 8.0 or higher
- C# 12.0 or higher

## License
This project is licensed under the terms of the MIT license.
