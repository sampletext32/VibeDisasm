using System.Numerics;
using ImGuiNET;
using VibeDisasm.CfgVisualizer.Models.Geometry;
using VibeDisasm.CfgVisualizer.Models.Graph;

namespace VibeDisasm.CfgVisualizer.ImGuiUI.Rendering;

/// <summary>
/// Handles rendering of graph elements (nodes and edges)
/// </summary>
public class GraphRenderer
{
    /// <summary>
    /// Renders all nodes in the graph
    /// </summary>
    /// <param name="drawList">ImGui draw list</param>
    /// <param name="nodes">Collection of nodes to render</param>
    /// <param name="canvasPos">Canvas position</param>
    /// <param name="transform">Transform matrix</param>
    /// <param name="selectedNode">Currently selected node</param>
    /// <param name="hoveredNode">Currently hovered node</param>
    /// <param name="zoom">Current zoom level</param>
    public void RenderNodes(
        ImDrawListPtr drawList, 
        IEnumerable<CfgNodeView> nodes, 
        Vector2 canvasPos, 
        Matrix3x2 transform, 
        CfgNodeView? selectedNode, 
        CfgNodeView? hoveredNode, 
        float zoom)
    {
        foreach (var node in nodes)
        {
            RenderNode(drawList, node, canvasPos, transform, selectedNode, hoveredNode, zoom);
        }
    }

    /// <summary>
    /// Renders all edges in the graph
    /// </summary>
    /// <param name="drawList">ImGui draw list</param>
    /// <param name="edges">Collection of edges to render</param>
    /// <param name="canvasPos">Canvas position</param>
    /// <param name="transform">Transform matrix</param>
    /// <param name="selectedNode">Currently selected node</param>
    /// <param name="hoveredNode">Currently hovered node</param>
    /// <param name="zoom">Current zoom level</param>
    public void RenderEdges(
        ImDrawListPtr drawList, 
        IEnumerable<CfgEdgeView> edges, 
        Vector2 canvasPos, 
        Matrix3x2 transform, 
        CfgNodeView? selectedNode, 
        CfgNodeView? hoveredNode, 
        float zoom)
    {
        foreach (var edge in edges)
        {
            RenderEdge(drawList, edge, canvasPos, transform, selectedNode, hoveredNode, zoom);
        }
    }

    /// <summary>
    /// Renders a single node
    /// </summary>
    private void RenderNode(
        ImDrawListPtr drawList, 
        CfgNodeView node, 
        Vector2 canvasPos, 
        Matrix3x2 transform, 
        CfgNodeView? selectedNode, 
        CfgNodeView? hoveredNode, 
        float zoom)
    {
        // Transform node position and size
        var pos = Vector2.Transform(node.Position, transform) + canvasPos;
        var size = node.Size * zoom;
        
        // Calculate node rectangle
        var rect = new Rect(
            pos.X - size.X / 2,
            pos.Y - size.Y / 2,
            size.X,
            size.Y
        );
        
        // Determine node color
        var color = node.Color;
        if (node == selectedNode)
            color = new Vector4(0.2f, 0.6f, 1.0f, 1.0f);
        else if (node == hoveredNode)
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
    /// Renders a single edge
    /// </summary>
    private void RenderEdge(
        ImDrawListPtr drawList, 
        CfgEdgeView edge, 
        Vector2 canvasPos, 
        Matrix3x2 transform, 
        CfgNodeView? selectedNode, 
        CfgNodeView? hoveredNode, 
        float zoom)
    {
        // Transform node positions
        var sourcePos = Vector2.Transform(edge.Source.Position, transform) + canvasPos;
        var targetPos = Vector2.Transform(edge.Target.Position, transform) + canvasPos;
        
        // Calculate edge points
        var sourceSize = edge.Source.Size * zoom;
        var targetSize = edge.Target.Size * zoom;
        
        // Calculate direction vector
        var dir = Vector2.Normalize(targetPos - sourcePos);

        // Determine edge color
        var color = edge.Color;
        if (edge.Source == selectedNode || edge.Target == selectedNode)
            color = new Vector4(0.2f, 0.6f, 1.0f, 1.0f);
        else if (edge.Source == hoveredNode || edge.Target == hoveredNode)
            color = new Vector4(0.4f, 0.8f, 1.0f, 1.0f);
        
        // Draw edge line
        var start = sourcePos + dir * (Math.Max(sourceSize.X, sourceSize.Y) / 2);
        var end = targetPos - dir * (Math.Max(targetSize.X, targetSize.Y) / 2);
        
        // Draw edge line - fallthrough edges are thicker, taken branches are thinner
        drawList.AddLine(
            start,
            end,
            ImGui.ColorConvertFloat4ToU32(color),
            edge.IsFallthrough ? 2.0f : 1.0f
        );
        
        // Draw arrow - adjust size based on edge type
        var arrowSize = edge.IsFallthrough ? 12.0f * zoom : 8.0f * zoom;
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
}
