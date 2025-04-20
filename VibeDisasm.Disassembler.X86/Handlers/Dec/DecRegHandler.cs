using VibeDisasm.Disassembler.X86.Operands;

namespace VibeDisasm.Disassembler.X86.Handlers.Dec;

/// <summary>
/// Handler for DEC r32 instructions (0x48-0x4F)
/// </summary>
public class DecRegHandler : InstructionHandler
{
    /// <summary>
    /// Initializes a new instance of the DecRegHandler class
    /// </summary>
    /// <param name="decoder">The instruction decoder that owns this handler</param>
    public DecRegHandler(InstructionDecoder decoder)
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
        // DEC EAX = 0x48, DEC ECX = 0x49, ..., DEC EDI = 0x4F
        // Only handle when the operand size prefix is NOT present
        // This ensures 16-bit handlers get priority when the prefix is present
        return opcode >= 0x48 && opcode <= 0x4F && !Decoder.HasOperandSizePrefix();
    }

    /// <summary>
    /// Decodes a DEC r32 instruction
    /// </summary>
    /// <param name="opcode">The opcode of the instruction</param>
    /// <param name="instruction">The instruction object to populate</param>
    /// <returns>True if the instruction was successfully decoded</returns>
    public override bool Decode(byte opcode, Instruction instruction)
    {
        // Calculate the register index (0 for EAX, 1 for ECX, etc.)
        RegisterIndex reg = (RegisterIndex)(opcode - 0x48);
        
        // Set the instruction type
        instruction.Type = InstructionType.Dec;
        
        // Create the register operand
        var regOperand = OperandFactory.CreateRegisterOperand(reg, 32);
        
        // Set the structured operands
        instruction.StructuredOperands = 
        [
            regOperand
        ];
        
        return true;
    }
}
