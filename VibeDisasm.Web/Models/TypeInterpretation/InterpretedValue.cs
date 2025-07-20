using System.Buffers.Binary;
using System.Diagnostics;
using System.Text;

namespace VibeDisasm.Web.Models.TypeInterpretation;

public interface IInterpretedValue
{
    public static virtual Endianness DefaultEndianness => Endianness.LittleEndian;

    public static abstract T Create<T>(Memory<byte> sourceMemory, Endianness endianness)
        where T : class, IInterpretedValue;
}

public abstract class InterpretedValue(Memory<byte> sourceMemory, Endianness endianness)
{
    public Memory<byte> SourceMemory { get; set; } = sourceMemory;
    public Endianness Endianness { get; set; } = endianness;

    public int Size => SourceMemory.Length;
    public abstract string DebugDisplay { get; }

    public abstract T Reinterpret<T>() where T : InterpretedRawValue, IInterpretedValue;
}

[DebuggerDisplay("{DebugDisplay}")]
public class InterpretedRawValue(Memory<byte> sourceMemory, Endianness endianness)
    : InterpretedValue(sourceMemory, endianness)
{
    public override string DebugDisplay => SourceMemory.Length <= 20
        ? $"[{SourceMemory.MemoryString()}]"
        : $"[{SourceMemory[..20].MemoryString()}, ...{SourceMemory.Length - 20} more bytes]";

    public override T Reinterpret<T>() => typeof(T) == this.GetType() ? (this as T)! : T.Create<T>(SourceMemory, T.DefaultEndianness);
}

[DebuggerDisplay("{DebugDisplay}")]
public class InterpretedSignedInteger(Memory<byte> sourceMemory, Endianness endianness)
    : InterpretedRawValue(sourceMemory, endianness), IInterpretedValue
{
    public long Value => Size switch
    {
        1 => (sbyte)SourceMemory.Span[0],
        2 => Endianness is Endianness.LittleEndian
            ? BinaryPrimitives.ReadInt16LittleEndian(SourceMemory.Span)
            : BinaryPrimitives.ReadInt16BigEndian(SourceMemory.Span),
        4 => Endianness is Endianness.LittleEndian
            ? BinaryPrimitives.ReadInt32LittleEndian(SourceMemory.Span)
            : BinaryPrimitives.ReadInt32BigEndian(SourceMemory.Span),
        8 => Endianness is Endianness.LittleEndian
            ? BinaryPrimitives.ReadInt64LittleEndian(SourceMemory.Span)
            : BinaryPrimitives.ReadInt64BigEndian(SourceMemory.Span),
        _ => throw new ArgumentOutOfRangeException()
    };

    public override string DebugDisplay => $"{Value} [{SourceMemory.MemoryString()}]";

    public static T Create<T>(Memory<byte> sourceMemory, Endianness endianness) where T : class, IInterpretedValue =>
        (new InterpretedSignedInteger(sourceMemory, endianness) as T)!;
    public long Get() => Value;
}

[DebuggerDisplay("{DebugDisplay}")]
public class InterpretedUnsignedInteger(Memory<byte> sourceMemory, Endianness endianness)
    : InterpretedRawValue(sourceMemory, endianness), IInterpretedValue
{
    public ulong Value => Size switch
    {
        1 => SourceMemory.Span[0],
        2 => Endianness is Endianness.LittleEndian
            ? BinaryPrimitives.ReadUInt16LittleEndian(SourceMemory.Span)
            : BinaryPrimitives.ReadUInt16BigEndian(SourceMemory.Span),
        4 => Endianness is Endianness.LittleEndian
            ? BinaryPrimitives.ReadUInt32LittleEndian(SourceMemory.Span)
            : BinaryPrimitives.ReadUInt32BigEndian(SourceMemory.Span),
        8 => Endianness is Endianness.LittleEndian
            ? BinaryPrimitives.ReadUInt64LittleEndian(SourceMemory.Span)
            : BinaryPrimitives.ReadUInt64BigEndian(SourceMemory.Span),
        _ => throw new ArgumentOutOfRangeException()
    };

    public override string DebugDisplay => $"{Value} [{SourceMemory.MemoryString()}]";

    public static T Create<T>(Memory<byte> sourceMemory, Endianness endianness) where T : class, IInterpretedValue =>
        (new InterpretedUnsignedInteger(sourceMemory, endianness) as T)!;

    public ulong Get() => Value;
}

[DebuggerDisplay("{DebugDisplay}")]
public class InterpretedFloat(Memory<byte> sourceMemory, Endianness endianness)
    : InterpretedRawValue(sourceMemory, endianness), IInterpretedValue
{
    public float Value => BinaryPrimitives.ReadSingleLittleEndian(SourceMemory.Span);

    public override string DebugDisplay => $"{Value} [{SourceMemory.MemoryString()}]";

    public static T Create<T>(Memory<byte> sourceMemory, Endianness endianness) where T : class, IInterpretedValue =>
        (new InterpretedFloat(sourceMemory, endianness) as T)!;
    public float Get() => Value;
}

[DebuggerDisplay("{DebugDisplay}")]
public class InterpretedDouble(Memory<byte> sourceMemory, Endianness endianness)
    : InterpretedRawValue(sourceMemory, endianness), IInterpretedValue
{
    public double Value => Endianness is Endianness.LittleEndian
        ? BinaryPrimitives.ReadDoubleLittleEndian(SourceMemory.Span)
        : BinaryPrimitives.ReadDoubleBigEndian(SourceMemory.Span);

    public override string DebugDisplay => $"{Value} [{SourceMemory.MemoryString()}]";
    public static T Create<T>(Memory<byte> sourceMemory, Endianness endianness) where T : class, IInterpretedValue =>
        (new InterpretedDouble(sourceMemory, endianness) as T)!;
    public double Get() => Value;
}

[DebuggerDisplay("{DebugDisplay}")]
public class InterpretedBoolean(Memory<byte> sourceMemory, Endianness endianness)
    : InterpretedRawValue(sourceMemory, endianness), IInterpretedValue
{
    public bool Value => Endianness is Endianness.LittleEndian
        ? BinaryPrimitives.ReadUInt64LittleEndian(SourceMemory.Span) != 0
        : BinaryPrimitives.ReadUInt64BigEndian(SourceMemory.Span) != 0;

    public override string DebugDisplay => $"{(Value ? "False" : "True")} [{SourceMemory.MemoryString()}]";
    public static T Create<T>(Memory<byte> sourceMemory, Endianness endianness) where T : class, IInterpretedValue =>
        (new InterpretedBoolean(sourceMemory, endianness) as T)!;
    public bool Get() => Value;
}

[DebuggerDisplay("{DebugDisplay}")]
public class InterpretedAsciiString(Memory<byte> sourceMemory)
    : InterpretedRawValue(sourceMemory, Endianness.LittleEndian), IInterpretedValue
{
    public string Value => SourceMemory.Span.IndexOf((byte)0) == -1
        ? Encoding.ASCII.GetString(SourceMemory.Span)
        : Encoding.ASCII.GetString(SourceMemory.Span[..SourceMemory.Span.IndexOf((byte)0)]);

    public override string DebugDisplay => $"{Value} [{SourceMemory.MemoryString()}]";
    public static T Create<T>(Memory<byte> sourceMemory, Endianness endianness) where T : class, IInterpretedValue =>
        (new InterpretedAsciiString(sourceMemory) as T)!;
    public string Get() => Value;
}

[DebuggerDisplay("{DebugDisplay}")]
public class InterpretedWideString(Memory<byte> sourceMemory)
    : InterpretedRawValue(sourceMemory, Endianness.LittleEndian), IInterpretedValue
{
    public string Value
    {
        get
        {
            var length = 0;
            while (length < SourceMemory.Length - 1)
            {
                if (SourceMemory.Span[length] == 0 && SourceMemory.Span[length + 1] == 0)
                {
                    break;
                }

                length += 2;
            }

            if (length > SourceMemory.Length)
            {
                length = SourceMemory.Length - (SourceMemory.Length % 2);
            }

            var value = Encoding.Unicode.GetString(SourceMemory.Span[..length]);
            return value;
        }
    }

    public override string DebugDisplay => $"{Value} [{SourceMemory.MemoryString()}]";
    public static T Create<T>(Memory<byte> sourceMemory, Endianness endianness) where T : class, IInterpretedValue =>
        (new InterpretedWideString(sourceMemory) as T)!;

    public string Get() => Value;
}
