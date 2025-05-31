using VibeDisasm.DecompilerEngine.IR.Expressions;
using VibeDisasm.DecompilerEngine.IR.Instructions;
using VibeDisasm.DecompilerEngine.IR.Model;
using VibeDisasm.DecompilerEngine.IRAnalyzers.IRLiftedInstructions;

namespace VibeDisasm.DecompilerEngine.IR.Visitors;

public interface IIRNodeReturningVisitor<out TReturn>
{
    TReturn Visit(IRAddExpr expr);
    TReturn Visit(IRCompareExpr expr);
    TReturn Visit(IRConstantExpr expr);
    TReturn Visit(IRDerefExpr expr);
    TReturn Visit(IRFlagExpr expr);
    TReturn Visit(IRLogicalExpr expr);
    TReturn Visit(IRMemoryExpr expr);
    TReturn Visit(IRMulExpr expr);
    TReturn Visit(IRNotExpr expr);
    TReturn Visit(IRRegisterExpr expr);
    TReturn Visit(IRSegmentExpr expr);
    TReturn Visit(IRSubExpr expr);
    TReturn Visit(IRXorExpr expr);
    TReturn Visit(IRAdcInstruction instr);
    TReturn Visit(IRAddInstruction instr);
    TReturn Visit(IRAndInstruction instr);
    TReturn Visit(IRCallInstruction instr);
    TReturn Visit(IRCmpInstruction instr);
    TReturn Visit(IRDecInstruction instr);
    TReturn Visit(IRDivInstruction instr);
    TReturn Visit(IRIDivInstruction instr);
    TReturn Visit(IRIncInstruction instr);
    TReturn Visit(IRJumpInstruction instr);
    TReturn Visit(IRLeaInstruction instr);
    TReturn Visit(IRMoveInstruction instr);
    TReturn Visit(IRMulInstruction instr);
    TReturn Visit(IRNegInstruction instr);
    TReturn Visit(IRNotInstruction instr);
    TReturn Visit(IROrInstruction instr);
    TReturn Visit(IRPopInstruction instr);
    TReturn Visit(IRPushInstruction instr);
    TReturn Visit(IRReturnInstruction instr);
    TReturn Visit(IRSbbInstruction instr);
    TReturn Visit(IRSubInstruction instr);
    TReturn Visit(IRTestInstruction instr);
    TReturn Visit(IRXorInstruction instr);
    TReturn Visit(StubIRInstruction instr);

    TReturn Visit(IRFlagEffect effect);
    TReturn Visit(IRBlock block);
    TReturn Visit(IRFunction function);
    TReturn Visit(IRProgram program);
    TReturn Visit(IRType type);
    TReturn Visit(IRVariable variable);

    TReturn Visit(IRUnflaggedJumpInstruction instr);
    TReturn Visit(IRWiredJumpInstruction instr);
}
