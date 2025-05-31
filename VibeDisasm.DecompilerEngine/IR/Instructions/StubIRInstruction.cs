using VibeDisasm.DecompilerEngine.IR.Expressions;
using VibeDisasm.DecompilerEngine.IR.Visitors;
using VibeDisasm.Disassembler.X86;

namespace VibeDisasm.DecompilerEngine.IR.Instructions;

public sealed class StubIRInstruction : IRInstruction
{
    private readonly InstructionType _instructionType;
    public override IRExpression? Result => null;
    public override IReadOnlyList<IRExpression> Operands => [];

    public StubIRInstruction(InstructionType instructionType)
    {
        _instructionType = instructionType;
    }

    public override string ToString() => $"StubIRInstruction - {_instructionType:G} Not implemented";


    public override void Accept(IIRNodeVisitor visitor) => visitor.Visit(this);

    public override T Accept<T>(IIRNodeReturningVisitor<T> visitor) => visitor.Visit(this);
}