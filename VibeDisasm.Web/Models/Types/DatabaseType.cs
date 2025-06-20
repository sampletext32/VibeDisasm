namespace VibeDisasm.Web.Models.Types;

/// <summary>
/// Any type, that can be declared or used in the program, e.g. int, word, void*, MyStructure etc.
/// </summary>
public abstract class DatabaseType
{
    public Guid Id { get; set; }

    public string Namespace { get; set; }

    public string Name { get; set; }

    public string FullName => $"{Namespace}::{Name}";

    protected DatabaseType(Guid id, string @namespace, string name)
    {
        Id = id;
        Namespace = @namespace;
        Name = name;
    }

    public abstract T Accept<T>(DatabaseTypeVisitor<T> visitor);

    protected internal abstract string DebugDisplay { get; }
}
