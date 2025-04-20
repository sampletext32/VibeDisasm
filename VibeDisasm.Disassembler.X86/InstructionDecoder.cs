using System.Diagnostics.Contracts;
using VibeDisasm.Disassembler.X86.Handlers;
using VibeDisasm.Disassembler.X86.Operands;

namespace VibeDisasm.Disassembler.X86;

public static class Printer
{
    public static Action<string>? WriteLine;
}

/// <summary>
/// Decodes x86 instructions from a byte buffer
/// </summary>
public class InstructionDecoder
{
    // The buffer containing the code to decode
    private readonly byte[] _codeBuffer;

    // The length of the buffer
    private readonly int _length;

    // The current position in the buffer
    private uint _position;

    // The instruction handler factory
    private readonly InstructionHandlerFactory _handlerFactory;

    // Specialized decoders
    private readonly PrefixDecoder _prefixDecoder;

    /// <summary>
    /// Initializes a new instance of the InstructionDecoder class
    /// </summary>
    /// <param name="codeBuffer">The buffer containing the code to decode</param>
    /// <param name="length">The length of the buffer</param>
    public InstructionDecoder(byte[] codeBuffer, int length)
    {
        _codeBuffer = codeBuffer;
        _length = length;
        _position = 0;

        // Create specialized decoders
        _prefixDecoder = new PrefixDecoder();

        // Create the instruction handler factory
        _handlerFactory = new InstructionHandlerFactory(this);
    }

    /// <summary>
    /// Decodes an instruction at the current position
    /// </summary>
    /// <returns>The decoded instruction, or null if the decoding failed</returns>
    public Instruction? DecodeInstruction()
    {
        if (!CanReadByte())
        {
            return null;
        }

        // Reset prefix flags
        _prefixDecoder.Reset();

        // Save the start position of the instruction
        uint startPosition = _position;

        // Create a new instruction
        Instruction instruction = new Instruction
        {
            Address = startPosition,
        };

        // Handle prefixes
        while (CanReadByte())
        {
            byte prefix = _codeBuffer[_position];
            
            if (_prefixDecoder.DecodePrefix(prefix))
            {
                _position++;
            }
            else
            {
                break;
            }
        }
        
        // If only prefixes were found and we're at the end of the buffer, return null
        if (_position > startPosition && !CanReadByte())
        {
            return null;
        }
        
        // Read the opcode
        byte opcode = ReadByte();
        
        // Get a handler for the opcode
        var handler = _handlerFactory.GetHandler(opcode);

        Printer.WriteLine?.Invoke($"Resolved handler {handler?.GetType().Name}");

        bool handlerSuccess;

        // Try to decode with a handler first
        if (handler != null)
        {
            // Store the current segment override state
            bool hasSegmentOverride = _prefixDecoder.HasSegmentOverridePrefix();
            string segmentOverride = _prefixDecoder.GetSegmentOverride();

            // Save the position before decoding

            // Decode the instruction
            handlerSuccess = handler.Decode(opcode, instruction);

            if (!handlerSuccess)
            {
                Printer.WriteLine?.Invoke($"Handler {handler.GetType().Name} failed!");
            }

            instruction.Length = GetPosition() - instruction.Address;

            // Apply segment override prefix to the structured operands if needed
            if (handlerSuccess && hasSegmentOverride)
            {
                // Apply segment override to memory operands
                foreach (var operand in instruction.StructuredOperands)
                {
                    if (operand is MemoryOperand memoryOperand)
                    {
                        memoryOperand.SegmentOverride = segmentOverride;
                    }
                }
            }
        }
        else
        {
            instruction.Type = InstructionType.Unknown;
            instruction.StructuredOperands = [];
            handlerSuccess = true;
        }

        // If no handler is found or decoding fails, create a default instruction
        if (!handlerSuccess)
        {
            instruction.Type = InstructionType.Unknown;
            instruction.StructuredOperands = [];
        }

        // Apply REP/REPNE prefix to the instruction type if needed
        if (_prefixDecoder.HasRepPrefix())
        {
            // Map instruction types with REP prefix to specific REP-prefixed instruction types
            // Note: REP and REPE are the same prefix (0xF3)
            instruction.Type = instruction.Type switch
            {
                InstructionType.MovsB => InstructionType.RepMovsB,
                InstructionType.MovsW => InstructionType.RepMovsW,
                InstructionType.MovsD => InstructionType.RepMovsD,
                InstructionType.StosB => InstructionType.RepStosB,
                InstructionType.StosW => InstructionType.RepStosW,
                InstructionType.StosD => InstructionType.RepStosD,
                InstructionType.LodsB => InstructionType.RepLodsB,
                InstructionType.LodsW => InstructionType.RepLodsW,
                InstructionType.LodsD => InstructionType.RepLodsD,
                InstructionType.ScasB => InstructionType.RepScasB,
                InstructionType.ScasW => InstructionType.RepScasW,
                InstructionType.ScasD => InstructionType.RepScasD,
                InstructionType.CmpsB => InstructionType.RepeCmpsB,
                InstructionType.CmpsW => InstructionType.RepeCmpsW,
                InstructionType.CmpsD => InstructionType.RepeCmpsD,
                _ => instruction.Type // Keep original type for other instructions
            };
        }
        else if (_prefixDecoder.HasRepnePrefix())
        {
            // Map instruction types with REPNE prefix to specific REPNE-prefixed instruction types
            instruction.Type = instruction.Type switch
            {
                InstructionType.StosB => InstructionType.RepneStosB,
                InstructionType.StosW => InstructionType.RepneStosW,
                InstructionType.StosD => InstructionType.RepneStosD,
                InstructionType.ScasB => InstructionType.RepneScasB,
                InstructionType.ScasW => InstructionType.RepneScasW,
                InstructionType.ScasD => InstructionType.RepneScasD,
                InstructionType.CmpsB => InstructionType.RepneCmpsB,
                InstructionType.CmpsW => InstructionType.RepneCmpsW,
                InstructionType.CmpsD => InstructionType.RepneCmpsD,
                _ => instruction.Type // Keep original type for other instructions
            };
        }
        
        return instruction;
    }

    /// <summary>
    /// Gets the current position in the buffer
    /// </summary>
    /// <returns>The current position</returns>
    [Pure]
    public uint GetPosition()
    {
        return _position;
    }

    /// <summary>
    /// Sets the current position in the buffer
    /// </summary>
    /// <param name="position">The new position</param>
    public void SetPosition(uint position)
    {
        _position = position;
    }

    /// <summary>
    /// Checks if the operand size prefix is present
    /// </summary>
    /// <returns>True if the operand size prefix is present</returns>
    public bool HasOperandSizePrefix()
    {
        return _prefixDecoder.HasOperandSizePrefix();
    }

    /// <summary>
    /// Checks if the instruction has an operand size override prefix (0x66)
    /// </summary>
    /// <returns>True if the instruction has an operand size override prefix</returns>
    public bool HasOperandSizeOverridePrefix()
    {
        return _prefixDecoder.HasOperandSizePrefix();
    }

    /// <summary>
    /// Checks if a single byte can be read from the current position
    /// </summary>
    /// <returns>True if there is at least one byte available to read</returns>
    public bool CanReadByte()
    {
        return _position < _length;
    }

    /// <summary>
    /// Checks if a 16-bit unsigned short (2 bytes) can be read from the current position
    /// </summary>
    /// <returns>True if there are at least two bytes available to read</returns>
    public bool CanReadUShort()
    {
        return _position + 1 < _length;
    }

    /// <summary>
    /// Checks if a 32-bit unsigned integer (4 bytes) can be read from the current position
    /// </summary>
    /// <returns>True if there are at least four bytes available to read</returns>
    public bool CanReadUInt()
    {
        return _position + 3 < _length;
    }

    /// <summary>
    /// Peaks a byte from the buffer without adjusting position
    /// </summary>
    /// <returns>The byte peaked</returns>
    public byte PeakByte()
    {
        if (_position >= _length)
        {
            return 0;
        }

        return _codeBuffer[_position];
    }

    /// <summary>
    /// Peaks a byte from the buffer without adjusting position
    /// </summary>
    /// <returns>The byte peaked</returns>
    public (byte b1, byte b2) PeakTwoBytes()
    {
        if (_position + 1 >= _length)
        {
            return (0,0);
        }

        return (_codeBuffer[_position], _codeBuffer[_position + 1]);
    }
    
    /// <summary>
    /// Peaks a byte from the buffer at the specified offset from current position without adjusting position
    /// </summary>
    /// <param name="offset">The offset from the current position</param>
    /// <returns>The byte peaked</returns>
    public byte PeakByte(uint offset)
    {
        uint targetPosition = _position + offset;
        
        if (targetPosition >= _length || targetPosition < 0)
        {
            return 0;
        }

        return _codeBuffer[targetPosition];
    }

    /// <summary>
    /// Reads a byte from the buffer and advances the position
    /// </summary>
    /// <returns>The byte read</returns>
    public byte ReadByte()
    {
        if (_position >= _length)
        {
            return 0;
        }

        return _codeBuffer[_position++];
    }

    /// <summary>
    /// Reads a 16-bit value from the buffer and advances the position
    /// </summary>
    /// <returns>The 16-bit value read</returns>
    public ushort ReadUInt16()
    {
        if (_position + 1 >= _length)
        {
            return 0;
        }

        ushort value = (ushort) (_codeBuffer[_position] | (_codeBuffer[_position + 1] << 8));
        _position += 2;
        return value;
    }

    /// <summary>
    /// Reads a 32-bit value from the buffer and advances the position
    /// </summary>
    /// <returns>The 32-bit value read</returns>
    public uint ReadUInt32()
    {
        if (_position + 3 >= _length)
        {
            return 0;
        }

        uint value = (uint) (_codeBuffer[_position] |
                             (_codeBuffer[_position + 1] << 8) |
                             (_codeBuffer[_position + 2] << 16) |
                             (_codeBuffer[_position + 3] << 24));
        _position += 4;
        return value;
    }

    public bool CanRead(int expectedBytes)
    {
        return _position + expectedBytes - 1 < _length;
    }
}