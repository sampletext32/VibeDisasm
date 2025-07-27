using VibeDisasm.Web.Models;
using VibeDisasm.Web.Models.Types;
using VibeDisasm.Web.Temporary;

namespace VibeDisasm.Web.Services;

public interface ITypeArchiveStorage
{
    IReadOnlyList<RuntimeTypeArchive> TypeArchives { get; }

    void Import(RuntimeTypeArchive typeArchive);
    RuntimeTypeArchive? FindArchive(string @namespace);
    RuntimeTypeArchive FindRequiredArchive(string @namespace);
    RuntimeDatabaseType? FindType(string @namespace, string name);
    RuntimeDatabaseType FindRequiredType(string @namespace, string name);

    T? FindType<T>(string @namespace, string name)
        where T : RuntimeDatabaseType;

    T FindRequiredType<T>(string @namespace, string name)
        where T : RuntimeDatabaseType;
}

public class TypeArchiveStorage : ITypeArchiveStorage
{
    private readonly ILogger<TypeArchiveStorage> _logger;
    private readonly List<RuntimeTypeArchive> _typeArchives = [];
    public IReadOnlyList<RuntimeTypeArchive> TypeArchives => _typeArchives;

    public TypeArchiveStorage(ILogger<TypeArchiveStorage> logger)
    {
        _logger = logger;
        var builtinArchive = DefaultWindowsTypesPopulator.CreateBuiltinArchive();
        var win32Archive = DefaultWindowsTypesPopulator.CreateWin32Archive(builtinArchive);

        _typeArchives.Add(builtinArchive);
        _typeArchives.Add(win32Archive);
        logger.LogInformation("Embedded type archives initialized.");
    }

    public void Import(RuntimeTypeArchive typeArchive)
    {
        if (_typeArchives.Any(x => x.Namespace == typeArchive.Namespace))
        {
            _logger.LogWarning(
                "Type archive with namespace '{Namespace}' already exists. Skipping import.",
                typeArchive.Namespace
            );
            return;
        }

        _typeArchives.Add(typeArchive);
        _logger.LogInformation("Imported type archive with namespace '{Namespace}'.", typeArchive.Namespace);
    }

    public RuntimeTypeArchive? FindArchive(string @namespace) =>
        _typeArchives.FirstOrDefault(x => x.Namespace == @namespace);

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
}
