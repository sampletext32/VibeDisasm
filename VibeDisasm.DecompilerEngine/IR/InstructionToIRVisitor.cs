using VibeDisasm.Disassembler.X86;
using VibeDisasm.Disassembler.X86.Operands;
using VibeDisasm.DecompilerEngine.IR.Expressions;
using VibeDisasm.DecompilerEngine.IR.Instructions;
using VibeDisasm.DecompilerEngine.IR.Model;

namespace VibeDisasm.DecompilerEngine.IR;

/// <summary>
/// Converts AsmInstruction to IRInstruction using the visitor pattern.
/// </summary>
public sealed class InstructionToIRVisitor : IInstructionVisitor<IRInstruction>
{
    private static readonly OperandToIRExpressionVisitor OperandVisitor = new();

    private static IRExpression OperandToIR(Operand? op)
        => op is null ? new IRConstantExpr { Value = 0, Type = new IRType { Name = "int" } }
        : op.Accept(OperandVisitor);

    public IRInstruction Visit(AsmInstruction instruction)
    {
        var ops = instruction.RawInstruction.StructuredOperands;
        switch (instruction.Type)
        {
            case InstructionType.Mov:
                return new IRMoveInstruction(
                    OperandToIR(ops.ElementAtOrDefault(0)),
                    OperandToIR(ops.ElementAtOrDefault(1))
                );
            case InstructionType.Add:
                return new IRAddInstruction(
                    OperandToIR(ops.ElementAtOrDefault(0)),
                    OperandToIR(ops.ElementAtOrDefault(1))
                );
            case InstructionType.Sub:
                return new IRSubInstruction(
                    OperandToIR(ops.ElementAtOrDefault(0)),
                    OperandToIR(ops.ElementAtOrDefault(1))
                );
            case InstructionType.Mul:
            case InstructionType.IMul:
                return new IRMulInstruction(
                    OperandToIR(ops.ElementAtOrDefault(0)),
                    OperandToIR(ops.ElementAtOrDefault(1))
                );
            case InstructionType.Div:
                // EAX = EAX / src, EDX = EAX % src (unsigned)
                return new IRDivInstruction(
                    new IRRegisterExpr(IRRegister.EAX),
                    OperandToIR(ops.ElementAtOrDefault(0)),
                    new IRRegisterExpr(IRRegister.EAX),
                    new IRRegisterExpr(IRRegister.EDX)
                );
            case InstructionType.IDiv:
                // EAX = EAX / src, EDX = EAX % src (signed)
                return new IRIDivInstruction(
                    new IRRegisterExpr(IRRegister.EAX),
                    OperandToIR(ops.ElementAtOrDefault(0)),
                    new IRRegisterExpr(IRRegister.EAX),
                    new IRRegisterExpr(IRRegister.EDX)
                );
            case InstructionType.And:
                return new IRAndInstruction(
                    OperandToIR(ops.ElementAtOrDefault(0)),
                    OperandToIR(ops.ElementAtOrDefault(1))
                );
            case InstructionType.Or:
                return new IROrInstruction(
                    OperandToIR(ops.ElementAtOrDefault(0)),
                    OperandToIR(ops.ElementAtOrDefault(1))
                );
            case InstructionType.Xor:
                return new IRXorInstruction(
                    OperandToIR(ops.ElementAtOrDefault(0)),
                    OperandToIR(ops.ElementAtOrDefault(1))
                );
            case InstructionType.Not:
                return new IRNotInstruction(
                    OperandToIR(ops.ElementAtOrDefault(0))
                );
            case InstructionType.Test:
            case InstructionType.Cmp:
                return new IRCmpInstruction(
                    OperandToIR(ops.ElementAtOrDefault(0)),
                    OperandToIR(ops.ElementAtOrDefault(1))
                );
            case InstructionType.Ret:
                return new IRReturnInstruction(OperandToIR(ops.ElementAtOrDefault(0)));
            case InstructionType.Push:
                return new IRPushInstruction(
                    OperandToIR(ops.ElementAtOrDefault(0))
                );
            case InstructionType.Pop:
                return new IRPopInstruction(
                    OperandToIR(ops.ElementAtOrDefault(0))
                );
            case InstructionType.Lea:
                return new IRLeaInstruction(
                    OperandToIR(ops.ElementAtOrDefault(0)),
                    OperandToIR(ops.ElementAtOrDefault(1))
                );
            case InstructionType.Inc:
                return new IRIncInstruction(
                    OperandToIR(ops.ElementAtOrDefault(0))
                );
            case InstructionType.Dec:
                return new IRDecInstruction(
                    OperandToIR(ops.ElementAtOrDefault(0))
                );
            case InstructionType.Neg:
                return new IRNegInstruction(
                    OperandToIR(ops.ElementAtOrDefault(0))
                );
            case InstructionType.Adc:
                return new IRAdcInstruction(
                    OperandToIR(ops.ElementAtOrDefault(0)),
                    OperandToIR(ops.ElementAtOrDefault(1))
                );
            case InstructionType.Sbb:
                return new IRSbbInstruction(
                    OperandToIR(ops.ElementAtOrDefault(0)),
                    OperandToIR(ops.ElementAtOrDefault(1))
                );
            case InstructionType.Call:
                return new IRCallInstruction(
                    OperandToIR(ops.ElementAtOrDefault(0))
                );
            case InstructionType.Jmp:
                return new IRJumpInstruction(OperandToIR(ops.ElementAtOrDefault(0)));
                
            // Simple flag-based jumps
            case InstructionType.Jz:
                return new IRJumpInstruction(OperandToIR(ops.ElementAtOrDefault(0)), new IRFlagConditionExpr(IRFlag.Zero, true));
            case InstructionType.Jnz:
                return new IRJumpInstruction(OperandToIR(ops.ElementAtOrDefault(0)), new IRFlagConditionExpr(IRFlag.Zero, false));
            case InstructionType.Js:
                return new IRJumpInstruction(OperandToIR(ops.ElementAtOrDefault(0)), new IRFlagConditionExpr(IRFlag.Sign, true));
            case InstructionType.Jns:
                return new IRJumpInstruction(OperandToIR(ops.ElementAtOrDefault(0)), new IRFlagConditionExpr(IRFlag.Sign, false));
            case InstructionType.Jo:
                return new IRJumpInstruction(OperandToIR(ops.ElementAtOrDefault(0)), new IRFlagConditionExpr(IRFlag.Overflow, true));
            case InstructionType.Jno:
                return new IRJumpInstruction(OperandToIR(ops.ElementAtOrDefault(0)), new IRFlagConditionExpr(IRFlag.Overflow, false));
            case InstructionType.Jp:
                return new IRJumpInstruction(OperandToIR(ops.ElementAtOrDefault(0)), new IRFlagConditionExpr(IRFlag.Parity, true));
            case InstructionType.Jpo:
                return new IRJumpInstruction(OperandToIR(ops.ElementAtOrDefault(0)), new IRFlagConditionExpr(IRFlag.Parity, false));
                
            // Flag comparison jumps
            case InstructionType.Jnge:
                return new IRJumpInstruction(OperandToIR(ops.ElementAtOrDefault(0)), 
                    new IRFlagConditionExpr(IRFlag.Sign, IRFlagConditionExpr.ComparisonType.NotEquals, IRFlag.Overflow));

            case InstructionType.Jnl:
                return new IRJumpInstruction(OperandToIR(ops.ElementAtOrDefault(0)), 
                    new IRFlagConditionExpr(IRFlag.Sign, IRFlagConditionExpr.ComparisonType.Equals, IRFlag.Overflow));
                
            // Unsigned comparison jumps
            case InstructionType.Jnbe:
                return new IRJumpInstruction(OperandToIR(ops.ElementAtOrDefault(0)), 
                    new IRFlagConditionExpr(
                        new IRFlagConditionExpr(IRFlag.Carry, false),
                        IRFlagConditionExpr.ComparisonType.And,
                        new IRFlagConditionExpr(IRFlag.Zero, false)));
                        
            case InstructionType.Jae:
                return new IRJumpInstruction(OperandToIR(ops.ElementAtOrDefault(0)), 
                    new IRFlagConditionExpr(IRFlag.Carry, false));
                    
            case InstructionType.Jb:
                return new IRJumpInstruction(OperandToIR(ops.ElementAtOrDefault(0)), 
                    new IRFlagConditionExpr(IRFlag.Carry, true));
                    
            case InstructionType.Jna:
                return new IRJumpInstruction(OperandToIR(ops.ElementAtOrDefault(0)), 
                    new IRFlagConditionExpr(
                        new IRFlagConditionExpr(IRFlag.Carry, true),
                        IRFlagConditionExpr.ComparisonType.Or,
                        new IRFlagConditionExpr(IRFlag.Zero, true)));
                
                        
            // Loop instructions
            case InstructionType.Loop:
                // Loop decrements ECX and jumps if ECX != 0
                return new IRJumpInstruction(OperandToIR(ops.ElementAtOrDefault(0)), 
                    new IRFlagConditionExpr("ECX != 0")); // Would need special handling for ECX
                    
            case InstructionType.Loope:
                // Loop decrements ECX and jumps if ECX != 0 and ZF = 1
                return new IRJumpInstruction(OperandToIR(ops.ElementAtOrDefault(0)), 
                    new IRFlagConditionExpr(
                        new IRFlagConditionExpr("ECX != 0"),
                        IRFlagConditionExpr.ComparisonType.And,
                        new IRFlagConditionExpr(IRFlag.Zero, true)));
                        
            case InstructionType.Loopne:
                // Loop decrements ECX and jumps if ECX != 0 and ZF = 0
                return new IRJumpInstruction(OperandToIR(ops.ElementAtOrDefault(0)), 
                    new IRFlagConditionExpr(
                        new IRFlagConditionExpr("ECX != 0"),
                        IRFlagConditionExpr.ComparisonType.And,
                        new IRFlagConditionExpr(IRFlag.Zero, false)));
                        
            case InstructionType.Jcxz:
                // Jump if CX = 0
                return new IRJumpInstruction(OperandToIR(ops.ElementAtOrDefault(0)), 
                    new IRFlagConditionExpr("CX == 0"));
                    
            case InstructionType.Jecxz:
                // Jump if ECX = 0
                return new IRJumpInstruction(OperandToIR(ops.ElementAtOrDefault(0)), 
                    new IRFlagConditionExpr("ECX == 0"));
            default:
                return new StubIRInstruction();
        }
    }

    private sealed class StubIRInstruction : IRInstruction
    {
        public override IRExpression? Result => null;
        public override IReadOnlyList<IRExpression> Operands => [];

        public override string ToString() => "StubIRInstruction - Not implemented";
    }
}
