using VibeDisasm.Disassembler.X86;

namespace VibeDisasm.DecompilerEngine.ControlFlow;

/// <summary>
/// Represents a directed edge in the control flow graph between two basic blocks.
/// </summary>
public class ControlFlowEdge
{
    /// <summary>Start address of the source basic block.</summary>
    public uint FromBlockAddress { get; set; }

    /// <summary>Start address of the target basic block.</summary>
    public uint ToBlockAddress { get; set; }

    /// <summary>Type of control flow transition between blocks.</summary>
    public AsmJumpType JumpType { get; set; }
}
