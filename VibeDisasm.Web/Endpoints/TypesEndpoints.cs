using Microsoft.AspNetCore.Mvc;
using VibeDisasm.Web.Handlers;
using VibeDisasm.Web.Models.Types;

namespace VibeDisasm.Web.Endpoints;

public static class TypesEndpoints
{
    public static void MapTypesEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/types/{projectId:guid}/{programId:guid}").WithTags("Types");

        group.MapGet("/list", ListTypes)
            .WithName("ListTypes")
            .WithDescription("Lists all types in a program");
    }

    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(DatabaseType))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(string))]
    private static async Task<IResult> ListTypes(
        ListTypesHandler handler,
        Guid projectId, Guid programId)
    {
        var result = await handler.Handle(projectId, programId);
        return result.IsSuccess
            ? Results.Json(result.Value.Item1, result.Value.Item2)
            : Results.BadRequest(result.Errors.First().Message);
    }
}
