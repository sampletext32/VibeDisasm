using VibeDisasm.DecompilerEngine.IR.Expressions;
using VibeDisasm.DecompilerEngine.IR.Instructions;
using VibeDisasm.DecompilerEngine.IR.Model;
using VibeDisasm.DecompilerEngine.IRAnalyzers.IRLiftedInstructions;

namespace VibeDisasm.DecompilerEngine.IR.Visitors;

public interface IIRNodeVisitor
{
    public void Visit(IRAddExpr expr);
    public void Visit(IRCompareExpr expr);
    public void Visit(IRConstantExpr expr);
    public void Visit(IRDerefExpr expr);
    public void Visit(IRFlagExpr expr);
    public void Visit(IRLogicalExpr expr);
    public void Visit(IRMemoryExpr expr);
    public void Visit(IRMulExpr expr);
    public void Visit(IRNotExpr expr);
    public void Visit(IRRegisterExpr expr);
    public void Visit(IRSegmentExpr expr);
    public void Visit(IRSubExpr expr);
    public void Visit(IRXorExpr expr);
    public void Visit(IRAdcInstruction instr);
    public void Visit(IRAddInstruction instr);
    public void Visit(IRAndInstruction instr);
    public void Visit(IRCallInstruction instr);
    public void Visit(IRCmpInstruction instr);
    public void Visit(IRDecInstruction instr);
    public void Visit(IRDivInstruction instr);
    public void Visit(IRIDivInstruction instr);
    public void Visit(IRIncInstruction instr);
    public void Visit(IRJumpInstruction instr);
    public void Visit(IRLeaInstruction instr);
    public void Visit(IRMoveInstruction instr);
    public void Visit(IRMulInstruction instr);
    public void Visit(IRNegInstruction instr);
    public void Visit(IRNotInstruction instr);
    public void Visit(IROrInstruction instr);
    public void Visit(IRPopInstruction instr);
    public void Visit(IRPushInstruction instr);
    public void Visit(IRReturnInstruction instr);
    public void Visit(IRSbbInstruction instr);
    public void Visit(IRSubInstruction instr);
    public void Visit(IRTestInstruction instr);
    public void Visit(IRXorInstruction instr);
    public void Visit(StubIRInstruction instr);
    
    public void Visit(IRFlagEffect effect);
    public void Visit(IRBlock block);
    public void Visit(IRFunction function);
    public void Visit(IRProgram program);
    public void Visit(IRType type);
    public void Visit(IRVariable variable);

    public void Visit(IRUnflaggedJumpInstruction instr);
    public void Visit(IRWiredJumpInstruction instr);
}