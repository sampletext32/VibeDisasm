using System.Numerics;
using ImGuiNET;
using VibeDisasm.CfgVisualizer.Abstractions;
using VibeDisasm.CfgVisualizer.ViewModels;

namespace VibeDisasm.CfgVisualizer.ImGuiUI.Panels;

/// <summary>
/// Panel for displaying and selecting entry points
/// </summary>
public class EntryPointsPanel : IImGuiPanel
{
    // View model
    private readonly EntryPointsPanelViewModel _panelViewModel;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="panelViewModel">Entry points view model</param>
    public EntryPointsPanel(EntryPointsPanelViewModel panelViewModel)
    {
        _panelViewModel = panelViewModel;
    }

    /// <summary>
    /// Renders the entry points panel
    /// </summary>
    public void OnImGuiRender()
    {
        // Begin the panel
        var isOpen = ImGui.Begin("Entry Points");
        if (isOpen)
        {
            if (_panelViewModel.EntryPoints.Count == 0)
            {
                ImGui.TextColored(new Vector4(0.7f, 0.7f, 0.7f, 1.0f), "No entry points available");
            }
            else
            {
                // Calculate a good height for the list box
                var listBoxHeight = ImGui.GetContentRegionAvail().Y - ImGui.GetFrameHeightWithSpacing();

                if (ImGui.BeginListBox("##EntryPoints", new Vector2(-1, listBoxHeight)))
                {
                    for (var i = 0; i < _panelViewModel.EntryPoints.Count; i++)
                    {
                        var entryPoint = _panelViewModel.EntryPoints[i];
                        var label = entryPoint.ComputedView;

                        var isSelected = i == _panelViewModel.SelectedEntryPointIndex;
                        if (ImGui.Selectable(label, isSelected))
                        {
                            _panelViewModel.SelectEntryPoint(i);
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
