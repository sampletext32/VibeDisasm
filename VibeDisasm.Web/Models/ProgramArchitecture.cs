namespace VibeDisasm.Web.Models;

/// <summary>
/// Processor architecture of a program
/// </summary>
public enum ProgramArchitecture
{
    Undefined,
    X86,
    X64,
    Arm32,
    Arm64,
    Mips32,
    Mips64,
    RiscV32,
    RiscV64
}

public static class ProgramArchitectureExtensions
{
    public static int GetPointerSize(this ProgramArchitecture architecture) => architecture switch
    {
        ProgramArchitecture.X86 => 4,
        ProgramArchitecture.X64 => 8,
        ProgramArchitecture.Arm32 => 4,
        ProgramArchitecture.Arm64 => 8,
        ProgramArchitecture.Mips32 => 4,
        ProgramArchitecture.Mips64 => 8,
        ProgramArchitecture.RiscV32 => 4,
        ProgramArchitecture.RiscV64 => 8,
        ProgramArchitecture.Undefined => throw new ArgumentOutOfRangeException(nameof(architecture), architecture, null),
        _ => 4
    };
}
