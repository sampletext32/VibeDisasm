using System.Text;
using VibeDisasm.Pe.Raw;

namespace VibeDisasm.Pe.Extractors;

public static class Util
{
    /// <summary>
    /// Converts a Relative Virtual Address (RVA) to a file offset
    /// </summary>
    /// <param name="rawPeFile">The raw PE file</param>
    /// <param name="rva">The RVA to convert</param>
    /// <returns>The corresponding file offset</returns>
    public static uint RvaToOffset(RawPeFile rawPeFile, uint rva)
    {
        // Find the section containing the RVA
        foreach (var section in rawPeFile.SectionHeaders)
        {
            var sectionStart = section.VirtualAddress;
            var sectionEnd = sectionStart + Math.Max(section.VirtualSize, section.SizeOfRawData);

            if (rva >= sectionStart && rva < sectionEnd)
            {
                // Calculate the offset within the section
                var offset = rva - sectionStart + section.PointerToRawData;
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

    /// <summary>
    /// Reads a null-terminated ASCII string from the specified offset
    /// </summary>
    /// <param name="data">The raw data</param>
    /// <param name="offset">The offset of the string</param>
    /// <returns>The string read from the offset</returns>
    public static string ReadAsciiString(byte[] data, uint offset)
    {
        var length = 0;
        while (offset + length < data.Length && data[offset + length] != 0)
        {
            length++;
        }

        return Encoding.ASCII.GetString(data, (int)offset, length);
    }

    public static string ReadNullTerminatedUnicodeString(this BinaryReader reader)
    {
        var bytes = new List<byte>();
        var buffer = new byte[2];

        while (true)
        {
            buffer[0] = reader.ReadByte();
            buffer[1] = reader.ReadByte();

            if (buffer[0] == 0 && buffer[1] == 0)
            {
                // Null terminator reached
                break;
            }

            bytes.Add(buffer[0]);
            bytes.Add(buffer[1]);
        }

        return Encoding.Unicode.GetString(bytes.ToArray());
    }

    public static string ReadFixedLengthUnicodeString(this BinaryReader reader, ushort length)
    {
        var data = reader.ReadBytes(length * 2); // Unicode: 2 bytes per char
        return Encoding.Unicode.GetString(data);
    }
}
