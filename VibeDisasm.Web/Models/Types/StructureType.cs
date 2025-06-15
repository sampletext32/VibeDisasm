namespace VibeDisasm.Web.Models.Types;

/// <summary>
/// Structure type, that can be declared or used in the program, e.g. _IMAGE_FILE_HEADER, MyStructure, vtable
/// </summary>
public class StructureType : DatabaseType
{
    public string Name { get; set; }

    public List<StructureTypeField> Fields { get; set; }

    public StructureType(string name, List<StructureTypeField> fields) : base(fields.Sum(x => x.Type.Size))
    {
        Name = name;
    }

    public override StructureType AsReadonly()
    {
        MakeReadonly();
        return this;
    }
}

public class StructureTypeField
{
    public DatabaseType Type { get; set; }

    public string Name { get; set; }

    public StructureTypeField(DatabaseType type, string name)
    {
        Type = type;
        Name = name;
    }
}
