namespace VibeDisasm.Web.Models.Types;

/// <summary>
/// Pointer to anything, e.g. void*, int*, MyStruct*
/// </summary>
public class PointerType : DatabaseType
{
    public DatabaseType PointedType { get; set; }

    public PointerType(DatabaseType pointedType, int size) : base(size)
    {
        PointedType = pointedType;
    }

    public override PointerType AsReadonly()
    {
        MakeReadonly();
        return this;
    }
}
