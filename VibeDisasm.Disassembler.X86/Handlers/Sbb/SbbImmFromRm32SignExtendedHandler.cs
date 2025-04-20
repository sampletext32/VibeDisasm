using VibeDisasm.Disassembler.X86.Operands;

namespace VibeDisasm.Disassembler.X86.Handlers.Sbb;

/// <summary>
/// Handler for SBB r/m32, imm8 (sign-extended) instruction (0x83 /3)
/// </summary>
public class SbbImmFromRm32SignExtendedHandler : InstructionHandler
{
    /// <summary>
    /// Initializes a new instance of the SbbImmFromRm32SignExtendedHandler class
    /// </summary>
    /// <param name="decoder">The instruction decoder that owns this handler</param>
    public SbbImmFromRm32SignExtendedHandler(InstructionDecoder decoder)
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
        if (opcode != 0x83)
            return false;

        // Check if the reg field of the ModR/M byte is 3 (SBB)
        if (!Decoder.CanReadByte())
            return false;

        var reg = ModRMDecoder.PeakModRMReg();

        return reg == 3; // 3 = SBB
    }

    /// <summary>
    /// Decodes a SBB r/m32, imm8 (sign-extended) instruction
    /// </summary>
    /// <param name="opcode">The opcode of the instruction</param>
    /// <param name="instruction">The instruction object to populate</param>
    /// <returns>True if the instruction was successfully decoded</returns>
    public override bool Decode(byte opcode, Instruction instruction)
    {
        // Set the instruction type
        instruction.Type = InstructionType.Sbb;

        if (!Decoder.CanReadByte())
        {
            return false;
        }

        // Read the ModR/M byte
        var (_, _, _, destOperand) = ModRMDecoder.ReadModRM();

        // Read the immediate value (sign-extended from 8 to 32 bits)
        if (!Decoder.CanReadByte())
        {
            return false;
        }

        // Sign-extend to 32 bits
        sbyte imm8 = (sbyte) Decoder.ReadByte();
        
        // Create the immediate operand with sign extension
        var immOperand = OperandFactory.CreateImmediateOperand((uint)imm8);
        
        // Set the structured operands
        instruction.StructuredOperands = 
        [
            destOperand,
            immOperand
        ];

        return true;
    }
}