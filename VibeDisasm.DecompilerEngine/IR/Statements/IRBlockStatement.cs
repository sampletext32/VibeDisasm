namespace VibeDisasm.DecompilerEngine.IR.Statements;

/// <summary>
/// Represents a block of statements in the IR
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
