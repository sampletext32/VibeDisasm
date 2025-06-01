using VibeDisasm.DecompilerEngine.IREverything.Visitors;

namespace VibeDisasm.DecompilerEngine.IREverything.Model;

/// <summary>
/// Represents the whole program in IR form.
/// Example: Module with several IRFunctions and global variables
/// </summary>
public sealed class IRProgram : IRNode
{
    public required IReadOnlyList<IRFunction> Functions { get; init; }
    public required IReadOnlyList<IRVariable> GlobalVariables { get; init; }

    public override string ToString() =>
        (GlobalVariables.Count > 0
            ? string.Join("\n", GlobalVariables) + "\n\n"
            : "") +
        string.Join("\n\n", Functions);

    public override void Accept(IIRNodeVisitor visitor) => visitor.VisitProgram(this);

    public override T? Accept<T>(IIRNodeReturningVisitor<T> visitor) where T : default => visitor.VisitProgram(this);
    internal override string DebugDisplay => $"IRProgram(Functions: {Functions.Count}, Globals: {GlobalVariables.Count})";
}
