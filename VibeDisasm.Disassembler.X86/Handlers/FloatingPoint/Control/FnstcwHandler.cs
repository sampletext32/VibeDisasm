using VibeDisasm.Disassembler.X86.Operands;

namespace VibeDisasm.Disassembler.X86.Handlers.FloatingPoint.Control;

/// <summary>
/// Handler for FNSTCW instruction (D9 /7)
/// </summary>
public class FnstcwHandler : InstructionHandler
{
    /// <summary>
    /// Initializes a new instance of the FnstcwHandler class
    /// </summary>
    /// <param name="decoder">The instruction decoder that owns this handler</param>
    public FnstcwHandler(InstructionDecoder decoder)
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
        // FNSTCW is D9 /7
        if (opcode != 0xD9)
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

        // Only handle memory operands (mod != 3)
        return reg == 7 && mod != 3;
    }

    /// <summary>
    /// Decodes a FNSTCW instruction
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
        instruction.Type = InstructionType.Fnstcw;

        // Create a 16-bit memory operand for control word operations
        Operand memoryOperand;

        if (rawOperand is DirectMemoryOperand directMemory)
        {
            memoryOperand = OperandFactory.CreateDirectMemoryOperand16(directMemory.Address);
        }
        else if (rawOperand is BaseRegisterMemoryOperand baseRegMemory)
        {
            memoryOperand = OperandFactory.CreateBaseRegisterMemoryOperand16(baseRegMemory.BaseRegister);
        }
        else if (rawOperand is DisplacementMemoryOperand dispMemory)
        {
            memoryOperand = OperandFactory.CreateDisplacementMemoryOperand16(dispMemory.BaseRegister, dispMemory.Displacement);
        }
        else if (rawOperand is ScaledIndexMemoryOperand scaledMemory)
        {
            memoryOperand = OperandFactory.CreateScaledIndexMemoryOperand16(scaledMemory.IndexRegister, scaledMemory.Scale, scaledMemory.BaseRegister, scaledMemory.Displacement);
        }
        else
        {
            memoryOperand = rawOperand;
        }

        // Set the structured operands
        instruction.StructuredOperands =
        [
            memoryOperand
        ];

        return true;
    }
}
