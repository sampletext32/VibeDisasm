using VibeDisasm.DecompilerEngine.IR.Expressions;
using VibeDisasm.DecompilerEngine.IR.Statements;
using VibeDisasm.DecompilerEngine.IR.StructuredConstructs;

namespace VibeDisasm.DecompilerEngine.IR;

/// <summary>
/// Base visitor for traversing the IR tree
/// </summary>
public abstract class IRVisitor
{
    public virtual void Visit(IRNode node)
    {
        switch (node)
        {
            // Expressions
            case IRConstantExpression constant:
                VisitConstant(constant);
                break;
            case IRVariableExpression variable:
                VisitVariable(variable);
                break;
            case IRBinaryExpression binary:
                VisitBinary(binary);
                break;
            case IRRegisterExpression register:
                VisitRegister(register);
                break;
            case IRMemoryAccessExpression memAccess:
                VisitMemoryAccess(memAccess);
                break;
                
            // Statements
            case IRAssignmentStatement assignment:
                VisitAssignment(assignment);
                break;
            case IRBlockStatement block:
                VisitBlock(block);
                break;
            case IRJumpStatement jump:
                VisitJump(jump);
                break;
            case IRConditionalStatement conditional:
                VisitConditional(conditional);
                break;
            case IRReturnStatement ret:
                VisitReturn(ret);
                break;
                
            // Structured constructs
            case IRWhileLoop whileLoop:
                VisitWhileLoop(whileLoop);
                break;
            case IRDoWhileLoop doWhileLoop:
                VisitDoWhileLoop(doWhileLoop);
                break;
            case IRForLoop forLoop:
                VisitForLoop(forLoop);
                break;
            case IRSwitchStatement switchStmt:
                VisitSwitch(switchStmt);
                break;
        }
    }
    
    // Expression visitors
    protected virtual void VisitConstant(IRConstantExpression node) => VisitChildren(node);
    protected virtual void VisitVariable(IRVariableExpression node) => VisitChildren(node);
    protected virtual void VisitBinary(IRBinaryExpression node) => VisitChildren(node);
    protected virtual void VisitRegister(IRRegisterExpression node) => VisitChildren(node);
    protected virtual void VisitMemoryAccess(IRMemoryAccessExpression node) => VisitChildren(node);
    
    // Statement visitors
    protected virtual void VisitAssignment(IRAssignmentStatement node) => VisitChildren(node);
    protected virtual void VisitBlock(IRBlockStatement node) => VisitChildren(node);
    protected virtual void VisitJump(IRJumpStatement node) => VisitChildren(node);
    protected virtual void VisitConditional(IRConditionalStatement node) => VisitChildren(node);
    protected virtual void VisitReturn(IRReturnStatement node) => VisitChildren(node);
    
    // Structured construct visitors
    protected virtual void VisitWhileLoop(IRWhileLoop node) => VisitChildren(node);
    protected virtual void VisitDoWhileLoop(IRDoWhileLoop node) => VisitChildren(node);
    protected virtual void VisitForLoop(IRForLoop node) => VisitChildren(node);
    protected virtual void VisitSwitch(IRSwitchStatement node) => VisitChildren(node);
    
    // Helper to visit all children of a node
    protected virtual void VisitChildren(IRNode node)
    {
        foreach (var child in node.Children)
        {
            Visit(child);
        }
    }
}
