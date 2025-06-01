using System.Diagnostics;
using VibeDisasm.DecompilerEngine.IREverything.Expressions;
using VibeDisasm.DecompilerEngine.IREverything.Visitors;

namespace VibeDisasm.DecompilerEngine.IREverything.Instructions;

/// <summary>
/// Represents a LEA (load effective address) instruction in IR.
/// Example: lea eax, [ebx+4] -> IRLeaInstruction(eax, [ebx+4])
/// </summary>
[DebuggerDisplay("{DebugDisplay}")]
public sealed class IRLeaInstruction : IRInstruction
{
    public IRExpression Target { get; init; }
    public IRExpression Address { get; init; }
    public override IRExpression? Result => Target;
    public override IReadOnlyList<IRExpression> Operands => [Target, Address];

    public IRLeaInstruction(IRExpression target, IRExpression address)
    {
        Target = target;
        Address = address;
    }

    public override void Accept(IIRNodeVisitor visitor) => visitor.VisitLea(this);

    public override T? Accept<T>(IIRNodeReturningVisitor<T> visitor) where T : default => visitor.VisitLea(this);

    internal override string DebugDisplay => $"IRLeaInstruction({Target.DebugDisplay} = &{Address.DebugDisplay})";
}
