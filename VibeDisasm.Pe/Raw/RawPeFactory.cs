namespace VibeDisasm.Pe.Raw;

/// <summary>
/// Factory for creating RawPeFile instances
/// </summary>
public static class RawPeFactory
{
    /// <summary>
    /// Creates a RawPeFile from a file path
    /// </summary>
    /// <param name="filePath">The path to the PE file</param>
    /// <returns>The parsed raw PE file</returns>
    public static RawPeFile FromFile(string filePath)
    {
        if (string.IsNullOrEmpty(filePath))
        {
            throw new ArgumentNullException(nameof(filePath));
        }

        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException("PE file not found", filePath);
        }

        var fileData = File.ReadAllBytes(filePath);
        return FromBytes(fileData);
    }

    /// <summary>
    /// Creates a RawPeFile from a byte array
    /// </summary>
    /// <param name="fileData">The raw file data</param>
    /// <returns>The parsed raw PE file</returns>
    public static RawPeFile FromBytes(byte[] fileData)
    {
        if (fileData == null || fileData.Length == 0)
        {
            throw new ArgumentException("File data is null or empty", nameof(fileData));
        }

        var parser = new RawPeParser();
        return parser.Parse(fileData);
    }

    /// <summary>
    /// Creates a RawPeFile from a stream
    /// </summary>
    /// <param name="stream">The stream containing the PE file data</param>
    /// <returns>The parsed raw PE file</returns>
    public static RawPeFile FromStream(Stream stream)
    {
        if (stream == null)
        {
            throw new ArgumentNullException(nameof(stream));
        }

        if (!stream.CanRead)
        {
            throw new ArgumentException("Stream is not readable", nameof(stream));
        }

        using var memoryStream = new MemoryStream();
        stream.CopyTo(memoryStream);
        return FromBytes(memoryStream.ToArray());
    }
}
