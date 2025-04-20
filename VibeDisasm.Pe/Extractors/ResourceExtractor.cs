using System.Text;
using VibeDisasm.Pe.Raw;
using VibeDisasm.Pe.Raw.Structures;

namespace VibeDisasm.Pe.Extractors;

/// <summary>
/// Extracts resources from a PE file
/// </summary>
public class ResourceExtractor : IExtractor<ResourceInfo?>
{
    private readonly bool _includeData;
    private uint _resourceDirectoryRva;
    
    /// <summary>
    /// Initializes a new instance of the <see cref="ResourceExtractor"/> class
    /// </summary>
    /// <param name="includeData">Whether to include the raw resource data in the extraction</param>
    public ResourceExtractor(bool includeData = false)
    {
        _includeData = includeData;
    }
    
    /// <summary>
    /// Extracts resource information from a PE file
    /// </summary>
    /// <param name="rawPeFile">The raw PE file</param>
    /// <returns>Resource information, or null if the PE file has no resources</returns>
    public ResourceInfo? Extract(RawPeFile rawPeFile)
    {
        if (rawPeFile == null)
        {
            throw new ArgumentNullException(nameof(rawPeFile));
        }
        
        // Check if the PE file has a resource directory
        if (rawPeFile.OptionalHeader.DataDirectories.Length <= 2 ||
            rawPeFile.OptionalHeader.DataDirectories[2].VirtualAddress == 0)
        {
            return null;
        }
        
        // Get the resource directory RVA and size
        _resourceDirectoryRva = rawPeFile.OptionalHeader.DataDirectories[2].VirtualAddress;
        uint resourceDirectorySize = rawPeFile.OptionalHeader.DataDirectories[2].Size;
        
        var resourceInfo = new ResourceInfo
        {
            DirectoryRVA = _resourceDirectoryRva,
            DirectorySize = resourceDirectorySize
        };
        
        // Process the resource directory
        if (rawPeFile.ResourceDirectory != null)
        {
            try
            {
                using var stream = new MemoryStream(rawPeFile.RawData);
                using var reader = new BinaryReader(stream);
                
                // Get to the root resource directory
                uint rootOffset = RvaToOffset(rawPeFile, _resourceDirectoryRva);
                reader.BaseStream.Seek(rootOffset, SeekOrigin.Begin);
                
                // Read the root directory header
                reader.ReadUInt32(); // Characteristics
                reader.ReadUInt32(); // TimeDateStamp
                reader.ReadUInt16(); // MajorVersion
                reader.ReadUInt16(); // MinorVersion
                ushort namedEntries = reader.ReadUInt16();
                ushort idEntries = reader.ReadUInt16();
                
                // Process each type entry (level 1)
                for (int i = 0; i < namedEntries + idEntries; i++)
                {
                    uint nameOrId = reader.ReadUInt32();
                    uint offsetToData = reader.ReadUInt32();
                    
                    bool isNamed = (nameOrId & 0x80000000) != 0;
                    uint typeId = isNamed ? 0 : nameOrId;
                    bool isDirectory = (offsetToData & 0x80000000) != 0;
                    uint subDirOffset = offsetToData & 0x7FFFFFFF;
                    
                    if (isDirectory)
                    {
                        // Save current position
                        long currentPos = reader.BaseStream.Position;
                        
                        // Process the type directory (level 2)
                        ProcessTypeDirectory(rawPeFile, reader, subDirOffset, typeId, resourceInfo.Resources);
                        
                        // Restore position for next entry
                        reader.BaseStream.Seek(currentPos, SeekOrigin.Begin);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error processing resources: {ex.Message}");
            }
        }
        
        return resourceInfo;
    }
    
    /// <summary>
    /// Processes a type directory (level 2) in the resource hierarchy
    /// </summary>
    /// <param name="rawPeFile">The raw PE file</param>
    /// <param name="reader">The binary reader</param>
    /// <param name="subDirOffset">The offset of the type directory from the start of the resource section</param>
    /// <param name="typeId">The type ID</param>
    /// <param name="resources">The list of resources to add to</param>
    private void ProcessTypeDirectory(RawPeFile rawPeFile, BinaryReader reader, uint subDirOffset, uint typeId, List<ResourceEntryInfo> resources)
    {
        // Seek to the type directory
        uint typeOffset = RvaToOffset(rawPeFile, _resourceDirectoryRva + subDirOffset);
        reader.BaseStream.Seek(typeOffset, SeekOrigin.Begin);
        
        // Read the type directory header
        reader.ReadUInt32(); // Characteristics
        reader.ReadUInt32(); // TimeDateStamp
        reader.ReadUInt16(); // MajorVersion
        reader.ReadUInt16(); // MinorVersion
        ushort namedEntries = reader.ReadUInt16();
        ushort idEntries = reader.ReadUInt16();
        
        // Process each name/id entry (level 2)
        for (int i = 0; i < namedEntries + idEntries; i++)
        {
            uint nameOrId = reader.ReadUInt32();
            uint offsetToData = reader.ReadUInt32();
            
            bool isNamed = (nameOrId & 0x80000000) != 0;
            uint nameId = isNamed ? 0 : nameOrId;
            bool isDirectory = (offsetToData & 0x80000000) != 0;
            uint langDirOffset = offsetToData & 0x7FFFFFFF;
            
            if (isDirectory)
            {
                // Save current position
                long currentPos = reader.BaseStream.Position;
                
                // Process the language directory (level 3)
                ProcessLanguageDirectory(rawPeFile, reader, langDirOffset, typeId, nameId, resources);
                
                // Restore position for next entry
                reader.BaseStream.Seek(currentPos, SeekOrigin.Begin);
            }
        }
    }
    
    /// <summary>
    /// Processes a language directory (level 3) in the resource hierarchy
    /// </summary>
    /// <param name="rawPeFile">The raw PE file</param>
    /// <param name="reader">The binary reader</param>
    /// <param name="langDirOffset">The offset of the language directory from the start of the resource section</param>
    /// <param name="typeId">The type ID</param>
    /// <param name="nameId">The name/ID</param>
    /// <param name="resources">The list of resources to add to</param>
    private void ProcessLanguageDirectory(RawPeFile rawPeFile, BinaryReader reader, uint langDirOffset, uint typeId, uint nameId, List<ResourceEntryInfo> resources)
    {
        // Seek to the language directory
        uint langOffset = RvaToOffset(rawPeFile, _resourceDirectoryRva + langDirOffset);
        reader.BaseStream.Seek(langOffset, SeekOrigin.Begin);
        
        // Read the language directory header
        reader.ReadUInt32(); // Characteristics
        reader.ReadUInt32(); // TimeDateStamp
        reader.ReadUInt16(); // MajorVersion
        reader.ReadUInt16(); // MinorVersion
        ushort namedEntries = reader.ReadUInt16();
        ushort idEntries = reader.ReadUInt16();
        
        // Process each language entry (level 3)
        for (int i = 0; i < namedEntries + idEntries; i++)
        {
            uint langId = reader.ReadUInt32();
            uint offsetToData = reader.ReadUInt32();
            
            bool isNamed = (langId & 0x80000000) != 0;
            uint languageId = isNamed ? 0 : langId;
            bool isDirectory = (offsetToData & 0x80000000) != 0;
            
            if (!isDirectory)
            {
                // This is a data entry
                uint dataEntryOffset = RvaToOffset(rawPeFile, _resourceDirectoryRva + offsetToData);
                reader.BaseStream.Seek(dataEntryOffset, SeekOrigin.Begin);
                
                uint dataRva = reader.ReadUInt32();
                uint dataSize = reader.ReadUInt32();
                uint codePage = reader.ReadUInt32();
                reader.ReadUInt32(); // Reserved
                
                var resource = new ResourceEntryInfo
                {
                    Type = GetResourceType(typeId),
                    Id = nameId,
                    HasName = false, // We're using IDs for simplicity
                    LanguageId = languageId,
                    CodePage = codePage,
                    Size = dataSize,
                    RVA = dataRva
                };
                
                if (_includeData)
                {
                    try
                    {
                        // Read the resource data
                        uint dataOffset = RvaToOffset(rawPeFile, dataRva);
                        resource.Data = new byte[dataSize];
                        Array.Copy(rawPeFile.RawData, (int)dataOffset, resource.Data, 0, (int)dataSize);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error reading resource data: {ex.Message}");
                        resource.Data = Array.Empty<byte>();
                    }
                }
                
                resources.Add(resource);
            }
        }
    }
    
    /// <summary>
    /// Gets the resource type from the type ID
    /// </summary>
    /// <param name="typeId">The type ID</param>
    /// <returns>The resource type</returns>
    private ResourceType GetResourceType(uint typeId)
    {
        if (Enum.IsDefined(typeof(ResourceType), (int)typeId))
        {
            return (ResourceType)typeId;
        }
        
        return ResourceType.Unknown;
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
}
