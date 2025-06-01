using System.Diagnostics;
using VibeDisasm.DecompilerEngine.IREverything.Expressions;
using VibeDisasm.DecompilerEngine.IREverything.Visitors;

namespace VibeDisasm.DecompilerEngine.IREverything.Instructions;

/// <summary>
/// Represents a floating-point load instruction in IR.
/// Example: fld dword ptr [ebp+8] -> IRFldInstruction([ebp+8])
/// </summary>
[DebuggerDisplay("{DebugDisplay}")]
public sealed class IRFldInstruction : IRInstruction
{
    public IRExpression Source { get; init; }
    public override IRExpression? Result => new IRRegisterExpr(IRRegister.ST0); // FLD pushes to FPU stack, result is in ST(0)
    public override IReadOnlyList<IRExpression> Operands => [Source];

    public IRFldInstruction(IRExpression source)
    {
        Source = source;
    }

    public override void Accept(IIRNodeVisitor visitor) => visitor.VisitFld(this);

    public override T? Accept<T>(IIRNodeReturningVisitor<T> visitor) where T : default => visitor.VisitFld(this);

    internal override string DebugDisplay => $"IRFldInstruction(FPU_STACK <- {Source.DebugDisplay})";
}
