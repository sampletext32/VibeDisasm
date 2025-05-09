using System.Numerics;
using ImGuiNET;
using VibeDisasm.CfgVisualizer.Abstractions;
using VibeDisasm.CfgVisualizer.Models;

namespace VibeDisasm.CfgVisualizer.ImGuiUI.Panels;

/// <summary>
/// Panel for displaying details of a selected node
/// </summary>
public class NodeDetailsPanel : IImGuiPanel
{
    // View model
    private readonly NodeDetailsViewModel _viewModel;
    
    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="viewModel">Node details view model</param>
    public NodeDetailsPanel(NodeDetailsViewModel viewModel)
    {
        _viewModel = viewModel;
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
            if (_viewModel.SelectedNode == null)
            {
                ImGui.TextColored(new Vector4(0.7f, 0.7f, 0.7f, 1.0f), "Select a node to view details");
            }
            else
            {
                // Display basic node information
                ImGui.Text($"Address: 0x{_viewModel.SelectedNode.Block.StartAddress:X8}");
                ImGui.Text($"Instructions: {_viewModel.SelectedNode.Block.Instructions.Count}");
                
                ImGui.Separator();
                
                // Display instructions in a table
                if (ImGui.BeginTable("Instructions", 2, ImGuiTableFlags.Borders | ImGuiTableFlags.RowBg))
                {
                    ImGui.TableSetupColumn("Address");
                    ImGui.TableSetupColumn("Instruction");
                    ImGui.TableHeadersRow();
                    
                    foreach (var instruction in _viewModel.SelectedNode.Block.Instructions)
                    {
                        ImGui.TableNextRow();
                        
                        ImGui.TableNextColumn();
                        ImGui.Text($"0x{instruction.Address:X8}");
                        
                        ImGui.TableNextColumn();
                        ImGui.Text(instruction.ToString());
                    }
                    
                    ImGui.EndTable();
                }
            }
            
            // End the panel only if Begin returned true
            ImGui.End();
        }
    }
}
