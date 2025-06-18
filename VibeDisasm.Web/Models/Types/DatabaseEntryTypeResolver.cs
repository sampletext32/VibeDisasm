using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;

namespace VibeDisasm.Web.Models.Types;

/// <summary>
/// Polymorphic JSON resolver for UserProgramDatabaseEntry and its derived types.
/// </summary>
public sealed class DatabaseTypeResolver : DefaultJsonTypeInfoResolver
{
    private static readonly List<(Type, string)> _map = [
        (typeof(ArrayType), "array"),
        (typeof(FunctionType), "func"),
        (typeof(PointerType), "ptr"),
        (typeof(PrimitiveType), "primitive"),
        (typeof(StructureType), "struct"),
    ];

    public override JsonTypeInfo GetTypeInfo(Type type, JsonSerializerOptions options)
    {
        var jsonTypeInfo = base.GetTypeInfo(type, options);

        if (jsonTypeInfo.Type == typeof(DatabaseType))
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
