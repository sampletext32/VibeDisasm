namespace VibeDisasm.Web.ProjectArchive.TypeArchiveJsonElements;

public class ArrayArchiveJsonElement : TypeArchiveJsonElement
{
    public string Name { get; set; } = "";

    public TypeRefJsonElement ElementType { get; set; } = null!;

    public int ElementCount { get; set; }
}
