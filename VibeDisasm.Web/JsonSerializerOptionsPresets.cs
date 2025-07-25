using System.Text.Json;
using System.Text.Json.Serialization;
using VibeDisasm.Web.Dtos;
using VibeDisasm.Web.Models.DatabaseEntries;
using VibeDisasm.Web.Models.TypeInterpretation;
using VibeDisasm.Web.Models.Types;
using VibeDisasm.Web.ProjectArchive.TypeArchiveJsonElements;

namespace VibeDisasm.Web;

public static class JsonSerializerOptionsPresets
{
    public static readonly JsonSerializerOptions DatabaseEntryOptions = new(JsonSerializerOptions.Web)
    {
        Converters = { new JsonStringEnumConverter() },
        TypeInfoResolver = new DatabaseEntryTypeResolver()
    };
    public static readonly JsonSerializerOptions DatabaseTypeOptions = new(JsonSerializerOptions.Web)
    {
        Converters = { new JsonStringEnumConverter() },
        TypeInfoResolver = new RuntimeDatabaseTypeResolver()
    };
    public static readonly JsonSerializerOptions TypeArchiveJsonOptions = new(JsonSerializerOptions.Default)
    {
        Converters = { new JsonStringEnumConverter() },
        TypeInfoResolver = new TypeArchiveJsonResolver()
    };
    public static readonly JsonSerializerOptions TypeArchiveElementOptions = new(JsonSerializerOptions.Default)
    {
        Converters = { new JsonStringEnumConverter() },
        TypeInfoResolver = new TypeArchiveElementTypeResolver()
    };
    public static readonly JsonSerializerOptions InterpretedValueOptions = new(JsonSerializerOptions.Default)
    {
        Converters = { new JsonStringEnumConverter() },
        TypeInfoResolver = new InterpretedValueResolver()
    };
}
