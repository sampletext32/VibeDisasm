using System.Diagnostics;

namespace VibeDisasm.Web.Models.Types;

/// <summary>
/// Structure type, that can be declared or used in the program, e.g. _IMAGE_FILE_HEADER, MyStructure, vtable
/// </summary>
[DebuggerDisplay("{DebugDisplay}")]
public sealed class RuntimeStructureType : RuntimeDatabaseType
{
    public override string Namespace { get; set; }
    public override string Name { get; set; }

    public List<RuntimeStructureTypeField> Fields { get; set; }

    public RuntimeStructureType(Guid id, string @namespace, string name, List<RuntimeStructureTypeField> fields) : base(id)
    {
        Namespace = @namespace;
        Name = name;
        Fields = fields;
    }
    public override T Accept<T>(RuntimeDatabaseTypeVisitor<T> visitor) => visitor.VisitStruct(this);

    protected internal override string DebugDisplay => $"struct {Name} {{ {Fields.Count} fields }}";
}

[DebuggerDisplay("{DebugDisplay}")]
public class RuntimeStructureTypeField
{
    public RuntimeTypeRefType Type { get; set; }

    public string Name { get; set; }

    public RuntimeStructureTypeField(RuntimeTypeRefType type, string name)
    {
        Type = type;
        Name = name;
    }

    protected internal string DebugDisplay => $"{Type.DebugDisplay} {Name}";
}
