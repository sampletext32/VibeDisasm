using VibeDisasm.DecompilerEngine.IR;
using VibeDisasm.DecompilerEngine.IR.Expressions;
using VibeDisasm.DecompilerEngine.IR.Statements;
using VibeDisasm.Disassembler.X86;
using System.Collections.Generic;

namespace VibeDisasm.DecompilerEngine.Visitors;

/// <summary>
/// Visitor that translates x86 instructions to IR statements
/// </summary>
public class IRInstructionVisitor : BaseInstructionVisitor<IRStatement>
{
    private readonly IROperandVisitor _operandVisitor;
    
    public IRInstructionVisitor(IROperandVisitor operandVisitor)
    {
        _operandVisitor = operandVisitor;
    }
    
    public IRInstructionVisitor()
    {
        _operandVisitor = new IROperandVisitor();
    }
    
    /// <summary>
    /// Translates an x86 instruction to an IR statement
    /// </summary>
    public IRStatement? TranslateInstruction(Instruction instruction)
    {
        Result = null;
        Visit(instruction);
        return Result;
    }
    
    // Data movement instructions
    
    public override void VisitMov(Instruction instruction)
    {
        if (instruction.StructuredOperands.Count < 2)
        {
            return;
        }
        
        var destExpr = _operandVisitor.Visit(instruction.StructuredOperands[0]);
        var srcExpr = _operandVisitor.Visit(instruction.StructuredOperands[1]);
        
        Result = new IRAssignmentStatement(destExpr, srcExpr);
    }
    
    public override void VisitPush(Instruction instruction)
    {
        if (instruction.StructuredOperands.Count < 1)
        {
            return;
        }
        
        var valueExpr = _operandVisitor.Visit(instruction.StructuredOperands[0]);
        
        // Create a comment to represent the push operation
        var comment = $"PUSH {instruction.StructuredOperands[0]}";
        
        // For now, we'll represent this as a special assignment to a stack variable
        var stackExpr = new IRVariableExpression("stack");
        Result = new IRAssignmentStatement(stackExpr, valueExpr) { Comment = comment };
    }
    
    public override void VisitPop(Instruction instruction)
    {
        if (instruction.StructuredOperands.Count < 1)
        {
            return;
        }
        
        var targetExpr = _operandVisitor.Visit(instruction.StructuredOperands[0]);
        
        // Create a comment to represent the pop operation
        var comment = $"POP {instruction.StructuredOperands[0]}";
        
        // For now, we'll represent this as a special assignment from a stack variable
        var stackExpr = new IRVariableExpression("stack");
        Result = new IRAssignmentStatement(targetExpr, stackExpr) { Comment = comment };
    }
    
    // Arithmetic instructions
    
    public override void VisitAdd(Instruction instruction)
    {
        CreateBinaryOperation(instruction, IRBinaryExpression.BinaryOperator.Add);
    }
    
    public override void VisitSub(Instruction instruction)
    {
        CreateBinaryOperation(instruction, IRBinaryExpression.BinaryOperator.Subtract);
    }
    
    public override void VisitMul(Instruction instruction)
    {
        CreateBinaryOperation(instruction, IRBinaryExpression.BinaryOperator.Multiply);
    }
    
    public override void VisitDiv(Instruction instruction)
    {
        CreateBinaryOperation(instruction, IRBinaryExpression.BinaryOperator.Divide);
    }
    
    public override void VisitInc(Instruction instruction)
    {
        CreateUnaryOperation(instruction, IRUnaryExpression.UnaryOperator.Increment);
    }
    
    public override void VisitDec(Instruction instruction)
    {
        CreateUnaryOperation(instruction, IRUnaryExpression.UnaryOperator.Decrement);
    }
    
    public override void VisitNeg(Instruction instruction)
    {
        CreateUnaryOperation(instruction, IRUnaryExpression.UnaryOperator.Negate);
    }
    
    // Logical instructions
    
    public override void VisitAnd(Instruction instruction)
    {
        CreateBinaryOperation(instruction, IRBinaryExpression.BinaryOperator.BitwiseAnd);
    }
    
    public override void VisitOr(Instruction instruction)
    {
        CreateBinaryOperation(instruction, IRBinaryExpression.BinaryOperator.BitwiseOr);
    }
    
    public override void VisitXor(Instruction instruction)
    {
        CreateBinaryOperation(instruction, IRBinaryExpression.BinaryOperator.BitwiseXor);
    }
    
    public override void VisitNot(Instruction instruction)
    {
        CreateUnaryOperation(instruction, IRUnaryExpression.UnaryOperator.BitwiseNot);
    }
    
    public override void VisitTest(Instruction instruction)
    {
        if (instruction.StructuredOperands.Count < 2)
        {
            return;
        }
        
        var leftExpr = _operandVisitor.Visit(instruction.StructuredOperands[0]);
        var rightExpr = _operandVisitor.Visit(instruction.StructuredOperands[1]);
        
        // Create a comment to represent the test operation
        var comment = $"TEST {instruction.StructuredOperands[0]}, {instruction.StructuredOperands[1]}";
        
        // Test is a bitwise AND that doesn't store the result but sets flags
        // We'll represent it as an assignment to a special flags variable
        var binaryExpr = new IRBinaryExpression(IRBinaryExpression.BinaryOperator.BitwiseAnd, leftExpr, rightExpr);
        var flagsExpr = new IRVariableExpression("flags");
        Result = new IRAssignmentStatement(flagsExpr, binaryExpr) { Comment = comment };
    }
    
    public override void VisitCmp(Instruction instruction)
    {
        if (instruction.StructuredOperands.Count < 2)
        {
            return;
        }
        
        var leftExpr = _operandVisitor.Visit(instruction.StructuredOperands[0]);
        var rightExpr = _operandVisitor.Visit(instruction.StructuredOperands[1]);
        
        // Create a comment to represent the compare operation
        var comment = $"CMP {instruction.StructuredOperands[0]}, {instruction.StructuredOperands[1]}";
        
        // Compare is a subtraction that doesn't store the result but sets flags
        // We'll represent it as an assignment to a special flags variable
        var binaryExpr = new IRBinaryExpression(IRBinaryExpression.BinaryOperator.Subtract, leftExpr, rightExpr);
        var flagsExpr = new IRVariableExpression("flags");
        Result = new IRAssignmentStatement(flagsExpr, binaryExpr) { Comment = comment };
    }
    
    public override void VisitShl(Instruction instruction)
    {
        CreateBinaryOperation(instruction, IRBinaryExpression.BinaryOperator.LeftShift);
    }
    
    public override void VisitShr(Instruction instruction)
    {
        CreateBinaryOperation(instruction, IRBinaryExpression.BinaryOperator.RightShift);
    }
    
    // Control flow instructions
    
    public override void VisitJmp(Instruction instruction)
    {
        if (instruction.StructuredOperands.Count < 1)
        {
            return;
        }
        
        var targetExpr = _operandVisitor.Visit(instruction.StructuredOperands[0]);
        Result = new IRJumpStatement(targetExpr);
    }
    
    public override void VisitJcc(Instruction instruction)
    {
        if (instruction.StructuredOperands.Count < 1)
        {
            return;
        }
        
        var targetExpr = _operandVisitor.Visit(instruction.StructuredOperands[0]);
        var comment = $"{instruction.Type} {instruction.StructuredOperands[0]}";
        Result = new IRJumpStatement(targetExpr) { Comment = comment };
    }
    
    public override void VisitCall(Instruction instruction)
    {
        if (instruction.StructuredOperands.Count < 1)
        {
            return;
        }
        
        var targetExpr = _operandVisitor.Visit(instruction.StructuredOperands[0]);
        var comment = $"{instruction.Type} {instruction.StructuredOperands[0]}";
        Result = new IRCallStatement(targetExpr, []) { Comment = comment };
    }
    
    public override void VisitRet(Instruction instruction)
    {
        var comment = $"{instruction.Type}";
        Result = new IRReturnStatement() { Comment = comment };
    }
    
    public override void VisitUnknown(Instruction instruction)
    {
        // Create a comment for unknown instructions
        var comment = $"Unknown instruction: {instruction}";
        Result = new IRBlockStatement() { Comment = comment };
    }
    
    // Helper methods
    
    private void CreateBinaryOperation(Instruction instruction, IRBinaryExpression.BinaryOperator op)
    {
        if (instruction.StructuredOperands.Count < 2)
        {
            return;
        }
        
        var destExpr = _operandVisitor.Visit(instruction.StructuredOperands[0]);
        var srcExpr = _operandVisitor.Visit(instruction.StructuredOperands[1]);
        
        var binaryExpr = new IRBinaryExpression(op, destExpr, srcExpr);
        
        Result = new IRAssignmentStatement(destExpr, binaryExpr);
    }
    
    private void CreateUnaryOperation(Instruction instruction, IRUnaryExpression.UnaryOperator op)
    {
        if (instruction.StructuredOperands.Count < 1)
        {
            return;
        }
        
        var operandExpr = _operandVisitor.Visit(instruction.StructuredOperands[0]);
        var unaryExpr = new IRUnaryExpression(op, operandExpr);
        
        Result = new IRAssignmentStatement(operandExpr, unaryExpr);
    }
}
