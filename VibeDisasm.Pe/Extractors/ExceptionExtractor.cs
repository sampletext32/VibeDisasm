using VibeDisasm.Pe.Raw;

namespace VibeDisasm.Pe.Extractors;

/// <summary>
/// Extracts exception handling information from a PE file
/// </summary>
public static class ExceptionExtractor
{
    /// <summary>
    /// Extracts exception handling information from a PE file
    /// </summary>
    /// <param name="rawPeFile">The raw PE file</param>
    /// <returns>Exception handling information, or null if the PE file has no exception directory</returns>
    public static ExceptionInfo? Extract(RawPeFile rawPeFile)
    {
        if (rawPeFile is null)
        {
            throw new ArgumentNullException(nameof(rawPeFile));
        }

        // Check if the PE file has an exception directory
        if (rawPeFile.OptionalHeader.DataDirectories.Length <= 3 || 
            rawPeFile.OptionalHeader.DataDirectories[3].VirtualAddress == 0)
        {
            return null;
        }

        uint exceptionDirectoryRva = rawPeFile.OptionalHeader.DataDirectories[3].VirtualAddress;
        uint exceptionDirectorySize = rawPeFile.OptionalHeader.DataDirectories[3].Size;
        uint exceptionDirectoryOffset = Util.RvaToOffset(rawPeFile, exceptionDirectoryRva);
        
        var exceptionInfo = new ExceptionInfo();
        
        // Exception directory contains an array of runtime function entries
        // Each entry is 12 bytes (3 DWORDs)
        int entryCount = (int)(exceptionDirectorySize / 12);
        
        for (int i = 0; i < entryCount; i++)
        {
            uint entryOffset = exceptionDirectoryOffset + (uint)(i * 12);
            
            // Make sure we don't read past the end of the file
            if (entryOffset + 12 > rawPeFile.RawData.Length)
                break;
                
            // Read the function start RVA (first DWORD)
            uint beginAddress = BitConverter.ToUInt32(rawPeFile.RawData, (int)entryOffset);
            
            // Read the function end RVA (second DWORD)
            uint endAddress = BitConverter.ToUInt32(rawPeFile.RawData, (int)entryOffset + 4);
            
            // Read the unwind info RVA (third DWORD)
            uint unwindInfoAddress = BitConverter.ToUInt32(rawPeFile.RawData, (int)entryOffset + 8);
            
            // Convert RVAs to file offsets
            uint beginAddressOffset = Util.RvaToOffset(rawPeFile, beginAddress);
            uint unwindInfoOffset = Util.RvaToOffset(rawPeFile, unwindInfoAddress);
            
            exceptionInfo.Functions.Add(new RuntimeFunctionInfo
            {
                Index = i,
                BeginAddress = beginAddress,
                EndAddress = endAddress,
                UnwindInfoAddress = unwindInfoAddress,
                BeginAddressFileOffset = beginAddressOffset,
                UnwindInfoFileOffset = unwindInfoOffset
            });
        }
        
        return exceptionInfo;
    }
}
