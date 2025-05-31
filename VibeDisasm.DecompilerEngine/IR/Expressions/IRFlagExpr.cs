using VibeDisasm.DecompilerEngine.IR.Model;
using VibeDisasm.DecompilerEngine.IR.Visitors;

namespace VibeDisasm.DecompilerEngine.IR.Expressions;

public class IRFlagExpr : IRExpression
{
    public IRFlag Flag { get; init; }

    public override List<IRExpression> SubExpressions => [];

    public IRFlagExpr(IRFlag flag)
    {
        Flag = flag;
    }

    public override string ToString() => Flag.ToString();

    public override bool Equals(object? obj)
    {
        if (obj is IRFlagExpr other)
        {
            return Flag == other.Flag;
        }

        return false;
    }

    public override void Accept(IIRNodeVisitor visitor) => visitor.Visit(this);

    public override T? Accept<T>(IIRNodeReturningVisitor<T> visitor) where T : default => visitor.VisitFlag(this);

    public override int GetHashCode()
    {
        throw new NotImplementedException();
    }
}
