using System.Diagnostics;
using VibeDisasm.DecompilerEngine.IREverything.Expressions;
using VibeDisasm.DecompilerEngine.IREverything.Visitors;

namespace VibeDisasm.DecompilerEngine.IREverything.Instructions;

/// <summary>
/// Represents a signed division instruction in IR.
/// Example: idiv ebx -> IRIDivInstruction(eax, ebx, eax, edx)
/// EAX = EAX / src, EDX = EAX % src
/// </summary>
[DebuggerDisplay("{DebugDisplay}")]
public sealed class IRIDivInstruction : IRInstruction
{
    public IRExpression Dividend { get; init; }
    public IRExpression Divisor { get; init; }
    public IRRegisterExpr DestQuotient { get; init; }
    public IRRegisterExpr DestRemainder { get; init; }
    public override IRExpression? Result => DestQuotient;
    public override IReadOnlyList<IRExpression> Operands => [Dividend, Divisor];

    public override string ToString() => $"{DestQuotient} = {Dividend} / {Divisor}; {DestRemainder} = {Dividend} % {Divisor}";

    public IRIDivInstruction(IRExpression dividend, IRExpression divisor, IRRegisterExpr destQuotient, IRRegisterExpr destRemainder)
    {
        Dividend = dividend;
        Divisor = divisor;
        DestQuotient = destQuotient;
        DestRemainder = destRemainder;
    }

    public override void Accept(IIRNodeVisitor visitor) => visitor.VisitIDiv(this);

    public override T? Accept<T>(IIRNodeReturningVisitor<T> visitor) where T : default => visitor.VisitIDiv(this);

    internal override string DebugDisplay => $"IRIDivInstruction({DestQuotient.DebugDisplay} = {Dividend.DebugDisplay} / {Divisor.DebugDisplay}; {DestRemainder.DebugDisplay} = {Dividend.DebugDisplay} % {Divisor.DebugDisplay})";
}
