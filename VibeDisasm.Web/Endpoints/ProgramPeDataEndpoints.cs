using Microsoft.AspNetCore.Mvc;
using VibeDisasm.Web.Handlers;

namespace VibeDisasm.Web.Endpoints;

/// <summary>
/// Endpoints for accessing PE data from programs
/// </summary>
public static class ProgramPeDataEndpoints
{
    public static void MapProgramPeDataEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/program/{programId:guid}/pe").WithTags("Program PE Data");

        group.MapGet("/info", GetPeInfo)
            .WithName("GetPeInfo")
            .WithDescription("Returns basic PE information for the program");

        group.MapGet("/sections", GetSections)
            .WithName("GetSections")
            .WithDescription("Returns section information for the program");

        group.MapGet("/tls", GetTls)
            .WithName("GetTls")
            .WithDescription("Returns TLS information for the program");

        group.MapGet("/entrypoint", GetEntryPoint)
            .WithName("GetEntryPoint")
            .WithDescription("Returns entry point information for the program");

        group.MapGet("/imports", GetImports)
            .WithName("GetImports")
            .WithDescription("Returns import information for the program");

        group.MapGet("/exports", GetExports)
            .WithName("GetExports")
            .WithDescription("Returns export information for the program");

        group.MapGet("/resources", GetResources)
            .WithName("GetResources")
            .WithDescription("Returns resource information for the program");

        group.MapGet("/delayimports", GetDelayImports)
            .WithName("GetDelayImports")
            .WithDescription("Returns delay import information for the program");

        group.MapGet("/exceptions", GetExceptions)
            .WithName("GetExceptions")
            .WithDescription("Returns exception information for the program");

        group.MapGet("/version", GetVersionInfo)
            .WithName("GetVersionInfo")
            .WithDescription("Returns version information for the program");

        group.MapGet("/stringtable", GetStringTable)
            .WithName("GetStringTable")
            .WithDescription("Returns string table information for the program");
    }

    /// <summary>
    /// Gets basic PE information for a program
    /// </summary>
    /// <param name="programId">The ID of the program</param>
    /// <param name="handler">The handler for PE data extraction</param>
    /// <returns>PE information for the program</returns>
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    private static async Task<IResult> GetPeInfo(Guid programId, GetProgramPeDataHandler handler)
    {
        var result = await handler.GetPeInfo(programId);
        return result.IsSuccess 
            ? Results.Ok(result.Value) 
            : Results.Problem(result.Errors.First().Message);
    }

    /// <summary>
    /// Gets section information for a program
    /// </summary>
    /// <param name="programId">The ID of the program</param>
    /// <param name="handler">The handler for PE data extraction</param>
    /// <returns>Section information for the program</returns>
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    private static async Task<IResult> GetSections(Guid programId, GetProgramPeDataHandler handler)
    {
        var result = await handler.GetSections(programId);
        return result.IsSuccess 
            ? Results.Ok(result.Value) 
            : Results.Problem(result.Errors.First().Message);
    }

    /// <summary>
    /// Gets TLS information for a program
    /// </summary>
    /// <param name="programId">The ID of the program</param>
    /// <param name="handler">The handler for PE data extraction</param>
    /// <returns>TLS information for the program</returns>
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    private static async Task<IResult> GetTls(Guid programId, GetProgramPeDataHandler handler)
    {
        var result = await handler.GetTls(programId);
        return result.IsSuccess 
            ? Results.Ok(result.Value) 
            : Results.Problem(result.Errors.First().Message);
    }




    /// <summary>
    /// Gets entry point information for a program
    /// </summary>
    /// <param name="programId">The ID of the program</param>
    /// <param name="handler">The handler for PE data extraction</param>
    /// <returns>Entry point information for the program</returns>
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    private static async Task<IResult> GetEntryPoint(Guid programId, GetProgramPeDataHandler handler)
    {
        var result = await handler.GetEntryPoint(programId);
        return result.IsSuccess 
            ? Results.Ok(result.Value) 
            : Results.Problem(result.Errors.First().Message);
    }

    /// <summary>
    /// Gets import information for a program
    /// </summary>
    /// <param name="programId">The ID of the program</param>
    /// <param name="handler">The handler for PE data extraction</param>
    /// <returns>Import information for the program</returns>
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    private static async Task<IResult> GetImports(Guid programId, GetProgramPeDataHandler handler)
    {
        var result = await handler.GetImports(programId);
        return result.IsSuccess 
            ? Results.Ok(result.Value) 
            : Results.Problem(result.Errors.First().Message);
    }

    /// <summary>
    /// Gets export information for a program
    /// </summary>
    /// <param name="programId">The ID of the program</param>
    /// <param name="handler">The handler for PE data extraction</param>
    /// <returns>Export information for the program</returns>
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    private static async Task<IResult> GetExports(Guid programId, GetProgramPeDataHandler handler)
    {
        var result = await handler.GetExports(programId);
        return result.IsSuccess 
            ? Results.Ok(result.Value) 
            : Results.Problem(result.Errors.First().Message);
    }

    /// <summary>
    /// Gets resource information for a program
    /// </summary>
    /// <param name="programId">The ID of the program</param>
    /// <param name="handler">The handler for PE data extraction</param>
    /// <returns>Resource information for the program</returns>
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    private static async Task<IResult> GetResources(Guid programId, GetProgramPeDataHandler handler)
    {
        var result = await handler.GetResources(programId);
        return result.IsSuccess 
            ? Results.Ok(result.Value) 
            : Results.Problem(result.Errors.First().Message);
    }

    /// <summary>
    /// Gets delay import information for a program
    /// </summary>
    /// <param name="programId">The ID of the program</param>
    /// <param name="handler">The handler for PE data extraction</param>
    /// <returns>Delay import information for the program</returns>
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    private static async Task<IResult> GetDelayImports(Guid programId, GetProgramPeDataHandler handler)
    {
        var result = await handler.GetDelayImports(programId);
        return result.IsSuccess 
            ? Results.Ok(result.Value) 
            : Results.Problem(result.Errors.First().Message);
    }

    /// <summary>
    /// Gets exception information for a program
    /// </summary>
    /// <param name="programId">The ID of the program</param>
    /// <param name="handler">The handler for PE data extraction</param>
    /// <returns>Exception information for the program</returns>
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    private static async Task<IResult> GetExceptions(Guid programId, GetProgramPeDataHandler handler)
    {
        var result = await handler.GetExceptions(programId);
        return result.IsSuccess 
            ? Results.Ok(result.Value) 
            : Results.Problem(result.Errors.First().Message);
    }

    /// <summary>
    /// Gets version information for a program
    /// </summary>
    /// <param name="programId">The ID of the program</param>
    /// <param name="handler">The handler for PE data extraction</param>
    /// <returns>Version information for the program</returns>
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    private static async Task<IResult> GetVersionInfo(Guid programId, GetProgramPeDataHandler handler)
    {
        var result = await handler.GetVersionInfo(programId);
        return result.IsSuccess 
            ? Results.Ok(result.Value) 
            : Results.Problem(result.Errors.First().Message);
    }

    /// <summary>
    /// Gets string table information for a program
    /// </summary>
    /// <param name="programId">The ID of the program</param>
    /// <param name="handler">The handler for PE data extraction</param>
    /// <returns>String table information for the program</returns>
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    private static async Task<IResult> GetStringTable(Guid programId, GetProgramPeDataHandler handler)
    {
        var result = await handler.GetStringTable(programId);
        return result.IsSuccess 
            ? Results.Ok(result.Value) 
            : Results.Problem(result.Errors.First().Message);
    }
}
