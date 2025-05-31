using System.Text;

namespace VibeDisasm.Pe.Extractors;

/// <summary>
/// Extracts string tables from resource data
/// </summary>
public class StringTableExtractor
{
    /// <summary>
    /// Extracts all string tables from resource information
    /// </summary>
    /// <param name="resourceInfo">The resource information containing all extracted resources</param>
    /// <returns>A list of string table info objects</returns>
    public static List<StringTableInfo> ExtractAll(ResourceInfo resourceInfo)
    {
        var stringTables = new List<StringTableInfo>();

        // Find all string table resources
        var stringTableResources = resourceInfo.Resources
            .Where(r => r.Type == ResourceType.StringTable)
            .ToList();

        if (stringTableResources.Count == 0)
        {
            return new List<StringTableInfo>();
        }

        // Group string table resources by ID and language
        var groupedResources = stringTableResources
            .GroupBy(r => new { r.Id, r.LanguageId })
            .ToList();

        // Process each group
        foreach (var group in groupedResources)
        {
            // In PE files, there should only be one resource entry for each combination of ID and language
            // If there are multiple entries with the same ID and language, we take the first one
            var resource = group.First();

            // Skip if we don't have the data
            if (resource.Data.Length == 0)
            {
                continue;
            }

            // Extract the string table
            var stringTable = ExtractSingleStringTable(
                resource.Data,
                resource.Id,
                resource.LanguageId,
                resource.FileOffset);

            if (stringTable.Strings.Count > 0)
            {
                stringTables.Add(stringTable);
            }
        }

        return stringTables;
    }

    /// <summary>
    /// Extracts a string table from resource data
    /// </summary>
    /// <param name="resourceData">The raw resource data</param>
    /// <param name="id">The ID of the string table</param>
    /// <param name="languageId">The language ID of the string table</param>
    /// <param name="fileOffset">The absolute file offset of the resource data</param>
    /// <returns>A string table info object</returns>
    public static StringTableInfo ExtractSingleStringTable(byte[] resourceData, uint id, uint languageId, uint fileOffset = 0)
    {
        if (resourceData.Length == 0)
        {
            return new StringTableInfo
            {
                Id = id,
                LanguageId = languageId
            };
        }

        var stringTable = new StringTableInfo
        {
            Id = id,
            LanguageId = languageId,
            FileOffset = fileOffset
        };

        try
        {
            using var stream = new MemoryStream(resourceData);
            using var reader = new BinaryReader(stream);

            // String tables in PE files have a different format than we initially thought.
            // The actual format is a bit more complex and doesn't have a simple header.
            // Instead, it's a series of string entries directly.

            // String table resources in PE files are structured as follows:
            // - Each string table contains strings with IDs from (N*16) to (N*16+15)
            // - Each string is stored as:
            //   - 16-bit length prefix (in characters, not bytes)
            //   - Unicode string data (length * 2 bytes)
            // - Strings are stored in order by ID, with missing IDs having length 0

            // Calculate the base ID for this string table (ID * 16)
            var baseId = (ushort)(id * 16);

            // Parse each string in the table
            while (reader.BaseStream.Position < reader.BaseStream.Length)
            {
                // Try to read the length prefix
                if (reader.BaseStream.Position + 2 > reader.BaseStream.Length)
                {
                    break;
                }

                // Read the length prefix (in characters)
                var length = reader.ReadUInt16();

                // Calculate the current string ID based on position in the table
                var stringId = (ushort)(baseId + stringTable.Strings.Count);

                // Read the string data if length > 0
                if (length > 0)
                {
                    // Make sure we have enough data
                    if (reader.BaseStream.Position + (length * 2) > reader.BaseStream.Length)
                    {
                        break;
                    }

                    // Calculate and store the absolute file offset for this string
                    // Base offset + current position within the resource data
                    var stringOffset = fileOffset + (uint)reader.BaseStream.Position;
                    stringTable.StringFileOffsets[stringId] = stringOffset;

                    // Read the Unicode string data
                    var stringData = reader.ReadBytes(length * 2);
                    var value = Encoding.Unicode.GetString(stringData);

                    // Add the string to the table
                    stringTable.Strings[stringId] = value;
                }
                else
                {
                    // Empty string
                    stringTable.Strings[stringId] = string.Empty;
                    // Still track the position for empty strings
                    stringTable.StringFileOffsets[stringId] = fileOffset + (uint)reader.BaseStream.Position - 2; // -2 to point to the length field
                }
            }
        }
        catch (EndOfStreamException)
        {
            // Reached end of stream, just return what we have
        }
        catch (IOException ex)
        {
            System.Console.WriteLine($"Error parsing string table: {ex.Message}");
        }

        return stringTable;
    }
}
