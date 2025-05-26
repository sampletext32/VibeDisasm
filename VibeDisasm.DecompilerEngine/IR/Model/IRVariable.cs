namespace VibeDisasm.DecompilerEngine.IR.Model;

/// <summary>
/// Represents a variable in IR.
/// Example: int x -> IRVariable(Name="x", Type=int)
/// </summary>
public class IRVariable
{
    public required string Name { get; init; }
    public required IRType Type { get; init; }
    public bool IsArgument { get; init; }
    public bool IsLocal { get; init; }
}
