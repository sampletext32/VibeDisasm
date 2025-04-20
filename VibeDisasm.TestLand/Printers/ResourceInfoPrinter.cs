using System;
using System.Globalization;
using System.Text;
using VibeDisasm.Pe.Extractors;

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
                Console.WriteLine("  {0,-10} {1,-8} {2,-10} {3,-10} {4,-8} {5}", 
                    "Type", "ID", "Language", "Size", "RVA", "Data");
                Console.WriteLine("  " + new string('-', 70));
                
                foreach (var resource in resourceInfo.Resources)
                {
                    string dataPreview = "";
                    if (resource.Data.Length > 0)
                    {
                        // Show a preview of the data if available
                        if (resource.Type == ResourceType.StringTable || 
                            resource.Type == ResourceType.RcData ||
                            resource.Type == ResourceType.HTML ||
                            resource.Type == ResourceType.Manifest)
                        {
                            // Try to show as text for text-based resources
                            try
                            {
                                string text = System.Text.Encoding.UTF8.GetString(resource.Data);
                                if (text.Length > 20)
                                {
                                    text = text.Substring(0, 17) + "...";
                                }
                                dataPreview = text.Replace('\r', ' ').Replace('\n', ' ');
                            }
                            catch
                            {
                                dataPreview = "[Binary data]";
                            }
                        }
                        else
                        {
                            // Show as hex for binary resources
                            int previewLength = Math.Min(8, resource.Data.Length);
                            dataPreview = BitConverter.ToString(resource.Data, 0, previewLength).Replace("-", " ");
                            if (resource.Data.Length > 8)
                            {
                                dataPreview += "...";
                            }
                        }
                    }
                    else
                    {
                        dataPreview = "[No data loaded]";
                    }
                    
                    // Try to get language name for display
                    string languageDisplay = resource.LanguageId.ToString();
                    try
                    {
                        var culture = new CultureInfo((int)resource.LanguageId);
                        languageDisplay = culture.Name.Split('-')[0]; // Just use the language code, not the region
                    }
                    catch { /* Use default numeric value */ }
                    
                    Console.WriteLine("  {0,-10} {1,-8} {2,-10} 0x{3,-8:X} 0x{4,-6:X} {5}", 
                        resource.Type,
                        resource.Id,
                        languageDisplay,
                        resource.Size,
                        resource.RVA,
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
            string languageName = "Unknown";
            string languageCode = $"0x{table.LanguageId:X4}";
            
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
            string languageHex = $"0x{table.LanguageId:X4}";
            Console.WriteLine($"\r\n  String Table ID: {table.Id}, Language: {languageHex} ({languageName})");
            Console.WriteLine("  {0,-6} {1}", "ID", "String");
            Console.WriteLine("  " + new string('-', 60));
            
            foreach (var entry in table.Strings.OrderBy(s => s.Key))
            {
                // Format the string value for display
                string displayValue = entry.Value;
                if (displayValue.Length > 50)
                {
                    displayValue = displayValue.Substring(0, 47) + "...";
                }
                
                // Replace control characters for better display
                displayValue = displayValue.Replace('\r', '⏎').Replace('\n', '⏎');
                
                Console.WriteLine("  {0,-6} {1}", entry.Key, displayValue);
            }
        }
    }
}
