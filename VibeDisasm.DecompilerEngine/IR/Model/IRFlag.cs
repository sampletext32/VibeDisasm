namespace VibeDisasm.DecompilerEngine.IR.Model;

/// <summary>
/// Represents processor flags in the IR system.
/// </summary>
public enum IRFlag
{
    None,
    Zero,       // ZF - Set if result is zero
    Sign,       // SF - Set if result is negative
    Carry,      // CF - Set if operation produces a carry
    Overflow,   // OF - Set if signed overflow occurs
    Parity,     // PF - Set if result has even parity
    Auxiliary   // AF - Set if operation produces a carry from bit 3 to bit 4
}
