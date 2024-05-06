using RPGMakerLibCoreV2.Data.Structures.Internal;

namespace RPGMakerLibCoreV2;

public static class LibExtensions
{
    public static string UcFirst(this string s)
    {
        if (string.IsNullOrEmpty(s))
            return s;

        if (s.Length == 1)
            return s.ToUpper();

        var a = s.ToCharArray();
        a[0] = char.ToUpper(a[0]);

        return new string(a);
    }

    public static IEnumerable<(int index, T item)> WithIndex<T>(this IEnumerable<T> source)
    {
        return source.Select((item, index) => (index, item));
    }

    public static LookupList<T> ToLookupList<T>(this IEnumerable<T> source, Func<T, int>? indexer = null)
    {
        var l = new LookupList<T>();

        if (indexer != null)
        {
            foreach (var e in source)
                l.Set(indexer(e), e);
        }
        else
            l.AddRange(source);

        return l;
    }
}
