namespace VibeDisasm.Disassembler.X86.Handlers.FloatingPoint.Arithmetic;

/// <summary>
/// Handler for FMUL float64 instruction (DC /1)
/// </summary>
public class FmulFloat64Handler : InstructionHandler
{
    /// <summary>
    /// Initializes a new instance of the FmulFloat64Handler class
    /// </summary>
    /// <param name="decoder">The instruction decoder that owns this handler</param>
    public FmulFloat64Handler(InstructionDecoder decoder)
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
        // FMUL is DC /1
        if (opcode != 0xDC)
        {
            return false;
        }

        if (!Decoder.CanReadByte())
        {
            return false;
        }

        // Check if the ModR/M byte has reg field = 1 and mod != 3 (memory operand)
        var modRm = Decoder.PeakByte();
        var reg = (byte)((modRm >> 3) & 0x7);
        var mod = (byte)((modRm >> 6) & 0x3);

        // Only handle memory operands (mod != 3) with reg = 1
        return reg == 1 && mod != 3;
    }

    /// <summary>
    /// Decodes a FMUL float64 instruction
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

        // Read the ModR/M byte using the specialized FPU method for 64-bit operands
        var (mod, reg, fpuRm, rawOperand) = ModRMDecoder.ReadModRMFpu64();

        // We've already verified reg field is 1 (FMUL) in CanHandle
        // and we only handle memory operands (mod != 3)

        // Set the instruction type
        instruction.Type = InstructionType.Fmul;

        // Set the structured operands - the operand already has the correct size from ReadModRMFpu64
        instruction.StructuredOperands =
        [
            rawOperand
        ];

        return true;
    }
}
