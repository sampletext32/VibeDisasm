using Silk.NET.Input;

namespace VibeDisasm.CfgVisualizer.Abstractions;

/// <summary>
/// Interface for components that need to receive keyboard events
/// </summary>
public interface IKeyPressReceiver
{
    /// <summary>
    /// Called when a key is pressed
    /// </summary>
    void OnKeyPressed(Key key);

    /// <summary>
    /// Called when a key is held down
    /// </summary>
    void OnKeyDown(Key key);

    /// <summary>
    /// Called when a key is released
    /// </summary>
    void OnKeyReleased(Key key);
}
