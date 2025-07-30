using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using VibeDisasm.Web.Dtos;
using VibeDisasm.Web.Models.DatabaseEntries;
using VibeDisasm.Web.Models.TypeInterpretation;
using VibeDisasm.Web.Models.Types;
using VibeDisasm.Web.ProjectArchive.TypeArchiveJsonElements;

namespace VibeDisasm.Web;

/// <summary>
/// Unified polymorphic JSON resolver for all polymorphic types in the application.
/// </summary>
public sealed class UnifiedTypeResolver : DefaultJsonTypeInfoResolver
{
    // Type mappings for all polymorphic types in the application
    private static readonly Dictionary<Type, List<(Type, string)>> _typeMappings = new()
    {
        // TypeArchiveElementDto mappings
        [typeof(TypeArchiveElementDto)] =
        [
            (typeof(TypeArchiveArrayElementDto), "array"),
            (typeof(TypeArchiveEnumElementDto), "enum"),
            (typeof(TypeArchiveFunctionElementDto), "func"),
            (typeof(TypeArchivePointerElementDto), "ptr"),
            (typeof(TypeArchivePrimitiveElementDto), "primitive"),
            (typeof(TypeArchiveStructureElementDto), "struct"),
            (typeof(TypeArchiveTypeRefElementDto), "ref"),
        ],

        // UserProgramDatabaseEntry mappings
        [typeof(UserProgramDatabaseEntry)] =
        [
            (typeof(ArrayUserProgramDatabaseEntry), "array"),
            (typeof(PrimitiveUserProgramDatabaseEntry), "primitive"),
            (typeof(UndefinedUserProgramDatabaseEntry), "undefined"),
            (typeof(StructUserProgramDatabaseEntry), "struct"),
        ],

        // InterpretedValue mappings
        [typeof(InterpretValue2)] =
        [
            (typeof(InterpretValue2Raw), "r"),
            (typeof(InterpretValue2Char), "c"),
            (typeof(InterpretValue2Bool), "b"),
            (typeof(InterpretValue2U1), "u1"),
            (typeof(InterpretValue2U2), "u2"),
            (typeof(InterpretValue2U4), "u4"),
            (typeof(InterpretValue2U8), "u8"),
            (typeof(InterpretValue2I1), "i1"),
            (typeof(InterpretValue2I2), "i2"),
            (typeof(InterpretValue2I4), "i4"),
            (typeof(InterpretValue2I8), "i8"),
            (typeof(InterpretValue2F), "f"),
            (typeof(InterpretValue2D), "d"),
            (typeof(InterpretValue2Timestamp), "ts"),
            (typeof(InterpretValue2Array), "a"),
            (typeof(InterpretValue2WString), "ws"),
            (typeof(InterpretValue2AString), "as"),
            (typeof(InterpretValue2Struct), "s"),
            (typeof(InterpretValue2StructField), "sf"),
        ],

        // RuntimeDatabaseType mappings
        [typeof(IRuntimeDatabaseType)] =
        [
            (typeof(RuntimeArrayType), "array"),
            (typeof(RuntimeEnumType), "enum"),
            (typeof(RuntimeFunctionType), "func"),
            (typeof(RuntimePointerType), "ptr"),
            (typeof(RuntimePrimitiveType), "primitive"),
            (typeof(RuntimeStructureType), "struct"),
            (typeof(RuntimeStructureTypeField), "struct-field")
        ],

        // TypeArchiveJsonElement mappings
        [typeof(TypeArchiveJsonElement)] =
        [
            (typeof(ArrayArchiveJsonElement), "arr"),
            (typeof(EnumArchiveJsonElement), "enum"),
            (typeof(FunctionArchiveJsonElement), "fun"),
            (typeof(PointerArchiveJsonElement), "ptr"),
            (typeof(PrimitiveArchiveJsonElement), "prm"),
            (typeof(StructArchiveJsonElement), "str"),
            (typeof(StructFieldArchiveJsonElement), "strf"),
            (typeof(TypeRefJsonElement), "ref"),
        ]
    };

    [Pure]
    public override JsonTypeInfo GetTypeInfo(Type type, JsonSerializerOptions options)
    {
        var jsonTypeInfo = base.GetTypeInfo(type, options);

        if (_typeMappings.TryGetValue(jsonTypeInfo.Type, out var mappings))
        {
            jsonTypeInfo.PolymorphismOptions = new JsonPolymorphismOptions
            {
                TypeDiscriminatorPropertyName = "$type",
                IgnoreUnrecognizedTypeDiscriminators = true,
                UnknownDerivedTypeHandling = JsonUnknownDerivedTypeHandling.FailSerialization,
            };

            foreach (var (derivedType, discriminator) in mappings)
            {
                jsonTypeInfo.PolymorphismOptions.DerivedTypes.Add(new JsonDerivedType(derivedType, discriminator));
            }
        }

        if (jsonTypeInfo.Type.IsAssignableTo(typeof(IRuntimeDatabaseType)))
        {
            jsonTypeInfo.Properties.Remove(
                jsonTypeInfo.Properties.First(x => x.Name == JsonNamingPolicy.CamelCase.ConvertName(nameof(IRuntimeDatabaseType.DefaultInterpreter)))
            );
            jsonTypeInfo.Properties.Remove(
                jsonTypeInfo.Properties.First(x => x.Name == JsonNamingPolicy.CamelCase.ConvertName(nameof(IRuntimeDatabaseType.Interpreters)))
            );
            jsonTypeInfo.Properties.Remove(
                jsonTypeInfo.Properties.First(x => x.Name == JsonNamingPolicy.CamelCase.ConvertName(nameof(IRuntimeDatabaseType.InterpreterOverride)))
            );
        }

        return jsonTypeInfo;
    }
}

public static class JsonSerializerOptionsPresets
{
    public static readonly JsonSerializerOptions StandardOptions = new(JsonSerializerOptions.Web)
    {
        Converters = { new JsonStringEnumConverter() },
        WriteIndented = true,
        TypeInfoResolver = new UnifiedTypeResolver()
    };
}
