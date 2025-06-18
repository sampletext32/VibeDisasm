using Microsoft.AspNetCore.Mvc;
using VibeDisasm.Web.Handlers;
using VibeDisasm.Web.Models.DatabaseEntries;

namespace VibeDisasm.Web.Endpoints;

public static class ListingEndpoints
{
    public static void MapListingEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/listing/{projectId:guid}/{programId:guid}").WithTags("Listing");

        group.MapGet("/ataddress/{address:int}", AtAddress)
            .WithName("AtAddress")
            .WithDescription("Get listing at exact address");

        group.MapPost("/add/", AddEntry)
            .WithName("AddEntry")
            .WithDescription("Add an entry to the listing");
    }

    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UserProgramDatabaseEntry))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(string))]
    private static async Task<IResult> AtAddress(ListingAtAddressHandler handler, Guid projectId, Guid programId,
        uint address)
    {
        var result = await handler.Handle(projectId, programId, address);
        return result.IsSuccess
            ? Results.Json(result.Value.Item1, result.Value.Item2)
            : Results.BadRequest(result.Errors.First().Message);
    }

    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(string))]
    private static async Task<IResult> AddEntry(ListingAddEntryHandler handler, Guid projectId, Guid programId,
        HttpContext context)
    {
        var entry = await context.Request.ReadFromJsonAsync<UserProgramDatabaseEntry>(JsonSerializerOptionsPresets
            .DatabaseEntryTypeOptions
        );

        if (entry is null)
        {
            throw new InvalidOperationException("Unable to deserialize request.");
        }

        var result = await handler.Handle(projectId, programId, entry);
        return result.IsSuccess
            ? Results.Ok()
            : Results.BadRequest(result.Errors.First().Message);
    }
}
