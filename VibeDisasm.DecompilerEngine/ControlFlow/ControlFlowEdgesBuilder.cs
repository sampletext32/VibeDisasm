using System.Diagnostics;
using VibeDisasm.Disassembler.X86;

namespace VibeDisasm.DecompilerEngine.ControlFlow;

public static class ControlFlowEdgesBuilder
{
    public static ILookup<uint, ControlFlowEdge> Build(ControlFlowFunction function)
    {
        // build predecessor and successor relation

        Queue<ControlFlowBlock> blockQueue = new Queue<ControlFlowBlock>();
        HashSet<uint> visitedBlockAddresses = [];

        List<ControlFlowEdge> edges = [];

        blockQueue.Enqueue(
            function.Blocks.Values.FirstOrDefault(x => x.IsEntryBlock)
            ?? throw new InvalidOperationException("ControlFlowEdgesBuilder didn't expect a function without entry block")
        );

        while (blockQueue.Count > 0)
        {
            var block = blockQueue.Dequeue();

            if (!visitedBlockAddresses.Add(block.StartAddress))
            {
                Debug.WriteLine($"ControlFlowEdgesBuilder already visited block {block.StartAddress:X8}");
                continue;
            }

            if (block.Instructions.Count == 0)
            {
                throw new InvalidOperationException($"ControlFlowEdgesBuilder didn't expect empty block at address {block.StartAddress:X8}");
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
                
                blockQueue.Enqueue(function.Blocks[takenEdge.ToBlockAddress]);
                blockQueue.Enqueue(function.Blocks[fallthroughEdge.ToBlockAddress]);
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

                blockQueue.Enqueue(function.Blocks[takenEdge.ToBlockAddress]);
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

                blockQueue.Enqueue(function.Blocks[takenEdge.ToBlockAddress]);
            }
        }

        if (visitedBlockAddresses.Count != function.Blocks.Count)
        {
            Debug.WriteLine($"ControlFlowEdgesBuilder didn't visit all blocks of function. Only {visitedBlockAddresses.Count} of {function.Blocks.Count}.");
        }

        var lookup = edges.ToLookup(x => x.FromBlockAddress);

        return lookup;
    }
}