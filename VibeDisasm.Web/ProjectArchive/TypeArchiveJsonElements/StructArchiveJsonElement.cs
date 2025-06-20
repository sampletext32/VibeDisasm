namespace VibeDisasm.Web.ProjectArchive.TypeArchiveJsonElements;

public class StructArchiveJsonElement : TypeArchiveJsonElement
{
    public string Name { get; set; } = "";
    public List<StructFieldArchiveJsonElement> Fields { get; set; } = [];
}
