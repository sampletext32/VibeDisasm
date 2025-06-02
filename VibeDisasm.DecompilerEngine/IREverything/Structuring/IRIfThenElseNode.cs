using System.Diagnostics;
using System.Diagnostics.Contracts;
using VibeDisasm.DecompilerEngine.IREverything.Expressions;
using VibeDisasm.DecompilerEngine.IREverything.Visitors;

namespace VibeDisasm.DecompilerEngine.IREverything.Structuring;

/// <summary>
/// Represents an if-then or if-then-else control structure in the IR.
/// </summary>
[DebuggerDisplay("{DebugDisplay}")]
public class IRIfThenElseNode : IRStructuredNode
{
    public IRExpression Condition { get; }
    public IRBlock ThenBlock { get; }
    public IRBlock ElseBlock { get; }

    public IRIfThenElseNode(IRExpression condition, IRBlock thenBlock, IRBlock elseBlock)
    {
        Condition = condition;
        ThenBlock = thenBlock;
        ElseBlock = elseBlock;
    }

    public override IEnumerable<IRBlock> EnumerateBlocks()
    {
        yield return ThenBlock;
        yield return ElseBlock;
    }

    public override void Accept(IIRNodeVisitor visitor) => visitor.VisitIfThenElse(this);

    public override T? Accept<T>(IIRNodeReturningVisitor<T> visitor) where T : default => visitor.VisitIfThenElse(this);
    internal override string DebugDisplay => $"IRIfElse({Condition.DebugDisplay})";
}
