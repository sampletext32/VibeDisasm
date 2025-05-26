namespace VibeDisasm.DecompilerEngine.IR.Model;

/// <summary>
/// Represents a type in IR.
/// Example: int, float*, struct S
/// </summary>
public sealed class IRType
{
    public required string Name { get; init; }
    public bool IsPointer { get; init; }
    public int? Size { get; init; }
}
