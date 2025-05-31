using VibeDisasm.DecompilerEngine.IR.Expressions;
using VibeDisasm.DecompilerEngine.IR.Instructions;
using VibeDisasm.DecompilerEngine.IR.Model;
using VibeDisasm.DecompilerEngine.IRAnalyzers.IRLiftedInstructions;

namespace VibeDisasm.DecompilerEngine.IR.Visitors;

public abstract class BaseIRNodeReturningVisitor<TReturn> : IIRNodeReturningVisitor<TReturn>
{
    private readonly TReturn? _defaultReturn;

    protected BaseIRNodeReturningVisitor(TReturn? defaultReturn)
    {
        _defaultReturn = defaultReturn;
    }

    public TReturn? Visit(IRNode node) => node.Accept(this);

    public virtual TReturn? VisitAdd(IRAddExpr expr) => _defaultReturn;

    public virtual TReturn? VisitCompare(IRCompareExpr expr) => _defaultReturn;

    public virtual TReturn? VisitConstant(IRConstantExpr expr) => _defaultReturn;

    public virtual TReturn? VisitDeref(IRDerefExpr expr) => _defaultReturn;

    public virtual TReturn? VisitFlag(IRFlagExpr expr) => _defaultReturn;

    public virtual TReturn? VisitLogical(IRLogicalExpr expr) => _defaultReturn;

    public virtual TReturn? VisitMemory(IRMemoryExpr expr) => _defaultReturn;

    public virtual TReturn? VisitMul(IRMulExpr expr) => _defaultReturn;

    public virtual TReturn? VisitNot(IRNotExpr expr) => _defaultReturn;

    public virtual TReturn? VisitRegister(IRRegisterExpr expr) => _defaultReturn;

    public virtual TReturn? VisitSegment(IRSegmentExpr expr) => _defaultReturn;

    public virtual TReturn? VisitSub(IRSubExpr expr) => _defaultReturn;

    public virtual TReturn? VisitXor(IRXorExpr expr) => _defaultReturn;

    public virtual TReturn? VisitAdc(IRAdcInstruction instr) => _defaultReturn;

    public virtual TReturn? VisitAdd(IRAddInstruction instr) => _defaultReturn;

    public virtual TReturn? VisitAnd(IRAndInstruction instr) => _defaultReturn;

    public virtual TReturn? VisitCall(IRCallInstruction instr) => _defaultReturn;

    public virtual TReturn? VisitCmp(IRCmpInstruction instr) => _defaultReturn;

    public virtual TReturn? VisitDec(IRDecInstruction instr) => _defaultReturn;

    public virtual TReturn? VisitDiv(IRDivInstruction instr) => _defaultReturn;

    public virtual TReturn? VisitIDiv(IRIDivInstruction instr) => _defaultReturn;

    public virtual TReturn? VisitInc(IRIncInstruction instr) => _defaultReturn;

    public virtual TReturn? VisitJump(IRJumpInstruction instr) => _defaultReturn;

    public virtual TReturn? VisitLea(IRLeaInstruction instr) => _defaultReturn;

    public virtual TReturn? VisitMove(IRMoveInstruction instr) => _defaultReturn;

    public virtual TReturn? VisitMul(IRMulInstruction instr) => _defaultReturn;

    public virtual TReturn? VisitNeg(IRNegInstruction instr) => _defaultReturn;

    public virtual TReturn? VisitNot(IRNotInstruction instr) => _defaultReturn;

    public virtual TReturn? VisitOr(IROrInstruction instr) => _defaultReturn;

    public virtual TReturn? VisitPop(IRPopInstruction instr) => _defaultReturn;

    public virtual TReturn? VisitPush(IRPushInstruction instr) => _defaultReturn;

    public virtual TReturn? VisitReturn(IRReturnInstruction instr) => _defaultReturn;

    public virtual TReturn? VisitSbb(IRSbbInstruction instr) => _defaultReturn;

    public virtual TReturn? VisitSub(IRSubInstruction instr) => _defaultReturn;

    public virtual TReturn? VisitTest(IRTestInstruction instr) => _defaultReturn;

    public virtual TReturn? VisitXor(IRXorInstruction instr) => _defaultReturn;

    public virtual TReturn? VisitStub(StubIRInstruction instr) => _defaultReturn;

    public virtual TReturn? VisitFlagEffect(IRFlagEffect effect) => _defaultReturn;

    public virtual TReturn? VisitBlock(IRBlock block) => _defaultReturn;

    public virtual TReturn? VisitFunction(IRFunction function) => _defaultReturn;

    public virtual TReturn? VisitProgram(IRProgram program) => _defaultReturn;

    public virtual TReturn? VisitType(IRType type) => _defaultReturn;

    public virtual TReturn? VisitVariable(IRVariable variable) => _defaultReturn;

    public virtual TReturn? VisitUnflaggedJump(IRUnflaggedJumpInstruction instr) => _defaultReturn;

    public virtual TReturn? VisitWiredJump(IRWiredJumpInstruction instr) => _defaultReturn;
}
