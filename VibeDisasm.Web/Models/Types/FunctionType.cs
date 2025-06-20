using System.Diagnostics;

namespace VibeDisasm.Web.Models.Types;

/// <summary>
/// Function, e.g. void func(int a, int b), int* func()
/// </summary>
[DebuggerDisplay("{DebugDisplay}")]
public sealed class FunctionType : DatabaseType
{
    public override string Namespace { get; set; }
    public override string Name { get; set; }

    public TypeRefType ReturnType { get; }
    public List<FunctionArgument> Arguments { get; }

    public FunctionType(
        Guid id,
        string @namespace,
        string name,
        TypeRefType returnType,
        List<FunctionArgument> arguments
    ) : base(id)
    {
        Namespace = @namespace;
        Name = name;
        ReturnType = returnType;
        Arguments = arguments;
    }
    public override T Accept<T>(DatabaseTypeVisitor<T> visitor) => visitor.VisitFunction(this);

    protected internal override string DebugDisplay =>
        $"{ReturnType.DebugDisplay} func({string.Join(", ", Arguments.Select(x => x.DebugDisplay))})";
}

public class FunctionArgument
{
    public TypeRefType Type { get; set; }

    public string Name { get; set; }

    public FunctionArgument(TypeRefType type, string name)
    {
        Type = type;
        Name = name;
    }

    public string DebugDisplay => $"{Type.DebugDisplay} {Name}";
}
