using Microsoft.AspNetCore.Mvc;
using VibeDisasm.Web.Dtos;
using VibeDisasm.Web.Handlers;

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

        group.MapPost("/open/{projectId:guid}", OpenProject)
            .WithName("OpenProject")
            .WithDescription("Opens an existing project by its ID");

        group.MapGet("/list", ListProjects)
            .WithName("ListProjects")
            .WithDescription("Returns a list of all projects");
    }

    /// <summary>
    /// Creates a new project
    /// </summary>
    /// <param name="request">Project creation data</param>
    /// <param name="handler">Project creation handler</param>
    /// <returns>The ID of the created project</returns>
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
    /// Creates a new project
    /// </summary>
    /// <param name="handler">Project open handler</param>
    /// <param name="projectId">Project id to open</param>
    /// <response code="200">When open was successful</response>
    /// <response code="400">If the opening failed</response>
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(string))]
    private static async Task<IResult> OpenProject(OpenProjectHandler handler, Guid projectId)
    {
        var result = await handler.Handle(projectId);
        return result.IsSuccess
            ? Results.Ok()
            : Results.BadRequest(
                result.Errors.First()
                    .Message
            );
    }

    /// <summary>
    /// Lists all projects
    /// </summary>
    /// <param name="handler">Project listing handler</param>
    /// <returns>A list of all projects</returns>
    /// <response code="200">Returns a list of all projects</response>
    /// <response code="400">If an error occurs while retrieving projects</response>
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<ProjectDetailsDto>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(string))]
    private static async Task<IResult> ListProjects(ListProjectsHandler handler)
    {
        var result = await handler.Handle();
        return result.IsSuccess
            ? Results.Ok(result.Value)
            : Results.BadRequest(
                result.Errors.First()
                    .Message
            );
    }
}
