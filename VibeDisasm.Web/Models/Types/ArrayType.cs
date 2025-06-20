using System.Diagnostics;

namespace VibeDisasm.Web.Models.Types;

/// <summary>
/// Array of anything, e.g. int[3], MyStruct[1]
/// </summary>
[DebuggerDisplay("{DebugDisplay}")]
public class ArrayType : DatabaseType
{
    public DatabaseType ElementType { get; set; }
    public int ElementCount { get; set; }

    public ArrayType(Guid id, string @namespace, DatabaseType elementType, int elementCount) : base(id, @namespace, $"{elementType.Name}[{elementCount}]")
    {
        ElementType = elementType;
        ElementCount = elementCount;
    }

    public override T Accept<T>(DatabaseTypeVisitor<T> visitor) => visitor.VisitArray(this);

    protected internal override string DebugDisplay =>  $"{ElementType.DebugDisplay}[{ElementCount}]";
}
