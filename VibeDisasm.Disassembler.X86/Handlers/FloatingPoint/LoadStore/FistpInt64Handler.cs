namespace VibeDisasm.Disassembler.X86.Handlers.FloatingPoint.LoadStore;

/// <summary>
/// Handler for FISTP int64 instruction (DF /7)
/// </summary>
public class FistpInt64Handler : InstructionHandler
{
    /// <summary>
    /// Initializes a new instance of the FistpInt64Handler class
    /// </summary>
    /// <param name="decoder">The instruction decoder that owns this handler</param>
    public FistpInt64Handler(InstructionDecoder decoder)
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
        // FISTP int64 is DF /7
        if (opcode != 0xDF)
        {
            return false;
        }

        if (!Decoder.CanReadByte())
        {
            return false;
        }

        // Check if the ModR/M byte has reg field = 7
        var modRm = Decoder.PeakByte();
        var reg = (byte)((modRm >> 3) & 0x7);
        var mod = (byte)((modRm >> 6) & 0x3);

        // Only handle memory operands (mod != 3) with reg = 7
        return reg == 7 && mod != 3;
    }

    /// <summary>
    /// Decodes a FISTP int64 instruction
    /// </summary>
    /// <param name="opcode">The opcode of the instruction</param>
    /// <param name="instruction">The instruction object to populate</param>
    /// <returns>True if the instruction was successfully decoded</returns>
    public override bool Decode(byte opcode, Instruction instruction)
    {
        if (!Decoder.CanReadByte())
        {
            return false;
        }

        // Read the ModR/M byte
        var (mod, reg, rm, operand) = ModRMDecoder.ReadModRM64();

        // Set the instruction type
        instruction.Type = InstructionType.Fistp;

        // Set the structured operands
        instruction.StructuredOperands =
        [
            operand
        ];

        return true;
    }
}
