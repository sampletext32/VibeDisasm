namespace VibeDisasm.Pe.Raw;

/// <summary>
/// Parser for raw PE file structures
/// </summary>
public class RawPeParser
{
    /// <summary>
    /// Parses a PE file from raw file data
    /// </summary>
    /// <param name="fileData">The raw file data</param>
    /// <returns>A raw PE file with parsed structures</returns>
    public RawPeFile Parse(byte[] fileData)
    {
        if (fileData == null || fileData.Length == 0)
        {
            throw new ArgumentException("File data is null or empty", nameof(fileData));
        }

        var rawPeFile = new RawPeFile(fileData);

        using var stream = new MemoryStream(fileData);
        using var reader = new BinaryReader(stream);

        // Parse DOS header
        ParseDosHeader(reader, rawPeFile);

        // Move to PE header
        reader.BaseStream.Seek(rawPeFile.DosHeader.AddressOfPeHeader, SeekOrigin.Begin);

        // Read PE signature
        rawPeFile.PeSignature = reader.ReadUInt32();
        if (rawPeFile.PeSignature != 0x00004550) // "PE\0\0"
        {
            throw new InvalidDataException("Invalid PE signature");
        }

        // Parse File Header
        ParseFileHeader(reader, rawPeFile);

        // Parse Optional Header
        ParseOptionalHeader(reader, rawPeFile);

        // Parse Section Headers
        ParseSectionHeaders(reader, rawPeFile);

        // Parse additional directories if requested
        ParseDirectories(reader, rawPeFile);

        return rawPeFile;
    }

    /// <summary>
    /// Parses the DOS header
    /// </summary>
    private void ParseDosHeader(BinaryReader reader, RawPeFile rawPeFile)
    {
        var dosHeader = rawPeFile.DosHeader;

        dosHeader.Magic = reader.ReadUInt16();
        if (dosHeader.Magic != 0x5A4D) // "MZ"
        {
            throw new InvalidDataException("Invalid DOS signature (MZ)");
        }

        dosHeader.BytesOnLastPage = reader.ReadUInt16();
        dosHeader.PagesInFile = reader.ReadUInt16();
        dosHeader.Relocations = reader.ReadUInt16();
        dosHeader.SizeOfHeaderInParagraphs = reader.ReadUInt16();
        dosHeader.MinimumExtraParagraphs = reader.ReadUInt16();
        dosHeader.MaximumExtraParagraphs = reader.ReadUInt16();
        dosHeader.InitialSS = reader.ReadUInt16();
        dosHeader.InitialSP = reader.ReadUInt16();
        dosHeader.Checksum = reader.ReadUInt16();
        dosHeader.InitialIP = reader.ReadUInt16();
        dosHeader.InitialCS = reader.ReadUInt16();
        dosHeader.AddressOfRelocationTable = reader.ReadUInt16();
        dosHeader.OverlayNumber = reader.ReadUInt16();

        dosHeader.Reserved1 = new ushort[4];
        for (var i = 0; i < 4; i++)
        {
            dosHeader.Reserved1[i] = reader.ReadUInt16();
        }

        dosHeader.OemIdentifier = reader.ReadUInt16();
        dosHeader.OemInformation = reader.ReadUInt16();

        dosHeader.Reserved2 = new ushort[10];
        for (var i = 0; i < 10; i++)
        {
            dosHeader.Reserved2[i] = reader.ReadUInt16();
        }

        dosHeader.AddressOfPeHeader = reader.ReadUInt32();
    }

    /// <summary>
    /// Parses the file header
    /// </summary>
    private void ParseFileHeader(BinaryReader reader, RawPeFile rawPeFile)
    {
        var fileHeader = rawPeFile.FileHeader;

        fileHeader.Machine = reader.ReadUInt16();
        fileHeader.NumberOfSections = reader.ReadUInt16();
        fileHeader.TimeDateStamp = reader.ReadUInt32();
        fileHeader.PointerToSymbolTable = reader.ReadUInt32();
        fileHeader.NumberOfSymbols = reader.ReadUInt32();
        fileHeader.SizeOfOptionalHeader = reader.ReadUInt16();
        fileHeader.Characteristics = reader.ReadUInt16();
    }

    /// <summary>
    /// Parses the optional header
    /// </summary>
    private void ParseOptionalHeader(BinaryReader reader, RawPeFile rawPeFile)
    {
        var optionalHeader = rawPeFile.OptionalHeader;

        // Standard fields
        optionalHeader.Magic = reader.ReadUInt16();
        optionalHeader.MajorLinkerVersion = reader.ReadByte();
        optionalHeader.MinorLinkerVersion = reader.ReadByte();
        optionalHeader.SizeOfCode = reader.ReadUInt32();
        optionalHeader.SizeOfInitializedData = reader.ReadUInt32();
        optionalHeader.SizeOfUninitializedData = reader.ReadUInt32();
        optionalHeader.AddressOfEntryPoint = reader.ReadUInt32();
        optionalHeader.BaseOfCode = reader.ReadUInt32();

        // PE32 vs PE32+ specific fields
        if (optionalHeader.Magic == 0x10B) // PE32
        {
            optionalHeader.BaseOfData = reader.ReadUInt32();
            optionalHeader.ImageBase = reader.ReadUInt32();
        }
        else if (optionalHeader.Magic == 0x20B) // PE32+
        {
            optionalHeader.BaseOfData = 0; // Not present in PE32+
            optionalHeader.ImageBase = reader.ReadUInt64();
        }
        else
        {
            throw new InvalidDataException($"Unknown optional header magic: 0x{optionalHeader.Magic:X4}");
        }

        // Windows-specific fields
        optionalHeader.SectionAlignment = reader.ReadUInt32();
        optionalHeader.FileAlignment = reader.ReadUInt32();
        optionalHeader.MajorOperatingSystemVersion = reader.ReadUInt16();
        optionalHeader.MinorOperatingSystemVersion = reader.ReadUInt16();
        optionalHeader.MajorImageVersion = reader.ReadUInt16();
        optionalHeader.MinorImageVersion = reader.ReadUInt16();
        optionalHeader.MajorSubsystemVersion = reader.ReadUInt16();
        optionalHeader.MinorSubsystemVersion = reader.ReadUInt16();
        optionalHeader.Win32VersionValue = reader.ReadUInt32();
        optionalHeader.SizeOfImage = reader.ReadUInt32();
        optionalHeader.SizeOfHeaders = reader.ReadUInt32();
        optionalHeader.CheckSum = reader.ReadUInt32();
        optionalHeader.Subsystem = reader.ReadUInt16();
        optionalHeader.DllCharacteristics = reader.ReadUInt16();

        // PE32 vs PE32+ specific fields for stack and heap
        if (optionalHeader.Magic == 0x10B) // PE32
        {
            optionalHeader.SizeOfStackReserve = reader.ReadUInt32();
            optionalHeader.SizeOfStackCommit = reader.ReadUInt32();
            optionalHeader.SizeOfHeapReserve = reader.ReadUInt32();
            optionalHeader.SizeOfHeapCommit = reader.ReadUInt32();
        }
        else // PE32+
        {
            optionalHeader.SizeOfStackReserve = reader.ReadUInt64();
            optionalHeader.SizeOfStackCommit = reader.ReadUInt64();
            optionalHeader.SizeOfHeapReserve = reader.ReadUInt64();
            optionalHeader.SizeOfHeapCommit = reader.ReadUInt64();
        }

        optionalHeader.LoaderFlags = reader.ReadUInt32();
        optionalHeader.NumberOfRvaAndSizes = reader.ReadUInt32();

        // Data directories
        var numDirectories = (int)Math.Min(optionalHeader.NumberOfRvaAndSizes, 16);
        optionalHeader.DataDirectories = new RawDataDirectory[16];

        for (var i = 0; i < numDirectories; i++)
        {
            var dataDir = new RawDataDirectory
            {
                VirtualAddress = reader.ReadUInt32(),
                Size = reader.ReadUInt32()
            };
            optionalHeader.DataDirectories[i] = dataDir;
        }

        // Initialize any remaining directories to empty
        for (var i = numDirectories; i < 16; i++)
        {
            optionalHeader.DataDirectories[i] = new RawDataDirectory();
        }
    }

    /// <summary>
    /// Parses the section headers
    /// </summary>
    private void ParseSectionHeaders(BinaryReader reader, RawPeFile rawPeFile)
    {
        int numSections = rawPeFile.FileHeader.NumberOfSections;
        rawPeFile.SectionHeaders = new RawSectionHeader[numSections];

        for (var i = 0; i < numSections; i++)
        {
            var sectionHeader = new RawSectionHeader();

            // Read section name (8 bytes)
            sectionHeader.NameBytes = reader.ReadBytes(8);

            sectionHeader.VirtualSize = reader.ReadUInt32();
            sectionHeader.VirtualAddress = reader.ReadUInt32();
            sectionHeader.SizeOfRawData = reader.ReadUInt32();
            sectionHeader.PointerToRawData = reader.ReadUInt32();
            sectionHeader.PointerToRelocations = reader.ReadUInt32();
            sectionHeader.PointerToLinenumbers = reader.ReadUInt32();
            sectionHeader.NumberOfRelocations = reader.ReadUInt16();
            sectionHeader.NumberOfLinenumbers = reader.ReadUInt16();
            sectionHeader.Characteristics = reader.ReadUInt32();

            rawPeFile.SectionHeaders[i] = sectionHeader;
        }
    }

    /// <summary>
    /// Parses all the additional PE directories
    /// </summary>
    private void ParseDirectories(BinaryReader reader, RawPeFile rawPeFile)
    {
        // Create a directory parser
        var directoryParser = new RawPeDirectoryParser(rawPeFile);

        // Parse each directory
        rawPeFile.ExportDirectory = directoryParser.ParseExportDirectory(reader);
        rawPeFile.ImportDescriptors = directoryParser.ParseImportDescriptors(reader);
        rawPeFile.ResourceDirectory = directoryParser.ParseResourceDirectory(reader);
        rawPeFile.ExceptionDirectory = directoryParser.ParseExceptionDirectory(reader);
        rawPeFile.SecurityDirectory = directoryParser.ParseSecurityDirectory(reader);
        rawPeFile.BaseRelocationDirectory = directoryParser.ParseBaseRelocationDirectory(reader);
        rawPeFile.DebugDirectory = directoryParser.ParseDebugDirectory(reader);
        // Architecture directory is not commonly used
        rawPeFile.ArchitectureDirectory = Array.Empty<byte>();
        // Global pointer directory is not commonly used
        rawPeFile.GlobalPointerDirectory = 0;
        rawPeFile.TlsDirectory = directoryParser.ParseTlsDirectory(reader);
        rawPeFile.LoadConfigDirectory = directoryParser.ParseLoadConfigDirectory(reader);
        rawPeFile.BoundImportDirectory = directoryParser.ParseBoundImportDirectory(reader);
        // Import address table is just an RVA, no structure to parse
        if (rawPeFile.OptionalHeader.DataDirectories.Length > 12)
        {
            rawPeFile.ImportAddressTableDirectory = rawPeFile.OptionalHeader.DataDirectories[12].VirtualAddress;
        }

        rawPeFile.DelayImportDirectory = directoryParser.ParseDelayImportDirectory(reader);
        rawPeFile.ClrDirectory = directoryParser.ParseClrDirectory(reader);
    }
}
