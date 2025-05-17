using System.Text;
using VibeDisasm.Pe.Raw;

namespace VibeDisasm.Pe.Extractors;

/// <summary>
/// Extracts delay-loaded import information from a PE file
/// </summary>
public static class DelayImportExtractor
{
    /// <summary>
    /// Extracts delay-loaded import information from a PE file
    /// </summary>
    /// <param name="rawPeFile">The raw PE file</param>
    /// <returns>Delay-loaded import information, or null if the PE file has no delay imports</returns>
    public static DelayImportInfo? Extract(RawPeFile rawPeFile)
    {
        if (rawPeFile is null)
        {
            throw new ArgumentNullException(nameof(rawPeFile));
        }

        // Check if the PE file has a delay import directory
        if (rawPeFile.OptionalHeader.DataDirectories.Length <= 13 || 
            rawPeFile.OptionalHeader.DataDirectories[13].VirtualAddress == 0)
        {
            return null;
        }

        uint delayImportDirectoryRva = rawPeFile.OptionalHeader.DataDirectories[13].VirtualAddress;
        uint delayImportDirectoryOffset = Util.RvaToOffset(rawPeFile, delayImportDirectoryRva);
        
        var delayImportInfo = new DelayImportInfo();
        bool is64Bit = rawPeFile.OptionalHeader.Magic == 0x20B;
        
        // Process each delay-load import descriptor
        uint currentOffset = delayImportDirectoryOffset;
        while (currentOffset + 32 <= rawPeFile.RawData.Length) // Each descriptor is 32 bytes
        {
            // Read the descriptor
            uint attributesField = BitConverter.ToUInt32(rawPeFile.RawData, (int)currentOffset);
            uint nameRva = BitConverter.ToUInt32(rawPeFile.RawData, (int)currentOffset + 4);
            uint moduleHandleRva = BitConverter.ToUInt32(rawPeFile.RawData, (int)currentOffset + 8);
            uint delayImportAddressTableRva = BitConverter.ToUInt32(rawPeFile.RawData, (int)currentOffset + 12);
            uint delayImportNameTableRva = BitConverter.ToUInt32(rawPeFile.RawData, (int)currentOffset + 16);
            uint boundDelayImportTableRva = BitConverter.ToUInt32(rawPeFile.RawData, (int)currentOffset + 20);
            uint unloadDelayImportTableRva = BitConverter.ToUInt32(rawPeFile.RawData, (int)currentOffset + 24);
            uint timestampField = BitConverter.ToUInt32(rawPeFile.RawData, (int)currentOffset + 28);
            
            // If all fields are 0, we've reached the end of the descriptor array
            if (nameRva == 0 && delayImportAddressTableRva == 0 && delayImportNameTableRva == 0)
                break;
                
            var moduleInfo = new DelayImportModuleInfo
            {
                Attributes = attributesField,
                ModuleHandleRva = moduleHandleRva,
                DelayImportAddressTableRva = delayImportAddressTableRva,
                DelayImportNameTableRva = delayImportNameTableRva,
                BoundDelayImportTableRva = boundDelayImportTableRva,
                UnloadDelayImportTableRva = unloadDelayImportTableRva,
                Timestamp = timestampField
            };
            
            // Read the module name
            if (nameRva > 0)
            {
                uint nameOffset = Util.RvaToOffset(rawPeFile, nameRva);
                moduleInfo.Name = ReadNullTerminatedString(rawPeFile.RawData, (int)nameOffset);
            }
            
            // Process the import functions if the address table exists
            if (delayImportAddressTableRva > 0 && delayImportNameTableRva > 0)
            {
                uint iatOffset = Util.RvaToOffset(rawPeFile, delayImportAddressTableRva);
                uint intOffset = Util.RvaToOffset(rawPeFile, delayImportNameTableRva);
                
                int functionIndex = 0;
                while (true)
                {
                    uint iatEntryOffset = iatOffset + (uint)(functionIndex * (is64Bit ? 8 : 4));
                    uint intEntryOffset = intOffset + (uint)(functionIndex * (is64Bit ? 8 : 4));
                    
                    // Make sure we don't read past the end of the file
                    if (iatEntryOffset + (is64Bit ? 8 : 4) > rawPeFile.RawData.Length ||
                        intEntryOffset + (is64Bit ? 8 : 4) > rawPeFile.RawData.Length)
                        break;
                        
                    // Read the import address table entry and import name table entry
                    ulong iatEntry = is64Bit
                        ? BitConverter.ToUInt64(rawPeFile.RawData, (int)iatEntryOffset)
                        : BitConverter.ToUInt32(rawPeFile.RawData, (int)iatEntryOffset);
                        
                    ulong intEntry = is64Bit
                        ? BitConverter.ToUInt64(rawPeFile.RawData, (int)intEntryOffset)
                        : BitConverter.ToUInt32(rawPeFile.RawData, (int)intEntryOffset);
                        
                    // If both entries are 0, we've reached the end of the table
                    if (iatEntry == 0 && intEntry == 0)
                        break;
                        
                    var functionInfo = new DelayImportFunctionInfo
                    {
                        ImportAddressTableRva = (uint)(delayImportAddressTableRva + (functionIndex * (is64Bit ? 8 : 4))),
                        ImportAddressTableOffset = iatEntryOffset
                    };
                    
                    // Check if the import is by ordinal
                    bool importByOrdinal = is64Bit
                        ? (intEntry & 0x8000000000000000) != 0
                        : (intEntry & 0x80000000) != 0;
                        
                    functionInfo.ImportByOrdinal = importByOrdinal;
                    
                    if (importByOrdinal)
                    {
                        // Import by ordinal
                        functionInfo.Ordinal = is64Bit
                            ? (ushort)(intEntry & 0xFFFF)
                            : (ushort)(intEntry & 0xFFFF);
                    }
                    else
                    {
                        // Import by name
                        uint nameRvaValue = is64Bit
                            ? (uint)(intEntry & 0x7FFFFFFFFFFFFFFF)
                            : (uint)(intEntry & 0x7FFFFFFF);
                            
                        if (nameRvaValue > 0)
                        {
                            uint nameOffset = Util.RvaToOffset(rawPeFile, nameRvaValue);
                            
                            // The first 2 bytes are the hint/ordinal
                            if (nameOffset + 2 <= rawPeFile.RawData.Length)
                            {
                                functionInfo.Ordinal = BitConverter.ToUInt16(rawPeFile.RawData, (int)nameOffset);
                                
                                // The name follows the hint
                                functionInfo.Name = ReadNullTerminatedString(rawPeFile.RawData, (int)nameOffset + 2);
                            }
                        }
                    }
                    
                    moduleInfo.Functions.Add(functionInfo);
                    functionIndex++;
                }
            }
            
            delayImportInfo.Modules.Add(moduleInfo);
            
            // Move to the next descriptor
            currentOffset += 32;
        }
        
        return delayImportInfo;
    }
    
    /// <summary>
    /// Reads a null-terminated ASCII string from a byte array
    /// </summary>
    /// <param name="data">The byte array</param>
    /// <param name="offset">The offset to start reading from</param>
    /// <returns>The string</returns>
    private static string ReadNullTerminatedString(byte[] data, int offset)
    {
        int length = 0;
        while (offset + length < data.Length && data[offset + length] != 0)
        {
            length++;
        }
        
        return Encoding.ASCII.GetString(data, offset, length);
    }
}
