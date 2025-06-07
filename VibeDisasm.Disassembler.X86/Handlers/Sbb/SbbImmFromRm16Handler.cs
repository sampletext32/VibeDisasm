using VibeDisasm.Disassembler.X86.Operands;

namespace VibeDisasm.Disassembler.X86.Handlers.Sbb;

/// <summary>
/// Handler for SBB r/m16, imm16 instruction (0x81 /3 with 0x66 prefix)
/// </summary>
public class SbbImmFromRm16Handler : InstructionHandler
{
    /// <summary>
    /// Initializes a new instance of the SbbImmFromRm16Handler class
    /// </summary>
    /// <param name="decoder">The instruction decoder that owns this handler</param>
    public SbbImmFromRm16Handler(InstructionDecoder decoder)
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
        if (opcode != 0x81)
        {
            return false;
        }

        // Must have operand size prefix for 16-bit operation
        if (!Decoder.HasOperandSizePrefix())
        {
            return false;
        }

        // Check if the reg field of the ModR/M byte is 3 (SBB)
        if (!Decoder.CanReadByte())
        {
            return false;
        }

        var reg = ModRMDecoder.PeakModRMReg();

        return reg == 3; // 3 = SBB
    }

    /// <summary>
    /// Decodes a SBB r/m16, imm16 instruction
    /// </summary>
    /// <param name="opcode">The opcode of the instruction</param>
    /// <param name="instruction">The instruction object to populate</param>
    /// <returns>True if the instruction was successfully decoded</returns>
    public override bool Decode(byte opcode, Instruction instruction)
    {
        // Set the instruction type
        instruction.Type = InstructionType.Sbb;

        if (!Decoder.CanReadByte())
        {
            return false;
        }

        // Read the ModR/M byte
        // For SBB r/m16, imm16 (0x81 /3 with 0x66 prefix):
        // - The r/m field with mod specifies the destination operand (register or memory)
        // - The immediate value is the source operand
        var (_, _, _, destinationOperand) = ModRMDecoder.ReadModRM16();

        // Check if we have enough bytes for the immediate value
        if (!Decoder.CanReadUShort())
        {
            return false;
        }

        // Read the immediate value
        var imm16 = Decoder.ReadUInt16();

        // Create the immediate operand
        var sourceOperand = OperandFactory.CreateImmediateOperand(imm16, 16);

        // Set the structured operands
        instruction.StructuredOperands =
        [
            destinationOperand,
            sourceOperand
        ];

        return true;
    }
}
