using System.Diagnostics;

namespace VibeDisasm.Web.Models.Types;

/// <summary>
/// Primitive type, e.g. int, double, byte etc.
/// </summary>
[DebuggerDisplay("{DebugDisplay}")]
public class PrimitiveType : DatabaseType
{
    public PrimitiveType(Guid id, string @namespace, string name) : base(id, @namespace, name)
    {
    }

    public override T Accept<T>(DatabaseTypeVisitor<T> visitor) => visitor.VisitPrimitive(this);

    protected internal override string DebugDisplay => Name;
}
