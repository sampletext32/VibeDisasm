using System.Diagnostics;

namespace VibeDisasm.Web.Models.Types;

/// <summary>
/// Primitive type, e.g. int, double, byte etc.
/// </summary>
[DebuggerDisplay("{DebugDisplay}")]
public class PrimitiveType : DatabaseType
{
    public string Name { get; set; }

    public PrimitiveType(int size, string name) : base(size)
    {
        Name = name;
    }

    public override string Semantic => Name;

    public override PrimitiveType AsReadonly()
    {
        MakeReadonly();
        return this;
    }

    protected internal override string DebugDisplay => Name;
}
