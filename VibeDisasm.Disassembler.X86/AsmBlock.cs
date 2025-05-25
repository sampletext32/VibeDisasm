using System.Text;

namespace VibeDisasm.Disassembler.X86;

/// <summary>Basic block in the control flow graph.</summary>
public class AsmBlock
{
    /// <summary>The starting address of the block</summary>
    public uint StartAddress { get; }

    public string ComputedStartAddressView { get; set; }

    /// <summary>The list of raw instructions in the block</summary>
    public List<AsmInstruction> Instructions { get; set; } = [];
    
    /// <summary>Gets the last instruction in the block</summary>
    public AsmInstruction? LastControlFlowInstruction => 
        Instructions.Count > 0 ? Instructions[^1] : null;
    
    /// <summary>Indicates if this block is the entry point of the function</summary>
    public bool IsEntryBlock { get; set; }

    public AsmBlock(uint startAddress)
    {
        StartAddress = startAddress;

        ComputedStartAddressView = StartAddress.ToString("X8");
    }

    /// <summary>Gets a string representation of the block</summary>
    public override string ToString()
    {
        var sb = new StringBuilder();
        sb.AppendLine($"Block at {StartAddress:X8}:");
        
        foreach (var instruction in Instructions)
        {
            sb.AppendLine($"  {instruction.Address:X8}: {instruction}");
        }
        
        return sb.ToString();
    }
}