using VibeDisasm.Web.Models.Types;

namespace VibeDisasm.Web.Models;

public class RuntimeTypeStorage(RuntimeUserProgram program)
{
    public List<RuntimeTypeArchive> Archives { get; init; } = [];

    public RuntimeTypeArchive? FindArchive(string @namespace) =>
        Archives.FirstOrDefault(x => x.Namespace == @namespace);

    public RuntimeTypeArchive FindRequiredArchive(string @namespace) =>
        FindArchive(@namespace) ?? throw new Exception($"Archive for namespace '{@namespace}' not found.");

    public RuntimeDatabaseType? FindType(string @namespace, string name)
    {
        var archive = FindArchive(@namespace);

        if (archive is null)
        {
            return null;
        }

        var type = archive.FindType(name);

        return type ?? null;
    }

    public RuntimeDatabaseType FindRequiredType(string @namespace, string name)
    {
        var archive = FindRequiredArchive(@namespace);
        var type = archive.FindRequiredType(name);
        return type;
    }

    public T? FindType<T>(string @namespace, string name)
        where T : RuntimeDatabaseType
    {
        var archive = FindArchive(@namespace);

        if (archive is null)
        {
            return null;
        }

        var type = archive.FindType<T>(name);
        return type;
    }

    public T FindRequiredType<T>(string @namespace, string name)
        where T : RuntimeDatabaseType
    {
        var archive = FindRequiredArchive(@namespace);
        var type = archive.FindRequiredType<T>(name);
        return type;
    }

    /// <summary>
    /// Resolves a typeref, by looking in it's namespace-specified archive. If resolved type is a typeref itself - returns it as is.
    /// </summary>
    public RuntimeDatabaseType? ResolveTypeRef(RuntimeTypeRefType type)
    {
        var archive = Archives.FirstOrDefault(x => x.Namespace == type.Namespace);

        if (archive is null)
        {
            return null;
        }

        var resolvedType = archive.Types.FirstOrDefault(x => x.Id == type.Id);

        return resolvedType;
    }

    /// <summary>
    /// Resolves a typeref, by looking in it's namespace-specified archive. If resolved type is a typeref itself - resolves it to discrete type.
    /// </summary>
    public RuntimeDatabaseType? DeepResolveTypeRef(RuntimeTypeRefType type)
    {
        var resolvedType = ResolveTypeRef(type);

        if (resolvedType is RuntimeTypeRefType nestedRef)
        {
            return DeepResolveTypeRef(nestedRef);
        }

        return resolvedType;
    }
}
