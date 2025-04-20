namespace VibeDisasm.Pe.Extractors;

/// <summary>
/// Contains basic information about a PE file
/// </summary>
public class PeInfo
{
    /// <summary>
    /// Gets or sets whether the PE file is 64-bit (PE32+)
    /// </summary>
    public bool Is64Bit { get; set; }
    
    /// <summary>
    /// Gets or sets the address of the entry point
    /// </summary>
    public uint EntryPointRva { get; set; }
    
    /// <summary>
    /// Gets or sets the number of sections in the PE file
    /// </summary>
    public ushort NumberOfSections { get; set; }
    
    /// <summary>
    /// Gets or sets the characteristics of the PE file
    /// </summary>
    public ushort Characteristics { get; set; }
    
    /// <summary>
    /// Gets or sets the subsystem of the PE file
    /// </summary>
    public ushort Subsystem { get; set; }
    
    /// <summary>
    /// Gets or sets the DLL characteristics of the PE file
    /// </summary>
    public ushort DllCharacteristics { get; set; }
    
    /// <summary>
    /// Gets or sets the size of the image
    /// </summary>
    public uint SizeOfImage { get; set; }
    
    /// <summary>
    /// Gets or sets the size of the headers
    /// </summary>
    public uint SizeOfHeaders { get; set; }
    
    /// <summary>
    /// Gets or sets the checksum of the PE file
    /// </summary>
    public uint CheckSum { get; set; }
    
    /// <summary>
    /// Gets whether the PE file is a DLL
    /// </summary>
    public bool IsDll => (Characteristics & 0x2000) != 0;
    
    /// <summary>
    /// Gets whether the PE file is an executable
    /// </summary>
    public bool IsExecutable => (Characteristics & 0x0002) != 0;
    
    /// <summary>
    /// Gets whether the PE file is a system file
    /// </summary>
    public bool IsSystemFile => (Characteristics & 0x1000) != 0;
}
