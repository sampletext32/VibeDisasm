using System.Numerics;
using ImGuiNET;
using Silk.NET.OpenGL;
using VibeDisasm.CfgVisualizer.Abstractions;
using VibeDisasm.CfgVisualizer.Models;

namespace VibeDisasm.CfgVisualizer.ImGuiUI.Panels;

/// <summary>
/// Panel for visualizing the control flow graph
/// </summary>
public class CfgCanvasPanel : IImGuiPanel
{
    // View model
    private readonly CfgCanvasViewModel _viewModel;
    
    // Panning state
    private bool _isPanning = false;
    private Vector2 _lastMousePos = Vector2.Zero;
    
    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="viewModel">CFG canvas view model</param>
    public CfgCanvasPanel(CfgCanvasViewModel viewModel)
    {
        _viewModel = viewModel;
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
            if (_viewModel.CfgViewModel == null)
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
                var transform = _viewModel.GetTransform(canvasSize);
                
                // Draw edges
                foreach (var edge in _viewModel.CfgViewModel.Edges)
                {
                    DrawEdge(drawList, edge, canvasPos, transform);
                }
                
                // Draw nodes
                foreach (var node in _viewModel.CfgViewModel.Nodes)
                {
                    DrawNode(drawList, node, canvasPos, transform);
                }
                
                // Draw status text in the bottom-right corner
                string statusText = $"Zoom: {_viewModel.Zoom:F2}x | Pan: ({_viewModel.PanOffset.X:F1}, {_viewModel.PanOffset.Y:F1}) | Nodes: {_viewModel.CfgViewModel.Nodes.Count} | Edges: {_viewModel.CfgViewModel.Edges.Count}";
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
            float newZoom = _viewModel.Zoom * (1 - CfgCanvasViewModel.ZOOM_SPEED * 2);
            _viewModel.SetZoom(newZoom);
        }
        
        ImGui.SameLine();
        ImGui.SetNextItemWidth(100);
        float zoom = _viewModel.Zoom;
        if (ImGui.SliderFloat("##Zoom", ref zoom, CfgCanvasViewModel.MIN_ZOOM, CfgCanvasViewModel.MAX_ZOOM, "Zoom: %.2fx"))
        {
            _viewModel.SetZoom(zoom);
        }
        
        ImGui.SameLine();
        if (ImGui.Button("+"))
        {
            float newZoom = _viewModel.Zoom * (1 + CfgCanvasViewModel.ZOOM_SPEED * 2);
            _viewModel.SetZoom(newZoom);
        }
        
        ImGui.SameLine();
        if (ImGui.Button("Reset View"))
        {
            _viewModel.ResetView();
        }
        
        ImGui.SameLine();
        ImGui.Text("|");
        ImGui.SameLine();
        
        // Layout controls
        if (ImGui.Button("Auto Layout"))
        {
            if (_viewModel.CfgViewModel != null)
            {
                _viewModel.CfgViewModel.PerformLayout();
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
                _viewModel.AdjustPan(delta);
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
                _viewModel.AdjustPan(delta);
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
            var mouseWorldPos = _viewModel.ScreenToWorld(mousePos, canvasSize);
            
            // Apply zoom
            _viewModel.AdjustZoom(io.MouseWheel, mouseWorldPos);
        }
        
        // Handle zooming with keyboard
        if (ImGui.IsWindowFocused() && !ImGui.GetIO().WantTextInput)
        {
            // Zoom in with + key
            if (ImGui.IsKeyPressed(ImGuiKey.Equal) || ImGui.IsKeyPressed(ImGuiKey.KeypadAdd))
            {
                float newZoom = _viewModel.Zoom * (1 + CfgCanvasViewModel.ZOOM_SPEED);
                _viewModel.SetZoom(newZoom);
            }
            
            // Zoom out with - key
            if (ImGui.IsKeyPressed(ImGuiKey.Minus) || ImGui.IsKeyPressed(ImGuiKey.KeypadSubtract))
            {
                float newZoom = _viewModel.Zoom * (1 - CfgCanvasViewModel.ZOOM_SPEED);
                _viewModel.SetZoom(newZoom);
            }
            
            // Reset view with 0 key
            if (ImGui.IsKeyPressed(ImGuiKey._0) || ImGui.IsKeyPressed(ImGuiKey.Keypad0))
            {
                _viewModel.ResetView();
            }
            
            // Pan with arrow keys
            Vector2 keyPanDelta = Vector2.Zero;
            if (ImGui.IsKeyDown(ImGuiKey.LeftArrow)) keyPanDelta.X += 10.0f / _viewModel.Zoom;
            if (ImGui.IsKeyDown(ImGuiKey.RightArrow)) keyPanDelta.X -= 10.0f / _viewModel.Zoom;
            if (ImGui.IsKeyDown(ImGuiKey.UpArrow)) keyPanDelta.Y += 10.0f / _viewModel.Zoom;
            if (ImGui.IsKeyDown(ImGuiKey.DownArrow)) keyPanDelta.Y -= 10.0f / _viewModel.Zoom;
            
            if (keyPanDelta != Vector2.Zero)
            {
                _viewModel.AdjustPan(keyPanDelta);
            }
        }
        
        // Handle node selection
        if (ImGui.IsMouseClicked(ImGuiMouseButton.Left) && !ImGui.IsAnyItemHovered())
        {
            var worldPos = _viewModel.ScreenToWorld(mousePos, canvasSize);
            CfgNodeViewModel? selectedNode = null;
            
            // Check if a node was clicked
            foreach (var node in _viewModel.CfgViewModel?.Nodes ?? [])
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
            _viewModel.SelectNode(selectedNode);
        }
        
        // Handle node hovering
        if (!_isPanning)
        {
            var worldPos = _viewModel.ScreenToWorld(mousePos, canvasSize);
            CfgNodeViewModel? hoveredNode = null;
            
            // Check if a node is hovered
            foreach (var node in _viewModel.CfgViewModel?.Nodes ?? [])
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
            _viewModel.HoverNode(hoveredNode);
        }
    }
    
    /// <summary>
    /// Converts a screen position to a world position
    /// </summary>
    /// <param name="screenPos">Screen position</param>
    /// <param name="canvasPos">Canvas position</param>
    /// <param name="canvasSize">Canvas size</param>
    /// <returns>World position</returns>
    private Vector2 ScreenToWorld(Vector2 screenPos, Vector2 canvasPos, Vector2 canvasSize)
    {
        // Apply inverse transform
        var transform = Matrix3x2.CreateTranslation(-canvasSize / 2) * 
                       Matrix3x2.CreateScale(1.0f / _viewModel.Zoom) * 
                       Matrix3x2.CreateTranslation(-_viewModel.PanOffset);
                       
        return Vector2.Transform(screenPos, transform);
    }
    
    /// <summary>
    /// Converts a world position to a screen position
    /// </summary>
    /// <param name="worldPos">World position</param>
    /// <param name="canvasPos">Canvas position</param>
    /// <param name="canvasSize">Canvas size</param>
    /// <returns>Screen position</returns>
    private Vector2 WorldToScreen(Vector2 worldPos, Vector2 canvasPos, Vector2 canvasSize)
    {
        // Apply transform
        var transform = Matrix3x2.CreateTranslation(_viewModel.PanOffset) * 
                       Matrix3x2.CreateScale(_viewModel.Zoom) * 
                       Matrix3x2.CreateTranslation(canvasSize / 2);
                       
        return Vector2.Transform(worldPos, transform) + canvasPos;
    }
    
    /// <summary>
    /// Draws a node on the canvas
    /// </summary>
    /// <param name="drawList">Draw list</param>
    /// <param name="node">Node to draw</param>
    /// <param name="canvasPos">Canvas position</param>
    /// <param name="transform">Transform matrix</param>
    private void DrawNode(ImDrawListPtr drawList, CfgNodeViewModel node, Vector2 canvasPos, Matrix3x2 transform)
    {
        // Transform node position and size
        var pos = Vector2.Transform(node.Position, transform) + canvasPos;
        var size = node.Size * _viewModel.Zoom;
        
        // Calculate node rectangle
        var rect = new Rect(
            pos.X - size.X / 2,
            pos.Y - size.Y / 2,
            size.X,
            size.Y
        );
        
        // Determine node color
        var color = node.Color;
        if (node == _viewModel.SelectedNode)
            color = new Vector4(0.2f, 0.6f, 1.0f, 1.0f);
        else if (node == _viewModel.HoveredNode)
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
        var addressText = $"0x{node.Block.StartAddress:X8}";
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
            
        var contentTextSize = ImGui.CalcTextSize(contentText);
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
    private void DrawEdge(ImDrawListPtr drawList, CfgEdgeViewModel edge, Vector2 canvasPos, Matrix3x2 transform)
    {
        // Transform node positions
        var sourcePos = Vector2.Transform(edge.Source.Position, transform) + canvasPos;
        var targetPos = Vector2.Transform(edge.Target.Position, transform) + canvasPos;
        
        // Calculate edge points
        var sourceSize = edge.Source.Size * _viewModel.Zoom;
        var targetSize = edge.Target.Size * _viewModel.Zoom;
        
        // Calculate direction vector
        var dir = Vector2.Normalize(targetPos - sourcePos);
        
        // Calculate edge start and end points
        var start = sourcePos + dir * (sourceSize.Y / 2);
        var end = targetPos - dir * (targetSize.Y / 2);
        
        // Determine edge color
        var color = edge.Color;
        if (edge.Source == _viewModel.SelectedNode || edge.Target == _viewModel.SelectedNode)
            color = new Vector4(0.2f, 0.6f, 1.0f, 1.0f);
        else if (edge.Source == _viewModel.HoveredNode || edge.Target == _viewModel.HoveredNode)
            color = new Vector4(0.4f, 0.8f, 1.0f, 1.0f);
        
        // Draw edge line
        drawList.AddLine(
            start,
            end,
            ImGui.ColorConvertFloat4ToU32(color),
            2.0f
        );
        
        // Draw arrow
        var arrowSize = 10.0f * _viewModel.Zoom;
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
