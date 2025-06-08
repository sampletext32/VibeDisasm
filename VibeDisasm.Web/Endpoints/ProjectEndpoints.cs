using Microsoft.AspNetCore.Mvc;
using VibeDisasm.Web.Dtos;
using VibeDisasm.Web.Handlers;
using VibeDisasm.Web.Services;

namespace VibeDisasm.Web.Endpoints;

public static class ProjectEndpoints
{
    public static void MapProjectEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/projects")
            .WithTags("Projects");

        group.MapPost("/create", CreateProject)
            .WithName("CreateProject")
            .WithDescription("Creates a new project with the specified title");

        group.MapPost("/open-recent/{recentId:guid}", OpenRecent)
            .WithName("OpenRecent")
            .WithDescription("Opens a project from recent list by its ID");

        group.MapGet("/recent", RecentProjects)
            .WithName("RecentProjects")
            .WithDescription("Returns a list of projects that were recently opened");

        group.MapPost("/save/{projectId:guid}", SaveProject)
            .WithName("SaveProject")
            .WithDescription("Saves the project to an archive");
    }

    /// <summary>
    /// Creates a new project
    /// </summary>
    /// <response code="200">Returns the ID of the created project</response>
    /// <response code="400">If the project data is invalid</response>
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Guid))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(string))]
    private static async Task<IResult> CreateProject(CreateProjectDto request, CreateProjectHandler handler)
    {
        var result = await handler.Handle(request);
        return result.IsSuccess
            ? Results.Ok(result.Value)
            : Results.BadRequest(
                result.Errors.First()
                    .Message
            );
    }

    /// <summary>
    /// Opens a project from the recent list by its ID
    /// </summary>
    /// <response code="200">When open was successful</response>
    /// <response code="400">If the opening failed</response>
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(string))]
    private static async Task<IResult> OpenRecent(OpenRecentHandler handler, Guid recentId)
    {
        var result = await handler.Handle(recentId);
        return result.IsSuccess
            ? Results.Ok()
            : Results.BadRequest(
                result.Errors.First()
                    .Message
            );
    }

    /// <summary>
    /// Returns a list of recently opened projects
    /// </summary>
    /// <response code="200">In theory always 200</response>
    /// <response code="400">In theory, should never happen</response>
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<RecentJsonMetadata>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(string))]
    private static async Task<IResult> RecentProjects(ListRecentsHandler handler)
    {
        var result = await handler.Handle();
        return result.IsSuccess
            ? Results.Ok(result.Value)
            : Results.BadRequest(
                result.Errors.First()
                    .Message
            );
    }

    /// <summary>
    /// Saves the current project to an archive
    /// </summary>
    /// <response code="200">Save was successful</response>
    /// <response code="400">An error occured when saving project</response>
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(string))]
    private static async Task<IResult> SaveProject(SaveProjectHandler handler, Guid projectId)
    {
        var result = await handler.Handle(projectId);
        return result.IsSuccess
            ? Results.Ok()
            : Results.BadRequest(
                result.Errors.First()
                    .Message
            );
    }
}
