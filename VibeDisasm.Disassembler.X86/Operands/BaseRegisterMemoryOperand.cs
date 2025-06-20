namespace VibeDisasm.Disassembler.X86.Operands;

/// <summary>
/// Represents a memory operand with a base register in an x86 instruction (e.g., [eax])
/// </summary>
public class BaseRegisterMemoryOperand : MemoryOperand
{
    public override TResult Accept<TResult>(IOperandVisitor<TResult> visitor) => visitor.VisitBaseRegisterMemory(this);

    /// <summary>
    /// Gets or sets the base register
    /// </summary>
    public RegisterIndex BaseRegister { get; set; }

    /// <summary>
    /// Initializes a new instance of the BaseRegisterMemoryOperand class
    /// </summary>
    /// <param name="baseRegister">The base register</param>
    /// <param name="size">The size of the memory access in bits</param>
    /// <param name="segmentOverride">Optional segment override</param>
    public BaseRegisterMemoryOperand(RegisterIndex baseRegister, int size = 32, Segment? segmentOverride = null)
        : base(size, segmentOverride)
    {
        Type = OperandType.MemoryBaseReg;
        BaseRegister = baseRegister;
    }

    /// <summary>
    /// Returns a string representation of this operand
    /// </summary>
    public override string ToString()
    {
        var registerName = RegisterMapper.GetRegisterName(BaseRegister, 32);
        return $"{GetSizePrefix()}[{registerName}]";
    }
}
