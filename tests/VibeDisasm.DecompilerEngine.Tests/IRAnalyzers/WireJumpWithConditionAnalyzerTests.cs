using VibeDisasm.DecompilerEngine.IREverything.Expressions;
using VibeDisasm.DecompilerEngine.IREverything.Instructions;
using VibeDisasm.DecompilerEngine.IREverything.IRAnalyzers;
using VibeDisasm.DecompilerEngine.IREverything.IRAnalyzers.IRLiftedInstructions;
using VibeDisasm.DecompilerEngine.IREverything.Model;

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
                IR.Int(10)
            ),

            // cmp eax, 5
            new IRCmpInstruction(
                new IRRegisterExpr(IRRegister.EAX),
                IR.Int(5)
            ),

            // jz targetBlock
            new IRJumpInstruction(
                null!,
                IR.Int(0x2000),
                IR.CompareEqual(
                    new IRFlagExpr(IRFlag.Zero),
                    IR.True()
                )
            )
        ];

        var targetBlock = new IRBlock(
            0x00,
            targetInstructions,
            false
        );

        var srcBlock = new IRBlock(
            0x10,
            mainInstructions,
            false
        );

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
                IR.Int(5)
            ),

            // jnle targetBlock (Zero == false AND Sign == Overflow)
            new IRJumpInstruction(
                null!,
                IR.Int(0x2000),
                IR.LogicalAnd(
                    IR.CompareEqual(
                        new IRFlagExpr(IRFlag.Zero),
                        IR.False()
                    ),
                    IR.CompareEqual(
                        new IRFlagExpr(IRFlag.Sign),
                        new IRFlagExpr(IRFlag.Overflow)
                    )
                )
            )
        ];

        var targetBlock = new IRBlock(
            0x00,
            targetInstructions,
            false
        );

        var srcBlock = new IRBlock(
            0x10,
            mainInstructions,
            false
        );

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
                IR.Int(10)
            ),

            // jz targetBlock - but no previous instruction sets Zero flag
            new IRJumpInstruction(
                null!,
                IR.Int(0x2000),
                IR.CompareEqual(
                    new IRFlagExpr(IRFlag.Zero),
                    IR.True()
                )
            )
        ];

        var targetBlock = new IRBlock(
            0x00,
            targetInstructions,
            false
        );

        var srcBlock = new IRBlock(
            0x10,
            mainInstructions,
            false
        );

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
