using VibeDisasm.DecompilerEngine.ControlFlow;
using VibeDisasm.Disassembler.X86;
using VibeDisasm.Disassembler.X86.Operands;
using VibeDisasm.DecompilerEngine.IR.Expressions;
using VibeDisasm.DecompilerEngine.IR.Statements;

namespace VibeDisasm.DecompilerEngine.IR;

/// <summary>
/// Constructs the initial IR from control flow blocks
/// </summary>
public class IRBuilder
{
    private readonly Dictionary<uint, IRBlockStatement> _blockCache = [];
    
    /// <summary>
    /// Builds an IR representation from a control flow function
    /// </summary>
    public IRBlockStatement BuildFromFunction(ControlFlowFunction function)
    {
        if (function == null)
        {
            throw new ArgumentNullException(nameof(function));
        }
        
        // Find the entry block
        var entryBlock = function.Blocks.Values.FirstOrDefault(b => b.IsEntryBlock);
        if (entryBlock == null)
        {
            throw new InvalidOperationException("Function has no entry block");
        }
        
        // Build IR for the entire function starting from the entry block
        return BuildFromBlock(entryBlock);
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
        IRStatement? statement = null;
        
        switch (rawInst.Type)
        {
            // Data movement instructions
            case InstructionType.Mov:
                statement = BuildMoveInstruction(rawInst);
                break;
                
            // Arithmetic instructions
            case InstructionType.Add:
                statement = BuildBinaryOperation(rawInst, IRBinaryExpression.BinaryOperator.Add);
                break;
            case InstructionType.Sub:
                statement = BuildBinaryOperation(rawInst, IRBinaryExpression.BinaryOperator.Subtract);
                break;
            case InstructionType.Mul:
            case InstructionType.IMul:
                statement = BuildBinaryOperation(rawInst, IRBinaryExpression.BinaryOperator.Multiply);
                break;
            case InstructionType.Div:
            case InstructionType.IDiv:
                statement = BuildBinaryOperation(rawInst, IRBinaryExpression.BinaryOperator.Divide);
                break;
            case InstructionType.Inc:
                statement = BuildUnaryOperation(rawInst, IRUnaryExpression.UnaryOperator.Increment);
                break;
            case InstructionType.Dec:
                statement = BuildUnaryOperation(rawInst, IRUnaryExpression.UnaryOperator.Decrement);
                break;
            case InstructionType.Neg:
                statement = BuildUnaryOperation(rawInst, IRUnaryExpression.UnaryOperator.Negate);
                break;
                
            // Logical instructions
            case InstructionType.And:
                statement = BuildBinaryOperation(rawInst, IRBinaryExpression.BinaryOperator.BitwiseAnd);
                break;
            case InstructionType.Or:
                statement = BuildBinaryOperation(rawInst, IRBinaryExpression.BinaryOperator.BitwiseOr);
                break;
            case InstructionType.Xor:
                statement = BuildBinaryOperation(rawInst, IRBinaryExpression.BinaryOperator.BitwiseXor);
                break;
            case InstructionType.Not:
                statement = BuildUnaryOperation(rawInst, IRUnaryExpression.UnaryOperator.BitwiseNot);
                break;
            case InstructionType.Shl:
                statement = BuildBinaryOperation(rawInst, IRBinaryExpression.BinaryOperator.LeftShift);
                break;
            case InstructionType.Shr:
                statement = BuildBinaryOperation(rawInst, IRBinaryExpression.BinaryOperator.RightShift);
                break;
                
            // Control flow instructions
            case InstructionType.Jmp:
            case InstructionType.Jg:
            case InstructionType.Jge:
            case InstructionType.Jl:
            case InstructionType.Jle:
            case InstructionType.Ja:
            case InstructionType.Jae:
            case InstructionType.Jb:
            case InstructionType.Jbe:
            case InstructionType.Jz:
            case InstructionType.Jnz:
            case InstructionType.Jo:
            case InstructionType.Jno:
            case InstructionType.Js:
            case InstructionType.Jns:
            case InstructionType.Jp:
            case InstructionType.Jnp:
                var targetAddress = instruction.GetJumpTargetAddress();
                if (targetAddress.HasValue)
                {
                    var targetExpr = new IRConstantExpression(targetAddress.Value);
                    statement = new IRJumpStatement(targetExpr);
                }
                break;
                
            case InstructionType.Call:
                statement = BuildCallInstruction(rawInst);
                break;
                
            case InstructionType.Ret:
                statement = new IRReturnStatement();
                break;
        }
        
        if (statement != null)
        {
            statement.SourceReference = instruction;
        }
        
        return statement;
    }
    
    // Helper methods for building IR from specific instruction types
    
    private IRStatement BuildMoveInstruction(Instruction instruction)
    {
        if (instruction.StructuredOperands.Count < 2)
        {
            return new IRBlockStatement(); // Empty block as fallback
        }
        
        var destExpr = BuildOperandExpression(instruction.StructuredOperands[0]);
        var srcExpr = BuildOperandExpression(instruction.StructuredOperands[1]);
        
        return new IRAssignmentStatement(destExpr, srcExpr);
    }
    
    private IRStatement BuildBinaryOperation(Instruction instruction, IRBinaryExpression.BinaryOperator op)
    {
        if (instruction.StructuredOperands.Count < 2)
        {
            return new IRBlockStatement(); // Empty block as fallback
        }
        
        var destExpr = BuildOperandExpression(instruction.StructuredOperands[0]);
        var srcExpr = BuildOperandExpression(instruction.StructuredOperands[1]);
        
        var binaryExpr = new IRBinaryExpression(op, destExpr, srcExpr);
        
        return new IRAssignmentStatement(destExpr, binaryExpr);
    }
    
    private IRStatement BuildUnaryOperation(Instruction instruction, IRUnaryExpression.UnaryOperator op)
    {
        if (instruction.StructuredOperands.Count < 1)
        {
            return new IRBlockStatement(); // Empty block as fallback
        }
        
        var operandExpr = BuildOperandExpression(instruction.StructuredOperands[0]);
        var unaryExpr = new IRUnaryExpression(op, operandExpr);
        
        return new IRAssignmentStatement(operandExpr, unaryExpr);
    }
    
    private IRStatement BuildCallInstruction(Instruction instruction)
    {
        if (instruction.StructuredOperands.Count < 1)
        {
            return new IRBlockStatement(); // Empty block as fallback
        }
        
        var targetExpr = BuildOperandExpression(instruction.StructuredOperands[0]);
        
        // For now, we'll just create a simple call statement without parameters
        return new IRCallStatement(targetExpr, []);
    }
    
    // Helper method to build IR expressions from x86 operands
    
    public IRExpression BuildOperandExpression(Operand operand)
    {
        switch (operand.Type)
        {
            case OperandType.Register:
                if (operand is RegisterOperand regOp)
                {
                    string regName = RegisterMapper.GetRegisterName(regOp.Register, regOp.Size);
                    return new IRRegisterExpression(regName);
                }
                break;
                
            case OperandType.ImmediateValue:
                if (operand is ImmediateOperand immOp)
                {
                    return new IRConstantExpression(immOp.Value);
                }
                break;
                
            case OperandType.MemoryDirect:
                if (operand is DirectMemoryOperand memOp)
                {
                    return IRMemoryAccessExpression.FromMemoryOperand(memOp, this);
                }
                break;
                
            case OperandType.RelativeOffset:
                if (operand is RelativeOffsetOperand relOp)
                {
                    return new IRConstantExpression(relOp.TargetAddress);
                }
                break;
        }
        
        // Default fallback
        return new IRConstantExpression(0);
    }
}
