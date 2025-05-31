using System.Numerics;
using ImGuiNET;
using VibeDisasm.CfgVisualizer.ViewModels;

namespace VibeDisasm.CfgVisualizer.ImGuiUI.Controls;

/// <summary>
/// Toolbar for graph canvas with zoom and layout controls
/// </summary>
public class GraphToolbar
{
    private readonly CfgCanvasPanelViewModel _panelViewModel;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="panelViewModel">Canvas panel view model</param>
    public GraphToolbar(CfgCanvasPanelViewModel panelViewModel)
    {
        _panelViewModel = panelViewModel;
    }

    /// <summary>
    /// Renders the toolbar
    /// </summary>
    public void Render()
    {
        ImGui.PushStyleVar(ImGuiStyleVar.ItemSpacing, new Vector2(4, 4));

        // Zoom controls
        RenderZoomControls();

        // Layout controls
        RenderLayoutControls();

        ImGui.PopStyleVar();
    }

    /// <summary>
    /// Renders zoom controls
    /// </summary>
    private void RenderZoomControls()
    {
        // Zoom out button
        if (ImGui.Button("-"))
        {
            var newZoom = _panelViewModel.Zoom * (1 - CfgCanvasPanelViewModel.ZOOM_SPEED * 2);
            _panelViewModel.SetZoom(newZoom);
        }

        ImGui.SameLine();

        // Zoom slider
        ImGui.SetNextItemWidth(100);
        var zoom = _panelViewModel.Zoom;
        if (ImGui.SliderFloat("##Zoom", ref zoom, CfgCanvasPanelViewModel.MIN_ZOOM, CfgCanvasPanelViewModel.MAX_ZOOM, "Zoom: %.2fx"))
        {
            _panelViewModel.SetZoom(zoom);
        }

        ImGui.SameLine();

        // Zoom in button
        if (ImGui.Button("+"))
        {
            var newZoom = _panelViewModel.Zoom * (1 + CfgCanvasPanelViewModel.ZOOM_SPEED * 2);
            _panelViewModel.SetZoom(newZoom);
        }

        ImGui.SameLine();

        // Reset view button
        if (ImGui.Button("Reset View"))
        {
            _panelViewModel.ResetView();
        }
    }

    /// <summary>
    /// Renders layout controls
    /// </summary>
    private void RenderLayoutControls()
    {
        ImGui.SameLine();

        // Auto layout button
        if (ImGui.Button("Auto Layout"))
        {
            if (_panelViewModel.CfgViewModel != null)
            {
                _panelViewModel.CfgViewModel.PerformLayout();
            }
        }
    }
}
