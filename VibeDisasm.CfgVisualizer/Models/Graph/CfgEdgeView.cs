using System.Numerics;

namespace VibeDisasm.CfgVisualizer.Models.Graph;

/// <summary>
/// View model for a CFG edge in the visualization
/// </summary>
public class CfgEdgeView
{
    /// <summary>
    /// Source node of the edge
    /// </summary>
    public CfgNodeView Source { get; }
    
    /// <summary>
    /// Target node of the edge
    /// </summary>
    public CfgNodeView Target { get; }
    
    /// <summary>
    /// Whether this is a fallthrough edge (not a jump)
    /// </summary>
    public bool IsFallthrough { get; }
    
    /// <summary>
    /// Color of the edge
    /// </summary>
    public Vector4 Color { get; set; }
    
    /// <summary>
    /// Whether the edge is highlighted
    /// </summary>
    public bool IsHighlighted { get; set; }
    
    /// <summary>
    /// Unique identifier for the edge
    /// </summary>
    public string Id { get; }

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="source">Source node</param>
    /// <param name="target">Target node</param>
    /// <param name="isFallthrough">Whether this is a fallthrough edge</param>
    public CfgEdgeView(CfgNodeView source, CfgNodeView target, bool isFallthrough)
    {
        Source = source;
        Target = target;
        Id = $"{Source.Id}->{Target.Id}";

        IsFallthrough = isFallthrough;
        
        // Default color: green for fallthrough, red for jumps
        Color = isFallthrough 
            ? new Vector4(0.8f, 0.0f, 0.0f, 1.0f) 
            : new Vector4(0.0f, 0.8f, 0.0f, 1.0f);
            
        IsHighlighted = false;
    }
}
