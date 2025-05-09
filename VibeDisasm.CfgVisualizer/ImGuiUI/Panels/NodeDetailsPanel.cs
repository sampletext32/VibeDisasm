using System.Numerics;
using ImGuiNET;
using VibeDisasm.CfgVisualizer.Abstractions;
using VibeDisasm.CfgVisualizer.ViewModels;

namespace VibeDisasm.CfgVisualizer.ImGuiUI.Panels;

/// <summary>
/// Panel for displaying details of a selected node
/// </summary>
public class NodeDetailsPanel : IImGuiPanel
{
    // View model
    private readonly NodeDetailsPanelViewModel _panelViewModel;
    
    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="panelViewModel">Node details view model</param>
    public NodeDetailsPanel(NodeDetailsPanelViewModel panelViewModel)
    {
        _panelViewModel = panelViewModel;
    }
    
    /// <summary>
    /// Renders the node details panel
    /// </summary>
    public void OnImGuiRender()
    {
        // Begin the panel
        bool isOpen = ImGui.Begin("Node Details");
        if (isOpen)
        {
            if (_panelViewModel.SelectedNode == null)
            {
                ImGui.TextColored(new Vector4(0.7f, 0.7f, 0.7f, 1.0f), "Select a node to view details");
            }
            else
            {
                // Display basic node information
                ImGui.Text($"Address: 0x{_panelViewModel.SelectedNode.Block.StartAddress:X8}");
                ImGui.Text($"Instructions: {_panelViewModel.SelectedNode.Block.Instructions.Count}");
                
                ImGui.Separator();
                
                // Display instructions in a table
                if (ImGui.BeginTable("Instructions", 2, ImGuiTableFlags.Borders | ImGuiTableFlags.RowBg))
                {
                    ImGui.TableSetupColumn("Address");
                    ImGui.TableSetupColumn("Instruction");
                    ImGui.TableHeadersRow();
                    
                    foreach (var instruction in _panelViewModel.SelectedNode.Block.Instructions)
                    {
                        ImGui.TableNextRow();
                        
                        ImGui.TableNextColumn();
                        ImGui.Text(instruction.ComputedAddressView);
                        
                        ImGui.TableNextColumn();
                        ImGui.Text(instruction.ComputedView);
                    }
                    
                    ImGui.EndTable();
                }
            }
            
            // End the panel only if Begin returned true
            ImGui.End();
        }
    }
}
