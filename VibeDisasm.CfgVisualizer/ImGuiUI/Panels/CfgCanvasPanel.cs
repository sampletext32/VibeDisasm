using System.Numerics;
using ImGuiNET;
using Silk.NET.OpenGL;
using VibeDisasm.CfgVisualizer.Abstractions;
using VibeDisasm.CfgVisualizer.Models.Graph;
using VibeDisasm.CfgVisualizer.ViewModels;

namespace VibeDisasm.CfgVisualizer.ImGuiUI.Panels;

/// <summary>
/// Panel for visualizing the control flow graph
/// </summary>
public class CfgCanvasPanel : IImGuiPanel
{
    // View model
    private readonly CfgCanvasPanelViewModel _panelViewModel;
    
    // Panning state
    private bool _isPanning = false;
    private Vector2 _lastMousePos = Vector2.Zero;
    
    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="panelViewModel">CFG canvas view model</param>
    public CfgCanvasPanel(CfgCanvasPanelViewModel panelViewModel)
    {
        _panelViewModel = panelViewModel;
    }
    
    /// <summary>
    /// Renders the CFG canvas panel
    /// </summary>
    public void OnImGuiRender()
    {
        // Begin the panel
        bool isOpen = ImGui.Begin("CFG Canvas");
        if (isOpen)
        {
            if (_panelViewModel.CfgViewModel == null)
            {
                ImGui.TextColored(new Vector4(0.7f, 0.7f, 0.7f, 1.0f), "No CFG data available");
            }
            else
            {
                // Create a toolbar for zoom controls
                RenderToolbar();
                
                // Get canvas size and position
                var canvasPos = ImGui.GetCursorScreenPos();
                var canvasSize = ImGui.GetContentRegionAvail();
                var drawList = ImGui.GetWindowDrawList();
                
                // Handle input
                HandleCanvasInput(canvasPos, canvasSize);
                
                // Calculate transform
                var transform = _panelViewModel.GetTransform(canvasSize);
                
                // Draw edges
                foreach (var edge in _panelViewModel.CfgViewModel.Edges)
                {
                    DrawEdge(drawList, edge, canvasPos, transform);
                }
                
                // Draw nodes
                foreach (var node in _panelViewModel.CfgViewModel.Nodes)
                {
                    DrawNode(drawList, node, canvasPos, transform);
                }
                
                // Draw status text in the bottom-right corner
                string statusText = $"Zoom: {_panelViewModel.Zoom:F2}x | Pan: ({_panelViewModel.PanOffset.X:F1}, {_panelViewModel.PanOffset.Y:F1}) | Nodes: {_panelViewModel.CfgViewModel.Nodes.Count} | Edges: {_panelViewModel.CfgViewModel.Edges.Count}";
                var statusTextSize = ImGui.CalcTextSize(statusText);
                drawList.AddText(
                    new Vector2(canvasPos.X + canvasSize.X - statusTextSize.X - 10, canvasPos.Y + canvasSize.Y - statusTextSize.Y - 10),
                    ImGui.ColorConvertFloat4ToU32(new Vector4(0.7f, 0.7f, 0.7f, 1.0f)),
                    statusText
                );
                
                // Render status bar
                RenderStatusBar();
            }
            
            // End the panel only if Begin returned true
            ImGui.End();
        }
    }
    
    /// <summary>
    /// Renders the toolbar with zoom controls
    /// </summary>
    private void RenderToolbar()
    {
        ImGui.PushStyleVar(ImGuiStyleVar.ItemSpacing, new Vector2(4, 4));
        
        // Zoom controls
        if (ImGui.Button("-"))
        {
            float newZoom = _panelViewModel.Zoom * (1 - CfgCanvasPanelViewModel.ZOOM_SPEED * 2);
            _panelViewModel.SetZoom(newZoom);
        }
        
        ImGui.SameLine();
        ImGui.SetNextItemWidth(100);
        float zoom = _panelViewModel.Zoom;
        if (ImGui.SliderFloat("##Zoom", ref zoom, CfgCanvasPanelViewModel.MIN_ZOOM, CfgCanvasPanelViewModel.MAX_ZOOM, "Zoom: %.2fx"))
        {
            _panelViewModel.SetZoom(zoom);
        }
        
        ImGui.SameLine();
        if (ImGui.Button("+"))
        {
            float newZoom = _panelViewModel.Zoom * (1 + CfgCanvasPanelViewModel.ZOOM_SPEED * 2);
            _panelViewModel.SetZoom(newZoom);
        }
        
        ImGui.SameLine();
        if (ImGui.Button("Reset View"))
        {
            _panelViewModel.ResetView();
        }
        
        ImGui.SameLine();
        ImGui.Text("|");
        ImGui.SameLine();
        
        // Layout controls
        if (ImGui.Button("Auto Layout"))
        {
            if (_panelViewModel.CfgViewModel != null)
            {
                _panelViewModel.CfgViewModel.PerformLayout();
            }
        }
        
        ImGui.PopStyleVar();
    }
    
    /// <summary>
    /// Renders the status bar
    /// </summary>
    private void RenderStatusBar()
    {
        ImGui.Text("Navigation: Right/Middle mouse to pan | Mouse wheel to zoom | Arrow keys to pan | +/- to zoom | 0 to reset view");
    }
    
    /// <summary>
    /// Handles input for the canvas
    /// </summary>
    /// <param name="canvasPos">Canvas position</param>
    /// <param name="canvasSize">Canvas size</param>
    private void HandleCanvasInput(Vector2 canvasPos, Vector2 canvasSize)
    {
        // Get mouse position relative to canvas
        var mousePos = ImGui.GetMousePos() - canvasPos;
        
        // Handle panning with right mouse button
        if (ImGui.IsMouseDragging(ImGuiMouseButton.Right))
        {
            if (!_isPanning)
            {
                _isPanning = true;
                _lastMousePos = mousePos;
            }
            else
            {
                var delta = mousePos - _lastMousePos;
                _panelViewModel.AdjustPan(delta);
                _lastMousePos = mousePos;
            }
        }
        // Handle panning with middle mouse button
        else if (ImGui.IsMouseDragging(ImGuiMouseButton.Middle))
        {
            if (!_isPanning)
            {
                _isPanning = true;
                _lastMousePos = mousePos;
            }
            else
            {
                var delta = mousePos - _lastMousePos;
                _panelViewModel.AdjustPan(delta);
                _lastMousePos = mousePos;
            }
        }
        else
        {
            _isPanning = false;
        }
        
        // Handle zooming with mouse wheel
        var io = ImGui.GetIO();
        if (ImGui.IsWindowHovered() && io.MouseWheel != 0)
        {
            // Calculate mouse position in world space before zoom
            var mouseWorldPos = _panelViewModel.ScreenToWorld(mousePos, canvasSize);
            
            // Apply zoom
            _panelViewModel.AdjustZoom(io.MouseWheel, mouseWorldPos);
        }
        
        // Handle zooming with keyboard
        if (ImGui.IsWindowFocused() && !ImGui.GetIO().WantTextInput)
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
            Vector2 keyPanDelta = Vector2.Zero;
            if (ImGui.IsKeyDown(ImGuiKey.LeftArrow)) keyPanDelta.X += 10.0f / _panelViewModel.Zoom;
            if (ImGui.IsKeyDown(ImGuiKey.RightArrow)) keyPanDelta.X -= 10.0f / _panelViewModel.Zoom;
            if (ImGui.IsKeyDown(ImGuiKey.UpArrow)) keyPanDelta.Y += 10.0f / _panelViewModel.Zoom;
            if (ImGui.IsKeyDown(ImGuiKey.DownArrow)) keyPanDelta.Y -= 10.0f / _panelViewModel.Zoom;
            
            if (keyPanDelta != Vector2.Zero)
            {
                _panelViewModel.AdjustPan(keyPanDelta);
            }
        }
        
        // Handle node selection
        if (ImGui.IsMouseClicked(ImGuiMouseButton.Left) && !ImGui.IsAnyItemHovered())
        {
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
        
        // Handle node hovering
        if (!_isPanning)
        {
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

    /// <summary>
    /// Draws a node on the canvas
    /// </summary>
    /// <param name="drawList">Draw list</param>
    /// <param name="node">Node to draw</param>
    /// <param name="canvasPos">Canvas position</param>
    /// <param name="transform">Transform matrix</param>
    private void DrawNode(ImDrawListPtr drawList, CfgNodeView node, Vector2 canvasPos, Matrix3x2 transform)
    {
        // Transform node position and size
        var pos = Vector2.Transform(node.Position, transform) + canvasPos;
        var size = node.Size * _panelViewModel.Zoom;
        
        // Calculate node rectangle
        var rect = new Rect(
            pos.X - size.X / 2,
            pos.Y - size.Y / 2,
            size.X,
            size.Y
        );
        
        // Determine node color
        var color = node.Color;
        if (node == _panelViewModel.SelectedNode)
            color = new Vector4(0.2f, 0.6f, 1.0f, 1.0f);
        else if (node == _panelViewModel.HoveredNode)
            color = new Vector4(0.4f, 0.8f, 1.0f, 1.0f);
        
        // Draw node background
        drawList.AddRectFilled(
            new Vector2(rect.X, rect.Y),
            new Vector2(rect.X + rect.Width, rect.Y + rect.Height),
            ImGui.ColorConvertFloat4ToU32(color),
            5.0f
        );
        
        // Draw node border
        drawList.AddRect(
            new Vector2(rect.X, rect.Y),
            new Vector2(rect.X + rect.Width, rect.Y + rect.Height),
            ImGui.ColorConvertFloat4ToU32(new Vector4(0.0f, 0.0f, 0.0f, 1.0f)),
            5.0f,
            ImDrawFlags.None,
            2.0f
        );
        
        // Draw node address
        var addressText = node.Block.ComputedStartAddressView;
        var addressTextSize = ImGui.CalcTextSize(addressText);
        drawList.AddText(
            new Vector2(
                rect.X + (rect.Width - addressTextSize.X) / 2,
                rect.Y + 5
            ),
            ImGui.ColorConvertFloat4ToU32(new Vector4(0.0f, 0.0f, 0.0f, 1.0f)),
            addressText
        );
        
        // Draw node content (first few instructions)
        var contentText = string.Join("\n", node.Block.Instructions.Take(3).Select(i => i.ToString()));
        if (node.Block.Instructions.Count > 3)
            contentText += "\n...";

        drawList.AddText(
            new Vector2(
                rect.X + 5,
                rect.Y + addressTextSize.Y + 10
            ),
            ImGui.ColorConvertFloat4ToU32(new Vector4(0.0f, 0.0f, 0.0f, 1.0f)),
            contentText
        );
    }
    
    /// <summary>
    /// Draws an edge on the canvas
    /// </summary>
    /// <param name="drawList">Draw list</param>
    /// <param name="edge">Edge to draw</param>
    /// <param name="canvasPos">Canvas position</param>
    /// <param name="transform">Transform matrix</param>
    private void DrawEdge(ImDrawListPtr drawList, CfgEdgeView edge, Vector2 canvasPos, Matrix3x2 transform)
    {
        // Transform node positions
        var sourcePos = Vector2.Transform(edge.Source.Position, transform) + canvasPos;
        var targetPos = Vector2.Transform(edge.Target.Position, transform) + canvasPos;
        
        // Calculate edge points
        var sourceSize = edge.Source.Size * _panelViewModel.Zoom;
        var targetSize = edge.Target.Size * _panelViewModel.Zoom;
        
        // Calculate direction vector
        var dir = Vector2.Normalize(targetPos - sourcePos);
        
        // Calculate edge start and end points
        var start = sourcePos + dir * (sourceSize.Y / 2);
        var end = targetPos - dir * (targetSize.Y / 2);
        
        // Determine edge color
        var color = edge.Color;
        if (edge.Source == _panelViewModel.SelectedNode || edge.Target == _panelViewModel.SelectedNode)
            color = new Vector4(0.2f, 0.6f, 1.0f, 1.0f);
        else if (edge.Source == _panelViewModel.HoveredNode || edge.Target == _panelViewModel.HoveredNode)
            color = new Vector4(0.4f, 0.8f, 1.0f, 1.0f);
        
        // Draw edge line
        drawList.AddLine(
            start,
            end,
            ImGui.ColorConvertFloat4ToU32(color),
            2.0f
        );
        
        // Draw arrow
        var arrowSize = 10.0f * _panelViewModel.Zoom;
        var arrowDir = Vector2.Normalize(end - start);
        var arrowPerp = new Vector2(-arrowDir.Y, arrowDir.X);
        
        var arrowPoint1 = end - arrowDir * arrowSize + arrowPerp * arrowSize * 0.5f;
        var arrowPoint2 = end - arrowDir * arrowSize - arrowPerp * arrowSize * 0.5f;
        
        drawList.AddTriangleFilled(
            end,
            arrowPoint1,
            arrowPoint2,
            ImGui.ColorConvertFloat4ToU32(color)
        );
    }
    
    /// <summary>
    /// Rectangle structure for hit testing
    /// </summary>
    private struct Rect
    {
        public float X;
        public float Y;
        public float Width;
        public float Height;
        
        public Rect(float x, float y, float width, float height)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }
        
        public bool Contains(Vector2 point)
        {
            return point.X >= X && point.X <= X + Width &&
                   point.Y >= Y && point.Y <= Y + Height;
        }
    }
}
