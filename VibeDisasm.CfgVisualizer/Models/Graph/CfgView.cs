using System.Numerics;
using VibeDisasm.DecompilerEngine.ControlFlow;

namespace VibeDisasm.CfgVisualizer.Models.Graph;

/// <summary>
/// View model for the entire CFG visualization
/// </summary>
public class CfgView
{
    /// <summary>
    /// The underlying control flow function
    /// </summary>
    public ControlFlowFunction Function { get; }
    
    /// <summary>
    /// Nodes in the CFG
    /// </summary>
    public List<CfgNodeView> Nodes { get; } = [];
    
    /// <summary>
    /// Edges in the CFG
    /// </summary>
    public List<CfgEdgeView> Edges { get; } = [];
    
    /// <summary>
    /// Camera position in the visualization
    /// </summary>
    public Vector2 CameraPosition { get; set; } = Vector2.Zero;
    
    /// <summary>
    /// Camera zoom level
    /// </summary>
    public float Zoom { get; set; } = 1.0f;
    
    /// <summary>
    /// Selected node
    /// </summary>
    public CfgNodeView? SelectedNode { get; set; }

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="function">Control flow function</param>
    public CfgView(ControlFlowFunction function)
    {
        Function = function;

        var edges = ControlFlowEdgesBuilder.Build(function);
        
        // Create node view models
        var nodeMap = new Dictionary<uint, CfgNodeView>();
        foreach (var block in function.Blocks)
        {
            var nodeViewModel = new CfgNodeView(block.Value);
            Nodes.Add(nodeViewModel);
            nodeMap[block.Key] = nodeViewModel;
        }
        
        // Create edge view models
        foreach (var edge in edges.SelectMany(x => x))
        {
            if (nodeMap.TryGetValue(edge.FromBlockAddress, out var source) && 
                nodeMap.TryGetValue(edge.ToBlockAddress, out var target))
            {
                var isFallthrough = edge.JumpType == ControlFlowJumpType.Fallthrough;
                var edgeViewModel = new CfgEdgeView(source, target, isFallthrough);
                Edges.Add(edgeViewModel);
            }
        }
        
        // Perform initial layout
        PerformLayout();
    }
    
    /// <summary>
    /// Performs improved layout of the nodes for better control flow visualization.
    /// For conditional jumps, the taken branch is always to the left, fallthrough below.
    /// </summary>
    public void PerformLayout()
    {
        // Reset all node positions
        foreach (var node in Nodes)
            node.Position = Vector2.Zero;

        // Track visited nodes to avoid cycles
        var visited = new HashSet<CfgNodeView>();

        // Start with the entry node
        var entryNode = Nodes.FirstOrDefault(n => n.Block.IsEntryBlock);
        if (entryNode == null)
            return;

        // Recursively layout the graph
        LayoutNode(entryNode, 0, 0, visited);
    }

    /// <summary>
    /// Recursively lays out the graph, placing taken branches to the left
    /// and fallthroughs below the node.
    /// </summary>
    /// <param name="node">Current node</param>
    /// <param name="x">X position (grid units)</param>
    /// <param name="y">Y position (grid units)</param>
    /// <param name="visited">Visited nodes</param>
    private void LayoutNode(CfgNodeView node, int x, int y, HashSet<CfgNodeView> visited)
    {
        // Constants for layout
        const float nodeWidth = 200.0f;
        const float nodeHeight = 100.0f;
        const float horizontalSpacing = 80.0f;
        const float verticalSpacing = 80.0f;

        if (!visited.Add(node))
            return;

        // Set node position
        node.Position = new Vector2(x * (nodeWidth + horizontalSpacing), y * (nodeHeight + verticalSpacing));

        // Get outgoing edges
        var outgoing = Edges.Where(e => e.Source == node).ToList();
        if (outgoing.Count == 0)
            return;

        // Classify edges
        var takenEdge = outgoing.FirstOrDefault(e => !e.IsFallthrough);
        var fallthroughEdge = outgoing.FirstOrDefault(e => e.IsFallthrough);

        int nextY = y + 1;

        // Layout taken branch to the left
        if (takenEdge != null)
        {
            LayoutNode(takenEdge.Target, x - 1, nextY, visited);
        }
        // Layout fallthrough branch below
        if (fallthroughEdge != null)
        {
            LayoutNode(fallthroughEdge.Target, x, nextY, visited);
        }

        // Layout any other outgoing edges (e.g. unconditional jumps) to the right
        foreach (var edge in outgoing)
        {
            if (edge != takenEdge && edge != fallthroughEdge)
            {
                LayoutNode(edge.Target, x + 1, nextY, visited);
            }
        }
    }
}
