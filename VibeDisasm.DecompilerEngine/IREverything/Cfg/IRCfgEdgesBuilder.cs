using VibeDisasm.DecompilerEngine.IREverything.Abstractions;
using VibeDisasm.DecompilerEngine.IREverything.Expressions;
using VibeDisasm.DecompilerEngine.IREverything.Instructions;
using VibeDisasm.DecompilerEngine.IREverything.Structuring;

namespace VibeDisasm.DecompilerEngine.IREverything.Cfg;

public class IRCfgEdgesBuilder
{
    public static IRCfgEdgesLookup BuildEdges(IRSequenceNode functionBody)
    {
        var blocks = functionBody.EnumerateBlocks()
            .OrderBy(x => x.Address)
            .ToList();

        List<IRCfgEdge> edges = [];

        foreach (var irBlock in blocks)
        {
            var lastInstruction = irBlock.Instructions.LastOrDefault();

            if (lastInstruction is null)
                continue;

            if (lastInstruction is IIRConditionalJump conditionalJump)
            {
                if (conditionalJump.Target is not IRConstantExpr constant)
                {
                    throw new InvalidOperationException("conditionalJump.Target must be a constant expression.");
                }

                var nextBlock = blocks.FirstOrDefault(x => x.Address > irBlock.Address);

                if (nextBlock is null)
                {
                    throw new InvalidOperationException("No next block found after the current block.");
                }

                edges.Add(new(irBlock.Address, (uint)constant.Value, IREdge.Taken));
                edges.Add(new(irBlock.Address, nextBlock.Address, IREdge.Fallthrough));
                continue;
            }
            else if (lastInstruction is IRJumpInstruction regularJump)
            {
                if (regularJump.Target is not IRConstantExpr constant)
                {
                    throw new InvalidOperationException("regularJump.Target must be a constant expression.");
                }
                edges.Add(new(irBlock.Address, (uint)constant.Value, IREdge.Taken));
                continue;
            }
            else
            {
                // just a fallthrough

                var nextBlock = blocks.FirstOrDefault(x => x.Address > irBlock.Address);

                if (nextBlock is null)
                {
                    Console.WriteLine("No next block found after the current block, skipping fallthrough edge.");
                    continue;
                }

                edges.Add(new(irBlock.Address, nextBlock.Address, IREdge.Fallthrough));
            }
        }

        var edgesByFrom = edges.ToLookup(x => x.From);
        var edgesByTo = edges.ToLookup(x => x.To);

        return new IRCfgEdgesLookup(edges, edgesByFrom, edgesByTo);
    }
}
