using VibeDisasm.Disassembler.X86.Operands;

namespace VibeDisasm.Disassembler.X86.Handlers.FloatingPoint.Arithmetic;

/// <summary>
/// Handler for FSUBR ST(i), ST instruction (DC E0-E7)
/// </summary>
public class FsubStiStHandler : InstructionHandler
{
    /// <summary>
    /// Initializes a new instance of the FsubStiStHandler class
    /// </summary>
    /// <param name="decoder">The instruction decoder that owns this handler</param>
    public FsubStiStHandler(InstructionDecoder decoder)
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
        // FSUBR ST(i), ST is DC E0-E7
        if (opcode != 0xDC)
        {
            return false;
        }

        if (!Decoder.CanReadByte())
        {
            return false;
        }

        // Check second opcode byte
        var secondOpcode = Decoder.PeakByte();

        // Only handle E0-E7
        return secondOpcode is >= 0xE0 and <= 0xE7;
    }

    /// <summary>
    /// Decodes a FSUBR ST(i), ST instruction
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

        // Read the ModR/M byte and calculate ST(i) index
        var stIndex = (FpuRegisterIndex)(Decoder.ReadByte() - 0xE0);

        // Set the instruction type
        instruction.Type = InstructionType.Fsubr;

        // Create the FPU register operands
        var stiOperand = OperandFactory.CreateFPURegisterOperand(stIndex);
        var st0Operand = OperandFactory.CreateFPURegisterOperand(FpuRegisterIndex.ST0);

        // Set the structured operands
        instruction.StructuredOperands =
        [
            stiOperand,
            st0Operand
        ];

        return true;
    }
}
