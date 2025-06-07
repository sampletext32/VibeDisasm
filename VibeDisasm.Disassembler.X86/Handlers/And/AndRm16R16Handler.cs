using VibeDisasm.Disassembler.X86.Operands;

namespace VibeDisasm.Disassembler.X86.Handlers.And;

/// <summary>
/// Handler for AND r/m16, r16 instruction (0x21 with 0x66 prefix)
/// </summary>
public class AndRm16R16Handler : InstructionHandler
{
    /// <summary>
    /// Initializes a new instance of the AndRm16R16Handler class
    /// </summary>
    /// <param name="decoder">The instruction decoder that owns this handler</param>
    public AndRm16R16Handler(InstructionDecoder decoder)
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
        // AND r/m16, r16 is encoded as 0x21 with 0x66 prefix
        if (opcode != 0x21)
        {
            return false;
        }

        // Only handle when the operand size prefix is present
        return Decoder.HasOperandSizePrefix();
    }

    /// <summary>
    /// Decodes an AND r/m16, r16 instruction
    /// </summary>
    /// <param name="opcode">The opcode of the instruction</param>
    /// <param name="instruction">The instruction object to populate</param>
    /// <returns>True if the instruction was successfully decoded</returns>
    public override bool Decode(byte opcode, Instruction instruction)
    {
        // Set the instruction type
        instruction.Type = InstructionType.And;

        // Check if we can read the ModR/M byte
        if (!Decoder.CanReadByte())
        {
            return false;
        }

        // For AND r/m16, r16 (0x21 with 0x66 prefix):
        // - The reg field of the ModR/M byte specifies the source register
        // - The r/m field with mod specifies the destination operand (register or memory)
        var (_, reg, _, destinationOperand) = ModRMDecoder.ReadModRM16();

        // Create the source register operand with 16-bit size
        var sourceOperand = OperandFactory.CreateRegisterOperand(reg, 16);

        // Set the structured operands
        instruction.StructuredOperands =
        [
            destinationOperand,
            sourceOperand
        ];

        return true;
    }
}
