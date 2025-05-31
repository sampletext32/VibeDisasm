using VibeDisasm.DecompilerEngine.IR.Visitors;

namespace VibeDisasm.DecompilerEngine.IR.Model;

/// <summary>
/// Represents a variable in IR.
/// Example: int x -> IRVariable(Name="x", Type=int)
/// </summary>
public class IRVariable : IRNode
{
    public override string ToString() => $"{Type?.Name ?? "var"} {Name}";

    public required string Name { get; init; }
    public required IRType Type { get; init; }
    public bool IsArgument { get; init; }
    public bool IsLocal { get; init; }

    public override void Accept(IIRNodeVisitor visitor) => visitor.VisitVariable(this);

    public override T? Accept<T>(IIRNodeReturningVisitor<T> visitor) where T : default => visitor.VisitVariable(this);
    internal override string DebugDisplay => $"IRVariable(Name={Name}, Type={Type?.Name ?? "unknown"}, IsArgument={IsArgument}, IsLocal={IsLocal})";
}
