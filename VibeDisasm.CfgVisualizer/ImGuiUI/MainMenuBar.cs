using ImGuiNET;
using VibeDisasm.CfgVisualizer.Abstractions;

namespace VibeDisasm.CfgVisualizer.ImGuiUI;

/// <summary>
/// Main menu bar for the application
/// </summary>
public class MainMenuBar : IImGuiPanel
{
    /// <summary>
    /// Renders the main menu bar
    /// </summary>
    public void OnImGuiRender()
    {
        if (ImGui.BeginMainMenuBar())
        {
            if (ImGui.BeginMenu("File"))
            {
                if (ImGui.MenuItem("Open PE File", "Ctrl+O"))
                {
                    // TODO: Implement file open dialog
                }
                
                ImGui.Separator();
                
                if (ImGui.MenuItem("Exit", "Alt+F4"))
                {
                    App.Instance.Window.Close();
                }
                
                ImGui.EndMenu();
            }
            
            if (ImGui.BeginMenu("View"))
            {
                if (ImGui.MenuItem("Reset Layout"))
                {
                    // TODO: Implement layout reset
                }
                
                ImGui.EndMenu();
            }
            
            if (ImGui.BeginMenu("Help"))
            {
                if (ImGui.MenuItem("About"))
                {
                    // TODO: Implement about dialog
                }
                
                ImGui.EndMenu();
            }
            
            ImGui.EndMainMenuBar();
        }
    }
}
