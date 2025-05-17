using VibeDisasm.DecompilerEngine.ControlFlow;

namespace VibeDisasm.DecompilerEngine.IR;

/// <summary>
/// Base class for all nodes in the intermediate representation (IR).
/// <para>
/// The IR is a tree-like structure that represents the program's semantics in a form
/// that's easier to analyze than raw assembly code. Each node in the IR tree represents
/// a specific construct in the program.
/// </para>
/// <para>
/// The IR hierarchy consists of:
/// - Expressions: Computations that yield values (e.g., 5, eax, eax+5)
/// - Statements: Operations that change state or control flow (e.g., assignments, jumps)
/// - Functions: Collections of blocks representing a complete function
/// - Blocks: Groups of statements that execute sequentially
/// </para>
/// <para>
/// Each node may have a reference to its source (the original assembly instruction)
/// and maintains parent-child relationships to form the IR tree.
/// </para>
/// </summary>
public abstract class IRNode
{
    public IRNodeType NodeType { get; }
    
    public object? SourceReference { get; set; }
    
    public IRNode? Parent { get; set; }
    
    public List<IRNode> Children { get; } = [];
    
    protected IRNode(IRNodeType nodeType)
    {
        NodeType = nodeType;
    }
    
    public void AddChild(IRNode child)
    {
        Children.Add(child);
        child.Parent = this;
    }
    
    public void RemoveChild(IRNode child)
    {
        if (Children.Remove(child))
        {
            child.Parent = null;
        }
    }
    
    public void ReplaceChild(IRNode oldChild, IRNode newChild)
    {
        var index = Children.IndexOf(oldChild);
        if (index >= 0)
        {
            oldChild.Parent = null;
            Children[index] = newChild;
            newChild.Parent = this;
        }
    }
}
