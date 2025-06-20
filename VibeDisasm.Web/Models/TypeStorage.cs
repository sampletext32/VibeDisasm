using VibeDisasm.Web.Models.Types;

namespace VibeDisasm.Web.Models;

public class TypeStorage(UserRuntimeProgram program)
{
    public List<DatabaseType> Types { get; set; } = [];

    public T AddType<T>(T type)
        where T : DatabaseType
    {
        var existingType = Types.FirstOrDefault(x => x == type);
        if (existingType is not null)
        {
            Console.WriteLine($"Type already exists {existingType.DebugDisplay}");
            return existingType as T ?? throw new InvalidOperationException($"Failed to cast {type.DebugDisplay} to {typeof(T).Name}");
        }
        Types.Add(type);
        return type;
    }
}
