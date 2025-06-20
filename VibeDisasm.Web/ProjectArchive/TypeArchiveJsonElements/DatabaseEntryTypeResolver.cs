using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;

namespace VibeDisasm.Web.ProjectArchive.TypeArchiveJsonElements;

/// <summary>
/// Polymorphic JSON resolver for TypeArchiveJsonElement and its derived types.
/// </summary>
public sealed class TypeArchiveJsonResolver : DefaultJsonTypeInfoResolver
{
    private static readonly List<(Type, string)> _map = [
        (typeof(ArrayArchiveJsonElement), "arr"),
        (typeof(FunctionArchiveJsonElement), "fun"),
        (typeof(PointerArchiveJsonElement), "ptr"),
        (typeof(PrimitiveArchiveJsonElement), "prm"),
        (typeof(StructArchiveJsonElement), "str"),
    ];

    public override JsonTypeInfo GetTypeInfo(Type type, JsonSerializerOptions options)
    {
        var jsonTypeInfo = base.GetTypeInfo(type, options);

        if (jsonTypeInfo.Type == typeof(TypeArchiveJsonElement))
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
