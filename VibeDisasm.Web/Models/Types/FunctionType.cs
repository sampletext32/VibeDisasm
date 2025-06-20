using System.Diagnostics;

namespace VibeDisasm.Web.Models.Types;

/// <summary>
/// Function, e.g. void func(int a, int b), int* func()
/// </summary>
[DebuggerDisplay("{DebugDisplay}")]
public class FunctionType : DatabaseType
{
    public DatabaseType ReturnType { get; }
    public List<FunctionArgument> Arguments { get; }

    public FunctionType(
        Guid id,
        string @namespace,
        string name,
        DatabaseType returnType,
        List<FunctionArgument> arguments
    ) : base(id, @namespace, name)
    {
        ReturnType = returnType;
        Arguments = arguments;
    }

    public override T Accept<T>(DatabaseTypeVisitor<T> visitor) => visitor.VisitFunction(this);

    protected internal override string DebugDisplay =>
        $"{ReturnType.DebugDisplay} func({string.Join(", ", Arguments.Select(x => x.DebugDisplay))})";
}

public class FunctionArgument
{
    public DatabaseType Type { get; set; }

    public string Name { get; set; }

    public FunctionArgument(DatabaseType type, string name)
    {
        Type = type;
        Name = name;
    }

    public string DebugDisplay => $"{Type.DebugDisplay} {Name}";
}
