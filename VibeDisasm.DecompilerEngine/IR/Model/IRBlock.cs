using VibeDisasm.DecompilerEngine.IR.Instructions;

namespace VibeDisasm.DecompilerEngine.IR.Model;

/// <summary>
/// Represents a basic block in IR.
/// Example: Block with instructions for a loop body
/// </summary>
public class IRBlock
{
    public required string Id { get; init; }
    public required List<IRInstruction> Instructions { get; init; }

    public IEnumerable<T> EnumerateInstructionOfType<T>()
        where T : IRInstruction
    {
        return Instructions.OfType<T>();
    }

    public override string ToString() => $"// Block {Id}\n" + string.Join("\n", Instructions);
}
