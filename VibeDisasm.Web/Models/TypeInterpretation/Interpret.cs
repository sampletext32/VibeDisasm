using System.Buffers.Binary;
using System.Text;
using VibeDisasm.Web.Models.Types;
using VibeDisasm.Web.Overlay;

namespace VibeDisasm.Web.Models.TypeInterpretation;

public interface IInterpret
{
    bool CanInterpretTo<TTarget>(OverlayedType source)
        where TTarget : InterpretValue2;

    InterpretValue2 Run(OverlayedType source);
}

public abstract record InterpretBase
{
    public abstract InterpretValue2 Run(OverlayedType source);

    public static InterpretAsRaw AsRaw() => new();

    public static InterpretAsUint AsUint(Endianness endianness = Endianness.LittleEndian) => new(endianness);

    public static InterpretAsInt AsInt(Endianness endianness = Endianness.LittleEndian) => new(endianness);

    public static InterpretAsChar AsChar() => new();
    public static InterpretAsBool AsBool() => new();

    public static InterpretAsFloat AsFloat(Endianness endianness = Endianness.LittleEndian) => new(endianness);

    public static InterpretAsTimestamp AsTimestamp(Endianness endianness = Endianness.LittleEndian) => new(endianness);

    public static InterpretAsArrayOf AsArrayOf(IRuntimeDatabaseType elementType, IInterpret elementInterpreter) =>
        new(elementType, elementInterpreter);

    public static InterpretAsWString AsWString() => new();
    public static InterpretAsAString AsAString() => new();

    public static InterpretAsStruct AsStruct() => new();
}

public record InterpretAsRaw : InterpretBase, IInterpret
{
    public bool CanInterpretTo<TTarget>(OverlayedType source) where TTarget : InterpretValue2 => true;

    public override InterpretValue2 Run(OverlayedType source) =>
        new InterpretValue2Raw(source.UnderlyingType, source.Bytes);
}

public record InterpretAsChar : InterpretBase, IInterpret
{
    public bool CanInterpretTo<TTarget>(OverlayedType source) where TTarget : InterpretValue2 =>
        source.UnderlyingType.Size == 1;

    public override InterpretValue2 Run(OverlayedType source) => new InterpretValue2Char(
        source.UnderlyingType,
        source.Bytes,
        (char)source.Bytes.Span[0]
    );
}

public record InterpretAsBool : InterpretBase, IInterpret
{
    public bool CanInterpretTo<TTarget>(OverlayedType source) where TTarget : InterpretValue2 =>
        source.UnderlyingType.Size == 1;

    public override InterpretValue2 Run(OverlayedType source) => new InterpretValue2Bool(
        source.UnderlyingType,
        source.Bytes,
        value: source.Bytes.Span[0] != 0
    );
}

public record InterpretAsTimestamp(Endianness Endianness) : InterpretBase, IInterpret
{
    public bool CanInterpretTo<TTarget>(OverlayedType source) where TTarget : InterpretValue2 =>
        source.UnderlyingType.Size == 4;

    public override InterpretValue2 Run(OverlayedType source) => new InterpretValue2Timestamp(
        source.UnderlyingType,
        source.Bytes,
        new DateTime(1970, 1, 1).AddSeconds(
            Endianness is Endianness.LittleEndian
                ? BinaryPrimitives.ReadInt32LittleEndian(source.Bytes.Span)
                : BinaryPrimitives.ReadInt32BigEndian(source.Bytes.Span)
        )
    );
}

public record InterpretAsUint(Endianness Endianness) : InterpretBase, IInterpret
{
    public bool CanInterpretTo<TTarget>(OverlayedType source) where TTarget : InterpretValue2 =>
        source.UnderlyingType is { Size: 1 or 2 or 4 or 8 };

    public override InterpretValue2 Run(OverlayedType source)
    {
        return source.Bytes.Length switch
        {
            1 => new InterpretValue2U1(source.UnderlyingType, source.Bytes, source.Bytes.Span[0]),
            2 => new InterpretValue2U2(
                source.UnderlyingType,
                source.Bytes,
                Endianness is Endianness.LittleEndian
                    ? BinaryPrimitives.ReadUInt16LittleEndian(source.Bytes.Span)
                    : BinaryPrimitives.ReadUInt16BigEndian(source.Bytes.Span)
            ),
            4 => new InterpretValue2U4(
                source.UnderlyingType,
                source.Bytes,
                Endianness is Endianness.LittleEndian
                    ? BinaryPrimitives.ReadUInt32LittleEndian(source.Bytes.Span)
                    : BinaryPrimitives.ReadUInt32BigEndian(source.Bytes.Span)
            ),
            8 => new InterpretValue2U8(
                source.UnderlyingType,
                source.Bytes,
                Endianness is Endianness.LittleEndian
                    ? BinaryPrimitives.ReadUInt64LittleEndian(source.Bytes.Span)
                    : BinaryPrimitives.ReadUInt64BigEndian(source.Bytes.Span)
            ),
            _ => throw new ArgumentOutOfRangeException(
                nameof(source),
                "Unsupported byte length for unsigned integer interpretation."
            )
        };
    }
}

public record InterpretAsInt(Endianness Endianness) : InterpretBase, IInterpret
{
    public bool CanInterpretTo<TTarget>(OverlayedType source) where TTarget : InterpretValue2 =>
        source.UnderlyingType is { Size: 1 or 2 or 4 or 8 };

    public override InterpretValue2 Run(OverlayedType source)
    {
        return source.Bytes.Length switch
        {
            1 => new InterpretValue2I1(source.UnderlyingType, source.Bytes, (sbyte)source.Bytes.Span[0]),
            2 => new InterpretValue2I2(
                source.UnderlyingType,
                source.Bytes,
                Endianness is Endianness.LittleEndian
                    ? BinaryPrimitives.ReadInt16LittleEndian(source.Bytes.Span)
                    : BinaryPrimitives.ReadInt16BigEndian(source.Bytes.Span)
            ),
            4 => new InterpretValue2I4(
                source.UnderlyingType,
                source.Bytes,
                Endianness is Endianness.LittleEndian
                    ? BinaryPrimitives.ReadInt32LittleEndian(source.Bytes.Span)
                    : BinaryPrimitives.ReadInt32BigEndian(source.Bytes.Span)
            ),
            8 => new InterpretValue2I8(
                source.UnderlyingType,
                source.Bytes,
                Endianness is Endianness.LittleEndian
                    ? BinaryPrimitives.ReadInt64LittleEndian(source.Bytes.Span)
                    : BinaryPrimitives.ReadInt64BigEndian(source.Bytes.Span)
            ),
            _ => throw new ArgumentOutOfRangeException(
                nameof(source),
                "Unsupported byte length for signed integer interpretation."
            )
        };
    }
}

public record InterpretAsFloat(Endianness Endianness) : InterpretBase, IInterpret
{
    public bool CanInterpretTo<TTarget>(OverlayedType source) where TTarget : InterpretValue2 =>
        source.UnderlyingType is { Size: 4 or 8 };

    public override InterpretValue2 Run(OverlayedType source)
    {
        return source.Bytes.Length switch
        {
            4 => new InterpretValue2F(
                source.UnderlyingType,
                source.Bytes,
                Endianness is Endianness.LittleEndian
                    ? BinaryPrimitives.ReadSingleLittleEndian(source.Bytes.Span)
                    : BinaryPrimitives.ReadSingleBigEndian(source.Bytes.Span)
            ),
            8 => new InterpretValue2D(
                source.UnderlyingType,
                source.Bytes,
                Endianness is Endianness.LittleEndian
                    ? BinaryPrimitives.ReadDoubleLittleEndian(source.Bytes.Span)
                    : BinaryPrimitives.ReadDoubleBigEndian(source.Bytes.Span)
            ),
            _ => throw new ArgumentOutOfRangeException(
                nameof(source),
                "Unsupported byte length for float interpretation."
            )
        };
    }
}

public record InterpretAsArrayOf(IRuntimeDatabaseType TargetElementType, IInterpret TargetElementInterpreter)
    : InterpretBase, IInterpret
{
    public bool CanInterpretTo<TTarget>(OverlayedType source) where TTarget : InterpretValue2 =>
        source is OverlayedArray;

    public override InterpretValue2 Run(OverlayedType source)
    {
        if (source is not OverlayedArray array)
        {
            throw new ArgumentException("Source must be an OverlayedArray", nameof(source));
        }

        if (source.Bytes.Length > TargetElementType.Size)
        {
            throw new ArgumentException("Source size was larger than target", nameof(source));
        }

        if (source.Bytes.Length == 0)
        {
            return new InterpretValue2Array(TargetElementType, source.Bytes, []);
        }

        var elements = array.Elements
            .Select(arrayElement => OverlayHelper.OverlayType(TargetElementType, arrayElement.Bytes, 0))
            .Select(overlayedElement => TargetElementInterpreter.Run(overlayedElement))
            .ToList();

        return new InterpretValue2Array(array.UnderlyingType, source.Bytes, elements);
    }
}

public record InterpretAsWString() : InterpretBase, IInterpret
{
    public bool CanInterpretTo<TTarget>(OverlayedType source) where TTarget : InterpretValue2 => true;

    public override InterpretValue2 Run(OverlayedType source)
    {
        var length = 0;
        while (length < source.Bytes.Length - 1)
        {
            if (source.Bytes.Span[length] == 0 && source.Bytes.Span[length + 1] == 0)
            {
                break;
            }

            length += 2;
        }

        if (length > source.Bytes.Length)
        {
            length = source.Bytes.Length - (source.Bytes.Length % 2);
        }

        var value = Encoding.Unicode.GetString(source.Bytes.Span[..length]);

        return new InterpretValue2WString(
            source.UnderlyingType,
            source.Bytes,
            value
        );
    }
}

public record InterpretAsAString() : InterpretBase, IInterpret
{
    public bool CanInterpretTo<TTarget>(OverlayedType source) where TTarget : InterpretValue2 => true;

    public override InterpretValue2 Run(OverlayedType source)
    {
        var zero = source.Bytes.Span.IndexOf((byte)0);

        return new InterpretValue2AString(
            source.UnderlyingType,
            source.Bytes,
            zero == -1
                ? Encoding.ASCII.GetString(source.Bytes.Span)
                : Encoding.ASCII.GetString(source.Bytes.Span[..zero])
        );
    }
}

public record InterpretAsStruct() : InterpretBase, IInterpret
{
    public bool CanInterpretTo<TTarget>(OverlayedType source) where TTarget : InterpretValue2 => true;

    public override InterpretValue2 Run(OverlayedType source)
    {
        if (source is not OverlayedStructure structSource)
        {
            throw new ArgumentException("Source must be an OverlayedStructure", nameof(source));
        }

        return new InterpretValue2Struct(
            source.UnderlyingType,
            source.Bytes,
            structSource.Fields
                .Select(x => new InterpretValue2StructField(TypeInterpreter.InterpretDefault2(x)))
                .ToList()
        );
    }
}
