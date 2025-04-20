# VibeDisasm.Disassembler.X86

## Overview
VibeDisasm.Disassembler.X86 is a comprehensive x86 instruction disassembler library written in C#. It provides a robust and extensible framework for disassembling x86 machine code with high accuracy and detailed operand information.

## Features

### Comprehensive Instruction Coverage
- Complete x86 instruction set support
- Arithmetic operations (ADD, SUB, MUL, DIV, etc.)
- Logical operations (AND, OR, XOR, NOT, etc.)
- Data movement (MOV, PUSH, POP, XCHG, etc.)
- Control flow (JMP, Jcc, CALL, RET, etc.)
- Floating-point operations (extensive FPU support)
- String operations (MOVS, STOS, LODS, etc.)
- Bit manipulation (BT, BTS, BTR, BSF, BSR)

### Advanced Decoding Capabilities
- Full prefix handling (REP/REPNE, segment overrides)
- All addressing modes supported:
  - Direct addressing
  - Register indirect
  - Base+displacement
  - Scale-Index-Base (SIB)
- ModR/M and SIB byte decoding
- Proper operand size handling (8-bit, 16-bit, 32-bit)

### Modular Architecture
- Handler-based instruction decoding
- Clean separation of concerns:
  - Instruction decoding
  - Prefix handling
  - Operand representation
  - ModR/M and SIB decoding
- Easily extensible for new instructions

## Project Structure

### Core Components
- `Disassembler.cs`: Main entry point for disassembling code
- `InstructionDecoder.cs`: Handles instruction decoding logic
- `ModRMDecoder.cs`: Decodes ModR/M bytes for addressing modes
- `SIBDecoder.cs`: Handles Scale-Index-Base byte decoding
- `PrefixDecoder.cs`: Processes instruction prefixes

### Instruction Handlers
Organized by instruction category in the `Handlers` directory:
- Arithmetic operations (Add, Sub, Mul, Div, etc.)
- Logical operations (And, Or, Xor, etc.)
- Data movement (Mov, Push, Pop, etc.)
- Control flow (Jump, Call, Ret, etc.)
- Floating-point operations
- String operations

### Operand System
Rich operand type hierarchy in the `Operands` directory:
- `RegisterOperand`: CPU registers
- `ImmediateOperand`: Immediate values
- `MemoryOperand`: Memory references
- `DirectMemoryOperand`: Direct memory addressing
- `BaseRegisterMemoryOperand`: Register-based addressing
- `ScaledIndexMemoryOperand`: SIB-based addressing

## Usage Examples

### Basic Disassembly
```csharp
// Create a byte array with x86 machine code
byte[] codeBuffer = new byte[] { 0x8B, 0x45, 0x08, 0x01, 0xC8, 0xC3 };

// Create a disassembler with a base address
var disassembler = new Disassembler(codeBuffer, 0x1000);

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

### Output
```
0x00001000: Mov
  EAX
  [EBP+8]
0x00001003: Add
  EAX
  ECX
0x00001005: Ret
```

### Integration with PE File Analysis
```csharp
// Load a PE file
var rawPeFile = RawPeFile.FromFile("example.exe");

// Get the entry point and code section
var peInfo = PeInfoExtractor.Extract(rawPeFile);
var textSection = SectionExtractor.FindSection(rawPeFile, ".text");

// Calculate the entry point RVA relative to the .text section
uint entryPointRva = peInfo.AddressOfEntryPoint;
uint codeOffset = entryPointRva - textSection.VirtualAddress;

// Extract code bytes from the .text section
byte[] codeBytes = new byte[Math.Min(1000, textSection.Data.Length - (int)codeOffset)];
Array.Copy(textSection.Data, (int)codeOffset, codeBytes, 0, codeBytes.Length);

// Create a disassembler with the entry point as base address
var disassembler = new Disassembler(codeBytes, entryPointRva);

// Disassemble the entry point code
var instructions = disassembler.Disassemble();

// Print the disassembled instructions
foreach (var instruction in instructions)
{
    Console.WriteLine($"0x{instruction.Address:X8}: {instruction.Type}");
}
```

## Extending the Disassembler

### Adding a New Instruction Handler
1. Create a new handler class in the appropriate category directory
2. Implement the `IInstructionHandler` interface
3. Add the handler to the `InstructionHandlerFactory`

Example:
```csharp
public class MyNewInstructionHandler : InstructionHandler
{
    public MyNewInstructionHandler(InstructionDecoder decoder) : base(decoder)
    {
    }

    public override bool Decode(byte opcode, Instruction instruction)
    {
        // Decode the instruction
        instruction.Type = InstructionType.MyNewInstruction;
        
        // Add operands
        instruction.StructuredOperands = [
            OperandFactory.CreateRegisterOperand(RegisterIndex.EAX),
            DecodeModRmOperand(32)
        ];
        
        return true;
    }
}
```

## Requirements
- .NET 8.0 or higher
- C# 12.0 or higher

## License
This project is released into the public domain under the Unlicense. See the main repository [LICENSE](../LICENSE) file for details.
