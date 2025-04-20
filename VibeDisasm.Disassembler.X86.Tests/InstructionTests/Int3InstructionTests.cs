namespace VibeDisasm.Disassembler.X86.Tests.InstructionTests;

/// <summary>
/// Tests for INT3 instruction handler
/// </summary>
public class Int3InstructionTests
{
    /// <summary>
    /// Tests the Int3Handler for decoding INT3 instruction
    /// </summary>
    [Fact]
    public void Int3Handler_DecodesInt3_Correctly()
    {
        // Arrange
        // INT3 (CC)
        byte[] codeBuffer = new byte[] { 0xCC };
        var decoder = new InstructionDecoder(codeBuffer, codeBuffer.Length);
        
        // Act
        var instruction = decoder.DecodeInstruction();
        
        // Assert
        Assert.NotNull(instruction);
        Assert.Equal(InstructionType.Int3, instruction.Type);
        
        // Check that we have no operands
        Assert.Empty(instruction.StructuredOperands);
    }
}
