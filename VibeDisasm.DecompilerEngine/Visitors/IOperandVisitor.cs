using VibeDisasm.Disassembler.X86.Operands;
using VibeDisasm.Disassembler.X86;

namespace VibeDisasm.DecompilerEngine.Visitors;

/// <summary>
/// Interface for visitors that process x86 instruction operands
/// </summary>
public interface IOperandVisitor<T>
{
    T Visit(Operand operand);
    
    // Operand type visitors
    T VisitRegister(RegisterOperand operand);
    T VisitImmediate(ImmediateOperand operand);
    T VisitDirectMemory(DirectMemoryOperand operand);
    T VisitBaseRegisterMemory(BaseRegisterMemoryOperand operand);
    T VisitScaledIndexMemory(ScaledIndexMemoryOperand operand);
    T VisitRelativeOffset(RelativeOffsetOperand operand);
    
    // Default handler for unimplemented operand types
    T VisitUnknown(Operand operand);
}
