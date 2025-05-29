namespace VibeDisasm.DecompilerEngine.IR.Expressions;

public class IRFlagExpr : IRExpression
{
    public IRFlag Flag { get; init; }
    public override string ToString() => Flag.ToString();

    public IRFlagExpr(IRFlag flag)
    {
        Flag = flag;
    }
}