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
    public static PeResources? Extract(RawPeFile rawPeFile)
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

        var resourceInfo = new PeResources {DirectoryRva = resourceDirectoryRva, DirectorySize = resourceDirectorySize};

        // Process the resource directory
        if (rawPeFile.ResourceDirectory == null)
        {
            return resourceInfo;
        }

        using var stream = new MemoryStream(rawPeFile.RawData);
        using var reader = new BinaryReader(stream);

        // Get to the root resource directory
        var rootOffset = Util.RvaToOffset(rawPeFile, resourceDirectoryRva);
        reader.BaseStream.Seek(rootOffset, SeekOrigin.Begin);

        // Read the root directory header
        resourceInfo.Characteristics = reader.ReadUInt32(); // Characteristics
        resourceInfo.TimeDateStamp = reader.ReadUInt32(); // TimeDateStamp
        resourceInfo.MajorVersion = reader.ReadUInt16(); // MajorVersion
        resourceInfo.MinorVersion = reader.ReadUInt16(); // MinorVersion
        resourceInfo.NamedEntries = reader.ReadUInt16();
        resourceInfo.IdEntries = reader.ReadUInt16();

        // Process each type entry (level 1)
        for (var i = 0; i < resourceInfo.NamedEntries + resourceInfo.IdEntries; i++)
        {
            var nameOrId = reader.ReadUInt32();
            var offsetToData = reader.ReadUInt32();

            var isNamed = (nameOrId & 0x80000000) != 0;
            var resourceName = "";

            if (isNamed)
            {
                resourceName = ReadResourceName(reader, nameOrId, rootOffset);
            }

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
                var typeDirectory = ProcessTypeDirectory(
                    rawPeFile,
                    reader,
                    subDirOffset,
                    (ResourceType)typeId,
                    isNamed,
                    resourceName,
                    resourceDirectoryRva
                );

                // Add the type directory to the root directory
                resourceInfo.RootDirectory.Add(typeDirectory);

                // Restore position for next entry
                reader.BaseStream.Seek(currentPos, SeekOrigin.Begin);
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
    /// <param name="typeIsNamed">Whether the type entry is named</param>
    /// <param name="typeName">The type name if the type entry is named</param>
    /// <param name="resourceDirectoryRva">The RVA of the resource directory</param>
    /// <returns>A ResourceDirectoryEntry representing this type directory</returns>
    private static ResourceDirectoryEntry ProcessTypeDirectory(RawPeFile rawPeFile, BinaryReader reader, uint subDirOffset, ResourceType typeId, bool typeIsNamed, string typeName, uint resourceDirectoryRva)
    {
        // Create a directory entry for this type level
        var typeDirectory = new ResourceDirectoryEntry {TypeId = typeId, HasName = typeIsNamed, Name = typeName};

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
            var resourceName = !isNamed ? "" : ReadResourceName(reader, nameOrId, typeOffset);

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
                var langDirectory = ProcessLanguageDirectory(
                    rawPeFile,
                    reader,
                    langDirOffset,
                    typeId,
                    (ResourceType)nameId,
                    isNamed,
                    resourceName,
                    resourceDirectoryRva
                );

                // Add the language directory to this type directory
                typeDirectory.Directories.Add(langDirectory);

                // Restore position for next entry
                reader.BaseStream.Seek(currentPos, SeekOrigin.Begin);
            }
        }

        return typeDirectory;
    }

    /// <summary>
    /// Processes a language directory (level 3) in the resource hierarchy
    /// </summary>
    /// <param name="rawPeFile">The raw PE file</param>
    /// <param name="reader">The binary reader</param>
    /// <param name="langDirOffset">The offset of the language directory from the start of the resource section</param>
    /// <param name="typeId">The type ID</param>
    /// <param name="nameId">The name/ID</param>
    /// <param name="nameIsNamed">Whether the name entry is named</param>
    /// <param name="nameString">The name string if the name entry is named</param>
    /// <param name="resourceDirectoryRva">The RVA of the resource directory</param>
    /// <returns>A ResourceDirectoryEntry representing this language directory</returns>
    private static ResourceDirectoryEntry ProcessLanguageDirectory(RawPeFile rawPeFile, BinaryReader reader, uint langDirOffset, ResourceType typeId, ResourceType nameId, bool nameIsNamed, string nameString, uint resourceDirectoryRva)
    {
        // Create a directory entry for this language level
        var langDirectory = new ResourceDirectoryEntry {TypeId = nameId, HasName = nameIsNamed, Name = nameString};

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
            var langName = !isNamed ? "" : ReadResourceName(reader, langId, langOffset);

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
                    Type = typeId,
                    NameId = nameId,
                    HasName = nameIsNamed,
                    Name = nameString,
                    LanguageId = languageId,
                    CodePage = codePage,
                    Size = dataSize,
                    RVA = dataRva
                };

                // Read the resource data
                var dataOffset = Util.RvaToOffset(rawPeFile, dataRva);
                // Store the absolute file offset
                resource.FileOffset = dataOffset;

                // Add to both the flat list and the hierarchical structure
                langDirectory.DataEntries.Add(resource);
            }
        }

        return langDirectory;
    }

    /// <summary>
    /// Reads a resource name string from the resource directory
    /// </summary>
    /// <param name="reader">The binary reader</param>
    /// <param name="nameId">The name ID with the high bit set</param>
    /// <param name="baseOffset">The base offset of the current directory</param>
    /// <returns>The resource name string</returns>
    private static string ReadResourceName(BinaryReader reader, uint nameId, long baseOffset)
    {
        // Save current position
        var currentPos = reader.BaseStream.Position;

        // Calculate absolute offset to the name string
        var nameOffset = nameId & 0x7FFFFFFF;
        var absoluteNameOffset = baseOffset + nameOffset;

        // Read the name string
        reader.BaseStream.Seek(absoluteNameOffset, SeekOrigin.Begin);
        var nameLength = reader.ReadUInt16(); // Length in WCHARs
        var name = reader.ReadFixedLengthUnicodeString(nameLength);

        // Restore position
        reader.BaseStream.Seek(currentPos, SeekOrigin.Begin);

        return name;
    }
}
