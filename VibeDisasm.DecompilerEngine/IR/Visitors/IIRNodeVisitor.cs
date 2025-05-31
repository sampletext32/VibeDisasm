using VibeDisasm.DecompilerEngine.IR.Expressions;
using VibeDisasm.DecompilerEngine.IR.Instructions;
using VibeDisasm.DecompilerEngine.IR.Model;
using VibeDisasm.DecompilerEngine.IRAnalyzers.IRLiftedInstructions;

namespace VibeDisasm.DecompilerEngine.IR.Visitors;

public interface IIRNodeVisitor
{
    public void Visit(IRNode node) => node.Accept(this);
    void VisitAdd(IRAddExpr expr);
    void VisitCompare(IRCompareExpr expr);
    void VisitConstant(IRConstantExpr expr);
    void VisitDeref(IRDerefExpr expr);
    void VisitFlag(IRFlagExpr expr);
    void VisitLogical(IRLogicalExpr expr);
    void VisitMemory(IRMemoryExpr expr);
    void VisitMul(IRMulExpr expr);
    void VisitNot(IRNotExpr expr);
    void VisitRegister(IRRegisterExpr expr);
    void VisitSegment(IRSegmentExpr expr);
    void VisitSub(IRSubExpr expr);
    void VisitXor(IRXorExpr expr);
    void VisitAdc(IRAdcInstruction instr);
    void VisitAdd(IRAddInstruction instr);
    void VisitAnd(IRAndInstruction instr);
    void VisitCall(IRCallInstruction instr);
    void VisitCmp(IRCmpInstruction instr);
    void VisitDec(IRDecInstruction instr);
    void VisitDiv(IRDivInstruction instr);
    void VisitIDiv(IRIDivInstruction instr);
    void VisitInc(IRIncInstruction instr);
    void VisitJump(IRJumpInstruction instr);
    void VisitLea(IRLeaInstruction instr);
    void VisitMove(IRMoveInstruction instr);
    void VisitMul(IRMulInstruction instr);
    void VisitNeg(IRNegInstruction instr);
    void VisitNot(IRNotInstruction instr);
    void VisitOr(IROrInstruction instr);
    void VisitPop(IRPopInstruction instr);
    void VisitPush(IRPushInstruction instr);
    void VisitReturn(IRReturnInstruction instr);
    void VisitSbb(IRSbbInstruction instr);
    void VisitSub(IRSubInstruction instr);
    void VisitTest(IRTestInstruction instr);
    void VisitXor(IRXorInstruction instr);

    void VisitStub(StubIRInstruction instr);

    void VisitFlagEffect(IRFlagEffect effect);
    void VisitBlock(IRBlock block);
    void VisitFunction(IRFunction function);
    void VisitProgram(IRProgram program);
    void VisitType(IRType type);

    void VisitVariable(IRVariable variable);

    void VisitUnflaggedJump(IRUnflaggedJumpInstruction instr);
    void VisitWiredJump(IRWiredJumpInstruction instr);
    
    void VisitFld(IRFldInstruction instr);
    void VisitFstp(IRFstpInstruction instr);
    void VisitFadd(IRFaddInstruction instr);
    
    void VisitShl(IRShlInstruction instr);
    void VisitShr(IRShrInstruction instr);
    
    void VisitMovzx(IRMovzxInstruction instr);
}
