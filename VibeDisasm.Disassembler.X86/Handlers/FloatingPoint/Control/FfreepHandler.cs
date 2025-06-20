using VibeDisasm.Disassembler.X86.Operands;

namespace VibeDisasm.Disassembler.X86.Handlers.FloatingPoint.Control;

/// <summary>
/// Handler for FFREEP ST(i) instruction (DF C0-C7)
/// </summary>
public class FfreepHandler : InstructionHandler
{
    /// <summary>
    /// Initializes a new instance of the FfreepHandler class
    /// </summary>
    /// <param name="decoder">The instruction decoder that owns this handler</param>
    public FfreepHandler(InstructionDecoder decoder)
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
        // FFREEP ST(i) is DF C0-C7
        if (opcode != 0xDF)
        {
            return false;
        }

        if (!Decoder.CanReadByte())
        {
            return false;
        }

        // Check if the ModR/M byte has reg field = 0 and mod = 3
        var modRm = Decoder.PeakByte();
        var reg = (byte)((modRm >> 3) & 0x7);
        var mod = (byte)((modRm >> 6) & 0x3);

        // Only handle register operands (mod = 3) with reg = 0
        return reg == 0 && mod == 3;
    }

    /// <summary>
    /// Decodes a FFREEP ST(i) instruction
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
        var (mod, reg, rm, _) = ModRMDecoder.ReadModRM();

        // Set the instruction type
        instruction.Type = InstructionType.Ffreep;

        // Map rm field to FPU register index
        var stIndex = rm switch
        {
            RegisterIndex.A => FpuRegisterIndex.ST0,
            RegisterIndex.C => FpuRegisterIndex.ST1,
            RegisterIndex.D => FpuRegisterIndex.ST2,
            RegisterIndex.B => FpuRegisterIndex.ST3,
            RegisterIndex.Sp => FpuRegisterIndex.ST4,
            RegisterIndex.Bp => FpuRegisterIndex.ST5,
            RegisterIndex.Si => FpuRegisterIndex.ST6,
            RegisterIndex.Di => FpuRegisterIndex.ST7,
            _ => FpuRegisterIndex.ST0 // Default case, should not happen
        };

        // Create the FPU register operand
        var fpuRegisterOperand = OperandFactory.CreateFPURegisterOperand(stIndex);

        // Set the structured operands
        instruction.StructuredOperands =
        [
            fpuRegisterOperand
        ];

        return true;
    }
}
