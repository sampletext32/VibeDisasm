using VibeDisasm.Web.ProjectArchive.TypeArchiveJsonElements;

namespace VibeDisasm.Web.ProjectArchive;

public record TypeArchiveJson(
    string Namespace,
    List<TypeArchiveJsonElement> Types
);
