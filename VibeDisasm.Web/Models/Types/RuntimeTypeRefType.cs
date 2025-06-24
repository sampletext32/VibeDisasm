using System.Diagnostics;

namespace VibeDisasm.Web.Models.Types;

/// <summary>
/// A type representing a reference to another type
/// </summary>
[DebuggerDisplay("{DebugDisplay}")]
public sealed class RuntimeTypeRefType : RuntimeDatabaseType
{
    public override string Namespace { get; set; }

    public override string Name { get; set; }

    public RuntimeTypeRefType(Guid id, string @namespace) : base(id)
    {
        Namespace = @namespace;
    }

    public override T Accept<T>(RuntimeDatabaseTypeVisitor<T> visitor) => visitor.VisitRef(this);

    protected internal override string DebugDisplay => $"ref_{Id}_in_{Namespace}";
}
