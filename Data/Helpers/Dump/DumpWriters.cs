namespace RPGMDataKit.Data.Helpers.Dump;

/// <summary>The one place that knows which writer a dump format has.</summary>
public static class DumpWriters
{
    public static IDumpWriter For(DumpFormat format) => format switch
    {
        DumpFormat.Text => new TextDumpWriter(),
        DumpFormat.Html => new HtmlDumpWriter(),
        _ => throw new NotSupportedException($"Dump format {format} is not supported for a dump of commands"),
    };
}
