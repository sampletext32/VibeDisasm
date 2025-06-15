using Microsoft.AspNetCore.Mvc;
using VibeDisasm.Web.Dtos;
using VibeDisasm.Web.Handlers;

namespace VibeDisasm.Web.Endpoints;

public static class ListingEndpoints
{
    public static void MapListingEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/listing/{projectId:guid}/{programId:guid}").WithTags("Listing");

        group.MapGet("/ataddress/{address:int}", AtAddress)
            .WithName("AtAddress")
            .WithDescription("Get listing at exact address");
    }

    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ListingElementDto))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(string))]
    private static async Task<IResult> AtAddress(ListingAtAddressHandler handler, Guid projectId, Guid programId, uint address)
    {
        var result = await handler.Handle(projectId, programId, address);
        return result.IsSuccess
            ? Results.Json(result.Value)
            : Results.BadRequest(result.Errors.First().Message);
    }
}
