using System.Globalization;
using VibeDisasm.Pe.Extractors;
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
    public static void Print(ResourceInfo? resourceInfo)
    {
        Console.WriteLine("\r\nResource Information:");
        if (resourceInfo != null)
        {
            Console.WriteLine($"  Resource Directory RVA: 0x{resourceInfo.DirectoryRVA:X8}");
            Console.WriteLine($"  Resource Directory Size: 0x{resourceInfo.DirectorySize:X8}");
            Console.WriteLine($"  Number of Resources: {resourceInfo.Resources.Count}");

            if (resourceInfo.Resources.Count > 0)
            {
                Console.WriteLine("\r\n  Resources:");
                Console.WriteLine("  {0,-10} {1,-8} {2,-10} {3,-10} {4,-8} {5,-10} {6}",
                    "Type", "ID", "Language", "Size", "RVA", "FileOffset", "Data");
                Console.WriteLine("  " + new string('-', 70));

                foreach (var resource in resourceInfo.Resources)
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
                    catch { /* Use default numeric value */ }

                    Console.WriteLine("  {0,-10} {1,-8} {2,-10} 0x{3,-8:X} 0x{4,-6:X} 0x{5,-8:X} {6}",
                        resource.Type,
                        resource.Id,
                        languageDisplay,
                        resource.Size,
                        resource.RVA,
                        resource.FileOffset,
                        dataPreview);
                }

                // String tables are printed separately via the PrintStringTables method
            }
        }
        else
        {
            Console.WriteLine("  No resource information found.");
        }
    }

    /// <summary>
    /// Prints string tables to the console
    /// </summary>
    /// <param name="stringTables">The string tables to print</param>
    public static void PrintStringTables(IEnumerable<StringTableInfo> stringTables)
    {
        Console.WriteLine("\r\n  String Tables:");

        foreach (var table in stringTables)
        {
            // Get the language name using CultureInfo
            var languageName = "Unknown";
            var languageCode = $"0x{table.LanguageId:X4}";

            try
            {
                var culture = new CultureInfo((int)table.LanguageId);
                languageName = culture.EnglishName;
            }
            catch
            {
                // If CultureInfo fails, just use the numeric code
                languageName = $"Language {languageCode}";
            }

            // Format the language ID in hexadecimal for display
            var languageHex = $"0x{table.LanguageId:X4}";
            Console.WriteLine($"\r\n  String Table ID: {table.Id}, Language: {languageHex} ({languageName}), FileOffset: 0x{table.FileOffset:X8}");
            Console.WriteLine("  {0,-6} {1,-12} {2}", "ID", "FileOffset", "String");
            Console.WriteLine("  " + new string('-', 60));

            foreach (var entry in table.Strings.OrderBy(s => s.Key))
            {
                // Format the string value for display
                var displayValue = entry.Value;
                if (displayValue.Length > 50)
                {
                    displayValue = displayValue.Substring(0, 47) + "...";
                }

                // Replace control characters for better display
                displayValue = displayValue.Replace('\r', '⏎').Replace('\n', '⏎');

                // Get the file offset for this string, if available
                var fileOffsetDisplay = "N/A";
                if (table.StringFileOffsets.TryGetValue(entry.Key, out var offset))
                {
                    fileOffsetDisplay = $"0x{offset:X8}";
                }

                Console.WriteLine("  {0,-6} {1,-12} {2}", entry.Key, fileOffsetDisplay, displayValue);
            }
        }
    }
}
