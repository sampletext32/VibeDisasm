namespace VibeDisasm.Web.Models.Types;

/// <summary>
/// A type representing a reference to another type
/// </summary>
public sealed class TypeRefType : DatabaseType
{
    public override string Namespace { get; set; }

    [Obsolete]
    public override string Name
    {
        get => throw new InvalidOperationException("Get Name property of TypeRefType is prohibited.");
        set => throw new InvalidOperationException("Set Name property of TypeRefType is prohibited.");
    }

    public TypeRefType(Guid id, string @namespace) : base(id)
    {
        Namespace = @namespace;
    }

    public override T Accept<T>(DatabaseTypeVisitor<T> visitor) => visitor.VisitRef(this);

    protected internal override string DebugDisplay => $"ref_{Id}_in_{Namespace}";
}
