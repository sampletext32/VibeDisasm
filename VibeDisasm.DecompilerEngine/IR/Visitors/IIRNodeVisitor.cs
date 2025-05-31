using VibeDisasm.DecompilerEngine.IR.Expressions;
using VibeDisasm.DecompilerEngine.IR.Instructions;
using VibeDisasm.DecompilerEngine.IR.Model;
using VibeDisasm.DecompilerEngine.IRAnalyzers.IRLiftedInstructions;

namespace VibeDisasm.DecompilerEngine.IR.Visitors;

public interface IIRNodeVisitor
{
    void Visit(IRAddExpr expr);
    void Visit(IRCompareExpr expr);
    void Visit(IRConstantExpr expr);
    void Visit(IRDerefExpr expr);
    void Visit(IRFlagExpr expr);
    void Visit(IRLogicalExpr expr);
    void Visit(IRMemoryExpr expr);
    void Visit(IRMulExpr expr);
    void Visit(IRNotExpr expr);
    void Visit(IRRegisterExpr expr);
    void Visit(IRSegmentExpr expr);
    void Visit(IRSubExpr expr);
    void Visit(IRXorExpr expr);
    void Visit(IRAdcInstruction instr);
    void Visit(IRAddInstruction instr);
    void Visit(IRAndInstruction instr);
    void Visit(IRCallInstruction instr);
    void Visit(IRCmpInstruction instr);
    void Visit(IRDecInstruction instr);
    void Visit(IRDivInstruction instr);
    void Visit(IRIDivInstruction instr);
    void Visit(IRIncInstruction instr);
    void Visit(IRJumpInstruction instr);
    void Visit(IRLeaInstruction instr);
    void Visit(IRMoveInstruction instr);
    void Visit(IRMulInstruction instr);
    void Visit(IRNegInstruction instr);
    void Visit(IRNotInstruction instr);
    void Visit(IROrInstruction instr);
    void Visit(IRPopInstruction instr);
    void Visit(IRPushInstruction instr);
    void Visit(IRReturnInstruction instr);
    void Visit(IRSbbInstruction instr);
    void Visit(IRSubInstruction instr);
    void Visit(IRTestInstruction instr);
    void Visit(IRXorInstruction instr);
    void Visit(StubIRInstruction instr);

    void Visit(IRFlagEffect effect);
    void Visit(IRBlock block);
    void Visit(IRFunction function);
    void Visit(IRProgram program);
    void Visit(IRType type);
    void Visit(IRVariable variable);

    void Visit(IRUnflaggedJumpInstruction instr);
    void Visit(IRWiredJumpInstruction instr);
}
