namespace VibeDisasm.Disassembler.X86;

public static class InstructionTypeExtensions
{
    /// <summary>
    /// Determines if the instruction type is a return instruction
    /// </summary>
    public static bool IsRet(this InstructionType type)
    {
        return type is InstructionType.Ret or InstructionType.Retf;
    }

    /// <summary>
    /// Determines if the instruction type is an unconditional jump instruction
    /// </summary>
    public static bool IsUnconditionalJump(this InstructionType type)
    {
        return type is
            InstructionType.Jmp; // Jump unconditionally
    }

    /// <summary>
    /// Determines if the instruction type is a conditional jump instruction
    /// </summary>
    public static bool IsConditionalJump(this InstructionType type)
    {
        return type is
            InstructionType.Jg or         // Jump if greater
            InstructionType.Jge or        // Jump if greater or equal
            InstructionType.Jl or         // Jump if less
            InstructionType.Jle or        // Jump if less or equal
            InstructionType.Ja or         // Jump if above (unsigned)
            InstructionType.Jae or        // Jump if above or equal (unsigned)
            InstructionType.Jb or         // Jump if below (unsigned)
            InstructionType.Jbe or        // Jump if below or equal (unsigned)
            InstructionType.Jz or         // Jump if zero
            InstructionType.Jnz or        // Jump if not zero
            InstructionType.Jo or         // Jump if overflow
            InstructionType.Jno or        // Jump if not overflow
            InstructionType.Js or         // Jump if sign
            InstructionType.Jns or        // Jump if not sign
            InstructionType.Jp or         // Jump if parity (even)
            InstructionType.Jnp;        // Jump if not parity (odd)
    }
    
    /// <summary>
    /// Determines if the instruction type is a call instruction
    /// </summary>
    public static bool IsCall(this InstructionType type)
    {
        return type is InstructionType.Call;
    }
    
    /// <summary>
    /// Gets a description of the jump condition for a conditional jump instruction
    /// </summary>
    public static string GetJumpConditionDescription(this InstructionType type)
    {
        return type switch
        {
            InstructionType.Jz => "ZF=1 (Equal)",
            InstructionType.Jnz => "ZF=0 (Not Equal)",
            InstructionType.Jl => "SF≠OF (Less)",
            InstructionType.Jle => "ZF=1 or SF≠OF (Less or Equal)",
            InstructionType.Jg => "ZF=0 and SF=OF (Greater)",
            InstructionType.Jge => "SF=OF (Greater or Equal)",
            InstructionType.Jb => "CF=1 (Below/Carry)",
            InstructionType.Jbe => "CF=1 or ZF=1 (Below or Equal)",
            InstructionType.Ja => "CF=0 and ZF=0 (Above)",
            InstructionType.Jae => "CF=0 (Above or Equal)",
            InstructionType.Js => "SF=1 (Sign)",
            InstructionType.Jns => "SF=0 (Not Sign)",
            InstructionType.Jo => "OF=1 (Overflow)",
            InstructionType.Jno => "OF=0 (Not Overflow)",
            InstructionType.Jp => "PF=1 (Parity)",
            InstructionType.Jnp => "PF=0 (Not Parity)",
            _ => "Unknown Condition"
        };
    }
}