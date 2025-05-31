using VibeDisasm.DecompilerEngine.IR.Expressions;
using VibeDisasm.DecompilerEngine.IR.Instructions;
using VibeDisasm.DecompilerEngine.IR.Model;
using VibeDisasm.DecompilerEngine.IRAnalyzers.IRLiftedInstructions;

namespace VibeDisasm.DecompilerEngine.IR.Visitors;

public abstract class BaseIRNodeVisitor : IIRNodeVisitor
{
    public virtual void Visit(IRAddExpr expr)
    {
    }

    public virtual void Visit(IRCompareExpr expr)
    {
    }

    public virtual void Visit(IRConstantExpr expr)
    {
    }

    public virtual void Visit(IRDerefExpr expr)
    {
    }

    public virtual void Visit(IRFlagExpr expr)
    {
    }

    public virtual void Visit(IRLogicalExpr expr)
    {
    }

    public virtual void Visit(IRMemoryExpr expr)
    {
    }

    public virtual void Visit(IRMulExpr expr)
    {
    }

    public virtual void Visit(IRNotExpr expr)
    {
    }

    public virtual void Visit(IRRegisterExpr expr)
    {
    }

    public virtual void Visit(IRSegmentExpr expr)
    {
    }

    public virtual void Visit(IRSubExpr expr)
    {
    }

    public virtual void Visit(IRXorExpr expr)
    {
    }

    public virtual void Visit(IRAdcInstruction instr)
    {
    }

    public virtual void Visit(IRAddInstruction instr)
    {
    }

    public virtual void Visit(IRAndInstruction instr)
    {
    }

    public virtual void Visit(IRCallInstruction instr)
    {
    }

    public virtual void Visit(IRCmpInstruction instr)
    {
    }

    public virtual void Visit(IRDecInstruction instr)
    {
    }

    public virtual void Visit(IRDivInstruction instr)
    {
    }

    public virtual void Visit(IRIDivInstruction instr)
    {
    }

    public virtual void Visit(IRIncInstruction instr)
    {
    }

    public virtual void Visit(IRJumpInstruction instr)
    {
    }

    public virtual void Visit(IRLeaInstruction instr)
    {
    }

    public virtual void Visit(IRMoveInstruction instr)
    {
    }

    public virtual void Visit(IRMulInstruction instr)
    {
    }

    public virtual void Visit(IRNegInstruction instr)
    {
    }

    public virtual void Visit(IRNotInstruction instr)
    {
    }

    public virtual void Visit(IROrInstruction instr)
    {
    }

    public virtual void Visit(IRPopInstruction instr)
    {
    }

    public virtual void Visit(IRPushInstruction instr)
    {
    }

    public virtual void Visit(IRReturnInstruction instr)
    {
    }

    public virtual void Visit(IRSbbInstruction instr)
    {
    }

    public virtual void Visit(IRSubInstruction instr)
    {
    }

    public virtual void Visit(IRTestInstruction instr)
    {
    }

    public virtual void Visit(IRXorInstruction instr)
    {
    }

    public virtual void Visit(StubIRInstruction instr)
    {
    }

    public virtual void Visit(IRFlagEffect effect)
    {
    }

    public virtual void Visit(IRBlock block)
    {
    }

    public virtual void Visit(IRFunction function)
    {
    }

    public virtual void Visit(IRProgram program)
    {
    }

    public virtual void Visit(IRType type)
    {
    }

    public virtual void Visit(IRVariable variable)
    {
    }

    public virtual void Visit(IRUnflaggedJumpInstruction instr)
    {
    }

    public virtual void Visit(IRWiredJumpInstruction instr)
    {
    }
}
