using System.Diagnostics;
using VibeDisasm.Web.Models.TypeInterpretation;

namespace VibeDisasm.Web.Models.Types;

/// <summary>
/// Array of anything, e.g. int[3], MyStruct[1]
/// </summary>
[DebuggerDisplay("{DebugDisplay}")]
public sealed class RuntimeArrayType : RuntimeDatabaseType
{
    public override string Namespace { get; set; }
    public override string Name { get; set; }

    public RuntimeDatabaseType ElementType { get; set; }
    public int ElementCount { get; set; }

    public RuntimeArrayType(Guid id, string @namespace, RuntimeDatabaseType elementType, int elementCount) : base(id)
    {
        Namespace = @namespace;
        Name = $"{elementType.Name}[{elementCount}]";
        ElementType = elementType;
        ElementCount = elementCount;
        SetSize(elementType.Size * elementCount);
    }

    public override T Accept<T>(RuntimeDatabaseTypeVisitor<T> visitor) => visitor.VisitArray(this);

    protected internal override string DebugDisplay => $"{ElementType.DebugDisplay}[{ElementCount}]";
}
