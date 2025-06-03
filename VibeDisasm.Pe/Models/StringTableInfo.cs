namespace VibeDisasm.Pe.Models
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
        public Dictionary<ushort, string> Strings { get; set; } = new();

        /// <summary>
        /// Gets or sets the absolute file offset of the string table
        /// </summary>
        public uint FileOffset { get; set; }

        /// <summary>
        /// Gets or sets a dictionary mapping string IDs to their file offsets
        /// </summary>
        public Dictionary<ushort, uint> StringFileOffsets { get; set; } = new();
    }
}
