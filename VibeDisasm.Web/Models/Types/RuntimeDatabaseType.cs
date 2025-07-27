using VibeDisasm.Web.Models.TypeInterpretation;

namespace VibeDisasm.Web.Models.Types;

/// <summary>
/// Any type, that can be declared or used in the program, e.g. int, word, void*, MyStructure etc.
/// </summary>
public abstract class RuntimeDatabaseType
{
    public Guid Id { get; set; }

    public abstract string Namespace { get; set; }

    public abstract string Name { get; set; }

    public InterpretAs InterpretAs { get; set; }

    public string FullName => $"{Namespace}::{Name}";

    protected RuntimeDatabaseType(Guid id)
    {
        Id = id;
    }

    public abstract T Accept<T>(RuntimeDatabaseTypeVisitor<T> visitor);

    protected internal abstract string DebugDisplay { get; }
}
