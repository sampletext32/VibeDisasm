using VibeDisasm.DecompilerEngine.IR.Expressions;
using VibeDisasm.DecompilerEngine.IR.Instructions;
using VibeDisasm.DecompilerEngine.IR.Model;
using VibeDisasm.DecompilerEngine.IR.Visitors;
using VibeDisasm.DecompilerEngine.IRAnalyzers.IRLiftedInstructions;

namespace VibeDisasm.DecompilerEngine.IRAnalyzers;

public class WireJumpWithConditionAnalyzer
{
    public void Handle(IRFunction function)
    {
        foreach (var functionBlock in function.Blocks)
        {
            var instructions = functionBlock.Instructions;
            for (var i = 0; i < instructions.Count; i++)
            {
                if (instructions[i] is IRJumpInstruction jump)
                {
                    if (jump.Condition is null)
                    {
                        continue;
                    }

                    // backtrack instruction to find condition
                    var neededFlags = jump.EnumerateAllExpressionsOfType<IRFlagExpr>()
                        .DistinctBy(x => x.Flag)
                        .Select(x => x.Flag)
                        .ToList();

                    var found = false;
                    for (var j = i - 1; j >= 0; j--)
                    {
                        var sideEffects = SideEffectsVisitor.Instance.Visit(instructions[j])
                            ?? throw new InvalidOperationException();

                        // TODO: there might be an instruction that modifies a part of flags (e.g. only ZF), and another for other part (only CF)
                        // TODO: in this case we would create an invalid wiring, but it's okay for now.
                        // need to check if the instruction covers all flags, because for example Jnbe modifies Carry and Zero
                        if (sideEffects.Select(x => x.Flag)
                            .Intersect(neededFlags)
                            .Any())
                        {
                            // if the instruction modifies the flags we need, we can replace the jump condition with the instruction
                            instructions[i] = new IRWiredJumpInstruction(jump, instructions[j]);

                            Console.WriteLine(
                                $"IR wired jump {CodeEmitVisitor.Instance.Visit(jump)} with condition {CodeEmitVisitor.Instance.Visit(instructions[j])}."
                            );

                            found = true;
                            break;
                        }
                    }

                    if (!found)
                    {
                        Console.WriteLine(
                            $"IR attempted to wire jump {jump}" +
                            $"but no preceeding instruction found that modifies the flags {string.Join(", ", neededFlags)}."
                        );
                    }
                }
            }
        }
    }
}
