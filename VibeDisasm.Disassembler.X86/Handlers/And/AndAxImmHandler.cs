using VibeDisasm.Disassembler.X86.Operands;

namespace VibeDisasm.Disassembler.X86.Handlers.And;

/// <summary>
/// Handler for AND AX, imm16 instruction (0x25 with 0x66 prefix)
/// </summary>
public class AndAxImmHandler : InstructionHandler
{
    /// <summary>
    /// Initializes a new instance of the AndAxImmHandler class
    /// </summary>
    /// <param name="decoder">The instruction decoder that owns this handler</param>
    public AndAxImmHandler(InstructionDecoder decoder)
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
        // AND AX, imm16 is encoded as 0x25 with 0x66 prefix
        if (opcode != 0x25)
        {
            return false;
        }

        // Only handle when the operand size prefix is present
        return Decoder.HasOperandSizePrefix();
    }

    /// <summary>
    /// Decodes an AND AX, imm16 instruction
    /// </summary>
    /// <param name="opcode">The opcode of the instruction</param>
    /// <param name="instruction">The instruction object to populate</param>
    /// <returns>True if the instruction was successfully decoded</returns>
    public override bool Decode(byte opcode, Instruction instruction)
    {
        // Set the instruction type
        instruction.Type = InstructionType.And;

        // Check if we have enough bytes for the immediate value
        if (!Decoder.CanReadUShort())
        {
            return false;
        }

        // Read the immediate value
        var imm16 = Decoder.ReadUInt16();

        // Create the AX register operand
        var axOperand = OperandFactory.CreateRegisterOperand(RegisterIndex.A, 16);

        // Create the immediate operand
        var immOperand = OperandFactory.CreateImmediateOperand(imm16);

        // Set the structured operands
        instruction.StructuredOperands =
        [
            axOperand,
            immOperand
        ];

        return true;
    }
}
