using VibeDisasm.DecompilerEngine.IR.Visitors;

namespace VibeDisasm.DecompilerEngine.IR.Model;

/// <summary>
/// Base class for all IR types. Only needed for generics and polymorphism.
/// </summary>
public abstract class IRNode
{
    public abstract void Accept(IIRNodeVisitor visitor);
    public abstract T Accept<T>(IIRNodeReturningVisitor<T> visitor);
}
