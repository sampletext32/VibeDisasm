using System.Text.Json;
using VibeDisasm.Web.Dtos;
using VibeDisasm.Web.Models.DatabaseEntries;
using VibeDisasm.Web.Models.Types;
using VibeDisasm.Web.ProjectArchive.TypeArchiveJsonElements;

namespace VibeDisasm.Web;

public static class JsonSerializerOptionsPresets
{
    public static readonly JsonSerializerOptions DatabaseEntryOptions = new(JsonSerializerOptions.Web)
    {
        TypeInfoResolver = new DatabaseEntryTypeResolver()
    };
    public static readonly JsonSerializerOptions DatabaseTypeOptions = new(JsonSerializerOptions.Web)
    {
        TypeInfoResolver = new RuntimeDatabaseTypeResolver()
    };
    public static readonly JsonSerializerOptions TypeArchiveJsonOptions = new(JsonSerializerOptions.Default)
    {
        TypeInfoResolver = new TypeArchiveJsonResolver()
    };
    public static readonly JsonSerializerOptions TypeArchiveElementOptions = new(JsonSerializerOptions.Default)
    {
        TypeInfoResolver = new TypeArchiveElementTypeResolver()
    };
}
