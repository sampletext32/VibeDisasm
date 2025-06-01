namespace VibeDisasm.DecompilerEngine.IREverything.Model;

/// <summary>
/// Represents a control flow edge in the CFG.
/// Example: Edge from block A to block B (jump, branch, call)
/// </summary>
public sealed class IRControlFlowEdge
{
    public required IRBlock Source { get; init; }
    public required IRBlock Target { get; init; }
    public required EdgeType Type { get; init; }

    public enum EdgeType { Jump, Branch, Call, Return }
}
