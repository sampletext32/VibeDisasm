using System.Diagnostics;
using VibeDisasm.DecompilerEngine.IR.Expressions;
using VibeDisasm.DecompilerEngine.IR.Model;
using VibeDisasm.DecompilerEngine.IR.Visitors;

namespace VibeDisasm.DecompilerEngine.IR.Instructions;

/// <summary>
/// Represents an unsigned division instruction in IR.
/// Example: div ebx -> IRDivInstruction(eax, ebx, eax, edx)
/// EAX = EAX / src, EDX = EAX % src
/// </summary>
[DebuggerDisplay("{DebugDisplay}")]
public sealed class IRDivInstruction : IRInstruction
{
    public IRExpression Dividend { get; init; }
    public IRExpression Divisor { get; init; }
    public IRRegisterExpr DestQuotient { get; init; }
    public IRRegisterExpr DestRemainder { get; init; }
    public override IRExpression? Result => DestQuotient;
    public override IReadOnlyList<IRExpression> Operands => [Dividend, Divisor];

    public IRDivInstruction(IRExpression dividend, IRExpression divisor, IRRegisterExpr destQuotient, IRRegisterExpr destRemainder)
    {
        Dividend = dividend;
        Divisor = divisor;
        DestQuotient = destQuotient;
        DestRemainder = destRemainder;
    }

    public override void Accept(IIRNodeVisitor visitor) => visitor.VisitDiv(this);

    public override T? Accept<T>(IIRNodeReturningVisitor<T> visitor) where T : default => visitor.VisitDiv(this);

    internal override string DebugDisplay => $"IRDivInstruction({DestQuotient.DebugDisplay} = {Dividend.DebugDisplay} / {Divisor.DebugDisplay}; {DestRemainder.DebugDisplay} = {Dividend.DebugDisplay} % {Divisor.DebugDisplay})";
}
