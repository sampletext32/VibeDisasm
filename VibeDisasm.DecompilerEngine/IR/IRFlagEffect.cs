using VibeDisasm.DecompilerEngine.IR.Model;

namespace VibeDisasm.DecompilerEngine.IR;

/// <summary>
/// Represents a processor flag modified by an IR instruction.
/// </summary>
public sealed class IRFlagEffect : IRNode
{
    public IRFlag Flag { get; init; }

    public IRFlagEffect(IRFlag flag)
    {
        Flag = flag;
    }
}