using VibeDisasm.Pe.Extractors;

namespace VibeDisasm.TestLand.Printers;

/// <summary>
/// Printer for import information
/// </summary>
public class ImportInfoPrinter
{
    /// <summary>
    /// Prints import information to the console
    /// </summary>
    /// <param name="importInfo">The import information to print</param>
    public static void Print(ImportInfo? importInfo)
    {
        Console.WriteLine("\r\nImport Information:");
        if (importInfo != null)
        {
            Console.WriteLine($"  Number of Imported Modules: {importInfo.Modules.Count}");
            
            if (importInfo.Modules.Count > 0)
            {
                Console.WriteLine("\r\n  Imported Modules:");
                
                foreach (var module in importInfo.Modules)
                {
                    Console.WriteLine($"  {module.Name} ({module.Functions.Count} functions)");
                    
                    if (module.Functions.Count > 0)
                    {
                        Console.WriteLine("    {0,-8} {1,-30} {2}", "Hint/Ord", "Name", "IAT RVA");
                        Console.WriteLine("    " + new string('-', 50));
                        
                        foreach (var function in module.Functions)
                        {
                            string hintOrd = function.IsByOrdinal ? $"Ord:{function.Ordinal}" : $"Hint:{function.Hint}";
                            Console.WriteLine("    {0,-8} {1,-30} 0x{2:X8}", 
                                hintOrd, 
                                function.Name.Length > 30 ? function.Name.Substring(0, 27) + "..." : function.Name, 
                                function.ImportAddressTableRva);
                        }
                    }
                    
                    Console.WriteLine();
                }
            }
        }
        else
        {
            Console.WriteLine("  No import information found.");
        }
    }
}
