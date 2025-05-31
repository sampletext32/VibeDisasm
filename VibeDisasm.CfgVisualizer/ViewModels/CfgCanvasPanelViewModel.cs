using System.Numerics;
using VibeDisasm.CfgVisualizer.Models.Graph;
using VibeDisasm.CfgVisualizer.State;
using VibeDisasm.Disassembler.X86;
using VibeDisasm.Pe.Extractors;

namespace VibeDisasm.CfgVisualizer.ViewModels;

/// <summary>
/// View model for the CFG canvas
/// </summary>
public class CfgCanvasPanelViewModel : IViewModel
{
    private readonly AppState _state;

    // Current CFG view model
    public CfgView? CfgViewModel { get; private set; }

    // Panning and zooming
    public Vector2 PanOffset { get; private set; } = Vector2.Zero;
    public float Zoom { get; private set; } = 1.0f;

    // Node selection
    public CfgNodeView? SelectedNode { get; set; }
    public CfgNodeView? HoveredNode { get; set; }

    // Constants
    public const float MIN_ZOOM = 0.1f;
    public const float MAX_ZOOM = 10.0f;
    public const float ZOOM_SPEED = 0.1f;
    public const float PAN_SPEED = 1.0f;

    public CfgCanvasPanelViewModel(AppState state)
    {
        _state = state;
        state.EntryPointSelected += OnEntryPointSelected;
    }

    private void OnEntryPointSelected(EntryPointInfo obj)
    {
        var function = AsmFunctionDisassembler.DisassembleFunction(_state.OpenedFile!.FileData, obj.FileOffset);
        var viewModel = new CfgView(function);
        CfgViewModel = viewModel;
        ResetView();
    }

    private (float, Vector2, int, int) _statusStringTuple = (0, new(), 0, 0);
    private string StatusString { get; set; } = null!;

    public string GetStatusString()
    {
        var state = (Zoom, PanOffset, CfgViewModel!.Nodes.Count, CfgViewModel!.Edges.Count);
        if (state != _statusStringTuple)
        {
            _statusStringTuple = state;
            StatusString = $"Zoom: {Zoom:F2}x | Pan: ({PanOffset.X:F1}, {PanOffset.Y:F1}) | Nodes: {CfgViewModel.Nodes.Count} | Edges: {CfgViewModel.Edges.Count}";
        }

        return StatusString;
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
        var zoomDelta = delta * ZOOM_SPEED;
        var newZoom = Zoom * (1 + zoomDelta);
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
    public void SelectNode(CfgNodeView? node)
    {
        SelectedNode = node;
        _state.OnCfgNodeSelected(node);
    }

    /// <summary>
    /// Sets the hovered node
    /// </summary>
    /// <param name="node">Node to hover</param>
    public void HoverNode(CfgNodeView? node)
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
