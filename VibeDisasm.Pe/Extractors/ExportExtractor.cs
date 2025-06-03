using VibeDisasm.Pe.Models;
using VibeDisasm.Pe.Raw;

namespace VibeDisasm.Pe.Extractors;

/// <summary>
/// Extracts export information from a PE file
/// </summary>
public static class ExportExtractor
{
    /// <summary>
    /// Extracts export information from a PE file
    /// </summary>
    /// <param name="rawPeFile">The raw PE file</param>
    /// <returns>Export information, or null if the PE file has no exports</returns>
    public static ExportInfo? Extract(RawPeFile rawPeFile)
    {
        if (rawPeFile == null)
        {
            throw new ArgumentNullException(nameof(rawPeFile));
        }

        // Check if the PE file has an export directory
        if (rawPeFile.ExportDirectory == null)
        {
            return null;
        }

        var exportInfo = new ExportInfo
        {
            OrdinalBase = rawPeFile.ExportDirectory.OrdinalBase,
            TimeDateStamp = rawPeFile.ExportDirectory.TimeDateStamp,
            MajorVersion = rawPeFile.ExportDirectory.MajorVersion,
            MinorVersion = rawPeFile.ExportDirectory.MinorVersion
        };

        // Get the DLL name
        if (rawPeFile.ExportDirectory.NameRva != 0)
        {
            var nameOffset = Util.RvaToOffset(rawPeFile, rawPeFile.ExportDirectory.NameRva);
            exportInfo.Name = Util.ReadAsciiString(rawPeFile.RawData, nameOffset);
        }

        // Process exported functions
        if (rawPeFile.ExportDirectory.NumberOfFunctions > 0)
        {
            var functionsRva = rawPeFile.ExportDirectory.AddressOfFunctions;
            var namesRva = rawPeFile.ExportDirectory.AddressOfNames;
            var ordinalsRva = rawPeFile.ExportDirectory.AddressOfNameOrdinals;

            // Create a mapping of ordinals to names
            var ordinalToName = new Dictionary<uint, string>();
            for (uint i = 0; i < rawPeFile.ExportDirectory.NumberOfNames; i++)
            {
                var nameRvaOffset = Util.RvaToOffset(rawPeFile, namesRva + (i * 4));
                var nameRva = BitConverter.ToUInt32(rawPeFile.RawData, (int)nameRvaOffset);

                var ordinalOffset = Util.RvaToOffset(rawPeFile, ordinalsRva + (i * 2));
                var ordinal = BitConverter.ToUInt16(rawPeFile.RawData, (int)ordinalOffset);

                var nameOffset = Util.RvaToOffset(rawPeFile, nameRva);
                var name = Util.ReadAsciiString(rawPeFile.RawData, nameOffset);

                ordinalToName[ordinal] = name;
            }

            // Get the export directory RVA and size for checking forwarded exports
            uint exportDirRva = 0;
            uint exportDirSize = 0;
            if (rawPeFile.OptionalHeader.DataDirectories.Length > 0)
            {
                exportDirRva = rawPeFile.OptionalHeader.DataDirectories[0].VirtualAddress;
                exportDirSize = rawPeFile.OptionalHeader.DataDirectories[0].Size;
            }

            // Process each function
            for (uint i = 0; i < rawPeFile.ExportDirectory.NumberOfFunctions; i++)
            {
                var functionRvaOffset = Util.RvaToOffset(rawPeFile, functionsRva + (i * 4));
                var functionRva = BitConverter.ToUInt32(rawPeFile.RawData, (int)functionRvaOffset);

                // Skip functions with RVA = 0 (not exported)
                if (functionRva == 0)
                {
                    continue;
                }

                var function = new ExportFunctionInfo
                {
                    Ordinal = (ushort)(i + rawPeFile.ExportDirectory.OrdinalBase),
                    RelativeVirtualAddress = functionRva
                };

                // Set the function name if available
                if (ordinalToName.TryGetValue(i, out var name))
                {
                    function.Name = name;
                }

                // Check if this is a forwarded export
                if (exportDirRva <= functionRva && functionRva < exportDirRva + exportDirSize)
                {
                    function.IsForwarded = true;
                    var forwardOffset = Util.RvaToOffset(rawPeFile, functionRva);
                    function.ForwardedName = Util.ReadAsciiString(rawPeFile.RawData, forwardOffset);
                }

                exportInfo.Functions.Add(function);
            }
        }

        return exportInfo;
    }

}
