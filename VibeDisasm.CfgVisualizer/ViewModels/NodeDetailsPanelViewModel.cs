using VibeDisasm.CfgVisualizer.Models.Graph;
using VibeDisasm.CfgVisualizer.State;

namespace VibeDisasm.CfgVisualizer.ViewModels;

/// <summary>
/// View model for the node details panel
/// </summary>
public class NodeDetailsPanelViewModel : IViewModel
{
    // Selected node
    public CfgNodeView? SelectedNode { get; private set; }
    
    /// <summary>
    /// Constructor
    /// </summary>
    public NodeDetailsPanelViewModel(AppState state)
    {
        state.CfgNodeSelected += OnCfgNodeSelected;
    }

    private void OnCfgNodeSelected(CfgNodeView? node)
    {
        SelectedNode = node;
    }
}
