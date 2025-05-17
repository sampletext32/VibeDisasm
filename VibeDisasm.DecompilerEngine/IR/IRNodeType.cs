namespace VibeDisasm.DecompilerEngine.IR;

/// <summary>
/// Defines the types of nodes in the intermediate representation
/// </summary>
public enum IRNodeType
{
    // Expressions
    Constant,
    Variable,
    Binary,
    Unary,
    MemoryAccess,
    Register,
    
    // Statements
    Assignment,
    Call,
    Return,
    Jump,
    Conditional,
    Block,
    
    // Structured constructs
    WhileLoop,
    DoWhileLoop,
    ForLoop,
    Switch
}
