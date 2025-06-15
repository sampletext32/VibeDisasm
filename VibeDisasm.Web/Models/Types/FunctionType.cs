namespace VibeDisasm.Web.Models.Types;

/// <summary>
/// Function, e.g. void func(int a, int b), int* func()
/// </summary>
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

    public override FunctionType AsReadonly()
    {
        MakeReadonly();
        return this;
    }
}
