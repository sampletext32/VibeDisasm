using System.Diagnostics;

namespace VibeDisasm.Web.Models.Types;

/// <summary>
/// Array of anything, e.g. int[3], MyStruct[1]
/// </summary>
[DebuggerDisplay("{DebugDisplay}")]
public sealed class ArrayType : DatabaseType
{
    public override string Namespace { get; set; }
    public override string Name { get; set; }

    public TypeRefType ElementType { get; set; }
    public int ElementCount { get; set; }

    public ArrayType(Guid id, string @namespace, TypeRefType elementType, int elementCount) : base(id)
    {
        Namespace = @namespace;
        Name = $"{elementType.Name}[{elementCount}]";
        ElementType = elementType;
        ElementCount = elementCount;
    }

    public override T Accept<T>(DatabaseTypeVisitor<T> visitor) => visitor.VisitArray(this);

    protected internal override string DebugDisplay =>  $"{ElementType.DebugDisplay}[{ElementCount}]";
}
