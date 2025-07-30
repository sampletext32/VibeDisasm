using System.Collections;
using System.Diagnostics;
using VibeDisasm.Web.Models.Types;

namespace VibeDisasm.Web.Models.TypeInterpretation;

public abstract class InterpretValue2(IRuntimeDatabaseType underlyingType, Memory<byte> sourceMemory)
{
    internal abstract string DebugDisplay { get; }
    public IRuntimeDatabaseType UnderlyingType { get; init; } = underlyingType;
    public Memory<byte> SourceMemory { get; init; } = sourceMemory;
}

[DebuggerDisplay("{DebugDisplay}")]
public class InterpretValue2<TValue>(IRuntimeDatabaseType underlyingType, Memory<byte> sourceMemory, TValue value)
    : InterpretValue2(underlyingType, sourceMemory)
{
    internal override string DebugDisplay => $"{UnderlyingType.Name} {Value}";
    public TValue Value { get; } = value;
}

public class InterpretValue2Raw(
    IRuntimeDatabaseType underlyingType,
    Memory<byte> sourceMemory
) : InterpretValue2(underlyingType, sourceMemory)
{
    internal override string DebugDisplay => SourceMemory.MemoryString();
}

public class InterpretValue2Char(
    IRuntimeDatabaseType underlyingType,
    Memory<byte> sourceMemory,
    char value
) : InterpretValue2<char>(underlyingType, sourceMemory, value);

public class InterpretValue2Bool(
    IRuntimeDatabaseType underlyingType,
    Memory<byte> sourceMemory,
    bool value
) : InterpretValue2<bool>(underlyingType, sourceMemory, value);

public class InterpretValue2U1(
    IRuntimeDatabaseType underlyingType,
    Memory<byte> sourceMemory,
    byte value
) : InterpretValue2<byte>(underlyingType, sourceMemory, value);

public class InterpretValue2U2(
    IRuntimeDatabaseType underlyingType,
    Memory<byte> sourceMemory,
    ushort value
) : InterpretValue2<ushort>(underlyingType, sourceMemory, value);

public class InterpretValue2U4(
    IRuntimeDatabaseType underlyingType,
    Memory<byte> sourceMemory,
    uint value
) : InterpretValue2<uint>(underlyingType, sourceMemory, value);

public class InterpretValue2U8(
    IRuntimeDatabaseType underlyingType,
    Memory<byte> sourceMemory,
    ulong value
) : InterpretValue2<ulong>(underlyingType, sourceMemory, value);

public class InterpretValue2I1(
    IRuntimeDatabaseType underlyingType,
    Memory<byte> sourceMemory,
    sbyte value
) : InterpretValue2<sbyte>(underlyingType, sourceMemory, value);

public class InterpretValue2I2(
    IRuntimeDatabaseType underlyingType,
    Memory<byte> sourceMemory,
    short value
) : InterpretValue2<short>(underlyingType, sourceMemory, value);

public class InterpretValue2I4(
    IRuntimeDatabaseType underlyingType,
    Memory<byte> sourceMemory,
    int value
) : InterpretValue2<int>(underlyingType, sourceMemory, value);

public class InterpretValue2I8(
    IRuntimeDatabaseType underlyingType,
    Memory<byte> sourceMemory,
    long value
) : InterpretValue2<long>(underlyingType, sourceMemory, value);

public class InterpretValue2F(
    IRuntimeDatabaseType underlyingType,
    Memory<byte> sourceMemory,
    float value
) : InterpretValue2<float>(underlyingType, sourceMemory, value);

public class InterpretValue2D(
    IRuntimeDatabaseType underlyingType,
    Memory<byte> sourceMemory,
    double value
) : InterpretValue2<double>(underlyingType, sourceMemory, value);

public class InterpretValue2Timestamp(
    IRuntimeDatabaseType underlyingType,
    Memory<byte> sourceMemory,
    DateTime value
) : InterpretValue2<DateTime>(underlyingType, sourceMemory, value);

public class InterpretValue2Array(
    IRuntimeDatabaseType underlyingType,
    Memory<byte> sourceMemory,
    List<InterpretValue2> elements
) : InterpretValue2<List<InterpretValue2>>(underlyingType, sourceMemory, elements), IReadOnlyList<InterpretValue2>
{
    internal override string DebugDisplay => $"{UnderlyingType.Name}[{Value.Count}]";

    public IEnumerator<InterpretValue2> GetEnumerator() => Value.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public int Count => Value.Count;

    public InterpretValue2 this[int index] => Value[index];
}

public class InterpretValue2WString(
    IRuntimeDatabaseType underlyingType,
    Memory<byte> sourceMemory,
    string value
) : InterpretValue2<string>(underlyingType, sourceMemory, value);

public class InterpretValue2AString(
    IRuntimeDatabaseType underlyingType,
    Memory<byte> sourceMemory,
    string value
) : InterpretValue2<string>(underlyingType, sourceMemory, value);

public class InterpretValue2StructField(
    InterpretValue2 value
) : InterpretValue2<InterpretValue2>(value.UnderlyingType, value.SourceMemory, value)
{
    internal override string DebugDisplay => $"{Value.DebugDisplay}";
}

public class InterpretValue2Struct(
    IRuntimeDatabaseType underlyingType,
    Memory<byte> sourceMemory,
    List<InterpretValue2StructField> value
) : InterpretValue2<List<InterpretValue2StructField>>(underlyingType, sourceMemory, value)
{
    internal override string DebugDisplay => $"{UnderlyingType.Name} ({Value.Count} fields)";
}
