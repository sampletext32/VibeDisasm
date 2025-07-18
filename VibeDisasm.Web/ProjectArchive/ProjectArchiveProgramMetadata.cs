using VibeDisasm.Web.Models;

namespace VibeDisasm.Web.ProjectArchive;

public record ProjectArchiveProgramMetadata(
    Guid Id,
    string Name,
    string FilePath,
    long FileLength,
    ProgramKind Kind,
    ProgramArchitecture Architecture,
    List<string> TypeArchivePaths
)
{
    public static ProjectArchiveProgramMetadata FromUserProgram(Models.RuntimeUserProgram program) =>
        new(
            program.Id,
            program.Name,
            program.FilePath,
            program.FileLength,
            program.Kind,
            program.Architecture,
            program.Database.TypeStorage.Archives
                .Select(x => x.AbsoluteFilePath ?? throw new InvalidOperationException(
                    $"When attempting to save program to archive, type archive {x.Namespace} didn't have absolute file path. User needs to save it first."))
                .ToList()
        );

    public Models.RuntimeUserProgram ToUserProgram() =>
        new(Id, FilePath, Name, FileLength)
        {
            Kind = Kind,
            Architecture = Architecture
        };
}
