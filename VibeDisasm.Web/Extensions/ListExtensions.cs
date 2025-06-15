namespace VibeDisasm.Web.Extensions;

public static class ListExtensions
{
    public static T AddReturn<T>(this List<T> list, T value)
    {
        list.Add(value);
        return value;
    }
}
