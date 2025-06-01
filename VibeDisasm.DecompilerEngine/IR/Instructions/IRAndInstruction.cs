using System.Diagnostics;
using VibeDisasm.DecompilerEngine.IR.Expressions;
using VibeDisasm.DecompilerEngine.IR.Visitors;

namespace VibeDisasm.DecompilerEngine.IR.Instructions;

/// <summary>
/// Represents a bitwise AND instruction in IR.
/// Example: and eax, 1 -> IRAndInstruction(eax, 1)
/// </summary>
[DebuggerDisplay("{DebugDisplay}")]
public sealed class IRAndInstruction : IRInstruction
{
    public IRExpression Left { get; init; }
    public IRExpression Right { get; init; }
    public override IRExpression? Result => Left;
    public override IReadOnlyList<IRExpression> Operands => [Left, Right];

    public IRAndInstruction(IRExpression left, IRExpression right)
    {
        Left = left;
        Right = right;
    }

    public override void Accept(IIRNodeVisitor visitor) => visitor.VisitAnd(this);

    public override T? Accept<T>(IIRNodeReturningVisitor<T> visitor) where T : default => visitor.VisitAnd(this);

    internal override string DebugDisplay => $"IRAndInstruction({Left.DebugDisplay} &= {Right.DebugDisplay})";
}
