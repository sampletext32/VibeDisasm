using System.Diagnostics.Contracts;
using VibeDisasm.CfgVisualizer.Services;
using VibeDisasm.CfgVisualizer.State;
using VibeDisasm.Pe.Extractors;
using VibeDisasm.Pe.Models;

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

    /// <summary>
    /// Adds a custom entry point with the specified RVA
    /// </summary>
    /// <param name="rva">Relative Virtual Address of the custom entry point</param>
    [Pure]
    public bool AddCustomEntryPoint(uint rva)
    {
        // Check if the RVA is within a valid section
        var isValidRva = _state.OpenedFile.Sections.Any(section =>
            rva >= section.VirtualAddress &&
            rva < section.VirtualAddress + section.VirtualSize);

        if (!isValidRva)
        {
            return false;
        }

        // Convert RVA to file offset
        var fileOffset = Util.RvaToOffset(_state.OpenedFile.RawPeFile, rva);

        // Create a new entry point info
        var customEntryPoint = new EntryPointInfo(
            fileOffset,
            rva,
            "Custom",
            $"User-defined entry point at RVA {rva:X8}");

        // Create a new list with the custom entry point added
        var newEntryPoints = new List<EntryPointInfo>(EntryPoints) { customEntryPoint };

        // Update the entry points list
        SetEntryPoints(newEntryPoints);

        // Select the newly added entry point
        SelectEntryPoint(newEntryPoints.Count - 1);

        return true;
    }
}
