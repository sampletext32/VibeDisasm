using VibeDisasm.DecompilerEngine.IREverything.IRAnalyzers.IRLiftedInstructions;
using VibeDisasm.DecompilerEngine.IREverything.Model;
using VibeDisasm.DecompilerEngine.IREverything.Visitors;

namespace VibeDisasm.DecompilerEngine.IREverything.IRAnalyzers;

/// <summary>
/// Analyzer that replaces flag-based conditions with semantic logical expressions.
/// This analyzer expects that a previous analyzer has already created IRWiredJumpInstruction objects.
/// </summary>
public class IRFlagConditionReplacementAnalyzer
{
    /// <summary>
    /// Process a function by replacing all flag conditions with high-level expressions
    /// </summary>
    /// <param name="function">The IR function to process</param>
    /// <returns>The transformed function with semantic conditions</returns>
    public void Handle(IRFunction function)
    {
        // For each block in the function
        foreach (var block in function.Body.EnumerateBlocks())
        {
            // For each instruction in the block
            for (var i = 0; i < block.Instructions.Count; i++)
            {
                var instruction = block.Instructions[i];

                // find only wired jumps, there is no need to transform other instructions
                if (instruction is IRWiredJumpInstruction wiredJump)
                {
                    var transformedCondition = IRFlagConditionTransformer.TransformCondition(
                        wiredJump.Condition,
                        wiredJump.ConditionInstruction
                    );

                    if (transformedCondition != null)
                    {
                        var newJump = new IRSemanticIfJumpInstruction(wiredJump, transformedCondition);

                        // Replace the wired jump with the new high-level jump
                        block.Instructions[i] = newJump;

                        Console.WriteLine(
                            $"IR replaced flag jump {CodeEmitVisitor.Instance.Visit(wiredJump)} with condition {CodeEmitVisitor.Instance.Visit(newJump)}."
                        );
                    }
                    else
                    {
                        Console.WriteLine(
                            $"IR failed to replace flag jump {CodeEmitVisitor.Instance.Visit(wiredJump)} with condition."
                        );
                    }
                }
            }
        }
    }
}
