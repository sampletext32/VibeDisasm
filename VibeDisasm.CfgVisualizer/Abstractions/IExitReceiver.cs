namespace VibeDisasm.CfgVisualizer.Abstractions;

/// <summary>
/// Interface for components that need to perform cleanup on application exit
/// </summary>
public interface IExitReceiver
{
    /// <summary>
    /// Called when the application is exiting
    /// </summary>
    void Exit();
}
