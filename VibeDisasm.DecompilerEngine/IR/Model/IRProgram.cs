using VibeDisasm.DecompilerEngine.IR.Visitors;

namespace VibeDisasm.DecompilerEngine.IR.Model;

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

    public override void Accept(IIRNodeVisitor visitor)
    {
        visitor.Visit(this);
    }

    public override T Accept<T>(IIRNodeReturningVisitor<T> visitor) => visitor.Visit(this);
}
