using VibeDisasm.DecompilerEngine.ControlFlow;

namespace VibeDisasm.DecompilerEngine.IR;

/// <summary>
/// Base class for all nodes in the intermediate representation
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
