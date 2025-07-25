namespace VibeDisasm.Web.Dtos;

public record TypeArchiveListItem(
    string Namespace,
    string? AbsoluteFilePath,
    int TypeCount
);
