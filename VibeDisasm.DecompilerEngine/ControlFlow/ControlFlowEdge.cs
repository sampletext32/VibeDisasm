using VibeDisasm.Disassembler.X86;

namespace VibeDisasm.DecompilerEngine.ControlFlow;

public class ControlFlowEdge
{
    public uint FromBlockAddress { get; set; }

    public uint ToBlockAddress { get; set; }

    public AsmJumpType JumpType { get; set; }
}