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

    public static Dictionary<int, TV?> MorphToBase<T, TV>(this Dictionary<int, T?> dict)
        where T : class, TV
        where TV : class
    {
        Dictionary<int, TV?> morphed = new();
        foreach (var (i, p) in dict)
            morphed[i] = p;

        return morphed;
    }

    public static void Grow<T>(this IList<T?> list, int newSize, T? defaultValue = default)
    {
        var toAdd = newSize - list.Count;
        while (toAdd-- > 0)
            list.Add(defaultValue);
    }
}
