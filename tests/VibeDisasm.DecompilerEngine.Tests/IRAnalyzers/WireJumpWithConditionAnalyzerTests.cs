using VibeDisasm.DecompilerEngine.IR.Expressions;
using VibeDisasm.DecompilerEngine.IR.Instructions;
using VibeDisasm.DecompilerEngine.IR.Model;
using VibeDisasm.DecompilerEngine.IRAnalyzers;
using VibeDisasm.DecompilerEngine.IRAnalyzers.IRLiftedInstructions;

namespace VibeDisasm.DecompilerEngine.Tests.IRAnalyzers;

/// <summary>
/// Tests for the WireJumpWithConditionAnalyzer class which connects jump instructions
/// to their flag-setting instructions.
/// </summary>
public class WireJumpWithConditionAnalyzerTests
{
    /// <summary>
    /// Tests that a simple conditional jump is correctly wired to its flag-setting instruction.
    /// </summary>
    [Fact]
    public void WireSimpleJumpWithCondition()
    {
        // Setup IR blocks and instructions for a simple conditional jump
        List<IRInstruction> targetInstructions = [];
        List<IRInstruction> mainInstructions =
        [
            // mov eax, 10
            new IRMoveInstruction(
                new IRRegisterExpr(IRRegister.EAX),
                IRConstantExpr.Int(10)
            ),

            // cmp eax, 5
            new IRCmpInstruction(
                new IRRegisterExpr(IRRegister.EAX),
                IRConstantExpr.Int(5)
            ),

            // jz targetBlock
            new IRJumpInstruction(
                null!,
                IRConstantExpr.Int(0x2000),
                new IRCompareExpr(
                    new IRFlagExpr(IRFlag.Zero),
                    IRConstantExpr.Bool(true),
                    IRComparisonType.Equal
                )
            )
        ];

        var targetBlock = new IRBlock
        {
            Id = "target",
            Instructions = targetInstructions
        };

        var srcBlock = new IRBlock
        {
            Id = "main",
            Instructions = mainInstructions
        };

        var irFunction = new IRFunction(
            "test_function",
            IRType.Int,
            [],
            [srcBlock, targetBlock]
        );
        var analyzer = new WireJumpWithConditionAnalyzer();

        analyzer.Handle(irFunction);

        var block = irFunction.Blocks[0];
        Assert.Equal(3, block.Instructions.Count);

        var wiredJump = Assert.IsType<IRWiredJumpInstruction>(block.Instructions[2]);
        Assert.NotNull(wiredJump.ConditionInstruction);
        Assert.IsType<IRCmpInstruction>(wiredJump.ConditionInstruction);
    }

    /// <summary>
    /// Tests that a jump with multiple flag conditions is correctly wired to its flag-setting instruction.
    /// </summary>
    [Fact]
    public void WireMultipleFlagsJumpWithCondition()
    {
        // Setup IR blocks and instructions with multiple flag conditions
        List<IRInstruction> targetInstructions = [];
        List<IRInstruction> mainInstructions =
        [
            new IRCmpInstruction(
                new IRRegisterExpr(IRRegister.EAX),
                IRConstantExpr.Int(5)
            ),

            // jnle targetBlock (Zero == false AND Sign == Overflow)
            new IRJumpInstruction(
                null!,
                IRConstantExpr.Int(0x2000),
                new IRLogicalExpr(
                    new IRCompareExpr(
                        new IRFlagExpr(IRFlag.Zero),
                        IRConstantExpr.Bool(false),
                        IRComparisonType.Equal
                    ),
                    new IRCompareExpr(
                        new IRFlagExpr(IRFlag.Sign),
                        new IRFlagExpr(IRFlag.Overflow),
                        IRComparisonType.Equal
                    ),
                    IRLogicalOperation.And
                )
            )
        ];

        var targetBlock = new IRBlock
        {
            Id = "target",
            Instructions = targetInstructions
        };

        var srcBlock = new IRBlock
        {
            Id = "main",
            Instructions = mainInstructions
        };

        var irFunction = new IRFunction(
            "test_function",
            IRType.Int,
            [],
            [srcBlock, targetBlock]
        );
        var analyzer = new WireJumpWithConditionAnalyzer();

        analyzer.Handle(irFunction);

        var block = irFunction.Blocks[0];
        Assert.Equal(2, block.Instructions.Count);

        var wiredJump = Assert.IsType<IRWiredJumpInstruction>(block.Instructions[1]);
        Assert.NotNull(wiredJump.ConditionInstruction);
        Assert.IsType<IRCmpInstruction>(wiredJump.ConditionInstruction);
    }

    /// <summary>
    /// Tests that no wiring occurs when there is no flag-setting instruction before the jump.
    /// </summary>
    [Fact]
    public void NoWiringWhenNoFlagSettingInstruction()
    {
        // Setup IR blocks and instructions without flag-setting instruction
        List<IRInstruction> targetInstructions = [];
        List<IRInstruction> mainInstructions =
        [
            // mov eax, 10 (does not set flags)
            new IRMoveInstruction(
                new IRRegisterExpr(IRRegister.EAX),
                IRConstantExpr.Int(10)
            ),

            // jz targetBlock - but no previous instruction sets Zero flag
            new IRJumpInstruction(
                null!,
                IRConstantExpr.Int(0x2000),
                new IRCompareExpr(
                    new IRFlagExpr(IRFlag.Zero),
                    IRConstantExpr.Bool(true),
                    IRComparisonType.Equal
                )
            )
        ];

        var targetBlock = new IRBlock
        {
            Id = "target",
            Instructions = targetInstructions
        };

        var srcBlock = new IRBlock
        {
            Id = "main",
            Instructions = mainInstructions
        };

        var irFunction = new IRFunction(
            "test_function",
            IRType.Int,
            [],
            [srcBlock, targetBlock]
        );
        var analyzer = new WireJumpWithConditionAnalyzer();

        analyzer.Handle(irFunction);
        var block = irFunction.Blocks[0];
        Assert.IsType<IRJumpInstruction>(block.Instructions[1]);
        Assert.IsNotType<IRWiredJumpInstruction>(block.Instructions[1]);
    }
}
