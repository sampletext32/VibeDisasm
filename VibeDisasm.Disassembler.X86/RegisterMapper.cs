namespace VibeDisasm.Disassembler.X86;

/// <summary>
/// Handles mapping between register indices and register enums
/// </summary>
public static class RegisterMapper
{
    /// <summary>
    /// Gets the register name based on the register index and size
    /// </summary>
    /// <param name="regIndex">The register index as RegisterIndex enum</param>
    /// <param name="size">The register size (16, 32 bits)</param>
    /// <returns>The register name</returns>
    public static string GetRegisterName(RegisterIndex regIndex, int size)
    {
        return size switch
        {
            16 => Constants.RegisterNames16[(int)regIndex],
            32 => Constants.RegisterNames32[(int)regIndex],
            _ => throw new InvalidOperationException($"Didn't expect {size} register size"),
        };
    }

    /// <summary>
    /// Gets the 8-bit register name based on the RegisterIndex8 enum value
    /// </summary>
    /// <param name="regIndex8">The register index as RegisterIndex8 enum</param>
    /// <returns>The 8-bit register name</returns>
    public static string GetRegisterName(RegisterIndex8 regIndex8)
    {
        return regIndex8.ToString().ToLower();
    }
}
