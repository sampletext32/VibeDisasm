using System.Text;

namespace VibeDisasm.DecompilerEngine.ControlFlow;

/// <summary>
/// Represents a basic block of instructions in the control flow graph
/// </summary>
public class ControlFlowBlock
{
    /// <summary>
    /// The starting address of the block
    /// </summary>
    public uint StartAddress { get; }

    public string ComputedStartAddressView { get; set; }

    public ControlFlowBlock(uint startAddress)
    {
        StartAddress = startAddress;

        ComputedStartAddressView = StartAddress.ToString($"0x{StartAddress:X8}");
    }

    /// <summary>
    /// The list of raw instructions in the block
    /// </summary>
    public List<ControlFlowInstruction> Instructions { get; set; } = [];
    
    /// <summary>
    /// Gets the last instruction in the block as a control flow instruction
    /// </summary>
    public ControlFlowInstruction? LastControlFlowInstruction => 
        Instructions.Count > 0 ? Instructions[^1] : null;
    
    /// <summary>
    /// Indicates if this block is the entry point of the function
    /// </summary>
    public bool IsEntryBlock { get; set; }

    /// <summary>
    /// Gets a string representation of the block
    /// </summary>
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