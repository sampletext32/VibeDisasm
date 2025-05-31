namespace VibeDisasm.DecompilerEngine.IR.Expressions;

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
