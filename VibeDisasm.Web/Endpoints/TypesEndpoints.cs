using Microsoft.AspNetCore.Mvc;
using VibeDisasm.Web.Dtos;
using VibeDisasm.Web.Handlers;
using VibeDisasm.Web.Models.Types;

namespace VibeDisasm.Web.Endpoints;

public static class TypesEndpoints
{
    public static void MapTypesEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/types/{projectId:guid}/{programId:guid}").WithTags("Types");

        group.MapGet("/list-archives", ListArchives)
            .WithName("ListArchives")
            .WithDescription("Lists type archives associated with the program");

        group.MapGet("/{archiveNamespace}/list", ListArchiveTypes)
            .WithName("ListArchiveTypes")
            .WithDescription("Lists types in the specified archive");
    }

    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<TypeArchiveListItem>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(string))]
    private static async Task<IResult> ListArchives(
        ListArchivesHandler handler,
        Guid projectId, Guid programId)
    {
        var result = await handler.Handle(projectId, programId);
        return result.IsSuccess
            ? Results.Json(result.Value)
            : Results.BadRequest(result.Errors.First().Message);
    }

    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<TypeArchiveListItem>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(string))]
    private static async Task<IResult> ListArchiveTypes(
        ListArchiveTypesHandler handler,
        Guid projectId, Guid programId, string archiveNamespace)
    {
        var result = await handler.Handle(projectId, programId, archiveNamespace);
        return result.IsSuccess
            ? Results.Json(result.Value)
            : Results.BadRequest(result.Errors.First().Message);
    }
}
