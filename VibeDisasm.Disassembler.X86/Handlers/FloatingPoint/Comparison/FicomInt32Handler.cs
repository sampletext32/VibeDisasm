namespace VibeDisasm.Disassembler.X86.Handlers.FloatingPoint.Comparison;

/// <summary>
/// Handler for FICOM int32 instruction (DA /2)
/// </summary>
public class FicomInt32Handler : InstructionHandler
{
    /// <summary>
    /// Initializes a new instance of the FicomInt32Handler class
    /// </summary>
    /// <param name="decoder">The instruction decoder that owns this handler</param>
    public FicomInt32Handler(InstructionDecoder decoder)
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
        // FICOM is DA /2
        if (opcode != 0xDA)
        {
            return false;
        }

        if (!Decoder.CanReadByte())
        {
            return false;
        }

        // Check if the ModR/M byte has reg field = 2
        var modRm = Decoder.PeakByte();
        var reg = (byte)((modRm >> 3) & 0x7);
        var mod = (byte)((modRm >> 6) & 0x3);

        // Only handle memory operands (mod != 3)
        return reg == 2 && mod != 3;
    }

    /// <summary>
    /// Decodes a FICOM int32 instruction
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
        var (mod, reg, rm, rawOperand) = ModRMDecoder.ReadModRM();

        // Set the instruction type
        instruction.Type = InstructionType.Ficom;

        // Set the structured operands - the operand already has the correct size from ReadModRM
        instruction.StructuredOperands =
        [
            rawOperand
        ];

        return true;
    }
}
