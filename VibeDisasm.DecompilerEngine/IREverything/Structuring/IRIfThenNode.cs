using System.Diagnostics;
using VibeDisasm.DecompilerEngine.IREverything.Expressions;
using VibeDisasm.DecompilerEngine.IREverything.Model;
using VibeDisasm.DecompilerEngine.IREverything.Visitors;

namespace VibeDisasm.DecompilerEngine.IREverything.Structuring;

[DebuggerDisplay("{DebugDisplay}")]
public class IRIfThenNode : IRStructuredNode
{
    public IRExpression Condition { get; }
    public IRBlock ThenBlock { get; }

    public IRIfThenNode(IRExpression condition, IRBlock thenBlock)
    {
        Condition = condition;
        ThenBlock = thenBlock;
    }

    public override IEnumerable<IRBlock> EnumerateBlocks()
    {
        yield return ThenBlock;
    }

    public override void Accept(IIRNodeVisitor visitor) => visitor.VisitIfThen(this);

    public override T? Accept<T>(IIRNodeReturningVisitor<T> visitor) where T : default => visitor.VisitIfThen(this);

    internal override string DebugDisplay => $"IRIfThenNode(Condition: {Condition.DebugDisplay}. ThenBlock: {ThenBlock.Address:X8})";
}
