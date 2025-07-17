using System.Diagnostics;

namespace VibeDisasm.Web.Models.Types;

/// <summary>
/// Primitive type, e.g. int, double, byte etc.
/// </summary>
[DebuggerDisplay("{DebugDisplay}")]
public sealed class RuntimePrimitiveType : RuntimeDatabaseType
{
    public override string Namespace { get; set; }
    public override string Name { get; set; }

    public int Size { get; set; }

    public RuntimePrimitiveType(Guid id, string @namespace, string name, int size) : base(id)
    {
        Namespace = @namespace;
        Name = name;
        Size = size;
    }

    public override T Accept<T>(RuntimeDatabaseTypeVisitor<T> visitor) => visitor.VisitPrimitive(this);

    protected internal override string DebugDisplay => Name;
}
