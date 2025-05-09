using System.Numerics;
using VibeDisasm.DecompilerEngine.ControlFlow;

namespace VibeDisasm.CfgVisualizer.Models;

/// <summary>
/// View model for the entire CFG visualization
/// </summary>
public class CfgViewModel
{
    /// <summary>
    /// The underlying control flow function
    /// </summary>
    public ControlFlowFunction Function { get; }
    
    /// <summary>
    /// Nodes in the CFG
    /// </summary>
    public List<CfgNodeViewModel> Nodes { get; } = [];
    
    /// <summary>
    /// Edges in the CFG
    /// </summary>
    public List<CfgEdgeViewModel> Edges { get; } = [];
    
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
    public CfgNodeViewModel? SelectedNode { get; set; }

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="function">Control flow function</param>
    public CfgViewModel(ControlFlowFunction function)
    {
        Function = function;

        var edges = ControlFlowEdgesBuilder.Build(function);
        
        // Create node view models
        var nodeMap = new Dictionary<uint, CfgNodeViewModel>();
        foreach (var block in function.Blocks)
        {
            var nodeViewModel = new CfgNodeViewModel(block.Value);
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
                var edgeViewModel = new CfgEdgeViewModel(source, target, isFallthrough);
                Edges.Add(edgeViewModel);
            }
        }
        
        // Perform initial layout
        PerformLayout();
    }
    
    /// <summary>
    /// Performs automatic layout of the nodes
    /// </summary>
    public void PerformLayout()
    {
        // Simple layered layout algorithm
        // Group nodes by their depth in the CFG
        var layers = new Dictionary<int, List<CfgNodeViewModel>>();
        var visited = new HashSet<CfgNodeViewModel>();
        var queue = new Queue<(CfgNodeViewModel Node, int Depth)>();
        
        // Start with the entry node
        var entryNode = Nodes.FirstOrDefault(n => n.Block.IsEntryBlock);
        if (entryNode != null)
        {
            queue.Enqueue((entryNode, 0));
            visited.Add(entryNode);
        }
        
        // Breadth-first traversal to assign layers
        while (queue.Count > 0)
        {
            var (node, depth) = queue.Dequeue();
            
            if (!layers.TryGetValue(depth, out var layer))
            {
                layer = [];
                layers[depth] = layer;
            }
            
            layer.Add(node);
            
            // Enqueue successors
            foreach (var edge in Edges.Where(e => e.Source == node))
            {
                if (!visited.Contains(edge.Target))
                {
                    queue.Enqueue((edge.Target, depth + 1));
                    visited.Add(edge.Target);
                }
            }
        }
        
        // Position nodes based on their layer
        const float layerHeight = 150.0f;
        const float nodeSpacing = 50.0f;
        
        foreach (var (depth, nodes) in layers.OrderBy(kv => kv.Key))
        {
            float layerWidth = nodes.Count * 250.0f;
            float startX = -layerWidth / 2;
            
            for (int i = 0; i < nodes.Count; i++)
            {
                nodes[i].Position = new Vector2(
                    startX + i * (nodes[i].Size.X + nodeSpacing),
                    depth * layerHeight
                );
            }
        }
        
        // Handle any nodes that weren't visited
        float y = (layers.Count > 0 ? layers.Keys.Max() + 1 : 0) * layerHeight;
        float x = -((Nodes.Count - visited.Count) * 250.0f) / 2;
        
        foreach (var node in Nodes.Where(n => !visited.Contains(n)))
        {
            node.Position = new Vector2(x, y);
            x += node.Size.X + nodeSpacing;
        }
    }
}
