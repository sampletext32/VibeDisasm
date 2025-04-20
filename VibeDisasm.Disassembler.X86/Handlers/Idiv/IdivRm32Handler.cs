namespace VibeDisasm.Disassembler.X86.Handlers.Idiv;

/// <summary>
/// Handler for IDIV r/m32 instruction (0xF7 /7)
/// </summary>
public class IdivRm32Handler : InstructionHandler
{
    /// <summary>
    /// Initializes a new instance of the IdivRm32Handler class
    /// </summary>
    /// <param name="decoder">The instruction decoder that owns this handler</param>
    public IdivRm32Handler(InstructionDecoder decoder)
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
        // IDIV r/m32 is encoded as 0xF7 with reg field 7
        if (opcode != 0xF7)
            return false;

        // Check if we can read the ModR/M byte
        if (!Decoder.CanReadByte())
            return false;

        // Check if the reg field of the ModR/M byte is 7 (IDIV)
        var reg = ModRMDecoder.PeakModRMReg();

        // reg = 7 means IDIV operation
        return reg == 7;
    }

    /// <summary>
    /// Decodes an IDIV r/m32 instruction
    /// </summary>
    /// <param name="opcode">The opcode of the instruction</param>
    /// <param name="instruction">The instruction object to populate</param>
    /// <returns>True if the instruction was successfully decoded</returns>
    public override bool Decode(byte opcode, Instruction instruction)
    {
        // Set the instruction type
        instruction.Type = InstructionType.IDiv;

        if (!Decoder.CanReadByte())
        {
            return false;
        }

        // Read the ModR/M byte
        // For IDIV r/m32 (0xF7 /7):
        // - The r/m field with mod specifies the operand (register or memory)
        var (_, _, _, operand) = ModRMDecoder.ReadModRM();

        // Set the structured operands
        // IDIV has only one operand
        instruction.StructuredOperands = 
        [
            operand
        ];

        return true;
    }
}
