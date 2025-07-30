using VibeDisasm.Web.Models.TypeInterpretation;

namespace VibeDisasm.Web.Models.Types;

public interface IRuntimeDatabaseType
{
    string Name { get; }
    int Size { get; }

    IInterpret DefaultInterpreter { get; }

    IEnumerable<IInterpret> Interpreters { get; }

    IInterpret? InterpreterOverride { get; }

    IRuntimeDatabaseType WithInterpreterOverride(IInterpret interpreter);
}

public interface ISetSize
{
    public void SetSize(int size);
}

/// <summary>
/// Any type, that can be declared or used in the program, e.g. int, word, void*, MyStructure etc.
/// </summary>
public abstract class RuntimeDatabaseType : IRuntimeDatabaseType
{
    public virtual IInterpret DefaultInterpreter => InterpretBase.AsRaw();
    public IInterpret? InterpreterOverride { get; private set; }

    public Guid Id { get; set; }

    public abstract string Namespace { get; set; }

    public abstract string Name { get; set; }
    public int Size { get; private set; }


    private readonly List<IInterpret> _interpreters = [];
    public IReadOnlyList<IInterpret> Interpreters => _interpreters;

    IEnumerable<IInterpret> IRuntimeDatabaseType.Interpreters => [.._interpreters, DefaultInterpreter];

    protected RuntimeDatabaseType(Guid id)
    {
        Id = id;
        Size = 0;
    }

    protected void SetSize(int size)
    {
        Size = size;
    }

    public RuntimeDatabaseType WithInterpreter(IInterpret interpreter)
    {
        _interpreters.Add(interpreter);
        return this;
    }

    public IRuntimeDatabaseType WithInterpreterOverride(IInterpret interpreter)
    {
        InterpreterOverride = interpreter;
        return this;
    }

    public abstract T Accept<T>(RuntimeDatabaseTypeVisitor<T> visitor);

    protected internal abstract string DebugDisplay { get; }
}
