using VibeDisasm.Disassembler.X86.Operands;

namespace VibeDisasm.Disassembler.X86;

/// <summary>
/// Base class for all x86 instruction operands
/// </summary>
public interface IOperandVisitor<out TResult>
{
    TResult VisitRegister(RegisterOperand operand);
    TResult VisitImmediate(ImmediateOperand operand);
    TResult VisitRelativeOffset(RelativeOffsetOperand operand);
    TResult VisitDisplacementMemory(DisplacementMemoryOperand operand);
    TResult VisitBaseRegisterMemory(BaseRegisterMemoryOperand operand);
    TResult VisitScaledIndexMemory(ScaledIndexMemoryOperand operand);
    TResult VisitFarPointer(FarPointerOperand operand);
    TResult VisitDirectMemory(DirectMemoryOperand operand);
    TResult VisitFPURegister(FPURegisterOperand operand);
    TResult VisitRegister8(Register8Operand operand);
    TResult VisitOperand(Operand operand); // fallback
}
