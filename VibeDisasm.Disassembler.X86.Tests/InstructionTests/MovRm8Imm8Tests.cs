using System.Collections;
using VibeDisasm.Disassembler.X86.Operands;

namespace VibeDisasm.Disassembler.X86.Tests.InstructionTests;

/// <summary>
/// Tests for MOV r/m8, imm8 instruction (0xC6)
/// </summary>
public class MovRm8Imm8Tests
{
    /// <summary>
    /// Tests the MOV r8, imm8 instruction (0xC6) with register operand
    /// </summary>
    [Fact]
    public void TestMovR8Imm8()
    {
        // Arrange
        byte[] code = { 0xC6, 0xC0, 0x42 }; // MOV AL, 0x42
        
        // Act
        Disassembler disassembler = new Disassembler(code, 0x1000);
        var instructions = disassembler.Disassemble();
        
        // Assert
        Assert.Single((IEnumerable) instructions);
        var instruction = instructions[0];
        Assert.Equal(InstructionType.Mov, instruction.Type);
        
        // Check that we have two operands
        Assert.Equal(2, instruction.StructuredOperands.Count);
        
        // Check the first operand (AL)
        var alOperand = instruction.StructuredOperands[0];
        Assert.IsType<Register8Operand>(alOperand);
        var registerOperand = (Register8Operand)alOperand;
        Assert.Equal(RegisterIndex8.AL, registerOperand.Register);
        Assert.Equal(8, registerOperand.Size); // Validate that it's an 8-bit register (AL)
        
        // Check the second operand (immediate value)
        var immOperand = instruction.StructuredOperands[1];
        Assert.IsType<ImmediateOperand>(immOperand);
        var immediateOperand = (ImmediateOperand)immOperand;
        Assert.Equal(0x42U, immediateOperand.Value);
        Assert.Equal(8, immediateOperand.Size); // Validate that it's an 8-bit immediate
    }
    
    /// <summary>
    /// Tests the MOV m8, imm8 instruction (0xC6) with memory operand
    /// </summary>
    [Fact]
    public void TestMovM8Imm8()
    {
        // Arrange
        byte[] code = { 0xC6, 0x01, 0x01 }; // MOV BYTE PTR [ECX], 0x01
        
        // Act
        Disassembler disassembler = new Disassembler(code, 0x1000);
        var instructions = disassembler.Disassemble();
        
        // Assert
        Assert.Single((IEnumerable) instructions);
        var instruction = instructions[0];
        Assert.Equal(InstructionType.Mov, instruction.Type);
        
        // Check that we have two operands
        Assert.Equal(2, instruction.StructuredOperands.Count);
        
        // Check the first operand (memory operand)
        var memOperand = instruction.StructuredOperands[0];
        Assert.IsType<BaseRegisterMemoryOperand>(memOperand);
        var memoryOperand = (BaseRegisterMemoryOperand)memOperand;
        Assert.Equal(RegisterIndex.C, memoryOperand.BaseRegister);
        Assert.Equal(8, memoryOperand.Size); // Validate that it's an 8-bit memory reference
        
        // Check the second operand (immediate value)
        var immOperand = instruction.StructuredOperands[1];
        Assert.IsType<ImmediateOperand>(immOperand);
        var immediateOperand = (ImmediateOperand)immOperand;
        Assert.Equal(0x01U, immediateOperand.Value);
        Assert.Equal(8, immediateOperand.Size); // Validate that it's an 8-bit immediate
    }
}
