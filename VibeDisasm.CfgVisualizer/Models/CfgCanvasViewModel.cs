using System.Numerics;

namespace VibeDisasm.CfgVisualizer.Models;

/// <summary>
/// View model for the CFG canvas
/// </summary>
public class CfgCanvasViewModel : IViewModel
{
    // Current CFG view model
    public CfgViewModel? CfgViewModel { get; private set; }
    
    // Panning and zooming
    public Vector2 PanOffset { get; private set; } = Vector2.Zero;
    public float Zoom { get; private set; } = 1.0f;
    
    // Node selection
    public CfgNodeViewModel? SelectedNode { get; private set; }
    public CfgNodeViewModel? HoveredNode { get; private set; }
    
    // Constants
    public const float MIN_ZOOM = 0.1f;
    public const float MAX_ZOOM = 10.0f;
    public const float ZOOM_SPEED = 0.1f;
    public const float PAN_SPEED = 1.0f;
    
    // Events
    public event EventHandler<NodeSelectionChangedEventArgs>? NodeSelectionChanged;
    
    /// <summary>
    /// Constructor
    /// </summary>
    public CfgCanvasViewModel()
    {
    }
    
    /// <summary>
    /// Sets the CFG view model
    /// </summary>
    /// <param name="cfgViewModel">CFG view model</param>
    public void SetCfgViewModel(CfgViewModel? cfgViewModel)
    {
        CfgViewModel = cfgViewModel;
        ResetView();
    }
    
    /// <summary>
    /// Resets the view (pan and zoom)
    /// </summary>
    public void ResetView()
    {
        PanOffset = Vector2.Zero;
        Zoom = 1.0f;
        SelectedNode = null;
        HoveredNode = null;
        
        // Notify that node selection has changed
        NodeSelectionChanged?.Invoke(this, new NodeSelectionChangedEventArgs(null));
    }
    
    /// <summary>
    /// Adjusts the pan offset
    /// </summary>
    /// <param name="delta">Delta to add to the pan offset</param>
    public void AdjustPan(Vector2 delta)
    {
        PanOffset += delta / Zoom * PAN_SPEED;
    }
    
    /// <summary>
    /// Adjusts the zoom level
    /// </summary>
    /// <param name="delta">Delta to add to the zoom level</param>
    /// <param name="mouseWorldPos">Mouse position in world space</param>
    /// <returns>New mouse position in world space</returns>
    public Vector2 AdjustZoom(float delta, Vector2 mouseWorldPos)
    {
        // Apply zoom
        float zoomDelta = delta * ZOOM_SPEED;
        float newZoom = Zoom * (1 + zoomDelta);
        Zoom = Math.Clamp(newZoom, MIN_ZOOM, MAX_ZOOM);
        
        // Calculate mouse position in world space after zoom
        var newMouseWorldPos = mouseWorldPos;
        
        // Adjust pan offset to keep mouse position fixed
        PanOffset += (mouseWorldPos - newMouseWorldPos);
        
        return newMouseWorldPos;
    }
    
    /// <summary>
    /// Sets the zoom level directly
    /// </summary>
    /// <param name="zoom">New zoom level</param>
    public void SetZoom(float zoom)
    {
        Zoom = Math.Clamp(zoom, MIN_ZOOM, MAX_ZOOM);
    }
    
    /// <summary>
    /// Sets the selected node
    /// </summary>
    /// <param name="node">Node to select</param>
    public void SelectNode(CfgNodeViewModel? node)
    {
        SelectedNode = node;
        
        // Notify that node selection has changed
        NodeSelectionChanged?.Invoke(this, new NodeSelectionChangedEventArgs(node));
    }
    
    /// <summary>
    /// Sets the hovered node
    /// </summary>
    /// <param name="node">Node to hover</param>
    public void HoverNode(CfgNodeViewModel? node)
    {
        HoveredNode = node;
    }
    
    /// <summary>
    /// Converts a screen position to a world position
    /// </summary>
    /// <param name="screenPos">Screen position</param>
    /// <param name="canvasSize">Canvas size</param>
    /// <returns>World position</returns>
    public Vector2 ScreenToWorld(Vector2 screenPos, Vector2 canvasSize)
    {
        // Apply inverse transform
        var transform = Matrix3x2.CreateTranslation(-canvasSize / 2) * 
                       Matrix3x2.CreateScale(1.0f / Zoom) * 
                       Matrix3x2.CreateTranslation(-PanOffset);
                       
        return Vector2.Transform(screenPos, transform);
    }
    
    /// <summary>
    /// Converts a world position to a screen position
    /// </summary>
    /// <param name="worldPos">World position</param>
    /// <param name="canvasSize">Canvas size</param>
    /// <returns>Screen position</returns>
    public Vector2 WorldToScreen(Vector2 worldPos, Vector2 canvasSize)
    {
        // Apply transform
        var transform = Matrix3x2.CreateTranslation(PanOffset) * 
                       Matrix3x2.CreateScale(Zoom) * 
                       Matrix3x2.CreateTranslation(canvasSize / 2);
                       
        return Vector2.Transform(worldPos, transform);
    }
    
    /// <summary>
    /// Gets the transform matrix for rendering
    /// </summary>
    /// <param name="canvasSize">Canvas size</param>
    /// <returns>Transform matrix</returns>
    public Matrix3x2 GetTransform(Vector2 canvasSize)
    {
        return Matrix3x2.CreateTranslation(PanOffset) * 
               Matrix3x2.CreateScale(Zoom) * 
               Matrix3x2.CreateTranslation(canvasSize / 2);
    }
}

/// <summary>
/// Event arguments for when node selection changes
/// </summary>
public class NodeSelectionChangedEventArgs : EventArgs
{
    /// <summary>
    /// Selected node
    /// </summary>
    public CfgNodeViewModel? SelectedNode { get; }
    
    /// <summary>
    /// Constructor
    /// </summary>
    public NodeSelectionChangedEventArgs(CfgNodeViewModel? selectedNode)
    {
        SelectedNode = selectedNode;
    }
}
