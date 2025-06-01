using System.Diagnostics;
using System.Diagnostics.Contracts;
using VibeDisasm.DecompilerEngine.IREverything.Expressions;
using VibeDisasm.DecompilerEngine.IREverything.Visitors;

namespace VibeDisasm.DecompilerEngine.IREverything.Structuring;

/// <summary>
/// Type of a loop structure.
/// </summary>
public enum IRLoopType
{
    While,    // Condition checked before body
    DoWhile,  // Condition checked after body
    For       // With initialization, condition, and update
}

/// <summary>
/// Represents a loop control structure in the IR.
/// </summary>
[DebuggerDisplay("{DebugDisplay}")]
public class IRLoopNode : IRStructuredNode
{
    public IRLoopType LoopType { get; }
    public IRExpression Condition { get; }
    public IRStructuredNode Body { get; }
    public IRExpression? Initialization { get; }
    public IRExpression? Update { get; }

    public IRLoopNode(IRLoopType loopType, IRExpression condition, IRStructuredNode body)
    {
        if (loopType == IRLoopType.For)
        {
            throw new ArgumentException("For loops require initialization and update expressions.", nameof(loopType));
        }

        LoopType = loopType;
        Condition = condition;
        Body = body;
        Body.Parent = this;
        Initialization = null;
        Update = null;
    }

    public IRLoopNode(IRExpression condition, IRExpression initialization, IRExpression update, IRStructuredNode body)
    {
        LoopType = IRLoopType.For;
        Condition = condition;
        Initialization = initialization;
        Update = update;
        Body = body;
        Body.Parent = this;
    }

    [Pure]
    public bool IsForLoop => LoopType == IRLoopType.For;

    [Pure]
    public bool IsWhileLoop => LoopType == IRLoopType.While;

    [Pure]
    public bool IsDoWhileLoop => LoopType == IRLoopType.DoWhile;

    public override void Accept(IIRNodeVisitor visitor) => visitor.VisitLoop(this);

    public override T? Accept<T>(IIRNodeReturningVisitor<T> visitor) where T : default => visitor.VisitLoop(this);
    internal override string DebugDisplay => $"IRLoop({LoopType:G}, {Condition.DebugDisplay})";
}
