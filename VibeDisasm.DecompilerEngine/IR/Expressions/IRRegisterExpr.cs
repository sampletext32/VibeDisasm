using System.Diagnostics;
using VibeDisasm.DecompilerEngine.IR.Visitors;

namespace VibeDisasm.DecompilerEngine.IR.Expressions;

/// <summary>
/// Represents a register operand in IR.
/// Example: eax -> IRRegister("eax")
/// </summary>
[DebuggerDisplay("{DebugDisplay}")]
public sealed class IRRegisterExpr : IRExpression
{
    public IRRegister Register { get; init; }

    public override List<IRExpression> SubExpressions => [];

    public IRRegisterExpr(IRRegister register) => Register = register;

    public override bool Equals(object? obj)
    {
        if (obj is IRRegisterExpr other)
        {
            return Register == other.Register;
        }

        return false;
    }

    public override void Accept(IIRNodeVisitor visitor) => visitor.VisitRegister(this);

    public override T? Accept<T>(IIRNodeReturningVisitor<T> visitor) where T : default => visitor.VisitRegister(this);

    public override int GetHashCode() => throw new NotImplementedException();

    internal override string DebugDisplay => $"IRRegisterExpr({Register:G})";
}

public enum IRRegister
{
    // 8-bit general-purpose
    AL, CL, DL, BL, AH, CH, DH, BH,

    // 16-bit general-purpose
    AX, CX, DX, BX, SP, BP, SI, DI,

    // 32-bit general-purpose
    EAX, ECX, EDX, EBX, ESP, EBP, ESI, EDI,

    // 64-bit general-purpose
    RAX, RCX, RDX, RBX, RSP, RBP, RSI, RDI,
    R8, R9, R10, R11, R12, R13, R14, R15,

    // FPU registers
    ST0, ST1, ST2, ST3, ST4, ST5, ST6, ST7,
}
