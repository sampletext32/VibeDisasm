using VibeDisasm.Web.Models;

namespace VibeDisasm.Web.Dtos;

public record ProgramDetailsDto(
    Guid Id,
    string Name,
    string FilePath,
    long FileLength,
    ProgramKind Kind,
    ProgramArchitecture Architecture
);
