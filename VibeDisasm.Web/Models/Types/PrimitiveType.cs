using System.Diagnostics;

namespace VibeDisasm.Web.Models.Types;

/// <summary>
/// Primitive type, e.g. int, double, byte etc.
/// </summary>
[DebuggerDisplay("{DebugDisplay}")]
public sealed class PrimitiveType : DatabaseType
{
    public override string Namespace { get; set; }
    public override string Name { get; set; }

    public PrimitiveType(Guid id, string @namespace, string name) : base(id)
    {
        Namespace = @namespace;
        Name = name;
    }

    public override T Accept<T>(DatabaseTypeVisitor<T> visitor) => visitor.VisitPrimitive(this);

    protected internal override string DebugDisplay => Name;
}
