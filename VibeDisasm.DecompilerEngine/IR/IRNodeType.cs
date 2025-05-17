namespace VibeDisasm.DecompilerEngine.IR;

/// <summary>
/// Defines the types of nodes in the intermediate representation
/// </summary>
public enum IRNodeType
{
    // Base types
    Unknown,
    
    // Expressions
    Constant,
    Variable,
    Binary,
    Unary,
    Call,
    MemoryAccess,
    Register,
    
    // Statements
    Assignment,
    CallStatement,
    Return,
    Jump,
    Conditional,
    Loop,
    Block,
    
    // Structured constructs
    Sequence,
    IfThenElse,
    WhileLoop,
    DoWhileLoop,
    ForLoop,
    Switch
}
