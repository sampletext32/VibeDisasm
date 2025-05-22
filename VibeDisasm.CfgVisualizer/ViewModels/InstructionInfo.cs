namespace VibeDisasm.CfgVisualizer.ViewModels;

/// <summary>
/// Represents a single assembly instruction
/// </summary>
public class InstructionInfo
{
    /// <summary>
    /// Memory address of the instruction
    /// </summary>
    public ulong Address { get; }
    
    /// <summary>
    /// Instruction opcode (e.g., MOV, PUSH, CALL)
    /// </summary>
    public string Opcode { get; }
    
    /// <summary>
    /// Instruction operands
    /// </summary>
    public string Operands { get; }
    
    /// <summary>
    /// Raw bytes of the instruction
    /// </summary>
    public byte[] Bytes { get; }
    
    /// <summary>
    /// Constructor
    /// </summary>
    public InstructionInfo(ulong address, string opcode, string operands, byte[] bytes)
    {
        Address = address;
        Opcode = opcode;
        Operands = operands;
        Bytes = bytes;
    }
}
