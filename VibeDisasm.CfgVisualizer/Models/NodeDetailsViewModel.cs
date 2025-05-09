namespace VibeDisasm.CfgVisualizer.Models;

/// <summary>
/// View model for the node details panel
/// </summary>
public class NodeDetailsViewModel : IViewModel
{
    // Selected node
    public CfgNodeViewModel? SelectedNode { get; private set; }
    
    /// <summary>
    /// Constructor
    /// </summary>
    public NodeDetailsViewModel()
    {
    }
    
    /// <summary>
    /// Sets the selected node
    /// </summary>
    /// <param name="node">Node to select</param>
    public void SetSelectedNode(CfgNodeViewModel? node)
    {
        SelectedNode = node;
    }
}
