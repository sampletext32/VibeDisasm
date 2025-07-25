using VibeDisasm.Web.Models;

namespace VibeDisasm.Web.Analysis;

/// <summary>
/// Utility class to determine program kind
/// </summary>
public static class DetermineProgramKindAnalyser
{
    public static ProgramKind? Run(byte[] programData)
    {
        if (IsPEFile(programData))
        {
            return ProgramKind.PE;
        }

        return null;
    }

    private static bool IsPEFile(byte[] data)
    {
        if (data.Length < 64)
        {
            return false;
        }

        // Step 1: Check for 'MZ' at start
        if (data[0] != 0x4D || data[1] != 0x5A) // 'MZ'
        {
            return false;
        }

        // Step 2: Read PE header offset from 0x3C
        var peOffset = BitConverter.ToInt32(data, 0x3C);
        if (peOffset + 4 > data.Length)
        {
            return false;
        }

        // Step 3: Check for 'PE\0\0' at PE header offset
        return data[peOffset] == 0x50 &&      // 'P'
               data[peOffset + 1] == 0x45 &&  // 'E'
               data[peOffset + 2] == 0x00 &&
               data[peOffset + 3] == 0x00;
    }
}
