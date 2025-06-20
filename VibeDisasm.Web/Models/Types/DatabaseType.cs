namespace VibeDisasm.Web.Models.Types;

/// <summary>
/// Any type, that can be declared or used in the program, e.g. int, word, void*, MyStructure etc.
/// </summary>
public abstract class DatabaseType
{
    public Guid Id { get; set; }

    public abstract string Namespace { get; set; }

    public abstract string Name { get; set; }

    public string FullName => $"{Namespace}::{Name}";

    protected DatabaseType(Guid id)
    {
        Id = id;
    }

    public TypeRefType MakeRef() => new(Id, Namespace);

    public abstract T Accept<T>(DatabaseTypeVisitor<T> visitor);

    protected internal abstract string DebugDisplay { get; }
}
