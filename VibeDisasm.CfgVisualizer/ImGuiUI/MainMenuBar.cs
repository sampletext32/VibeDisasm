using ImGuiNET;
using VibeDisasm.CfgVisualizer.Abstractions;

namespace VibeDisasm.CfgVisualizer.ImGuiUI;

/// <summary>
/// Main menu bar for the application
/// </summary>
public class MainMenuBar : IImGuiPanel
{
    // Reference to the CFG viewer panel
    private readonly CfgViewerPanel _cfgViewerPanel;
    
    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="cfgViewerPanel">CFG viewer panel</param>
    public MainMenuBar(CfgViewerPanel cfgViewerPanel)
    {
        _cfgViewerPanel = cfgViewerPanel;
    }
    
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
                    // Open file dialog using NativeFileDialogSharp
                    var result = NativeFileDialogSharp.Dialog.FileOpen();
                    
                    if (result.IsOk)
                    {
                        _cfgViewerPanel.TryLoadFile(result.Path);
                    }
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
