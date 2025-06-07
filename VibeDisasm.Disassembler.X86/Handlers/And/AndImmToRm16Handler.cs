using VibeDisasm.Disassembler.X86.Operands;

namespace VibeDisasm.Disassembler.X86.Handlers.And;

/// <summary>
/// Handler for AND r/m16, imm16 instruction (0x81 /4 with 0x66 prefix)
/// </summary>
public class AndImmToRm16Handler : InstructionHandler
{
    /// <summary>
    /// Initializes a new instance of the AndImmToRm16Handler class
    /// </summary>
    /// <param name="decoder">The instruction decoder that owns this handler</param>
    public AndImmToRm16Handler(InstructionDecoder decoder)
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
        // AND r/m16, imm16 is encoded as 0x81 with 0x66 prefix
        if (opcode != 0x81)
        {
            return false;
        }

        // Only handle when the operand size prefix is present
        if (!Decoder.HasOperandSizePrefix())
        {
            return false;
        }

        // Check if we can read the ModR/M byte
        if (!Decoder.CanReadByte())
        {
            return false;
        }

        // Check if the reg field of the ModR/M byte is 4 (AND)
        var reg = ModRMDecoder.PeakModRMReg();
        return reg == 4; // 4 = AND
    }

    /// <summary>
    /// Decodes an AND r/m16, imm16 instruction
    /// </summary>
    /// <param name="opcode">The opcode of the instruction</param>
    /// <param name="instruction">The instruction object to populate</param>
    /// <returns>True if the instruction was successfully decoded</returns>
    public override bool Decode(byte opcode, Instruction instruction)
    {
        // Set the instruction type
        instruction.Type = InstructionType.And;

        // Read the ModR/M byte to get the destination operand
        var (_, reg, _, destinationOperand) = ModRMDecoder.ReadModRM16();

        // Check if we have enough bytes for the immediate value
        if (!Decoder.CanReadUShort())
        {
            return false;
        }

        // Read the immediate value
        var imm16 = Decoder.ReadUInt16();

        // Create the immediate operand
        var sourceOperand = OperandFactory.CreateImmediateOperand(imm16);

        // Set the structured operands
        instruction.StructuredOperands =
        [
            destinationOperand,
            sourceOperand
        ];

        return true;
    }
}
