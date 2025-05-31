using VibeDisasm.DecompilerEngine.IR.Expressions;
using VibeDisasm.DecompilerEngine.IR.Model;
using VibeDisasm.DecompilerEngine.IR.Visitors;

namespace VibeDisasm.DecompilerEngine.IR.Instructions;

/// <summary>
/// Represents a call instruction in IR.
/// Example: call 0x401000 -> IRCallInstruction(0x401000)
/// </summary>
public sealed class IRCallInstruction : IRInstruction
{
    public IRExpression Target { get; init; }
    public override IRExpression? Result => null;
    public override IReadOnlyList<IRExpression> Operands => [Target];
    
    // Call instructions don't modify flags in x86
    public override IReadOnlyList<IRFlagEffect> SideEffects => [];
    
    public IRCallInstruction(IRExpression target)
    {
        Target = target;
    }

    public override string ToString() => $"call {Target}";


    public override void Accept(IIRNodeVisitor visitor) => visitor.Visit(this);

    public override T Accept<T>(IIRNodeReturningVisitor<T> visitor) => visitor.Visit(this);
}
