using VibeDisasm.DecompilerEngine.ControlFlow;
using VibeDisasm.DecompilerEngine.IR.Statements;
using VibeDisasm.DecompilerEngine.Visitors;

namespace VibeDisasm.DecompilerEngine.IR;

/// <summary>
/// Constructs the initial IR from control flow blocks
/// </summary>
public class IRBuilder
{
    private readonly IRInstructionVisitor _instructionVisitor;
    private readonly Dictionary<uint, IRBlockStatement> _blockCache = new();
    
    public IRBuilder()
    {
        var operandVisitor = new IROperandVisitor();
        _instructionVisitor = new IRInstructionVisitor(operandVisitor);
    }
    
    /// <summary>
    /// Builds an IR representation from a control flow function
    /// </summary>
    public IRFunction BuildFromFunction(ControlFlowFunction function)
    {
        if (function == null)
        {
            throw new ArgumentNullException(nameof(function));
        }
        
        var irFunction = new IRFunction(function);
        
        // Process all blocks in the function
        foreach (var block in function.Blocks.Values)
        {
            var irBlock = BuildFromBlock(block);
            irFunction.AddBlock(block.StartAddress, irBlock);
        }
        
        return irFunction;
    }
    
    /// <summary>
    /// Builds an IR block from a control flow block
    /// </summary>
    public IRBlockStatement BuildFromBlock(ControlFlowBlock block)
    {
        // Check if we've already processed this block
        if (_blockCache.TryGetValue(block.StartAddress, out var cachedBlock))
        {
            return cachedBlock;
        }
        
        // Create a new block statement
        var blockStatement = new IRBlockStatement();
        blockStatement.SourceReference = block;
        
        // Add to cache immediately to handle cycles
        _blockCache[block.StartAddress] = blockStatement;
        
        // Process each instruction in the block
        foreach (var instruction in block.Instructions)
        {
            var irStatement = BuildFromInstruction(instruction);
            if (irStatement != null)
            {
                blockStatement.AddStatement(irStatement);
            }
        }
        
        return blockStatement;
    }
    
    /// <summary>
    /// Builds an IR statement from a control flow instruction
    /// </summary>
    public IRStatement? BuildFromInstruction(ControlFlowInstruction instruction)
    {
        var rawInst = instruction.RawInstruction;
        var statement = _instructionVisitor.TranslateInstruction(rawInst);
        
        if (statement != null)
        {
            statement.SourceReference = instruction;
        }
        
        return statement;
    }
}
