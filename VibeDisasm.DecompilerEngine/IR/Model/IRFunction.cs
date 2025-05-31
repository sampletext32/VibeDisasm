using VibeDisasm.DecompilerEngine.IR.Visitors;

namespace VibeDisasm.DecompilerEngine.IR.Model;

/// <summary>
/// Represents a function in IR form.
/// Example: int add(int a, int b) { return a + b; } -> IRFunction(Name="add", ...)
/// </summary>
public class IRFunction : IRNode
{
    public  string Name { get; init; }
    public  IRType ReturnType { get; init; }
    public  List<IRVariable> Parameters { get; init; }
    public  List<IRBlock> Blocks { get; init; }

    public IRFunction(string name, IRType returnType, List<IRVariable> parameters, List<IRBlock> blocks)
    {
        Name = name;
        ReturnType = returnType;
        Parameters = parameters;
        Blocks = blocks;
    }

    public override string ToString() => $"{ReturnType.Name} {Name}({string.Join(", ", Parameters.Select(p => p.ToString()))})\n" + string.Join("\n\n", Blocks);

    public override void Accept(IIRNodeVisitor visitor)
    {
        visitor.Visit(this);
    }

    public override T Accept<T>(IIRNodeReturningVisitor<T> visitor) => visitor.Visit(this);
}
