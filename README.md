# VibeDisasm

[![Build and Test](https://github.com/sampletext32/VibeDisasm/actions/workflows/build-and-test.yml/badge.svg)](https://github.com/sampletext32/VibeDisasm/actions/workflows/build-and-test.yml)

## Overview
VibeDisasm is a comprehensive .NET toolkit for analyzing and disassembling executable files, with a focus on the Portable Executable (PE) format and x86 instruction set. The project provides tools for extracting detailed information from PE files and disassembling x86 machine code with high precision and fidelity.

## Project Components

### VibeDisasm.Pe
A library for parsing and analyzing Portable Executable (PE) files, providing detailed access to:

- PE headers and sections
- Import and export tables
- Resources with absolute file offset tracking
- String tables and version information
- Section characteristics and memory layout

[View VibeDisasm.Pe Documentation](VibeDisasm.Pe/README.md)

### VibeDisasm.Disassembler.X86
A powerful x86 instruction disassembler that supports:

- Comprehensive x86 instruction set coverage
- Proper handling of instruction prefixes
- All addressing modes (direct, register indirect, SIB, etc.)
- Floating-point instructions
- String operations and bit manipulation

### VibeDisasm.TestLand
A test application that demonstrates the capabilities of the libraries, including:

- PE file analysis with detailed output
- Resource extraction and display
- String table and version information extraction
- Formatted output of PE file components

## Key Features

### PE File Analysis
- Complete PE header parsing
- Resource extraction with file offset tracking
- Import and export table analysis
- Section analysis and characteristics

### x86 Disassembly
- Modular, handler-based instruction decoding
- Support for the almost full x86 instruction set
- Proper operand representation
- Prefix handling (REP/REPNE, segment overrides)

## Getting Started

### Prerequisites
- .NET 8.0 or higher
- C# 12.0 or higher

### Building the Project
```bash
dotnet build
```

### Running the Test Application
```bash
dotnet run --project VibeDisasm.TestLand
```

## Usage Examples

### Analyzing a PE File
```csharp
// Load a PE file
var rawPeFile = RawPeFile.FromFile("example.exe");

// Extract basic information
var peInfo = PeInfoExtractor.Extract(rawPeFile);
Console.WriteLine($"Machine Type: {peInfo.Machine}");
Console.WriteLine($"Timestamp: {peInfo.TimeDateStamp}");

// Extract resources with file offsets
var resourceInfo = ResourceExtractor.Extract(rawPeFile);
foreach (var resource in resourceInfo.Resources)
{
    Console.WriteLine($"Resource Type: {resource.Type}");
    Console.WriteLine($"Resource ID: {resource.Id}");
    Console.WriteLine($"File Offset: 0x{resource.FileOffset:X8}");
}
```

### Disassembling x86 Code
```csharp
// Create a disassembler for a code buffer
var disassembler = new Disassembler(codeBuffer, baseAddress);

// Disassemble the code
var instructions = disassembler.Disassemble();

// Print the disassembled instructions
foreach (var instruction in instructions)
{
    Console.WriteLine($"0x{instruction.Address:X8}: {instruction.Type}");
    
    // Print operands
    foreach (var operand in instruction.StructuredOperands)
    {
        Console.WriteLine($"  {operand}");
    }
}
```

## License
This project is released into the public domain under the Unlicense - see the [LICENSE](LICENSE) file for details.

## Acknowledgments
- The PE format specification from Microsoft
- Intel's x86 instruction set documentation
