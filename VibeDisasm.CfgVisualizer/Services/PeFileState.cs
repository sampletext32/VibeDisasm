using VibeDisasm.Pe.Extractors;
using VibeDisasm.Pe.Raw;

namespace VibeDisasm.CfgVisualizer.Services;

/// <summary>
/// State of a loaded PE file
/// </summary>
public record PeFileState(
    byte[] FileData,
    string LoadedFilePath,
    RawPeFile RawPeFile,
    PeInfo PeInfo,
    ExportInfo? ExportInfo,
    IReadOnlyList<SectionInfo> Sections,
    IReadOnlyList<EntryPointInfo> EntryPoints);
