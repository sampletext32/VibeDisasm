using System.Diagnostics;

namespace VibeDisasm.Web.Models.Types;

/// <summary>
/// Pointer to anything, e.g. void*, int*, MyStruct*
/// </summary>
[DebuggerDisplay("{DebugDisplay}")]
public class PointerType : DatabaseType
{
    public DatabaseType PointedType { get; set; }

    public PointerType(Guid id, string @namespace, string name, DatabaseType pointedType) : base(id, @namespace, name)
    {
        PointedType = pointedType;
    }

    public override T Accept<T>(DatabaseTypeVisitor<T> visitor) => visitor.VisitPointer(this);

    protected internal override string DebugDisplay => $"{PointedType.DebugDisplay}*";
}
