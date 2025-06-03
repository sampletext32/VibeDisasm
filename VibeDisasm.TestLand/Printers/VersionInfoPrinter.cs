using System.Globalization;
using VibeDisasm.Pe.Extractors;
using VibeDisasm.Pe.Models;

namespace VibeDisasm.TestLand.Printers;

/// <summary>
/// Printer for version information
/// </summary>
public static class VersionInfoPrinter
{
    /// <summary>
    /// Prints version information to the console
    /// </summary>
    /// <param name="versionInfos">The version information to print</param>
    public static void Print(System.Collections.Generic.IEnumerable<VersionInfo> versionInfos)
    {
        Console.WriteLine("\r\n  Version Information:");

        foreach (var versionInfo in versionInfos)
        {
            // Get the language name using CultureInfo
            var languageName = "Unknown";
            var languageCode = $"0x{versionInfo.LanguageId:X4}";

            try
            {
                var culture = new CultureInfo((int)versionInfo.LanguageId);
                languageName = culture.EnglishName;
            }
            catch
            {
                // If CultureInfo fails, just use the numeric code
                languageName = $"Language {languageCode}";
            }

            // Print basic version info
            Console.WriteLine($"\r\n  Language: {languageCode} ({languageName})");
            Console.WriteLine($"  Code Page: 0x{versionInfo.CodePage:X4}");
            Console.WriteLine($"  File Version: {versionInfo.FileVersion}");
            Console.WriteLine($"  Product Version: {versionInfo.ProductVersion}");

            // Print file flags
            Console.WriteLine($"  File Flags: 0x{versionInfo.FileFlags:X8}");
            if (versionInfo.FileFlags != 0)
            {
                Console.Write("    ");
                if ((versionInfo.FileFlags & 0x00000001) != 0)
                {
                    Console.Write("DEBUG ");
                }

                if ((versionInfo.FileFlags & 0x00000002) != 0)
                {
                    Console.Write("PRERELEASE ");
                }

                if ((versionInfo.FileFlags & 0x00000004) != 0)
                {
                    Console.Write("PATCHED ");
                }

                if ((versionInfo.FileFlags & 0x00000008) != 0)
                {
                    Console.Write("PRIVATEBUILD ");
                }

                if ((versionInfo.FileFlags & 0x00000010) != 0)
                {
                    Console.Write("INFOINFERRED ");
                }

                if ((versionInfo.FileFlags & 0x00000020) != 0)
                {
                    Console.Write("SPECIALBUILD ");
                }

                Console.WriteLine();
            }

            // Print file OS
            Console.WriteLine($"  File OS: 0x{versionInfo.FileOS:X8}");
            Console.WriteLine($"    {GetFileOSString(versionInfo.FileOS)}");

            // Print file type
            Console.WriteLine($"  File Type: 0x{versionInfo.FileType:X8}");
            Console.WriteLine($"    {GetFileTypeString(versionInfo.FileType, versionInfo.FileSubtype)}");

            // Print string file info
            if (versionInfo.StringFileInfo.Count > 0)
            {
                Console.WriteLine("\r\n  String File Info:");
                Console.WriteLine("  {0,-20} {1}", "Key", "Value");
                Console.WriteLine("  " + new string('-', 60));

                foreach (var entry in versionInfo.StringFileInfo.OrderBy(e => e.Key))
                {
                    // Format the string value for display
                    var displayValue = entry.Value;
                    if (displayValue.Length > 50)
                    {
                        displayValue = displayValue.Substring(0, 47) + "...";
                    }

                    // Replace control characters for better display
                    displayValue = displayValue.Replace('\r', '⏎').Replace('\n', '⏎');

                    Console.WriteLine("  {0,-20} {1}", entry.Key, displayValue);
                }
            }
        }
    }

    /// <summary>
    /// Gets a string representation of the file OS
    /// </summary>
    /// <param name="fileOS">The file OS value</param>
    /// <returns>A string representation of the file OS</returns>
    private static string GetFileOSString(uint fileOS)
    {
        return fileOS switch
        {
            0x00000001 => "VOS_UNKNOWN",
            0x00000002 => "VOS_DOS",
            0x00000004 => "VOS_OS216",
            0x00000008 => "VOS_OS232",
            0x00000010 => "VOS_NT",
            0x00010000 => "VOS__BASE",
            0x00020000 => "VOS__WINDOWS16",
            0x00030000 => "VOS__PM16",
            0x00040000 => "VOS__PM32",
            0x00050000 => "VOS__WINDOWS32",
            0x00010001 => "VOS_DOS_WINDOWS16",
            0x00010004 => "VOS_DOS_WINDOWS32",
            0x00020002 => "VOS_OS216_PM16",
            0x00030003 => "VOS_OS232_PM32",
            0x00040004 => "VOS_NT_WINDOWS32",
            _ => $"Unknown OS (0x{fileOS:X8})"
        };
    }

    private static string GetFileTypeString(uint fileType, uint fileSubtype)
    {
        var typeString = fileType switch
        {
            0x00000000 => "VFT_UNKNOWN",
            0x00000001 => "VFT_APP",
            0x00000002 => "VFT_DLL",
            0x00000003 => "VFT_DRV",
            0x00000004 => "VFT_FONT",
            0x00000005 => "VFT_VXD",
            0x00000007 => "VFT_STATIC_LIB",
            _ => $"Unknown Type (0x{fileType:X8})"
        };

        // Add subtype information for certain file types
        if (fileType == 0x00000003) // VFT_DRV
        {
            var subtypeString = fileSubtype switch
            {
                0x00000000 => "VFT2_UNKNOWN",
                0x00000001 => "VFT2_DRV_PRINTER",
                0x00000002 => "VFT2_DRV_KEYBOARD",
                0x00000003 => "VFT2_DRV_LANGUAGE",
                0x00000004 => "VFT2_DRV_DISPLAY",
                0x00000005 => "VFT2_DRV_MOUSE",
                0x00000006 => "VFT2_DRV_NETWORK",
                0x00000007 => "VFT2_DRV_SYSTEM",
                0x00000008 => "VFT2_DRV_INSTALLABLE",
                0x00000009 => "VFT2_DRV_SOUND",
                0x0000000A => "VFT2_DRV_COMM",
                0x0000000B => "VFT2_DRV_INPUTMETHOD",
                0x0000000C => "VFT2_DRV_VERSIONED_PRINTER",
                _ => $"Unknown Driver Subtype (0x{fileSubtype:X8})"
            };
            return $"{typeString} ({subtypeString})";
        }
        else if (fileType == 0x00000004) // VFT_FONT
        {
            var subtypeString = fileSubtype switch
            {
                0x00000000 => "VFT2_UNKNOWN",
                0x00000001 => "VFT2_FONT_RASTER",
                0x00000002 => "VFT2_FONT_VECTOR",
                0x00000003 => "VFT2_FONT_TRUETYPE",
                _ => $"Unknown Font Subtype (0x{fileSubtype:X8})"
            };
            return $"{typeString} ({subtypeString})";
        }

        return typeString;
    }
}
