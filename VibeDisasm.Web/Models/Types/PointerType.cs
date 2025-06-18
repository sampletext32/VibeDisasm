using System.Diagnostics;

namespace VibeDisasm.Web.Models.Types;

/// <summary>
/// Pointer to anything, e.g. void*, int*, MyStruct*
/// </summary>
[DebuggerDisplay("{DebugDisplay}")]
public class PointerType : DatabaseType
{
    public DatabaseType PointedType { get; set; }

    public PointerType(DatabaseType pointedType, int size) : base(size)
    {
        PointedType = pointedType;
    }

    public override string Semantic => $"{PointedType.Semantic}*";

    public override PointerType AsReadonly()
    {
        MakeReadonly();
        return this;
    }

    protected internal override string DebugDisplay => $"{PointedType.DebugDisplay}*";
}
