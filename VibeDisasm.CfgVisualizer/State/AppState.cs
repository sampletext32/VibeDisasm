using VibeDisasm.CfgVisualizer.Models.Graph;
using VibeDisasm.CfgVisualizer.Services;
using VibeDisasm.CfgVisualizer.ViewModels;
using VibeDisasm.Pe.Extractors;

namespace VibeDisasm.CfgVisualizer.State;

/// <summary>
/// Main view model that coordinates all panel view models
/// </summary>
public class AppState
{
    /// <summary>
    /// Constructor
    /// </summary>
    public AppState()
    {
    }
    
    public event Action<PeFileState>? FileLoaded;
    public event Action<EntryPointInfo>? EntryPointSelected;
    public event Action<CfgNodeView?>? CfgNodeSelected;

    public event Action<InstructionInfo>? InstructionSelected;

    public void OnPeFileLoaded(PeFileState state)
    {
        OpenedFile = state;
        FileLoaded?.Invoke(state);
    }

    public PeFileState? OpenedFile { get; set; }

    public void OnEntryPointSelected(EntryPointInfo entryPoint)
    {
        EntryPointSelected?.Invoke(entryPoint);
    }

    public void OnCfgNodeSelected(CfgNodeView? node)
    {
        CfgNodeSelected?.Invoke(node);
    }

    public void OnInstructionSelected(InstructionInfo instruction)
    {
        InstructionSelected?.Invoke(instruction);
    }
}