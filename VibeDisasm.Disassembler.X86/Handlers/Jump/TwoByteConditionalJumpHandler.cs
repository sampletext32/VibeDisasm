using VibeDisasm.Disassembler.X86.Operands;

namespace VibeDisasm.Disassembler.X86.Handlers.Jump;

/// <summary>
/// Handler for two-byte conditional jump instructions (0x0F 0x80-0x8F)
/// </summary>
public class TwoByteConditionalJumpHandler : InstructionHandler
{
    // Instruction types for conditional jumps
    private static readonly InstructionType[] InstructionTypes =
    [
        InstructionType.Jo, InstructionType.Jno, InstructionType.Jb, InstructionType.Jae, 
        InstructionType.Jz, InstructionType.Jnz, InstructionType.Jna, InstructionType.Jnbe,
        InstructionType.Js, InstructionType.Jns, InstructionType.Jp, InstructionType.Jpo, 
        InstructionType.Jnge, InstructionType.Jnl, InstructionType.Jng, InstructionType.Jnle
    ];

    /// <summary>
    /// Initializes a new instance of the TwoByteConditionalJumpHandler class
    /// </summary>
    /// <param name="decoder">The instruction decoder that owns this handler</param>
    public TwoByteConditionalJumpHandler(InstructionDecoder decoder) 
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
        // Two-byte conditional jumps start with 0x0F
        if (opcode != 0x0F)
        {
            return false;
        }

        if (!Decoder.CanReadByte())
        {
            return false;
        }
            
        byte secondByte = Decoder.PeakByte();
        // Second byte must be in the range 0x80-0x8F
        return secondByte >= 0x80 && secondByte <= 0x8F;
    }
    
    /// <summary>
    /// Decodes a two-byte conditional jump instruction
    /// </summary>
    /// <param name="opcode">The opcode of the instruction</param>
    /// <param name="instruction">The instruction object to populate</param>
    /// <returns>True if the instruction was successfully decoded</returns>
    public override bool Decode(byte opcode, Instruction instruction)
    {
        // Check if we have enough bytes for the second byte
        if (!Decoder.CanReadByte())
        {   
            return false;
        }
        
        // Read the second byte of the opcode
        byte secondByte = Decoder.ReadByte();
        
        // Get the instruction type from the table
        int index = secondByte - 0x80;
        instruction.Type = InstructionTypes[index];
        
        // Check if we have enough bytes for the offset
        if (!Decoder.CanReadUInt())
        {
            return false;
        }
        
        // Read the relative offset (32-bit)
        uint offset = Decoder.ReadUInt32();
        
        // Calculate the target address
        // For two-byte conditional jumps, the instruction is 6 bytes: first opcode (1) + second opcode (1) + offset (4)
        uint targetAddress = (uint)(instruction.Address + 6 + offset);
        
        // Create the relative offset operand
        var targetOperand = OperandFactory.CreateRelativeOffsetOperand(targetAddress);
        
        // Set the structured operands
        instruction.StructuredOperands = 
        [
            targetOperand
        ];
        
        return true;
    }
}
