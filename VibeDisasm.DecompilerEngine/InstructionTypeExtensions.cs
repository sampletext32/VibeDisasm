using VibeDisasm.Disassembler.X86;

namespace VibeDisasm.DecompilerEngine;

public static class InstructionTypeExtensions
{
    public static bool IsRet(this InstructionType type)
    {
        return type is InstructionType.Ret or InstructionType.Retf;
    }

    public static bool IsUnconditionalJump(this InstructionType type)
    {
        return type is
            InstructionType.Jmp; // Jump unconditionally
    }

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
}