using System.Diagnostics.Contracts;
using VibeDisasm.DecompilerEngine.IREverything.Abstractions;
using VibeDisasm.DecompilerEngine.IREverything.Cfg;
using VibeDisasm.DecompilerEngine.IREverything.Expressions;
using VibeDisasm.DecompilerEngine.IREverything.Model;
using VibeDisasm.DecompilerEngine.IREverything.Structuring;

namespace VibeDisasm.DecompilerEngine.IREverything.IRAnalyzers;

public class IfThenElseAnalyzer
{
    [Pure]
    private static bool IsIfThenElsePattern(
        IRCfgEdgesLookup cfgEdges, 
        uint currentBlockAddr, 
        uint takenBlockAddr, 
        uint fallthroughBlockAddr, 
        uint continuationBlockAddr)
    {
        var continuationIncomingEdges = cfgEdges.EdgesByTo[continuationBlockAddr].ToList();
        if (continuationIncomingEdges.Count != 2)
            return false;
            
        var incomingFromTakenBlock = continuationIncomingEdges.Any(e => e.From == takenBlockAddr);
        var incomingFromFallthroughBlock = continuationIncomingEdges.Any(e => e.From == fallthroughBlockAddr);
        
        return incomingFromTakenBlock && incomingFromFallthroughBlock;
    }

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
                Console.WriteLine($"IfThenElseAnalyzer stepped onto block {currentBlock.Address:X8} without last instruction.");
                continue;
            }

            if (lastInstruction is not IIRConditionalJump jump)
            {
                Console.WriteLine($"IfThenElseAnalyzer stepped onto block {currentBlock.Address:X8} with last instruction not a conditional jump, but {lastInstruction.GetType().Name}.");
                continue;
            }

            if (jump.Target is not IRConstantExpr targetExpr)
            {
                Console.WriteLine($"IfThenElseAnalyzer block {currentBlock.Address:X8} has conditional jump with not a constant target {jump.Target.GetType().Name}.");
                continue;
            }

            var outgoingEdges = cfgEdges.EdgesByFrom[currentBlock.Address].ToList();
            if (outgoingEdges.Count != 2)
                continue;

            var takenEdge = outgoingEdges.FirstOrDefault(e => e.Type == IREdge.Taken);
            var fallthroughEdge = outgoingEdges.FirstOrDefault(e => e.Type == IREdge.Fallthrough);
            
            if (takenEdge is null || fallthroughEdge is null)
                continue;

            var takenBlockAddr = takenEdge.To;
            var fallthroughBlockAddr = fallthroughEdge.To;
            
            // Find the taken and fallthrough blocks
            var takenBlock = bodyBlocks.FirstOrDefault(b => b.Address == takenBlockAddr);
            var fallthroughBlock = bodyBlocks.FirstOrDefault(b => b.Address == fallthroughBlockAddr);
            
            if (takenBlock is null || fallthroughBlock is null)
            {
                Console.WriteLine($"IfThenElseAnalyzer couldn't find taken or fallthrough block");
                continue;
            }
            
            // Find potential continuation blocks after both branches
            var takenBlockOutgoingEdges = cfgEdges.EdgesByFrom[takenBlockAddr].ToList();
            var fallthroughBlockOutgoingEdges = cfgEdges.EdgesByFrom[fallthroughBlockAddr].ToList();
            
            if (takenBlockOutgoingEdges.Count != 1 || fallthroughBlockOutgoingEdges.Count != 1)
                continue;
                
            var takenBlockTarget = takenBlockOutgoingEdges[0].To;
            var fallthroughBlockTarget = fallthroughBlockOutgoingEdges[0].To;
            
            // If both branches target the same block, we have an if-then-else pattern
            if (takenBlockTarget == fallthroughBlockTarget)
            {
                var continuationBlockAddr = takenBlockTarget;
                
                if (IsIfThenElsePattern(cfgEdges, currentBlock.Address, takenBlockAddr, fallthroughBlockAddr, continuationBlockAddr))
                {
                    // This is an if-then-else structure with a common continuation point
                    // | currentBlock
                    // |     /     \
                    // |    /       \
                    // |   v         v
                    // |thenBlock  elseBlock
                    // |   |         |
                    // |   v         v
                    // | continuationBlock
                    
                    var ifThenElse = new IRIfThenElseNode(
                        jump.Condition,
                        fallthroughBlock,
                        takenBlock
                    );
                    
                    var thenBlockIndex = body.FindNodeIndex(node => 
                        node is IRBlock block && block.Address == fallthroughBlockAddr);
                    
                    var elseBlockIndex = body.FindNodeIndex(node => 
                        node is IRBlock block && block.Address == takenBlockAddr);
                    
                    if (thenBlockIndex == -1 || elseBlockIndex == -1)
                    {
                        Console.WriteLine($"IfThenElseAnalyzer couldn't find then or else block in body nodes");
                        continue;
                    }
                    
                    // Use the new helper method to insert the if-then-else node and remove the original blocks
                    body.InsertAndRemoveMultiple(i + 1, ifThenElse, thenBlockIndex, elseBlockIndex);
                }
            }
        }
    }
}
