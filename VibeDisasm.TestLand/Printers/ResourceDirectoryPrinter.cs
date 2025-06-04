using VibeDisasm.Pe.Models;

namespace VibeDisasm.TestLand.Printers;

/// <summary>
/// Printer for resource directory information
/// </summary>
public class ResourceDirectoryPrinter
{
    /// <summary>
    /// Prints resource directory information to the console
    /// </summary>
    /// <param name="resourceDirectoryInfo">The resource directory information to print</param>
    public static void Print(ResourceDirectoryInfo? resourceDirectoryInfo)
    {
        Console.WriteLine("\r\nResource Directory Information:");

        if (resourceDirectoryInfo != null)
        {
            Console.WriteLine($"  Resource Directory Found: Yes");
            Console.WriteLine($"  Characteristics: 0x{resourceDirectoryInfo.Characteristics:X8}");
            Console.WriteLine($"  Time/Date Stamp: 0x{resourceDirectoryInfo.TimeDateStamp:X8}");
            Console.WriteLine($"  Version: {resourceDirectoryInfo.MajorVersion}.{resourceDirectoryInfo.MinorVersion}");
            Console.WriteLine($"  Named Entries: {resourceDirectoryInfo.NumberOfNamedEntries}");
            Console.WriteLine($"  ID Entries: {resourceDirectoryInfo.NumberOfIdEntries}");
            Console.WriteLine($"  Total Entries: {resourceDirectoryInfo.NumberOfNamedEntries + resourceDirectoryInfo.NumberOfIdEntries}");
        }
        else
        {
            Console.WriteLine("  Resource Directory Found: No");
        }
    }
}
