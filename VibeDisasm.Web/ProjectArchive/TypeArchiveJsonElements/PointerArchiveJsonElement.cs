namespace VibeDisasm.Web.ProjectArchive.TypeArchiveJsonElements;

/// <summary>
/// Pointer type can point to anything, even to other archive types
/// </summary>
public class PointerArchiveJsonElement : TypeArchiveJsonElement
{
    public TypeRefJsonElement PointedType { get; set; } = null!;
}
