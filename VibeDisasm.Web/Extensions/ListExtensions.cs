namespace VibeDisasm.Web.Extensions;

public static class ListExtensions
{
    public static T AddReturn<T>(this List<T> list, T value)
    {
        list.Add(value);
        return value;
    }

    public static void EnqueueRange<T>(this Queue<T> queue, IEnumerable<T> values)
    {
        foreach (var value in values)
        {
            queue.Enqueue(value);
        }
    }
}
