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
                Console.WriteLine("  {0,-10} {1,-8} {2,-8} {3,-10} {4,-8} {5}", 
                    "Type", "ID", "Lang ID", "Size", "RVA", "Data");
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
                    
                    Console.WriteLine("  {0,-10} {1,-8} {2,-8} 0x{3,-8:X} 0x{4,-6:X} {5}", 
                        resource.Type,
                        resource.Id,
                        resource.LanguageId,
                        resource.Size,
                        resource.RVA,
                        dataPreview);
                }
            }
        }
        else
        {
            Console.WriteLine("  No resource information found.");
        }
    }
}
