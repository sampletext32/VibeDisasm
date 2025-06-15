namespace VibeDisasm.Web.Models.Types;

/// <summary>
/// Any type, that can be declared or used in the program, e.g. int, word, void*, MyStructure etc.
/// </summary>
public abstract class DatabaseType
{
    public int Size { get; set; }

    public bool ReadOnly { get; set; }

    public DatabaseType(int size)
    {
        Size = size;
        ReadOnly = false;
    }

    protected void MakeReadonly()
    {
        ReadOnly = true;
    }

    public abstract DatabaseType AsReadonly();
}
