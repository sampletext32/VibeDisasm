using Microsoft.AspNetCore.Mvc;
using VibeDisasm.Web.Handlers;

namespace VibeDisasm.Web.Endpoints;

/// <summary>
/// Endpoints that are to be moved to some group later
/// </summary>
public static class AEndpoints
{
    public static void MapGeneralEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/general/{projectId:guid}/{programId:guid}").WithTags("General");

        group.MapGet("/launch-analysis", LaunchBinaryAnalysis)
            .WithName("LaunchBinaryAnalysis")
            .WithDescription("Launches binary analysis on specified program");
    }

    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(string))]
    private static async Task<IResult> LaunchBinaryAnalysis(
        LaunchBinaryAnalysisHandler handler,
        Guid projectId, Guid programId)
    {
        var result = await handler.Handle(projectId, programId);
        return result.IsSuccess
            ? Results.Ok(result.Value)
            : Results.BadRequest(result.Errors.First().Message);
    }
}
