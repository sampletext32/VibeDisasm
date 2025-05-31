using VibeDisasm.CfgVisualizer.State;

namespace VibeDisasm.CfgVisualizer.Services;

/// <summary>
/// Service for handling application actions
/// </summary>
public class ActionsService : IService
{
    private readonly PeFileService _peFileService;
    private readonly AppState _appState;

    public ActionsService(PeFileService peFileService, AppState appState)
    {
        _peFileService = peFileService;
        _appState = appState;
    }

    public bool TryLoadFile(string path)
    {
        var peFileState = _peFileService.LoadPeFile(path);

        _appState.OnPeFileLoaded(peFileState);

        return true;
    }
}
