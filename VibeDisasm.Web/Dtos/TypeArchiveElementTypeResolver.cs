using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;

namespace VibeDisasm.Web.Dtos;

/// <summary>
/// Polymorphic JSON resolver for UserProgramDatabaseEntry and its derived types.
/// </summary>
public sealed class TypeArchiveElementTypeResolver : DefaultJsonTypeInfoResolver
{
    private static readonly List<(Type, string)> _map = [
        (typeof(TypeArchiveArrayElementDto), "array"),
        (typeof(TypeArchiveEnumElementDto), "enum"),
        (typeof(TypeArchiveFunctionElementDto), "func"),
        (typeof(TypeArchivePointerElementDto), "ptr"),
        (typeof(TypeArchivePrimitiveElementDto), "primitive"),
        (typeof(TypeArchiveStructureElementDto), "struct"),
        (typeof(TypeArchiveTypeRefElementDto), "ref"),
    ];

    public override JsonTypeInfo GetTypeInfo(Type type, JsonSerializerOptions options)
    {
        var jsonTypeInfo = base.GetTypeInfo(type, options);

        if (jsonTypeInfo.Type == typeof(TypeArchiveElementDto))
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
