namespace VibeDisasm.DecompilerEngine.ControlFlow;

public static class ControlFlowIrRewriter
{
    public static void Rewrite(ControlFlowFunction function, ILookup<uint, ControlFlowEdge> edges)
    {
        var entryBlock = function.Blocks.Values.FirstOrDefault(x => x.IsEntryBlock)
                         ?? throw new InvalidOperationException("ControlFlowRestructurer didn't expect a function without entry block");

        // every function is basically a sequence of nodes
        var ir = new SequenceNode();
        
        // add raw content, which is not a control flow instruction
        foreach (var instruction in entryBlock.Instructions.Take(entryBlock.Instructions.Count - 1))
        {
            ir.Content.Add(new NativeNode(instruction));
        }

        var currentEdges = edges.Contains(entryBlock.StartAddress)
            ? edges[entryBlock.StartAddress]
            : [];

        if (currentEdges.Any())
        {
            // no edges, the function ends here.
            
            return;
        }

        var lastInstruction = entryBlock.LastControlFlowInstruction
            ?? throw new InvalidOperationException($"ControlFlowRestructurer didn't expect LastControlFlowInstruction of block {entryBlock.StartAddress:X8} to be null");

        new ConditionNode()
        {
            Condition = new NativeNode(lastInstruction),
        };
    }
}

public abstract class IrNode
{
}

public class NativeNode : IrNode
{
    public ControlFlowInstruction Instruction { get; set; }

    public NativeNode(ControlFlowInstruction instruction)
    {
        Instruction = instruction;
    }
}

public class SequenceNode : IrNode
{
    public List<IrNode> Content { get; set; } = [];
}

public class ConditionNode : IrNode
{
    public IrNode Condition { get; set; }

    public IrNode Content { get; set; }
}