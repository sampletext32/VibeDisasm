using System.Diagnostics.Contracts;
using VibeDisasm.Disassembler.X86.Operands;

namespace VibeDisasm.Disassembler.X86;

/// <summary>Higher-level abstraction over x86 instructions for control flow analysis.</summary>
public class AsmInstruction
{
    public TResult Accept<TResult>(IInstructionVisitor<TResult> visitor) => visitor.Visit(this);
    public Instruction RawInstruction { get; }
    public string ComputedView { get; }
    public string ComputedAddressView { get; }

    public ulong Address => RawInstruction.Address;
    public byte Length => (byte)RawInstruction.Length;
    public InstructionType Type => RawInstruction.Type;

    public AsmInstruction(Instruction instruction)
    {
        RawInstruction = instruction;
        ComputedView = instruction.ToString();
        ComputedAddressView = instruction.Address.ToString("X8");
    }

    [Pure] public uint GetNextSequentialAddress() => (uint)(Address + Length);
    [Pure] public bool IsJump() => Type.IsConditionalJump() || Type.IsUnconditionalJump();
    [Pure] public bool IsConditionalJump() => Type.IsConditionalJump();
    [Pure] public bool IsUnconditionalJump() => Type.IsUnconditionalJump();

    /// <summary>Gets the jump target address if this is a jump instruction.</summary>
    [Pure]
    public uint? GetJumpTargetAddress()
    {
        if (!IsJump())
        {
            return null;
        }

        if (RawInstruction.StructuredOperands.Count > 0 &&
            RawInstruction.StructuredOperands[0] is ImmediateOperand imm)
        {
            return (uint)imm.Value;
        }

        if (RawInstruction.StructuredOperands.Count > 0 &&
            RawInstruction.StructuredOperands[0] is RelativeOffsetOperand roo)
        {
            return roo.TargetAddress;
        }

        return null;
    }

    public override string ToString() => $"{Address:X8}: {ComputedView}";
}
