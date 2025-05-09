using System.Numerics;
using ImGuiNET;
using VibeDisasm.CfgVisualizer.Abstractions;
using VibeDisasm.CfgVisualizer.ViewModels;

namespace VibeDisasm.CfgVisualizer.ImGuiUI.Panels;

/// <summary>
/// Panel for loading PE files
/// </summary>
public class FileLoadingPanel : IImGuiPanel
{
    // View model
    private readonly FileLoadingPanelViewModel _panelViewModel;
    
    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="panelViewModel">File loading view model</param>
    public FileLoadingPanel(FileLoadingPanelViewModel panelViewModel)
    {
        _panelViewModel = panelViewModel;
    }
    
    /// <summary>
    /// Renders the file loading panel
    /// </summary>
    public void OnImGuiRender()
    {
        // Begin the panel
        bool isOpen = ImGui.Begin("File");
        if (isOpen)
        {
            // Display loaded file name
            ImGui.Text("Loaded: " + (_panelViewModel.LoadedFilePath.Length > 0 ? 
                Path.GetFileName(_panelViewModel.LoadedFilePath) : "None"));
            
            ImGui.SameLine();
            
            // Load file button
            if (ImGui.Button("Load PE File"))
            {
                // Open file dialog using NativeFileDialogSharp
                var result = NativeFileDialogSharp.Dialog.FileOpen();
                
                if (result.IsOk)
                {
                    _panelViewModel.TryLoadFile(result.Path);
                }
            }
            
            // End the panel only if Begin returned true
            ImGui.End();
        }
    }
}
