using System.Diagnostics;
using VibeDisasm.DecompilerEngine.IR.Expressions;
using VibeDisasm.DecompilerEngine.IR.Visitors;
using VibeDisasm.Disassembler.X86;

namespace VibeDisasm.DecompilerEngine.IR.Instructions;

[DebuggerDisplay("{DebugDisplay}")]
public sealed class StubIRInstruction : IRInstruction
{
    public InstructionType InstructionType { get; init; }
    public override IRExpression? Result => null;
    public override IReadOnlyList<IRExpression> Operands => [];

    public StubIRInstruction(InstructionType instructionType) => InstructionType = instructionType;

    public override string ToString() => $"StubIRInstruction - {InstructionType:G} Not implemented";

    public override void Accept(IIRNodeVisitor visitor) => visitor.VisitStub(this);

    public override T? Accept<T>(IIRNodeReturningVisitor<T> visitor) where T : default => visitor.VisitStub(this);

    internal override string DebugDisplay => $"StubIRInstruction({InstructionType:G} Not implemented)";
}
