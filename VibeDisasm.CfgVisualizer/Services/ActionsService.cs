using VibeDisasm.CfgVisualizer.ViewModels;

namespace VibeDisasm.CfgVisualizer.Services;

public class ActionsService : IService
{
    private readonly FileLoadingPanelViewModel _fileLoadingPanelViewModel;

    public ActionsService(FileLoadingPanelViewModel fileLoadingPanelViewModel)
    {
        _fileLoadingPanelViewModel = fileLoadingPanelViewModel;
    }

    public bool TryLoadFile(string path) => _fileLoadingPanelViewModel.TryLoadFile(path);
}