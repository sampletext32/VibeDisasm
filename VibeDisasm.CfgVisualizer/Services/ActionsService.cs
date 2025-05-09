using VibeDisasm.CfgVisualizer.Models;

namespace VibeDisasm.CfgVisualizer.Services;

public class ActionsService : IService
{
    private readonly FileLoadingViewModel _fileLoadingViewModel;

    public ActionsService(FileLoadingViewModel fileLoadingViewModel)
    {
        _fileLoadingViewModel = fileLoadingViewModel;
    }

    public bool TryLoadFile(string path) => _fileLoadingViewModel.TryLoadFile(path);
}