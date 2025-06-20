using System.Diagnostics;

namespace VibeDisasm.Web.Models.Types;

/// <summary>
/// Structure type, that can be declared or used in the program, e.g. _IMAGE_FILE_HEADER, MyStructure, vtable
/// </summary>
[DebuggerDisplay("{DebugDisplay}")]
public class StructureType : DatabaseType
{
    public List<StructureTypeField> Fields { get; set; }

    public StructureType(Guid id, string @namespace, string name, List<StructureTypeField> fields) : base(id, @namespace, name)
    {
        Fields = fields;
    }

    public override T Accept<T>(DatabaseTypeVisitor<T> visitor) => visitor.VisitStruct(this);

    protected internal override string DebugDisplay => $"struct {Name} {{ {Fields.Count} fields }}";
}

[DebuggerDisplay("{DebugDisplay}")]
public class StructureTypeField
{
    public DatabaseType Type { get; set; }

    public string Name { get; set; }

    public StructureTypeField(DatabaseType type, string name)
    {
        Type = type;
        Name = name;
    }

    protected internal string DebugDisplay => $"{Type.DebugDisplay} {Name}";
}
