using System.Collections.Generic;
using System.IO;
using System.Text;

namespace VibeDisasm.Pe.Extractors
{
    /// <summary>
    /// Represents a string table resource
    /// </summary>
    public class StringTableInfo
    {
        /// <summary>
        /// Gets or sets the ID of the string table
        /// </summary>
        public uint Id { get; set; }

        /// <summary>
        /// Gets or sets the language ID of the string table
        /// </summary>
        public uint LanguageId { get; set; }

        /// <summary>
        /// Gets or sets the collection of strings in the string table
        /// </summary>
        public Dictionary<ushort, string> Strings { get; set; } = new Dictionary<ushort, string>();
    }

    /// <summary>
    /// Extracts string tables from resource data
    /// </summary>
    public class StringTableExtractor
    {
        /// <summary>
        /// Extracts a string table from resource data
        /// </summary>
        /// <param name="resourceData">The raw resource data</param>
        /// <param name="id">The ID of the string table</param>
        /// <param name="languageId">The language ID of the string table</param>
        /// <returns>A string table info object</returns>
        public StringTableInfo Extract(byte[] resourceData, uint id, uint languageId)
        {
            if (resourceData == null || resourceData.Length == 0)
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
                LanguageId = languageId
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
                ushort baseId = (ushort)(id * 16);

                // Parse each string in the table
                while (reader.BaseStream.Position < reader.BaseStream.Length)
                {
                    // Try to read the length prefix
                    if (reader.BaseStream.Position + 2 > reader.BaseStream.Length)
                    {
                        break;
                    }

                    // Read the length prefix (in characters)
                    ushort length = reader.ReadUInt16();

                    // Calculate the current string ID based on position in the table
                    ushort stringId = (ushort)(baseId + stringTable.Strings.Count);

                    // Read the string data if length > 0
                    if (length > 0)
                    {
                        // Make sure we have enough data
                        if (reader.BaseStream.Position + (length * 2) > reader.BaseStream.Length)
                        {
                            break;
                        }

                        // Read the Unicode string data
                        byte[] stringData = reader.ReadBytes(length * 2);
                        string value = Encoding.Unicode.GetString(stringData);

                        // Add the string to the table
                        stringTable.Strings[stringId] = value;
                    }
                    else
                    {
                        // Empty string
                        stringTable.Strings[stringId] = string.Empty;
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
}
