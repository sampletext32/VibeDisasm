using VibeDisasm.CfgVisualizer.Models.Graph;

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
    public NodeDetailsPanelViewModel()
    {
    }
    
    /// <summary>
    /// Sets the selected node
    /// </summary>
    /// <param name="node">Node to select</param>
    public void SetSelectedNode(CfgNodeView? node)
    {
        SelectedNode = node;
    }
}
