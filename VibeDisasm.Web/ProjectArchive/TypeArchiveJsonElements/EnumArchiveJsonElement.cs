namespace VibeDisasm.Web.ProjectArchive.TypeArchiveJsonElements;

public class EnumArchiveJsonElement : TypeArchiveJsonElement
{
    public string Name { get; set; } = "";
    public TypeRefJsonElement UnderlyingType { get; set; } = null!;
    public List<EnumMemberJsonElement> Members { get; set; } = [];
}
