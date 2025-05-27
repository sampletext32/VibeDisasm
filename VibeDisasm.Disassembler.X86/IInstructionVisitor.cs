namespace VibeDisasm.Disassembler.X86;

public interface IInstructionVisitor<out TResult>
{
    TResult Visit(AsmInstruction instruction);
}
