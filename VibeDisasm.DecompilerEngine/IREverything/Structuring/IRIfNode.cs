using System.Diagnostics;
using System.Diagnostics.Contracts;
using VibeDisasm.DecompilerEngine.IREverything.Expressions;
using VibeDisasm.DecompilerEngine.IREverything.Visitors;

namespace VibeDisasm.DecompilerEngine.IREverything.Structuring;

/// <summary>
/// Represents an if-then or if-then-else control structure in the IR.
/// </summary>
[DebuggerDisplay("{DebugDisplay}")]
public class IRIfNode : IRStructuredNode
{
    public IRExpression Condition { get; }
    public IRStructuredNode ThenBranch { get; }
    public IRStructuredNode? ElseBranch { get; }

    public IRIfNode(IRExpression condition, IRStructuredNode thenBranch)
    {
        Condition = condition;
        ThenBranch = thenBranch;
        ThenBranch.Parent = this;
        ElseBranch = null;
    }

    public IRIfNode(IRExpression condition, IRStructuredNode thenBranch, IRStructuredNode elseBranch)
    {
        Condition = condition;
        ThenBranch = thenBranch;
        ThenBranch.Parent = this;
        ElseBranch = elseBranch;
        ElseBranch.Parent = this;
    }

    [Pure]
    public bool HasElseBranch => ElseBranch != null;

    public override void Accept(IIRNodeVisitor visitor) => visitor.VisitIf(this);

    public override T? Accept<T>(IIRNodeReturningVisitor<T> visitor) where T : default => visitor.VisitIf(this);
    internal override string DebugDisplay => $"IRIf({Condition.DebugDisplay})";
}
