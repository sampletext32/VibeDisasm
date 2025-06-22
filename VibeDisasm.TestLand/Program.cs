// using System;
// using System.IO;
// using System.Text;
//

// using var fs = new FileStream("C:\\Users\\Admin\\Downloads\\vc12msvcprt.sig", FileMode.Open);
// using var binaryReader = new BinaryReader(fs);
//
// var header = ParseHeader(binaryReader);
//
// _ = 5;
//
// byte ReadU8(BinaryReader reader) => reader.ReadByte();
//
// ushort ReadU16LE(BinaryReader reader) => reader.ReadUInt16();
//
// uint ReadU32LE(BinaryReader reader) => reader.ReadUInt32();
//
// FlirtHeader ParseHeader(BinaryReader reader)
// {
//     var magic = reader.ReadBytes(6);
//     if (Encoding.ASCII.GetString(magic) != "IDASGN")
//     {
//         throw new FlirtException("Wrong file type");
//     }
//
//     byte version = ReadU8(reader);
//     if (version < 5 || version > 10)
//     {
//         throw new FlirtException($"Unknown version: {version}");
//     }
//
//     byte arch = ReadU8(reader);
//     uint fileTypes = ReadU32LE(reader);
//     ushort osTypes = ReadU16LE(reader);
//     ushort appTypes = ReadU16LE(reader);
//     ushort features = ReadU16LE(reader);
//     ushort oldNFunctions = ReadU16LE(reader);
//     ushort crc16 = ReadU16LE(reader);
//     byte[] ctype = reader.ReadBytes(12);
//     byte libraryNameLen = ReadU8(reader);
//     ushort ctypesCrc16 = ReadU16LE(reader);
//
//     uint? nFunctions = null;
//     ushort? patternSize = null;
//
//     if (version >= 6)
//     {
//         nFunctions = ReadU32LE(reader);
//         if (version >= 8)
//         {
//             patternSize = ReadU16LE(reader);
//             if (version > 9)
//             {
//                 ReadU16LE(reader); // unknown field
//             }
//         }
//     }
//
//     byte[] libraryNameBytes = reader.ReadBytes(libraryNameLen);
//
//     var libraryName = Encoding.ASCII.GetString(libraryNameBytes, 0, libraryNameLen);
//
//     return new FlirtHeader(version, arch, fileTypes, osTypes, appTypes, features, oldNFunctions, crc16,
//         ctype, ctypesCrc16, nFunctions, patternSize, libraryNameBytes, libraryName);
// }
//
//
// public class FlirtHeader
// {
//     public byte Version;
//     public byte Arch;
//     public uint FileTypes;
//     public ushort OsTypes;
//     public ushort AppTypes;
//     public ushort Features;
//     public ushort OldNFunctions;
//     public ushort Crc16;
//     public byte[] CType;
//     public ushort CTypesCrc16;
//     public uint? NFunctions;
//     public ushort? PatternSize;
//     public byte[] LibraryNameBytes;
//     public string LibraryName;
//
//     public FlirtHeader(byte version, byte arch, uint fileTypes, ushort osTypes, ushort appTypes, ushort features,
//         ushort oldNFunctions, ushort crc16, byte[] ctype, ushort ctypesCrc16, uint? nFunctions, ushort? patternSize,
//         byte[] libraryNameBytes, string libraryName)
//     {
//         LibraryName = libraryName;
//         Version = version;
//         Arch = arch;
//         FileTypes = fileTypes;
//         OsTypes = osTypes;
//         AppTypes = appTypes;
//         Features = features;
//         OldNFunctions = oldNFunctions;
//         Crc16 = crc16;
//         CType = ctype;
//         CTypesCrc16 = ctypesCrc16;
//         NFunctions = nFunctions;
//         PatternSize = patternSize;
//         LibraryNameBytes = libraryNameBytes;
//     }
// }
//
// public class FlirtException : Exception
// {
//     public FlirtException(string message) : base(message) { }
// }

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
