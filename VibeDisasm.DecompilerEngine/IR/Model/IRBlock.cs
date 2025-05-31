using System.Diagnostics;
using VibeDisasm.DecompilerEngine.IR.Instructions;
using VibeDisasm.DecompilerEngine.IR.Visitors;

namespace VibeDisasm.DecompilerEngine.IR.Model;

/// <summary>
/// Represents a basic block in IR.
/// Example: Block with instructions for a loop body
/// </summary>
[DebuggerDisplay("{DebugDisplay}")]
public class IRBlock : IRNode
{
    public required string Id { get; init; }
    public required List<IRInstruction> Instructions { get; init; }

    public IEnumerable<T> EnumerateInstructionOfType<T>()
        where T : IRInstruction =>
        Instructions.OfType<T>();

    public override void Accept(IIRNodeVisitor visitor) => visitor.VisitBlock(this);

    public override T? Accept<T>(IIRNodeReturningVisitor<T> visitor) where T : default => visitor.VisitBlock(this);

    internal override string DebugDisplay => $"IRBlock({Id})";
}
