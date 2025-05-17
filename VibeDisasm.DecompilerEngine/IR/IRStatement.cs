namespace VibeDisasm.DecompilerEngine.IR;

/// <summary>
/// Base class for all statement nodes that represent actions or control flow.
/// <para>
/// Statements represent operations that change program state or control flow. Examples include:
/// - Assignments (e.g., eax = 5, [ebp+8] = ecx)
/// - Jumps (e.g., goto 0x1000, jnz 0x2000)
/// - Function calls (e.g., call printf)
/// - Returns (e.g., return, return eax)
/// - Blocks of statements (e.g., { stmt1; stmt2; stmt3; })
/// - Conditional branches (e.g., if (condition) { ... } else { ... })
/// </para>
/// </summary>
public abstract class IRStatement : IRNode
{
    /// <summary>
    /// Optional comment for this statement
    /// </summary>
    public string? Comment { get; set; }
    
    public IRStatement(IRNodeType nodeType) : base(nodeType)
    {
    }
}
