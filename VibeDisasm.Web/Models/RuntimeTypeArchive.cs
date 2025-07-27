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
    /// Absolute path to the file, containing this archive. Can be null if this archive is a newly created one or embedded.
    /// </summary>
    public string? AbsoluteFilePath { get; set; }

    /// <summary>
    /// If an archive is embedded, it means that it is not supposed to be stored in a file
    /// </summary>
    public bool IsEmbedded { get; set; }

    public RuntimeTypeArchive(string @namespace, bool isEmbedded)
    {
        Namespace = @namespace;
        IsEmbedded = isEmbedded;
    }

    public RuntimeDatabaseType AddType(RuntimeDatabaseType type)
    {
        Types.Add(type);
        return type;
    }

    public RuntimeDatabaseType? FindType(string name)
    {
        var type = Types.FirstOrDefault(x => x.Name == name);

        return type ?? null;
    }
    public RuntimeDatabaseType FindRequiredType(string name)
    {
        var type = Types.FirstOrDefault(x => x.Name == name);

        return type ?? throw new Exception($"Type with name '{name}' not found in archive '{Namespace}'.");
    }

    public T? FindType<T>(string name)
        where T : RuntimeDatabaseType
    {
        var type = Types.FirstOrDefault(x => x.Name == name);

        return type as T;
    }
    public T FindRequiredType<T>(string name)
        where T : RuntimeDatabaseType
    {
        var type = Types.FirstOrDefault(x => x.Name == name);

        return type as T ?? throw new Exception($"Type with name '{name}' not found in archive '{Namespace}'.");
    }
}
