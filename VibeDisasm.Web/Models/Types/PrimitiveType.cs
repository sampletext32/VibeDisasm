namespace VibeDisasm.Web.Models.Types;

/// <summary>
/// Primitive type, e.g. int, double, byte etc.
/// </summary>
public class PrimitiveType : DatabaseType
{
    public string Name { get; set; }

    public PrimitiveType(int size, string name) : base(size)
    {
        Name = name;
    }

    public override PrimitiveType AsReadonly()
    {
        MakeReadonly();
        return this;
    }
}
