using VibeDisasm.Disassembler.X86;
using VibeDisasm.Disassembler.X86.Operands;

namespace VibeDisasm.DecompilerEngine.IR.Expressions;

/// <summary>
/// Represents a memory access operation in the IR
/// </summary>
public class IRMemoryAccessExpression : IRExpression
{
    public IRExpression Address { get; }
    public int Size { get; }
    
    public IRMemoryAccessExpression(IRExpression address, int size) 
        : base(IRNodeType.MemoryAccess)
    {
        Address = address;
        Size = size;
        
        AddChild(address);
    }
    
    public static IRMemoryAccessExpression FromMemoryOperand(MemoryOperand memoryOperand, IRBuilder builder)
    {
        // This is a simplified implementation that will need to be expanded
        // based on the actual memory operand types
        
        IRExpression addressExpression;
        
        if (memoryOperand is DirectMemoryOperand directMemory)
        {
            // Direct memory access like [0x12345678]
            addressExpression = new IRConstantExpression((ulong)directMemory.Address);
        }
        else if (memoryOperand is BaseRegisterMemoryOperand baseRegisterMemory)
        {
            // Base register
            var baseReg = new IRRegisterExpression(
                RegisterMapper.GetRegisterName(baseRegisterMemory.BaseRegister, baseRegisterMemory.Size)
            );

            addressExpression = baseReg;
        }
        else if (memoryOperand is ScaledIndexMemoryOperand scaledIndexMemory)
        {
            // Complex addressing like [base + index*scale + disp]
            var components = new List<IRExpression>();

            // Add base register if present
            if (scaledIndexMemory.BaseRegister is not null)
            {
                components.Add(
                    new IRRegisterExpression(
                        RegisterMapper.GetRegisterName(scaledIndexMemory.BaseRegister.Value, scaledIndexMemory.Size)
                    )
                );
            }

            // Add index*scale if present
            var indexReg = new IRRegisterExpression(
                RegisterMapper.GetRegisterName(scaledIndexMemory.IndexRegister, scaledIndexMemory.Size)
            );

            if (scaledIndexMemory.Scale > 1)
            {
                var scaledIndex = new IRBinaryExpression(
                    IRBinaryExpression.BinaryOperator.Multiply,
                    indexReg,
                    new IRConstantExpression((ulong) scaledIndexMemory.Scale)
                );

                components.Add(scaledIndex);
            }
            else
            {
                components.Add(indexReg);
            }
            
            // Add displacement if non-zero
            if (scaledIndexMemory.Displacement != 0)
            {
                components.Add(new IRConstantExpression((ulong)scaledIndexMemory.Displacement));
            }
            
            // Combine all components with addition
            if (components.Count == 0)
            {
                addressExpression = new IRConstantExpression(0);
            }
            else if (components.Count == 1)
            {
                addressExpression = components[0];
            }
            else
            {
                // Combine with binary expressions
                addressExpression = components[0];
                for (int i = 1; i < components.Count; i++)
                {
                    addressExpression = new IRBinaryExpression(
                        IRBinaryExpression.BinaryOperator.Add,
                        addressExpression,
                        components[i]);
                }
            }
        }
        else
        {
            // Default fallback for other memory operand types
            addressExpression = new IRConstantExpression(0);
        }
        
        return new IRMemoryAccessExpression(addressExpression, memoryOperand.Size);
    }
    
    public override string ToString() => $"*({Address}){SizeToSuffix()}";
    
    private string SizeToSuffix()
    {
        return Size switch
        {
            8 => ".b",
            16 => ".w",
            32 => ".d",
            64 => ".q",
            _ => ""
        };
    }
}
