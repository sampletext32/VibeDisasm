namespace VibeDisasm.Web.Extensions;

public static class ListExtensions
{
    public static T AddReturn<T>(this List<T> list, T value)
    {
        list.Add(value);
        return value;
    }

    public static void AddRange<T>(this HashSet<T> hashset, IEnumerable<T> values)
    {
        foreach (var value in values)
        {
            hashset.Add(value);
        }
    }
}
