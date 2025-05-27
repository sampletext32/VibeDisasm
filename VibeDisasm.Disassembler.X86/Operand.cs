namespace VibeDisasm.Disassembler.X86;

public abstract class Operand
{
    public OperandType Type { get; protected set; }
    public int Size { get; protected set; }
    public abstract override string ToString();
    public virtual TResult Accept<TResult>(IOperandVisitor<TResult> visitor) => visitor.VisitOperand(this);
}

