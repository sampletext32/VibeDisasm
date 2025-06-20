using System.Text.Json;
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
        TypeInfoResolver = new DatabaseTypeResolver()
    };
    public static readonly JsonSerializerOptions TypeArchiveJsonOptions = new(JsonSerializerOptions.Default)
    {
        TypeInfoResolver = new TypeArchiveJsonResolver()
    };
}
