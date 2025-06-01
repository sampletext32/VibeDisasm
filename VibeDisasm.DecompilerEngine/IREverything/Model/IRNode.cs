using VibeDisasm.DecompilerEngine.IREverything.Visitors;

namespace VibeDisasm.DecompilerEngine.IREverything.Model;

/// <summary>
/// Base class for all IR types. Only needed for generics and polymorphism.
/// </summary>
public abstract class IRNode
{
    public abstract void Accept(IIRNodeVisitor visitor);
    public abstract T? Accept<T>(IIRNodeReturningVisitor<T> visitor);

    internal abstract string DebugDisplay { get; }
}
