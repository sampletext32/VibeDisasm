namespace VibeDisasm.DecompilerEngine.IR.Statements;

/// <summary>
/// Represents a block of statements in the IR.
/// <para>
/// Block statements represent a sequence of statements that execute one after another.
/// In assembly, this corresponds to a basic block - a sequence of instructions with a single
/// entry point and a single exit point (no internal branches).
/// </para>
/// <para>
/// Examples:
/// - A sequence of assignments: { x = 1; y = 2; z = x + y; }
/// - A basic block in assembly: { mov eax, 5; add ebx, eax; push ebx; }
/// - The body of a function or control structure
/// </para>
/// <para>
/// In IR form:
/// ```
/// Block_0x401000:
/// {
///   eax = 5;
///   ebx = ebx + eax;
///   stack = ebx;
/// }
/// ```
/// </para>
/// </summary>
public class IRBlockStatement : IRStatement
{
    public List<IRStatement> Statements => Children.Cast<IRStatement>().ToList();
    
    public IRBlockStatement() : base(IRNodeType.Block)
    {
    }
    
    public void AddStatement(IRStatement statement)
    {
        AddChild(statement);
    }
    
    public void InsertStatement(int index, IRStatement statement)
    {
        Children.Insert(index, statement);
        statement.Parent = this;
    }
    
    public void RemoveStatement(IRStatement statement)
    {
        RemoveChild(statement);
    }
}
