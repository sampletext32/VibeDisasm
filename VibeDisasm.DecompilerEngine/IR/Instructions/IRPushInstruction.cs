using VibeDisasm.DecompilerEngine.IR.Expressions;

namespace VibeDisasm.DecompilerEngine.IR.Instructions;

/// <summary>
/// Represents a stack PUSH instruction in IR.
/// Example: push eax -> IRPushInstruction(eax)
/// </summary>
public sealed class IRPushInstruction : IRInstruction
{
    public IRExpression Value { get; init; }
    public override IRExpression? Result => null;
    public override IReadOnlyList<IRExpression> Operands => [Value];
    
    // PUSH doesn't affect flags in x86
    public override IReadOnlyList<IRFlagEffect> SideEffects => [];
    
    public IRPushInstruction(IRExpression value)
    {
        Value = value;
    }

    public override string ToString() => $"push({Value})";
}
