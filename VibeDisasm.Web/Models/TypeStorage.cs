using VibeDisasm.Web.Models.Types;

namespace VibeDisasm.Web.Models;

public class TypeStorage
{
    public List<DatabaseType> Types { get; set; } = [];

    public T AddType<T>(T type)
        where T : DatabaseType
    {
        Types.Add(type);
        return type;
    }
}
