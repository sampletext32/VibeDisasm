using System.Diagnostics;

namespace VibeDisasm.Web.Models.Types;

/// <summary>
/// Function, e.g. void func(int a, int b), int* func()
/// </summary>
[DebuggerDisplay("{DebugDisplay}")]
public sealed class RuntimeFunctionType : RuntimeDatabaseType
{
    public override string Namespace { get; set; }
    public override string Name { get; set; }

    public RuntimeDatabaseType ReturnType { get; }
    public List<FunctionArgument> Arguments { get; }

    public RuntimeFunctionType(
        Guid id,
        string @namespace,
        string name,
        RuntimeDatabaseType returnType,
        List<FunctionArgument> arguments
    ) : base(id)
    {
        Namespace = @namespace;
        Name = name;
        ReturnType = returnType;
        Arguments = arguments;
    }
    public override T Accept<T>(RuntimeDatabaseTypeVisitor<T> visitor) => visitor.VisitFunction(this);

    protected internal override string DebugDisplay =>
        $"{ReturnType.DebugDisplay} {Name}({string.Join(", ", Arguments.Select(x => x.DebugDisplay))})";
}

public class FunctionArgument
{
    public RuntimeDatabaseType Type { get; set; }

    public string Name { get; set; }

    public FunctionArgument(RuntimeDatabaseType type, string name)
    {
        Type = type;
        Name = name;
    }

    public string DebugDisplay => $"{Type.DebugDisplay} {Name}";
}
