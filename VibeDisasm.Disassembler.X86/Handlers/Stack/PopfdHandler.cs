namespace VibeDisasm.Disassembler.X86.Handlers.Stack;

/// <summary>
/// Handler for POPFD instruction (0x9D)
/// Pops the value from the stack and loads it into the EFLAGS register
/// </summary>
public class PopfdHandler : InstructionHandler
{
    /// <summary>
    /// Initializes a new instance of the PopfdHandler class
    /// </summary>
    /// <param name="decoder">The instruction decoder that owns this handler</param>
    public PopfdHandler(InstructionDecoder decoder)
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
        // POPFD is 0x9D
        return opcode == 0x9D;
    }

    /// <summary>
    /// Decodes a POPFD instruction
    /// </summary>
    /// <param name="opcode">The opcode of the instruction</param>
    /// <param name="instruction">The instruction object to populate</param>
    /// <returns>True if the instruction was successfully decoded</returns>
    public override bool Decode(byte opcode, Instruction instruction)
    {
        // Set the instruction type
        instruction.Type = InstructionType.Popfd;

        // POPFD has no operands
        instruction.StructuredOperands = [];

        return true;
    }
}
