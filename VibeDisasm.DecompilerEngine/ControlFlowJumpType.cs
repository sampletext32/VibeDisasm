namespace VibeDisasm.DecompilerEngine;

public enum ControlFlowJumpType
{
    /// <summary>
    /// The jump is taken
    /// </summary>
    Taken,
    /// <summary>
    /// There is no jump, block switching is direct (one after another)
    /// </summary>
    Fallthrough
}