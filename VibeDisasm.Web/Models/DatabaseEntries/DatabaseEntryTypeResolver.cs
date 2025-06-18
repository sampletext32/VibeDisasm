using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;

namespace VibeDisasm.Web.Models.DatabaseEntries;

public static class JsonSerializerOptionsPresets
{
    public static readonly JsonSerializerOptions DatabaseEntryTypeOptions = new(JsonSerializerOptions.Web)
    {
        TypeInfoResolver = new DatabaseEntryTypeResolver()
    };
}


/// <summary>
/// Polymorphic JSON resolver for UserProgramDatabaseEntry and its derived types.
/// </summary>
public sealed class DatabaseEntryTypeResolver : DefaultJsonTypeInfoResolver
{
    private static readonly List<(Type, string)> _map = [
        (typeof(ArrayUserProgramDatabaseEntry), "array"),
        (typeof(PrimitiveUserProgramDatabaseEntry), "primitive"),
        (typeof(UndefinedUserProgramDatabaseEntry), "undefined"),
    ];

    public override JsonTypeInfo GetTypeInfo(Type type, JsonSerializerOptions options)
    {
        var jsonTypeInfo = base.GetTypeInfo(type, options);

        if (jsonTypeInfo.Type == typeof(UserProgramDatabaseEntry))
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
