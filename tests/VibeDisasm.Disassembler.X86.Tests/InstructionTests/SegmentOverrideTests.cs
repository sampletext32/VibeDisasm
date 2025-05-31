using VibeDisasm.Disassembler.X86.Operands;
using Xunit.Abstractions;

namespace VibeDisasm.Disassembler.X86.Tests.InstructionTests;

/// <summary>
/// Tests for segment override prefixes
/// </summary>
public class SegmentOverrideTests
{
    private readonly ITestOutputHelper _testOutputHelper;

    public SegmentOverrideTests(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
    }

    /// <summary>
    /// Tests that the CS segment override prefix (0x2E) is correctly recognized
    /// </summary>
    [Fact]
    public void CsSegmentOverride_IsRecognized()
    {
        // Arrange
        // CS segment override prefix (0x2E) followed by MOV EAX, [0] (8B 05 00 00 00 00)
        var codeBuffer = new byte[] { 0x2E, 0x8B, 0x05, 0x00, 0x00, 0x00, 0x00 };
        var disassembler = new Disassembler(codeBuffer, 0);

        // Act
        var instructions = disassembler.Disassemble();

        // Assert
        Assert.Single(instructions);
        var instruction = instructions[0];
        Assert.NotNull(instruction);
        Assert.Equal(InstructionType.Mov, instruction.Type);

        // Check that we have two operands
        Assert.Equal(2, instruction.StructuredOperands.Count);

        // Check the first operand (EAX)
        var eaxOperand = instruction.StructuredOperands[0];
        Assert.IsType<RegisterOperand>(eaxOperand);
        var registerOperand = (RegisterOperand)eaxOperand;
        Assert.Equal(RegisterIndex.A, registerOperand.Register);
        Assert.Equal(32, registerOperand.Size); // Validate that it's a 32-bit register (EAX)

        // Check the second operand (memory operand with CS segment override)
        var memOperand = instruction.StructuredOperands[1];
        Assert.IsType<DirectMemoryOperand>(memOperand);
        var memoryOperand = (DirectMemoryOperand)memOperand;
        Assert.Equal(32, memoryOperand.Size); // Validate that it's a 32-bit memory reference
        Assert.Equal(Segment.Cs, memoryOperand.SegmentOverride);
    }

    /// <summary>
    /// Tests that the DS segment override prefix (0x3E) is correctly recognized
    /// </summary>
    [Fact]
    public void DsSegmentOverride_IsRecognized()
    {
        // Arrange
        // DS segment override prefix (0x3E) followed by MOV EAX, [0] (8B 05 00 00 00 00)
        var codeBuffer = new byte[] { 0x3E, 0x8B, 0x05, 0x00, 0x00, 0x00, 0x00 };
        var disassembler = new Disassembler(codeBuffer, 0);

        // Act
        var instructions = disassembler.Disassemble();

        // Assert
        Assert.Single(instructions);
        var instruction = instructions[0];
        Assert.NotNull(instruction);
        Assert.Equal(InstructionType.Mov, instruction.Type);

        // Check that we have two operands
        Assert.Equal(2, instruction.StructuredOperands.Count);

        // Check the first operand (EAX)
        var eaxOperand = instruction.StructuredOperands[0];
        Assert.IsType<RegisterOperand>(eaxOperand);
        var registerOperand = (RegisterOperand)eaxOperand;
        Assert.Equal(RegisterIndex.A, registerOperand.Register);
        Assert.Equal(32, registerOperand.Size); // Validate that it's a 32-bit register (EAX)

        // Check the second operand (memory operand with DS segment override)
        var memOperand = instruction.StructuredOperands[1];
        Assert.IsType<DirectMemoryOperand>(memOperand);
        var memoryOperand = (DirectMemoryOperand)memOperand;
        Assert.Equal(32, memoryOperand.Size); // Validate that it's a 32-bit memory reference
        Assert.Equal(Segment.Ds, memoryOperand.SegmentOverride);
    }

    /// <summary>
    /// Tests that the ES segment override prefix (0x26) is correctly recognized
    /// </summary>
    [Fact]
    public void EsSegmentOverride_IsRecognized()
    {
        // Arrange
        // ES segment override prefix (0x26) followed by MOV EAX, [0] (8B 05 00 00 00 00)
        var codeBuffer = new byte[] { 0x26, 0x8B, 0x05, 0x00, 0x00, 0x00, 0x00 };
        var disassembler = new Disassembler(codeBuffer, 0);

        // Act
        var instructions = disassembler.Disassemble();

        // Assert
        Assert.Single(instructions);
        var instruction = instructions[0];
        Assert.NotNull(instruction);
        Assert.Equal(InstructionType.Mov, instruction.Type);

        // Check that we have two operands
        Assert.Equal(2, instruction.StructuredOperands.Count);

        // Check the first operand (EAX)
        var eaxOperand = instruction.StructuredOperands[0];
        Assert.IsType<RegisterOperand>(eaxOperand);
        var registerOperand = (RegisterOperand)eaxOperand;
        Assert.Equal(RegisterIndex.A, registerOperand.Register);
        Assert.Equal(32, registerOperand.Size); // Validate that it's a 32-bit register (EAX)

        // Check the second operand (memory operand with ES segment override)
        var memOperand = instruction.StructuredOperands[1];
        Assert.IsType<DirectMemoryOperand>(memOperand);
        var memoryOperand = (DirectMemoryOperand)memOperand;
        Assert.Equal(32, memoryOperand.Size); // Validate that it's a 32-bit memory reference
        Assert.Equal(Segment.Es, memoryOperand.SegmentOverride);
    }

    /// <summary>
    /// Tests that the FS segment override prefix (0x64) is correctly recognized
    /// </summary>
    [Fact]
    public void FsSegmentOverride_IsRecognized()
    {
        // Arrange
        // FS segment override prefix (0x64) followed by MOV ESP, [0] (8B 25 00 00 00 00)
        var codeBuffer = new byte[] { 0x64, 0x8B, 0x25, 0x00, 0x00, 0x00, 0x00 };
        var disassembler = new Disassembler(codeBuffer, 0);

        // Act
        var instructions = disassembler.Disassemble();

        // Assert
        Assert.Single(instructions);
        var instruction = instructions[0];
        Assert.NotNull(instruction);
        Assert.Equal(InstructionType.Mov, instruction.Type);

        // Check that we have two operands
        Assert.Equal(2, instruction.StructuredOperands.Count);

        // Check the first operand (ESP)
        var espOperand = instruction.StructuredOperands[0];
        Assert.IsType<RegisterOperand>(espOperand);
        var registerOperand = (RegisterOperand)espOperand;
        Assert.Equal(RegisterIndex.Sp, registerOperand.Register);
        Assert.Equal(32, registerOperand.Size); // Validate that it's a 32-bit register (ESP)

        // Check the second operand (memory operand with FS segment override)
        var memOperand = instruction.StructuredOperands[1];
        Assert.IsType<DirectMemoryOperand>(memOperand);
        var memoryOperand = (DirectMemoryOperand)memOperand;
        Assert.Equal(32, memoryOperand.Size); // Validate that it's a 32-bit memory reference
        Assert.Equal(Segment.Fs, memoryOperand.SegmentOverride);
    }

    /// <summary>
    /// Tests that the GS segment override prefix (0x65) is correctly recognized
    /// </summary>
    [Fact]
    public void GsSegmentOverride_IsRecognized()
    {
        // Arrange
        // GS segment override prefix (0x65) followed by MOV EAX, [0] (8B 05 00 00 00 00)
        var codeBuffer = new byte[] { 0x65, 0x8B, 0x05, 0x00, 0x00, 0x00, 0x00 };
        var disassembler = new Disassembler(codeBuffer, 0);

        // Act
        var instructions = disassembler.Disassemble();

        // Debug output
        _testOutputHelper.WriteLine($"Number of instructions: {instructions.Count}");
        for (var i = 0; i < instructions.Count; i++)
        {
            _testOutputHelper.WriteLine($"Instruction {i}: Type={instructions[i].Type}, Address={instructions[i].Address:X}");
        }

        // Assert
        Assert.Single(instructions);
        var instruction = instructions[0];
        Assert.NotNull(instruction);
        Assert.Equal(InstructionType.Mov, instruction.Type);

        // Check that we have two operands
        Assert.Equal(2, instruction.StructuredOperands.Count);

        // Check the first operand (EAX)
        var eaxOperand = instruction.StructuredOperands[0];
        Assert.IsType<RegisterOperand>(eaxOperand);
        var registerOperand = (RegisterOperand)eaxOperand;
        Assert.Equal(RegisterIndex.A, registerOperand.Register);
        Assert.Equal(32, registerOperand.Size); // Validate that it's a 32-bit register (EAX)

        // Check the second operand (memory operand with GS segment override)
        var memOperand = instruction.StructuredOperands[1];
        Assert.IsType<DirectMemoryOperand>(memOperand);
        var memoryOperand = (DirectMemoryOperand)memOperand;
        Assert.Equal(32, memoryOperand.Size); // Validate that it's a 32-bit memory reference
        Assert.Equal(Segment.Gs, memoryOperand.SegmentOverride);
    }

    /// <summary>
    /// Tests that the SS segment override prefix (0x36) is correctly recognized
    /// </summary>
    [Fact]
    public void SsSegmentOverride_IsRecognized()
    {
        // Arrange
        // SS segment override prefix (0x36) followed by MOV EAX, [EBP-4] (8B 45 FC)
        var codeBuffer = new byte[] { 0x36, 0x8B, 0x45, 0xFC };
        var disassembler = new Disassembler(codeBuffer, 0);

        // Act
        var instructions = disassembler.Disassemble();

        // Assert
        Assert.Single(instructions);
        var instruction = instructions[0];
        Assert.NotNull(instruction);
        Assert.Equal(InstructionType.Mov, instruction.Type);

        // Check that we have two operands
        Assert.Equal(2, instruction.StructuredOperands.Count);

        // Check the first operand (EAX)
        var eaxOperand = instruction.StructuredOperands[0];
        Assert.IsType<RegisterOperand>(eaxOperand);
        var registerOperand = (RegisterOperand)eaxOperand;
        Assert.Equal(RegisterIndex.A, registerOperand.Register);
        Assert.Equal(32, registerOperand.Size); // Validate that it's a 32-bit register (EAX)

        // Check the second operand (memory operand with SS segment override)
        var memOperand = instruction.StructuredOperands[1];
        Assert.IsType<DisplacementMemoryOperand>(memOperand);
        var memoryOperand = (DisplacementMemoryOperand)memOperand;
        Assert.Equal(32, memoryOperand.Size); // Validate that it's a 32-bit memory reference
        Assert.Equal(Segment.Ss, memoryOperand.SegmentOverride);
        Assert.Equal(RegisterIndex.Bp, memoryOperand.BaseRegister);
        Assert.Equal(-4, memoryOperand.Displacement);
    }

    /// <summary>
    /// Tests segment override with a complex addressing mode
    /// </summary>
    [Fact]
    public void SegmentOverride_WithComplexAddressing_IsRecognized()
    {
        // Arrange
        // FS segment override prefix (0x64) followed by MOV EAX, [EBX+ECX*4+0x10] (8B 84 8B 10 00 00 00)
        var codeBuffer = new byte[] { 0x64, 0x8B, 0x84, 0x8B, 0x10, 0x00, 0x00, 0x00 };
        var disassembler = new Disassembler(codeBuffer, 0);

        // Act
        var instructions = disassembler.Disassemble();

        // Assert
        Assert.Single(instructions);
        var instruction = instructions[0];
        Assert.NotNull(instruction);
        Assert.Equal(InstructionType.Mov, instruction.Type);

        // Check that we have two operands
        Assert.Equal(2, instruction.StructuredOperands.Count);

        // Check the first operand (EAX)
        var eaxOperand = instruction.StructuredOperands[0];
        Assert.IsType<RegisterOperand>(eaxOperand);
        var registerOperand = (RegisterOperand)eaxOperand;
        Assert.Equal(RegisterIndex.A, registerOperand.Register);
        Assert.Equal(32, registerOperand.Size); // Validate that it's a 32-bit register (EAX)

        // Check the second operand (memory operand with FS segment override)
        var memOperand = instruction.StructuredOperands[1];
        Assert.IsType<ScaledIndexMemoryOperand>(memOperand);
        var memoryOperand = (ScaledIndexMemoryOperand)memOperand;
        Assert.Equal(32, memoryOperand.Size); // Validate that it's a 32-bit memory reference
        Assert.Equal(Segment.Fs, memoryOperand.SegmentOverride);
        Assert.Equal(RegisterIndex.B, memoryOperand.BaseRegister);
        Assert.Equal(RegisterIndex.C, memoryOperand.IndexRegister);
        Assert.Equal(4, memoryOperand.Scale);
        Assert.Equal(0x10, memoryOperand.Displacement);
    }

    /// <summary>
    /// Tests segment override with a string instruction
    /// </summary>
    [Fact]
    public void SegmentOverride_WithStringInstruction_IsRecognized()
    {
        // Arrange
        // ES segment override prefix (0x26) followed by LODS DWORD PTR DS:[ESI] (AD)
        var codeBuffer = new byte[] { 0x26, 0xAD };
        var disassembler = new Disassembler(codeBuffer, 0);

        // Act
        var instructions = disassembler.Disassemble();

        // Assert
        Assert.Single(instructions);
        var instruction = instructions[0];
        Assert.NotNull(instruction);
        Assert.Equal(InstructionType.LodsD, instruction.Type);

        // Check that we have two operands
        Assert.Equal(2, instruction.StructuredOperands.Count);

        // Check the first operand (EAX)
        var eaxOperand = instruction.StructuredOperands[0];
        Assert.IsType<RegisterOperand>(eaxOperand);
        var registerOperand = (RegisterOperand)eaxOperand;
        Assert.Equal(RegisterIndex.A, registerOperand.Register);
        Assert.Equal(32, registerOperand.Size); // Validate that it's a 32-bit register (EAX)

        // Check the second operand (memory operand with ES segment override)
        var memOperand = instruction.StructuredOperands[1];
        Assert.IsType<BaseRegisterMemoryOperand>(memOperand);
        var memoryOperand = (BaseRegisterMemoryOperand)memOperand;
        Assert.Equal(32, memoryOperand.Size); // Validate that it's a 32-bit memory reference
        Assert.Equal(RegisterIndex.Si, memoryOperand.BaseRegister);
        Assert.Equal(Segment.Es, memoryOperand.SegmentOverride);
    }

    /// <summary>
    /// Tests segment override with a REP prefix
    /// </summary>
    [Fact]
    public void SegmentOverride_WithRepPrefix_IsRecognized()
    {
        // Arrange
        // REP prefix (F3) followed by FS segment override prefix (0x64) followed by MOVS (A4)
        var codeBuffer = new byte[] { 0xF3, 0x64, 0xA4 };
        var disassembler = new Disassembler(codeBuffer, 0);

        // Act
        var instructions = disassembler.Disassemble();

        // Assert
        Assert.Single(instructions);
        var instruction = instructions[0];
        Assert.NotNull(instruction);
        Assert.Equal(InstructionType.RepMovsB, instruction.Type);

        // Check that we have two operands
        Assert.Equal(2, instruction.StructuredOperands.Count);

        // Check the first operand (memory operand with FS segment override)
        var memOperand = instruction.StructuredOperands[0];
        Assert.IsType<BaseRegisterMemoryOperand>(memOperand);
        var memoryOperand = (BaseRegisterMemoryOperand)memOperand;
        Assert.Equal(8, memoryOperand.Size); // Validate that it's a byte memory reference
        Assert.Equal(Segment.Fs, memoryOperand.SegmentOverride);
        Assert.Equal(RegisterIndex.Di, memoryOperand.BaseRegister);

        // Check the second operand (memory operand with FS segment override)
        var memOperand2 = instruction.StructuredOperands[1];
        Assert.IsType<BaseRegisterMemoryOperand>(memOperand2);
        var memoryOperand2 = (BaseRegisterMemoryOperand)memOperand2;
        Assert.Equal(8, memoryOperand2.Size); // Validate that it's a byte memory reference
        Assert.Equal(Segment.Fs, memoryOperand2.SegmentOverride);
        Assert.Equal(RegisterIndex.Si, memoryOperand2.BaseRegister);
    }
}
