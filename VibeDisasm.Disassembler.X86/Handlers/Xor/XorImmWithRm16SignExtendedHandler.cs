using VibeDisasm.Disassembler.X86.Operands;

namespace VibeDisasm.Disassembler.X86.Handlers.Xor;

/// <summary>
/// Handler for XOR r/m16, imm8 (sign-extended) instruction (0x83 /6 with 0x66 prefix)
/// </summary>
public class XorImmWithRm16SignExtendedHandler : InstructionHandler
{
    /// <summary>
    /// Initializes a new instance of the XorImmWithRm16SignExtendedHandler class
    /// </summary>
    /// <param name="decoder">The instruction decoder that owns this handler</param>
    public XorImmWithRm16SignExtendedHandler(InstructionDecoder decoder)
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
        if (opcode != 0x83 || !Decoder.HasOperandSizePrefix())
        {
            return false;
        }

        // Check if the reg field of the ModR/M byte is 6 (XOR)
        if (!Decoder.CanReadByte())
        {
            return false;
        }

        var reg = ModRMDecoder.PeakModRMReg();

        return reg == 6; // 6 = XOR
    }

    /// <summary>
    /// Decodes a XOR r/m16, imm8 (sign-extended) instruction
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
        // For XOR r/m16, imm8 (sign-extended) (0x83 /6 with 0x66 prefix):
        // - The r/m field with mod specifies the destination operand (register or memory)
        // - The immediate value is the source operand (sign-extended from 8 to 16 bits)
        var (_, _, _, destinationOperand) = ModRMDecoder.ReadModRM16();

        // Note: The operand size is already set to 16-bit by the ReadModRM16 method

        // Read the immediate value (sign-extended from 8 to 16 bits)
        if (!Decoder.CanReadByte())
        {
            return false;
        }

        // Read the immediate value as a signed byte and automatically sign-extend it to short
        short imm16 = (sbyte)Decoder.ReadByte();

        // Create the source immediate operand with the sign-extended value
        var sourceOperand = OperandFactory.CreateImmediateOperand((ushort)imm16, 16);

        // Set the structured operands
        instruction.StructuredOperands =
        [
            destinationOperand,
            sourceOperand
        ];

        return true;
    }
}
