using VibeDisasm.Disassembler.X86;
using VibeDisasm.Disassembler.X86.Operands;

namespace VibeDisasm.DecompilerEngine.Visitors;

/// <summary>
/// Interface for visitors that process x86 instructions
/// </summary>
public interface IInstructionVisitor
{
    void Visit(Instruction instruction);
    
    // Instruction type visitors
    void VisitMov(Instruction instruction);
    void VisitPush(Instruction instruction);
    void VisitPop(Instruction instruction);
    void VisitAdd(Instruction instruction);
    void VisitSub(Instruction instruction);
    void VisitMul(Instruction instruction);
    void VisitDiv(Instruction instruction);
    void VisitInc(Instruction instruction);
    void VisitDec(Instruction instruction);
    void VisitNeg(Instruction instruction);
    void VisitAnd(Instruction instruction);
    void VisitOr(Instruction instruction);
    void VisitXor(Instruction instruction);
    void VisitNot(Instruction instruction);
    void VisitTest(Instruction instruction);
    void VisitCmp(Instruction instruction);
    void VisitShl(Instruction instruction);
    void VisitShr(Instruction instruction);
    void VisitJmp(Instruction instruction);
    void VisitJcc(Instruction instruction); // For conditional jumps
    void VisitCall(Instruction instruction);
    void VisitRet(Instruction instruction);
    
    // Default handler for unimplemented instructions
    void VisitUnknown(Instruction instruction);
}
