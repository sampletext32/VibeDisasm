using System.Diagnostics;

namespace VibeDisasm.Web.Models.Types;

/// <summary>
/// Pointer to anything, e.g. void*, int*, MyStruct*
/// </summary>
[DebuggerDisplay("{DebugDisplay}")]
public sealed class PointerType : DatabaseType
{
    public override string Namespace { get; set; }
    public override string Name { get; set; }

    public TypeRefType PointedType { get; set; }

    public PointerType(Guid id, string @namespace, TypeRefType pointedType) : base(id)
    {
        Namespace = @namespace;
        PointedType = pointedType;
        Name = $"ptr_to_{PointedType.Name}";
    }
    public override T Accept<T>(DatabaseTypeVisitor<T> visitor) => visitor.VisitPointer(this);

    protected internal override string DebugDisplay => $"{PointedType.DebugDisplay}*";
}
