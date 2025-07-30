using System.Diagnostics;
using VibeDisasm.Web.Models.TypeInterpretation;

namespace VibeDisasm.Web.Models.Types;

/// <summary>
/// Pointer to anything, e.g. void*, int*, MyStruct*
/// </summary>
[DebuggerDisplay("{DebugDisplay}")]
public sealed class RuntimePointerType : RuntimeDatabaseType, ISetSize
{
    public override string Namespace { get; set; }
    public override string Name { get; set; }

    public RuntimeDatabaseType PointedType { get; set; }

    public RuntimePointerType(Guid id, string @namespace, RuntimeDatabaseType pointedType, int size) : base(id)
    {
        Namespace = @namespace;
        PointedType = pointedType;
        Name = $"ptr_to_{PointedType.Name}";
        SetSize(size);
    }
    public override T Accept<T>(RuntimeDatabaseTypeVisitor<T> visitor) => visitor.VisitPointer(this);

    protected internal override string DebugDisplay => $"{PointedType.DebugDisplay}*";

    public new void SetSize(int size) => base.SetSize(size);
    void ISetSize.SetSize(int size) => base.SetSize(size);
}
