using VibeDisasm.Pe.Extractors;

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
        System.Console.WriteLine("\r\nResource Directory Information:");
        
        if (resourceDirectoryInfo != null)
        {
            System.Console.WriteLine($"  Resource Directory Found: Yes");
            System.Console.WriteLine($"  Characteristics: 0x{resourceDirectoryInfo.Characteristics:X8}");
            System.Console.WriteLine($"  Time/Date Stamp: 0x{resourceDirectoryInfo.TimeDateStamp:X8}");
            System.Console.WriteLine($"  Version: {resourceDirectoryInfo.MajorVersion}.{resourceDirectoryInfo.MinorVersion}");
            System.Console.WriteLine($"  Named Entries: {resourceDirectoryInfo.NumberOfNamedEntries}");
            System.Console.WriteLine($"  ID Entries: {resourceDirectoryInfo.NumberOfIdEntries}");
            System.Console.WriteLine($"  Total Entries: {resourceDirectoryInfo.NumberOfNamedEntries + resourceDirectoryInfo.NumberOfIdEntries}");
        }
        else
        {
            System.Console.WriteLine("  Resource Directory Found: No");
        }
    }
}
