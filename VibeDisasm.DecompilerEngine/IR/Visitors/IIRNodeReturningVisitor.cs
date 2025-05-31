using VibeDisasm.DecompilerEngine.IR.Expressions;
using VibeDisasm.DecompilerEngine.IR.Instructions;
using VibeDisasm.DecompilerEngine.IR.Model;
using VibeDisasm.DecompilerEngine.IRAnalyzers.IRLiftedInstructions;

namespace VibeDisasm.DecompilerEngine.IR.Visitors;

public interface IIRNodeReturningVisitor<out TReturn>
{
    public TReturn Visit(IRAddExpr expr);
    public TReturn Visit(IRCompareExpr expr);
    public TReturn Visit(IRConstantExpr expr);
    public TReturn Visit(IRDerefExpr expr);
    public TReturn Visit(IRFlagExpr expr);
    public TReturn Visit(IRLogicalExpr expr);
    public TReturn Visit(IRMemoryExpr expr);
    public TReturn Visit(IRMulExpr expr);
    public TReturn Visit(IRNotExpr expr);
    public TReturn Visit(IRRegisterExpr expr);
    public TReturn Visit(IRSegmentExpr expr);
    public TReturn Visit(IRSubExpr expr);
    public TReturn Visit(IRXorExpr expr);
    public TReturn Visit(IRAdcInstruction instr);
    public TReturn Visit(IRAddInstruction instr);
    public TReturn Visit(IRAndInstruction instr);
    public TReturn Visit(IRCallInstruction instr);
    public TReturn Visit(IRCmpInstruction instr);
    public TReturn Visit(IRDecInstruction instr);
    public TReturn Visit(IRDivInstruction instr);
    public TReturn Visit(IRIDivInstruction instr);
    public TReturn Visit(IRIncInstruction instr);
    public TReturn Visit(IRJumpInstruction instr);
    public TReturn Visit(IRLeaInstruction instr);
    public TReturn Visit(IRMoveInstruction instr);
    public TReturn Visit(IRMulInstruction instr);
    public TReturn Visit(IRNegInstruction instr);
    public TReturn Visit(IRNotInstruction instr);
    public TReturn Visit(IROrInstruction instr);
    public TReturn Visit(IRPopInstruction instr);
    public TReturn Visit(IRPushInstruction instr);
    public TReturn Visit(IRReturnInstruction instr);
    public TReturn Visit(IRSbbInstruction instr);
    public TReturn Visit(IRSubInstruction instr);
    public TReturn Visit(IRTestInstruction instr);
    public TReturn Visit(IRXorInstruction instr);
    public TReturn Visit(StubIRInstruction instr);
    
    public TReturn Visit(IRFlagEffect effect);
    public TReturn Visit(IRBlock block);
    public TReturn Visit(IRFunction function);
    public TReturn Visit(IRProgram program);
    public TReturn Visit(IRType type);
    public TReturn Visit(IRVariable variable);

    public TReturn Visit(IRUnflaggedJumpInstruction instr);
    public TReturn Visit(IRWiredJumpInstruction instr);
}