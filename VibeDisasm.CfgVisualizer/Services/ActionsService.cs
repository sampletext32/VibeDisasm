using VibeDisasm.CfgVisualizer.ViewModels;

namespace VibeDisasm.CfgVisualizer.Services;

/// <summary>
/// Service for handling application actions
/// </summary>
public class ActionsService : IService
{
    private readonly PeFileService _peFileService;
    private readonly FileLoadingPanelViewModel _fileLoadingPanelViewModel;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="peFileService">Service for loading PE files</param>
    /// <param name="fileLoadingPanelViewModel">File loading panel view model</param>
    public ActionsService(PeFileService peFileService, FileLoadingPanelViewModel fileLoadingPanelViewModel)
    {
        _peFileService = peFileService;
        _fileLoadingPanelViewModel = fileLoadingPanelViewModel;
    }

    /// <summary>
    /// Tries to load a PE file
    /// </summary>
    /// <param name="path">Path to the PE file</param>
    /// <returns>True if successful, false otherwise</returns>
    public bool TryLoadFile(string path) => _peFileService.TryLoadFile(path);
}