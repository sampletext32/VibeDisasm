using VibeDisasm.Pe.Extractors;

namespace VibeDisasm.CfgVisualizer.Services;

/// <summary>
/// State of a loaded PE file
/// </summary>
public record PeFileState(
    byte[] FileData,
    string LoadedFilePath,
    PeInfo PeInfo,
    ExportInfo? ExportInfo,
    IReadOnlyList<SectionInfo> Sections,
    IReadOnlyList<EntryPointInfo> EntryPoints);