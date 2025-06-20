namespace VibeDisasm.Web.ProjectArchive;

public record ProjectArchiveMetadata(
    Guid ProjectId,
    string Title,
    DateTime CreatedAt
);
