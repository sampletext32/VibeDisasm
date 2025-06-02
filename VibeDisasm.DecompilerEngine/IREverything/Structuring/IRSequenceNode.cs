using System.Diagnostics;
using VibeDisasm.DecompilerEngine.IREverything.Visitors;

namespace VibeDisasm.DecompilerEngine.IREverything.Structuring;

/// <summary>
/// Represents a sequence of structured IR nodes executed in order.
/// </summary>
[DebuggerDisplay("{DebugDisplay}")]
public class IRSequenceNode : IRStructuredNode
{
    public List<IRStructuredNode> Nodes { get; }

    public IRSequenceNode(List<IRStructuredNode> nodes)
    {
        Nodes = nodes;
        foreach (var node in Nodes)
        {
            node.Parent = this;
        }
    }

    public void AddNode(IRStructuredNode node)
    {
        node.Parent = this;
        Nodes.Add(node);
    }

    public void AddNodes(IEnumerable<IRStructuredNode> nodes)
    {
        foreach (var node in nodes)
        {
            AddNode(node);
        }
    }

    public override IEnumerable<IRBlock> EnumerateBlocks()
    {
        foreach (var node in Nodes)
        {
            var blocks = node.EnumerateBlocks();
            foreach (var block in blocks)
            {
                yield return block;
            }
        }
    }

    public override void Accept(IIRNodeVisitor visitor) => visitor.VisitSequence(this);

    public override T? Accept<T>(IIRNodeReturningVisitor<T> visitor) where T : default => visitor.VisitSequence(this);
    internal override string DebugDisplay => $"IRSequence({Nodes.Count} nodes)";

    public void Insert(int index, IRStructuredNode node)
    {
        node.Parent = this;
        Nodes.Insert(index, node);
    }
}
