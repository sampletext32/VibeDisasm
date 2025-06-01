using System.Diagnostics;
using VibeDisasm.DecompilerEngine.IREverything.Expressions;
using VibeDisasm.DecompilerEngine.IREverything.Visitors;

namespace VibeDisasm.DecompilerEngine.IREverything.Instructions;

/// <summary>
/// Represents a TEST instruction in IR which is commonly used to check if a register is zero.
/// Example: test eax, eax -> IRTestInstruction(eax, eax)
/// </summary>
[DebuggerDisplay("{DebugDisplay}")]
public sealed class IRTestInstruction : IRInstruction
{
    public IRExpression Left { get; init; }
    public IRExpression Right { get; init; }
    public override IRExpression? Result => null;
    public override IReadOnlyList<IRExpression> Operands => [Left, Right];

    public IRTestInstruction(IRExpression left, IRExpression right)
    {
        Left = left;
        Right = right;
    }

    public override void Accept(IIRNodeVisitor visitor) => visitor.VisitTest(this);

    public override T? Accept<T>(IIRNodeReturningVisitor<T> visitor) where T : default => visitor.VisitTest(this);

    internal override string DebugDisplay => $"IRTestInstruction(Test({Left.DebugDisplay}, {Right.DebugDisplay}))";
}
