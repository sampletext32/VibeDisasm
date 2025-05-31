using System.Diagnostics;
using VibeDisasm.DecompilerEngine.IR.Expressions;
using VibeDisasm.DecompilerEngine.IR.Visitors;

namespace VibeDisasm.DecompilerEngine.IR.Instructions;

/// <summary>
/// Represents a floating-point store and pop instruction in IR.
/// Example: fstp dword ptr [ebp-4] -> IRFstpInstruction([ebp-4])
/// </summary>
[DebuggerDisplay("{DebugDisplay}")]
public sealed class IRFstpInstruction : IRInstruction
{
    public IRExpression Destination { get; init; }
    // FSTP stores ST(0) to destination and pops the FPU stack, so there's no direct result
    // The destination is modified, but the instruction itself doesn't produce a result that can be used by other instructions
    public override IRExpression? Result => Destination;
    public override IReadOnlyList<IRExpression> Operands => [Destination];

    public IRFstpInstruction(IRExpression destination)
    {
        Destination = destination;
    }

    public override void Accept(IIRNodeVisitor visitor) => visitor.VisitFstp(this);

    public override T? Accept<T>(IIRNodeReturningVisitor<T> visitor) where T : default => visitor.VisitFstp(this);

    internal override string DebugDisplay => $"IRFstpInstruction({Destination.DebugDisplay} <- FPU_STACK, pop)";
}
