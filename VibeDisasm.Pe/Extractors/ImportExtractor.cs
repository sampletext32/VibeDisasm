using System.Text;
using VibeDisasm.Pe.Raw;

namespace VibeDisasm.Pe.Extractors;

/// <summary>
/// Extracts import information from a PE file
/// </summary>
public static class ImportExtractor
{
    /// <summary>
    /// Extracts import information from a PE file
    /// </summary>
    /// <param name="rawPeFile">The raw PE file</param>
    /// <returns>Import information, or null if the PE file has no imports</returns>
    public static ImportInfo? Extract(RawPeFile rawPeFile)
    {
        if (rawPeFile == null)
        {
            throw new ArgumentNullException(nameof(rawPeFile));
        }

        // Check if the PE file has import descriptors
        if (rawPeFile.ImportDescriptors == null || rawPeFile.ImportDescriptors.Length == 0)
        {
            return null;
        }

        var importInfo = new ImportInfo();

        // Process each import descriptor (each represents a DLL)
        foreach (var descriptor in rawPeFile.ImportDescriptors)
        {
            var moduleInfo = new ImportModuleInfo();

            // Get the module name
            if (descriptor.Name != 0)
            {
                uint nameOffset = Util.RvaToOffset(rawPeFile, descriptor.Name);
                moduleInfo.Name = Util.ReadAsciiString(rawPeFile.RawData, nameOffset);
            }

            // Process the imported functions
            uint thunkRva = descriptor.OriginalFirstThunk != 0 ? descriptor.OriginalFirstThunk : descriptor.FirstThunk;
            uint iatRva = descriptor.FirstThunk;

            if (thunkRva != 0)
            {
                uint thunkOffset = Util.RvaToOffset(rawPeFile, thunkRva);
                uint iatOffset = Util.RvaToOffset(rawPeFile, iatRva);
                int thunkSize = rawPeFile.IsPe32Plus ? 8 : 4;

                for (int i = 0; ; i++)
                {
                    // Read the thunk value
                    ulong thunkValue;
                    if (rawPeFile.IsPe32Plus)
                    {
                        if (thunkOffset + 8 > rawPeFile.RawData.Length)
                        {
                            break;
                        }
                        thunkValue = BitConverter.ToUInt64(rawPeFile.RawData, (int)thunkOffset + (i * thunkSize));
                    }
                    else
                    {
                        if (thunkOffset + 4 > rawPeFile.RawData.Length)
                        {
                            break;
                        }
                        thunkValue = BitConverter.ToUInt32(rawPeFile.RawData, (int)thunkOffset + (i * thunkSize));
                    }

                    // Check if this is the end of the thunk array
                    if (thunkValue == 0)
                    {
                        break;
                    }

                    var functionInfo = new ImportFunctionInfo
                    {
                        ImportAddressTableRva = iatRva + (uint)(i * thunkSize)
                    };

                    // Check if the import is by ordinal
                    bool isByOrdinal = rawPeFile.IsPe32Plus
                        ? (thunkValue & 0x8000000000000000UL) != 0
                        : (thunkValue & 0x80000000UL) != 0;
                    functionInfo.IsByOrdinal = isByOrdinal;

                    if (isByOrdinal)
                    {
                        // Import by ordinal
                        functionInfo.Ordinal = (ushort)(thunkValue & 0xFFFF);
                        functionInfo.Name = $"Ordinal_{functionInfo.Ordinal}";
                    }
                    else
                    {
                        // Import by name
                        uint hintNameRva = (uint)(thunkValue & (rawPeFile.IsPe32Plus ? 0x7FFFFFFFFFFFFFFFUL : 0x7FFFFFFFUL));
                        uint hintNameOffset = Util.RvaToOffset(rawPeFile, hintNameRva);

                        // Read the hint
                        functionInfo.Hint = BitConverter.ToUInt16(rawPeFile.RawData, (int)hintNameOffset);

                        // Read the name
                        functionInfo.Name = Util.ReadAsciiString(rawPeFile.RawData, hintNameOffset + 2);
                    }

                    moduleInfo.Functions.Add(functionInfo);
                }
            }

            importInfo.Modules.Add(moduleInfo);
        }

        return importInfo;
    }
}
