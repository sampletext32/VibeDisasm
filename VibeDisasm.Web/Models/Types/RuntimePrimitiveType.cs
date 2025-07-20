using System.Diagnostics;
using VibeDisasm.Web.Models.TypeInterpretation;

namespace VibeDisasm.Web.Models.Types;

/// <summary>
/// Primitive type, e.g. int, double, byte etc.
/// </summary>
[DebuggerDisplay("{DebugDisplay}")]
public sealed class RuntimePrimitiveType : RuntimeDatabaseType
{
    public override string Namespace { get; set; }
    public override string Name { get; set; }

    public int Size { get; set; }

    public RuntimePrimitiveType(Guid id, string @namespace, string name, int size, InterpretAs interpretAs = InterpretAs.Bytes) : base(id)
    {
        Namespace = @namespace;
        Name = name;
        Size = size;
        InterpretAs = interpretAs;
    }

    public override T Accept<T>(RuntimeDatabaseTypeVisitor<T> visitor) => visitor.VisitPrimitive(this);

    protected internal override string DebugDisplay => Name;
}
