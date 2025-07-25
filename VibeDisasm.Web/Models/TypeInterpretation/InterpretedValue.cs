using System.Buffers.Binary;
using System.Diagnostics;
using System.Text;

namespace VibeDisasm.Web.Models.TypeInterpretation;

public interface IInterpretedValue
{
    static virtual Endianness DefaultEndianness => Endianness.LittleEndian;

    static abstract T Create<T>(Memory<byte> sourceMemory, Endianness endianness)
        where T : class, IInterpretedValue;
}

public abstract class InterpretedValue(Memory<byte> sourceMemory, Endianness endianness)
{
    public Memory<byte> SourceMemory { get; set; } = sourceMemory;
    public Endianness Endianness { get; set; } = endianness;

    public int Size => SourceMemory.Length;
    internal abstract string DebugDisplay { get; }

    public abstract T Reinterpret<T>() where T : InterpretedRawValue, IInterpretedValue;
}

public class InterpretedRawValue(Memory<byte> sourceMemory, Endianness endianness)
    : InterpretedValue(sourceMemory, endianness)
{
    internal override string DebugDisplay => $"[{SourceMemory.MemoryString()}]";

    public override T Reinterpret<T>() =>
        typeof(T) == this.GetType() ? (this as T)! : T.Create<T>(SourceMemory, T.DefaultEndianness);
}

[DebuggerDisplay("{DebugDisplay}")]
public class InterpretedArrayValue(Memory<byte> sourceMemory, List<InterpretedValue> values)
    : InterpretedRawValue(sourceMemory, Endianness.LittleEndian), IInterpretedValue
{
    public List<InterpretedValue> Values { get; set; } = values;

    internal override string DebugDisplay =>
        $"[{string.Join(',', Values.Select(x => x.DebugDisplay))}]";

    public static T Create<T>(Memory<byte> sourceMemory, Endianness endianness) where T : class, IInterpretedValue =>
        throw new Exception("Attempted to Reinterpret array. Arrays should not be reinterpreted.");
}

[DebuggerDisplay("{DebugDisplay}")]
public class InterpretedStructValue(Memory<byte> sourceMemory, string name, List<InterpretedStructField> fields)
    : InterpretedRawValue(sourceMemory, Endianness.LittleEndian), IInterpretedValue
{
    public string Name { get; } = name;
    public List<InterpretedStructField> Fields { get; set; } = fields;

    internal override string DebugDisplay =>
        $"struct {Name} {{{Fields.Count} fields}}";

    public static T Create<T>(Memory<byte> sourceMemory, Endianness endianness) where T : class, IInterpretedValue =>
        throw new Exception("Attempted to Reinterpret struct. Structs should not be reinterpreted.");
}

[DebuggerDisplay("{DebugDisplay}")]
public class InterpretedStructField(Memory<byte> sourceMemory, string name, InterpretedValue value)
    : InterpretedRawValue(sourceMemory, Endianness.LittleEndian), IInterpretedValue
{
    public string Name { get; } = name;
    public InterpretedValue Value { get; set; } = value;

    internal override string DebugDisplay =>
        $"{Name} = {Value.DebugDisplay}";

    public static T Create<T>(Memory<byte> sourceMemory, Endianness endianness) where T : class, IInterpretedValue =>
        throw new Exception("Attempted to Reinterpret struct field. Struct fields should not be reinterpreted.");
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

    internal override string DebugDisplay => $"{Value} [{SourceMemory.MemoryString()}]";

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

    internal override string DebugDisplay => $"{Value} [{SourceMemory.MemoryString()}]";

    public static T Create<T>(Memory<byte> sourceMemory, Endianness endianness) where T : class, IInterpretedValue =>
        (new InterpretedUnsignedInteger(sourceMemory, endianness) as T)!;

    public ulong Get() => Value;
}

[DebuggerDisplay("{DebugDisplay}")]
public class InterpretedFloat(Memory<byte> sourceMemory, Endianness endianness)
    : InterpretedRawValue(sourceMemory, endianness), IInterpretedValue
{
    public float Value => Endianness == Endianness.LittleEndian
        ? BinaryPrimitives.ReadSingleLittleEndian(SourceMemory.Span)
        : BinaryPrimitives.ReadSingleBigEndian(SourceMemory.Span);

    internal override string DebugDisplay => $"{Value} [{SourceMemory.MemoryString()}]";

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

    internal override string DebugDisplay => $"{Value} [{SourceMemory.MemoryString()}]";

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

    internal override string DebugDisplay => $"{(Value ? "False" : "True")} [{SourceMemory.MemoryString()}]";

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

    internal override string DebugDisplay => $"{Value} [{SourceMemory.MemoryString()}]";

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

    internal override string DebugDisplay => $"{Value} [{SourceMemory.MemoryString()}]";

    public static T Create<T>(Memory<byte> sourceMemory, Endianness endianness) where T : class, IInterpretedValue =>
        (new InterpretedWideString(sourceMemory) as T)!;

    public string Get() => Value;
}
