using System.Numerics;
using ImGuiNET;
using VibeDisasm.CfgVisualizer.Abstractions;
using VibeDisasm.CfgVisualizer.Models;

namespace VibeDisasm.CfgVisualizer.ImGuiUI.Panels;

/// <summary>
/// Panel for loading PE files
/// </summary>
public class FileLoadingPanel : IImGuiPanel
{
    // View model
    private readonly FileLoadingViewModel _viewModel;
    
    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="viewModel">File loading view model</param>
    public FileLoadingPanel(FileLoadingViewModel viewModel)
    {
        _viewModel = viewModel;
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
            ImGui.Text("Loaded: " + (_viewModel.LoadedFilePath.Length > 0 ? 
                Path.GetFileName(_viewModel.LoadedFilePath) : "None"));
            
            ImGui.SameLine();
            
            // Load file button
            if (ImGui.Button("Load PE File"))
            {
                // Open file dialog using NativeFileDialogSharp
                var result = NativeFileDialogSharp.Dialog.FileOpen();
                
                if (result.IsOk)
                {
                    _viewModel.TryLoadFile(result.Path);
                }
            }
            
            // End the panel only if Begin returned true
            ImGui.End();
        }
    }
}
