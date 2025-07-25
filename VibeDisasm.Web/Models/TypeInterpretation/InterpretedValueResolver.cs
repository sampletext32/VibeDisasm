using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using VibeDisasm.Web.Models.DatabaseEntries;

namespace VibeDisasm.Web.Models.TypeInterpretation;

/// <summary>
/// Polymorphic JSON resolver for InterpretedValue and its derived types.
/// </summary>
public sealed class InterpretedValueResolver : DefaultJsonTypeInfoResolver
{
    private static readonly List<(Type, string)> _map = [
        (typeof(InterpretedRawValue), "raw"),
        (typeof(InterpretedArrayValue), "array"),
        (typeof(InterpretedStructValue), "struct"),
        (typeof(InterpretedStructField), "struct-field"),
        (typeof(InterpretedSignedInteger), "sint"),
        (typeof(InterpretedUnsignedInteger), "uint"),
        (typeof(InterpretedFloat), "float"),
        (typeof(InterpretedDouble), "double"),
        (typeof(InterpretedBoolean), "bool"),
        (typeof(InterpretedAsciiString), "ascii"),
        (typeof(InterpretedWideString), "wide"),
    ];

    public override JsonTypeInfo GetTypeInfo(Type type, JsonSerializerOptions options)
    {
        var jsonTypeInfo = base.GetTypeInfo(type, options);

        if (jsonTypeInfo.Type == typeof(InterpretedValue))
        {
            jsonTypeInfo.PolymorphismOptions = new JsonPolymorphismOptions
            {
                TypeDiscriminatorPropertyName = "$type",
                IgnoreUnrecognizedTypeDiscriminators = true,
                UnknownDerivedTypeHandling = JsonUnknownDerivedTypeHandling.FailSerialization,
            };

            foreach (var valueTuple in _map)
            {
                jsonTypeInfo.PolymorphismOptions.DerivedTypes.Add(new JsonDerivedType(valueTuple.Item1, valueTuple.Item2));
            }

        }

        return jsonTypeInfo;
    }
}
