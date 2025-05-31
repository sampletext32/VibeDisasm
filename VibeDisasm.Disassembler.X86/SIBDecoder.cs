using VibeDisasm.Disassembler.X86.Operands;

namespace VibeDisasm.Disassembler.X86;

/// <summary>
/// Handles decoding of SIB (Scale-Index-Base) bytes in x86 instructions
/// </summary>
public class SIBDecoder
{
    private readonly InstructionDecoder _decoder;

    /// <summary>
    /// Initializes a new instance of the SIBDecoder class
    /// </summary>
    /// <param name="decoder">The instruction decoder that owns this SIB decoder</param>
    public SIBDecoder(InstructionDecoder decoder)
    {
        _decoder = decoder;
    }

    /// <summary>
    /// Decodes a SIB byte
    /// </summary>
    /// <param name="sib">The SIB byte</param>
    /// <param name="displacement">The displacement value</param>
    /// <param name="operandSize">The size of the operand in bits (8, 16, 32, or 64)</param>
    /// <param name="mod">The mod field from the ModR/M byte</param>
    /// <returns>The decoded SIB operand</returns>
    public Operand DecodeSIB(byte sib, uint displacement, int operandSize, byte mod = 0)
    {
        // Extract fields from SIB byte
        var scale = (byte)((sib & Constants.SIB_SCALE_MASK) >> 6);
        var index = (RegisterIndex)((sib & Constants.SIB_INDEX_MASK) >> 3);
        var @base = (RegisterIndex)(sib & Constants.SIB_BASE_MASK);

        // Calculate scale factor once
        var scaleFactor = 1 << scale; // 1, 2, 4, or 8

        // Handle special case: no index register (index = ESP/SP)
        if (index == RegisterIndex.Sp)
        {
            // Special case: [EBP] with mod=00 means [disp32]
            if (@base == RegisterIndex.Bp && mod == 0)
            {
                var disp32 = _decoder.CanReadUInt() ? _decoder.ReadUInt32() : 0;
                return OperandFactory.CreateDirectMemoryOperand(disp32, operandSize);
            }

            // Simple base + displacement
            return OperandFactory.CreateDisplacementMemoryOperand(@base, (int)displacement, operandSize);
        }

        // Handle special case: no base register (base = EBP with mod=00)
        if (@base == RegisterIndex.Bp && mod == 0)
        {
            var disp32 = _decoder.CanReadUInt() ? _decoder.ReadUInt32() : 0;
            return OperandFactory.CreateScaledIndexMemoryOperand(
                index, scaleFactor, null, (int)disp32, operandSize);
        }

        // Normal case: base + (index * scale) + displacement
        return OperandFactory.CreateScaledIndexMemoryOperand(
            index, scaleFactor, @base, (int)displacement, operandSize);
    }
}
