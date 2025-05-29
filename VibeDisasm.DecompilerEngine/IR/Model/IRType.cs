namespace VibeDisasm.DecompilerEngine.IR.Model;

/// <summary>
/// Represents a type in IR.
/// Example: int, float*, struct S
/// </summary>
public sealed class IRType
{
    public string Name { get; init; }

    private IRType(string name)
    {
        Name = name;
    }

    public static IRType Int => new IRType("number");
    public static IRType Uint => new IRType("uint");
    public static IRType Long => new IRType("long");
    public static IRType Ulong => new IRType("ulong");
    public static IRType Bool => new IRType("bool");
}
