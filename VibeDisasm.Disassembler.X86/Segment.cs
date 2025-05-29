namespace VibeDisasm.Disassembler.X86;

public enum Segment
{
    /// <summary> Represents the CS (Code Segment) register. </summary>
    Cs,

    /// <summary> Represents the DS (Data Segment) register. </summary>
    Ds,

    /// <summary> Represents the SS (Stack Segment) register. </summary>
    Ss,

    /// <summary> Represents the ES (Extra Segment) register. </summary>
    Es,

    /// <summary> Represents the FS segment register. </summary>
    Fs,

    /// <summary> Represents the GS segment register. </summary>
    Gs
}