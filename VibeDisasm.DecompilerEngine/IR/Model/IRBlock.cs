using VibeDisasm.DecompilerEngine.IR.Instructions;
using VibeDisasm.DecompilerEngine.IR.Visitors;

namespace VibeDisasm.DecompilerEngine.IR.Model;

/// <summary>
/// Represents a basic block in IR.
/// Example: Block with instructions for a loop body
/// </summary>
public class IRBlock : IRNode
{
    public required string Id { get; init; }
    public required List<IRInstruction> Instructions { get; init; }

    public IEnumerable<T> EnumerateInstructionOfType<T>()
        where T : IRInstruction
    {
        return Instructions.OfType<T>();
    }

    public override string ToString() => $"// Block {Id}\n" + string.Join("\n", Instructions);

    public override void Accept(IIRNodeVisitor visitor)
    {
        visitor.Visit(this);
    }

    public override T Accept<T>(IIRNodeReturningVisitor<T> visitor) => visitor.Visit(this);
}
