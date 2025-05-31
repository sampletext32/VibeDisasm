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
        if (ImGui.Begin("File"))
        {
            // Display loaded file name
            ImGui.Text(
                "Loaded: " + (!string.IsNullOrEmpty(_panelViewModel.LoadedFilePath)
                    ? Path.GetFileName(_panelViewModel.LoadedFilePath)
                    : "None")
            );

            ImGui.SameLine();

            // Load file button
            if (ImGui.Button("Load PE File"))
            {
                // Open file dialog using NativeFileDialogSharp
                var result = NativeFileDialogSharp.Dialog.FileOpen();

                if (result.IsOk)
                {
                    _panelViewModel.LaunchLoad(result.Path);
                }
            }

            ImGui.End();
        }
    }
}
