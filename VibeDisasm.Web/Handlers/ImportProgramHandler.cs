using FluentResults;
using VibeDisasm.Web.Dtos;
using VibeDisasm.Web.Models;

namespace VibeDisasm.Web.Handlers;

public class ImportProgramHandler(AppState state)
{
    public Task<Result<Guid>> Handle(ImportProgramDto request)
    {
        var project = state.Projects.FirstOrDefault(x => x.Id == request.ProjectId);
        if (project is null)
        {
            return Task.FromResult(Result.Fail<Guid>($"Project with ID {request.ProjectId} not found"));
        }

        const string filePath = @"C:\Projects\CSharp\VibeDisasm\VibeDisasm.TestLand\DLLs\iron_3d.exe";
        var fileData = File.ReadAllBytes(filePath);

        var program = new UserProgram(Guid.NewGuid(), filePath, fileData);
        project.Programs.Add(program);

        return Task.FromResult(Result.Ok(program.Id));
    }
}
