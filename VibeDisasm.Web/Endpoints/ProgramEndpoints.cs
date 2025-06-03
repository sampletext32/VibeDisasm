using FluentResults;
using Microsoft.AspNetCore.Mvc;
using VibeDisasm.Web.Dtos;
using VibeDisasm.Web.Handlers;

namespace VibeDisasm.Web.Endpoints;

public static class ProgramEndpoints
{
    public static void MapProgramEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/programs").WithTags("Programs");

        group.MapPost("/import", ImportProgram)
            .WithName("ImportProgram")
            .WithDescription("Imports a program into the specified project");

        group.MapGet("/byproject", ListProgramsByProject)
            .WithName("ListProgramsByProject")
            .WithDescription("Returns a list of all programs in the specified project");
    }

    /// <summary>
    /// Imports a program into a project
    /// </summary>
    /// <param name="handler">Program import handler</param>
    /// <returns>The ID of the imported program</returns>
    /// <response code="200">Returns the ID of the imported program</response>
    /// <response code="400">If the project ID is invalid or import fails</response>
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Guid))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(string))]
    private static async Task<IResult> ImportProgram(ImportProgramHandler handler)
    {
        var result = await handler.Handle();
        return result.IsSuccess
            ? Results.Ok(result.Value)
            : Results.BadRequest(result.Errors.First().Message);
    }

    /// <summary>
    /// Lists all programs in a project
    /// </summary>
    /// <param name="projectId">ID of the project to list programs for</param>
    /// <param name="handler">Program listing handler</param>
    /// <returns>A list of programs in the specified project</returns>
    /// <response code="200">Returns the list of programs</response>
    /// <response code="400">If the project ID is missing or invalid</response>
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<ProgramDetailsDto>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(string))]
    private static async Task<IResult> ListProgramsByProject(ListProgramsHandler handler)
    {
        var result = await handler.Handle();
        return result.IsSuccess
            ? Results.Ok(result.Value)
            : Results.BadRequest(result.Errors.First().Message);
    }
}
