using VibeDisasm.CfgVisualizer.Models.Graph;
using VibeDisasm.CfgVisualizer.Services;
using VibeDisasm.DecompilerEngine.ControlFlow;
using VibeDisasm.Pe.Extractors;
using VibeDisasm.Pe.Raw;

namespace VibeDisasm.CfgVisualizer.ViewModels;

/// <summary>
/// View model for file loading functionality
/// </summary>
public class FileLoadingPanelViewModel : IViewModel
{
    // Service for loading PE files
    private readonly PeFileService _peFileService;
    
    /// <summary>
    /// Loaded file path
    /// </summary>
    public string LoadedFilePath => _peFileService.LoadedFilePath;
    
    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="peFileService">Service for loading PE files</param>
    public FileLoadingPanelViewModel(PeFileService peFileService)
    {
        _peFileService = peFileService;
    }
    
    /// <summary>
    /// Tries to load a PE file and analyze its control flow
    /// </summary>
    /// <param name="filePath">Path to the PE file</param>
    /// <returns>True if successful, false otherwise</returns>
    public bool TryLoadFile(string filePath)
    {
        // Delegate to the PeFileService
        return _peFileService.TryLoadFile(filePath);
    }
}