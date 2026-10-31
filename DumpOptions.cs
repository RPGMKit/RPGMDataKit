namespace RPGMDataKit;

public class DumpOptions
{
    public DumpFormat Format = DumpFormat.Text;
    public bool WithIndent = true;
    public bool AppendCommandCode = true;
    public bool ExtendedFormat = false;
    public string LineNumberFormat = "";

    /// <summary>The extension of the files a dump writes.</summary>
    public string FileExtension => Format == DumpFormat.Html ? "html" : "txt";
}