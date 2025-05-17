using System.Text;
using VibeDisasm.DecompilerEngine.IR.Expressions;
using VibeDisasm.DecompilerEngine.IR.Statements;
using VibeDisasm.DecompilerEngine.IR.StructuredConstructs;

namespace VibeDisasm.DecompilerEngine.IR;

/// <summary>
/// Generates a textual representation of the IR for debugging
/// </summary>
public class IRPrinter : IRVisitor
{
    private readonly StringBuilder _sb = new();
    private int _indentLevel = 0;
    
    public string Print(IRNode node)
    {
        _sb.Clear();
        _indentLevel = 0;
        Visit(node);
        return _sb.ToString();
    }
    
    private void Indent()
    {
        _indentLevel++;
    }
    
    private void Dedent()
    {
        _indentLevel = Math.Max(0, _indentLevel - 1);
    }
    
    private void AppendLine(string text)
    {
        _sb.Append(new string(' ', _indentLevel * 2));
        _sb.AppendLine(text);
    }
    
    // Expression visitors
    protected override void VisitConstant(IRConstantExpression node)
    {
        _sb.Append(node.Value);
    }
    
    protected override void VisitVariable(IRVariableExpression node)
    {
        _sb.Append(node.Name);
    }
    
    protected override void VisitRegister(IRRegisterExpression node)
    {
        _sb.Append(node.RegisterName);
    }
    
    protected override void VisitBinary(IRBinaryExpression node)
    {
        _sb.Append("(");
        Visit(node.Left);
        _sb.Append(" ");
        _sb.Append(GetOperatorSymbol(node.Operator));
        _sb.Append(" ");
        Visit(node.Right);
        _sb.Append(")");
    }
    
    // Statement visitors
    protected override void VisitAssignment(IRAssignmentStatement node)
    {
        Visit(node.Target);
        _sb.Append(" = ");
        Visit(node.Value);
        _sb.AppendLine(";");
    }
    
    protected override void VisitBlock(IRBlockStatement node)
    {
        AppendLine("{");
        Indent();
        
        foreach (var statement in node.Statements)
        {
            Visit(statement);
        }
        
        Dedent();
        AppendLine("}");
    }
    
    protected override void VisitJump(IRJumpStatement node)
    {
        _sb.Append("goto ");
        Visit(node.Target);
        _sb.AppendLine(";");
    }
    
    protected override void VisitReturn(IRReturnStatement node)
    {
        _sb.Append("return");
        
        if (node.Value != null)
        {
            _sb.Append(" ");
            Visit(node.Value);
        }
        
        _sb.AppendLine(";");
    }
    
    protected override void VisitConditional(IRConditionalStatement node)
    {
        _sb.Append("if (");
        Visit(node.Condition);
        _sb.AppendLine(")");
        
        Visit(node.ThenBlock);
        
        if (node.ElseBlock != null)
        {
            AppendLine("else");
            Visit(node.ElseBlock);
        }
    }
    
    // Structured construct visitors
    protected override void VisitWhileLoop(IRWhileLoop node)
    {
        _sb.Append("while (");
        Visit(node.Condition);
        _sb.AppendLine(")");
        
        Visit(node.Body);
    }
    
    protected override void VisitDoWhileLoop(IRDoWhileLoop node)
    {
        AppendLine("do");
        Visit(node.Body);
        
        _sb.Append("while (");
        Visit(node.Condition);
        _sb.AppendLine(");");
    }
    
    protected override void VisitForLoop(IRForLoop node)
    {
        _sb.Append("for (");
        Visit(node.Initialization);
        _sb.Append(" ");
        Visit(node.Condition);
        _sb.Append("; ");
        Visit(node.Iteration);
        _sb.AppendLine(")");
        
        Visit(node.Body);
    }
    
    protected override void VisitSwitch(IRSwitchStatement node)
    {
        _sb.Append("switch (");
        Visit(node.Expression);
        _sb.AppendLine(")");
        AppendLine("{");
        Indent();
        
        foreach (var caseBlock in node.Cases)
        {
            _sb.Append("case ");
            Visit(caseBlock.Value);
            _sb.AppendLine(":");
            Indent();
            Visit(caseBlock.Body);
            Dedent();
        }
        
        if (node.DefaultCase != null)
        {
            AppendLine("default:");
            Indent();
            Visit(node.DefaultCase);
            Dedent();
        }
        
        Dedent();
        AppendLine("}");
    }
    
    // Helper methods
    private string GetOperatorSymbol(IRBinaryExpression.BinaryOperator op)
    {
        return op switch
        {
            IRBinaryExpression.BinaryOperator.Add => "+",
            IRBinaryExpression.BinaryOperator.Subtract => "-",
            IRBinaryExpression.BinaryOperator.Multiply => "*",
            IRBinaryExpression.BinaryOperator.Divide => "/",
            IRBinaryExpression.BinaryOperator.BitwiseAnd => "&",
            IRBinaryExpression.BinaryOperator.BitwiseOr => "|",
            IRBinaryExpression.BinaryOperator.BitwiseXor => "^",
            IRBinaryExpression.BinaryOperator.LeftShift => "<<",
            IRBinaryExpression.BinaryOperator.RightShift => ">>",
            _ => "??"
        };
    }
}
