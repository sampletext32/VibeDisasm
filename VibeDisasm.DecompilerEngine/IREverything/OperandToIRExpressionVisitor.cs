using VibeDisasm.DecompilerEngine.IREverything.Expressions;
using VibeDisasm.Disassembler.X86;
using VibeDisasm.Disassembler.X86.Operands;

namespace VibeDisasm.DecompilerEngine.IREverything;

/// <summary>
/// Converts x86 Operand to IRExpression using the Operand visitor pattern.
/// </summary>
public sealed class OperandToIRExpressionVisitor : IOperandVisitor<IRExpression>
{
    public IRExpression VisitRegister(RegisterOperand operand)
        => new IRRegisterExpr(X86RegisterToIrRegister(operand.Register, operand.Size));

    public IRExpression VisitImmediate(ImmediateOperand operand)
        => IR.FromSize(operand.Value, operand.Size);

    public IRExpression VisitRelativeOffset(RelativeOffsetOperand operand)
        => IR.FromSize(operand.TargetAddress, operand.Size);

    public IRExpression VisitDisplacementMemory(DisplacementMemoryOperand operand)
        => new IRDerefExpr(
            IR.Add(
                new IRRegisterExpr(X86RegisterToIrRegister(operand.BaseRegister, operand.Size)),
                IR.Long(operand.Displacement)
            )
        );

    public IRExpression VisitBaseRegisterMemory(BaseRegisterMemoryOperand operand)
        => new IRDerefExpr(new IRRegisterExpr(X86RegisterToIrRegister(operand.BaseRegister, operand.Size)));

    public IRExpression VisitScaledIndexMemory(ScaledIndexMemoryOperand operand)
    {
        var indexExpr = new IRRegisterExpr(X86RegisterToIrRegister(operand.IndexRegister, operand.Size));
        var scaleExpr = IR.Int(operand.Scale);
        var dispExpr = IR.Long(operand.Displacement);
        if (operand.BaseRegister is not null)
        {
            var baseExpr = new IRRegisterExpr(X86RegisterToIrRegister(operand.BaseRegister.Value, operand.Size));

            // *(base + (index * scale) + displacement)
            return new IRDerefExpr(
                IR.Add(
                    baseExpr,
                    IR.Add(
                        new IRMulExpr(indexExpr, scaleExpr),
                        dispExpr
                    )
                )
            );
        }
        else
        {
            // *((index * scale) + displacement)
            return new IRDerefExpr(
                IR.Add(
                    new IRMulExpr(indexExpr, scaleExpr),
                    dispExpr
                )
            );
        }
    }

    public IRExpression VisitDirectMemory(DirectMemoryOperand operand)
        => new IRDerefExpr(IR.Long(operand.Address));

    public IRExpression VisitFPURegister(FPURegisterOperand operand)
        => new IRRegisterExpr(X86RegisterToIrRegister(operand.RegisterIndex, operand.Size));

    public IRExpression VisitRegister8(Register8Operand operand)
        => new IRRegisterExpr(X86RegisterToIrRegister(operand.Register, operand.Size));

    public IRExpression VisitFarPointer(FarPointerOperand operand)
        // TODO: Handle far pointers properly
        => new IRMemoryExpr($"{operand.BaseRegister}:{operand.Displacement}");

    public IRExpression VisitOperand(Operand operand)
        => throw new InvalidOperationException($"Unsupported Operand to IR conversion. {operand.GetType().Name}");

    private static IRRegister X86RegisterToIrRegister(RegisterIndex registerIndex, int size)
    {
        if (size == 16)
        {
            return registerIndex switch
            {
                RegisterIndex.A => IRRegister.AX,
                RegisterIndex.B => IRRegister.BX,
                RegisterIndex.C => IRRegister.CX,
                RegisterIndex.D => IRRegister.DX,
                RegisterIndex.Si => IRRegister.SI,
                RegisterIndex.Di => IRRegister.DI,
                RegisterIndex.Bp => IRRegister.BP,
                RegisterIndex.Sp => IRRegister.SP,
                _ => throw new ArgumentOutOfRangeException(nameof(registerIndex), registerIndex, null)
            };
        }
        else if (size == 32)
        {
            return registerIndex switch
            {
                RegisterIndex.A => IRRegister.EAX,
                RegisterIndex.B => IRRegister.EBX,
                RegisterIndex.C => IRRegister.ECX,
                RegisterIndex.D => IRRegister.EDX,
                RegisterIndex.Si => IRRegister.ESI,
                RegisterIndex.Di => IRRegister.EDI,
                RegisterIndex.Bp => IRRegister.EBP,
                RegisterIndex.Sp => IRRegister.ESP,
                _ => throw new ArgumentOutOfRangeException(nameof(registerIndex), registerIndex, null)
            };
        }
        else if (size == 64)
        {
            return registerIndex switch
            {
                RegisterIndex.A => IRRegister.RAX,
                RegisterIndex.B => IRRegister.RBX,
                RegisterIndex.C => IRRegister.RCX,
                RegisterIndex.D => IRRegister.RDX,
                RegisterIndex.Si => IRRegister.RSI,
                RegisterIndex.Di => IRRegister.RDI,
                RegisterIndex.Bp => IRRegister.RBP,
                RegisterIndex.Sp => IRRegister.RSP,
                _ => throw new ArgumentOutOfRangeException(nameof(registerIndex), registerIndex, null)
            };
        }
        else
        {
            // by default assume 32-bit registers
            return registerIndex switch
            {
                RegisterIndex.A => IRRegister.EAX,
                RegisterIndex.B => IRRegister.EBX,
                RegisterIndex.C => IRRegister.ECX,
                RegisterIndex.D => IRRegister.EDX,
                RegisterIndex.Si => IRRegister.ESI,
                RegisterIndex.Di => IRRegister.EDI,
                RegisterIndex.Bp => IRRegister.EBP,
                RegisterIndex.Sp => IRRegister.ESP,
                _ => throw new ArgumentOutOfRangeException(nameof(registerIndex), registerIndex, null)
            };
        }
    }

    private static IRRegister X86RegisterToIrRegister(RegisterIndex8 registerIndex, int size)
    {
        return registerIndex switch
        {
            RegisterIndex8.AL => IRRegister.AL,
            RegisterIndex8.CL => IRRegister.CL,
            RegisterIndex8.DL => IRRegister.DL,
            RegisterIndex8.BL => IRRegister.BL,
            RegisterIndex8.AH => IRRegister.AH,
            RegisterIndex8.CH => IRRegister.CH,
            RegisterIndex8.DH => IRRegister.DH,
            RegisterIndex8.BH => IRRegister.BH,
            _ => throw new ArgumentOutOfRangeException(nameof(registerIndex), registerIndex, null)
        };
    }

    private static IRRegister X86RegisterToIrRegister(FpuRegisterIndex registerIndex, int size)
    {
        return registerIndex switch
        {
            FpuRegisterIndex.ST0 => IRRegister.ST0,
            FpuRegisterIndex.ST1 => IRRegister.ST1,
            FpuRegisterIndex.ST2 => IRRegister.ST2,
            FpuRegisterIndex.ST3 => IRRegister.ST3,
            FpuRegisterIndex.ST4 => IRRegister.ST4,
            FpuRegisterIndex.ST5 => IRRegister.ST5,
            FpuRegisterIndex.ST6 => IRRegister.ST6,
            FpuRegisterIndex.ST7 => IRRegister.ST7,
            _ => throw new ArgumentOutOfRangeException(nameof(registerIndex), registerIndex, null)
        };
    }
}
