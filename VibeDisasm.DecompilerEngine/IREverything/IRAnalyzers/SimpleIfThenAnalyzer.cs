using VibeDisasm.DecompilerEngine.IREverything.Abstractions;
using VibeDisasm.DecompilerEngine.IREverything.Expressions;
using VibeDisasm.DecompilerEngine.IREverything.Model;
using VibeDisasm.DecompilerEngine.IREverything.Structuring;

namespace VibeDisasm.DecompilerEngine.IREverything.IRAnalyzers;

public class SimpleIfThenAnalyzer
{
    public void Handle(IRFunction function)
    {
        var body = function.Body;

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

            var targetAddr = targetExpr.Value is uint
                ? (uint) targetExpr.Value
                : 0;

            if (targetAddr > currentBlock.Address)
            {
                // blocks are already ordered by address
                var nextBlockIndex = bodyBlocks.FindIndex(x => x.Address > currentBlock.Address);

                if (nextBlockIndex is -1)
                {
                    throw new InvalidOperationException("Didn't expect to not find next block after overjump.");
                }
                var nextBlock = bodyBlocks[nextBlockIndex];

                if (nextBlock.Address < targetAddr)
                {
                    // this is overjump (skipping part of logic) - basically a simple if-then structure
                    // | currentBlock
                    // |         \
                    // | ---------\
                    // |nextBlock  |
                    // | ---------/
                    // |        /
                    // | overjump (jumpTarget)

                    // in this case jump is actually inverted
                    // if (!condition) overjump;

                    var ifThen = new IRIfThenNode(
                        jump.Condition.Invert(),
                        nextBlock
                    );

                    body.Insert(i + 1, ifThen);
                    if (i < nextBlockIndex)
                    {
                        body.Nodes.RemoveAt(nextBlockIndex + 1);
                    }
                    else
                    {
                        body.Nodes.RemoveAt(nextBlockIndex);
                    }
                }
            }
        }
    }
}
