using System.Diagnostics;
using VibeDisasm.Web.Models.TypeInterpretation;

namespace VibeDisasm.Web.Models.Types;

/// <summary>
/// Structure type, that can be declared or used in the program, e.g. _IMAGE_FILE_HEADER, MyStructure, vtable
/// </summary>
[DebuggerDisplay("{DebugDisplay}")]
public sealed class RuntimeStructureType : RuntimeDatabaseType
{
    public override IInterpret DefaultInterpreter => InterpretBase.AsStruct();

    public override string Namespace { get; set; }
    public override string Name { get; set; }

    public List<RuntimeStructureTypeField> Fields { get; }

    public RuntimeStructureType(
        Guid id,
        string @namespace,
        string name,
        List<IRuntimeDatabaseType> fields
    ) : base(id)
    {
        Namespace = @namespace;
        Name = name;

        if (fields.Any(x => x is not RuntimeStructureTypeField))
        {
            throw new ArgumentException("All fields must be of type RuntimeStructureTypeField", nameof(fields));
        }

        Fields = fields.OfType<RuntimeStructureTypeField>().ToList();
        SetSize(Fields.Sum(x => x.Size));
    }
    public RuntimeStructureType(
        Guid id,
        string @namespace,
        string name,
        List<RuntimeStructureTypeField> fields
    ) : base(id)
    {
        Namespace = @namespace;
        Name = name;

        Fields = fields;
        SetSize(Fields.Sum(x => x.Size));
    }

    public override T Accept<T>(RuntimeDatabaseTypeVisitor<T> visitor) => visitor.VisitStruct(this);

    protected internal override string DebugDisplay => $"struct {Name} {{ {Fields.Count} fields }}";
}

[DebuggerDisplay("{DebugDisplay}")]
public class RuntimeStructureTypeField(RuntimeDatabaseType type, string name) : IRuntimeDatabaseType
{
    public RuntimeDatabaseType Type { get; set; } = type;

    public string Name { get; set; } = name;
    public int Size => Type.Size;
    public IInterpret DefaultInterpreter => Type.DefaultInterpreter;
    public IEnumerable<IInterpret> Interpreters => Type.Interpreters;
    public IInterpret? InterpreterOverride { get; private set; }

    public IRuntimeDatabaseType WithInterpreterOverride(IInterpret interpreter)
    {
        InterpreterOverride = interpreter;
        return this;
    }

    protected internal string DebugDisplay => $"{Type.DebugDisplay} {Name}";
}
