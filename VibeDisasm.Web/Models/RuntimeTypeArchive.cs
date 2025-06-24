using VibeDisasm.Web.Models.Types;

namespace VibeDisasm.Web.Models;

public class RuntimeTypeArchive
{
    /// <summary>
    /// Namespace declared by this archive, effectively it's Id
    /// </summary>
    public string Namespace { get; init; }

    public List<RuntimeDatabaseType> Types { get; set; } = [];

    /// <summary>
    /// Absolute path to the file, containing this archive. Can be null if this archive is a newly created one.
    /// </summary>
    public string? AbsoluteFilePath { get; set; }

    public RuntimeTypeArchive(string @namespace)
    {
        Namespace = @namespace;
    }

    public RuntimeDatabaseType AddType(RuntimeDatabaseType type)
    {
        Types.Add(type);
        return type;
    }
}
