using VibeDisasm.CfgVisualizer.Models.Graph;
using VibeDisasm.CfgVisualizer.Services;
using VibeDisasm.CfgVisualizer.State;
using VibeDisasm.DecompilerEngine.ControlFlow;
using VibeDisasm.Pe.Extractors;
using VibeDisasm.Pe.Raw;

namespace VibeDisasm.CfgVisualizer.ViewModels;

/// <summary>
/// View model for file loading functionality
/// </summary>
public class FileLoadingPanelViewModel : IViewModel
{
    private readonly ActionsService _actionsService;
    private readonly AppState _state;

    public FileLoadingPanelViewModel(ActionsService actionsService, AppState state)
    {
        _actionsService = actionsService;
        _state = state;
        
        state.FileLoaded += OnFileLoaded;
    }

    private void OnFileLoaded(PeFileState obj)
    {
        LoadedFilePath = obj.LoadedFilePath;
    }

    public string? LoadedFilePath { get; set; }

    public void LaunchLoad(string path)
    {
        _actionsService.TryLoadFile(path);
    }
}