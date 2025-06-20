using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;

namespace VibeDisasm.Web.Models.Types;

/// <summary>
/// Polymorphic JSON resolver for UserProgramDatabaseEntry and its derived types.
/// </summary>
public sealed class RuntimeDatabaseTypeResolver : DefaultJsonTypeInfoResolver
{
    private static readonly List<(Type, string)> _map = [
        (typeof(RuntimeArrayType), "array"),
        (typeof(RuntimeFunctionType), "func"),
        (typeof(RuntimePointerType), "ptr"),
        (typeof(RuntimePrimitiveType), "primitive"),
        (typeof(RuntimeStructureType), "struct"),
        (typeof(RuntimeTypeRefType), "ref"),
    ];

    public override JsonTypeInfo GetTypeInfo(Type type, JsonSerializerOptions options)
    {
        var jsonTypeInfo = base.GetTypeInfo(type, options);

        if (jsonTypeInfo.Type == typeof(RuntimeDatabaseType))
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
