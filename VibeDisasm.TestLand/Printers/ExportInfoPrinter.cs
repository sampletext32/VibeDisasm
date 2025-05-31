using VibeDisasm.Pe.Extractors;

namespace VibeDisasm.TestLand.Printers;

/// <summary>
/// Printer for export information
/// </summary>
public class ExportInfoPrinter
{
    /// <summary>
    /// Prints export information to the console
    /// </summary>
    /// <param name="exportInfo">The export information to print</param>
    public static void Print(ExportInfo? exportInfo)
    {
        Console.WriteLine("\r\nExport Information:");
        if (exportInfo != null)
        {
            Console.WriteLine($"  Module Name: {exportInfo.Name}");
            Console.WriteLine($"  Ordinal Base: {exportInfo.OrdinalBase}");
            Console.WriteLine($"  Number of Functions: {exportInfo.Functions.Count}");

            if (exportInfo.Functions.Count > 0)
            {
                Console.WriteLine("\r\n  Exported Functions:");
                Console.WriteLine("  {0,-8} {1,-40} {2,-10} {3}", "Ordinal", "Name", "RVA", "Forwarded");
                Console.WriteLine("  " + new string('-', 70));

                foreach (var function in exportInfo.Functions)
                {
                    var forwardInfo = function.IsForwarded ? function.ForwardedName : "";
                    Console.WriteLine("  {0,-8} {1,-40} 0x{2:X8} {3}",
                        function.Ordinal,
                        function.Name.Length > 40 ? function.Name.Substring(0, 37) + "..." : function.Name,
                        function.RelativeVirtualAddress,
                        forwardInfo);
                }
            }
        }
        else
        {
            Console.WriteLine("  No export information found.");
        }
    }
}
