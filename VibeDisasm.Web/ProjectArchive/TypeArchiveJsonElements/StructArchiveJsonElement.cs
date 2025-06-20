namespace VibeDisasm.Web.ProjectArchive.TypeArchiveJsonElements;

public class StructArchiveJsonElement : TypeArchiveJsonElement
{
    public List<StructFieldArchiveJsonElement> Fields { get; set; } = [];
}