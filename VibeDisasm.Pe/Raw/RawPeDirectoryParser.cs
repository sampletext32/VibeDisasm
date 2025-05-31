using System.Text;
using VibeDisasm.Pe.Raw.Structures;

namespace VibeDisasm.Pe.Raw;

/// <summary>
/// Parser for raw PE directory structures
/// </summary>
public class RawPeDirectoryParser
{
    private readonly RawPeFile _rawPeFile;

    /// <summary>
    /// Initializes a new instance of the RawPeDirectoryParser class
    /// </summary>
    /// <param name="rawPeFile">The raw PE file</param>
    public RawPeDirectoryParser(RawPeFile rawPeFile)
    {
        _rawPeFile = rawPeFile ?? throw new ArgumentNullException(nameof(rawPeFile));
    }

    /// <summary>
    /// Parses the export directory if present
    /// </summary>
    /// <param name="reader">The binary reader</param>
    /// <returns>The parsed export directory, or null if not present</returns>
    public RawExportDirectory? ParseExportDirectory(BinaryReader reader)
    {
        // Check if export directory exists
        if (_rawPeFile.OptionalHeader.DataDirectories.Length <= 0 ||
            _rawPeFile.OptionalHeader.DataDirectories[0].VirtualAddress == 0)
        {
            return null;
        }

        // Get the RVA and convert to file offset
        var rva = _rawPeFile.OptionalHeader.DataDirectories[0].VirtualAddress;
        var offset = RvaToOffset(rva);

        // Seek to the export directory
        reader.BaseStream.Seek(offset, SeekOrigin.Begin);

        // Parse the export directory
        var exportDir = new RawExportDirectory
        {
            Characteristics = reader.ReadUInt32(),
            TimeDateStamp = reader.ReadUInt32(),
            MajorVersion = reader.ReadUInt16(),
            MinorVersion = reader.ReadUInt16(),
            NameRva = reader.ReadUInt32(),
            OrdinalBase = reader.ReadUInt32(),
            NumberOfFunctions = reader.ReadUInt32(),
            NumberOfNames = reader.ReadUInt32(),
            AddressOfFunctions = reader.ReadUInt32(),
            AddressOfNames = reader.ReadUInt32(),
            AddressOfNameOrdinals = reader.ReadUInt32()
        };

        return exportDir;
    }

    /// <summary>
    /// Parses the import descriptors if present
    /// </summary>
    /// <param name="reader">The binary reader</param>
    /// <returns>An array of import descriptors, or an empty array if not present</returns>
    public RawImportDescriptor[] ParseImportDescriptors(BinaryReader reader)
    {
        // Check if import directory exists
        if (_rawPeFile.OptionalHeader.DataDirectories.Length <= 1 ||
            _rawPeFile.OptionalHeader.DataDirectories[1].VirtualAddress == 0)
        {
            return Array.Empty<RawImportDescriptor>();
        }

        // Get the RVA and convert to file offset
        var rva = _rawPeFile.OptionalHeader.DataDirectories[1].VirtualAddress;
        var offset = RvaToOffset(rva);

        // Seek to the import directory
        reader.BaseStream.Seek(offset, SeekOrigin.Begin);

        // Parse the import descriptors
        var importDescriptors = new List<RawImportDescriptor>();

        while (true)
        {
            var importDesc = new RawImportDescriptor
            {
                OriginalFirstThunk = reader.ReadUInt32(),
                TimeDateStamp = reader.ReadUInt32(),
                ForwarderChain = reader.ReadUInt32(),
                Name = reader.ReadUInt32(),
                FirstThunk = reader.ReadUInt32()
            };

            // Check if this is the last import descriptor (all zeros)
            if (importDesc.OriginalFirstThunk == 0 && importDesc.Name == 0 && importDesc.FirstThunk == 0)
            {
                break;
            }

            importDescriptors.Add(importDesc);
        }

        return importDescriptors.ToArray();
    }

    /// <summary>
    /// Parses the resource directory if present
    /// </summary>
    /// <param name="reader">The binary reader</param>
    /// <returns>The parsed resource directory, or null if not present</returns>
    public RawResourceDirectory? ParseResourceDirectory(BinaryReader reader)
    {
        // Check if resource directory exists
        if (_rawPeFile.OptionalHeader.DataDirectories.Length <= 2 ||
            _rawPeFile.OptionalHeader.DataDirectories[2].VirtualAddress == 0)
        {
            return null;
        }

        // Get the RVA and convert to file offset
        var rva = _rawPeFile.OptionalHeader.DataDirectories[2].VirtualAddress;
        var offset = RvaToOffset(rva);

        // Seek to the resource directory
        reader.BaseStream.Seek(offset, SeekOrigin.Begin);

        // Parse the resource directory
        var resourceDir = new RawResourceDirectory
        {
            Characteristics = reader.ReadUInt32(),
            TimeDateStamp = reader.ReadUInt32(),
            MajorVersion = reader.ReadUInt16(),
            MinorVersion = reader.ReadUInt16(),
            NumberOfNamedEntries = reader.ReadUInt16(),
            NumberOfIdEntries = reader.ReadUInt16()
        };

        return resourceDir;
    }

    /// <summary>
    /// Parses the resource directory entries for a resource directory
    /// </summary>
    /// <param name="reader">The binary reader</param>
    /// <param name="resourceDir">The resource directory</param>
    /// <returns>An array of resource directory entries</returns>
    public RawResourceDirectoryEntry[] ParseResourceDirectoryEntries(BinaryReader reader, RawResourceDirectory resourceDir)
    {
        var totalEntries = resourceDir.NumberOfNamedEntries + resourceDir.NumberOfIdEntries;
        var entries = new RawResourceDirectoryEntry[totalEntries];

        for (var i = 0; i < totalEntries; i++)
        {
            entries[i] = new RawResourceDirectoryEntry
            {
                NameOrId = reader.ReadUInt32(),
                OffsetToData = reader.ReadUInt32()
            };
        }

        return entries;
    }

    /// <summary>
    /// Parses a resource data entry
    /// </summary>
    /// <param name="reader">The binary reader</param>
    /// <param name="offset">The offset to the resource data entry</param>
    /// <returns>The parsed resource data entry</returns>
    public RawResourceDataEntry ParseResourceDataEntry(BinaryReader reader, uint offset)
    {
        // Seek to the resource data entry
        reader.BaseStream.Seek(offset, SeekOrigin.Begin);

        // Parse the resource data entry
        var dataEntry = new RawResourceDataEntry
        {
            OffsetToData = reader.ReadUInt32(),
            Size = reader.ReadUInt32(),
            CodePage = reader.ReadUInt32(),
            Reserved = reader.ReadUInt32()
        };

        return dataEntry;
    }

    /// <summary>
    /// Parses the exception directory if present
    /// </summary>
    /// <param name="reader">The binary reader</param>
    /// <returns>An array of exception directory entries, or an empty array if not present</returns>
    public RawExceptionDirectory[] ParseExceptionDirectory(BinaryReader reader)
    {
        // Check if exception directory exists
        if (_rawPeFile.OptionalHeader.DataDirectories.Length <= 3 ||
            _rawPeFile.OptionalHeader.DataDirectories[3].VirtualAddress == 0)
        {
            return Array.Empty<RawExceptionDirectory>();
        }

        // Get the RVA and convert to file offset
        var rva = _rawPeFile.OptionalHeader.DataDirectories[3].VirtualAddress;
        var size = _rawPeFile.OptionalHeader.DataDirectories[3].Size;
        var offset = RvaToOffset(rva);

        // Seek to the exception directory
        reader.BaseStream.Seek(offset, SeekOrigin.Begin);

        // Calculate the number of entries
        var numEntries = (int)(size / 12); // Each entry is 12 bytes

        // Parse the exception directory entries
        var entries = new RawExceptionDirectory[numEntries];

        for (var i = 0; i < numEntries; i++)
        {
            entries[i] = new RawExceptionDirectory
            {
                BeginAddress = reader.ReadUInt32(),
                EndAddress = reader.ReadUInt32(),
                UnwindInfoAddress = reader.ReadUInt32()
            };
        }

        return entries;
    }

    /// <summary>
    /// Parses the security directory if present
    /// </summary>
    /// <param name="reader">The binary reader</param>
    /// <returns>The raw security data, or an empty array if not present</returns>
    public byte[] ParseSecurityDirectory(BinaryReader reader)
    {
        // Check if security directory exists
        if (_rawPeFile.OptionalHeader.DataDirectories.Length <= 4 ||
            _rawPeFile.OptionalHeader.DataDirectories[4].VirtualAddress == 0)
        {
            return Array.Empty<byte>();
        }

        // For security directory, VirtualAddress is actually a file offset, not an RVA
        var offset = _rawPeFile.OptionalHeader.DataDirectories[4].VirtualAddress;
        var size = _rawPeFile.OptionalHeader.DataDirectories[4].Size;

        // Seek to the security directory
        reader.BaseStream.Seek(offset, SeekOrigin.Begin);

        // Read the security data
        return reader.ReadBytes((int)size);
    }

    /// <summary>
    /// Parses the base relocation directory if present
    /// </summary>
    /// <param name="reader">The binary reader</param>
    /// <returns>An array of base relocation blocks, or an empty array if not present</returns>
    public RawBaseRelocation[] ParseBaseRelocationDirectory(BinaryReader reader)
    {
        // Check if base relocation directory exists
        if (_rawPeFile.OptionalHeader.DataDirectories.Length <= 5 ||
            _rawPeFile.OptionalHeader.DataDirectories[5].VirtualAddress == 0)
        {
            return Array.Empty<RawBaseRelocation>();
        }

        // Get the RVA and convert to file offset
        var rva = _rawPeFile.OptionalHeader.DataDirectories[5].VirtualAddress;
        var size = _rawPeFile.OptionalHeader.DataDirectories[5].Size;
        var offset = RvaToOffset(rva);

        // Seek to the base relocation directory
        reader.BaseStream.Seek(offset, SeekOrigin.Begin);

        // Parse the base relocation blocks
        var blocks = new List<RawBaseRelocation>();
        uint bytesRead = 0;

        while (bytesRead < size)
        {
            var block = new RawBaseRelocation
            {
                VirtualAddress = reader.ReadUInt32(),
                SizeOfBlock = reader.ReadUInt32()
            };

            if (block.SizeOfBlock == 0)
            {
                break;
            }

            blocks.Add(block);

            // Skip the relocation entries (we're only parsing the block headers)
            reader.BaseStream.Seek(block.SizeOfBlock - 8, SeekOrigin.Current);
            bytesRead += block.SizeOfBlock;
        }

        return blocks.ToArray();
    }

    /// <summary>
    /// Parses the debug directory if present
    /// </summary>
    /// <param name="reader">The binary reader</param>
    /// <returns>An array of debug directory entries, or an empty array if not present</returns>
    public RawDebugDirectory[] ParseDebugDirectory(BinaryReader reader)
    {
        // Check if debug directory exists
        if (_rawPeFile.OptionalHeader.DataDirectories.Length <= 6 ||
            _rawPeFile.OptionalHeader.DataDirectories[6].VirtualAddress == 0)
        {
            return Array.Empty<RawDebugDirectory>();
        }

        // Get the RVA and convert to file offset
        var rva = _rawPeFile.OptionalHeader.DataDirectories[6].VirtualAddress;
        var size = _rawPeFile.OptionalHeader.DataDirectories[6].Size;
        var offset = RvaToOffset(rva);

        // Seek to the debug directory
        reader.BaseStream.Seek(offset, SeekOrigin.Begin);

        // Calculate the number of entries
        var numEntries = (int)(size / 28); // Each entry is 28 bytes

        // Parse the debug directory entries
        var entries = new RawDebugDirectory[numEntries];

        for (var i = 0; i < numEntries; i++)
        {
            entries[i] = new RawDebugDirectory
            {
                Characteristics = reader.ReadUInt32(),
                TimeDateStamp = reader.ReadUInt32(),
                MajorVersion = reader.ReadUInt16(),
                MinorVersion = reader.ReadUInt16(),
                Type = reader.ReadUInt32(),
                SizeOfData = reader.ReadUInt32(),
                AddressOfRawData = reader.ReadUInt32(),
                PointerToRawData = reader.ReadUInt32()
            };
        }

        return entries;
    }

    /// <summary>
    /// Parses the TLS directory if present
    /// </summary>
    /// <param name="reader">The binary reader</param>
    /// <returns>The parsed TLS directory, or null if not present</returns>
    public RawTlsDirectory? ParseTlsDirectory(BinaryReader reader)
    {
        // Check if TLS directory exists
        if (_rawPeFile.OptionalHeader.DataDirectories.Length <= 9 ||
            _rawPeFile.OptionalHeader.DataDirectories[9].VirtualAddress == 0)
        {
            return null;
        }

        // Get the RVA and convert to file offset
        var rva = _rawPeFile.OptionalHeader.DataDirectories[9].VirtualAddress;
        var offset = RvaToOffset(rva);

        // Seek to the TLS directory
        reader.BaseStream.Seek(offset, SeekOrigin.Begin);

        // Parse the TLS directory
        var tlsDir = new RawTlsDirectory();

        if (_rawPeFile.IsPe32Plus)
        {
            // PE32+ (64-bit)
            tlsDir.StartAddressOfRawData = reader.ReadUInt64();
            tlsDir.EndAddressOfRawData = reader.ReadUInt64();
            tlsDir.AddressOfIndex = reader.ReadUInt64();
            tlsDir.AddressOfCallBacks = reader.ReadUInt64();
        }
        else
        {
            // PE32 (32-bit)
            tlsDir.StartAddressOfRawData = reader.ReadUInt32();
            tlsDir.EndAddressOfRawData = reader.ReadUInt32();
            tlsDir.AddressOfIndex = reader.ReadUInt32();
            tlsDir.AddressOfCallBacks = reader.ReadUInt32();
        }

        tlsDir.SizeOfZeroFill = reader.ReadUInt32();
        tlsDir.Characteristics = reader.ReadUInt32();

        return tlsDir;
    }

    /// <summary>
    /// Parses the load config directory if present
    /// </summary>
    /// <param name="reader">The binary reader</param>
    /// <returns>The parsed load config directory, or null if not present</returns>
    public RawLoadConfigDirectory? ParseLoadConfigDirectory(BinaryReader reader)
    {
        // Check if load config directory exists
        if (_rawPeFile.OptionalHeader.DataDirectories.Length <= 10 ||
            _rawPeFile.OptionalHeader.DataDirectories[10].VirtualAddress == 0)
        {
            return null;
        }

        // Get the RVA and convert to file offset
        var rva = _rawPeFile.OptionalHeader.DataDirectories[10].VirtualAddress;
        var offset = RvaToOffset(rva);

        // Seek to the load config directory
        reader.BaseStream.Seek(offset, SeekOrigin.Begin);

        // Parse the load config directory
        var loadConfigDir = new RawLoadConfigDirectory
        {
            Size = reader.ReadUInt32(),
            TimeDateStamp = reader.ReadUInt32(),
            MajorVersion = reader.ReadUInt16(),
            MinorVersion = reader.ReadUInt16(),
            GlobalFlagsClear = reader.ReadUInt32(),
            GlobalFlagsSet = reader.ReadUInt32(),
            CriticalSectionDefaultTimeout = reader.ReadUInt32()
        };

        if (_rawPeFile.IsPe32Plus)
        {
            // PE32+ (64-bit)
            loadConfigDir.DeCommitFreeBlockThreshold = reader.ReadUInt64();
            loadConfigDir.DeCommitTotalFreeThreshold = reader.ReadUInt64();
            loadConfigDir.LockPrefixTable = reader.ReadUInt64();
            loadConfigDir.MaximumAllocationSize = reader.ReadUInt64();
            loadConfigDir.VirtualMemoryThreshold = reader.ReadUInt64();
            loadConfigDir.ProcessAffinityMask = reader.ReadUInt64();
        }
        else
        {
            // PE32 (32-bit)
            loadConfigDir.DeCommitFreeBlockThreshold = reader.ReadUInt32();
            loadConfigDir.DeCommitTotalFreeThreshold = reader.ReadUInt32();
            loadConfigDir.LockPrefixTable = reader.ReadUInt32();
            loadConfigDir.MaximumAllocationSize = reader.ReadUInt32();
            loadConfigDir.VirtualMemoryThreshold = reader.ReadUInt32();
            loadConfigDir.ProcessAffinityMask = reader.ReadUInt32();
        }

        loadConfigDir.ProcessHeapFlags = reader.ReadUInt32();
        loadConfigDir.CSDVersion = reader.ReadUInt16();
        loadConfigDir.Reserved1 = reader.ReadUInt16();

        if (_rawPeFile.IsPe32Plus)
        {
            // PE32+ (64-bit)
            loadConfigDir.EditList = reader.ReadUInt64();
            loadConfigDir.SecurityCookie = reader.ReadUInt64();
        }
        else
        {
            // PE32 (32-bit)
            loadConfigDir.EditList = reader.ReadUInt32();
            loadConfigDir.SecurityCookie = reader.ReadUInt32();
        }

        return loadConfigDir;
    }

    /// <summary>
    /// Parses the bound import directory if present
    /// </summary>
    /// <param name="reader">The binary reader</param>
    /// <returns>An array of bound import descriptors, or an empty array if not present</returns>
    public RawBoundImportDescriptor[] ParseBoundImportDirectory(BinaryReader reader)
    {
        // Check if bound import directory exists
        if (_rawPeFile.OptionalHeader.DataDirectories.Length <= 11 ||
            _rawPeFile.OptionalHeader.DataDirectories[11].VirtualAddress == 0)
        {
            return Array.Empty<RawBoundImportDescriptor>();
        }

        // Get the RVA and convert to file offset (for bound imports, the RVA is actually a file offset)
        var offset = _rawPeFile.OptionalHeader.DataDirectories[11].VirtualAddress;

        // Seek to the bound import directory
        reader.BaseStream.Seek(offset, SeekOrigin.Begin);

        // Parse the bound import descriptors
        var descriptors = new List<RawBoundImportDescriptor>();

        while (true)
        {
            var descriptor = new RawBoundImportDescriptor
            {
                TimeDateStamp = reader.ReadUInt32(),
                OffsetModuleName = reader.ReadUInt16(),
                NumberOfModuleForwarderRefs = reader.ReadUInt16()
            };

            // Check if this is the last descriptor (all zeros)
            if (descriptor.TimeDateStamp == 0 && descriptor.OffsetModuleName == 0 && descriptor.NumberOfModuleForwarderRefs == 0)
            {
                break;
            }

            descriptors.Add(descriptor);

            // Skip forwarder refs if any
            if (descriptor.NumberOfModuleForwarderRefs > 0)
            {
                reader.BaseStream.Seek(descriptor.NumberOfModuleForwarderRefs * 8, SeekOrigin.Current);
            }
        }

        return descriptors.ToArray();
    }

    /// <summary>
    /// Parses the delay import directory if present
    /// </summary>
    /// <param name="reader">The binary reader</param>
    /// <returns>An array of delay load descriptors, or an empty array if not present</returns>
    public RawDelayLoadDescriptor[] ParseDelayImportDirectory(BinaryReader reader)
    {
        // Check if delay import directory exists
        if (_rawPeFile.OptionalHeader.DataDirectories.Length <= 13 ||
            _rawPeFile.OptionalHeader.DataDirectories[13].VirtualAddress == 0)
        {
            return Array.Empty<RawDelayLoadDescriptor>();
        }

        // Get the RVA and convert to file offset
        var rva = _rawPeFile.OptionalHeader.DataDirectories[13].VirtualAddress;
        var offset = RvaToOffset(rva);

        // Seek to the delay import directory
        reader.BaseStream.Seek(offset, SeekOrigin.Begin);

        // Parse the delay load descriptors
        var descriptors = new List<RawDelayLoadDescriptor>();

        while (true)
        {
            var descriptor = new RawDelayLoadDescriptor
            {
                Attributes = reader.ReadUInt32(),
                DllNameRva = reader.ReadUInt32(),
                ModuleHandleRva = reader.ReadUInt32(),
                ImportAddressTableRva = reader.ReadUInt32(),
                ImportNameTableRva = reader.ReadUInt32(),
                BoundImportAddressTableRva = reader.ReadUInt32(),
                UnloadInformationTableRva = reader.ReadUInt32(),
                TimeDateStamp = reader.ReadUInt32()
            };

            // Check if this is the last descriptor (all zeros)
            if (descriptor.DllNameRva == 0 && descriptor.ImportAddressTableRva == 0)
            {
                break;
            }

            descriptors.Add(descriptor);
        }

        return descriptors.ToArray();
    }

    /// <summary>
    /// Parses the CLR runtime header if present
    /// </summary>
    /// <param name="reader">The binary reader</param>
    /// <returns>The parsed CLR header, or null if not present</returns>
    public RawClrDirectory? ParseClrDirectory(BinaryReader reader)
    {
        // Check if CLR directory exists
        if (_rawPeFile.OptionalHeader.DataDirectories.Length <= 14 ||
            _rawPeFile.OptionalHeader.DataDirectories[14].VirtualAddress == 0)
        {
            return null;
        }

        // Get the RVA and convert to file offset
        var rva = _rawPeFile.OptionalHeader.DataDirectories[14].VirtualAddress;
        var offset = RvaToOffset(rva);

        // Seek to the CLR directory
        reader.BaseStream.Seek(offset, SeekOrigin.Begin);

        // Parse the CLR directory
        var clrDir = new RawClrDirectory
        {
            HeaderSize = reader.ReadUInt32(),
            MajorRuntimeVersion = reader.ReadUInt16(),
            MinorRuntimeVersion = reader.ReadUInt16(),
            MetadataRva = reader.ReadUInt32(),
            MetadataSize = reader.ReadUInt32(),
            Flags = reader.ReadUInt32(),
            EntryPointTokenOrRva = reader.ReadUInt32(),
            ResourcesRva = reader.ReadUInt32(),
            ResourcesSize = reader.ReadUInt32(),
            StrongNameSignatureRva = reader.ReadUInt32(),
            StrongNameSignatureSize = reader.ReadUInt32(),
            CodeManagerTableRva = reader.ReadUInt32(),
            CodeManagerTableSize = reader.ReadUInt32(),
            VTableFixupsRva = reader.ReadUInt32(),
            VTableFixupsSize = reader.ReadUInt32(),
            ExportAddressTableJumpsRva = reader.ReadUInt32(),
            ExportAddressTableJumpsSize = reader.ReadUInt32(),
            ManagedNativeHeaderRva = reader.ReadUInt32(),
            ManagedNativeHeaderSize = reader.ReadUInt32()
        };

        return clrDir;
    }

    /// <summary>
    /// Converts a Relative Virtual Address (RVA) to a file offset
    /// </summary>
    /// <param name="rva">The RVA to convert</param>
    /// <returns>The corresponding file offset</returns>
    private uint RvaToOffset(uint rva)
    {
        if (rva == 0)
        {
            return 0;
        }

        // Check if the RVA is within the headers
        if (rva < _rawPeFile.OptionalHeader.SizeOfHeaders)
        {
            return rva;
        }

        // Find the section that contains the RVA
        foreach (var section in _rawPeFile.SectionHeaders)
        {
            if (rva >= section.VirtualAddress && rva < section.VirtualAddress + section.VirtualSize)
            {
                // Calculate the offset within the section
                var offsetInSection = rva - section.VirtualAddress;

                // Make sure we don't exceed the raw data size
                if (offsetInSection < section.SizeOfRawData)
                {
                    return section.PointerToRawData + offsetInSection;
                }
            }
        }

        throw new ArgumentException($"RVA {rva:X8} is not within any section");
    }

    /// <summary>
    /// Reads a null-terminated ASCII string from the specified RVA
    /// </summary>
    /// <param name="reader">The binary reader</param>
    /// <param name="rva">The RVA of the string</param>
    /// <returns>The string read from the RVA</returns>
    public string ReadAsciiString(BinaryReader reader, uint rva)
    {
        if (rva == 0)
        {
            return string.Empty;
        }

        var offset = RvaToOffset(rva);
        reader.BaseStream.Seek(offset, SeekOrigin.Begin);

        var bytes = new List<byte>();
        byte b;

        while ((b = reader.ReadByte()) != 0)
        {
            bytes.Add(b);
        }

        return Encoding.ASCII.GetString(bytes.ToArray());
    }
}
