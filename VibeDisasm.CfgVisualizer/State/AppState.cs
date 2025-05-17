using VibeDisasm.CfgVisualizer.Services;
using VibeDisasm.CfgVisualizer.ViewModels;

namespace VibeDisasm.CfgVisualizer.State;

/// <summary>
/// Main view model that coordinates all panel view models
/// </summary>
public class AppState
{
    // Panel view models
    public FileLoadingPanelViewModel FileLoadingPanelViewModel { get; }
    public EntryPointsPanelViewModel EntryPointsPanelViewModel { get; }
    public CfgCanvasPanelViewModel CfgCanvasPanelViewModel { get; }
    public NodeDetailsPanelViewModel NodeDetailsPanelViewModel { get; }

    /// <summary>
    /// Constructor
    /// </summary>
    public AppState(
        FileLoadingPanelViewModel fileLoadingPanelViewModel,
        EntryPointsPanelViewModel entryPointsPanelViewModel,
        CfgCanvasPanelViewModel cfgCanvasPanelViewModel,
        NodeDetailsPanelViewModel nodeDetailsPanelViewModel,
        PeFileService peFileService
    )
    {
        FileLoadingPanelViewModel = fileLoadingPanelViewModel;
        EntryPointsPanelViewModel = entryPointsPanelViewModel;
        CfgCanvasPanelViewModel = cfgCanvasPanelViewModel;
        NodeDetailsPanelViewModel = nodeDetailsPanelViewModel;
        
        peFileService.PeFileLoaded += OnPeFileLoaded;
    }

    private void OnPeFileLoaded(PeFileLoadedEventArgs e)
    {
    }
}