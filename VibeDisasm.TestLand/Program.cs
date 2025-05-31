using VibeDisasm.DecompilerEngine.IR;
using VibeDisasm.DecompilerEngine.IRAnalyzers;
using VibeDisasm.Disassembler.X86;
using VibeDisasm.Pe.Extractors;
using VibeDisasm.Pe.Raw;

// Path to the PE file to analyze
const string filePath = @"./DLLs/ArealMap.dll";
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
var asmFunction = AsmFunctionDisassembler.DisassembleFunction(fileData, 0xaed0);

var irFunction = IrFromAsmConverter.Convert(asmFunction);

new WireJumpWithConditionAnalyzer().Handle(irFunction);
new IRFlagConditionReplacementAnalyzer().Handle(irFunction);

Console.WriteLine(irFunction);

_ = 5;