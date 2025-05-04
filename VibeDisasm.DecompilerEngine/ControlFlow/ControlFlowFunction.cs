namespace VibeDisasm.DecompilerEngine.ControlFlow;

public class ControlFlowFunction
{
    public Dictionary<uint, ControlFlowBlock> Blocks { get; set; } = [];

    public ControlFlowGraph? ControlFlowGraph { get; set; }
}