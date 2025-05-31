using System.Numerics;
using ImGuiNET;
using VibeDisasm.CfgVisualizer.ViewModels;

namespace VibeDisasm.CfgVisualizer.ImGuiUI.Controls;

/// <summary>
/// Status bar for graph canvas showing zoom level, pan position, and node/edge counts
/// </summary>
public class GraphStatusBar
{
    private readonly CfgCanvasPanelViewModel _panelViewModel;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="panelViewModel">Canvas panel view model</param>
    public GraphStatusBar(CfgCanvasPanelViewModel panelViewModel)
    {
        _panelViewModel = panelViewModel;
    }

    /// <summary>
    /// Renders the status bar
    /// </summary>
    /// <param name="canvasPos">Canvas position</param>
    /// <param name="canvasSize">Canvas size</param>
    public void Render(Vector2 canvasPos, Vector2 canvasSize, ImDrawListPtr drawList)
    {
        if (_panelViewModel.CfgViewModel == null)
        {
            return;
        }

        // Create status text
        var statusText = _panelViewModel.GetStatusString();

        // Calculate text size and position
        var statusTextSize = ImGui.CalcTextSize(statusText);
        var textPos = new Vector2(
            canvasPos.X + canvasSize.X - statusTextSize.X - 10,
            canvasPos.Y + canvasSize.Y - statusTextSize.Y - 10
        );

        // Draw text
        drawList.AddText(
            textPos,
            ImGui.ColorConvertFloat4ToU32(new Vector4(0.7f, 0.7f, 0.7f, 1.0f)),
            statusText
        );
    }
}
