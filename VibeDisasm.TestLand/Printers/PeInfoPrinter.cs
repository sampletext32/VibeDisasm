using VibeDisasm.Pe.Extractors;
using VibeDisasm.Pe.Models;

namespace VibeDisasm.TestLand.Printers;

/// <summary>
/// Printer for PE file information
/// </summary>
public class PeInfoPrinter
{
    /// <summary>
    /// Prints PE file information to the console
    /// </summary>
    /// <param name="peInfo">The PE information to print</param>
    public static void Print(PeInfo peInfo, string fileName)
    {
        Console.WriteLine($"PE File: {fileName}");
        Console.WriteLine($"Architecture: {(peInfo.Is64Bit ? "64-bit" : "32-bit")}");
        Console.WriteLine($"Entry Point RVA: 0x{peInfo.EntryPointRva:X8}");
        Console.WriteLine($"Number of Sections: {peInfo.NumberOfSections}");
    }
}
