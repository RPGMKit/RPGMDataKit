namespace RPGMDataKit.Data.Helpers.Dump;

/// <summary>The styles and scripts of the html files: files of the project (<c>Html/</c>), packed into the dll.</summary>
internal static class HtmlResources
{
    public static string Get(string name)
    {
        using var stream = typeof(HtmlResources).Assembly.GetManifestResourceStream("html-dump/" + name)
                           ?? throw new InvalidOperationException($"The resource html-dump/{name} is missing");
        using var reader = new StreamReader(stream);

        return reader.ReadToEnd();
    }
}
