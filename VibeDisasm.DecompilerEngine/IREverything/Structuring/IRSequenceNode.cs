using System.Diagnostics;
using System.Diagnostics.Contracts;
using VibeDisasm.DecompilerEngine.IREverything.Visitors;

namespace VibeDisasm.DecompilerEngine.IREverything.Structuring;

/// <summary>
/// Represents a sequence of structured IR nodes executed in order.
/// </summary>
[DebuggerDisplay("{DebugDisplay}")]
public class IRSequenceNode : IRStructuredNode
{
    public List<IRStructuredNode> Nodes { get; }

    public IRSequenceNode()
    {
        Nodes = [];
    }

    public IRSequenceNode(IEnumerable<IRStructuredNode> nodes)
    {
        Nodes = nodes.ToList();
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

    public override void Accept(IIRNodeVisitor visitor) => visitor.VisitSequence(this);

    public override T? Accept<T>(IIRNodeReturningVisitor<T> visitor) where T : default => visitor.VisitSequence(this);
    internal override string DebugDisplay => $"IRSequence({Nodes.Count} nodes)";
}
