using System.Numerics;
using ImGuiNET;
using VibeDisasm.CfgVisualizer.Models.Geometry;
using VibeDisasm.CfgVisualizer.Models.Graph;
using VibeDisasm.CfgVisualizer.ViewModels;

namespace VibeDisasm.CfgVisualizer.ImGuiUI.Input;

/// <summary>
/// Handles input interactions for the graph canvas
/// </summary>
public class GraphInputHandler
{
    private bool _isPanning;
    private Vector2 _lastMousePos;
    private readonly CfgCanvasPanelViewModel _panelViewModel;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="panelViewModel">Canvas panel view model</param>
    public GraphInputHandler(CfgCanvasPanelViewModel panelViewModel)
    {
        _panelViewModel = panelViewModel;
        _isPanning = false;
        _lastMousePos = Vector2.Zero;
    }

    /// <summary>
    /// Handles all input for the graph canvas
    /// </summary>
    /// <param name="canvasPos">Canvas position</param>
    /// <param name="canvasSize">Canvas size</param>
    public void HandleInput(Vector2 canvasPos, Vector2 canvasSize)
    {
        HandlePanning(canvasPos, canvasSize);
        HandleZooming(canvasPos, canvasSize);
        HandleKeyboardInput(canvasPos, canvasSize);
        HandleNodeSelection(canvasPos, canvasSize);
        HandleNodeHovering(canvasPos, canvasSize);
    }

    /// <summary>
    /// Handles panning with mouse
    /// </summary>
    private void HandlePanning(Vector2 canvasPos, Vector2 canvasSize)
    {
        var mousePos = ImGui.GetMousePos() - canvasPos;
        var io = ImGui.GetIO();
        
        // Start panning when middle mouse button is pressed
        if (ImGui.IsWindowHovered() && ImGui.IsMouseClicked(ImGuiMouseButton.Middle))
        {
            _isPanning = true;
            _lastMousePos = mousePos;
            ImGui.SetMouseCursor(ImGuiMouseCursor.ResizeAll);
        }
        
        // Stop panning when middle mouse button is released
        if (ImGui.IsMouseReleased(ImGuiMouseButton.Middle))
        {
            _isPanning = false;
            ImGui.SetMouseCursor(ImGuiMouseCursor.Arrow);
        }
        
        // Apply panning
        if (_isPanning)
        {
            if (ImGui.IsMouseDragging(ImGuiMouseButton.Middle))
            {
                var delta = mousePos - _lastMousePos;
                _panelViewModel.AdjustPan(delta);
                _lastMousePos = mousePos;
            }
        }
    }

    /// <summary>
    /// Handles zooming with mouse wheel
    /// </summary>
    private void HandleZooming(Vector2 canvasPos, Vector2 canvasSize)
    {
        var io = ImGui.GetIO();
        var mousePos = ImGui.GetMousePos() - canvasPos;
        
        // Handle mouse wheel zooming
        if (ImGui.IsWindowHovered() && io.MouseWheel != 0)
        {
            // Calculate mouse position in world space before zoom
            var mouseWorldPos = _panelViewModel.ScreenToWorld(mousePos, canvasSize);
            
            // Apply zoom
            _panelViewModel.AdjustZoom(io.MouseWheel, mouseWorldPos);
        }
    }

    /// <summary>
    /// Handles keyboard input for navigation
    /// </summary>
    private void HandleKeyboardInput(Vector2 canvasPos, Vector2 canvasSize)
    {
        if (ImGui.IsWindowFocused())
        {
            // Zoom in with + key
            if (ImGui.IsKeyPressed(ImGuiKey.Equal) || ImGui.IsKeyPressed(ImGuiKey.KeypadAdd))
            {
                float newZoom = _panelViewModel.Zoom * (1 + CfgCanvasPanelViewModel.ZOOM_SPEED);
                _panelViewModel.SetZoom(newZoom);
            }
            
            // Zoom out with - key
            if (ImGui.IsKeyPressed(ImGuiKey.Minus) || ImGui.IsKeyPressed(ImGuiKey.KeypadSubtract))
            {
                float newZoom = _panelViewModel.Zoom * (1 - CfgCanvasPanelViewModel.ZOOM_SPEED);
                _panelViewModel.SetZoom(newZoom);
            }
            
            // Reset view with 0 key
            if (ImGui.IsKeyPressed(ImGuiKey._0) || ImGui.IsKeyPressed(ImGuiKey.Keypad0))
            {
                _panelViewModel.ResetView();
            }
            
            // Pan with arrow keys
            var keyPanDelta = Vector2.Zero;
            if (ImGui.IsKeyDown(ImGuiKey.LeftArrow)) keyPanDelta.X += 10.0f / _panelViewModel.Zoom;
            if (ImGui.IsKeyDown(ImGuiKey.RightArrow)) keyPanDelta.X -= 10.0f / _panelViewModel.Zoom;
            if (ImGui.IsKeyDown(ImGuiKey.UpArrow)) keyPanDelta.Y += 10.0f / _panelViewModel.Zoom;
            if (ImGui.IsKeyDown(ImGuiKey.DownArrow)) keyPanDelta.Y -= 10.0f / _panelViewModel.Zoom;
            
            if (keyPanDelta != Vector2.Zero)
            {
                _panelViewModel.AdjustPan(keyPanDelta);
            }
        }
    }

    /// <summary>
    /// Handles node selection on mouse click
    /// </summary>
    private void HandleNodeSelection(Vector2 canvasPos, Vector2 canvasSize)
    {
        // Handle node selection
        if (ImGui.IsMouseClicked(ImGuiMouseButton.Left) && !ImGui.IsAnyItemHovered())
        {
            var mousePos = ImGui.GetMousePos() - canvasPos;
            var worldPos = _panelViewModel.ScreenToWorld(mousePos, canvasSize);
            CfgNodeView? selectedNode = null;
            
            // Check if a node was clicked
            foreach (var node in _panelViewModel.CfgViewModel?.Nodes ?? [])
            {
                var nodeRect = new Rect(
                    node.Position.X - node.Size.X / 2,
                    node.Position.Y - node.Size.Y / 2,
                    node.Size.X,
                    node.Size.Y
                );
                
                if (nodeRect.Contains(worldPos))
                {
                    selectedNode = node;
                    break;
                }
            }
            
            // Update selected node in view model
            _panelViewModel.SelectNode(selectedNode);
        }
    }

    /// <summary>
    /// Handles node hovering on mouse move
    /// </summary>
    private void HandleNodeHovering(Vector2 canvasPos, Vector2 canvasSize)
    {
        // Handle node hovering
        if (!_isPanning)
        {
            var mousePos = ImGui.GetMousePos() - canvasPos;
            var worldPos = _panelViewModel.ScreenToWorld(mousePos, canvasSize);
            CfgNodeView? hoveredNode = null;
            
            // Check if a node is hovered
            foreach (var node in _panelViewModel.CfgViewModel?.Nodes ?? [])
            {
                var nodeRect = new Rect(
                    node.Position.X - node.Size.X / 2,
                    node.Position.Y - node.Size.Y / 2,
                    node.Size.X,
                    node.Size.Y
                );
                
                if (nodeRect.Contains(worldPos))
                {
                    hoveredNode = node;
                    break;
                }
            }
            
            // Update hovered node in view model
            _panelViewModel.HoverNode(hoveredNode);
        }
    }
}
