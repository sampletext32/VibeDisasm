using VibeDisasm.DecompilerEngine.IREverything;
using VibeDisasm.DecompilerEngine.IREverything.Cfg;
using VibeDisasm.DecompilerEngine.IREverything.IRAnalyzers;
using VibeDisasm.DecompilerEngine.IREverything.Visitors;
using VibeDisasm.Disassembler.X86;
using VibeDisasm.Pe.Extractors;
using VibeDisasm.Pe.Raw;

// Path to the PE file to analyze
const string filePath = @"./DLLs/ArealMap.dll";
var fileName = Path.GetFileName(filePath);

// Read the file bytes
var fileData = File.ReadAllBytes(filePath);

// Parse the PE file using the raw parser
var rawPeFile = RawPeFactory.FromBytes(fileData);

// Extract basic PE information
var peInfo = PeInfoExtractor.Extract(rawPeFile);

var resources = ResourceExtractor.Extract(rawPeFile);

var version = VersionExtractor.ExtractAll(rawPeFile, resources);

var stringTables = StringTableExtractor.ExtractAll(rawPeFile, resources);

// Extract all definite code offsets
var codeOffsetsInfo = CodeOffsetsExtractor.Extract(rawPeFile);

var entryPointCodeOffset = codeOffsetsInfo.Offsets.FirstOrDefault(x => x.Source == "Entry Point")!;

// Disassemble the entry point into basic blocks (control flow function)
var asmFunction = AsmFunctionDisassembler.DisassembleFunction(fileData, 0x2487);

var irFunction = IrFromAsmConverter.Convert(asmFunction);

new WireJumpWithConditionAnalyzer().Handle(irFunction);
new IRFlagConditionReplacementAnalyzer().Handle(irFunction);
new SimpleIfThenAnalyzer().Handle(irFunction);
new IfThenElseAnalyzer().Handle(irFunction);

var code = CodeEmitVisitor.Instance.Visit(irFunction);

Console.WriteLine(code);

_ = 5;
