namespace VibeDisasm.DecompilerEngine.IR;

/// <summary>
/// Base class for all expression nodes that produce a value.
/// <para>
/// Expressions represent computations that yield values. Examples include:
/// - Constants (e.g., 42, 0x1000)
/// - Register references (e.g., eax, ebx)
/// - Memory accesses (e.g., [eax+4], [0x1000])
/// - Binary operations (e.g., x + y, a * b)
/// - Unary operations (e.g., !x, -y)
/// </para>
/// </summary>
public abstract class IRExpression : IRNode
{
    public IRExpression(IRNodeType nodeType) : base(nodeType)
    {
    }
}
