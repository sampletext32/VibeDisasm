using System.Globalization;
using VibeDisasm.Pe.Models;

namespace VibeDisasm.TestLand.Printers;

/// <summary>
/// Printer for resource information
/// </summary>
public class ResourceInfoPrinter
{
    /// <summary>
    /// Prints resource information to the console
    /// </summary>
    /// <param name="resourceInfo">The resource information to print</param>
    public static void Print(PeResources? resourceInfo)
    {
        Console.WriteLine("\r\nResource Information:");
        if (resourceInfo == null)
        {
            Console.WriteLine("  No resource information found.");
            return;
        }

        var resources = resourceInfo.FlattenResources().ToList();

        Console.WriteLine($"  Resource Directory RVA: 0x{resourceInfo.DirectoryRva:X8}");
        Console.WriteLine($"  Resource Directory Size: 0x{resourceInfo.DirectorySize:X8}");
        Console.WriteLine($"  Number of Resources: {resources.Count}");

        if (resources.Count <= 0)
        {
            return;
        }

        Console.WriteLine("\r\n  Resources:");
        Console.WriteLine(
            "  {0,-10} {1,-8} {2,-10} {3,-10} {4,-8} {5,-10} {6}",
            "Type",
            "ID",
            "Language",
            "Size",
            "RVA",
            "FileOffset",
            "Data"
        );
        Console.WriteLine("  " + new string('-', 70));

        foreach (var resource in resources)
        {
            var dataPreview = "";
            if (resource.Size > 0)
            {
                // Show a preview of the data if available
                if (resource.Type == ResourceType.StringTable ||
                    resource.Type == ResourceType.RcData ||
                    resource.Type == ResourceType.HTML ||
                    resource.Type == ResourceType.Manifest)
                {
                    dataPreview = "[String data]";
                }
                else
                {
                    // Show as hex for binary resources
                    dataPreview = "[Binary data]";
                }
            }
            else
            {
                dataPreview = "[No data loaded]";
            }

            // Try to get language name for display
            var languageDisplay = resource.LanguageId.ToString();
            try
            {
                var culture = new CultureInfo((int)resource.LanguageId);
                languageDisplay = culture.Name.Split('-')[0]; // Just use the language code, not the region
            }
            catch
            {
                /* Use default numeric value */
            }

            Console.WriteLine(
                "  {0,-10} {1,-8} {2,-10} 0x{3,-8:X} 0x{4,-6:X} 0x{5,-8:X} {6}",
                resource.Type,
                resource.NameId,
                languageDisplay,
                resource.Size,
                resource.RVA,
                resource.FileOffset,
                dataPreview
            );
        }
    }
}
