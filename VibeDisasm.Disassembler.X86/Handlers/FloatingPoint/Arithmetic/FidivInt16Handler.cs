namespace VibeDisasm.Disassembler.X86.Handlers.FloatingPoint.Arithmetic;

/// <summary>
/// Handler for FIDIV int16 instruction (DE /6)
/// </summary>
public class FidivInt16Handler : InstructionHandler
{
    /// <summary>
    /// Initializes a new instance of the FidivInt16Handler class
    /// </summary>
    /// <param name="decoder">The instruction decoder that owns this handler</param>
    public FidivInt16Handler(InstructionDecoder decoder)
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
        // FIDIV int16 is DE /6
        if (opcode != 0xDE)
        {
            return false;
        }

        if (!Decoder.CanReadByte())
        {
            return false;
        }

        // Check if the ModR/M byte has reg field = 6
        var modRm = Decoder.PeakByte();
        var reg = (byte)((modRm >> 3) & 0x7);
        var mod = (byte)((modRm >> 6) & 0x3);

        // Only handle memory operands (mod != 3) with reg = 6
        return reg == 6 && mod != 3;
    }

    /// <summary>
    /// Decodes a FIDIV int16 instruction
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

        // Read the ModR/M byte, specifying that we're dealing with 16-bit operands
        var (mod, reg, rm, memoryOperand) = ModRMDecoder.ReadModRM16();

        // Set the instruction type
        instruction.Type = InstructionType.Fidiv;

        // Set the structured operands
        instruction.StructuredOperands =
        [
            memoryOperand
        ];

        return true;
    }
}
