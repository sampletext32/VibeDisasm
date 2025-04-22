using System.Diagnostics;
using VibeDisasm.Disassembler.X86;

namespace VibeDisasm.DecompilerEngine;

public class ControlFlowGraph
{
    private readonly ControlFlowFunction _function;

    public ControlFlowGraph(ControlFlowFunction function)
    {
        _function = function;
    }

    public List<ControlFlowEdge> Build()
    {
        // build predecessor and successor relation

        Queue<ControlFlowBlock> blockQueue = new Queue<ControlFlowBlock>();
        HashSet<uint> visitedBlockAddresses = [];

        List<ControlFlowEdge> edges = [];

        blockQueue.Enqueue(
            _function.Blocks.Values.FirstOrDefault(x => x.IsEntryBlock)
            ?? throw new InvalidOperationException("ControlFlowGraph didn't expect a function without entry block")
        );

        while (blockQueue.Count > 0)
        {
            var block = blockQueue.Dequeue();

            if (!visitedBlockAddresses.Add(block.StartAddress))
            {
                Debug.WriteLine($"ControlFlowGraph already visited block {block.StartAddress:X8}");
                continue;
            }

            if (block.Instructions.Count == 0)
            {
                throw new InvalidOperationException($"ControlFlowGraph didn't expect empty block at address {block.StartAddress:X8}");
            }

            var lastInstruction = block.LastControlFlowInstruction!;

            if (lastInstruction.IsConditionalJump())
            {
                var takenEdge = new ControlFlowEdge()
                {
                    FromBlockAddress = block.StartAddress,
                    ToBlockAddress = lastInstruction.GetJumpTargetAddress()!.Value,
                    JumpType = ControlFlowJumpType.Taken
                };
                edges.Add(
                    takenEdge
                );

                var fallthroughEdge = new ControlFlowEdge()
                {
                    FromBlockAddress = block.StartAddress,
                    ToBlockAddress = lastInstruction.GetNextSequentialAddress(),
                    JumpType = ControlFlowJumpType.Fallthrough
                };

                edges.Add(
                    fallthroughEdge
                );
                
                blockQueue.Enqueue(_function.Blocks[takenEdge.ToBlockAddress]);
                blockQueue.Enqueue(_function.Blocks[fallthroughEdge.ToBlockAddress]);
            }
            else if (lastInstruction.IsUnconditionalJump())
            {
                var takenEdge = new ControlFlowEdge()
                {
                    FromBlockAddress = block.StartAddress,
                    ToBlockAddress = lastInstruction.GetJumpTargetAddress()!.Value,
                    JumpType = ControlFlowJumpType.Taken
                };
                edges.Add(
                    takenEdge
                );

                blockQueue.Enqueue(_function.Blocks[takenEdge.ToBlockAddress]);
            }
            else if(lastInstruction.RawInstruction.Type == InstructionType.Ret)
            {
                // do nothing, RET has no descendants
            }
            else
            {
                // this is a direct fallthrough block

                var takenEdge = new ControlFlowEdge()
                {
                    FromBlockAddress = block.StartAddress,
                    ToBlockAddress = lastInstruction.GetNextSequentialAddress(),
                    JumpType = ControlFlowJumpType.Fallthrough
                };
                edges.Add(
                    takenEdge
                );

                blockQueue.Enqueue(_function.Blocks[takenEdge.ToBlockAddress]);
            }
        }

        if (visitedBlockAddresses.Count != _function.Blocks.Count)
        {
            Debug.WriteLine($"ControlFlowGraph didn't visit all blocks of function. Only {visitedBlockAddresses.Count} of {_function.Blocks.Count}.");
        }

        _ = 5;

        return edges;
    }
}