using System.Diagnostics;
using VibeDisasm.DecompilerEngine.IREverything.Expressions;
using VibeDisasm.DecompilerEngine.IREverything.Visitors;

namespace VibeDisasm.DecompilerEngine.IREverything.Instructions;

/// <summary>
/// Represents a subtract-with-borrow (SBB) instruction in IR.
/// Example: sbb eax, 1 -> IRSbbInstruction(eax, 1)
/// </summary>
[DebuggerDisplay("{DebugDisplay}")]
public sealed class IRSbbInstruction : IRInstruction
{
    public IRExpression Left { get; init; }
    public IRExpression Right { get; init; }
    public override IRExpression? Result => Left;

    public override IReadOnlyList<IRExpression> Operands => [Left, Right];

    public IRSbbInstruction(IRExpression left, IRExpression right)
    {
        Left = left;
        Right = right;
    }

    public override void Accept(IIRNodeVisitor visitor) => visitor.VisitSbb(this);

    public override T? Accept<T>(IIRNodeReturningVisitor<T> visitor) where T : default => visitor.VisitSbb(this);

    internal override string DebugDisplay => $"IRSbbInstruction({Left.DebugDisplay} -= {Right.DebugDisplay} - CF)";
}
