using VibeDisasm.Disassembler.X86.Operands;

namespace VibeDisasm.Disassembler.X86.Handlers.FloatingPoint.Arithmetic;

/// <summary>
/// Handler for FDIVR ST(i), ST instruction (DC F0-F7)
/// </summary>
public class FdivrStiStHandler : InstructionHandler
{
    /// <summary>
    /// Initializes a new instance of the FdivStiStHandler class
    /// </summary>
    /// <param name="decoder">The instruction decoder that owns this handler</param>
    public FdivrStiStHandler(InstructionDecoder decoder)
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
        // FDIVR ST(i), ST is DC F0-F7
        if (opcode != 0xDC) return false;

        if (!Decoder.CanReadByte())
        {
            return false;
        }

        // Check second opcode byte
        byte secondOpcode = Decoder.PeakByte();
        
        // Only handle F0-F7
        return secondOpcode is >= 0xF0 and <= 0xF7;
    }
    
    /// <summary>
    /// Decodes a FDIVR ST(i), ST instruction
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
        var stIndex = (FpuRegisterIndex)(Decoder.ReadByte() - 0xF0);
        
        // Set the instruction type
        instruction.Type = InstructionType.Fdivr;
        
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
