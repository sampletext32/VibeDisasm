using System.Diagnostics;
using System.Text;
using VibeDisasm.DecompilerEngine;
using VibeDisasm.DecompilerEngine.ControlFlow;
using VibeDisasm.DecompilerEngine.IR;
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

// Extract all definite code offsets
CodeOffsetsInfo codeOffsetsInfo = CodeOffsetsExtractor.Extract(rawPeFile);

var entryPointCodeOffset = codeOffsetsInfo.Offsets.FirstOrDefault(x => x.Source == "Entry Point")!;

// Disassemble the entry point into basic blocks (control flow function)
var controlFlowFunction = ControlFlowBlockDisassembler.DisassembleBlock(fileData, entryPointCodeOffset.FileOffset);

// Build IR from the control flow function
var irBuilder = new IRBuilder();
var irFunction = irBuilder.BuildFromFunction(controlFlowFunction);
irFunction.Name = "EntryPoint";

// Print the IR
var irPrinter = new IRPrinter();
var irStr = irPrinter.Print(irFunction);

Console.WriteLine(irStr);

// Print raw instructions for a specific block (for debugging)
Console.WriteLine("\nRaw instructions for block 0x5A6F3:");
if (controlFlowFunction.Blocks.TryGetValue(0x5A6F3, out var block))
{
    foreach (var instruction in block.Instructions)
    {
        Console.WriteLine($"  {instruction.Address:X8}: {instruction.RawInstruction.Type} {string.Join(", ", instruction.RawInstruction.StructuredOperands)}");
    }
}
else
{
    Console.WriteLine("  Block not found");
}

_ = 5;