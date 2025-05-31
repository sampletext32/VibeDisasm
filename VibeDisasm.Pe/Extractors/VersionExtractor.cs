namespace VibeDisasm.Pe.Extractors;

/// <summary>
/// Extracts version information from PE files
/// </summary>
public static class VersionExtractor
{
    /// <summary>
    /// Extracts all version information from the resource information
    /// </summary>
    /// <param name="resourceInfo">The resource information</param>
    /// <returns>A list of version information objects</returns>
    public static List<VersionInfo> ExtractAll(ResourceInfo resourceInfo)
    {
        if (resourceInfo == null)
        {
            throw new ArgumentNullException(nameof(resourceInfo));
        }

        var result = new List<VersionInfo>();

        // Find all version resources
        var versionResources = resourceInfo.Resources.Where(r => r.Type == ResourceType.Version).ToList();
        if (versionResources.Count == 0)
        {
            return result;
        }

        // Process each version resource
        foreach (var resource in versionResources)
        {
            // Skip if no data
            if (resource.Data.Length == 0)
            {
                continue;
            }

            // Extract version info
            var versionInfo = ExtractVersionInfo(resource.Data);
            if (versionInfo != null)
            {
                // Set the language ID and codepage from the resource
                versionInfo.LanguageId = resource.LanguageId;
                versionInfo.CodePage = resource.CodePage;
                result.Add(versionInfo);
            }
        }

        return result;
    }

    /// <summary>
    /// Extracts version information from a version resource data
    /// </summary>
    /// <param name="resourceData">The version resource data</param>
    /// <returns>The extracted version information, or null if not found</returns>
    private static VersionInfo? ExtractVersionInfo(byte[] resourceData)
    {
        // Minimum size check for a valid version resource
        if (resourceData.Length < 40)
        {
            return null;
        }

        try
        {
            using var stream = new MemoryStream(resourceData);
            using var reader = new BinaryReader(stream);

            // Parse VS_VERSIONINFO structure
            // Reference: https://learn.microsoft.com/en-us/windows/win32/menurc/vs-versioninfo

            // Read the header
            var length = reader.ReadUInt16();    // wLength - Total structure length
            _ = reader.ReadUInt16();                // wValueLength - Value field length
            _ = reader.ReadUInt16();                // wType - 0=binary, 1=text

            // Read the key (should be "VS_VERSION_INFO")
            var key = reader.ReadNullTerminatedUnicodeString();

            // Be lenient with non-standard version resources
            if (!key.Contains("VERSION", StringComparison.OrdinalIgnoreCase) && key.Length > 1)
            {
                Console.WriteLine($"Warning: Unexpected version info key: {key}");
            }

            // Align to 4-byte boundary before reading the VS_FIXEDFILEINFO
            AlignTo4ByteBoundary(reader);

            // Read VS_FIXEDFILEINFO structure
            // Reference: https://learn.microsoft.com/en-us/windows/win32/api/verrsrc/ns-verrsrc-vs_fixedfileinfo

            var signature = reader.ReadUInt32();  // dwSignature - Always 0xFEEF04BD

            if (signature != 0xFEEF04BD)
            {
                Console.WriteLine("Invalid VS_FIXEDFILEINFO signature.");
                return null;
            }

            // Read version information fields
            _ = reader.ReadUInt32();                        // dwStrucVersion
            var fileVersionMS = reader.ReadUInt32();       // dwFileVersionMS
            var fileVersionLS = reader.ReadUInt32();       // dwFileVersionLS
            var productVersionMS = reader.ReadUInt32();    // dwProductVersionMS
            var productVersionLS = reader.ReadUInt32();    // dwProductVersionLS
            _ = reader.ReadUInt64();                        // dwFileFlagsMask
            var fileFlags = reader.ReadUInt32();           // dwFileFlags
            var fileOS = reader.ReadUInt32();              // dwFileOS
            var fileType = reader.ReadUInt32();            // dwFileType
            var fileSubtype = reader.ReadUInt32();         // dwFileSubtype
            _ = reader.ReadUInt32();                        // dwFileDateMS

            // Create and populate the version info object
            var versionInfo = new VersionInfo
            {
                FileVersion = FormatVersion(fileVersionMS, fileVersionLS),
                ProductVersion = FormatVersion(productVersionMS, productVersionLS),
                FileFlags = fileFlags,
                FileOS = fileOS,
                FileType = fileType,
                FileSubtype = fileSubtype
            };

            // Parse child blocks (StringFileInfo and VarFileInfo)
            while (reader.BaseStream.Position < length)
            {
                var childStartPos = reader.BaseStream.Position;
                var childLength = reader.ReadUInt16();           // wLength
                _ = reader.ReadUInt16();                            // wValueLength
                _ = reader.ReadUInt16();                            // wType
                var childKey = reader.ReadNullTerminatedUnicodeString();
                AlignTo4ByteBoundary(reader);
                var childHeaderLength = reader.BaseStream.Position - childStartPos;
                var childDataLength = childLength - childHeaderLength;

                // Process child blocks based on their key
                if (childKey == "StringFileInfo" && childDataLength > 0)
                {
                    ParseStringFileInfo(reader, versionInfo, childDataLength);
                }
                else if (childKey == "VarFileInfo" && childDataLength > 0)
                {
                    ParseVarFileInfo(reader, versionInfo, childDataLength);
                }
                else
                {
                    // Skip unknown blocks
                    reader.BaseStream.Seek(childLength - (reader.BaseStream.Position - childStartPos), SeekOrigin.Current);
                }
            }

            return versionInfo;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error parsing version info: {ex.Message}");
            return null;
        }
    }

    /// <summary>
    /// Aligns the reader position to a 4-byte boundary
    /// </summary>
    /// <param name="reader">The binary reader</param>
    private static void AlignTo4ByteBoundary(BinaryReader reader)
    {
        var position = reader.BaseStream.Position;
        var padding = (int)((4 - (position % 4)) % 4);
        reader.BaseStream.Seek(padding, SeekOrigin.Current);
    }

    /// <summary>
    /// Parses the StringFileInfo block which contains string properties
    /// </summary>
    /// <param name="reader">The binary reader</param>
    /// <param name="versionInfo">The version info to populate</param>
    /// <param name="length">The length of the StringFileInfo block</param>
    private static void ParseStringFileInfo(BinaryReader reader, VersionInfo versionInfo, long length)
    {
        var startPos = reader.BaseStream.Position;
        var endPos = startPos + length;

        // Process all string tables in the StringFileInfo block
        while (reader.BaseStream.Position < endPos)
        {
            var tableStartPos = reader.BaseStream.Position;

            // Read string table header
            var tableLength = reader.ReadUInt16();
            _ = reader.ReadUInt16();
            _ = reader.ReadUInt16();
            _ = reader.ReadNullTerminatedUnicodeString(); // e.g., "040904E3"
            AlignTo4ByteBoundary(reader);

            // Calculate the data length of the string table
            var tableHeaderLength = reader.BaseStream.Position - tableStartPos;
            var tableDataLength = tableLength - tableHeaderLength;

            // Parse the string table and add its entries to the version info
            if (tableDataLength > 0)
            {
                var stringValues = ParseStringTable(reader, tableDataLength);
                foreach (var kvp in stringValues)
                {
                    versionInfo.StringFileInfo[kvp.Key] = kvp.Value;
                }
            }
        }
    }

    /// <summary>
    /// Parses a string table containing key-value pairs
    /// </summary>
    /// <param name="reader">The binary reader</param>
    /// <param name="length">The length of the string table</param>
    /// <returns>A dictionary of string keys and values</returns>
    private static Dictionary<string, string> ParseStringTable(BinaryReader reader, long length)
    {
        var items = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        var startPos = reader.BaseStream.Position;
        var endPos = startPos + length;

        // Process all string entries in the table
        while (reader.BaseStream.Position < endPos)
        {
            // Read string entry header
            _ = reader.ReadUInt16();
            var stringValueLength = reader.ReadUInt16();
            _ = reader.ReadUInt16();
            var stringKey = reader.ReadNullTerminatedUnicodeString();
            AlignTo4ByteBoundary(reader);

            // Read the string value
            if (stringValueLength > 0)
            {
                var value = reader.ReadFixedLengthUnicodeString(stringValueLength);
                AlignTo4ByteBoundary(reader);

                // Store the key-value pair
                if (!string.IsNullOrEmpty(stringKey) && !string.IsNullOrEmpty(value))
                {
                    items[stringKey] = value;
                }
            }
        }

        return items;
    }

    /// <summary>
    /// Parses the VarFileInfo block which contains translation information
    /// </summary>
    /// <param name="reader">The binary reader</param>
    /// <param name="versionInfo">The version info to populate</param>
    /// <param name="length">The length of the VarFileInfo block</param>
    private static void ParseVarFileInfo(BinaryReader reader, VersionInfo versionInfo, long length)
    {
        var startPos = reader.BaseStream.Position;
        var endPos = startPos + length;

        // Process all var entries in the VarFileInfo block
        while (reader.BaseStream.Position < endPos)
        {
            var varStartPos = reader.BaseStream.Position;

            // Read var entry header
            var varLength = reader.ReadUInt16();
            var varValueLength = reader.ReadUInt16();
            _ = reader.ReadUInt16();
            var varKey = reader.ReadNullTerminatedUnicodeString(); // e.g., "Translation"
            AlignTo4ByteBoundary(reader);

            // Process the Translation entry
            if (varKey == "Translation" && varValueLength > 0)
            {
                // Each translation is a DWORD (4 bytes): language ID (low word) + codepage (high word)
                var numTranslations = varValueLength / 4;
                for (var i = 0; i < numTranslations; i++)
                {
                    var translation = reader.ReadUInt32();
                    var languageId = (ushort)(translation & 0xFFFF);
                    var codepage = (ushort)(translation >> 16);

                    // Add the translation to the version info
                    versionInfo.Translations.Add(new Translation
                    {
                        LanguageId = languageId,
                        Codepage = codepage
                    });
                }
            }

            // Ensure we're at the end of the var entry
            reader.BaseStream.Position = varStartPos + varLength;
        }
    }

    /// <summary>
    /// Formats a version number from MS and LS parts
    /// </summary>
    /// <param name="ms">The most significant part (major.minor)</param>
    /// <param name="ls">The least significant part (build.revision)</param>
    /// <returns>The formatted version string</returns>
    private static string FormatVersion(uint ms, uint ls)
    {
        // Extract components from MS (major, minor) and LS (build, revision)
        var major = (ushort)(ms >> 16);     // High 16 bits of MS
        var minor = (ushort)(ms & 0xFFFF);  // Low 16 bits of MS
        var build = (ushort)(ls >> 16);     // High 16 bits of LS
        var revision = (ushort)(ls & 0xFFFF); // Low 16 bits of LS

        return $"{major}.{minor}.{build}.{revision}";
    }
}
