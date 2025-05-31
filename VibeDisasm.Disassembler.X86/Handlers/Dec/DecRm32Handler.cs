namespace VibeDisasm.Disassembler.X86.Handlers.Dec;

/// <summary>
/// Handler for DEC r/m32 instruction (0xFF /1)
/// </summary>
public class DecRm32Handler : InstructionHandler
{
    /// <summary>
    /// Initializes a new instance of the DecRm32Handler class
    /// </summary>
    /// <param name="decoder">The instruction decoder that owns this handler</param>
    public DecRm32Handler(InstructionDecoder decoder)
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
        // DEC r/m32 is encoded as FF /1
        if (opcode != 0xFF)
        {
            return false;
        }

        // Check if we have enough bytes to read the ModR/M byte
        if (!Decoder.CanReadByte())
        {
            return false;
        }

        var reg = ModRMDecoder.PeakModRMReg();

        // DEC r/m32 is encoded as FF /1 (reg field = 1)
        // Only handle when the operand size prefix is NOT present
        // This ensures 16-bit handlers get priority when the prefix is present
        return reg == 1 && !Decoder.HasOperandSizePrefix();
    }

    /// <summary>
    /// Decodes a DEC r/m32 instruction
    /// </summary>
    /// <param name="opcode">The opcode of the instruction</param>
    /// <param name="instruction">The instruction object to populate</param>
    /// <returns>True if the instruction was successfully decoded</returns>
    public override bool Decode(byte opcode, Instruction instruction)
    {
        // Set the instruction type
        instruction.Type = InstructionType.Dec;

        // Check if we have enough bytes for the ModR/M byte
        if (!Decoder.CanReadByte())
        {
            return false;
        }

        // Read the ModR/M byte
        // For DEC r/m32 (FF /1):
        // - The r/m field with mod specifies the operand (register or memory)
        var (_, _, _, operand) = ModRMDecoder.ReadModRM();

        // Set the structured operands
        // DEC has only one operand
        instruction.StructuredOperands =
        [
            operand
        ];

        return true;
    }
}
