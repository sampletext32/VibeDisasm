using VibeDisasm.DecompilerEngine.IR.Expressions;

namespace VibeDisasm.DecompilerEngine.IR.Instructions;

/// <summary>
/// Represents a LEA (load effective address) instruction in IR.
/// Example: lea eax, [ebx+4] -> IRLeaInstruction(eax, [ebx+4])
/// </summary>
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

    public override string ToString() => $"{Target} = &{Address}";
}
