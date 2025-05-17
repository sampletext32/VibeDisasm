using VibeDisasm.Disassembler.X86;
using VibeDisasm.Disassembler.X86.Operands;

namespace VibeDisasm.DecompilerEngine.Visitors;

/// <summary>
/// Base implementation of the operand visitor interface
/// </summary>
public abstract class BaseOperandVisitor<T> : IOperandVisitor<T>
{
    public virtual T Visit(Operand operand)
    {
        return operand.Type switch
        {
            OperandType.Register => operand is RegisterOperand regOp ? VisitRegister(regOp) : VisitUnknown(operand),
            OperandType.ImmediateValue => operand is ImmediateOperand immOp ? VisitImmediate(immOp) : VisitUnknown(operand),
            OperandType.MemoryDirect => operand is DirectMemoryOperand dirMemOp ? VisitDirectMemory(dirMemOp) : VisitUnknown(operand),
            OperandType.MemoryBaseReg => operand is BaseRegisterMemoryOperand baseRegOp ? VisitBaseRegisterMemory(baseRegOp) : VisitUnknown(operand),
            OperandType.MemoryIndexed => operand is ScaledIndexMemoryOperand scaledIdxOp ? VisitScaledIndexMemory(scaledIdxOp) : VisitUnknown(operand),
            OperandType.RelativeOffset => operand is RelativeOffsetOperand relOffOp ? VisitRelativeOffset(relOffOp) : VisitUnknown(operand),
            _ => VisitUnknown(operand)
        };
    }
    
    // Default implementations that can be overridden by derived classes
    public virtual T VisitRegister(RegisterOperand operand) => VisitUnknown(operand);
    public virtual T VisitImmediate(ImmediateOperand operand) => VisitUnknown(operand);
    public virtual T VisitDirectMemory(DirectMemoryOperand operand) => VisitUnknown(operand);
    public virtual T VisitBaseRegisterMemory(BaseRegisterMemoryOperand operand) => VisitUnknown(operand);
    public virtual T VisitScaledIndexMemory(ScaledIndexMemoryOperand operand) => VisitUnknown(operand);
    public virtual T VisitRelativeOffset(RelativeOffsetOperand operand) => VisitUnknown(operand);
    
    public abstract T VisitUnknown(Operand operand);
}
