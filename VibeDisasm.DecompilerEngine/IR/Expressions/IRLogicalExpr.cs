using VibeDisasm.DecompilerEngine.IR.Visitors;

namespace VibeDisasm.DecompilerEngine.IR.Expressions;

public class IRLogicalExpr : IRExpression
{
    public IRExpression Operand1 { get; init; }
    public IRExpression Operand2 { get; init; }

    public IRLogicalOperation Operation { get; init; }

    public override List<IRExpression> SubExpressions => [Operand1, Operand2];

    public IRLogicalExpr(IRExpression operand1, IRExpression operand2, IRLogicalOperation operation)
    {
        Operand1 = operand1;
        Operand2 = operand2;
        Operation = operation;
    }

    public override string ToString() => $"{Operand1} {Operation.ToLangString()} {Operand2}";

    public override bool Equals(object? obj)
    {
        if (obj is IRLogicalExpr other)
        {
            return Operand1.Equals(other.Operand1) && Operand2.Equals(other.Operand2) && Operation == other.Operation;
        }

        return false;
    }

    public override void Accept(IIRNodeVisitor visitor) => visitor.Visit(this);

    public override T Accept<T>(IIRNodeReturningVisitor<T> visitor) => visitor.Visit(this);

    public override int GetHashCode()
    {
        throw new NotImplementedException();
    }
}

public enum IRLogicalOperation
{
    And,
    Or
}

public static class IRLogicalOperationExtensions
{
    public static string ToLangString(this IRLogicalOperation operation) => operation switch
    {
        IRLogicalOperation.And => "&&",
        IRLogicalOperation.Or => "||",
        _ => throw new ArgumentOutOfRangeException(nameof(operation), operation, null)
    };
}
