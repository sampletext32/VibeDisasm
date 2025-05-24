using System.Numerics;
using VibeDisasm.DecompilerEngine.ControlFlow;
using VibeDisasm.Disassembler.X86;

namespace VibeDisasm.CfgVisualizer.Models.Graph;

/// <summary>
/// View model for a CFG node in the visualization
/// </summary>
public class CfgNodeView
{
    /// <summary>
    /// The underlying control flow block
    /// </summary>
    public AsmBlock Block { get; }
    
    /// <summary>
    /// Position of the node in the visualization
    /// </summary>
    public Vector2 Position { get; set; }
    
    /// <summary>
    /// Size of the node in the visualization
    /// </summary>
    public Vector2 Size { get; set; }
    
    /// <summary>
    /// Color of the node
    /// </summary>
    public Vector4 Color { get; set; }
    
    /// <summary>
    /// Whether the node is selected
    /// </summary>
    public bool IsSelected { get; set; }
    
    /// <summary>
    /// Whether the node is highlighted
    /// </summary>
    public bool IsHighlighted { get; set; }
    
    /// <summary>
    /// Unique identifier for the node
    /// </summary>
    public string Id { get; }

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="block">Control flow block</param>
    public CfgNodeView(AsmBlock block)
    {
        Block = block;
        Id = block.ComputedStartAddressView;
        Position = Vector2.Zero;
        Size = new Vector2(200, 100);
        Color = new Vector4(0.7f, 0.7f, 0.7f, 1.0f);
        IsSelected = false;
        IsHighlighted = false;
    }
}
