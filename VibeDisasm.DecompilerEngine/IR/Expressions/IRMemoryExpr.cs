namespace VibeDisasm.DecompilerEngine.IR.Expressions;

/// <summary>
/// Represents a memory operand in IR.
/// Example: [ebp+8] -> IRMemory("ebp+8")
/// </summary>
public sealed class IRMemoryExpr : IRExpression
{
    public string Address { get; init; }
    public IRMemoryExpr(string address) => Address = address;
    public override string ToString() => $"[{Address}]";
}
