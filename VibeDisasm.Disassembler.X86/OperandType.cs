namespace VibeDisasm.Disassembler.X86;

/// <summary>
/// Represents the type of operand in an x86 instruction
/// </summary>
public enum OperandType
{
    // Register operands
    Register,

    // Immediate values
    ImmediateValue,

    // Memory operands
    MemoryDirect,             // Direct memory address [addr]
    MemoryBaseReg,            // Base register [reg]
    MemoryBaseRegPlusOffset,  // Base register + offset [reg+offset]
    MemoryIndexed,            // Base + index*scale + offset [base+index*scale+offset]

    // Relative addresses
    RelativeOffset,           // Relative jump/call target
}
