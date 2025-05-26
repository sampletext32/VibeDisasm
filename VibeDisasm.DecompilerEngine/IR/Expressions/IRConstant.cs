using VibeDisasm.DecompilerEngine.IR.Model;

namespace VibeDisasm.DecompilerEngine.IR.Expressions;

/// <summary>
/// Represents a constant value in IR.
/// Example: 5 -> IRConstant(5)
/// </summary>
public sealed class IRConstant : IRExpression
{
    public required object Value { get; init; }
    public required IRType Type { get; init; }
}
