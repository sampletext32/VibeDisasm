using VibeDisasm.Web.Models.DatabaseEntries;

namespace VibeDisasm.Web.Models;

public class RuntimeDatabaseEntryManager(RuntimeUserProgram program)
{
    private readonly List<UserProgramDatabaseEntry> _entries = [];

    [Pure]
    public UserProgramDatabaseEntry? GetEntryAt(uint address)
    {
        // if (_entries.Count == 0)
        // {
        //     return null;
        // }
        //
        // if (_entries.Count == 1)
        // {
        //     var entry = _entries[0];
        //     return entry.Address <= address && address < entry.Address + entry.Size ? entry : null;
        // }
        //
        // // binary search for speed
        // int left = 0, right = _entries.Count - 1;
        // while (left <= right)
        // {
        //     int mid = left + ((right - left) >> 1);
        //     var entry = _entries[mid];
        //     if (address < entry.Address)
        //     {
        //         right = mid - 1;
        //     }
        //     else if (address >= entry.Address + entry.Size)
        //     {
        //         left = mid + 1;
        //     }
        //     else
        //     {
        //         return entry;
        //     }
        // }
        return null;
    }

    public void AddEntry(UserProgramDatabaseEntry entry)
    {
        if (GetEntryAt(entry.Address) is not null)
        {
            throw new InvalidOperationException("Adding overlapping entries is not supported yet.");
        }

        _entries.Add(entry);
    }
}
