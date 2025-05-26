namespace VibeDisasm.DecompilerEngine.IR.Model;

/// <summary>
/// Represents a function in IR form.
/// Example: int add(int a, int b) { return a + b; } -> IRFunction(Name="add", ...)
/// </summary>
public class IRFunction
{
    public required string Name { get; init; }
    public required IRType ReturnType { get; init; }
    public required IReadOnlyList<IRVariable> Parameters { get; init; }
    public required IReadOnlyList<IRBlock> Blocks { get; init; }
}
