using FluentResults;
using VibeDisasm.Pe.Extractors;
using VibeDisasm.Pe.Models;
using VibeDisasm.Pe.Raw;
using VibeDisasm.Web.Repositories;

namespace VibeDisasm.Web.Handlers;

/// <summary>
/// Handler for retrieving PE data from a program
/// </summary>
public class GetProgramPeDataHandler
{
    private readonly UserProgramRepository _programRepository;
    private readonly UserProgramDataRepository _programDataRepository;

    public GetProgramPeDataHandler(UserProgramRepository programRepository, UserProgramDataRepository programDataRepository)
    {
        _programRepository = programRepository;
        _programDataRepository = programDataRepository;
    }

    /// <summary>
    /// Gets the sections of a PE file
    /// </summary>
    /// <param name="programId">The ID of the program</param>
    /// <returns>Result containing section information</returns>
    public async Task<Result<List<SectionInfo>>> GetSections(Guid programId)
    {
        return await ExtractPeData(programId, rawPeFile => SectionExtractor.Extract(rawPeFile));
    }

    /// <summary>
    /// Gets the TLS information of a PE file
    /// </summary>
    /// <param name="programId">The ID of the program</param>
    /// <returns>Result containing TLS information</returns>
    public async Task<Result<TlsInfo?>> GetTls(Guid programId)
    {
        return await ExtractPeData(programId, rawPeFile => TlsExtractor.Extract(rawPeFile));
    }

    /// <summary>
    /// Gets the entry point information of a PE file
    /// </summary>
    /// <param name="programId">The ID of the program</param>
    /// <returns>Result containing entry point information</returns>
    public async Task<Result<List<EntryPointInfo>>> GetEntryPoint(Guid programId)
    {
        return await ExtractPeData(programId, rawPeFile => EntryPointExtractor.Extract(rawPeFile, ExportExtractor.Extract(rawPeFile)));
    }

    /// <summary>
    /// Gets the import information of a PE file
    /// </summary>
    /// <param name="programId">The ID of the program</param>
    /// <returns>Result containing import information</returns>
    public async Task<Result<ImportInfo?>> GetImports(Guid programId)
    {
        return await ExtractPeData(programId, rawPeFile => ImportExtractor.Extract(rawPeFile));
    }

    /// <summary>
    /// Gets the export information of a PE file
    /// </summary>
    /// <param name="programId">The ID of the program</param>
    /// <returns>Result containing export information</returns>
    public async Task<Result<ExportInfo?>> GetExports(Guid programId)
    {
        return await ExtractPeData(programId, rawPeFile => ExportExtractor.Extract(rawPeFile));
    }

    /// <summary>
    /// Gets the resource information of a PE file
    /// </summary>
    /// <param name="programId">The ID of the program</param>
    /// <returns>Result containing resource information</returns>
    public async Task<Result<ResourceInfo?>> GetResources(Guid programId)
    {
        return await ExtractPeData(programId, rawPeFile => ResourceExtractor.Extract(rawPeFile));
    }

    /// <summary>
    /// Gets the delay import information of a PE file
    /// </summary>
    /// <param name="programId">The ID of the program</param>
    /// <returns>Result containing delay import information</returns>
    public async Task<Result<DelayImportInfo?>> GetDelayImports(Guid programId)
    {
        return await ExtractPeData(programId, rawPeFile => DelayImportExtractor.Extract(rawPeFile));
    }

    /// <summary>
    /// Gets the exception information of a PE file
    /// </summary>
    /// <param name="programId">The ID of the program</param>
    /// <returns>Result containing exception information</returns>
    public async Task<Result<ExceptionInfo?>> GetExceptions(Guid programId)
    {
        return await ExtractPeData(programId, rawPeFile => ExceptionExtractor.Extract(rawPeFile));
    }

    /// <summary>
    /// Gets the basic PE information of a file
    /// </summary>
    /// <param name="programId">The ID of the program</param>
    /// <returns>Result containing PE information</returns>
    public async Task<Result<PeInfo>> GetPeInfo(Guid programId)
    {
        return await ExtractPeData(programId, rawPeFile => PeInfoExtractor.Extract(rawPeFile));
    }

    /// <summary>
    /// Gets the version information of a PE file
    /// </summary>
    /// <param name="programId">The ID of the program</param>
    /// <returns>Result containing version information</returns>
    public async Task<Result<List<VersionInfo>>> GetVersionInfo(Guid programId)
    {
        return await ExtractPeData(programId, rawPeFile => VersionExtractor.ExtractAll(rawPeFile, ResourceExtractor.Extract(rawPeFile)));
    }

    /// <summary>
    /// Gets the string table information of a PE file
    /// </summary>
    /// <param name="programId">The ID of the program</param>
    /// <returns>Result containing string table information</returns>
    public async Task<Result<List<StringTableInfo>>> GetStringTable(Guid programId)
    {
        return await ExtractPeData(programId, rawPeFile => StringTableExtractor.ExtractAll(rawPeFile, ResourceExtractor.Extract(rawPeFile)));
    }

    private async Task<Result<T>> ExtractPeData<T>(Guid programId, Func<RawPeFile, T> extractorFunc)
    {
        // Find the program
        var program = await _programRepository.GetById(programId);
        if (program is null)
        {
            return Result.Fail($"Program with ID {programId} not found");
        }

        try
        {
            // Get the program data
            var programData = await _programDataRepository.GetOrLoad(program);

            // Parse the PE file
            var rawPeFile = RawPeFactory.FromBytes(programData);

            // Extract the requested data
            var extractedData = extractorFunc(rawPeFile);

            return Result.Ok(extractedData);
        }
        catch (Exception ex)
        {
            return Result.Fail($"Failed to extract PE data: {ex.Message}");
        }
    }
}
