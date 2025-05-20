using System.Diagnostics;
using VibeDisasm.Pe.Extractors;
using VibeDisasm.Pe.Raw;

namespace VibeDisasm.CfgVisualizer.Services;

/// <summary>
/// Service for loading and analyzing PE files
/// </summary>
public sealed class PeFileService : IService
{
    /// <summary>
    /// Loads a PE file and analyzes its structure
    /// </summary>
    /// <param name="filePath">Path to the PE file</param>
    /// <returns>Result of the load operation</returns>
    public PeFileState LoadPeFile(string filePath)
    {
        var fileData = File.ReadAllBytes(filePath);
        var rawPe = RawPeFactory.FromBytes(fileData);

        var exportInfo = ExportExtractor.Extract(rawPe);
        return new PeFileState(
            FileData: fileData,
            LoadedFilePath: filePath,
            PeInfo: PeInfoExtractor.Extract(rawPe),
            ExportInfo: exportInfo,
            Sections: SectionExtractor.Extract(rawPe),
            EntryPoints: EntryPointExtractor.Extract(rawPe, exportInfo)
        );
    }
}