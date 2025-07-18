using VibeDisasm.Web.Models;

namespace VibeDisasm.Web.Analysis;

/// <summary>
/// Utility class to determine PE binary architecture
/// </summary>
public static class PeBitnessAnalyser
{
    public static ProgramArchitecture? Run(byte[] data)
    {
        if (data.Length < 0x40) // Ensure minimum for 0x3C + PE header
        {
            return null;
        }

        // Read PE header offset from 0x3C
        var peOffset = BitConverter.ToInt32(data, 0x3C);
        var magicOffset = peOffset + 0x18;

        if (magicOffset + 2 > data.Length)
        {
            return null;
        }

        var magic = BitConverter.ToUInt16(data, magicOffset);

        return magic switch
        {
            0x10b => ProgramArchitecture.X86,
            0x20b => ProgramArchitecture.X64,
            _ => null
        };
    }
}
