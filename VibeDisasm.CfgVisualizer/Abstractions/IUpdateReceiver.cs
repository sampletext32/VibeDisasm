namespace VibeDisasm.CfgVisualizer.Abstractions;

/// <summary>
/// Interface for components that need to receive update events
/// </summary>
public interface IUpdateReceiver
{
    /// <summary>
    /// Called on each frame update
    /// </summary>
    /// <param name="deltaTime">Time since last frame in seconds</param>
    void Update(double deltaTime);
}
