using VibeDisasm.Disassembler.X86.Operands;

namespace VibeDisasm.Disassembler.X86.Handlers.Add;

/// <summary>
/// Handler for ADD r/m8, imm8 instruction (0x80 /0)
/// </summary>
public class AddImmToRm8Handler : InstructionHandler
{
    /// <summary>
    /// Initializes a new instance of the AddImmToRm8Handler class
    /// </summary>
    /// <param name="decoder">The instruction decoder that owns this handler</param>
    public AddImmToRm8Handler(InstructionDecoder decoder)
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
        if (opcode != 0x80)
        {
            return false;
        }

        if (!Decoder.CanReadByte())
        {
            return false;
        }

        var reg = ModRMDecoder.PeakModRMReg();

        return reg == 0; // 0 = ADD
    }

    /// <summary>
    /// Decodes an ADD r/m8, imm8 instruction
    /// </summary>
    /// <param name="opcode">The opcode of the instruction</param>
    /// <param name="instruction">The instruction object to populate</param>
    /// <returns>True if the instruction was successfully decoded</returns>
    public override bool Decode(byte opcode, Instruction instruction)
    {
        // Set the instruction type and mnemonic
        instruction.Type = InstructionType.Add;

        if (!Decoder.CanReadByte())
        {
            return false;
        }

        // Read the ModR/M byte, specifying that we're dealing with 8-bit operands
        var (_, _, _, destOperand) = ModRMDecoder.ReadModRM8();

        // Note: The operand size is already set to 8-bit by the ReadModRM8 method

        // Read the immediate value
        if (!Decoder.CanReadByte())
        {
            return false;
        }

        var imm8 = Decoder.ReadByte();

        // Create the immediate operand
        var sourceOperand = OperandFactory.CreateImmediateOperand(imm8, 8);

        // Set the structured operands
        instruction.StructuredOperands =
        [
            destOperand,
            sourceOperand
        ];

        return true;
    }
}
