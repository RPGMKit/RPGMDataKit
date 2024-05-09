using RPGMakerLibCoreV2.Data.Enums;

namespace RPGMakerLibCoreV2.Data.Helpers;

public class FormatOptions
{
    public FormatStyle? Style;
    public int? IdLength;
    public BracketType? BracketType = null;
    public string Literal = "";
    public bool? SkipId;
    public bool? SkipName;
    public string Prefix = "";
    public string Separator = ":";
    public bool SkipTitle = false;
}
