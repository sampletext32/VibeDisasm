using VibeDisasm.Pe.Models;
using VibeDisasm.Pe.Raw;

namespace VibeDisasm.Pe.Extractors;

/// <summary>
/// Extracts resources from a PE file
/// </summary>
public static class ResourceExtractor
{
    /// <summary>
    /// Extracts resource information from a PE file
    /// </summary>
    /// <param name="rawPeFile">The raw PE file</param>
    /// <returns>Resource information, or null if the PE file has no resources</returns>
    public static ResourceInfo? Extract(RawPeFile rawPeFile)
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
        var resourceDirectoryRva = rawPeFile.OptionalHeader.DataDirectories[2].VirtualAddress;
        var resourceDirectorySize = rawPeFile.OptionalHeader.DataDirectories[2].Size;

        var resourceInfo = new ResourceInfo {DirectoryRVA = resourceDirectoryRva, DirectorySize = resourceDirectorySize};

        // Process the resource directory
        if (rawPeFile.ResourceDirectory != null)
        {
            try
            {
                using var stream = new MemoryStream(rawPeFile.RawData);
                using var reader = new BinaryReader(stream);

                // Get to the root resource directory
                var rootOffset = Util.RvaToOffset(rawPeFile, resourceDirectoryRva);
                reader.BaseStream.Seek(rootOffset, SeekOrigin.Begin);

                // Read the root directory header
                reader.ReadUInt32(); // Characteristics
                reader.ReadUInt32(); // TimeDateStamp
                reader.ReadUInt16(); // MajorVersion
                reader.ReadUInt16(); // MinorVersion
                var namedEntries = reader.ReadUInt16();
                var idEntries = reader.ReadUInt16();

                // Process each type entry (level 1)
                for (var i = 0; i < namedEntries + idEntries; i++)
                {
                    var nameOrId = reader.ReadUInt32();
                    var offsetToData = reader.ReadUInt32();

                    var isNamed = (nameOrId & 0x80000000) != 0;
                    var typeId = isNamed
                        ? 0
                        : nameOrId;
                    var isDirectory = (offsetToData & 0x80000000) != 0;
                    var subDirOffset = offsetToData & 0x7FFFFFFF;

                    if (isDirectory)
                    {
                        // Save current position
                        var currentPos = reader.BaseStream.Position;

                        // Process the type directory (level 2)
                        ProcessTypeDirectory(
                            rawPeFile,
                            reader,
                            subDirOffset,
                            typeId,
                            resourceInfo.Resources,
                            resourceDirectoryRva
                        );

                        // Restore position for next entry
                        reader.BaseStream.Seek(currentPos, SeekOrigin.Begin);
                    }
                }

                // String tables will be extracted separately by StringTableExtractor
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
    private static void ProcessTypeDirectory(RawPeFile rawPeFile, BinaryReader reader, uint subDirOffset, uint typeId, List<ResourceEntryInfo> resources, uint resourceDirectoryRva)
    {
        // Seek to the type directory
        var typeOffset = Util.RvaToOffset(rawPeFile, resourceDirectoryRva + subDirOffset);
        reader.BaseStream.Seek(typeOffset, SeekOrigin.Begin);

        // Read the type directory header
        reader.ReadUInt32(); // Characteristics
        reader.ReadUInt32(); // TimeDateStamp
        reader.ReadUInt16(); // MajorVersion
        reader.ReadUInt16(); // MinorVersion
        var namedEntries = reader.ReadUInt16();
        var idEntries = reader.ReadUInt16();

        // Process each name/id entry (level 2)
        for (var i = 0; i < namedEntries + idEntries; i++)
        {
            var nameOrId = reader.ReadUInt32();
            var offsetToData = reader.ReadUInt32();

            var isNamed = (nameOrId & 0x80000000) != 0;
            var nameId = isNamed
                ? 0
                : nameOrId;
            var isDirectory = (offsetToData & 0x80000000) != 0;
            var langDirOffset = offsetToData & 0x7FFFFFFF;

            if (isDirectory)
            {
                // Save current position
                var currentPos = reader.BaseStream.Position;

                // Process the language directory (level 3)
                ProcessLanguageDirectory(
                    rawPeFile,
                    reader,
                    langDirOffset,
                    typeId,
                    nameId,
                    resources,
                    resourceDirectoryRva
                );

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
    private static void ProcessLanguageDirectory(RawPeFile rawPeFile, BinaryReader reader, uint langDirOffset, uint typeId, uint nameId, List<ResourceEntryInfo> resources, uint resourceDirectoryRva)
    {
        // Seek to the language directory
        var langOffset = Util.RvaToOffset(rawPeFile, resourceDirectoryRva + langDirOffset);
        reader.BaseStream.Seek(langOffset, SeekOrigin.Begin);

        // Read the language directory header
        reader.ReadUInt32(); // Characteristics
        reader.ReadUInt32(); // TimeDateStamp
        reader.ReadUInt16(); // MajorVersion
        reader.ReadUInt16(); // MinorVersion
        var namedEntries = reader.ReadUInt16();
        var idEntries = reader.ReadUInt16();

        // Process each language entry (level 3)
        for (var i = 0; i < namedEntries + idEntries; i++)
        {
            var langId = reader.ReadUInt32();
            var offsetToData = reader.ReadUInt32();

            var isNamed = (langId & 0x80000000) != 0;
            var languageId = isNamed
                ? 0
                : langId;
            var isDirectory = (offsetToData & 0x80000000) != 0;

            if (!isDirectory)
            {
                // This is a data entry
                var dataEntryOffset = Util.RvaToOffset(rawPeFile, resourceDirectoryRva + offsetToData);
                reader.BaseStream.Seek(dataEntryOffset, SeekOrigin.Begin);

                var dataRva = reader.ReadUInt32();
                var dataSize = reader.ReadUInt32();
                var codePage = reader.ReadUInt32();
                reader.ReadUInt32(); // Reserved

                var resource = new ResourceEntryInfo
                {
                    Type = (ResourceType)typeId,
                    Id = nameId,
                    HasName = false, // We're using IDs for simplicity
                    LanguageId = languageId,
                    CodePage = codePage,
                    Size = dataSize,
                    RVA = dataRva
                };

                // Read the resource data
                var dataOffset = Util.RvaToOffset(rawPeFile, dataRva);
                // Store the absolute file offset
                resource.FileOffset = dataOffset;

                resources.Add(resource);
            }
        }
    }
}
