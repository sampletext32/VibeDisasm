namespace VibeDisasm.Web.ProjectArchive.TypeArchiveJsonElements;

public class FunctionArchiveJsonElement : TypeArchiveJsonElement
{
    public Guid ReturnTypeId { get; set; }

    public List<FunctionArgumentJsonElement> Arguments { get; set; } = [];
}