namespace VibeDisasm.DecompilerEngine.IR.Model;

/// <summary>
/// Represents the whole program in IR form.
/// Example: Module with several IRFunctions and global variables
/// </summary>
public sealed class IRProgram
{
    public required IReadOnlyList<IRFunction> Functions { get; init; }
    public required IReadOnlyList<IRVariable> GlobalVariables { get; init; }

    public override string ToString() =>
        (GlobalVariables.Count > 0 ? string.Join("\n", GlobalVariables) + "\n\n" : "") +
        string.Join("\n\n", Functions);
}
