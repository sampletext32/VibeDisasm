using VibeDisasm.Disassembler.X86.Handlers;
using VibeDisasm.Disassembler.X86.Handlers.Inc;

namespace VibeDisasm.Disassembler.X86.Tests.InstructionTests;

/// <summary>
/// Tests for handler selection in the InstructionHandlerFactory
/// </summary>
public class HandlerSelectionTests
{
    /// <summary>
    /// Tests that the IncRegHandler is NOT selected for the 0x83 opcode
    /// </summary>
    [Fact]
    public void InstructionHandlerFactory_DoesNotSelectIncRegHandler_For0x83Opcode()
    {
        // Arrange
        var codeBuffer = new byte[] { 0x83, 0xC1, 0x04 }; // ADD ecx, 0x04
        var decoder = new InstructionDecoder(codeBuffer, codeBuffer.Length);
        var factory = new InstructionHandlerFactory(decoder);

        // Act
        var handler = factory.GetHandler(0x83);

        // Assert
        Assert.NotNull(handler);
        Assert.IsNotType<IncRegHandler>(handler);
    }
}
