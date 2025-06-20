namespace VibeDisasm.Web.ProjectArchive.TypeArchiveJsonElements;

public class FunctionArchiveJsonElement : TypeArchiveJsonElement
{
    public string Name { get; set; } = "";
    public TypeRefJsonElement ReturnType { get; set; } = null!;

    public List<FunctionArgumentJsonElement> Arguments { get; set; } = [];
}
