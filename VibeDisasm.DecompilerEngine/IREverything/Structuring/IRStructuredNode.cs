using System.Diagnostics.Contracts;
using VibeDisasm.DecompilerEngine.IREverything.Model;
using VibeDisasm.DecompilerEngine.IREverything.Visitors;

namespace VibeDisasm.DecompilerEngine.IREverything.Structuring;

/// <summary>
/// Base class for all structured IR nodes that represent high-level control flow constructs.
/// </summary>
public abstract class IRStructuredNode : IRNode
{
    public IRStructuredNode? Parent { get; internal set; }

    public abstract IEnumerable<IRBlock> EnumerateBlocks();

    public abstract override void Accept(IIRNodeVisitor visitor);

    public abstract override T? Accept<T>(IIRNodeReturningVisitor<T> visitor) where T : default;
}
