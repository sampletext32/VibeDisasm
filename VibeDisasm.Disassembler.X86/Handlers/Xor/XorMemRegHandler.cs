using VibeDisasm.Disassembler.X86.Operands;

namespace VibeDisasm.Disassembler.X86.Handlers.Xor;

/// <summary>
/// Handler for XOR r/m32, r32 instruction (0x31)
/// </summary>
public class XorMemRegHandler : InstructionHandler
{
    /// <summary>
    /// Initializes a new instance of the XorMemRegHandler class
    /// </summary>
    /// <param name="decoder">The instruction decoder that owns this handler</param>
    public XorMemRegHandler(InstructionDecoder decoder)
        : base(decoder)
    {
    }

    /// <summary>
    /// Checks if this handler can decode the given opcode
    /// </summary>
    /// <param name="opcode">The opcode to check</param>
    /// <returns>True if this handler can decode the opcode</returns>
    public override bool CanHandle(byte opcode)
    {
        // Only handle opcode 0x31 when the operand size prefix is NOT present
        // This ensures 16-bit handlers get priority when the prefix is present
        return opcode == 0x31 && !Decoder.HasOperandSizePrefix();
    }

    /// <summary>
    /// Decodes an XOR r/m32, r32 instruction
    /// </summary>
    /// <param name="opcode">The opcode of the instruction</param>
    /// <param name="instruction">The instruction object to populate</param>
    /// <returns>True if the instruction was successfully decoded</returns>
    public override bool Decode(byte opcode, Instruction instruction)
    {
        // Set the instruction type
        instruction.Type = InstructionType.Xor;

        if (!Decoder.CanReadByte())
        {
            return false;
        }

        // Read the ModR/M byte
        var (_, reg, _, destOperand) = ModRMDecoder.ReadModRM();

        // Create the source register operand
        var srcOperand = OperandFactory.CreateRegisterOperand(reg, 32);

        // Set the structured operands
        instruction.StructuredOperands =
        [
            destOperand,
            srcOperand
        ];

        return true;
    }
}
