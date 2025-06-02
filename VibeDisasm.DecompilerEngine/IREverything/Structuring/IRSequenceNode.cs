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

    [Pure]
    public int FindNodeIndex(Func<IRStructuredNode, bool> predicate)
    {
        for (var i = 0; i < Nodes.Count; i++)
        {
            if (predicate(Nodes[i]))
                return i;
        }

        return -1;
    }

    public bool Replace(IRStructuredNode nodeToReplace, IRStructuredNode replacement)
    {
        var index = Nodes.IndexOf(nodeToReplace);
        if (index == -1)
            return false;

        replacement.Parent = this;
        Nodes[index] = replacement;
        return true;
    }

    public bool ReplaceAt(int index, IRStructuredNode replacement)
    {
        if (index < 0 || index >= Nodes.Count)
            return false;

        replacement.Parent = this;
        Nodes[index] = replacement;
        return true;
    }

    public bool ReplaceAndInsertAfter(int targetIndex, int insertAfterIndex, IRStructuredNode replacement)
    {
        if (targetIndex < 0 || targetIndex >= Nodes.Count || insertAfterIndex < 0 || insertAfterIndex >= Nodes.Count)
            return false;

        replacement.Parent = this;
        Insert(insertAfterIndex + 1, replacement);
        Nodes.RemoveAt(targetIndex + (targetIndex > insertAfterIndex ? 1 : 0));
        return true;
    }
}
