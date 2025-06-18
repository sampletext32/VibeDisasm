using System.Diagnostics;

namespace VibeDisasm.Web.Models.Types;

/// <summary>
/// Function, e.g. void func(int a, int b), int* func()
/// </summary>
[DebuggerDisplay("{DebugDisplay}")]
public class FunctionType : DatabaseType
{
    public DatabaseType ReturnType { get; }
    public List<DatabaseType> ParameterTypes { get; }

    public FunctionType(DatabaseType returnType, List<DatabaseType> parameterTypes)
        : base(4)
    {
        ReturnType = returnType;

        ParameterTypes = parameterTypes;
    }

    public override string Semantic =>
        $"{ReturnType.Semantic} func({string.Join(", ", ParameterTypes.Select(x => x.Semantic))})";

    public override FunctionType AsReadonly()
    {
        MakeReadonly();
        return this;
    }

    protected internal override string DebugDisplay =>  $"{ReturnType.DebugDisplay} func({string.Join(", ", ParameterTypes.Select(x => x.DebugDisplay))})";
}
