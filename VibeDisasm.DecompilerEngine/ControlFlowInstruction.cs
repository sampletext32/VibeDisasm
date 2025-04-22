using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using VibeDisasm.Disassembler.X86;
using VibeDisasm.Disassembler.X86.Operands;

namespace VibeDisasm.DecompilerEngine;

/// <summary>
/// Provides a higher-level abstraction over x86 instructions for control flow analysis
/// </summary>
public class ControlFlowInstruction
{
    /// <summary>
    /// The underlying raw x86 instruction
    /// </summary>
    public Instruction RawInstruction { get; }
    
    /// <summary>
    /// The address of the instruction
    /// </summary>
    public ulong Address => RawInstruction.Address;
    
    /// <summary>
    /// The length of the instruction in bytes
    /// </summary>
    public byte Length => (byte)RawInstruction.Length;
    
    /// <summary>
    /// The type of the instruction
    /// </summary>
    public InstructionType Type => RawInstruction.Type;
    
    /// <summary>
    /// Creates a new control flow instruction wrapper
    /// </summary>
    public ControlFlowInstruction(Instruction instruction)
    {
        RawInstruction = instruction;
    }
    
    /// <summary>
    /// Gets the next sequential instruction address
    /// </summary>
    [Pure]
    public uint GetNextSequentialAddress()
    {
        return (uint)(Address + Length);
    }
    
    /// <summary>
    /// Determines if this instruction is a jump instruction
    /// </summary>
    [Pure]
    public bool IsJump()
    {
        return Type.IsConditionalJump() || Type.IsUnconditionalJump();
    }
    
    /// <summary>
    /// Determines if this instruction is a conditional jump
    /// </summary>
    [Pure]
    public bool IsConditionalJump()
    {
        return Type.IsConditionalJump();
    }
    
    /// <summary>
    /// Determines if this instruction is an unconditional jump
    /// </summary>
    [Pure]
    public bool IsUnconditionalJump()
    {
        return Type.IsUnconditionalJump();
    }
    
    /// <summary>
    /// Gets the jump target address if this is a jump instruction
    /// </summary>
    /// <returns>The jump target address, or null if this is not a jump or target cannot be determined</returns>
    [Pure]
    public uint? GetJumpTargetAddress()
    {
        if (!IsJump())
        {
            return null;
        }
        
        // Get the jump target from the first operand if it's an immediate value
        if (RawInstruction.StructuredOperands.Count > 0 && 
            RawInstruction.StructuredOperands[0] is ImmediateOperand imm)
        {
            return (uint)imm.Value;
        }
        // Get the jump target from the first operand if it's an immediate value
        if (RawInstruction.StructuredOperands.Count > 0 && 
            RawInstruction.StructuredOperands[0] is RelativeOffsetOperand roo)
        {
            return roo.TargetAddress;
        }
        
        return null;
    }
    
    /// <summary>
    /// String representation of the instruction
    /// </summary>
    public override string ToString()
    {
        return RawInstruction.ToString();
    }
}
