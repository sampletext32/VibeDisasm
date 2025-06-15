using System.Diagnostics.Contracts;
using VibeDisasm.Web.Models.DatabaseEntries;

namespace VibeDisasm.Web.Models;

public class DatabaseEntryManager
{
    private readonly OrderedDictionary<uint, UserProgramDatabaseEntry> _entries = [];

    public void AddEntry(UserProgramDatabaseEntry entry)
    {
        if (entry is null)
            throw new ArgumentNullException(nameof(entry));

        if (!_entries.TryAdd(entry.Address, entry))
        {
            throw new InvalidOperationException($"Entry at address 0x{entry.Address:X} already exists");
        }
    }

    [Pure]
    public UserProgramDatabaseEntry? GetEntryAt(uint address) =>
        _entries.GetValueOrDefault(address);

    [Pure]
    public T? GetEntryAt<T>(uint address) where T : UserProgramDatabaseEntry =>
        GetEntryAt(address) as T;

    [Pure]
    public IEnumerable<UserProgramDatabaseEntry> GetAllEntries() =>
        _entries.Values;

    [Pure]
    public IEnumerable<T> GetEntriesByType<T>() where T : UserProgramDatabaseEntry =>
        GetAllEntries().OfType<T>();
}
