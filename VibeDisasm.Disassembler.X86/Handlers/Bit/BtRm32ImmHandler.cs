using VibeDisasm.Disassembler.X86.Operands;

namespace VibeDisasm.Disassembler.X86.Handlers.Bit;

/// <summary>
/// Handler for BT r/m32, imm8 instruction (0F BA /4)
/// </summary>
public class BtRm32ImmHandler : InstructionHandler
{
    /// <summary>
    /// Initializes a new instance of the BtRm32ImmHandler class
    /// </summary>
    /// <param name="decoder">The instruction decoder that owns this handler</param>
    public BtRm32ImmHandler(InstructionDecoder decoder)
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
        // BT r/m32, imm8 is encoded as 0F BA /4
        if (opcode != 0x0F)
        {
            return false;
        }

        // Check if we have enough bytes to read the second opcode byte
        if (!Decoder.CanRead(2))
        {
            return false;
        }

        var (secondByte, modRm) = Decoder.PeakTwoBytes();

        // Check if the second byte is BA
        if (secondByte != 0xBA)
        {
            return false;
        }

        // Check if the reg field of the ModR/M byte is 4 (BT)
        var reg = ModRMDecoder.GetRegFromModRM(modRm);

        // Only handle when the operand size prefix is NOT present
        // This ensures 16-bit handlers get priority when the prefix is present
        return reg == 4 && !Decoder.HasOperandSizePrefix();
    }

    /// <summary>
    /// Decodes a BT r/m32, imm8 instruction
    /// </summary>
    /// <param name="opcode">The opcode of the instruction</param>
    /// <param name="instruction">The instruction object to populate</param>
    /// <returns>True if the instruction was successfully decoded</returns>
    public override bool Decode(byte opcode, Instruction instruction)
    {
        // Set the instruction type
        instruction.Type = InstructionType.Bt;

        // Read the second opcode byte (BA)
        Decoder.ReadByte();

        // Check if we have enough bytes for the ModR/M byte
        if (!Decoder.CanReadByte())
        {
            return false;
        }

        // Read the ModR/M byte
        // For BT r/m32, imm8 (0F BA /4):
        // - The r/m field with mod specifies the destination operand (register or memory)
        // - The immediate value specifies the bit index
        var (_, _, _, destinationOperand) = ModRMDecoder.ReadModRM();

        // Check if we have enough bytes for the immediate value
        if (!Decoder.CanReadByte())
        {
            return false;
        }

        // Read the immediate byte for the bit position
        var imm8 = Decoder.ReadByte();

        // Create the immediate operand
        var bitIndexOperand = OperandFactory.CreateImmediateOperand(imm8, 8);

        // Set the structured operands
        instruction.StructuredOperands =
        [
            destinationOperand,
            bitIndexOperand
        ];

        return true;
    }
}
