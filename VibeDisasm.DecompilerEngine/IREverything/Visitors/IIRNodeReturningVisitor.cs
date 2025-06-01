using VibeDisasm.DecompilerEngine.IREverything.Expressions;
using VibeDisasm.DecompilerEngine.IREverything.Instructions;
using VibeDisasm.DecompilerEngine.IREverything.IRAnalyzers.IRLiftedInstructions;
using VibeDisasm.DecompilerEngine.IREverything.Model;
using VibeDisasm.DecompilerEngine.IREverything.Structuring;

namespace VibeDisasm.DecompilerEngine.IREverything.Visitors;

public interface IIRNodeReturningVisitor<out TReturn>
{
    TReturn? Visit(IRNode node) => node.Accept(this);
    TReturn? VisitAdd(IRAddExpr expr);
    TReturn? VisitCompare(IRCompareExpr expr);
    TReturn? VisitConstant(IRConstantExpr expr);
    TReturn? VisitDeref(IRDerefExpr expr);
    TReturn? VisitFlag(IRFlagExpr expr);
    TReturn? VisitLogical(IRLogicalExpr expr);
    TReturn? VisitMemory(IRMemoryExpr expr);
    TReturn? VisitMul(IRMulExpr expr);
    TReturn? VisitNot(IRNotExpr expr);
    TReturn? VisitRegister(IRRegisterExpr expr);
    TReturn? VisitSegment(IRSegmentExpr expr);
    TReturn? VisitSub(IRSubExpr expr);
    TReturn? VisitXor(IRXorExpr expr);
    TReturn? VisitAdc(IRAdcInstruction instr);
    TReturn? VisitAdd(IRAddInstruction instr);
    TReturn? VisitAnd(IRAndInstruction instr);
    TReturn? VisitCall(IRCallInstruction instr);
    TReturn? VisitCmp(IRCmpInstruction instr);
    TReturn? VisitDec(IRDecInstruction instr);
    TReturn? VisitDiv(IRDivInstruction instr);
    TReturn? VisitIDiv(IRIDivInstruction instr);
    TReturn? VisitInc(IRIncInstruction instr);
    TReturn? VisitJump(IRJumpInstruction instr);
    TReturn? VisitLea(IRLeaInstruction instr);
    TReturn? VisitMove(IRMoveInstruction instr);
    TReturn? VisitMul(IRMulInstruction instr);
    TReturn? VisitNeg(IRNegInstruction instr);
    TReturn? VisitNot(IRNotInstruction instr);
    TReturn? VisitOr(IROrInstruction instr);
    TReturn? VisitPop(IRPopInstruction instr);
    TReturn? VisitPush(IRPushInstruction instr);
    TReturn? VisitReturn(IRReturnInstruction instr);
    TReturn? VisitSbb(IRSbbInstruction instr);
    TReturn? VisitSub(IRSubInstruction instr);
    TReturn? VisitTest(IRTestInstruction instr);
    TReturn? VisitXor(IRXorInstruction instr);

    TReturn? VisitStub(StubIRInstruction instr);

    TReturn? VisitFlagEffect(IRFlagEffect effect);
    TReturn? VisitBlock(IRBlock block);
    TReturn? VisitFunction(IRFunction function);
    TReturn? VisitProgram(IRProgram program);
    TReturn? VisitType(IRType type);

    TReturn? VisitVariable(IRVariable variable);

    TReturn? VisitUnflaggedJump(IRUnflaggedJumpInstruction instr);
    TReturn? VisitWiredJump(IRWiredJumpInstruction instr);

    TReturn? VisitFld(IRFldInstruction instr);
    TReturn? VisitFstp(IRFstpInstruction instr);
    TReturn? VisitFadd(IRFaddInstruction instr);

    TReturn? VisitShl(IRShlInstruction instr);
    TReturn? VisitShr(IRShrInstruction instr);

    TReturn? VisitMovzx(IRMovzxInstruction instr);

    TReturn? VisitSequence(IRSequenceNode node);
    TReturn? VisitIfThen(IRIfThenNode node);
    TReturn? VisitIfElse(IRIfElseNode elseNode);

}
