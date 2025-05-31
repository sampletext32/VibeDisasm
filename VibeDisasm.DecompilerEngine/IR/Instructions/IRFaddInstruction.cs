using System.Diagnostics;
using VibeDisasm.DecompilerEngine.IR.Expressions;
using VibeDisasm.DecompilerEngine.IR.Visitors;

namespace VibeDisasm.DecompilerEngine.IR.Instructions;

/// <summary>
/// Represents a floating-point add instruction in IR.
/// Example: fadd dword ptr [ebp+8] -> IRFaddInstruction([ebp+8])
/// </summary>
[DebuggerDisplay("{DebugDisplay}")]
public sealed class IRFaddInstruction : IRInstruction
{
    public IRExpression Source { get; init; }
    public override IRExpression? Result => new IRRegisterExpr(IRRegister.ST0); // FADD modifies ST(0)
    public override IReadOnlyList<IRExpression> Operands => [Source];

    public IRFaddInstruction(IRExpression source)
    {
        Source = source;
    }

    public override void Accept(IIRNodeVisitor visitor) => visitor.VisitFadd(this);

    public override T? Accept<T>(IIRNodeReturningVisitor<T> visitor) where T : default => visitor.VisitFadd(this);

    internal override string DebugDisplay => $"IRFaddInstruction(ST(0) += {Source.DebugDisplay})";
}
