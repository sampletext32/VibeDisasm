using System.Diagnostics.Contracts;
using VibeDisasm.DecompilerEngine.IREverything.Abstractions;
using VibeDisasm.DecompilerEngine.IREverything.Cfg;
using VibeDisasm.DecompilerEngine.IREverything.Expressions;
using VibeDisasm.DecompilerEngine.IREverything.Model;
using VibeDisasm.DecompilerEngine.IREverything.Structuring;

namespace VibeDisasm.DecompilerEngine.IREverything.IRAnalyzers;

/// <summary>
/// Analyzer that identifies simple if-then structures in the IR.
/// </summary>
public class SimpleIfThenAnalyzer
{
    [Pure]
    private static bool IsOverjump(uint fallthroughAddr, uint targetAddr) =>
        targetAddr > fallthroughAddr;

    public void Handle(IRFunction function)
    {
        var body = function.Body;
        var cfgEdges = function.CfgEdges;
        
        var bodyBlocks = body.EnumerateBlocks()
            .OrderBy(x => x.Address)
            .ToList();
            
        for (var i = 0; i < bodyBlocks.Count; i++)
        {
            var currentBlock = bodyBlocks[i];
            var lastInstruction = currentBlock.Instructions.LastOrDefault();

            if (lastInstruction is null)
            {
                Console.WriteLine($"SimpleIfThenAnalyzer stepped onto block {currentBlock.Address:X8} without last instruction.");
                continue;
            }

            if (lastInstruction is not IIRConditionalJump jump)
            {
                Console.WriteLine($"SimpleIfThenAnalyzer stepped onto block {currentBlock.Address:X8} with last instruction not a conditional jump, but {lastInstruction.GetType().Name}.");
                continue;
            }

            if (jump.Target is not IRConstantExpr targetExpr)
            {
                Console.WriteLine($"SimpleIfThenAnalyzer block {currentBlock.Address:X8} has conditional jump with not a constant target {jump.Target.GetType().Name}.");
                continue;
            }

            var outgoingEdges = cfgEdges.EdgesByFrom[currentBlock.Address].ToList();
            if (outgoingEdges.Count != 2)
                continue;

            var takenEdge = outgoingEdges.FirstOrDefault(e => e.Type == IREdge.Taken);
            var fallthroughEdge = outgoingEdges.FirstOrDefault(e => e.Type == IREdge.Fallthrough);
            
            if (takenEdge is null || fallthroughEdge is null)
                continue;

            var fallthroughBlockAddr = fallthroughEdge.To;
            var targetBlockAddr = takenEdge.To;
            
            var fallthroughBlock = bodyBlocks.FirstOrDefault(b => b.Address == fallthroughBlockAddr);
            if (fallthroughBlock is null)
            {
                Console.WriteLine($"SimpleIfThenAnalyzer couldn't find fallthrough block at address {fallthroughBlockAddr:X8}");
                continue;
            }
            
            // Check if this is a potential if-then structure (overjump pattern)
            if (IsOverjump(fallthroughBlockAddr, targetBlockAddr))
            {
                // For a proper if-then structure, the target block should have exactly two incoming edges:
                // 1. From the if head (current block)
                // 2. From the then block (fallthrough block)
                var targetIncomingEdges = cfgEdges.EdgesByTo[targetBlockAddr].ToList();
                
                // Check if we have exactly 2 incoming edges to the target block
                if (targetIncomingEdges.Count != 2)
                {
                    continue;
                }
                
                // Check if the incoming edges are from the current block and the fallthrough block
                var incomingFromCurrentBlock = targetIncomingEdges.Any(e => e.From == currentBlock.Address);
                var incomingFromFallthroughBlock = targetIncomingEdges.Any(e => e.From == fallthroughBlockAddr);
                
                if (!incomingFromCurrentBlock || !incomingFromFallthroughBlock)
                {
                    continue;
                }
                
                // This is an overjump (skipping part of logic) - a simple if-then structure
                // | currentBlock
                // |         \
                // | ---------\
                // |fallthroughBlock  |
                // | ---------/
                // |        /
                // | overjump (targetBlockAddr)

                // In this case jump is actually inverted
                // if (!condition) overjump;
                
                var ifThen = new IRIfThenNode(
                    jump.Condition.Invert(),
                    fallthroughBlock
                );

                var fallthroughBlockIndex = body.FindNodeIndex(node => 
                    node is IRBlock block && block.Address == fallthroughBlockAddr);
                
                if (fallthroughBlockIndex == -1)
                {
                    Console.WriteLine($"SimpleIfThenAnalyzer couldn't find fallthrough block in body nodes");
                    continue;
                }

                body.ReplaceAndInsertAfter(fallthroughBlockIndex, i, ifThen);
            }
        }
    }
}
