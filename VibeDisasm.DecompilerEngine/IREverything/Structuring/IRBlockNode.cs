using System.Diagnostics;
using VibeDisasm.DecompilerEngine.IREverything.Model;
using VibeDisasm.DecompilerEngine.IREverything.Visitors;

namespace VibeDisasm.DecompilerEngine.IREverything.Structuring;

/// <summary>
/// Represents a basic block as a leaf node in the structured IR tree.
/// </summary>
[DebuggerDisplay("{DebugDisplay}")]
public class IRBlockNode : IRStructuredNode
{
    public IRBlock Block { get; }

    public IRBlockNode(IRBlock block)
    {
        Block = block;
    }

    public override void Accept(IIRNodeVisitor visitor) => visitor.VisitBlock(this);

    public override T? Accept<T>(IIRNodeReturningVisitor<T> visitor) where T : default => visitor.VisitBlock(this);
    internal override string DebugDisplay => $"IRBlockNode({Block.DebugDisplay})";
}
