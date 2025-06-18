namespace VibeDisasm.Web.ProjectArchive;

public record ProjectArchiveProgramMetadata(Guid Id, string Name, string FilePath, long FileLength)
{
    public static ProjectArchiveProgramMetadata FromUserProgram(Models.UserRuntimeProgram program) =>
        new(program.Id, program.Name, program.FilePath, program.FileLength);

    public Models.UserRuntimeProgram ToUserProgram() =>
        new(Id, FilePath, Name, FileLength);
}
