using VibeDisasm.DecompilerEngine.IR.Instructions;

namespace VibeDisasm.DecompilerEngine.IR.Model;

/// <summary>
/// Represents a basic block in IR.
/// Example: Block with instructions for a loop body
/// </summary>
public class IRBlock
{
    public required string Id { get; init; }
    public required IReadOnlyList<IRInstruction> Instructions { get; init; }
}
