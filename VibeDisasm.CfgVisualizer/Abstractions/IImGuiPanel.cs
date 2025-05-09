namespace VibeDisasm.CfgVisualizer.Abstractions;

/// <summary>
/// Interface for ImGui panels that can be rendered
/// </summary>
public interface IImGuiPanel
{
    /// <summary>
    /// Renders the ImGui panel
    /// </summary>
    void OnImGuiRender();
}
