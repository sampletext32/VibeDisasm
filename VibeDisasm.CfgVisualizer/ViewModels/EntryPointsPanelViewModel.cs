using VibeDisasm.CfgVisualizer.Services;
using VibeDisasm.CfgVisualizer.State;
using VibeDisasm.Pe.Extractors;

namespace VibeDisasm.CfgVisualizer.ViewModels;

/// <summary>
/// View model for the entry points panel
/// </summary>
public class EntryPointsPanelViewModel : IViewModel
{
    private readonly AppState _state;

    // Entry points list
    public IReadOnlyList<EntryPointInfo> EntryPoints { get; private set; } = [];

    // Selected entry point index
    public int SelectedEntryPointIndex { get; private set; } = -1;

    /// <summary>
    /// Constructor
    /// </summary>
    public EntryPointsPanelViewModel(AppState state)
    {
        _state = state;
        state.FileLoaded += OnFileLoaded;
    }

    private void OnFileLoaded(PeFileState obj)
    {
        SetEntryPoints(obj.EntryPoints);
    }

    /// <summary>
    /// Sets the entry points list
    /// </summary>
    /// <param name="entryPoints">Entry points list</param>
    public void SetEntryPoints(IReadOnlyList<EntryPointInfo> entryPoints)
    {
        EntryPoints = entryPoints;
        SelectedEntryPointIndex = -1;
    }

    /// <summary>
    /// Selects an entry point by index
    /// </summary>
    /// <param name="index">Index of the entry point to select</param>
    public void SelectEntryPoint(int index)
    {
        if (index >= 0 && index < EntryPoints.Count)
        {
            SelectedEntryPointIndex = index;

            _state.OnEntryPointSelected(EntryPoints[index]);
        }
    }
}
