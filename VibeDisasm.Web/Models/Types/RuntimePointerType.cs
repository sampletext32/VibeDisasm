using System.Diagnostics;

namespace VibeDisasm.Web.Models.Types;

/// <summary>
/// Pointer to anything, e.g. void*, int*, MyStruct*
/// </summary>
[DebuggerDisplay("{DebugDisplay}")]
public sealed class RuntimePointerType : RuntimeDatabaseType
{
    public override string Namespace { get; set; }
    public override string Name { get; set; }

    public RuntimeTypeRefType PointedType { get; set; }

    public RuntimePointerType(Guid id, string @namespace, RuntimeTypeRefType pointedType) : base(id)
    {
        Namespace = @namespace;
        PointedType = pointedType;
        Name = $"ptr_to_{PointedType.Name}";
    }
    public override T Accept<T>(RuntimeDatabaseTypeVisitor<T> visitor) => visitor.VisitPointer(this);

    protected internal override string DebugDisplay => $"{PointedType.DebugDisplay}*";
}
