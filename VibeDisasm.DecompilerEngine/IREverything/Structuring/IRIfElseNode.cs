using System.Diagnostics;
using System.Diagnostics.Contracts;
using VibeDisasm.DecompilerEngine.IREverything.Expressions;
using VibeDisasm.DecompilerEngine.IREverything.Visitors;

namespace VibeDisasm.DecompilerEngine.IREverything.Structuring;

/// <summary>
/// Represents an if-then or if-then-else control structure in the IR.
/// </summary>
[DebuggerDisplay("{DebugDisplay}")]
public class IRIfElseNode : IRStructuredNode
{
    public IRExpression Condition { get; }
    public IRBlock ThenBlock { get; }
    public IRBlock ElseBlock { get; }

    public IRIfElseNode(IRExpression condition, IRBlock thenBlock, IRBlock elseBlock)
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

    public override void Accept(IIRNodeVisitor visitor) => visitor.VisitIfElse(this);

    public override T? Accept<T>(IIRNodeReturningVisitor<T> visitor) where T : default => visitor.VisitIfElse(this);
    internal override string DebugDisplay => $"IRIfElse({Condition.DebugDisplay})";
}
