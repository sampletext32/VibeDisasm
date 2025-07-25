using VibeDisasm.Core;

namespace VibeDisasm.Disassembler.X86.Operands;

/// <summary>
/// Represents a memory operand with a base register and displacement in an x86 instruction (e.g., [eax+0x4])
/// </summary>
public class DisplacementMemoryOperand : MemoryOperand
{
    public override TResult Accept<TResult>(IOperandVisitor<TResult> visitor) => visitor.VisitDisplacementMemory(this);

    /// <summary>
    /// Gets or sets the base register
    /// </summary>
    public RegisterIndex BaseRegister { get; set; }

    /// <summary>
    /// Gets or sets the displacement value
    /// </summary>
    public long Displacement { get; set; }

    /// <summary>
    /// Initializes a new instance of the DisplacementMemoryOperand class
    /// </summary>
    /// <param name="baseRegister">The base register</param>
    /// <param name="displacement">The displacement value</param>
    /// <param name="size">The size of the memory access in bits</param>
    /// <param name="segmentOverride">Optional segment override</param>
    public DisplacementMemoryOperand(RegisterIndex baseRegister, long displacement, int size = 32, Segment? segmentOverride = null)
        : base(size, segmentOverride)
    {
        Type = OperandType.MemoryBaseRegPlusOffset;
        BaseRegister = baseRegister;
        Displacement = displacement;
    }

    /// <summary>
    /// Returns a string representation of this operand
    /// </summary>
    public override string ToString()
    {
        // Get register name
        var registerName = RegisterMapper.GetRegisterName(BaseRegister, 32);

        var formatted = NumberFormatter.FormatNumber(Displacement, true);

        return $"{GetSizePrefix()}[{registerName}{formatted}]";
    }
}
