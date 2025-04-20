using System.Text;
using VibeDisasm.Pe.Raw;

namespace VibeDisasm.Pe.Extractors;

/// <summary>
/// Extracts export information from a PE file
/// </summary>
public class ExportExtractor : IExtractor<ExportInfo?>
{
    /// <summary>
    /// Extracts export information from a PE file
    /// </summary>
    /// <param name="rawPeFile">The raw PE file</param>
    /// <returns>Export information, or null if the PE file has no exports</returns>
    public ExportInfo? Extract(RawPeFile rawPeFile)
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
            uint nameOffset = RvaToOffset(rawPeFile, rawPeFile.ExportDirectory.NameRva);
            exportInfo.Name = ReadAsciiString(rawPeFile.RawData, nameOffset);
        }

        // Process exported functions
        if (rawPeFile.ExportDirectory.NumberOfFunctions > 0)
        {
            uint functionsRva = rawPeFile.ExportDirectory.AddressOfFunctions;
            uint namesRva = rawPeFile.ExportDirectory.AddressOfNames;
            uint ordinalsRva = rawPeFile.ExportDirectory.AddressOfNameOrdinals;

            // Create a mapping of ordinals to names
            var ordinalToName = new Dictionary<uint, string>();
            for (uint i = 0; i < rawPeFile.ExportDirectory.NumberOfNames; i++)
            {
                uint nameRvaOffset = RvaToOffset(rawPeFile, namesRva + (i * 4));
                uint nameRva = BitConverter.ToUInt32(rawPeFile.RawData, (int)nameRvaOffset);

                uint ordinalOffset = RvaToOffset(rawPeFile, ordinalsRva + (i * 2));
                ushort ordinal = BitConverter.ToUInt16(rawPeFile.RawData, (int)ordinalOffset);

                uint nameOffset = RvaToOffset(rawPeFile, nameRva);
                string name = ReadAsciiString(rawPeFile.RawData, nameOffset);

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
                uint functionRvaOffset = RvaToOffset(rawPeFile, functionsRva + (i * 4));
                uint functionRva = BitConverter.ToUInt32(rawPeFile.RawData, (int)functionRvaOffset);

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
                if (ordinalToName.TryGetValue(i, out string? name))
                {
                    function.Name = name;
                }

                // Check if this is a forwarded export
                if (exportDirRva <= functionRva && functionRva < exportDirRva + exportDirSize)
                {
                    function.IsForwarded = true;
                    uint forwardOffset = RvaToOffset(rawPeFile, functionRva);
                    function.ForwardedName = ReadAsciiString(rawPeFile.RawData, forwardOffset);
                }

                exportInfo.Functions.Add(function);
            }
        }

        return exportInfo;
    }

    /// <summary>
    /// Converts a Relative Virtual Address (RVA) to a file offset
    /// </summary>
    /// <param name="rawPeFile">The raw PE file</param>
    /// <param name="rva">The RVA to convert</param>
    /// <returns>The corresponding file offset</returns>
    private uint RvaToOffset(RawPeFile rawPeFile, uint rva)
    {
        // Find the section containing the RVA
        foreach (var section in rawPeFile.SectionHeaders)
        {
            uint sectionStart = section.VirtualAddress;
            uint sectionEnd = sectionStart + Math.Max(section.VirtualSize, section.SizeOfRawData);

            if (rva >= sectionStart && rva < sectionEnd)
            {
                // Calculate the offset within the section
                uint offset = rva - sectionStart + section.PointerToRawData;
                return offset;
            }
        }

        // If the RVA is not in any section, it might be in the header
        if (rva < rawPeFile.OptionalHeader.SizeOfHeaders)
        {
            return rva;
        }

        throw new ArgumentException($"Invalid RVA: 0x{rva:X8}");
    }

    /// <summary>
    /// Reads a null-terminated ASCII string from the specified offset
    /// </summary>
    /// <param name="data">The raw data</param>
    /// <param name="offset">The offset of the string</param>
    /// <returns>The string read from the offset</returns>
    private string ReadAsciiString(byte[] data, uint offset)
    {
        int length = 0;
        while (offset + length < data.Length && data[offset + length] != 0)
        {
            length++;
        }

        return Encoding.ASCII.GetString(data, (int)offset, length);
    }
}
