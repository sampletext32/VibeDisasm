using System.Numerics;

namespace VibeDisasm.CfgVisualizer.Models;

/// <summary>
/// View model for a CFG edge in the visualization
/// </summary>
public class CfgEdgeViewModel
{
    /// <summary>
    /// Source node of the edge
    /// </summary>
    public CfgNodeViewModel Source { get; }
    
    /// <summary>
    /// Target node of the edge
    /// </summary>
    public CfgNodeViewModel Target { get; }
    
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
    public string Id => $"{Source.Id}->{Target.Id}";

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="source">Source node</param>
    /// <param name="target">Target node</param>
    /// <param name="isFallthrough">Whether this is a fallthrough edge</param>
    public CfgEdgeViewModel(CfgNodeViewModel source, CfgNodeViewModel target, bool isFallthrough)
    {
        Source = source;
        Target = target;
        IsFallthrough = isFallthrough;
        
        // Default color: green for fallthrough, red for jumps
        Color = isFallthrough 
            ? new Vector4(0.0f, 0.8f, 0.0f, 1.0f) 
            : new Vector4(0.8f, 0.0f, 0.0f, 1.0f);
            
        IsHighlighted = false;
    }
}
