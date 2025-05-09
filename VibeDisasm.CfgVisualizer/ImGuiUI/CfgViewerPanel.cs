using System.Numerics;
using ImGuiNET;
using Silk.NET.OpenGL;
using VibeDisasm.CfgVisualizer.Abstractions;
using VibeDisasm.CfgVisualizer.Models;
using VibeDisasm.DecompilerEngine.ControlFlow;

namespace VibeDisasm.CfgVisualizer.ImGuiUI;

/// <summary>
/// Panel for visualizing control flow graphs
/// </summary>
public class CfgViewerPanel : IImGuiPanel
{
    // Current CFG view model
    private CfgViewModel? _cfgViewModel;
    
    // Panning and zooming
    private Vector2 _panOffset = Vector2.Zero;
    private float _zoom = 1.0f;
    private bool _isPanning = false;
    private Vector2 _lastMousePos = Vector2.Zero;
    
    // Node selection
    private CfgNodeViewModel? _selectedNode;
    private CfgNodeViewModel? _hoveredNode;
    
    // File loading
    private string _loadedFilePath = string.Empty;
    private string _selectedFunctionName = string.Empty;
    private List<string> _availableFunctions = [];
    
    /// <summary>
    /// Renders the CFG viewer panel
    /// </summary>
    public void OnImGuiRender()
    {
        ImGui.Begin("CFG Viewer");

        // File loading UI
        RenderFileLoadingUI();
        
        // CFG visualization
        RenderCfgVisualization();
        
        // Node details panel
        RenderNodeDetailsPanel();
        
        ImGui.End();
    }
    
    /// <summary>
    /// Renders the file loading UI
    /// </summary>
    private void RenderFileLoadingUI()
    {
        ImGui.Text("File: " + (_loadedFilePath.Length > 0 ? Path.GetFileName(_loadedFilePath) : "None"));
        
        ImGui.SameLine();
        
        if (ImGui.Button("Load PE File"))
        {
            // TODO: Implement file dialog
            // For now, we'll just use a hardcoded path for testing
            // TryLoadFile(@"C:\path\to\test.exe");
        }
    }
    
    /// <summary>
    /// Renders the CFG visualization
    /// </summary>
    private void RenderCfgVisualization()
    {
        if (_cfgViewModel == null)
            return;
            
        // Create a child window for the CFG visualization
        if (ImGui.BeginChild("CFG Canvas", new Vector2(0, -ImGui.GetFrameHeightWithSpacing())))
        {
            // Get canvas size and position
            var canvasPos = ImGui.GetCursorScreenPos();
            var canvasSize = ImGui.GetContentRegionAvail();
            var drawList = ImGui.GetWindowDrawList();
            
            // Handle input
            HandleCanvasInput(canvasPos, canvasSize);
            
            // Calculate transform
            var transform = Matrix3x2.CreateTranslation(_panOffset) * 
                           Matrix3x2.CreateScale(_zoom) * 
                           Matrix3x2.CreateTranslation(canvasSize / 2);
            
            // Draw edges
            foreach (var edge in _cfgViewModel.Edges)
            {
                DrawEdge(drawList, edge, canvasPos, transform);
            }
            
            // Draw nodes
            foreach (var node in _cfgViewModel.Nodes)
            {
                DrawNode(drawList, node, canvasPos, transform);
            }
            
            ImGui.EndChild();
        }
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
        
        // Handle panning
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
                _panOffset += delta / _zoom;
                _lastMousePos = mousePos;
            }
        }
        else
        {
            _isPanning = false;
        }
        
        // Handle zooming
        var io = ImGui.GetIO();
        if (ImGui.IsWindowHovered() && io.MouseWheel != 0)
        {
            // Calculate mouse position in world space before zoom
            var mouseWorldPos = ScreenToWorld(mousePos, canvasPos, canvasSize);
            
            // Apply zoom
            _zoom = Math.Clamp(_zoom * (1 + io.MouseWheel * 0.1f), 0.1f, 10.0f);
            
            // Calculate mouse position in world space after zoom
            var newMouseWorldPos = ScreenToWorld(mousePos, canvasPos, canvasSize);
            
            // Adjust pan offset to keep mouse position fixed
            _panOffset += (mouseWorldPos - newMouseWorldPos);
        }
        
        // Handle node selection
        if (ImGui.IsMouseClicked(ImGuiMouseButton.Left) && !ImGui.IsAnyItemHovered())
        {
            var worldPos = ScreenToWorld(mousePos, canvasPos, canvasSize);
            _selectedNode = null;
            
            // Check if a node was clicked
            foreach (var node in _cfgViewModel?.Nodes ?? [])
            {
                var nodeRect = new Rect(
                    node.Position.X - node.Size.X / 2,
                    node.Position.Y - node.Size.Y / 2,
                    node.Size.X,
                    node.Size.Y
                );
                
                if (nodeRect.Contains(worldPos))
                {
                    _selectedNode = node;
                    break;
                }
            }
        }
        
        // Handle node hovering
        if (!_isPanning)
        {
            var worldPos = ScreenToWorld(mousePos, canvasPos, canvasSize);
            _hoveredNode = null;
            
            // Check if a node is hovered
            foreach (var node in _cfgViewModel?.Nodes ?? [])
            {
                var nodeRect = new Rect(
                    node.Position.X - node.Size.X / 2,
                    node.Position.Y - node.Size.Y / 2,
                    node.Size.X,
                    node.Size.Y
                );
                
                if (nodeRect.Contains(worldPos))
                {
                    _hoveredNode = node;
                    break;
                }
            }
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
                       Matrix3x2.CreateScale(1.0f / _zoom) * 
                       Matrix3x2.CreateTranslation(-_panOffset);
                       
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
        var transform = Matrix3x2.CreateTranslation(_panOffset) * 
                       Matrix3x2.CreateScale(_zoom) * 
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
        var size = node.Size * _zoom;
        
        // Calculate node rectangle
        var rect = new Rect(
            pos.X - size.X / 2,
            pos.Y - size.Y / 2,
            size.X,
            size.Y
        );
        
        // Determine node color
        var color = node.Color;
        if (node == _selectedNode)
            color = new Vector4(0.2f, 0.6f, 1.0f, 1.0f);
        else if (node == _hoveredNode)
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
        var sourceSize = edge.Source.Size * _zoom;
        var targetSize = edge.Target.Size * _zoom;
        
        // Calculate direction vector
        var dir = Vector2.Normalize(targetPos - sourcePos);
        
        // Calculate edge start and end points
        var start = sourcePos + dir * (sourceSize.Y / 2);
        var end = targetPos - dir * (targetSize.Y / 2);
        
        // Determine edge color
        var color = edge.Color;
        if (edge.Source == _selectedNode || edge.Target == _selectedNode)
            color = new Vector4(0.2f, 0.6f, 1.0f, 1.0f);
        else if (edge.Source == _hoveredNode || edge.Target == _hoveredNode)
            color = new Vector4(0.4f, 0.8f, 1.0f, 1.0f);
        
        // Draw edge line
        drawList.AddLine(
            start,
            end,
            ImGui.ColorConvertFloat4ToU32(color),
            2.0f
        );
        
        // Draw arrow
        var arrowSize = 10.0f * _zoom;
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
    /// Renders the node details panel
    /// </summary>
    private void RenderNodeDetailsPanel()
    {
        if (_selectedNode == null)
            return;
            
        ImGui.BeginChild("Node Details", new Vector2(0, 0));
        
        ImGui.Text($"Address: 0x{_selectedNode.Block.StartAddress:X8}");
        ImGui.Text($"Instructions: {_selectedNode.Block.Instructions.Count}");
        
        ImGui.Separator();
        
        if (ImGui.BeginTable("Instructions", 2, ImGuiTableFlags.Borders | ImGuiTableFlags.RowBg))
        {
            ImGui.TableSetupColumn("Address");
            ImGui.TableSetupColumn("Instruction");
            ImGui.TableHeadersRow();
            
            foreach (var instruction in _selectedNode.Block.Instructions)
            {
                ImGui.TableNextRow();
                
                ImGui.TableNextColumn();
                ImGui.Text($"0x{instruction.Address:X8}");
                
                ImGui.TableNextColumn();
                ImGui.Text(instruction.ToString());
            }
            
            ImGui.EndTable();
        }
        
        ImGui.EndChild();
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
