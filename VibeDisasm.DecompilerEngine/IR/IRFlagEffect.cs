using VibeDisasm.DecompilerEngine.IR.Instructions;

namespace VibeDisasm.DecompilerEngine.IR;

/// <summary>
/// Represents a processor flag modified by an IR instruction.
/// </summary>
public sealed record IRFlagEffect(IRFlag Flag);
