using System.Numerics;
using ImGuiNET;
using VibeDisasm.CfgVisualizer.Abstractions;
using VibeDisasm.CfgVisualizer.Models;

namespace VibeDisasm.CfgVisualizer.ImGuiUI.Panels;

/// <summary>
/// Panel for displaying and selecting entry points
/// </summary>
public class EntryPointsPanel : IImGuiPanel
{
    // View model
    private readonly EntryPointsViewModel _viewModel;
    
    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="viewModel">Entry points view model</param>
    public EntryPointsPanel(EntryPointsViewModel viewModel)
    {
        _viewModel = viewModel;
    }
    
    /// <summary>
    /// Renders the entry points panel
    /// </summary>
    public void OnImGuiRender()
    {
        // Begin the panel
        bool isOpen = ImGui.Begin("Entry Points");
        if (isOpen)
        {
            if (_viewModel.EntryPoints.Count == 0)
            {
                ImGui.TextColored(new Vector4(0.7f, 0.7f, 0.7f, 1.0f), "No entry points available");
            }
            else
            {
                // Calculate a good height for the list box
                float listBoxHeight = ImGui.GetContentRegionAvail().Y - ImGui.GetFrameHeightWithSpacing();
                
                if (ImGui.BeginListBox("##EntryPoints", new Vector2(-1, listBoxHeight)))
                {
                    for (int i = 0; i < _viewModel.EntryPoints.Count; i++)
                    {
                        var entryPoint = _viewModel.EntryPoints[i];
                        string label = $"{entryPoint.RVA:X8} - {entryPoint.Source}: {entryPoint.Description}";
                        
                        bool isSelected = i == _viewModel.SelectedEntryPointIndex;
                        if (ImGui.Selectable(label, isSelected))
                        {
                            _viewModel.SelectEntryPoint(i);
                        }
                        
                        if (isSelected)
                        {
                            ImGui.SetItemDefaultFocus();
                        }
                    }
                    
                    ImGui.EndListBox();
                }
            }
            
            // End the panel only if Begin returned true
            ImGui.End();
        }
    }
}
