using VibeDisasm.DecompilerEngine.IR;
using VibeDisasm.DecompilerEngine.IR.Expressions;
using VibeDisasm.Disassembler.X86;
using VibeDisasm.Disassembler.X86.Operands;
using System.Collections.Generic;

namespace VibeDisasm.DecompilerEngine.Visitors;

/// <summary>
/// Visitor that translates x86 operands to IR expressions
/// </summary>
public class IROperandVisitor : BaseOperandVisitor<IRExpression>
{
    public override IRExpression VisitRegister(RegisterOperand operand)
    {
        string regName = RegisterMapper.GetRegisterName(operand.Register, operand.Size);
        return new IRRegisterExpression(regName);
    }
    
    public override IRExpression VisitImmediate(ImmediateOperand operand)
    {
        return new IRConstantExpression(operand.Value);
    }
    
    public override IRExpression VisitDirectMemory(DirectMemoryOperand operand)
    {
        // Direct memory access like [0x12345678]
        var addressExpr = new IRConstantExpression((ulong)operand.Address);
        return new IRMemoryAccessExpression(addressExpr, operand.Size);
    }
    
    public override IRExpression VisitBaseRegisterMemory(BaseRegisterMemoryOperand operand)
    {
        // Base register like [eax]
        var baseReg = new IRRegisterExpression(
            RegisterMapper.GetRegisterName(operand.BaseRegister, operand.Size)
        );

        return new IRMemoryAccessExpression(baseReg, operand.Size);
    }
    
    public override IRExpression VisitScaledIndexMemory(ScaledIndexMemoryOperand operand)
    {
        // Complex addressing like [base + index*scale + disp]
        var components = new List<IRExpression>();
        
        // Add base register if present
        if (operand.BaseRegister is not null)
        {
            components.Add(
                new IRRegisterExpression(
                    RegisterMapper.GetRegisterName(operand.BaseRegister.Value, operand.Size)
                )
            );
        }
        
        // Add index*scale if present
        var indexReg = new IRRegisterExpression(
            RegisterMapper.GetRegisterName(operand.IndexRegister, operand.Size)
        );
        
        if (operand.Scale > 1)
        {
            var scaledIndex = new IRBinaryExpression(
                IRBinaryExpression.BinaryOperator.Multiply,
                indexReg,
                new IRConstantExpression((ulong)operand.Scale)
            );
            
            components.Add(scaledIndex);
        }
        else
        {
            components.Add(indexReg);
        }
        
        // Add displacement if non-zero
        if (operand.Displacement != 0)
        {
            components.Add(new IRConstantExpression((ulong)operand.Displacement));
        }
        
        // Combine all components with addition
        IRExpression addressExpr;
        if (components.Count == 0)
        {
            addressExpr = new IRConstantExpression(0);
        }
        else if (components.Count == 1)
        {
            addressExpr = components[0];
        }
        else
        {
            // Combine with binary expressions
            addressExpr = components[0];
            for (int i = 1; i < components.Count; i++)
            {
                addressExpr = new IRBinaryExpression(
                    IRBinaryExpression.BinaryOperator.Add,
                    addressExpr,
                    components[i]);
            }
        }
        
        return new IRMemoryAccessExpression(addressExpr, operand.Size);
    }
    
    public override IRExpression VisitRelativeOffset(RelativeOffsetOperand operand)
    {
        return new IRConstantExpression(operand.TargetAddress);
    }
    
    public override IRExpression VisitUnknown(Operand operand)
    {
        // Default fallback for unknown operand types
        return new IRConstantExpression(0);
    }
}
