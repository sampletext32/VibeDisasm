using System.Numerics;
using ImGuiNET;
using VibeDisasm.CfgVisualizer.Abstractions;
using VibeDisasm.CfgVisualizer.ViewModels;

namespace VibeDisasm.CfgVisualizer.ImGuiUI.Panels;

/// <summary>
/// Panel for displaying and selecting code sections
/// </summary>
public class CodeSectionsPanel : IImGuiPanel
{
    // View model
    private readonly CodeSectionsPanelViewModel _panelViewModel;
    
    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="panelViewModel">Code sections view model</param>
    public CodeSectionsPanel(CodeSectionsPanelViewModel panelViewModel)
    {
        _panelViewModel = panelViewModel;
    }
    
    /// <summary>
    /// Renders the code sections panel
    /// </summary>
    public void OnImGuiRender()
    {
        // Begin the panel
        bool isOpen = ImGui.Begin("Code Sections");
        if (isOpen)
        {
            if (_panelViewModel.Sections.Count == 0)
            {
                ImGui.TextColored(new Vector4(0.7f, 0.7f, 0.7f, 1.0f), "No sections available");
            }
            else
            {
                // Calculate a good height for the list box
                float listBoxHeight = ImGui.GetContentRegionAvail().Y - ImGui.GetFrameHeightWithSpacing();
                
                if (ImGui.BeginListBox("##CodeSections", new Vector2(-1, listBoxHeight)))
                {
                    for (int i = 0; i < _panelViewModel.Sections.Count; i++)
                    {
                        var section = _panelViewModel.Sections[i];
                        string label = section.ComputedView;
                        
                        // Color-code sections based on their characteristics
                        Vector4 textColor = GetSectionColor(section);
                        
                        bool isSelected = i == _panelViewModel.SelectedSectionIndex;
                        ImGui.PushStyleColor(ImGuiCol.Text, textColor);
                        
                        if (ImGui.Selectable(label, isSelected))
                        {
                            _panelViewModel.SelectSection(i);
                        }
                        
                        ImGui.PopStyleColor();
                        
                        if (isSelected)
                        {
                            ImGui.SetItemDefaultFocus();
                        }
                        
                        // Show tooltip with additional information when hovering
                        if (ImGui.IsItemHovered())
                        {
                            ImGui.BeginTooltip();
                            ImGui.Text($"Name: {section.Name}");
                            ImGui.Text($"Virtual Address: 0x{section.VirtualAddress:X8}");
                            ImGui.Text($"Virtual Size: 0x{section.VirtualSize:X} bytes");
                            ImGui.Text($"Raw Data Address: 0x{section.RawDataAddress:X8}");
                            ImGui.Text($"Raw Data Size: 0x{section.RawDataSize:X} bytes");
                            ImGui.Text("Characteristics:");
                            
                            // Display section characteristics
                            if (section.IsExecutable) ImGui.BulletText("Executable");
                            if (section.IsReadable) ImGui.BulletText("Readable");
                            if (section.IsWritable) ImGui.BulletText("Writable");
                            if (section.ContainsCode) ImGui.BulletText("Contains Code");
                            if (section.ContainsInitializedData) ImGui.BulletText("Contains Initialized Data");
                            if (section.ContainsUninitializedData) ImGui.BulletText("Contains Uninitialized Data");
                            
                            ImGui.EndTooltip();
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
    /// Gets a color for a section based on its characteristics
    /// </summary>
    /// <param name="section">The section to get a color for</param>
    /// <returns>A color vector</returns>
    private Vector4 GetSectionColor(SectionDisplayInfo section)
    {
        // Color scheme for different section types
        if (section.ContainsCode && section.IsExecutable)
        {
            // Executable code sections (like .text) - bright green
            return new Vector4(0.0f, 0.9f, 0.0f, 1.0f);
        }
        else if (section.ContainsCode)
        {
            // Code sections that aren't executable - yellow-green
            return new Vector4(0.7f, 0.9f, 0.0f, 1.0f);
        }
        else if (section.ContainsInitializedData && section.IsReadable && !section.IsWritable)
        {
            // Read-only data sections (like .rdata) - light blue
            return new Vector4(0.4f, 0.7f, 1.0f, 1.0f);
        }
        else if (section.ContainsInitializedData)
        {
            // Other data sections (like .data) - blue
            return new Vector4(0.2f, 0.4f, 0.8f, 1.0f);
        }
        else if (section.ContainsUninitializedData)
        {
            // BSS sections - purple
            return new Vector4(0.6f, 0.2f, 0.8f, 1.0f);
        }
        else if (section.Name.StartsWith(".rsrc"))
        {
            // Resource sections - orange
            return new Vector4(1.0f, 0.6f, 0.0f, 1.0f);
        }
        else if (section.Name.StartsWith(".reloc"))
        {
            // Relocation sections - pink
            return new Vector4(1.0f, 0.4f, 0.7f, 1.0f);
        }
        
        // Default color for other sections - white
        return new Vector4(1.0f, 1.0f, 1.0f, 1.0f);
    }
}
