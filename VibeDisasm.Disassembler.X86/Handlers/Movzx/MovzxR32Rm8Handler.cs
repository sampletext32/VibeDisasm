using VibeDisasm.Disassembler.X86.Operands;

namespace VibeDisasm.Disassembler.X86.Handlers.Movzx;

/// <summary>
/// Handler for MOVZX r32, r/m8 instruction (0F B6)
/// </summary>
public class MovzxR32Rm8Handler : InstructionHandler
{
    /// <summary>
    /// Initializes a new instance of the MovzxR32Rm8Handler class
    /// </summary>
    /// <param name="decoder">The instruction decoder that owns this handler</param>
    public MovzxR32Rm8Handler(InstructionDecoder decoder)
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
        // MOVZX r32, r/m8 is a two-byte opcode: 0F B6
        if (opcode != 0x0F)
        {
            return false;
        }

        // Check if we have enough bytes to read the second opcode byte
        if (!Decoder.CanReadByte())
        {
            return false;
        }

        // Check if the second byte is B6
        var secondByte = Decoder.PeakByte();

        // Only handle when the operand size prefix is NOT present
        // This ensures 16-bit handlers get priority when the prefix is present
        return secondByte == 0xB6 && !Decoder.HasOperandSizePrefix();
    }

    /// <summary>
    /// Decodes a MOVZX r32, r/m8 instruction
    /// </summary>
    /// <param name="opcode">The opcode of the instruction</param>
    /// <param name="instruction">The instruction object to populate</param>
    /// <returns>True if the instruction was successfully decoded</returns>
    public override bool Decode(byte opcode, Instruction instruction)
    {
        // Set the instruction type
        instruction.Type = InstructionType.Movzx;

        // Read the second opcode byte (B6)
        Decoder.ReadByte();

        // Check if we have enough bytes for the ModR/M byte
        if (!Decoder.CanReadByte())
        {
            return false;
        }

        // For MOVZX r32, r/m8 (0F B6):
        // - The reg field specifies the destination register (32-bit)
        // - The r/m field with mod specifies the source operand (8-bit register or memory)
        var (_, reg8, _, sourceOperand8) = ModRMDecoder.ReadModRM8();

        // Create the register operand for the reg field (32-bit)
        var destinationOperand = OperandFactory.CreateRegisterOperand((RegisterIndex)reg8);

        // Set the structured operands
        instruction.StructuredOperands =
        [
            destinationOperand,
            sourceOperand8
        ];

        return true;
    }
}
