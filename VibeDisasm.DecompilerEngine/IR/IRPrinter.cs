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
    private StringBuilder _sb = new();
    private int _indentLevel = 0;
    
    public string Print(IRNode node)
    {
        _sb.Clear();
        _indentLevel = 0;
        Visit(node);
        return _sb.ToString();
    }
    
    public override void Visit(IRNode node)
    {
        if (node is IRFunction function)
        {
            VisitFunction(function);
        }
        else
        {
            base.Visit(node);
        }
    }
    
    protected void VisitFunction(IRFunction function)
    {
        _sb.AppendLine($"Function {function.Name}:");
        
        // Print blocks in address order for readability
        foreach (var blockEntry in function.Blocks.OrderBy(x => x.Key))
        {
            uint address = blockEntry.Key;
            IRBlockStatement block = blockEntry.Value;
            
            _sb.AppendLine();
            _sb.AppendLine($"Block_{address:X8}" + (block == function.EntryBlock ? " (Entry)" : "") + ":");
            VisitBlock(block);
        }
    }
    
    private void Indent()
    {
        _indentLevel++;
    }
    
    private void Dedent()
    {
        _indentLevel = Math.Max(0, _indentLevel - 1);
    }
    
    private void AppendLine(string text, string? comment = null)
    {
        _sb.Append(new string(' ', _indentLevel * 2));
        _sb.Append(text);
        
        if (!string.IsNullOrEmpty(comment))
        {
            // Add some spacing before the comment
            if (text.Length < 30)
            {
                _sb.Append(new string(' ', 30 - text.Length));
            }
            else
            {
                _sb.Append("  ");
            }
            
            _sb.Append("// ").Append(comment);
        }
        
        _sb.AppendLine();
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
    
    protected override void VisitMemoryAccess(IRMemoryAccessExpression node)
    {
        _sb.Append("[");
        
        // Special case for constant addresses - format them in hex
        if (node.Address is IRConstantExpression constExpr && constExpr.Value is ulong value)
        {
            _sb.Append($"0x{value:X}");
        }
        else
        {
            Visit(node.Address);
        }
        
        _sb.Append("]");
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
        _sb.Append(new string(' ', _indentLevel * 2));
        Visit(node.Target);
        _sb.Append(" = ");
        Visit(node.Value);
        _sb.Append(";");
        
        if (!string.IsNullOrEmpty(node.Comment))
        {
            // Add some spacing before the comment
            _sb.Append("  // ").Append(node.Comment);
        }
        
        _sb.AppendLine();
    }
    
    protected override void VisitBlock(IRBlockStatement node)
    {
        Indent();
        AppendLine("{");
        Indent();
        
        foreach (var statement in node.Statements)
        {
            Visit(statement);
        }
        
        Dedent();
        AppendLine("}");
        Dedent();
    }
    
    protected override void VisitJump(IRJumpStatement node)
    {
        AppendLine("goto " + FormatJumpTarget(node.Target) + ";", node.Comment);
    }
    
    protected override void VisitCall(IRCallStatement node)
    {
        // Format the call target
        string targetStr;
        if (node.Target is IRConstantExpression constExpr)
        {
            // Format the address as hex
            targetStr = $"0x{constExpr.Value:X8}";
        }
        else
        {
            // For other expression types, use a temporary StringBuilder
            var sb = new StringBuilder();
            var originalSb = _sb;
            _sb = sb;
            Visit(node.Target);
            _sb = originalSb;
            targetStr = sb.ToString();
        }
        
        // Format the arguments if any
        string argsStr = "";
        if (node.Arguments.Count > 0)
        {
            var sb = new StringBuilder();
            for (int i = 0; i < node.Arguments.Count; i++)
            {
                if (i > 0) sb.Append(", ");
                var originalSb = _sb;
                _sb = sb;
                Visit(node.Arguments[i]);
                _sb = originalSb;
            }
            argsStr = sb.ToString();
        }
        
        AppendLine($"call {targetStr}({argsStr});", node.Comment);
    }
    
    private string FormatJumpTarget(IRExpression target)
    {
        if (target is IRConstantExpression constExpr)
        {
            // Format the address as hex
            return $"0x{constExpr.Value:X8}";
        }
        
        // For other expression types, use the default ToString
        var sb = new StringBuilder();
        var originalSb = _sb;
        _sb = sb;
        Visit(target);
        _sb = originalSb;
        return sb.ToString();
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
    
    protected override void VisitUnknown(IRNode node)
    {
        // Print a warning for unsupported IR nodes
        AppendLine($"// UNSUPPORTED IR NODE: {node.GetType().Name}");
        
        // Still visit children to ensure we don't miss anything
        base.VisitUnknown(node);
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
