using System.Diagnostics;
using VibeDisasm.DecompilerEngine.IREverything.Visitors;

namespace VibeDisasm.DecompilerEngine.IREverything.Expressions;

[DebuggerDisplay("{DebugDisplay}")]
public class IRLogicalExpr : IRExpression
{
    public IRExpression Left { get; init; }
    public IRExpression Right { get; init; }

    public IRLogicalOperation Operation { get; init; }

    public override List<IRExpression> SubExpressions => [Left, Right];

    public IRLogicalExpr(IRExpression left, IRExpression right, IRLogicalOperation operation)
    {
        Left = left;
        Right = right;
        Operation = operation;
    }

    public override IRExpression Invert()
    {
        throw new InvalidOperationException("Cant invert this logical expression now");
    }

    public override bool Equals(object? obj)
    {
        if (obj is IRLogicalExpr other)
        {
            return Left.Equals(other.Left) && Right.Equals(other.Right) && Operation == other.Operation;
        }

        return false;
    }

    public override void Accept(IIRNodeVisitor visitor) => visitor.VisitLogical(this);

    public override T? Accept<T>(IIRNodeReturningVisitor<T> visitor) where T : default => visitor.VisitLogical(this);

    internal override string DebugDisplay => $"{Left.DebugDisplay} {Operation} {Right.DebugDisplay}";

    public override int GetHashCode()
    {
        throw new NotImplementedException();
    }
}
