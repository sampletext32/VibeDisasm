namespace VibeDisasm.Web.Models.Types;

/// <summary>
/// Array of anything, e.g. int[3], MyStruct[1]
/// </summary>
public class ArrayType : DatabaseType
{
    public DatabaseType ElementType { get; set; }
    public int ElementCount { get; set; }

    public ArrayType(DatabaseType elementType, int elementCount) : base(elementType.Size * elementCount)
    {
        ElementType = elementType;
        ElementCount = elementCount;
    }

    public override ArrayType AsReadonly()
    {
        MakeReadonly();
        return this;
    }
}
