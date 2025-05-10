using System.Numerics;
using ImGuiNET;
using VibeDisasm.CfgVisualizer.Abstractions;
using VibeDisasm.CfgVisualizer.ImGuiUI.Controls;
using VibeDisasm.CfgVisualizer.ImGuiUI.Input;
using VibeDisasm.CfgVisualizer.ImGuiUI.Rendering;
using VibeDisasm.CfgVisualizer.ViewModels;

namespace VibeDisasm.CfgVisualizer.ImGuiUI.Panels;

/// <summary>
/// Panel for visualizing the control flow graph
/// </summary>
public class CfgCanvasPanel : IImGuiPanel
{
    // Components
    private readonly CfgCanvasPanelViewModel _panelViewModel;
    private readonly GraphRenderer _renderer;
    private readonly GraphInputHandler _inputHandler;
    private readonly GraphToolbar _toolbar;
    private readonly GraphStatusBar _statusBar;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="panelViewModel">CFG canvas view model</param>
    public CfgCanvasPanel(CfgCanvasPanelViewModel panelViewModel)
    {
        _panelViewModel = panelViewModel;
        _renderer = new GraphRenderer();
        _inputHandler = new GraphInputHandler(panelViewModel);
        _toolbar = new GraphToolbar(panelViewModel);
        _statusBar = new GraphStatusBar(panelViewModel);
    }
    
    /// <summary>
    /// Renders the CFG canvas panel
    /// </summary>
    public void OnImGuiRender()
    {
        // Begin the panel
        bool isOpen = ImGui.Begin("CFG Canvas");
        if (!isOpen)
        {
            ImGui.End();
            return;
        }

        if (_panelViewModel.CfgViewModel == null)
        {
            ImGui.TextColored(new Vector4(0.7f, 0.7f, 0.7f, 1.0f), "No CFG data available");
            ImGui.End();
            return;
        }

        // Create a toolbar for zoom controls
        _toolbar.Render();
        
        // Get canvas size and position
        var canvasPos = ImGui.GetCursorScreenPos();
        var canvasSize = ImGui.GetContentRegionAvail();
        var drawList = ImGui.GetWindowDrawList();
        
        // Handle input
        _inputHandler.HandleInput(canvasPos, canvasSize);
        
        // Calculate transform
        var transform = _panelViewModel.GetTransform(canvasSize);
        
        // Draw edges
        _renderer.RenderEdges(
            drawList, 
            _panelViewModel.CfgViewModel.Edges, 
            canvasPos, 
            transform, 
            _panelViewModel.SelectedNode, 
            _panelViewModel.HoveredNode, 
            _panelViewModel.Zoom
        );
        
        // Draw nodes
        _renderer.RenderNodes(
            drawList, 
            _panelViewModel.CfgViewModel.Nodes, 
            canvasPos, 
            transform, 
            _panelViewModel.SelectedNode, 
            _panelViewModel.HoveredNode, 
            _panelViewModel.Zoom
        );
        
        // Render status bar
        _statusBar.Render(canvasPos, canvasSize, drawList);
        
        // End the panel
        ImGui.End();
    }
}
