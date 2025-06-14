namespace VibeDisasm.Disassembler.X86.Handlers.Flags;

/// <summary>
/// Handler for STC (Set Carry Flag) instruction (opcode F9)
/// </summary>
public class StcHandler : InstructionHandler
{
    /// <summary>
    /// Initializes a new instance of the StcHandler class
    /// </summary>
    /// <param name="decoder">The instruction decoder that owns this handler</param>
    public StcHandler(InstructionDecoder decoder)
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
        // STC is F9
        return opcode == 0xF9;
    }

    /// <summary>
    /// Decodes a STC instruction
    /// </summary>
    /// <param name="opcode">The opcode of the instruction</param>
    /// <param name="instruction">The instruction object to populate</param>
    /// <returns>True if the instruction was successfully decoded</returns>
    public override bool Decode(byte opcode, Instruction instruction)
    {
        // Set the instruction type
        instruction.Type = InstructionType.Stc;

        // STC has no operands
        instruction.StructuredOperands = [];

        return true;
    }
}
