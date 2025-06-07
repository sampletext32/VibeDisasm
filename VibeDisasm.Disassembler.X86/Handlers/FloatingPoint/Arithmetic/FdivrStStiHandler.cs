using VibeDisasm.Disassembler.X86.Operands;

namespace VibeDisasm.Disassembler.X86.Handlers.FloatingPoint.Arithmetic;

/// <summary>
/// Handler for FDIVR ST, ST(i) instruction (D8 F8-FF)
/// </summary>
public class FdivrStStiHandler : InstructionHandler
{
    /// <summary>
    /// Initializes a new instance of the FdivrStStHandler class
    /// </summary>
    /// <param name="decoder">The instruction decoder that owns this handler</param>
    public FdivrStStiHandler(InstructionDecoder decoder)
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
        // FDIVR ST, ST(i) is D8 F8-FF
        if (opcode != 0xD8)
        {
            return false;
        }

        if (!Decoder.CanReadByte())
        {
            return false;
        }

        // Check second opcode byte
        var secondOpcode = Decoder.PeakByte();

        // Only handle F8-FF
        return secondOpcode is >= 0xF8 and <= 0xFF;
    }

    /// <summary>
    /// Decodes a FDIVR ST, ST(i) instruction
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
        var stIndex = (FpuRegisterIndex)(Decoder.ReadByte() - 0xF8);

        // Set the instruction type
        instruction.Type = InstructionType.Fdivr;

        // Create the FPU register operands
        var st0Operand = OperandFactory.CreateFPURegisterOperand(FpuRegisterIndex.ST0);
        var stiOperand = OperandFactory.CreateFPURegisterOperand(stIndex);

        // Set the structured operands
        instruction.StructuredOperands =
        [
            st0Operand,
            stiOperand
        ];

        return true;
    }
}
