using VibeDisasm.DecompilerEngine.IREverything.Expressions;
using VibeDisasm.DecompilerEngine.IREverything.Instructions;
using VibeDisasm.DecompilerEngine.IREverything.IRAnalyzers.IRLiftedInstructions;
using VibeDisasm.DecompilerEngine.IREverything.Model;
using VibeDisasm.DecompilerEngine.IREverything.Structuring;

namespace VibeDisasm.DecompilerEngine.IREverything.Visitors;

public abstract class BaseIRNodeVisitor : IIRNodeVisitor
{
    public virtual void VisitAdd(IRAddExpr expr) { }

    public virtual void VisitCompare(IRCompareExpr expr) { }

    public virtual void VisitConstant(IRConstantExpr expr) { }

    public virtual void VisitDeref(IRDerefExpr expr) { }

    public virtual void VisitFlag(IRFlagExpr expr) { }

    public virtual void VisitLogical(IRLogicalExpr expr) { }

    public virtual void VisitMemory(IRMemoryExpr expr) { }

    public virtual void VisitMul(IRMulExpr expr) { }

    public virtual void VisitNot(IRNotExpr expr) { }

    public virtual void VisitRegister(IRRegisterExpr expr) { }

    public virtual void VisitSegment(IRSegmentExpr expr) { }

    public virtual void VisitSub(IRSubExpr expr) { }

    public virtual void VisitXor(IRXorExpr expr) { }

    public virtual void VisitAdc(IRAdcInstruction instr) { }

    public virtual void VisitAdd(IRAddInstruction instr) { }

    public virtual void VisitAnd(IRAndInstruction instr) { }

    public virtual void VisitCall(IRCallInstruction instr) { }

    public virtual void VisitCmp(IRCmpInstruction instr) { }

    public virtual void VisitDec(IRDecInstruction instr) { }

    public virtual void VisitDiv(IRDivInstruction instr) { }

    public virtual void VisitIDiv(IRIDivInstruction instr) { }

    public virtual void VisitInc(IRIncInstruction instr) { }

    public virtual void VisitJump(IRJumpInstruction instr) { }

    public virtual void VisitLea(IRLeaInstruction instr) { }

    public virtual void VisitMove(IRMoveInstruction instr) { }

    public virtual void VisitMul(IRMulInstruction instr) { }

    public virtual void VisitNeg(IRNegInstruction instr) { }

    public virtual void VisitNot(IRNotInstruction instr) { }

    public virtual void VisitOr(IROrInstruction instr) { }

    public virtual void VisitPop(IRPopInstruction instr) { }

    public virtual void VisitPush(IRPushInstruction instr) { }

    public virtual void VisitReturn(IRReturnInstruction instr) { }

    public virtual void VisitSbb(IRSbbInstruction instr) { }

    public virtual void VisitSub(IRSubInstruction instr) { }

    public virtual void VisitTest(IRTestInstruction instr) { }

    public virtual void VisitXor(IRXorInstruction instr) { }

    public virtual void VisitStub(StubIRInstruction instr) { }

    public virtual void VisitFlagEffect(IRFlagEffect effect) { }

    public virtual void VisitBlock(IRBlock block) { }

    public virtual void VisitFunction(IRFunction function) { }

    public virtual void VisitProgram(IRProgram program) { }

    public virtual void VisitType(IRType type) { }

    public virtual void VisitVariable(IRVariable variable) { }

    public virtual void VisitUnflaggedJump(IRUnflaggedJumpInstruction instr) { }

    public virtual void VisitWiredJump(IRWiredJumpInstruction instr) { }
    public virtual void VisitFld(IRFldInstruction instr) { }

    public virtual void VisitFstp(IRFstpInstruction instr) { }
    public virtual void VisitFadd(IRFaddInstruction instr) { }

    public virtual void VisitShl(IRShlInstruction instr) { }

    public virtual void VisitShr(IRShrInstruction instr) { }
    public virtual void VisitMovzx(IRMovzxInstruction instr) { }

    public virtual void VisitSequence(IRSequenceNode node) { }
    public virtual void VisitIfElse(IRIfElseNode elseNode) { }
    public virtual void VisitIfThen(IRIfThenNode node) { }
}
