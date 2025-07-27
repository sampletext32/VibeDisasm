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
        [typeof(TypeArchiveElementDto)] = [
            (typeof(TypeArchiveArrayElementDto), "array"),
            (typeof(TypeArchiveEnumElementDto), "enum"),
            (typeof(TypeArchiveFunctionElementDto), "func"),
            (typeof(TypeArchivePointerElementDto), "ptr"),
            (typeof(TypeArchivePrimitiveElementDto), "primitive"),
            (typeof(TypeArchiveStructureElementDto), "struct"),
            (typeof(TypeArchiveTypeRefElementDto), "ref"),
        ],

        // UserProgramDatabaseEntry mappings
        [typeof(UserProgramDatabaseEntry)] = [
            (typeof(ArrayUserProgramDatabaseEntry), "array"),
            (typeof(PrimitiveUserProgramDatabaseEntry), "primitive"),
            (typeof(UndefinedUserProgramDatabaseEntry), "undefined"),
            (typeof(StructUserProgramDatabaseEntry), "struct"),
        ],

        // InterpretedValue mappings
        [typeof(InterpretedValue)] = [
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
        ],

        // RuntimeDatabaseType mappings
        [typeof(RuntimeDatabaseType)] = [
            (typeof(RuntimeArrayType), "array"),
            (typeof(RuntimeEnumType), "enum"),
            (typeof(RuntimeFunctionType), "func"),
            (typeof(RuntimePointerType), "ptr"),
            (typeof(RuntimePrimitiveType), "primitive"),
            (typeof(RuntimeStructureType), "struct")
        ],

        // TypeArchiveJsonElement mappings
        [typeof(TypeArchiveJsonElement)] = [
            (typeof(ArrayArchiveJsonElement), "arr"),
            (typeof(EnumArchiveJsonElement), "enum"),
            (typeof(FunctionArchiveJsonElement), "fun"),
            (typeof(PointerArchiveJsonElement), "ptr"),
            (typeof(PrimitiveArchiveJsonElement), "prm"),
            (typeof(StructArchiveJsonElement), "str"),
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
