using VibeDisasm.DecompilerEngine.IREverything.Expressions;
using VibeDisasm.DecompilerEngine.IREverything.Structuring;

namespace VibeDisasm.DecompilerEngine.IREverything.Model;

/// <summary>
/// Represents a control flow edge in the CFG.
/// </summary>
public sealed class IRControlFlowEdge
{
    public required IRBlock Source { get; init; }
    public required IRBlock Target { get; init; }
    public required EdgeType Type { get; init; }
    public IRExpression? Condition { get; init; }

    public enum EdgeType { Unconditional, ConditionalTrue, ConditionalFalse, Fallthrough }
}
