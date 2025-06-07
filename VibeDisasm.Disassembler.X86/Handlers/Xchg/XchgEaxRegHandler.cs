using VibeDisasm.Disassembler.X86.Operands;

namespace VibeDisasm.Disassembler.X86.Handlers.Xchg;

/// <summary>
/// Handler for XCHG EAX, r32 instruction (0x90-0x97)
/// </summary>
public class XchgEaxRegHandler : InstructionHandler
{
    /// <summary>
    /// Initializes a new instance of the XchgEaxRegHandler class
    /// </summary>
    /// <param name="decoder">The instruction decoder that owns this handler</param>
    public XchgEaxRegHandler(InstructionDecoder decoder)
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
        // Only handle opcodes 0x91-0x97 when the operand size prefix is NOT present
        // This ensures 16-bit handlers get priority when the prefix is present
        return opcode >= 0x91 && opcode <= 0x97 && !Decoder.HasOperandSizePrefix();
    }

    /// <summary>
    /// Decodes an XCHG EAX, r32 instruction
    /// </summary>
    /// <param name="opcode">The opcode of the instruction</param>
    /// <param name="instruction">The instruction object to populate</param>
    /// <returns>True if the instruction was successfully decoded</returns>
    public override bool Decode(byte opcode, Instruction instruction)
    {
        // Set the instruction type
        instruction.Type = InstructionType.Xchg;

        // Register is encoded in the low 3 bits of the opcode
        var reg = (RegisterIndex)(opcode & 0x07);

        // Create the register operands
        var eaxOperand = OperandFactory.CreateRegisterOperand(RegisterIndex.A);
        var regOperand = OperandFactory.CreateRegisterOperand(reg);

        // Set the structured operands
        instruction.StructuredOperands =
        [
            eaxOperand,
            regOperand
        ];

        return true;
    }
}
