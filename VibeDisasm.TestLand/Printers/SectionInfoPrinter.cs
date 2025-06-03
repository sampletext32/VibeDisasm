using VibeDisasm.Pe.Extractors;
using VibeDisasm.Pe.Models;

namespace VibeDisasm.TestLand.Printers;

/// <summary>
/// Printer for section information
/// </summary>
public class SectionInfoPrinter
{
    /// <summary>
    /// Prints all sections to the console
    /// </summary>
    /// <param name="sections">The sections to print</param>
    public static void Print(SectionInfo[] sections)
    {
        Console.WriteLine("\r\nAll Sections:");
        Console.WriteLine("{0,-10} {1,-10} {2,-10} {3,-10} {4,-15}", "Name", "VirtAddr", "VirtSize", "RawAddr", "Properties");
        Console.WriteLine(new string('-', 65));

        foreach (var section in sections)
        {
            var properties = string.Empty;
            if (section.IsExecutable)
            {
                properties += "X";
            }

            if (section.IsReadable)
            {
                properties += "R";
            }

            if (section.IsWritable)
            {
                properties += "W";
            }

            if (section.ContainsCode)
            {
                properties += " Code";
            }

            if (section.ContainsInitializedData)
            {
                properties += " Data";
            }

            Console.WriteLine("{0,-10} 0x{1:X8} 0x{2:X8} 0x{3:X8} {4,-15}",
                section.Name,
                section.VirtualAddress,
                section.VirtualSize,
                section.RawDataAddress,
                properties.Trim());
        }
    }

    /// <summary>
    /// Prints a collection of sections with a title
    /// </summary>
    /// <param name="sections">The sections to print</param>
    /// <param name="title">The title for the section list</param>
    public static void PrintCollection(SectionInfo[] sections, string title)
    {
        Console.WriteLine($"\r\n{title}:");
        if (sections.Length > 0)
        {
            foreach (var section in sections)
            {
                Console.WriteLine($"  {section.Name} (0x{section.VirtualAddress:X8})");
            }
        }
        else
        {
            Console.WriteLine($"  No {title.ToLower()} found.");
        }
    }

    /// <summary>
    /// Prints detailed information about a single section
    /// </summary>
    /// <param name="section">The section to print</param>
    /// <param name="title">The title for the section details</param>
    public static void PrintDetails(SectionInfo? section, string title)
    {
        Console.WriteLine($"\r\n{title} Details:");
        if (section != null)
        {
            Console.WriteLine($"  Virtual Address: 0x{section.VirtualAddress:X8}");
            Console.WriteLine($"  Virtual Size: 0x{section.VirtualSize:X8}");
            Console.WriteLine($"  Raw Data Address: 0x{section.RawDataAddress:X8}");
            Console.WriteLine($"  Raw Data Size: 0x{section.RawDataSize:X8}");
            Console.WriteLine($"  Is Executable: {section.IsExecutable}");
            Console.WriteLine($"  Is Readable: {section.IsReadable}");
            Console.WriteLine($"  Is Writable: {section.IsWritable}");
            Console.WriteLine($"  Contains Code: {section.ContainsCode}");

            if (section.Data.Length > 0)
            {
                Console.WriteLine($"\r\nFirst 16 bytes of {title}:");
                Console.WriteLine(BitConverter.ToString(section.Data.Take(16).ToArray()).Replace("-", " "));
            }
        }
        else
        {
            Console.WriteLine($"  {title} not found.");
        }
    }
}
