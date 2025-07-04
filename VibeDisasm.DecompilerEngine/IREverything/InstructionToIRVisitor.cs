using VibeDisasm.DecompilerEngine.IREverything.Expressions;
using VibeDisasm.DecompilerEngine.IREverything.Instructions;
using VibeDisasm.DecompilerEngine.IREverything.Model;
using VibeDisasm.Disassembler.X86;

namespace VibeDisasm.DecompilerEngine.IREverything;

/// <summary>
/// Converts AsmInstruction to IRInstruction using the visitor pattern.
/// </summary>
public sealed class InstructionToIRVisitor : IInstructionVisitor<IRInstruction>
{
    private static readonly OperandToIRExpressionVisitor OperandVisitor = new();

    private static IRExpression OperandToIR(Operand op) => op.Accept(OperandVisitor);

    public IRInstruction Visit(AsmInstruction instruction)
    {
        var operands = instruction.RawInstruction.StructuredOperands;
        switch (instruction.Type)
        {
            case InstructionType.Mov:
                return new IRMoveInstruction(
                    OperandToIR(operands[0]),
                    OperandToIR(operands[1])
                );
            case InstructionType.Add:
                return new IRAddInstruction(
                    OperandToIR(operands[0]),
                    OperandToIR(operands[1])
                );
            case InstructionType.Sub:
                return new IRSubInstruction(
                    OperandToIR(operands[0]),
                    OperandToIR(operands[1])
                );
            case InstructionType.Mul:
            case InstructionType.IMul:
                return new IRMulInstruction(
                    OperandToIR(operands[0]),
                    OperandToIR(operands[1])
                );
            case InstructionType.Div:
                // EAX = EAX / src, EDX = EAX % src (unsigned)
                return new IRDivInstruction(
                    new IRRegisterExpr(IRRegister.EAX),
                    OperandToIR(operands[0]),
                    new IRRegisterExpr(IRRegister.EAX),
                    new IRRegisterExpr(IRRegister.EDX)
                );
            case InstructionType.IDiv:
                // EAX = EAX / src, EDX = EAX % src (signed)
                return new IRIDivInstruction(
                    new IRRegisterExpr(IRRegister.EAX),
                    OperandToIR(operands[0]),
                    new IRRegisterExpr(IRRegister.EAX),
                    new IRRegisterExpr(IRRegister.EDX)
                );
            case InstructionType.And:
                return new IRAndInstruction(
                    OperandToIR(operands[0]),
                    OperandToIR(operands[1])
                );
            case InstructionType.Or:
                return new IROrInstruction(
                    OperandToIR(operands[0]),
                    OperandToIR(operands[1])
                );
            case InstructionType.Xor:
                return new IRXorInstruction(
                    OperandToIR(operands[0]),
                    OperandToIR(operands[1])
                );
            case InstructionType.Not:
                return new IRNotInstruction(
                    OperandToIR(operands[0])
                );
            case InstructionType.Test:
                return new IRTestInstruction(
                    OperandToIR(operands[0]),
                    OperandToIR(operands[1])
                );
            case InstructionType.Cmp:
                return new IRCmpInstruction(
                    OperandToIR(operands[0]),
                    OperandToIR(operands[1])
                );
            case InstructionType.Ret:
                return operands.Count > 0
                    ? new IRReturnInstruction(OperandToIR(operands[0]))
                    : new IRReturnInstruction();
            case InstructionType.Push:
                return new IRPushInstruction(
                    OperandToIR(operands[0])
                );
            case InstructionType.Pop:
                return new IRPopInstruction(
                    OperandToIR(operands[0])
                );
            case InstructionType.Lea:
                return new IRLeaInstruction(
                    OperandToIR(operands[0]),
                    OperandToIR(operands[1])
                );
            case InstructionType.Inc:
                return new IRIncInstruction(
                    OperandToIR(operands[0])
                );
            case InstructionType.Dec:
                return new IRDecInstruction(
                    OperandToIR(operands[0])
                );
            case InstructionType.Neg:
                return new IRNegInstruction(
                    OperandToIR(operands[0])
                );
            case InstructionType.Adc:
                return new IRAdcInstruction(
                    OperandToIR(operands[0]),
                    OperandToIR(operands[1])
                );
            case InstructionType.Sbb:
                return new IRSbbInstruction(
                    OperandToIR(operands[0]),
                    OperandToIR(operands[1])
                );
            case InstructionType.Call:
                return new IRCallInstruction(
                    OperandToIR(operands[0])
                );
            case InstructionType.Jmp:
                return new IRJumpInstruction(
                    OperandToIR(operands[0])
                );

            // Simple flag-based jumps
            case InstructionType.Jz:
                return new IRConditionalJumpInstruction(
                    OperandToIR(operands[0]),
                    IR.CompareEqual(new IRFlagExpr(IRFlag.Zero), IR.True())
                );
            case InstructionType.Jnz:
                return new IRConditionalJumpInstruction(
                    OperandToIR(operands[0]),
                    IR.CompareEqual(new IRFlagExpr(IRFlag.Zero), IR.False())
                );
            case InstructionType.Js:
                return new IRConditionalJumpInstruction(
                    OperandToIR(operands[0]),
                    IR.CompareEqual(new IRFlagExpr(IRFlag.Sign), IR.True())
                );
            case InstructionType.Jns:
                return new IRConditionalJumpInstruction(
                    OperandToIR(operands[0]),
                    IR.CompareEqual(new IRFlagExpr(IRFlag.Sign), IR.False())
                );
            case InstructionType.Jo:
                return new IRConditionalJumpInstruction(
                    OperandToIR(operands[0]),
                    IR.CompareEqual(new IRFlagExpr(IRFlag.Overflow), IR.True())
                );
            case InstructionType.Jno:
                return new IRConditionalJumpInstruction(
                    OperandToIR(operands[0]),
                    IR.CompareEqual(new IRFlagExpr(IRFlag.Overflow), IR.False())
                );
            case InstructionType.Jp:
                return new IRConditionalJumpInstruction(
                    OperandToIR(operands[0]),
                    IR.CompareEqual(new IRFlagExpr(IRFlag.Parity), IR.True())
                );
            case InstructionType.Jpo:
                return new IRConditionalJumpInstruction(
                    OperandToIR(operands[0]),
                    IR.CompareEqual(new IRFlagExpr(IRFlag.Parity), IR.False())
                );
            // Flag comparison jumps
            case InstructionType.Jnge:
                return new IRConditionalJumpInstruction(
                    OperandToIR(operands[0]),
                    IR.CompareNotEqual(new IRFlagExpr(IRFlag.Sign), new IRFlagExpr(IRFlag.Overflow))
                );

            case InstructionType.Jnl:
                return new IRConditionalJumpInstruction(
                    OperandToIR(operands[0]),
                    IR.CompareEqual(new IRFlagExpr(IRFlag.Sign), new IRFlagExpr(IRFlag.Overflow))
                );

            case InstructionType.Jng: // Jump if Not Greater (same as JLE)
                return new IRConditionalJumpInstruction(
                    OperandToIR(operands[0]),
                    IR.LogicalOr(
                        IR.CompareEqual(new IRFlagExpr(IRFlag.Zero), IR.True()),
                        IR.CompareNotEqual(new IRFlagExpr(IRFlag.Sign), new IRFlagExpr(IRFlag.Overflow))
                    )
                );

            case InstructionType.Jnle:
                return new IRConditionalJumpInstruction(
                    OperandToIR(operands[0]),
                    IR.LogicalAnd(
                        IR.CompareEqual(new IRFlagExpr(IRFlag.Zero), IR.False()),
                        IR.CompareEqual(new IRFlagExpr(IRFlag.Sign), new IRFlagExpr(IRFlag.Overflow))
                    )
                );

            // Unsigned comparison jumps
            case InstructionType.Jnbe:
                return new IRConditionalJumpInstruction(
                    OperandToIR(operands[0]),
                    IR.LogicalAnd(
                        IR.CompareEqual(new IRFlagExpr(IRFlag.Carry), IR.False()),
                        IR.CompareEqual(new IRFlagExpr(IRFlag.Zero), IR.False())
                    )
                );

            case InstructionType.Jae:
                return new IRConditionalJumpInstruction(
                    OperandToIR(operands[0]),
                    IR.CompareEqual(new IRFlagExpr(IRFlag.Carry), IR.False())
                );

            case InstructionType.Jb:
                return new IRConditionalJumpInstruction(
                    OperandToIR(operands[0]),
                    IR.CompareEqual(new IRFlagExpr(IRFlag.Carry), IR.True())
                );

            case InstructionType.Jna:
                return new IRConditionalJumpInstruction(
                    OperandToIR(operands[0]),
                    IR.LogicalOr(
                        IR.CompareEqual(new IRFlagExpr(IRFlag.Carry), IR.False()),
                        IR.CompareEqual(new IRFlagExpr(IRFlag.Zero), IR.False())
                    )
                );
            // Loop instructions
            case InstructionType.Loop:
                // Loop decrements ECX and jumps if ECX != 0
                return new IRConditionalJumpInstruction(
                    OperandToIR(operands[0]),
                    IR.CompareNotEqual(new IRRegisterExpr(IRRegister.ECX), IR.Int(0))
                ); // Would need special handling for ECX

            case InstructionType.Loope:
                // Loop decrements ECX and jumps if ECX != 0 and ZF = 1
                return new IRConditionalJumpInstruction(
                    OperandToIR(operands[0]),
                    IR.LogicalAnd(
                        IR.CompareNotEqual(new IRRegisterExpr(IRRegister.ECX), IR.Int(0)),
                        IR.CompareEqual(new IRFlagExpr(IRFlag.Zero), IR.Int(0))
                    )
                );

            case InstructionType.Loopne:
                // Loop decrements ECX and jumps if ECX != 0 and ZF = 0
                return new IRConditionalJumpInstruction(
                    OperandToIR(operands[0]),
                    IR.LogicalAnd(
                        IR.CompareNotEqual(
                            new IRRegisterExpr(IRRegister.ECX),
                            IR.Int(0)
                        ),
                        IR.CompareEqual(
                            new IRFlagExpr(IRFlag.Zero),
                            IR.Int(0)
                        )
                    )
                );

            case InstructionType.Jcxz:
                // Jump if CX = 0
                return new IRConditionalJumpInstruction(
                    OperandToIR(operands[0]),
                    IR.CompareEqual(new IRRegisterExpr(IRRegister.CX), IR.Int(0))
                );

            case InstructionType.Jecxz:
                // Jump if ECX = 0
                return new IRConditionalJumpInstruction(
                    OperandToIR(operands[0]),
                    IR.CompareEqual(new IRRegisterExpr(IRRegister.ECX), IR.Int(0))
                );
            case InstructionType.Fld:
                return new IRFldInstruction(
                    OperandToIR(operands[0])
                );
            case InstructionType.Fstp:
                return new IRFstpInstruction(
                    OperandToIR(operands[0])
                );
            case InstructionType.Fadd:
                return new IRFaddInstruction(
                    operands.Count > 0 ? OperandToIR(operands[0]) : new IRRegisterExpr(IRRegister.ST1)
                );
            case InstructionType.Shl:
                return new IRShlInstruction(
                    OperandToIR(operands[0]),
                    OperandToIR(operands[1])
                );
            case InstructionType.Shr:
                return new IRShrInstruction(
                    OperandToIR(operands[0]),
                    OperandToIR(operands[1])
                );
            case InstructionType.Movzx:
                return new IRMovzxInstruction(
                    OperandToIR(operands[1]), // Source (smaller operand)
                    OperandToIR(operands[0])  // Destination (larger operand)
                );
            default:
                return new StubIRInstruction(instruction.Type);
        }
    }
}
