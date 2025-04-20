using System.Collections.Generic;
using System.Linq;
using System.Text;
using VibeDisasm.Disassembler.X86;
using VibeDisasm.Disassembler.X86.Operands;

namespace VibeDisasm.DecompilerEngine;

/// <summary>
/// Represents a basic block of instructions in the control flow graph
/// </summary>
public class InstructionBlock
{
    /// <summary>
    /// The starting address of the block
    /// </summary>
    public uint StartAddress { get; set; }

    /// <summary>
    /// The list of instructions in the block
    /// </summary>
    public List<Instruction> Instructions { get; set; } = [];
    
    /// <summary>
    /// Gets the last instruction in the block, or null if the block is empty
    /// </summary>
    public Instruction? LastInstruction => Instructions.Count > 0 ? Instructions[^1] : null;
    
    /// <summary>
    /// Gets a string representation of the block
    /// </summary>
    public override string ToString()
    {
        var sb = new StringBuilder();
        sb.AppendLine($"Block at {StartAddress:X8}:");
        
        foreach (var instruction in Instructions)
        {
            sb.AppendLine($"  {instruction.Address:X8}: {instruction}");
        }
        
        return sb.ToString();
    }
}