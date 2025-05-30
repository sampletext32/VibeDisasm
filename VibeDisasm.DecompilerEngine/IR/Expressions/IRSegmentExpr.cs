namespace VibeDisasm.DecompilerEngine.IR.Expressions;

/// <summary>
/// Represents a register operand in IR.
/// Example: eax -> IRRegister("eax")
/// </summary>
public sealed class IRSegmentExpr : IRExpression
{
    public IRSegment Segment { get; init; }
    
    public override List<IRExpression> SubExpressions => [];

    public IRSegmentExpr(IRSegment segment)
    {
        Segment = segment;
    }

    public override string ToString() => $"{Segment}";
}

public enum IRSegment
{
    /// <summary> Represents the CS (Code Segment) register. </summary>
    CS,

    /// <summary> Represents the DS (Data Segment) register. </summary>
    DS,

    /// <summary> Represents the SS (Stack Segment) register. </summary>
    SS,

    /// <summary> Represents the ES (Extra Segment) register. </summary>
    ES,

    /// <summary> Represents the FS segment register. </summary>
    FS,

    /// <summary> Represents the GS segment register. </summary>
    GS
}