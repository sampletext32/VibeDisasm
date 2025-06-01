using System.Diagnostics;
using VibeDisasm.DecompilerEngine.IREverything.Instructions;
using VibeDisasm.DecompilerEngine.IREverything.Model;
using VibeDisasm.DecompilerEngine.IREverything.Visitors;

namespace VibeDisasm.DecompilerEngine.IREverything.Structuring;

/// <summary>
/// Represents a basic block in IR.
/// Example: Block with instructions for a loop body
/// </summary>
[DebuggerDisplay("{DebugDisplay}")]
public class IRBlock : IRStructuredNode
{
    public uint Address { get; init; }
    public List<IRInstruction> Instructions { get; init; }
    public bool IsEntryBlock { get; init; }

    public IRBlock(uint address, List<IRInstruction> instructions, bool isEntryBlock)
    {
        IsEntryBlock = isEntryBlock;
        Address = address;
        Instructions = instructions;
    }

    public IEnumerable<T> EnumerateInstructionOfType<T>()
        where T : IRInstruction =>
        Instructions.OfType<T>();

    public override IEnumerable<IRBlock> EnumerateBlocks()
    {
        yield return this;
    }

    public override void Accept(IIRNodeVisitor visitor) => visitor.VisitBlock(this);

    public override T? Accept<T>(IIRNodeReturningVisitor<T> visitor) where T : default => visitor.VisitBlock(this);

    internal override string DebugDisplay => $"IRBlock({Address:X8})";
}
