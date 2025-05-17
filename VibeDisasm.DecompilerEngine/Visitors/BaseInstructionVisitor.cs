using VibeDisasm.Disassembler.X86;

namespace VibeDisasm.DecompilerEngine.Visitors;

/// <summary>
/// Base implementation of the instruction visitor interface
/// </summary>
/// <typeparam name="TResult">The type of result produced by the visitor</typeparam>
public abstract class BaseInstructionVisitor<TResult> : IInstructionVisitor
{
    protected TResult? Result { get; set; }
    
    public virtual TResult? GetResult()
    {
        return Result;
    }
    
    public virtual void Visit(Instruction instruction)
    {
        switch (instruction.Type)
        {
            // Data movement
            case InstructionType.Mov:
                VisitMov(instruction);
                break;
            case InstructionType.Push:
                VisitPush(instruction);
                break;
            case InstructionType.Pop:
                VisitPop(instruction);
                break;
                
            // Arithmetic
            case InstructionType.Add:
                VisitAdd(instruction);
                break;
            case InstructionType.Sub:
                VisitSub(instruction);
                break;
            case InstructionType.Mul:
            case InstructionType.IMul:
                VisitMul(instruction);
                break;
            case InstructionType.Div:
            case InstructionType.IDiv:
                VisitDiv(instruction);
                break;
            case InstructionType.Inc:
                VisitInc(instruction);
                break;
            case InstructionType.Dec:
                VisitDec(instruction);
                break;
            case InstructionType.Neg:
                VisitNeg(instruction);
                break;
                
            // Logical
            case InstructionType.And:
                VisitAnd(instruction);
                break;
            case InstructionType.Or:
                VisitOr(instruction);
                break;
            case InstructionType.Xor:
                VisitXor(instruction);
                break;
            case InstructionType.Not:
                VisitNot(instruction);
                break;
            case InstructionType.Test:
                VisitTest(instruction);
                break;
            case InstructionType.Cmp:
                VisitCmp(instruction);
                break;
            case InstructionType.Shl:
                VisitShl(instruction);
                break;
            case InstructionType.Shr:
                VisitShr(instruction);
                break;
                
            // Control flow
            case InstructionType.Jmp:
                VisitJmp(instruction);
                break;
            case InstructionType.Call:
                VisitCall(instruction);
                break;
            case InstructionType.Ret:
                VisitRet(instruction);
                break;
                
            // Conditional jumps
            case InstructionType.Jz:
            case InstructionType.Jnz:
            case InstructionType.Jg:
            case InstructionType.Jge:
            case InstructionType.Jl:
            case InstructionType.Jle:
            case InstructionType.Ja:
            case InstructionType.Jae:
            case InstructionType.Jb:
            case InstructionType.Jbe:
            case InstructionType.Jo:
            case InstructionType.Jno:
            case InstructionType.Js:
            case InstructionType.Jns:
            case InstructionType.Jp:
            case InstructionType.Jnp:
                VisitJcc(instruction);
                break;
                
            default:
                VisitUnknown(instruction);
                break;
        }
    }
    
    // Default implementations that can be overridden by derived classes
    public virtual void VisitMov(Instruction instruction) => VisitUnknown(instruction);
    public virtual void VisitPush(Instruction instruction) => VisitUnknown(instruction);
    public virtual void VisitPop(Instruction instruction) => VisitUnknown(instruction);
    public virtual void VisitAdd(Instruction instruction) => VisitUnknown(instruction);
    public virtual void VisitSub(Instruction instruction) => VisitUnknown(instruction);
    public virtual void VisitMul(Instruction instruction) => VisitUnknown(instruction);
    public virtual void VisitDiv(Instruction instruction) => VisitUnknown(instruction);
    public virtual void VisitInc(Instruction instruction) => VisitUnknown(instruction);
    public virtual void VisitDec(Instruction instruction) => VisitUnknown(instruction);
    public virtual void VisitNeg(Instruction instruction) => VisitUnknown(instruction);
    public virtual void VisitAnd(Instruction instruction) => VisitUnknown(instruction);
    public virtual void VisitOr(Instruction instruction) => VisitUnknown(instruction);
    public virtual void VisitXor(Instruction instruction) => VisitUnknown(instruction);
    public virtual void VisitNot(Instruction instruction) => VisitUnknown(instruction);
    public virtual void VisitTest(Instruction instruction) => VisitUnknown(instruction);
    public virtual void VisitCmp(Instruction instruction) => VisitUnknown(instruction);
    public virtual void VisitShl(Instruction instruction) => VisitUnknown(instruction);
    public virtual void VisitShr(Instruction instruction) => VisitUnknown(instruction);
    public virtual void VisitJmp(Instruction instruction) => VisitUnknown(instruction);
    public virtual void VisitJcc(Instruction instruction) => VisitUnknown(instruction);
    public virtual void VisitCall(Instruction instruction) => VisitUnknown(instruction);
    public virtual void VisitRet(Instruction instruction) => VisitUnknown(instruction);
    
    public abstract void VisitUnknown(Instruction instruction);
}
