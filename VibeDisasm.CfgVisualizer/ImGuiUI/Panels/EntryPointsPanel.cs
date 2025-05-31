using System.Numerics;
using ImGuiNET;
using System.Globalization;
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
    
    // Custom address input buffer
    private string _customAddressInput = string.Empty;

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
            // Custom address input section
            RenderCustomAddressInput();
            
            ImGui.Separator();
            
            if (_panelViewModel.EntryPoints.Count == 0)
            {
                ImGui.TextColored(new Vector4(0.7f, 0.7f, 0.7f, 1.0f), "No entry points available");
            }
            else
            {
                // Calculate a good height for the list box - account for the custom address input section
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
    
    /// <summary>
    /// Renders the custom address input section
    /// </summary>
    private void RenderCustomAddressInput()
    {
        ImGui.TextUnformatted("Add Custom Entry Point:");
        
        // Input field for custom address (hex)
        ImGui.SetNextItemWidth(ImGui.GetContentRegionAvail().X * 0.7f);
        if (ImGui.InputText("##CustomAddress", ref _customAddressInput, 16, ImGuiInputTextFlags.CharsHexadecimal))
        {
            // Limit to valid hex characters
            _customAddressInput = new string(_customAddressInput.Where(c => 
                (c >= '0' && c <= '9') || 
                (c >= 'a' && c <= 'f') || 
                (c >= 'A' && c <= 'F')).ToArray());
        }
        
        ImGui.SameLine();
        
        // Add button
        if (ImGui.Button("Add") && !string.IsNullOrWhiteSpace(_customAddressInput))
        {
            // Try to parse the hex string to a uint (RVA)
            if (uint.TryParse(_customAddressInput, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out var rva))
            {
                _panelViewModel.AddCustomEntryPoint(rva);
                _customAddressInput = string.Empty; // Clear the input after adding
            }
        }
        
        // Help tooltip
        if (ImGui.IsItemHovered())
        {
            ImGui.BeginTooltip();
            ImGui.TextUnformatted("Enter a hexadecimal RVA (Relative Virtual Address) to add as a custom entry point");
            ImGui.EndTooltip();
        }
    }
}
