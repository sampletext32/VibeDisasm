namespace VibeDisasm.Disassembler.X86;

/// <summary>
/// Handles decoding of instruction prefixes
/// </summary>
public class PrefixDecoder
{
    // Prefix flags
    private bool _operandSizePrefix;
    private bool _addressSizePrefix;
    private bool _segmentOverridePrefix;
    private bool _lockPrefix;
    private bool _repPrefix;
    private bool _repnePrefix;
    private Segment? _segmentOverride = null;
    
    /// <summary>
    /// Initializes a new instance of the PrefixDecoder class
    /// </summary>
    public PrefixDecoder()
    {
        Reset();
    }
    
    /// <summary>
    /// Resets all prefix flags
    /// </summary>
    public void Reset()
    {
        _operandSizePrefix = false;
        _addressSizePrefix = false;
        _segmentOverridePrefix = false;
        _lockPrefix = false;
        _repPrefix = false;
        _repnePrefix = false;
        _segmentOverride = null;
    }
    
    /// <summary>
    /// Decodes a prefix byte
    /// </summary>
    /// <param name="prefix">The prefix byte</param>
    /// <returns>True if the byte was a prefix, false otherwise</returns>
    public bool DecodePrefix(byte prefix)
    {
        if (prefix == 0x66) // Operand size prefix
        {
            _operandSizePrefix = true;
            return true;
        }
        else if (prefix == 0x67) // Address size prefix
        {
            _addressSizePrefix = true;
            return true;
        }
        else if ((prefix >= 0x26 && prefix <= 0x3E && (prefix & 0x7) == 0x6) || prefix == 0x64 || prefix == 0x65) // Segment override prefix
        {
            _segmentOverridePrefix = true;
            switch (prefix)
            {
                case 0x26: _segmentOverride = Segment.Es; break;
                case 0x2E: _segmentOverride = Segment.Cs; break;
                case 0x36: _segmentOverride = Segment.Ss; break;
                case 0x3E: _segmentOverride = Segment.Ds; break;
                case 0x64: _segmentOverride = Segment.Fs; break;
                case 0x65: _segmentOverride = Segment.Gs; break;
            }
            return true;
        }
        else if (prefix == 0xF0) // LOCK prefix
        {
            _lockPrefix = true;
            return true;
        }
        else if (prefix == 0xF3) // REP prefix
        {
            _repPrefix = true;
            return true;
        }
        else if (prefix == 0xF2) // REPNE prefix
        {
            _repnePrefix = true;
            return true;
        }
        
        return false;
    }
    
    /// <summary>
    /// Checks if the operand size prefix is present
    /// </summary>
    /// <returns>True if the operand size prefix is present</returns>
    public bool HasOperandSizePrefix()
    {
        return _operandSizePrefix;
    }
    
    /// <summary>
    /// Checks if the address size prefix is present
    /// </summary>
    /// <returns>True if the address size prefix is present</returns>
    public bool HasAddressSizePrefix()
    {
        return _addressSizePrefix;
    }
    
    /// <summary>
    /// Checks if a segment override prefix is present
    /// </summary>
    /// <returns>True if a segment override prefix is present</returns>
    public bool HasSegmentOverridePrefix()
    {
        return _segmentOverridePrefix;
    }
    
    /// <summary>
    /// Gets the segment override prefix
    /// </summary>
    /// <returns>The segment override prefix, or an empty string if none is present</returns>
    public Segment? GetSegmentOverride()
    {
        return _segmentOverride;
    }
    
    /// <summary>
    /// Checks if the LOCK prefix is present
    /// </summary>
    /// <returns>True if the LOCK prefix is present</returns>
    public bool HasLockPrefix()
    {
        return _lockPrefix;
    }
    
    /// <summary>
    /// Checks if the REP prefix is present
    /// </summary>
    /// <returns>True if the REP prefix is present</returns>
    public bool HasRepPrefix()
    {
        return _repPrefix;
    }
    
    /// <summary>
    /// Checks if the REPNE prefix is present
    /// </summary>
    /// <returns>True if the REPNE prefix is present</returns>
    public bool HasRepnePrefix()
    {
        return _repnePrefix;
    }
}
