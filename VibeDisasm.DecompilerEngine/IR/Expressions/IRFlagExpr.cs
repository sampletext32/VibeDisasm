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
}