using VibeDisasm.Disassembler.X86.Operands;

namespace VibeDisasm.Disassembler.X86.Handlers.Bit;

/// <summary>
/// Handler for BTS r32, r/m32 instruction (0F AB)
/// </summary>
public class BtsR32Rm32Handler : InstructionHandler
{
    /// <summary>
    /// Initializes a new instance of the BtsR32Rm32Handler class
    /// </summary>
    /// <param name="decoder">The instruction decoder that owns this handler</param>
    public BtsR32Rm32Handler(InstructionDecoder decoder)
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
        // BTS r32, r/m32 is a two-byte opcode: 0F AB
        if (opcode != 0x0F)
        {
            return false;
        }

        // Check if we have enough bytes to read the second opcode byte
        if (!Decoder.CanReadByte())
        {
            return false;
        }

        // Check if the second byte is AB
        var secondByte = Decoder.PeakByte();

        // Only handle when the operand size prefix is NOT present
        // This ensures 16-bit handlers get priority when the prefix is present
        return secondByte == 0xAB && !Decoder.HasOperandSizePrefix();
    }

    /// <summary>
    /// Decodes a BTS r32, r/m32 instruction
    /// </summary>
    /// <param name="opcode">The opcode of the instruction</param>
    /// <param name="instruction">The instruction object to populate</param>
    /// <returns>True if the instruction was successfully decoded</returns>
    public override bool Decode(byte opcode, Instruction instruction)
    {
        // Set the instruction type
        instruction.Type = InstructionType.Bts;

        // Read the second opcode byte (AB)
        Decoder.ReadByte();

        // Check if we have enough bytes for the ModR/M byte
        if (!Decoder.CanReadByte())
        {
            return false;
        }

        // Read the ModR/M byte
        // For BTS r/m32, r32 (0F AB):
        // - The r/m field with mod specifies the destination operand (register or memory)
        // - The reg field specifies the bit index register
        var (_, reg, _, destinationOperand) = ModRMDecoder.ReadModRM();

        // Create the register operand for the reg field
        var bitIndexOperand = OperandFactory.CreateRegisterOperand(reg);

        // Set the structured operands
        instruction.StructuredOperands =
        [
            destinationOperand,
            bitIndexOperand
        ];

        return true;
    }
}
