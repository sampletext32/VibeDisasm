using System.Text.Json;
using VibeDisasm.Web.Models.DatabaseEntries;
using VibeDisasm.Web.Models.Types;

namespace VibeDisasm.Web.Models;

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
}
