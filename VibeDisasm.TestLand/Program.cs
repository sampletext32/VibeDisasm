using VibeDisasm.DecompilerEngine.IREverything;
using VibeDisasm.DecompilerEngine.IREverything.IRAnalyzers;
using VibeDisasm.DecompilerEngine.IREverything.Visitors;
using VibeDisasm.Disassembler.X86;
using VibeDisasm.Pe.Extractors;
using VibeDisasm.Pe.Raw;

// Path to the PE file to analyze
const string filePath = @"./DLLs/iron_3d.exe";
var fileName = Path.GetFileName(filePath);

// Read the file bytes
var fileData = File.ReadAllBytes(filePath);

// Parse the PE file using the raw parser
var rawPeFile = RawPeFactory.FromBytes(fileData);

// Extract basic PE information
var peInfo = PeInfoExtractor.Extract(rawPeFile);

// Extract all definite code offsets
var codeOffsetsInfo = CodeOffsetsExtractor.Extract(rawPeFile);

var entryPointCodeOffset = codeOffsetsInfo.Offsets.FirstOrDefault(x => x.Source == "Entry Point")!;

// Disassemble the entry point into basic blocks (control flow function)
var asmFunction = AsmFunctionDisassembler.DisassembleFunction(fileData, entryPointCodeOffset.FileOffset);

var irFunction = IrFromAsmConverter.Convert(asmFunction);

new WireJumpWithConditionAnalyzer().Handle(irFunction);
new IRFlagConditionReplacementAnalyzer().Handle(irFunction);
new SimpleIfThenAnalyzer().Handle(irFunction);

var code = CodeEmitVisitor.Instance.Visit(irFunction);

Console.WriteLine(code);

_ = 5;
