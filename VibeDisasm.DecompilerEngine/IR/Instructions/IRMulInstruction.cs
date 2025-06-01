using System.Diagnostics;
using VibeDisasm.DecompilerEngine.IR.Expressions;
using VibeDisasm.DecompilerEngine.IR.Visitors;

namespace VibeDisasm.DecompilerEngine.IR.Instructions;

/// <summary>
/// Represents a multiplication instruction in IR.
/// Example: mul eax, 2 -> IRMulInstruction(eax, 2)
/// </summary>
[DebuggerDisplay("{DebugDisplay}")]
public sealed class IRMulInstruction : IRInstruction
{
    public IRExpression Left { get; init; }
    public IRExpression Right { get; init; }
    public override IRExpression? Result => Left;
    public override IReadOnlyList<IRExpression> Operands => [Left, Right];

    public IRMulInstruction(IRExpression left, IRExpression right)
    {
        Left = left;
        Right = right;
    }

    public override void Accept(IIRNodeVisitor visitor) => visitor.VisitMul(this);

    public override T? Accept<T>(IIRNodeReturningVisitor<T> visitor) where T : default => visitor.VisitMul(this);

    internal override string DebugDisplay => $"IRMulInstruction({Left.DebugDisplay} *= {Right.DebugDisplay})";
}
