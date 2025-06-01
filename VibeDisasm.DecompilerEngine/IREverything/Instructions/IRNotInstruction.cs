using System.Diagnostics;
using VibeDisasm.DecompilerEngine.IREverything.Expressions;
using VibeDisasm.DecompilerEngine.IREverything.Visitors;

namespace VibeDisasm.DecompilerEngine.IREverything.Instructions;

/// <summary>
/// Represents a bitwise NOT instruction in IR.
/// Example: not eax -> IRNotInstruction(eax)
/// </summary>
[DebuggerDisplay("{DebugDisplay}")]
public sealed class IRNotInstruction : IRInstruction
{
    public IRExpression Operand { get; init; }
    public override IRExpression? Result => Operand;
    public override IReadOnlyList<IRExpression> Operands => [Operand];

    public IRNotInstruction(IRExpression operand) => Operand = operand;

    public override void Accept(IIRNodeVisitor visitor) => visitor.VisitNot(this);

    public override T? Accept<T>(IIRNodeReturningVisitor<T> visitor) where T : default => visitor.VisitNot(this);

    internal override string DebugDisplay => $"IRNotInstruction({Operand.DebugDisplay} = ~{Operand.DebugDisplay})";
}
