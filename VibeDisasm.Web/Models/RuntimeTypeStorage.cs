using VibeDisasm.Web.Models.Types;

namespace VibeDisasm.Web.Models;

public class RuntimeTypeStorage(RuntimeUserProgram program)
{
    public List<RuntimeTypeArchive> Archives { get; set; } = [];

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

        if (resolvedType is null)
        {
            return null;
        }

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
