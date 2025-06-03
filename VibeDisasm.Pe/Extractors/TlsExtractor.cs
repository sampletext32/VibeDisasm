using VibeDisasm.Pe.Models;
using VibeDisasm.Pe.Raw;

namespace VibeDisasm.Pe.Extractors;

/// <summary>
/// Extracts Thread Local Storage (TLS) information from a PE file
/// </summary>
public static class TlsExtractor
{
    /// <summary>
    /// Extracts TLS information from a PE file
    /// </summary>
    /// <param name="rawPeFile">The raw PE file</param>
    /// <returns>TLS information, or null if the PE file has no TLS directory</returns>
    public static TlsInfo? Extract(RawPeFile rawPeFile)
    {
        if (rawPeFile is null)
        {
            throw new ArgumentNullException(nameof(rawPeFile));
        }

        // Check if the PE file has a TLS directory
        if (rawPeFile.OptionalHeader.DataDirectories.Length <= 9 ||
            rawPeFile.OptionalHeader.DataDirectories[9].VirtualAddress == 0)
        {
            return null;
        }

        var tlsDirectoryRva = rawPeFile.OptionalHeader.DataDirectories[9].VirtualAddress;
        var tlsDirectoryOffset = Util.RvaToOffset(rawPeFile, tlsDirectoryRva);

        // Determine if this is a 64-bit PE file
        var is64Bit = rawPeFile.OptionalHeader.Magic == 0x20B;

        var tlsInfo = new TlsInfo();

        // Read the TLS directory fields based on architecture
        if (is64Bit)
        {
            // 64-bit TLS directory structure
            tlsInfo.StartAddressOfRawData = BitConverter.ToUInt64(rawPeFile.RawData, (int)tlsDirectoryOffset);
            tlsInfo.EndAddressOfRawData = BitConverter.ToUInt64(rawPeFile.RawData, (int)tlsDirectoryOffset + 8);
            tlsInfo.AddressOfIndex = BitConverter.ToUInt64(rawPeFile.RawData, (int)tlsDirectoryOffset + 16);
            tlsInfo.AddressOfCallbacks = BitConverter.ToUInt64(rawPeFile.RawData, (int)tlsDirectoryOffset + 24);
            tlsInfo.SizeOfZeroFill = BitConverter.ToUInt32(rawPeFile.RawData, (int)tlsDirectoryOffset + 32);
            tlsInfo.Characteristics = BitConverter.ToUInt32(rawPeFile.RawData, (int)tlsDirectoryOffset + 36);
        }
        else
        {
            // 32-bit TLS directory structure
            tlsInfo.StartAddressOfRawData = BitConverter.ToUInt32(rawPeFile.RawData, (int)tlsDirectoryOffset);
            tlsInfo.EndAddressOfRawData = BitConverter.ToUInt32(rawPeFile.RawData, (int)tlsDirectoryOffset + 4);
            tlsInfo.AddressOfIndex = BitConverter.ToUInt32(rawPeFile.RawData, (int)tlsDirectoryOffset + 8);
            tlsInfo.AddressOfCallbacks = BitConverter.ToUInt32(rawPeFile.RawData, (int)tlsDirectoryOffset + 12);
            tlsInfo.SizeOfZeroFill = BitConverter.ToUInt32(rawPeFile.RawData, (int)tlsDirectoryOffset + 16);
            tlsInfo.Characteristics = BitConverter.ToUInt32(rawPeFile.RawData, (int)tlsDirectoryOffset + 20);
        }

        // Process TLS callbacks if they exist
        if (tlsInfo.AddressOfCallbacks != 0)
        {
            // Convert VA to RVA by subtracting the image base
            var callbackArrayRva = (uint)(tlsInfo.AddressOfCallbacks - rawPeFile.OptionalHeader.ImageBase);

            var callbackArrayOffset = Util.RvaToOffset(rawPeFile, callbackArrayRva);
            var callbackIndex = 0;

            // Read each callback until we find a null entry
            while (true)
            {
                ulong callbackVa;
                if (is64Bit)
                {
                    // 64-bit PE files use 8-byte pointers
                    if (callbackArrayOffset + (uint)(callbackIndex * 8) + 8 > rawPeFile.RawData.Length)
                    {
                        break;
                    }

                    callbackVa = BitConverter.ToUInt64(rawPeFile.RawData, (int)callbackArrayOffset + (callbackIndex * 8));
                    if (callbackVa == 0)
                    {
                        break;
                    }
                }
                else
                {
                    // 32-bit PE files use 4-byte pointers
                    if (callbackArrayOffset + (uint)(callbackIndex * 4) + 4 > rawPeFile.RawData.Length)
                    {
                        break;
                    }

                    callbackVa = BitConverter.ToUInt32(rawPeFile.RawData, (int)callbackArrayOffset + (callbackIndex * 4));
                    if (callbackVa == 0)
                    {
                        break;
                    }
                }

                // Convert the VA to an RVA by subtracting the image base
                var callbackRva = (uint)(callbackVa - rawPeFile.OptionalHeader.ImageBase);
                var callbackOffset = Util.RvaToOffset(rawPeFile, callbackRva);

                tlsInfo.Callbacks.Add(new TlsCallbackInfo
                {
                    Index = callbackIndex,
                    RelativeVirtualAddress = callbackRva,
                    FileOffset = callbackOffset
                });

                callbackIndex++;
            }
        }

        return tlsInfo;
    }
}
