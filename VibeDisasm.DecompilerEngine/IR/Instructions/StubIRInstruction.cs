using VibeDisasm.DecompilerEngine.IR.Expressions;
using VibeDisasm.DecompilerEngine.IR.Instructions;
using VibeDisasm.Disassembler.X86;

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
}