using VibeDisasm.Disassembler.X86.Operands;

namespace VibeDisasm.Disassembler.X86.Handlers.String;

/// <summary>
/// Handler for string instructions (MOVS, CMPS, STOS, LODS, SCAS)
/// The REP/REPNE prefixes are handled by the InstructionDecoder class
/// </summary>
public class StringInstructionHandler : InstructionHandler
{
    // Dictionary mapping opcodes to their instruction types and operand factories for 32-bit mode (default)
    private static readonly Dictionary<byte, (InstructionType Type, Func<Operand[]> CreateOperands)> StringInstructions32 = new()
    {
        // MOVS instructions
        { 0xA4, (InstructionType.MovsB, () =>
        [
            OperandFactory.CreateBaseRegisterMemoryOperand8(RegisterIndex.Di, Segment.Es),
            OperandFactory.CreateBaseRegisterMemoryOperand8(RegisterIndex.Si, Segment.Ds)
        ]) },  // MOVSB
        { 0xA5, (InstructionType.MovsD, () =>
        [
            OperandFactory.CreateBaseRegisterMemoryOperand(RegisterIndex.Di, 32, Segment.Es),
            OperandFactory.CreateBaseRegisterMemoryOperand(RegisterIndex.Si, 32, Segment.Ds)
        ]) },  // MOVSD
        
        // CMPS instructions
        { 0xA6, (InstructionType.CmpsB, () =>
        [
            OperandFactory.CreateBaseRegisterMemoryOperand8(RegisterIndex.Si, Segment.Ds),
            OperandFactory.CreateBaseRegisterMemoryOperand8(RegisterIndex.Di, Segment.Es)
        ]) },  // CMPSB
        { 0xA7, (InstructionType.CmpsD, () =>
        [
            OperandFactory.CreateBaseRegisterMemoryOperand(RegisterIndex.Si, 32, Segment.Ds),
            OperandFactory.CreateBaseRegisterMemoryOperand(RegisterIndex.Di, 32, Segment.Es)
        ]) },  // CMPSD
        
        // STOS instructions
        { 0xAA, (InstructionType.StosB, () =>
        [
            OperandFactory.CreateBaseRegisterMemoryOperand8(RegisterIndex.Di, Segment.Es),
            OperandFactory.CreateRegisterOperand8(RegisterIndex8.AL)
        ]) },  // STOSB
        { 0xAB, (InstructionType.StosD, () =>
        [
            OperandFactory.CreateBaseRegisterMemoryOperand(RegisterIndex.Di, 32, Segment.Es),
            OperandFactory.CreateRegisterOperand(RegisterIndex.A, 32)
        ]) },  // STOSD
        
        // LODS instructions
        { 0xAC, (InstructionType.LodsB, () =>
        [
            OperandFactory.CreateRegisterOperand8(RegisterIndex8.AL),
            OperandFactory.CreateBaseRegisterMemoryOperand8(RegisterIndex.Si, Segment.Ds)
        ]) },  // LODSB
        { 0xAD, (InstructionType.LodsD, () =>
        [
            OperandFactory.CreateRegisterOperand(RegisterIndex.A, 32),
            OperandFactory.CreateBaseRegisterMemoryOperand(RegisterIndex.Si, 32, Segment.Ds)
        ]) },  // LODSD
        
        // SCAS instructions
        { 0xAE, (InstructionType.ScasB, () =>
        [
            OperandFactory.CreateRegisterOperand8(RegisterIndex8.AL),
            OperandFactory.CreateBaseRegisterMemoryOperand8(RegisterIndex.Di, Segment.Es)
        ]) },  // SCASB
        { 0xAF, (InstructionType.ScasD, () =>
        [
            OperandFactory.CreateRegisterOperand(RegisterIndex.A, 32),
            OperandFactory.CreateBaseRegisterMemoryOperand(RegisterIndex.Di, 32, Segment.Es)
        ]) }   // SCASD
    };

    // Dictionary mapping opcodes to their instruction types and operand factories for 16-bit mode (with operand size prefix)
    private static readonly Dictionary<byte, (InstructionType Type, Func<Operand[]> CreateOperands)> StringInstructions16 = new()
    {
        // MOVS instructions
        { 0xA4, (InstructionType.MovsB, () =>
        [
            OperandFactory.CreateBaseRegisterMemoryOperand8(RegisterIndex.Di, Segment.Es),
            OperandFactory.CreateBaseRegisterMemoryOperand8(RegisterIndex.Si, Segment.Ds)
        ]) },  // MOVSB (same for 16-bit)
        { 0xA5, (InstructionType.MovsW, () =>
        [
            OperandFactory.CreateBaseRegisterMemoryOperand(RegisterIndex.Di, 16, Segment.Es),
            OperandFactory.CreateBaseRegisterMemoryOperand(RegisterIndex.Si, 16, Segment.Ds)
        ]) },  // MOVSW
        
        // CMPS instructions
        { 0xA6, (InstructionType.CmpsB, () =>
        [
            OperandFactory.CreateBaseRegisterMemoryOperand8(RegisterIndex.Si, Segment.Ds),
            OperandFactory.CreateBaseRegisterMemoryOperand8(RegisterIndex.Di, Segment.Es)
        ]) },  // CMPSB (same for 16-bit)
        { 0xA7, (InstructionType.CmpsW, () =>
        [
            OperandFactory.CreateBaseRegisterMemoryOperand(RegisterIndex.Si, 16, Segment.Ds),
            OperandFactory.CreateBaseRegisterMemoryOperand(RegisterIndex.Di, 16, Segment.Es)
        ]) },  // CMPSW
        
        // STOS instructions
        { 0xAA, (InstructionType.StosB, () =>
        [
            OperandFactory.CreateBaseRegisterMemoryOperand8(RegisterIndex.Di, Segment.Es),
            OperandFactory.CreateRegisterOperand8(RegisterIndex8.AL)
        ]) },  // STOSB (same for 16-bit)
        { 0xAB, (InstructionType.StosW, () =>
        [
            OperandFactory.CreateBaseRegisterMemoryOperand(RegisterIndex.Di, 16, Segment.Es),
            OperandFactory.CreateRegisterOperand(RegisterIndex.A, 16)
        ]) },  // STOSW
        
        // LODS instructions
        { 0xAC, (InstructionType.LodsB, () =>
        [
            OperandFactory.CreateRegisterOperand8(RegisterIndex8.AL),
            OperandFactory.CreateBaseRegisterMemoryOperand8(RegisterIndex.Si, Segment.Ds)
        ]) },  // LODSB (same for 16-bit)
        { 0xAD, (InstructionType.LodsW, () =>
        [
            OperandFactory.CreateRegisterOperand(RegisterIndex.A, 16),
            OperandFactory.CreateBaseRegisterMemoryOperand(RegisterIndex.Si, 16, Segment.Ds)
        ]) },  // LODSW
        
        // SCAS instructions
        { 0xAE, (InstructionType.ScasB, () =>
        [
            OperandFactory.CreateRegisterOperand8(RegisterIndex8.AL),
            OperandFactory.CreateBaseRegisterMemoryOperand8(RegisterIndex.Di, Segment.Es)
        ]) },  // SCASB (same for 16-bit)
        { 0xAF, (InstructionType.ScasW, () =>
        [
            OperandFactory.CreateRegisterOperand(RegisterIndex.A, 16),
            OperandFactory.CreateBaseRegisterMemoryOperand(RegisterIndex.Di, 16, Segment.Es)
        ]) }   // SCASW
    };

    /// <summary>
    /// Initializes a new instance of the StringInstructionHandler class
    /// </summary>
    /// <param name="decoder">The instruction decoder that owns this handler</param>
    public StringInstructionHandler(InstructionDecoder decoder)
        : base(decoder)
    {
    }

    /// <summary>
    /// Checks if this handler can handle the given opcode
    /// </summary>
    /// <param name="opcode">The opcode to check</param>
    /// <returns>True if this handler can handle the opcode</returns>
    public override bool CanHandle(byte opcode)
    {
        // Check if the opcode is a string instruction in either 16-bit or 32-bit mode
        return StringInstructions32.ContainsKey(opcode);
    }

    /// <summary>
    /// Decodes a string instruction
    /// </summary>
    /// <param name="opcode">The opcode to decode</param>
    /// <param name="instruction">The instruction to populate</param>
    /// <returns>True if the instruction was successfully decoded</returns>
    public override bool Decode(byte opcode, Instruction instruction)
    {
        // Select the appropriate dictionary based on operand size prefix
        var instructionsDict = Decoder.HasOperandSizePrefix()
            ? StringInstructions16
            : StringInstructions32;

        // Get the instruction type and operands for the string instruction
        if (instructionsDict.TryGetValue(opcode, out var instructionInfo))
        {
            // Set the instruction type
            instruction.Type = instructionInfo.Type;

            // Create and set the structured operands
            instruction.StructuredOperands = instructionInfo.CreateOperands().ToList();
            return true;
        }

        // This shouldn't happen if CanHandle is called first
        return false;
    }
}
