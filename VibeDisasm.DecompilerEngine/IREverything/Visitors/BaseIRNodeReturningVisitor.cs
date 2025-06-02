using VibeDisasm.DecompilerEngine.IREverything.Expressions;
using VibeDisasm.DecompilerEngine.IREverything.Instructions;
using VibeDisasm.DecompilerEngine.IREverything.IRAnalyzers.IRLiftedInstructions;
using VibeDisasm.DecompilerEngine.IREverything.Model;
using VibeDisasm.DecompilerEngine.IREverything.Structuring;

namespace VibeDisasm.DecompilerEngine.IREverything.Visitors;

public abstract class BaseIRNodeReturningVisitor<TReturn> : IIRNodeReturningVisitor<TReturn>
{
    private readonly Func<IRNode, TReturn?> _defaultReturn;

    protected BaseIRNodeReturningVisitor(TReturn? defaultReturn) => _defaultReturn = _ => defaultReturn;

    protected BaseIRNodeReturningVisitor(Func<IRNode, TReturn?> defaultReturn) => _defaultReturn = defaultReturn;

    public TReturn? Visit(IRNode node) => node.Accept(this);

    public virtual TReturn? VisitAdd(IRAddExpr expr) => _defaultReturn(expr);

    public virtual TReturn? VisitCompare(IRCompareExpr expr) => _defaultReturn(expr);

    public virtual TReturn? VisitConstant(IRConstantExpr expr) => _defaultReturn(expr);

    public virtual TReturn? VisitDeref(IRDerefExpr expr) => _defaultReturn(expr);

    public virtual TReturn? VisitFlag(IRFlagExpr expr) => _defaultReturn(expr);

    public virtual TReturn? VisitLogical(IRLogicalExpr expr) => _defaultReturn(expr);

    public virtual TReturn? VisitMemory(IRMemoryExpr expr) => _defaultReturn(expr);

    public virtual TReturn? VisitMul(IRMulExpr expr) => _defaultReturn(expr);

    public virtual TReturn? VisitNot(IRNotExpr expr) => _defaultReturn(expr);

    public virtual TReturn? VisitRegister(IRRegisterExpr expr) => _defaultReturn(expr);

    public virtual TReturn? VisitSegment(IRSegmentExpr expr) => _defaultReturn(expr);

    public virtual TReturn? VisitSub(IRSubExpr expr) => _defaultReturn(expr);

    public virtual TReturn? VisitXor(IRXorExpr expr) => _defaultReturn(expr);

    public virtual TReturn? VisitAdc(IRAdcInstruction instr) => _defaultReturn(instr);

    public virtual TReturn? VisitAdd(IRAddInstruction instr) => _defaultReturn(instr);

    public virtual TReturn? VisitAnd(IRAndInstruction instr) => _defaultReturn(instr);

    public virtual TReturn? VisitCall(IRCallInstruction instr) => _defaultReturn(instr);

    public virtual TReturn? VisitCmp(IRCmpInstruction instr) => _defaultReturn(instr);

    public virtual TReturn? VisitDec(IRDecInstruction instr) => _defaultReturn(instr);

    public virtual TReturn? VisitDiv(IRDivInstruction instr) => _defaultReturn(instr);

    public virtual TReturn? VisitIDiv(IRIDivInstruction instr) => _defaultReturn(instr);

    public virtual TReturn? VisitInc(IRIncInstruction instr) => _defaultReturn(instr);

    public virtual TReturn? VisitJump(IRJumpInstruction instr) => _defaultReturn(instr);
    public virtual TReturn? VisitConditionalJump(IRConditionalJumpInstruction instr) => _defaultReturn(instr);

    public virtual TReturn? VisitLea(IRLeaInstruction instr) => _defaultReturn(instr);

    public virtual TReturn? VisitMove(IRMoveInstruction instr) => _defaultReturn(instr);

    public virtual TReturn? VisitMul(IRMulInstruction instr) => _defaultReturn(instr);

    public virtual TReturn? VisitNeg(IRNegInstruction instr) => _defaultReturn(instr);

    public virtual TReturn? VisitNot(IRNotInstruction instr) => _defaultReturn(instr);

    public virtual TReturn? VisitOr(IROrInstruction instr) => _defaultReturn(instr);

    public virtual TReturn? VisitPop(IRPopInstruction instr) => _defaultReturn(instr);

    public virtual TReturn? VisitPush(IRPushInstruction instr) => _defaultReturn(instr);

    public virtual TReturn? VisitReturn(IRReturnInstruction instr) => _defaultReturn(instr);

    public virtual TReturn? VisitSbb(IRSbbInstruction instr) => _defaultReturn(instr);

    public virtual TReturn? VisitSub(IRSubInstruction instr) => _defaultReturn(instr);

    public virtual TReturn? VisitTest(IRTestInstruction instr) => _defaultReturn(instr);

    public virtual TReturn? VisitXor(IRXorInstruction instr) => _defaultReturn(instr);

    public virtual TReturn? VisitStub(StubIRInstruction instr) => _defaultReturn(instr);

    public virtual TReturn? VisitFlagEffect(IRFlagEffect effect) => _defaultReturn(effect);

    public virtual TReturn? VisitBlock(IRBlock block) => _defaultReturn(block);

    public virtual TReturn? VisitFunction(IRFunction function) => _defaultReturn(function);

    public virtual TReturn? VisitProgram(IRProgram program) => _defaultReturn(program);

    public virtual TReturn? VisitType(IRType type) => _defaultReturn(type);

    public virtual TReturn? VisitVariable(IRVariable variable) => _defaultReturn(variable);

    public virtual TReturn? VisitSemanticIfJump(IRSemanticIfJumpInstruction instr) => _defaultReturn(instr);

    public virtual TReturn? VisitWiredJump(IRWiredJumpInstruction instr) => _defaultReturn(instr);
    public virtual TReturn? VisitFld(IRFldInstruction instr) => _defaultReturn(instr);

    public virtual TReturn? VisitFstp(IRFstpInstruction instr) => _defaultReturn(instr);
    public virtual TReturn? VisitFadd(IRFaddInstruction instr) => _defaultReturn(instr);

    public virtual TReturn? VisitShl(IRShlInstruction instr) => _defaultReturn(instr);
    public virtual TReturn? VisitShr(IRShrInstruction instr) => _defaultReturn(instr);
    public virtual TReturn? VisitMovzx(IRMovzxInstruction instr) => _defaultReturn(instr);

    public virtual TReturn? VisitSequence(IRSequenceNode node) => _defaultReturn(node);
    public virtual TReturn? VisitIfThen(IRIfThenNode node) => _defaultReturn(node);

    public virtual TReturn? VisitIfThenElse(IRIfThenElseNode thenElseNode) => _defaultReturn(thenElseNode);
}
