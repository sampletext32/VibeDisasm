using Microsoft.AspNetCore.Mvc;
using VibeDisasm.Web.Dtos;
using VibeDisasm.Web.Handlers;

namespace VibeDisasm.Web.Endpoints;

public static class TypesEndpoints
{
    public static void MapTypesEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/types").WithTags("Types");

        group.MapGet("/list-archives", ListArchives)
            .WithName("ListArchives")
            .WithDescription("Lists type archives associated with the program");

        group.MapGet("/{archiveNamespace}/list", ListArchiveTypes)
            .WithName("ListArchiveTypes")
            .WithDescription("Lists types in the specified archive");

        group.MapPost("/{archiveNamespace}/save", SaveTypeArchive)
            .WithName("SaveTypeArchive")
            .WithDescription("Saves the specified type archive to a file");

        group.MapPost("/create-archive", CreateTypeArchive)
            .WithName("CreateTypeArchive")
            .WithDescription("Creates a new type archive with the specified namespace");
    }

    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<TypeArchiveListItem>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(string))]
    private static async Task<IResult> ListArchives(
        ListArchivesHandler handler
    )
    {
        var result = await handler.Handle();
        return result.IsSuccess
            ? Results.Json(result.Value)
            : Results.BadRequest(result.Errors.First().Message);
    }

    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<TypeArchiveListItem>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(string))]
    private static async Task<IResult> ListArchiveTypes(
        ListArchiveTypesHandler handler,
        string archiveNamespace
    )
    {
        var result = await handler.Handle(archiveNamespace);
        return result.IsSuccess
            ? Results.Json(result.Value.types, result.Value.SerializerOptions)
            : Results.BadRequest(result.Errors.First().Message);
    }

    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(string))]
    private static async Task<IResult> SaveTypeArchive(
        SaveTypeArchiveHandler handler,
        string archiveNamespace
    )
    {
        var result = await handler.Handle(archiveNamespace);
        return result.IsSuccess
            ? Results.Ok()
            : Results.BadRequest(result.Errors.First().Message);
    }

    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(string))]
    private static async Task<IResult> CreateTypeArchive(
        CreateTypeArchiveHandler handler,
        [FromQuery] string archiveNamespace
    )
    {
        var result = await handler.Handle(archiveNamespace);
        return result.IsSuccess
            ? Results.Ok()
            : Results.BadRequest(result.Errors.First().Message);
    }
}
